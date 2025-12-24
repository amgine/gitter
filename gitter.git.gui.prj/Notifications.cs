#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui;

using System;

using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class Notifications(GuiProvider guiProvider) : IDisposable
{
	private Repository? _repository;

	public Repository? Repository
	{
		get => _repository;
		set
		{
			if(_repository == value) return;

			if(_repository is not null)
			{
				DetachFromRepository(_repository);
			}
			_repository = value;
			if(_repository is not null)
			{
				AttachToRepository(_repository);
			}
		}
	}

	private PopupNotificationsStack? PopupsStack
		=> guiProvider.Environment?.ViewDockService.DockPanel.PopupsStack;

	private void AttachToRepository(Repository repository)
	{
		Assert.IsNotNull(repository);

		repository.Remotes.FetchCompleted += OnFetchCompleted;
		repository.Remotes.PullCompleted  += OnPullCompleted;
		repository.Remotes.PruneCompleted += OnPruneCompleted;

		repository.Status.Committed += OnCommitted;
	}

	private void DetachFromRepository(Repository repository)
	{
		Assert.IsNotNull(repository);

		repository.Remotes.FetchCompleted -= OnFetchCompleted;
		repository.Remotes.PullCompleted  -= OnPullCompleted;
		repository.Remotes.PruneCompleted -= OnPruneCompleted;

		repository.Status.Committed -= OnCommitted;
	}

	private void OnFetchCompleted(object? sender, FetchCompletedEventArgs e)
	{
		if(guiProvider.Environment is { InvokeRequired: true } environment)
		{
			try
			{
				environment.BeginInvoke(
					new Action<FetchCompletedEventArgs>(OnFetchCompleted), [e]); 
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			OnFetchCompleted(e);
		}
	}

	private void OnPullCompleted(object? sender, PullCompletedEventArgs e)
	{
		if(guiProvider.Environment is { InvokeRequired: true } environment)
		{
			try
			{
				environment.BeginInvoke(
					new Action<PullCompletedEventArgs>(OnPullCompleted), [e]); 
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			OnPullCompleted(e);
		}
	}

	private void OnPruneCompleted(object? sender, PruneCompletedEventArgs e)
	{
		if(guiProvider.Environment is { InvokeRequired: true } environment)
		{
			try
			{
				environment.BeginInvoke(
					new Action<PruneCompletedEventArgs>(OnPruneCompleted), [e]); 
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			OnPruneCompleted(e);
		}
	}

	private void OnCommitted(object? sender, CommitResultEventArgs e)
	{
		var message = e.CommitResult.Message;
		if(string.IsNullOrWhiteSpace(message)) return;

		if(guiProvider.Environment is { InvokeRequired: true } environment)
		{
			try
			{
				environment.BeginInvoke(
					new Action<CommitResultEventArgs>(OnCommitted), [e]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			OnCommitted(e);
		}
	}

	private void OnFetchCompleted(FetchCompletedEventArgs e)
	{
		Assert.IsNotNull(e);

		if(PopupsStack is not { IsDisposed: false } stack) return;

		var notification = new ReferencesChangedNotification(e.Changes)
		{
			Text = e.Remote is null ? Resources.StrFetch : Resources.StrFetch + ": " + e.Remote.Name,
		};
		stack.PushNotification(notification);
	}

	private void OnPullCompleted(PullCompletedEventArgs e)
	{
		Assert.IsNotNull(e);

		if(PopupsStack is not { IsDisposed: false } stack) return;

		var notification = new ReferencesChangedNotification(e.Changes)
		{
			Text = e.Remote is null ? Resources.StrPull : Resources.StrPull + ": " + e.Remote.Name,
		};
		stack.PushNotification(notification);
	}

	private void OnPruneCompleted(PruneCompletedEventArgs e)
	{
		Assert.IsNotNull(e);

		if(PopupsStack is not { IsDisposed: false } stack) return;

		var notification = new ReferencesChangedNotification(e.Changes)
		{
			Text = e.Remote is null ? Resources.StrPrune : Resources.StrPrune + ": " + e.Remote.Name,
		};
		stack.PushNotification(notification);
	}

	private void OnCommitted(CommitResultEventArgs e)
	{
		Assert.IsNotNull(e);

		var message = e.CommitResult.Message;
		if(!string.IsNullOrWhiteSpace(message))
		{
			if(PopupsStack is not { IsDisposed: false } stack) return;

			stack.PushNotification(new PlainTextNotificationContent(message) { Text = Resources.StrCommit });
		}
	}

	public void Dispose()
	{
		if(_repository is not null)
		{
			DetachFromRepository(_repository);
			_repository = null;
		}
	}
}
