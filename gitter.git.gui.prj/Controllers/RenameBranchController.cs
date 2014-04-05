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

namespace gitter.Git.Gui.Controllers
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RenameBranchController : ViewControllerBase<IRenameBranchView>, IRenameBranchController
	{
		#region Data

		private readonly Branch _branch;

		#endregion

		#region .ctor

		public RenameBranchController(Branch branch)
		{
			Verify.Argument.IsNotNull(branch, "branch");

			_branch = branch;
		}

		#endregion

		#region Properties

		private Branch Branch
		{
			get { return _branch; }
		}

		#endregion

		#region IRenameBranchController Members

		public bool TryRename()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var repository = Branch.Repository;
			var oldName = Branch.Name;
			var newName = View.NewName.Value.Trim();

			if(oldName == newName) return true;
			if(!GitControllerUtility.ValidateNewBranchName(newName, repository, View.NewName, View.ErrorNotifier))
			{
				return false;
			}

			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					Branch.Name = newName;
				}
			}
			catch(BranchAlreadyExistsException)
			{
				View.ErrorNotifier.NotifyError(View.NewName,
					new UserInputError(
						Resources.ErrInvalidBranchName,
						Resources.ErrBranchAlreadyExists));
				return false;
			}
			catch(InvalidBranchNameException exc)
			{
				View.ErrorNotifier.NotifyError(View.NewName,
					new UserInputError(
						Resources.ErrInvalidBranchName,
						exc.Message));
				return false;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					string.Format(Resources.ErrFailedToRenameBranch, oldName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
