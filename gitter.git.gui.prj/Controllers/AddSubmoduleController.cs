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

sealed class AddSubmoduleController(Repository repository)
	: ViewControllerBase<IAddSubmoduleView>, IAddSubmoduleController
{
	readonly record struct UserInput(
		string  Path,
		string  Url,
		string? Branch);

	private bool TryCollectUserInput(out UserInput input)
	{
		var view   = RequireView();
		var path   = view.Path.Value?.Trim();
		var url    = view.Url.Value?.Trim();
		var branch = default(string);

		if(!GitControllerUtility.ValidateRelativePath(path, view.Path, view.ErrorNotifier))
		{
			goto fail;
		}
		if(!GitControllerUtility.ValidateUrl(url, view.Url, view.ErrorNotifier))
		{
			goto fail;
		}
		if(view.UseCustomBranch.Value)
		{
			branch = view.BranchName.Value?.Trim();
			if(!GitControllerUtility.ValidateBranchName(branch, view.BranchName, view.ErrorNotifier))
			{
				goto fail;
			}
		}
		input = new(path!, url!, branch);
		return true;

	fail:
		input = default;
		return false;
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
		if(!TryCollectUserInput(out var input)) return false;

		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				repository.Submodules.Add(input.Path, input.Url, input.Branch);
			}
		}
		catch(GitException exc)
		{
			OnCreateFailed(exc, input.Path);
			return false;
		}
		return true;
	}

	public async Task<bool> TryAddSubmoduleAsync()
	{
		if(!TryCollectUserInput(out var input)) return false;

		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				await repository.Submodules
					.AddAsync(input.Path, input.Url, input.Branch)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch(GitException exc)
		{
			OnCreateFailed(exc, input.Path);
			return false;
		}
		return true;
	}
}
