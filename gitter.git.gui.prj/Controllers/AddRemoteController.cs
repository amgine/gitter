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

sealed class AddRemoteController : ViewControllerBase<IAddRemoteView>, IAddRemoteController
{
	public AddRemoteController(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;
	}

	private Repository Repository { get; }

	private bool ValidateInput(out string name, out string url)
	{
		name = View.RemoteName.Value.Trim();
		url  = View.Url.Value.Trim();

		if(!GitControllerUtility.ValidateNewRemoteName(name, Repository, View.RemoteName, View.ErrorNotifier))
		{
			return false;
		}
		if(!GitControllerUtility.ValidateUrl(url, View.Url, View.ErrorNotifier))
		{
			return false;
		}

		return true;
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
		Verify.State.IsTrue(View is not null, "Controller is not attached to a view.");

		if(!ValidateInput(out var name, out var url)) return false;

		var fetch        = View.Fetch.Value;
		var mirror       = View.Mirror.Value;
		var tagFetchMode = View.TagFetchMode.Value;

		try
		{
			using(View.ChangeCursor(MouseCursor.WaitCursor))
			{
				Repository.Remotes.Add(name, url, fetch, mirror, tagFetchMode);
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
		Verify.State.IsTrue(View is not null, "Controller is not attached to a view.");

		if(!ValidateInput(out var name, out var url)) return false;

		var fetch        = View.Fetch.Value;
		var mirror       = View.Mirror.Value;
		var tagFetchMode = View.TagFetchMode.Value;

		try
		{
			using(View.ChangeCursor(MouseCursor.WaitCursor))
			{
				await Repository.Remotes.AddAsync(name, url, fetch, mirror, tagFetchMode);
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
