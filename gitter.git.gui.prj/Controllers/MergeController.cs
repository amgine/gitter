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

sealed class MergeController(Repository repository)
	: ViewControllerBase<IMergeView>, IMergeController
{
	public bool TryMerge()
	{
		var view          = RequireView();
		var noCommit      = view.NoCommit.Value;
		var noFastForward = view.NoFastForward.Value;
		var squash        = view.Squash.Value;
		var message       = view.Message.Value;

		message = !string.IsNullOrWhiteSpace(message) ? message!.Trim() : null;

		if(view.Revisions.Value is not { Count: not 0 } revisions)
		{
			view.ErrorNotifier.NotifyError(view.Revisions,
				new UserInputError(
					Resources.ErrNoBranchNameSpecified,
					Resources.ErrYouMustSpecifyBranchToMergeWith));
			return false;
		}
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				repository.Head.Merge(revisions, noCommit, noFastForward, squash, message);
			}
		}
		catch(AutomaticMergeFailedException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				Resources.StrMerge,
				MessageBoxButton.Close,
				MessageBoxIcon.Information);
			return true;
		}
		catch(GitException exc)
		{
			var title = revisions.Count == 1 ?
				string.Format(Resources.ErrFailedToMergeWith, revisions[0].Pointer) :
				Resources.ErrFailedToMerge;
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				title,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}
