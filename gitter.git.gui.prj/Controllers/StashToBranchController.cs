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

sealed class StashToBranchController(StashedState stashedState)
	: ViewControllerBase<IStashToBranchView>, IStashToBranchController
{
	public bool TryCreateBranch()
	{
		var view = RequireView();
		var branchName = view.BranchName.Value?.Trim();

		if(!GitControllerUtility.ValidateNewBranchName(branchName, stashedState.Repository, view.BranchName, view.ErrorNotifier))
		{
			return false;
		}

		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				stashedState.ToBranch(branchName!);
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				string.Format(Resources.ErrFailedToCreateBranch, branchName),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}

		return true;
	}
}
