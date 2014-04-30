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

	using gitter.Framework.Mvc;
	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class PushController : ViewControllerBase<IPushView>, IPushController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public PushController(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		#endregion

		#region properties

		private Repository Repository
		{
			get { return _repository; }
		}

		#endregion

		#region IPushController Members

		public bool TryPush()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var forceOverwrite = View.ForceOverwrite.Value;
			var thinPack       = View.ThinPack.Value;
			var sendTags       = View.SendTags.Value;
			var branches       = View.References.Value;

			if(branches.Count == 0)
			{
				View.ErrorNotifier.NotifyError(View.References,
					new UserInputError(
					Resources.ErrNoBranchesSelected,
					Resources.ErrYouMustSelectBranchesToPush));
				return false;
			}

			GuiCommandStatus status;
			switch(View.PushTo.Value)
			{
				case PushTo.Remote:
					var remote = View.Remote.Value;
					if(remote == null)
					{
						View.ErrorNotifier.NotifyError(View.Remote,
							new UserInputError(
								Resources.ErrInvalidRemoteName,
								Resources.ErrRemoteNameCannotBeEmpty));
						return false;
					}
					status = GuiCommands.Push(View as IWin32Window, remote, branches, forceOverwrite, thinPack, sendTags);
					break;
				case PushTo.Url:
					var url = View.Url.Value;
					if(string.IsNullOrWhiteSpace(url))
					{
						View.ErrorNotifier.NotifyError(View.Url,
							new UserInputError(
								Resources.ErrInvalidUrl,
								Resources.ErrUrlCannotBeEmpty));
						return false;
					}
					status = GuiCommands.Push(View as IWin32Window, Repository, url, branches, forceOverwrite, thinPack, sendTags);
					break;
				default:
					return false;
			}
			return status == GuiCommandStatus.Completed;
		}

		#endregion
	}
}
