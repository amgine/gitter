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

	sealed class RenameRemoteController : ViewControllerBase<IRenameRemoteView>, IRenameRemoteController
	{
		#region Data

		private readonly Remote _remote;

		#endregion

		#region .ctor

		public RenameRemoteController(Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");

			_remote = remote;
		}

		#endregion

		#region Properties

		private Remote Remote
		{
			get { return _remote; }
		}

		#endregion

		#region IRenameBranchController Members

		public bool TryRename()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var repository = Remote.Repository;
			var oldName    = Remote.Name;
			var newName    = View.NewName.Value.Trim();
			if(oldName == newName) return true;
			if(newName.Length == 0)
			{
				View.ErrorNotifier.NotifyError(View.NewName,
					new UserInputError(
						Resources.ErrNoRemoteNameSpecified,
						Resources.ErrRemoteNameCannotBeEmpty));
				return false;
			}
			if(repository.Remotes.Contains(newName))
			{
				View.ErrorNotifier.NotifyError(View.NewName,
					new UserInputError(
						Resources.ErrInvalidRemoteName,
						Resources.ErrRemoteAlreadyExists));
				return false;
			}
			string errmsg;
			if(!Reference.ValidateName(newName, ReferenceType.Remote, out errmsg))
			{
				View.ErrorNotifier.NotifyError(View.NewName,
					new UserInputError(
						Resources.ErrInvalidRemoteName,
						errmsg));
				return false;
			}
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					Remote.Name = newName;
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					string.Format(Resources.ErrFailedToRenameRemote, oldName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
