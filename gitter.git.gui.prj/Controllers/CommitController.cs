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
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class CommitController : ViewControllerBase<ICommitView>, ICommitController
	{
		public CommitController(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			Repository = repository;
		}

		private Repository Repository { get; }

		private bool ValidateInput(out string message, out bool amend)
		{
			message = View.Message.Value;
			amend   = View.Amend.Value;

			if(!amend)
			{
				bool hasStagedItems;
				lock(Repository.Status.SyncRoot)
				{
					hasStagedItems = Repository.Status.StagedFiles.Count != 0;
				}
				if(!hasStagedItems)
				{
					View.ErrorNotifier.NotifyError(View.StagedItems,
						new UserInputError(
							Resources.ErrNothingToCommit,
							Resources.ErrNofilesStagedForCommit));
					return false;
				}
			}
			if(string.IsNullOrWhiteSpace(message))
			{
				View.ErrorNotifier.NotifyError(View.Message,
					new UserInputError(
						Resources.ErrEmptyCommitMessage,
						Resources.ErrEnterCommitMessage));
				return false;
			}
			message = message.Trim();
			if(message.Length < GitConstants.MinCommitMessageLength)
			{
				View.ErrorNotifier.NotifyError(View.Message,
					new UserInputError(
						Resources.ErrShortCommitMessage,
						Resources.ErrEnterLongerCommitMessage.UseAsFormat(GitConstants.MinCommitMessageLength)));
				return false;
			}
			return true;
		}

		private void OnCommitFailed(Exception exc)
			=> GitterApplication.MessageBoxService.Show(
				View as IWin32Window,
				exc.Message,
				Resources.ErrFailedToCommit,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);

		public bool TryCommit()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			if(!ValidateInput(out var message, out var amend)) return false;

			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					Repository.Status.Commit(message, amend);
				}
			}
			catch(GitException exc)
			{
				OnCommitFailed(exc);
				return false;
			}
			return true;
		}

		public async Task<bool> TryCommitAsync()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			if(!ValidateInput(out var message, out var amend)) return false;

			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					await Repository.Status.CommitAsync(message, amend);
				}
			}
			catch(GitException exc)
			{
				OnCommitFailed(exc);
				return false;
			}
			return true;
		}
	}
}
