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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;

using gitter.Git.AccessLayer;
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

public partial class ApplyPatchesDialog : GitDialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		public readonly LabelControl _lblPatches;
		public readonly CustomListBox _lstPatches;
		public readonly IButtonWidget _btnAddFiles;
		public readonly IButtonWidget _btnAddFromClipboard;
		public readonly GroupSeparator _grpApplyTo;
		public readonly IRadioButtonWidget _radWorkingDirectory;
		public readonly IRadioButtonWidget _radIndexAndWorkingDirectory;
		public readonly IRadioButtonWidget _radIndexOnly;
		public readonly GroupSeparator _grpOptions;
		public readonly ICheckBoxWidget _chkReverse;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_lblPatches = new();
			_grpApplyTo = new();
			_grpOptions = new();
			var rbf = style.RadioButtonFactory;
			_radWorkingDirectory = rbf.Create();
			_radIndexAndWorkingDirectory = rbf.Create();
			_radIndexOnly = rbf.Create();
			var bf = style.ButtonFactory;
			_btnAddFiles = bf.Create();
			_btnAddFromClipboard = bf.Create();
			_chkReverse = style.CheckBoxFactory.Create();
			_lstPatches = new()
			{
				Style          = style,
				HeaderStyle    = HeaderStyle.Hidden,
				ShowCheckBoxes = true,
				Multiselect    = true,
			};
			_lstPatches.Columns.Add(new NameColumn { SizeMode = ColumnSizeMode.Auto });
			_radWorkingDirectory.IsChecked = true;
		}

		public void Localize()
		{
			_lblPatches.Text = Resources.StrPatches.AddColon();
			_lstPatches.Text = Resources.StrsNoPatchesToApply;
			_btnAddFiles.Text = Resources.StrAddFiles.AddEllipsis();
			_btnAddFromClipboard.Text = Resources.StrAddFromClipboard;
			_grpApplyTo.Text = Resources.StrApplyTo;
			_radWorkingDirectory.Text = Resources.StrsWorkingDirectory;
			_radIndexOnly.Text = Resources.StrIndex;
			_radIndexAndWorkingDirectory.Text = Resources.StrsIndexAndWorkingDirectory;
			_grpOptions.Text = Resources.StrOptions;
			_chkReverse.Text = Resources.StrReverse;
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						/*  0 */ LayoutConstants.LabelRowHeight,
						/*  1 */ LayoutConstants.LabelRowSpacing,
						/*  2 */ SizeSpec.Everything(),
						/*  3 */ LayoutConstants.RowSpacing,
						/*  4 */ LayoutConstants.ButtonRowHeight,
						/*  5 */ LayoutConstants.GroupSeparatorRowHeight,
						/*  6 */ LayoutConstants.RadioButtonRowHeight,
						/*  7 */ LayoutConstants.RadioButtonRowHeight,
						/*  8 */ LayoutConstants.RadioButtonRowHeight,
						/*  9 */ LayoutConstants.GroupSeparatorRowHeight,
						/* 10 */ LayoutConstants.CheckBoxRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lblPatches, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_lstPatches, marginOverride: LayoutConstants.NoMargin), row: 2),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Everything(),
								SizeSpec.Absolute(93),
								SizeSpec.Absolute(6),
								SizeSpec.Absolute(154),
							],
							content:
							[
								new GridContent(new WidgetContent(_btnAddFiles,         marginOverride: LayoutConstants.NoMargin), column: 1),
								new GridContent(new WidgetContent(_btnAddFromClipboard, marginOverride: LayoutConstants.NoMargin), column: 3),
							]), row: 4),
						new GridContent(new ControlContent(_grpApplyTo,                  marginOverride: LayoutConstants.NoMargin), row: 5),
						new GridContent(new WidgetContent (_radWorkingDirectory,         marginOverride: LayoutConstants.GroupPadding), row: 6),
						new GridContent(new WidgetContent (_radIndexOnly,                marginOverride: LayoutConstants.GroupPadding), row: 7),
						new GridContent(new WidgetContent (_radIndexAndWorkingDirectory, marginOverride: LayoutConstants.GroupPadding), row: 8),
						new GridContent(new ControlContent(_grpOptions,                  marginOverride: LayoutConstants.NoMargin), row: 9),
						new GridContent(new WidgetContent (_chkReverse,                  marginOverride: LayoutConstants.GroupPadding), row: 10),
					]),
			};

			var tabIndex = 0;
			_lblPatches.TabIndex = tabIndex++;
			_lstPatches.TabIndex = tabIndex++;
			_btnAddFiles.TabIndex = tabIndex++;
			_btnAddFromClipboard.TabIndex = tabIndex++;
			_grpApplyTo.TabIndex = tabIndex++;
			_radWorkingDirectory.TabIndex = tabIndex++;
			_radIndexOnly.TabIndex = tabIndex++;
			_radIndexAndWorkingDirectory.TabIndex = tabIndex++;
			_grpOptions.TabIndex = tabIndex++;
			_chkReverse.TabIndex = tabIndex++;

			_lblPatches.Parent = parent;
			_lstPatches.Parent = parent;
			_btnAddFiles.Parent = parent;
			_btnAddFromClipboard.Parent = parent;
			_grpApplyTo.Parent = parent;
			_radWorkingDirectory.Parent = parent;
			_radIndexOnly.Parent = parent;
			_radIndexAndWorkingDirectory.Parent = parent;
			_grpOptions.Parent = parent;
			_chkReverse.Parent = parent;
		}
	}

	private readonly DialogControls _controls;

	public ApplyPatchesDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		Name = nameof(ApplyPatchesDialog);
		Text = Resources.StrApplyPatches;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(false);
		PerformLayout();

		_controls._lstPatches.KeyDown += OnPatchesKeyDown;
		_controls._btnAddFiles.Click += OnAddFilesClick;
		_controls._btnAddFromClipboard.Click += OnAddFromClipboardClick;
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 325));

	public Repository Repository { get; }

	public ApplyPatchTo ApplyTo
	{
		get
		{
			if(_controls._radWorkingDirectory.IsChecked)         return ApplyPatchTo.WorkingDirectory;
			if(_controls._radIndexOnly.IsChecked)                return ApplyPatchTo.Index;
			if(_controls._radIndexAndWorkingDirectory.IsChecked) return ApplyPatchTo.IndexAndWorkingDirectory;
			return ApplyPatchTo.WorkingDirectory;
		}
		set
		{
			var button = value switch
			{
				ApplyPatchTo.WorkingDirectory         => _controls._radWorkingDirectory,
				ApplyPatchTo.Index                    => _controls._radIndexOnly,
				ApplyPatchTo.IndexAndWorkingDirectory => _controls._radIndexAndWorkingDirectory,
				_ => throw new ArgumentException($"Unknown mode: {value}", nameof(value)),
			};
			button.IsChecked = true;
		}
	}

	public bool Reverse
	{
		get => _controls._chkReverse.IsChecked;
		set => _controls._chkReverse.IsChecked = value;
	}

	protected override string ActionVerb => Resources.StrApply;

	public IEnumerable<IPatchSource> SelectedPatchSources
	{
		get
		{
			foreach(PatchSourceListItem item in _controls._lstPatches.Items)
			{
				if(item.IsChecked)
				{
					yield return item.DataContext;
				}
			}
		}
	}

	private void AddPatchSource(IPatchSource patchSource)
	{
		Assert.IsNotNull(patchSource);

		var item = new PatchSourceListItem(patchSource);
		item.CheckedState = CheckedState.Checked;
		_controls._lstPatches.Items.Add(item);
		item.FocusAndSelect();
	}

	private void OnPatchesKeyDown(object? sender, KeyEventArgs e)
	{
		switch(e.KeyCode)
		{
			case Keys.Delete:
				while(_controls._lstPatches.SelectedItems.Count != 0)
				{
					_controls._lstPatches.SelectedItems[0].Remove();
				}
				break;
		}
	}

	private void OnAddFilesClick(object? sender, EventArgs e)
	{
		using var dialog = new OpenFileDialog()
		{
			Filter = "Patches (*.patch)|*.patch|All files|*.*",
			Multiselect = true,
		};
		if(dialog.ShowDialog(this) == DialogResult.OK)
		{
			foreach(var fileName in dialog.FileNames)
			{
				var src = new PatchFromFile(fileName);
				AddPatchSource(src);
			}
		}
	}

	private void OnAddFromClipboardClick(object? sender, EventArgs e)
	{
		var patch = default(string);
		try
		{
			patch = Clipboard.GetText();
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
		if(!string.IsNullOrWhiteSpace(patch))
		{
			var src = new PatchFromString("Patch from clipboard", patch!);
			AddPatchSource(src);
		}
	}

	public bool Execute()
	{
		var applyTo = ApplyTo;
		var reverse = Reverse;
		var sources = SelectedPatchSources;
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				Repository.Status.ApplyPatches(sources, applyTo, reverse);
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToApplyPatch,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}
