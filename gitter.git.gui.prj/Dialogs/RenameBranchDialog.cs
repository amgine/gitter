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

/// <summary>Dialog for renaming local branch.</summary>
[ToolboxItem(false)]
public partial class RenameBranchDialog : GitDialogBase, IExecutableDialog, IRenameBranchView
{
	readonly struct DialogControls
	{
		public readonly TextBox _txtOldName;
		public readonly TextBox _txtNewName;
		public readonly LabelControl _lblOldName;
		public readonly LabelControl _lblNewName;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_txtOldName = new() { ReadOnly = true };
			_txtNewName = new();
			_lblOldName = new();
			_lblNewName = new();

			GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);
		}

		public void Localize()
		{
			_lblOldName.Text = Resources.StrBranch.AddColon();
			_lblNewName.Text = Resources.StrNewName.AddColon();
		}

		public void Layout(Control parent)
		{
			var oldNameDec = new TextBoxDecorator(_txtOldName);
			var newNameDec = new TextBoxDecorator(_txtNewName);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblOldName, marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
						new GridContent(new ControlContent(oldNameDec, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
						new GridContent(new ControlContent(_lblNewName, marginOverride: LayoutConstants.NoMargin), column: 0, row: 1),
						new GridContent(new ControlContent(newNameDec, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
					]),
			};

			var tabIndex = 0;
			_lblOldName.TabIndex = tabIndex++;
			oldNameDec.TabIndex = tabIndex++;
			_lblNewName.TabIndex = tabIndex++;
			newNameDec.TabIndex = tabIndex++;

			_lblOldName.Parent = parent;
			oldNameDec.Parent = parent;
			_lblNewName.Parent = parent;
			newNameDec.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly IRenameBranchController _controller;

	/// <summary>Create <see cref="RenameBranchDialog"/>.</summary>
	/// <param name="branch">Branch to rename.</param>
	/// <exception cref="ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
	public RenameBranchDialog(Branch branch)
	{
		Verify.Argument.IsNotNull(branch);
		Verify.Argument.IsFalse(branch.IsDeleted, nameof(branch),
			Resources.ExcObjectIsDeleted.UseAsFormat(nameof(Branch)));

		Branch = branch;

		Name = nameof(RenameBranchDialog);
		Text = Resources.StrRenameBranch;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			NewName = new TextBoxInputSource(_controls._txtNewName),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_controls._txtNewName, ReferenceType.LocalBranch);

		var branchName = branch.Name;
		_controls._txtOldName.Text = branchName;
		_controls._txtNewName.Text = branchName;
		_controls._txtNewName.SelectAll();

		_controller = new RenameBranchController(branch) { View = this };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 53));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrRename;

	/// <summary>Branch to rename.</summary>
	public Branch Branch { get; }

	public IUserInputSource<string?> NewName { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtNewName.Focus);
	}

	/// <summary>Perform rename.</summary>
	/// <returns>true if rename succeeded.</returns>
	public bool Execute() => _controller.TryRename();
}
