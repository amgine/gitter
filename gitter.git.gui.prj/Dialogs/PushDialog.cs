#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class PushDialog : GitDialogBase, IPushView, IExecutableDialog
{
	private sealed class BranchesInputSource(ReferencesListBox referencesListBox)
		: IUserInputSource<Many<Branch>>, IWin32ControlInputSource
	{
		public Many<Branch> Value
		{
			get
			{
				var list = new List<Branch>(capacity: referencesListBox.Items.Count);
				foreach(var item in referencesListBox.Items)
				{
					if(item.CheckedState == CheckedState.Checked)
					{
						if(item is IRevisionPointerListItem { RevisionPointer: Branch branch })
						{
							list.Add(branch);
						}
					}
				}
				return list;
			}
			set
			{
				if(value.Count == 0)
				{
					foreach(var item in referencesListBox.Items)
					{
						item.CheckedState = CheckedState.Unchecked;
					}
					return;
				}
				foreach(var item in referencesListBox.Items)
				{
					if(item is IRevisionPointerListItem { RevisionPointer: Branch branch })
					{
						item.CheckedState = value.Contains(branch) ?
							CheckedState.Checked : CheckedState.Unchecked;
					}
				}
			}
		}

		public bool IsReadOnly
		{
			get => !referencesListBox.Enabled;
			set =>  referencesListBox.Enabled = !value;
		}

		public Control Control => referencesListBox;
	}

	readonly struct DialogControls
	{
		public readonly ReferencesListBox _lstReferences;
		public readonly LabelControl _lblBranches;
		public readonly ICheckBoxWidget _chkSendTags;
		public readonly ICheckBoxWidget _chkUseThinPack;
		public readonly ICheckBoxWidget _chkForceOverwriteBranches;
		public readonly Panel _pnlWarning;
		public readonly Label _lblUseWithCaution;
		public readonly PictureBox _picWarning;
		public readonly GroupSeparator _grpOptions;
		public readonly RemotePicker _remotePicker;
		public readonly GroupSeparator _grpPushTo;
		public readonly TextBox _txtUrl;
		public readonly IRadioButtonWidget _radUrl;
		public readonly IRadioButtonWidget _radRemote;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			var cbf = style.CheckBoxFactory;
			var rbf = style.RadioButtonFactory;
			_pnlWarning = new();
			_lblUseWithCaution = new();
			_picWarning = new();
			_grpOptions = new();
			_chkUseThinPack = cbf.Create();
			_chkForceOverwriteBranches = cbf.Create();
			_chkSendTags = cbf.Create();
			_lblBranches = new();
			_remotePicker = new();
			_lstReferences = new() { Style = style };
			_grpPushTo = new();
			_txtUrl = new();
			_radUrl = rbf.Create();
			_radRemote = rbf.Create();

			_pnlWarning.BorderStyle = BorderStyle.FixedSingle;
			_pnlWarning.Visible     = false;

			if(style.Type == GitterStyleType.DarkBackground)
			{
				_pnlWarning.BackColor = Color.FromArgb(67, 53, 25);
				_pnlWarning.ForeColor = Color.White;
			}
			else
			{
				_pnlWarning.BackColor = SystemColors.Info;
				_pnlWarning.ForeColor = SystemColors.InfoText;
			}

			_lblUseWithCaution.TextAlign = ContentAlignment.MiddleLeft;

			_lstReferences.HeaderStyle = HeaderStyle.Hidden;
			_lstReferences.DisableContextMenus = true;
			_lstReferences.ShowTreeLines = true;

			_radRemote.IsChecked = true;
			_chkUseThinPack.IsChecked = true;
		}

		public void Localize()
		{
			_lblBranches.Text = Resources.StrBranchesToPush.AddColon();
			_grpPushTo.Text = Resources.StrPushTo;
			_radRemote.Text = Resources.StrRemote;
			_radUrl.Text = Resources.StrUrl;
			_grpOptions.Text = Resources.StrOptions;
			_chkForceOverwriteBranches.Text = Resources.StrForceOverwriteRemoteBranches;
			_lblUseWithCaution.Text = Resources.StrUseWithCaution;
			_chkUseThinPack.Text = Resources.StrUseThinPack;
			_chkSendTags.Text = Resources.StrSendTags;
		}

		public void Layout(Control parent)
		{
			var panelPushTo = new Panel();
			var urlDec = new TextBoxDecorator(_txtUrl);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						/* 0 */ LayoutConstants.LabelRowHeight,
						/* 1 */ LayoutConstants.LabelRowSpacing,
						/* 2 */ SizeSpec.Everything(),
						/* 3 */ LayoutConstants.GroupSeparatorRowHeight,
						/* 4 */ LayoutConstants.TextInputRowHeight,
						/* 5 */ LayoutConstants.TextInputRowHeight,
						/* 6 */ LayoutConstants.GroupSeparatorRowHeight,
						/* 7 */ LayoutConstants.CheckBoxRowHeight,
						/* 8 */ LayoutConstants.CheckBoxRowHeight,
						/* 9 */ LayoutConstants.CheckBoxRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lblBranches,   marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_lstReferences, marginOverride: LayoutConstants.NoMargin), row: 2),
						new GridContent(new ControlContent(_grpPushTo,     marginOverride: LayoutConstants.NoMargin), row: 3),
						new GridContent(new ControlContent(panelPushTo,    marginOverride: LayoutConstants.GroupPadding), row: 4, rowSpan: 2),
						new GridContent(new ControlContent(_grpOptions,    marginOverride: LayoutConstants.NoMargin), row: 6),
						new GridContent(new Grid(
							padding: LayoutConstants.GroupPadding,
							columns:
							[
								SizeSpec.Absolute(216),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new WidgetContent (_chkForceOverwriteBranches, marginOverride: LayoutConstants.NoMargin)),
								new GridContent(new ControlContent(_pnlWarning,                marginOverride: LayoutConstants.NoMargin), column: 1),
							]), row: 7),
						new GridContent(new WidgetContent (_chkUseThinPack, marginOverride: LayoutConstants.GroupPadding), row: 8),
						new GridContent(new WidgetContent (_chkSendTags,    marginOverride: LayoutConstants.GroupPadding), row: 9),
					]),
			};

			_ = new ControlLayout(panelPushTo)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					rows:
					[
						/* 0 */ LayoutConstants.TextInputRowHeight,
						/* 1 */ LayoutConstants.TextInputRowHeight,
					],
					content:
					[
						new GridContent(new WidgetContent (_radRemote,    marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_remotePicker, marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new WidgetContent (_radUrl,       marginOverride: LayoutConstants.NoMargin), row: 1),
						new GridContent(new ControlContent(urlDec,        marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
					]),
			};

			_ = new ControlLayout(_pnlWarning)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(16),
						SizeSpec.Absolute(2),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_picWarning,        marginOverride: LayoutConstants.NoMargin, sizeOverride: DpiBoundValue.Size(new(16, 16)), verticalContentAlignment: VerticalContentAlignment.Center), column: 0),
						new GridContent(new ControlContent(_lblUseWithCaution, marginOverride: LayoutConstants.NoMargin), column: 2),
					]),
			};

			var tabIndex = 0;
			_lblBranches.TabIndex = tabIndex++;
			_lstReferences.TabIndex = tabIndex++;
			_grpPushTo.TabIndex = tabIndex++;
			_radRemote.TabIndex = tabIndex++;
			_remotePicker.TabIndex = tabIndex++;
			_radUrl.TabIndex = tabIndex++;
			urlDec.TabIndex = tabIndex++;
			panelPushTo.TabIndex = tabIndex++;
			_grpOptions.TabIndex = tabIndex++;
			_chkForceOverwriteBranches.TabIndex = tabIndex++;
			_picWarning.TabIndex = tabIndex++;
			_lblUseWithCaution.TabIndex = tabIndex++;
			_pnlWarning.TabIndex = tabIndex++;
			_chkUseThinPack.TabIndex = tabIndex++;
			_chkSendTags.TabIndex = tabIndex++; 

			_lblBranches.Parent = parent;
			_lstReferences.Parent = parent;
			_grpPushTo.Parent = parent;
			_radRemote.Parent = panelPushTo;
			_remotePicker.Parent = panelPushTo;
			_radUrl.Parent = panelPushTo;
			urlDec.Parent = panelPushTo;
			panelPushTo.Parent = parent;
			_grpOptions.Parent = parent;
			_chkForceOverwriteBranches.Parent = parent;
			_picWarning.Parent = _pnlWarning;
			_lblUseWithCaution.Parent = _pnlWarning;
			_pnlWarning.Parent = parent;
			_chkUseThinPack.Parent = parent;
			_chkSendTags.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly IPushController _controller;

	public PushDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		Name = nameof(PushDialog);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Layout(this);
		Localize();
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			PushTo = new RadioButtonWidgetGroupInputSource<PushTo>(
				[
					Tuple.Create(_controls._radRemote, gitter.Git.Gui.Interfaces.PushTo.Remote),
					Tuple.Create(_controls._radUrl,    gitter.Git.Gui.Interfaces.PushTo.Url),
				]),
			Remote         = PickerInputSource.Create(_controls._remotePicker),
			Url            = new TextBoxInputSource(_controls._txtUrl),
			References     = new BranchesInputSource(_controls._lstReferences),
			ForceOverwrite = new CheckBoxWidgetInputSource(_controls._chkForceOverwriteBranches),
			ThinPack       = new CheckBoxWidgetInputSource(_controls._chkUseThinPack),
			SendTags       = new CheckBoxWidgetInputSource(_controls._chkSendTags),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		var dpiBindings = new DpiBindings(this);
		dpiBindings.BindImage(_controls._picWarning, Icons.PushWarning);

		_controls._lstReferences.LoadData(Repository, ReferenceType.LocalBranch, false, false, null);
		_controls._lstReferences.EnableCheckboxes();

		if(!Repository.Head.IsDetached)
		{
			foreach(BranchListItem item in _controls._lstReferences.Items)
			{
				if(item.DataContext == Repository.Head.Pointer)
				{
					item.CheckedState = CheckedState.Checked;
					break;
				}
			}
		}

		_controls._remotePicker.LoadData(repository);
		_controls._remotePicker.SelectedValue = PickDefaultRemote(repository);

		_controls._chkForceOverwriteBranches.IsCheckedChanged += OnForceOverwriteCheckedChanged;
		_controls._txtUrl.TextChanged                         += OnUrlTextChanged;
		_controls._remotePicker.SelectedValueChanged          += OnRemotePickerSelectedValueChanged;
		_controls._lstReferences.ItemActivated                += static (_, e) => e.Item.IsChecked = !e.Item.IsChecked;

		_controller = new PushController(repository) { View = this };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 379));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrPush;

	public Repository Repository { get; }

	public IUserInputSource<PushTo> PushTo { get; }

	public IUserInputSource<Remote?> Remote { get; }

	public IUserInputSource<string?> Url { get; }

	public IUserInputSource<Many<Branch>> References { get; }

	public IUserInputSource<bool> ForceOverwrite { get; }

	public IUserInputSource<bool> ThinPack { get; }

	public IUserInputSource<bool> SendTags { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._lstReferences.Focus);
	}

	private static Remote? PickDefaultRemote(Repository repository)
	{
		Assert.IsNotNull(repository);

		var remotes = repository.Remotes;
		lock(remotes.SyncRoot)
		{
			if(remotes.Count == 0) return default;

			if(remotes.TryGetItem(GitConstants.DefaultRemoteName, out var origin))
			{
				return origin;
			}
			foreach(var remote in remotes)
			{
				return remote;
			}
		}
		return default;
	}

	private void Localize()
	{
		Text = Resources.StrPush;
		_controls.Localize();
		ToolTipService.Register(_controls._chkForceOverwriteBranches.Control, Resources.TipPushForceOverwrite);
		ToolTipService.Register(_controls._chkUseThinPack.Control, Resources.TipUseTinPack);
		ToolTipService.Register(_controls._chkSendTags.Control, Resources.TipSendTags);
	}

	private void OnForceOverwriteCheckedChanged(object? sender, EventArgs e)
	{
		_controls._pnlWarning.Visible = _controls._chkForceOverwriteBranches.IsChecked;
	}

	private void OnUrlTextChanged(object? sender, EventArgs e)
	{
		if(_controls._txtUrl.TextLength != 0)
		{
			_controls._radUrl.IsChecked = true;
		}
	}

	private void OnRemotePickerSelectedValueChanged(object? sender, EventArgs e)
	{
		if(_controls._remotePicker.SelectedValue is not null)
		{
			_controls._radRemote.IsChecked = true;
		}
	}

	public bool Execute() => _controller.TryPush();
}
