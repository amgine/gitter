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

	sealed class MergeController : ViewControllerBase<IMergeView>, IMergeController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public MergeController(Repository repository)
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

		#region IMergeController Members

		public bool TryMerge()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var noCommit      = View.NoCommit.Value;
			var noFastForward = View.NoFastForward.Value;
			var squash        = View.Squash.Value;
			var message       = View.Message.Value;

			if(!string.IsNullOrWhiteSpace(message))
			{
				message = message.Trim();
			}
			else
			{
				message = null;
			}

			var revisions = View.Revisions.Value;
			if(revisions == null || revisions.Count == 0)
			{
				View.ErrorNotifier.NotifyError(View.Revisions,
					new UserInputError(
						Resources.ErrNoBranchNameSpecified,
						Resources.ErrYouMustSpecifyBranchToMergeWith));
				return false;
			}
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					Repository.Head.Merge(revisions, noCommit, noFastForward, squash, message);
				}
			}
			catch(AutomaticMergeFailedException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
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
					View as IWin32Window,
					exc.Message,
					title,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
