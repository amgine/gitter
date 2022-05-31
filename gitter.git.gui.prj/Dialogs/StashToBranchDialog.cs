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

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class StashToBranchDialog : GitDialogBase, IExecutableDialog, IStashToBranchView
{
	#region Data

	private readonly IStashToBranchController _controller;

	#endregion

	#region .ctor

	public StashToBranchDialog(StashedState stashedState)
	{
		Verify.Argument.IsNotNull(stashedState);
		Verify.Argument.IsFalse(stashedState.IsDeleted, nameof(stashedState),
			Resources.ExcObjectIsDeleted.UseAsFormat(stashedState.GetType().Name));

		StashedState = stashedState;

		InitializeComponent();
		Localize();

		var inputs = new IUserInputSource[]
		{
			BranchName = new TextBoxInputSource(_txtBranchName),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_txtBranchName, ReferenceType.LocalBranch);

		_txtStashName.Text = ((IRevisionPointer)StashedState).Pointer;

		GitterApplication.FontManager.InputFont.Apply(_txtBranchName, _txtStashName);

		_controller = new StashToBranchController(stashedState) { View = this };
	}

	#endregion

	#region Properties

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 53));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCreate;

	public StashedState StashedState { get; }

	public IUserInputSource<string> BranchName { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	#endregion

	#region Methods

	private void Localize()
	{
		Text = Resources.StrStashToBranch;

		_lblBranchName.Text = Resources.StrBranch.AddColon();
		_lblStash.Text = Resources.StrStash.AddColon();
	}

	#endregion

	#region IExecutableDialog Members

	public bool Execute() => _controller.TryCreateBranch();

	#endregion
}
