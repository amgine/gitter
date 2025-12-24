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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class StashToBranchDialog : GitDialogBase, IExecutableDialog, IStashToBranchView
{
	readonly struct DialogControls
	{
		private readonly LabelControl _lblStash;
		public  readonly TextBox _txtStashName;
		private readonly LabelControl _lblBranchName;
		public  readonly TextBox _txtBranchName;

		public DialogControls(IGitterStyle? style)
		{
			style ??= GitterApplication.Style;

			_lblStash      = new();
			_txtStashName  = new() { ReadOnly = true };
			_lblBranchName = new();
			_txtBranchName = new();
		}

		public void Localize()
		{
			_lblBranchName.Text = Resources.StrBranch.AddColon();
			_lblStash.Text = Resources.StrStash.AddColon();
		}

		public void Layout(Control parent)
		{
			var decStashName  = new TextBoxDecorator(_txtStashName);
			var decBranchName = new TextBoxDecorator(_txtBranchName);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
					],
					columns:
					[
						SizeSpec.Absolute(60),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblStash,      marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0, column: 0),
						new GridContent(new ControlContent(decStashName,   marginOverride: LayoutConstants.TextBoxMargin),      row: 0, column: 1),
						new GridContent(new ControlContent(_lblBranchName, marginOverride: LayoutConstants.TextBoxLabelMargin), row: 1, column: 0),
						new GridContent(new ControlContent(decBranchName,  marginOverride: LayoutConstants.TextBoxMargin),      row: 1, column: 1),
					]),
			};

			var tabIndex = 0;
			_lblStash.TabIndex      = tabIndex++;
			_txtStashName.TabIndex  = tabIndex++;
			_lblBranchName.TabIndex = tabIndex++;
			_txtBranchName.TabIndex = tabIndex++;

			_lblStash.Parent      = parent;
			decStashName.Parent   = parent;
			_lblBranchName.Parent = parent;
			decBranchName.Parent  = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly IStashToBranchController _controller;

	public StashToBranchDialog(StashedState stashedState)
	{
		Verify.Argument.IsNotNull(stashedState);
		Verify.Argument.IsFalse(stashedState.IsDeleted, nameof(stashedState),
			Resources.ExcObjectIsDeleted.UseAsFormat(stashedState.GetType().Name));

		StashedState = stashedState;

		SuspendLayout();
		AutoScaleMode = AutoScaleMode.Dpi;
		AutoScaleDimensions = Dpi.Default;
		Name = nameof(StashToBranchDialog);
		Text = Resources.StrStashToBranch;
		Size = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			BranchName = new TextBoxInputSource(_controls._txtBranchName),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_controls._txtBranchName, ReferenceType.LocalBranch);

		_controls._txtStashName.Text = ((IRevisionPointer)StashedState).Pointer;

		GitterApplication.FontManager.InputFont.Apply(_controls._txtBranchName, _controls._txtStashName);

		_controller = new StashToBranchController(stashedState) { View = this };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 53));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCreate;

	public StashedState StashedState { get; }

	public IUserInputSource<string?> BranchName { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtBranchName.Focus);
	}

	public bool Execute() => _controller.TryCreateBranch();
}
