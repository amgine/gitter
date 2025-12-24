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
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class CleanDialog : DialogBase, IAsyncExecutableDialog
{
	readonly struct DialogControls
	{
		public  readonly TextBox _txtPattern;
		public  readonly LabelControl _lblIncludePattern;
		public  readonly TreeListBox _lstPreview;
		public  readonly IRadioButtonWidget _radIncludeUntracked;
		public  readonly IRadioButtonWidget _radIncludeIgnored;
		public  readonly IRadioButtonWidget _radIncludeBoth;
		public  readonly LabelControl _lblExcludePattern;
		public  readonly TextBox _txtExclude;
		public  readonly LabelControl _lblType;
		public  readonly ICheckBoxWidget _chkRemoveDirectories;
		private readonly LabelControl _lblPreview;

		private static TreeListBox CreatePreviewList(IGitterStyle style)
		{
			var list = new TreeListBox
			{
				Style         = style,
				ShowTreeLines = false,
				HeaderStyle   = HeaderStyle.Hidden,
			};
			for(int i = 0; i < list.Columns.Count; ++i)
			{
				var col = list.Columns[i];
				if(col.Id == (int)ColumnId.Name)
				{
					col.IsVisible = true;
					col.SizeMode = ColumnSizeMode.Auto;
				}
				else
				{
					col.IsVisible = false;
				}
			}
			return list;
		}

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_txtPattern           = new();
			_lblIncludePattern    = new();
			_radIncludeUntracked  = style.RadioButtonFactory.Create();
			_radIncludeIgnored    = style.RadioButtonFactory.Create();
			_radIncludeBoth       = style.RadioButtonFactory.Create();
			_lblExcludePattern    = new();
			_txtExclude           = new();
			_lblType              = new();
			_chkRemoveDirectories = style.CheckBoxFactory.Create();
			_lblPreview           = new();
			_lstPreview           = CreatePreviewList(style);

			GitterApplication.FontManager.InputFont.Apply(_txtPattern, _txtExclude);
		}

		public void Localize()
		{
			_lblIncludePattern.Text = Resources.StrsIncludePattern.AddColon();
			_lblExcludePattern.Text = Resources.StrsExcludePattern.AddColon();

			_lblType.Text = Resources.StrType.AddColon();

			_radIncludeUntracked.Text = Resources.StrUntracked;
			_radIncludeIgnored.Text = Resources.StrIgnored;
			_radIncludeBoth.Text = Resources.StrBoth;

			_chkRemoveDirectories.Text = Resources.StrsAlsoRemoveDirectories;

			_lblPreview.Text = Resources.StrsObjectsThatWillBeRemoved.AddColon();
			_lstPreview.Text = Resources.StrsNoFilesToRemove;
		}

		public void Layout(Control parent)
		{
			var includeDec = new TextBoxDecorator(_txtPattern);
			var excludeDec = new TextBoxDecorator(_txtExclude);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(120),
						SizeSpec.Everything(),
					],
					rows:
					[
						/* 0 */ LayoutConstants.TextInputRowHeight,
						/* 1 */ LayoutConstants.TextInputRowHeight,
						/* 2 */ LayoutConstants.RadioButtonRowHeight,
						/* 3 */ LayoutConstants.CheckBoxRowHeight,
						/* 4 */ LayoutConstants.LabelRowSpacing,
						/* 5 */ LayoutConstants.LabelRowHeight,
						/* 6 */ LayoutConstants.LabelRowSpacing,
						/* 7 */ SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblIncludePattern, marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
						new GridContent(new ControlContent(includeDec,         marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
						new GridContent(new ControlContent(_lblExcludePattern, marginOverride: LayoutConstants.NoMargin), column: 0, row: 1),
						new GridContent(new ControlContent(excludeDec,         marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
						new GridContent(new ControlContent(_lblType,           marginOverride: LayoutConstants.NoMargin), column: 0, row: 2),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(100),
								SizeSpec.Absolute(100),
								SizeSpec.Absolute(100),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new WidgetContent(_radIncludeUntracked, marginOverride: LayoutConstants.TextBoxMargin), column: 0),
								new GridContent(new WidgetContent(_radIncludeIgnored,   marginOverride: LayoutConstants.TextBoxMargin), column: 1),
								new GridContent(new WidgetContent(_radIncludeBoth,      marginOverride: LayoutConstants.TextBoxMargin), column: 2),
							]), column: 1, row: 2),
						new GridContent(new WidgetContent(_chkRemoveDirectories, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 3),
						new GridContent(new ControlContent(_lblPreview, marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 5),
						new GridContent(new ControlContent(_lstPreview, marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 7),
					]),
			};

			var tabIndex = 0;
			_lblIncludePattern.TabIndex = tabIndex++;
			includeDec.TabIndex = tabIndex++;
			_lblExcludePattern.TabIndex = tabIndex++;
			excludeDec.TabIndex = tabIndex++;
			_lblType.TabIndex = tabIndex++;
			_radIncludeUntracked.TabIndex = tabIndex++;
			_radIncludeIgnored.TabIndex = tabIndex++;
			_radIncludeBoth.TabIndex = tabIndex++;
			_chkRemoveDirectories.TabIndex = tabIndex++;
			_lblPreview.TabIndex = tabIndex++;
			_lstPreview.TabIndex = tabIndex++;

			includeDec.Parent = parent;
			_lblIncludePattern.Parent = parent;
			_radIncludeUntracked.Parent = parent;
			_radIncludeIgnored.Parent = parent;
			_radIncludeBoth.Parent = parent;
			_lblExcludePattern.Parent = parent;
			excludeDec.Parent = parent;
			_lblType.Parent = parent;
			_chkRemoveDirectories.Parent = parent;
			_lstPreview.Parent = parent;
			_lblPreview.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private FilesToCleanBinding? _dataBinding;

	/// <summary>Create <see cref="CleanDialog"/>.</summary>
	/// <param name="repository">Related <see cref="Repository"/>.</param>
	public CleanDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Name = nameof(CleanDialog);

		Repository = repository;
		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Size = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Layout(this);
		Localize();
		ResumeLayout(false);
		PerformLayout();

		if(!GitFeatures.CleanExcludeOption.IsAvailableFor(repository))
		{
			_controls._lblExcludePattern.Enabled = false;
			_controls._txtExclude.Enabled = false;
			_controls._txtExclude.Text = Resources.StrlRequiredVersionIs.UseAsFormat(
				GitFeatures.CleanExcludeOption.RequiredVersion);
		}

		LoadConfig();

		_controls._txtPattern.TextChanged += OnPatternTextChanged;
		_controls._radIncludeUntracked.IsCheckedChanged += OnRadioButtonCheckedChanged;
		_controls._radIncludeIgnored.IsCheckedChanged += OnRadioButtonCheckedChanged;
		_controls._radIncludeBoth.IsCheckedChanged += OnRadioButtonCheckedChanged;
		_controls._txtExclude.TextChanged += OnPatternTextChanged;
		_controls._chkRemoveDirectories.IsCheckedChanged += OnRemoveDirectoriesCheckedChanged;
		_controls._lstPreview.ItemActivated += OnFilesToClearItemActivated;
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DataBinding = null;
		}
		base.Dispose(disposing);
	}

	private void Localize()
	{
		Text = Resources.StrClean;
		_controls.Localize();
	}

	private FilesToCleanBinding? DataBinding
	{
		get => _dataBinding;
		set
		{
			if(_dataBinding == value) return;

			_dataBinding?.Dispose();
			_dataBinding = value;
			_dataBinding?.ReloadData();
		}
	}

	public Repository Repository { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtPattern.Focus);
	}

	/// <inheritdoc/>
	protected override void OnClosed(DialogResult result)
	{
		SaveConfig();
		base.OnClosed(result);
	}

	private void LoadConfig()
	{
		var section = Repository.ConfigSection.TryGetSection("CleanDialog");
		if(section is not null)
		{
			_controls._txtPattern.Text = section.GetValue<string>("Pattern", string.Empty);
			_controls._txtExclude.Text = section.GetValue<string>("Exclude", string.Empty);
			RemoveDirectories = section.GetValue<bool>("RemoveDirectories", false);
			Mode = section.GetValue<CleanFilesMode>("Mode", CleanFilesMode.Default);
		}
		else
		{
			_controls._txtPattern.Text = string.Empty;
			_controls._txtExclude.Text = string.Empty;
			RemoveDirectories = false;
			Mode = CleanFilesMode.Default;
		}
	}

	private void SaveConfig()
	{
		var section = Repository.ConfigSection.GetCreateSection("CleanDialog");
		section.SetValue<string>("Pattern", _controls._txtPattern.Text);
		section.SetValue<string>("Exclude", _controls._txtExclude.Text);
		section.SetValue<bool>("RemoveDirectories", RemoveDirectories);
		section.SetValue<CleanFilesMode>("Mode", Mode);
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(437, 359));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrClean;

	/// <inheritdoc/>
	protected override void OnShown()
	{
		base.OnShown();
		UpdateList();
	}

	public CleanFilesMode Mode
	{
		get
		{
			if(_controls._radIncludeUntracked.IsChecked)
				return CleanFilesMode.Default;
			if(_controls._radIncludeIgnored.IsChecked)
				return CleanFilesMode.OnlyIgnored;
			if(_controls._radIncludeBoth.IsChecked)
				return CleanFilesMode.IncludeIgnored;
			return CleanFilesMode.Default;
		}
		set
		{
			(value switch
			{
				CleanFilesMode.Default        => _controls._radIncludeUntracked,
				CleanFilesMode.OnlyIgnored    => _controls._radIncludeIgnored,
				CleanFilesMode.IncludeIgnored => _controls._radIncludeBoth,
				_ => throw new ArgumentException($"Unknown mode: {value}", nameof(value)),
			}).IsChecked = true;
		}
	}

	static string? GetPattern(TextBox input)
	{
		if(!input.Enabled) return default;
		var text = input.Text;
		if(string.IsNullOrWhiteSpace(text)) return default;
		return text.Trim();
	}

	public string? IncludePattern
	{
		get => GetPattern(_controls._txtPattern);
		set => _controls._txtPattern.Text = value;
	}

	public string? ExcludePattern
	{
		get => GetPattern(_controls._txtExclude);
		set
		{
			Verify.State.IsTrue(_controls._txtExclude.Enabled, "Exclude pattern is not supported.");

			_controls._txtExclude.Text = value;
		}
	}

	public bool RemoveDirectories
	{
		get => _controls._chkRemoveDirectories.IsChecked;
		set => _controls._chkRemoveDirectories.IsChecked = value;
	}

	private void UpdateList()
	{
		if(IsDisposed) return;

		_dataBinding ??= new FilesToCleanBinding(Repository, _controls._lstPreview);
		_dataBinding.IncludePattern = IncludePattern;
		_dataBinding.ExcludePattern = ExcludePattern;
		_dataBinding.CleanFilesMode = Mode;
		_dataBinding.IncludeDirectories = RemoveDirectories;
		_dataBinding.ReloadData();
	}

	private void OnPatternTextChanged(object? sender, EventArgs e)
	{
		UpdateList();
	}

	private void OnRadioButtonCheckedChanged(object? sender, EventArgs e)
	{
		if(sender is not IRadioButtonWidget { IsChecked: true }) return;
		UpdateList();
	}

	private void OnRemoveDirectoriesCheckedChanged(object? sender, EventArgs e)
	{
		UpdateList();
	}

	private void OnFilesToClearItemActivated(object? sender, ItemEventArgs e)
	{
		if(e.Item is ITreeItemListItem item)
		{
			Utility.OpenUrl(System.IO.Path.Combine(
				item.TreeItem.Repository.WorkingDirectory, item.TreeItem.RelativePath));
		}
	}

	/// <summary>Execute dialog associated action.</summary>
	/// <returns><c>true</c>, if action succeeded</returns>
	public async Task<bool> ExecuteAsync()
	{
		try
		{
			await Repository.Status.CleanAsync(IncludePattern, ExcludePattern, Mode, RemoveDirectories);
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrCleanFailed,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			UpdateList();
			return false;
		}
		return true;
	}
}
