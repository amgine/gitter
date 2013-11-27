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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class StashToBranchDialog : GitDialogBase, IExecutableDialog
	{
		private readonly StashedState _stashedState;

		public StashToBranchDialog(StashedState stashedState)
		{
			Verify.Argument.IsNotNull(stashedState, "stashedState");
			Verify.Argument.IsFalse(stashedState.IsDeleted, "stashedState",
				Resources.ExcObjectIsDeleted.UseAsFormat(stashedState.GetType().Name));

			_stashedState = stashedState;

			InitializeComponent();

			SetupReferenceNameInputBox(_txtBranchName, ReferenceType.LocalBranch);

			Text = Resources.StrStashToBranch;

			_lblBranchName.Text = Resources.StrBranch.AddColon();
			_lblStash.Text = Resources.StrStash.AddColon();

			_txtStashName.Text = ((IRevisionPointer)_stashedState).Pointer;

			GitterApplication.FontManager.InputFont.Apply(_txtBranchName, _txtStashName);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrCreate; }
		}

		public StashedState StashedState
		{
			get { return _stashedState; }
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			var branchName = _txtBranchName.Text.Trim();

			if(!ValidateNewBranchName(branchName, _txtBranchName, _stashedState.Repository))
			{
				return false;
			}

			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					_stashedState.ToBranch(branchName);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToCreateBranch, branchName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}

			return true;
		}

		#endregion
	}
}
