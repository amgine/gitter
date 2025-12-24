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

sealed class AddRemoteController(Repository repository)
	: ViewControllerBase<IAddRemoteView>, IAddRemoteController
{
	readonly record struct UserInput(
		string       Name,
		string       Url,
		bool         Fetch,
		bool         Mirror,
		TagFetchMode TagFetchMode);

	private bool TryCollectUserInput(out UserInput input)
	{
		var view = RequireView();
		var name = view.RemoteName.Value?.Trim();
		var url  = view.Url.Value?.Trim();

		if(!GitControllerUtility.ValidateNewRemoteName(name, repository, view.RemoteName, view.ErrorNotifier))
		{
			goto fail;
		}
		if(!GitControllerUtility.ValidateUrl(url, view.Url, view.ErrorNotifier))
		{
			goto fail;
		}

		input = new(
			Name:         name!,
			Url:          url!,
			Fetch:        view.Fetch.Value,
			Mirror:       view.Mirror.Value,
			TagFetchMode: view.TagFetchMode.Value);
		return true;

	fail:
		input = default;
		return false;
	}

	private void OnAddFailed(Exception exc)
		=> GitterApplication.MessageBoxService.Show(
			View as IWin32Window,
			exc.Message,
			Resources.ErrFailedToAddRemote,
			MessageBoxButton.Close,
			MessageBoxIcon.Error);

	public bool TryAddRemote()
	{
		if(!TryCollectUserInput(out var input)) return false;

		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				repository.Remotes.Add(input.Name, input.Url, input.Fetch, input.Mirror, input.TagFetchMode);
			}
		}
		catch(GitException exc)
		{
			OnAddFailed(exc);
			return false;
		}
		return true;
	}

	public async Task<bool> TryAddRemoteAsync()
	{
		if(!TryCollectUserInput(out var input)) return false;

		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				await repository.Remotes.AddAsync(input.Name, input.Url, input.Fetch, input.Mirror, input.TagFetchMode);
			}
		}
		catch(GitException exc)
		{
			OnAddFailed(exc);
			return false;
		}
		return true;
	}
}
