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

	/// <summary>Dialog for renaming local branch.</summary>
	[ToolboxItem(false)]
	public partial class RenameBranchDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Branch _branch;

		/// <summary>Create <see cref="RenameBranchDialog"/>.</summary>
		/// <param name="branch">Branch to rename.</param>
		/// <exception cref="ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
		public RenameBranchDialog(Branch branch)
		{
			Verify.Argument.IsNotNull(branch, "branch");
			Verify.Argument.IsFalse(branch.IsDeleted, "branch",
				Resources.ExcObjectIsDeleted.UseAsFormat("Branch"));

			_branch = branch;

			InitializeComponent();

			SetupReferenceNameInputBox(_txtNewName, ReferenceType.LocalBranch);

			Text = Resources.StrRenameBranch;

			_lblOldName.Text = Resources.StrBranch.AddColon();
			_lblNewName.Text = Resources.StrNewName.AddColon();

			_txtOldName.Text = branch.Name;
			_txtNewName.Text = branch.Name;
			_txtNewName.SelectAll();

			GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);
		}

		/// <summary>Verb, describing operation.</summary>
		protected override string ActionVerb
		{
			get { return Resources.StrRename; }
		}

		/// <summary>Branch to rename.</summary>
		public Branch Branch
		{
			get { return _branch; }
		}

		/// <summary>New branche's name.</summary>
		public string NewName
		{
			get { return _txtNewName.Text; }
			set { _txtNewName.Text = value; }
		}

		/// <summary>Perform rename.</summary>
		/// <returns>true if rename succeeded.</returns>
		public bool Execute()
		{
			var repository = _branch.Repository;
			var oldName = _txtOldName.Text;
			var newName = _txtNewName.Text.Trim();

			if(oldName == newName) return true;
			if(!ValidateNewBranchName(newName, _txtNewName, repository))
			{
				return false;
			}

			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					_branch.Name = newName;
				}
			}
			catch(BranchAlreadyExistsException)
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidBranchName,
					Resources.ErrBranchAlreadyExists);
				return false;
			}
			catch(InvalidBranchNameException exc)
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidBranchName,
					exc.Message);
				return false;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToRenameBranch, oldName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}
