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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;

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

		private readonly StashedState _stashedState;
		private readonly IUserInputSource<string> _branchNameInput;
		private readonly IUserInputErrorNotifier _errorNotifier;
		private readonly IStashToBranchController _controller;

		#endregion

		#region .ctor

		public StashToBranchDialog(StashedState stashedState)
		{
			Verify.Argument.IsNotNull(stashedState, "stashedState");
			Verify.Argument.IsFalse(stashedState.IsDeleted, "stashedState",
				Resources.ExcObjectIsDeleted.UseAsFormat(stashedState.GetType().Name));

			_stashedState = stashedState;

			InitializeComponent();
			Localize();

			var inputs = new IUserInputSource[]
			{
				_branchNameInput = new TextBoxInputSource(_txtBranchName),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

			SetupReferenceNameInputBox(_txtBranchName, ReferenceType.LocalBranch);

			_txtStashName.Text = ((IRevisionPointer)_stashedState).Pointer;

			GitterApplication.FontManager.InputFont.Apply(_txtBranchName, _txtStashName);

			_controller = new StashToBranchController(stashedState) { View = this };
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrCreate; }
		}

		public StashedState StashedState
		{
			get { return _stashedState; }
		}

		public IUserInputSource<string> BranchName
		{
			get { return _branchNameInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

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

		public bool Execute()
		{
			return _controller.TryCreateBranch();
		}

		#endregion
	}
}
