﻿#region Copyright Notice
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

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for renaming local branch.</summary>
[ToolboxItem(false)]
public partial class RenameBranchDialog : GitDialogBase, IExecutableDialog, IRenameBranchView
{
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

		InitializeComponent();
		Localize();

		var inputs = new IUserInputSource[]
		{
			NewName = new TextBoxInputSource(_txtNewName),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_txtNewName, ReferenceType.LocalBranch);

		var branchName = branch.Name;
		_txtOldName.Text = branchName;
		_txtNewName.Text = branchName;
		_txtNewName.SelectAll();

		GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);

		_controller = new RenameBranchController(branch) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 53));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrRename;

	/// <summary>Branch to rename.</summary>
	public Branch Branch { get; }

	public IUserInputSource<string> NewName { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_txtNewName.Focus);
	}

	private void Localize()
	{
		Text = Resources.StrRenameBranch;

		_lblOldName.Text = Resources.StrBranch.AddColon();
		_lblNewName.Text = Resources.StrNewName.AddColon();
	}

	/// <summary>Perform rename.</summary>
	/// <returns>true if rename succeeded.</returns>
	public bool Execute() => _controller.TryRename();
}
