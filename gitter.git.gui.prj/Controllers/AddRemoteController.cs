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

	sealed class AddRemoteController : ViewControllerBase<IAddRemoteView>, IAddRemoteController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public AddRemoteController(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		#endregion

		#region Properties

		private Repository Repository
		{
			get { return _repository; }
		}

		#endregion

		#region IAddRemoteController Members

		public bool TryAddRemote()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var name = View.RemoteName.Value;
			if(!GitControllerUtility.ValidateNewRemoteName(name, Repository, View.RemoteName, View.ErrorNotifier))
			{
				return false;
			}
			var url = View.Url.Value;
			if(!GitControllerUtility.ValidateUrl(url, View.Url, View.ErrorNotifier))
			{
				return false;
			}
			name = name.Trim();
			url  = url.Trim();
			var fetch        = View.Fetch.Value;
			var mirror       = View.Mirror.Value;
			var tagFetchMode = View.TagFetchMode.Value;
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					Repository.Remotes.AddRemote(name, url, fetch, mirror, tagFetchMode);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					Resources.ErrFailedToAddRemote,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
