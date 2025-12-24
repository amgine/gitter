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

sealed class CommitController(Repository repository)
	: ViewControllerBase<ICommitView>, ICommitController
{
	readonly record struct UserInput(
		string Message,
		bool   Amend);

	private bool TryCollectUserInput(out UserInput input)
	{
		var view    = RequireView();
		var message = view.Message.Value;
		var amend   = view.Amend.Value;

		if(!amend)
		{
			bool hasStagedItems;
			lock(repository.Status.SyncRoot)
			{
				hasStagedItems = repository.Status.StagedFiles.Count != 0;
			}
			if(!hasStagedItems)
			{
				view.ErrorNotifier.NotifyError(view.StagedItems,
					new UserInputError(
						Resources.ErrNothingToCommit,
						Resources.ErrNofilesStagedForCommit));
				goto fail;
			}
		}
		if(string.IsNullOrWhiteSpace(message))
		{
			view.ErrorNotifier.NotifyError(view.Message,
				new UserInputError(
					Resources.ErrEmptyCommitMessage,
					Resources.ErrEnterCommitMessage));
			goto fail;
		}
		message = message!.Trim();
		if(message.Length < GitConstants.MinCommitMessageLength)
		{
			view.ErrorNotifier.NotifyError(view.Message,
				new UserInputError(
					Resources.ErrShortCommitMessage,
					Resources.ErrEnterLongerCommitMessage.UseAsFormat(GitConstants.MinCommitMessageLength)));
			goto fail;
		}
		input = new(message, amend);
		return true;

	fail:
		input = default;
		return false;
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
		if(!TryCollectUserInput(out var input)) return false;

		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				repository.Status.Commit(input.Message, input.Amend);
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
		if(!TryCollectUserInput(out var input)) return false;

		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				await repository.Status.CommitAsync(input.Message, input.Amend);
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
