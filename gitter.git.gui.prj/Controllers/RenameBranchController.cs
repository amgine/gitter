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

namespace gitter.Git.Gui.Controllers;

using System;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Services;

using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class RenameBranchController(Branch branch)
	: ViewControllerBase<IRenameBranchView>, IRenameBranchController
{
	public bool TryRename()
	{
		var view = RequireView();
		var repository = branch.Repository;
		var oldName = branch.Name;
		var newName = view.NewName.Value?.Trim();

		if(oldName == newName) return true;
		if(!GitControllerUtility.ValidateNewBranchName(newName, repository, view.NewName, view.ErrorNotifier))
		{
			return false;
		}

		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				branch.Name = newName!;
			}
		}
		catch(BranchAlreadyExistsException)
		{
			view.ErrorNotifier.NotifyError(view.NewName,
				new UserInputError(
					Resources.ErrInvalidBranchName,
					Resources.ErrBranchAlreadyExists));
			return false;
		}
		catch(InvalidBranchNameException exc)
		{
			view.ErrorNotifier.NotifyError(view.NewName,
				new UserInputError(
					Resources.ErrInvalidBranchName,
					exc.Message));
			return false;
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				string.Format(Resources.ErrFailedToRenameBranch, oldName),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}
