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

using gitter.Framework.Mvc;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class PushController(Repository repository)
	: ViewControllerBase<IPushView>, IPushController
{
	public bool TryPush()
	{
		var view           = RequireView();
		var forceOverwrite = view.ForceOverwrite.Value;
		var thinPack       = view.ThinPack.Value;
		var sendTags       = view.SendTags.Value;
		var branches       = view.References.Value;

		if(branches.IsEmpty)
		{
			view.ErrorNotifier.NotifyError(view.References,
				new UserInputError(
				Resources.ErrNoBranchesSelected,
				Resources.ErrYouMustSelectBranchesToPush));
			return false;
		}

		GuiCommandStatus status;
		switch(view.PushTo.Value)
		{
			case PushTo.Remote:
				var remote = view.Remote.Value;
				if(remote is null)
				{
					view.ErrorNotifier.NotifyError(view.Remote,
						new UserInputError(
							Resources.ErrInvalidRemoteName,
							Resources.ErrRemoteNameCannotBeEmpty));
					return false;
				}
				status = GuiCommands.Push(view as IWin32Window, remote, branches, forceOverwrite, thinPack, sendTags);
				break;
			case PushTo.Url:
				var url = view.Url.Value;
				if(string.IsNullOrWhiteSpace(url))
				{
					view.ErrorNotifier.NotifyError(view.Url,
						new UserInputError(
							Resources.ErrInvalidUrl,
							Resources.ErrUrlCannotBeEmpty));
					return false;
				}
				status = GuiCommands.Push(view as IWin32Window, repository, url!, branches, forceOverwrite, thinPack, sendTags);
				break;
			default:
				return false;
		}
		return status == GuiCommandStatus.Completed;
	}
}
