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
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Services;

using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class AddSubmoduleController : ViewControllerBase<IAddSubmoduleView>, IAddSubmoduleController
{
	public AddSubmoduleController(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;
	}

	private Repository Repository { get; }

	private bool ValidateInput(out string path, out string url, out string branch)
	{
		path   = View.Path.Value.Trim();
		url    = View.Url.Value.Trim();
		branch = default;

		if(!GitControllerUtility.ValidateRelativePath(path, View.Path, View.ErrorNotifier))
		{
			return false;
		}
		if(!GitControllerUtility.ValidateUrl(url, View.Url, View.ErrorNotifier))
		{
			return false;
		}
		if(View.UseCustomBranch.Value)
		{
			branch = View.BranchName.Value.Trim();
			if(!GitControllerUtility.ValidateBranchName(branch, View.BranchName, View.ErrorNotifier))
			{
				return false;
			}
		}
		return true;
	}

	private void OnCreateFailed(Exception exc, string path)
		=> GitterApplication.MessageBoxService.Show(
			View as IWin32Window,
			exc.Message,
			string.Format(Resources.ErrFailedToAddSubmodule, path),
			MessageBoxButton.Close,
			MessageBoxIcon.Error);

	public bool TryAddSubmodule()
	{
		Verify.State.IsTrue(View is not null, "Controller is not attached to a view.");

		if(!ValidateInput(out var path, out var url, out var branch)) return false;

		try
		{
			using(View.ChangeCursor(MouseCursor.WaitCursor))
			{
				Repository.Submodules.Add(path, url, branch);
			}
		}
		catch(GitException exc)
		{
			OnCreateFailed(exc, path);
			return false;
		}
		return true;
	}

	public async Task<bool> TryAddSubmoduleAsync()
	{
		Verify.State.IsTrue(View is not null, "Controller is not attached to a view.");

		if(!ValidateInput(out var path, out var url, out var branch)) return false;

		try
		{
			using(View.ChangeCursor(MouseCursor.WaitCursor))
			{
				await Repository.Submodules
					.AddAsync(path, url, branch)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch(GitException exc)
		{
			OnCreateFailed(exc, path);
			return false;
		}
		return true;
	}
}
