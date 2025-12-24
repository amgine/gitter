#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class StageDialog : DialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		public readonly TextBox         _txtPattern;
		public readonly LabelControl    _lblPattern;
		public readonly TreeListBox     _lstUnstaged;
		public readonly ICheckBoxWidget _chkIncludeUntracked;
		public readonly ICheckBoxWidget _chkIncludeIgnored;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_txtPattern = new();
			_lblPattern = new();
			_lstUnstaged = new()
			{
				Style = style,
				HeaderStyle = HeaderStyle.Hidden,
				ShowTreeLines = false,
			};
			_chkIncludeUntracked = style.CheckBoxFactory.Create();
			_chkIncludeIgnored   = style.CheckBoxFactory.Create();

			for(int i = 0; i < _lstUnstaged.Columns.Count; ++i)
			{
				var col = _lstUnstaged.Columns[i];
				if(col.Id == (int)ColumnId.Name)
				{
					col.IsVisible = true;
					col.SizeMode  = ColumnSizeMode.Auto;
				}
				else
				{
					col.IsVisible = false;
				}
			}

			GitterApplication.FontManager.InputFont.Apply(_txtPattern);
		}

		public void Localize()
		{
			_lblPattern.Text          = Resources.StrPattern.AddColon();
			_chkIncludeUntracked.Text = Resources.StrIncludeUntracked;
			_chkIncludeIgnored.Text   = Resources.StrIncludeIgnored;
			_lstUnstaged.Text         = Resources.StrsNoUnstagedChanges;
		}

		public void Layout(Control parent)
		{
			var patternDec = new TextBoxDecorator(_txtPattern);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.CheckBoxRowHeight,
						LayoutConstants.RowSpacing,
						SizeSpec.Everything(),
					],
					columns:
					[
						SizeSpec.Absolute(64),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblPattern, marginOverride: LayoutConstants.NoMargin),      column: 0, row: 0),
						new GridContent(new ControlContent(patternDec,  marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(130),
								SizeSpec.Absolute(130),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new WidgetContent(_chkIncludeUntracked, marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new WidgetContent(_chkIncludeIgnored,   marginOverride: LayoutConstants.NoMargin), column: 1),
							]), row: 1, column: 1),
						new GridContent(new ControlContent(_lstUnstaged, marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 3),
					]),
			};

			var tabIndex = 0;
			_lblPattern.TabIndex = tabIndex++;
			patternDec.TabIndex = tabIndex++;
			_chkIncludeUntracked.TabIndex = tabIndex++;
			_chkIncludeIgnored.TabIndex = tabIndex++;
			_lstUnstaged.TabIndex = tabIndex++;

			_lblPattern.Parent = parent;
			patternDec.Parent = parent;
			_chkIncludeUntracked.Parent = parent;
			_chkIncludeIgnored.Parent = parent;
			_lstUnstaged.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private FilesToAddBinding? _dataBinding;

	/// <summary>Create <see cref="StageDialog"/>.</summary>
	/// <param name="repository">Related <see cref="Repository"/>.</param>
	public StageDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		Name = nameof(StageDialog);
		Text = Resources.StrStageFiles;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._txtPattern.TextChanged               += OnPatternTextChanged;
		_controls._lstUnstaged.ItemActivated            += OnFilesItemActivated;
		_controls._chkIncludeUntracked.IsCheckedChanged += OnIncludeUntrackedCheckedChanged;
		_controls._chkIncludeIgnored.IsCheckedChanged   += OnIncludeIgnoredCheckedChanged;
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DataBinding = null;
		}
		base.Dispose(disposing);
	}

	#region Properties

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 325));

	public Repository Repository { get; }

	private FilesToAddBinding? DataBinding
	{
		get => _dataBinding;
		set
		{
			if(_dataBinding == value) return;

			DisposableUtility.Dispose(ref _dataBinding);
			_dataBinding = value;
		}
	}

	protected override string ActionVerb => Resources.StrStage;

	public string Pattern
	{
		get => _controls._txtPattern.Text.Trim();
		set => _controls._txtPattern.Text = value;
	}

	public bool IncludeUntracked
	{
		get => _controls._chkIncludeUntracked.IsChecked;
		set => _controls._chkIncludeUntracked.IsChecked = value;
	}

	public bool IncludeIgnored
	{
		get => _controls._chkIncludeIgnored.IsChecked;
		set => _controls._chkIncludeIgnored.IsChecked = value;
	}

	#endregion

	#region Methods

	protected override void OnShown()
	{
		base.OnShown();
		UpdateList();
	}

	private void UpdateList()
	{
		DataBinding ??= new FilesToAddBinding(Repository, _controls._lstUnstaged);
		DataBinding.Pattern          = Pattern;
		DataBinding.IncludeUntracked = IncludeUntracked;
		DataBinding.IncludeIgnored   = IncludeIgnored;
		DataBinding.ReloadData();
	}

	private void OnPatternTextChanged(object? sender, EventArgs e)
	{
		UpdateList();
	}

	private void OnIncludeUntrackedCheckedChanged(object? sender, EventArgs e)
	{
		UpdateList();
	}

	private void OnIncludeIgnoredCheckedChanged(object? sender, EventArgs e)
	{
		UpdateList();
	}

	private void OnFilesItemActivated(object? sender, ItemEventArgs e)
	{
		if(e.Item is ITreeItemListItem { TreeItem.Status: not FileStatus.Removed } item)
		{
			if(item is not null) Utility.OpenUrl(System.IO.Path.Combine(
				item.TreeItem.Repository.WorkingDirectory, item.TreeItem.RelativePath));
		}
	}

	public bool Execute()
	{
		try
		{
			if(_controls._lstUnstaged.Items.Count == 0) return true;
			var pattern       = _controls._txtPattern.Text.Trim();
			bool addIgnored   = _controls._chkIncludeIgnored.IsChecked;
			bool addUntracked = _controls._chkIncludeUntracked.IsChecked;
			Repository.Status.Stage(pattern, addUntracked, addIgnored);
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToStage,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		return true;
	}

	#endregion
}
