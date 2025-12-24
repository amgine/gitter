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
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Services;

using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class InitController(IGitRepositoryProvider gitRepositoryProvider)
	: ViewControllerBase<IInitView>, IInitController
{
	readonly record struct UserInput(
		string  Path,
		string? Template,
		bool    Bare);

	private static bool TryCollectUserInput(IInitView view, out UserInput input)
	{
		var path     = view.RepositoryPath.Value?.Trim();
		var template = default(string);
		var bare     = view.Bare.Value;
		if(!GitControllerUtility.ValidateAbsolutePath(path, view.RepositoryPath, view.ErrorNotifier))
		{
			goto fail;
		}
		if(view.UseCustomTemplate.Value)
		{
			template = view.Template.Value?.Trim();
			if(!GitControllerUtility.ValidateAbsolutePath(template, view.Template, view.ErrorNotifier))
			{
				goto fail;
			}
		}
		input = new(path!, template, bare);
		return true;

	fail:
		input = default;
		return false;
	}

	private void OnCreateDirectoryFailed(Exception exc)
		=> GitterApplication.MessageBoxService.Show(
			View as IWin32Window,
			exc.Message,
			Resources.ErrFailedToCreateDirectory,
			MessageBoxButton.Close,
			MessageBoxIcon.Error);

	private void OnInitFailed(Exception exc)
		=> GitterApplication.MessageBoxService.Show(
			View as IWin32Window,
			exc.Message,
			Resources.ErrFailedToInit,
			MessageBoxButton.Close,
			MessageBoxIcon.Error);

	private bool EnsureDirectoryExists(string path)
	{
		try
		{
			if(!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			OnCreateDirectoryFailed(exc);
			return false;
		}
		return true;
	}

	public bool TryInit()
	{
		var view = RequireView();

		var accessor = gitRepositoryProvider.GitAccessor
			?? throw new InvalidOperationException("Git is not configured.");

		if(!TryCollectUserInput(view, out var input)) return false;
		if(!EnsureDirectoryExists(input.Path)) return false;

		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				Repository.Init(accessor, input.Path, input.Template, input.Bare);
			}
		}
		catch(GitException exc)
		{
			OnInitFailed(exc);
			return false;
		}
		return true;
	}

	public async Task<bool> TryInitAsync()
	{
		var view = RequireView();

		var accessor = gitRepositoryProvider.GitAccessor
			?? throw new InvalidOperationException("Git is not configured.");

		if(!TryCollectUserInput(view, out var input)) return false;
		if(!EnsureDirectoryExists(input.Path)) return false;

		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				await Repository
					.InitAsync(accessor, input.Path, input.Template, input.Bare)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch(GitException exc)
		{
			OnInitFailed(exc);
			return false;
		}
		return true;
	}
}
