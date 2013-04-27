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

namespace gitter.Git.Gui
{
	using System;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class Notifications : IDisposable
	{
		private readonly GuiProvider _guiProvider;
		private Repository _repository;

		public Notifications(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, "guiProvider");

			_guiProvider = guiProvider;
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					if(_repository != null)
					{
						DetachFromRepository(_repository);
					}
					_repository = value;
					if(_repository != null)
					{
						AttachToRepository(_repository);
					}
				}
			}
		}

		private PopupNotificationsStack PopupsStack
		{
			get { return _guiProvider.Environment.ViewDockService.Grid.PopupsStack; }
		}

		private void AttachToRepository(Repository repository)
		{
			repository.Remotes.FetchCompleted	+= OnFetchCompleted;
			repository.Remotes.PullCompleted	+= OnPullCompleted;
			repository.Remotes.PruneCompleted	+= OnPruneCompleted;
		}

		private void DetachFromRepository(Repository repository)
		{
			repository.Remotes.FetchCompleted	-= OnFetchCompleted;
			repository.Remotes.PullCompleted	-= OnPullCompleted;
			repository.Remotes.PruneCompleted	-= OnPruneCompleted;
		}

		private void OnFetchCompleted(object sender, FetchCompletedEventArgs e)
		{
			if(_guiProvider.Environment.InvokeRequired)
			{
				_guiProvider.Environment.BeginInvoke(
					new Action<FetchCompletedEventArgs>(OnFetchCompleted), new object[] { e }); 
			}
			else
			{
				OnFetchCompleted(e);
			}
		}

		private void OnPullCompleted(object sender, PullCompletedEventArgs e)
		{
			if(_guiProvider.Environment.InvokeRequired)
			{
				_guiProvider.Environment.BeginInvoke(
					new Action<PullCompletedEventArgs>(OnPullCompleted), new object[] { e }); 
			}
			else
			{
				OnPullCompleted(e);
			}
		}

		private void OnPruneCompleted(object sender, PruneCompletedEventArgs e)
		{
			if(_guiProvider.Environment.InvokeRequired)
			{
				_guiProvider.Environment.BeginInvoke(
					new Action<PruneCompletedEventArgs>(OnPruneCompleted), new object[] { e }); 
			}
			else
			{
				OnPruneCompleted(e);
			}
		}

		private void OnFetchCompleted(FetchCompletedEventArgs e)
		{
			var notification = new ReferencesChangedNotification(e.Changes)
			{
				Text = e.Remote == null ? Resources.StrFetch : Resources.StrFetch + ": " + e.Remote.Name,
			};
			PopupsStack.PushNotification(notification);
		}

		private void OnPullCompleted(PullCompletedEventArgs e)
		{
			var notification = new ReferencesChangedNotification(e.Changes)
			{
				Text = e.Remote == null ? Resources.StrPull : Resources.StrPull + ": " + e.Remote.Name,
			};
			PopupsStack.PushNotification(notification);
		}

		private void OnPruneCompleted(PruneCompletedEventArgs e)
		{
			var notification = new ReferencesChangedNotification(e.Changes)
			{
				Text = e.Remote == null ? Resources.StrPrune : Resources.StrPrune + ": " + e.Remote.Name,
			};
			PopupsStack.PushNotification(notification);
		}

		public void Dispose()
		{
			if(_repository != null)
			{
				DetachFromRepository(_repository);
				_repository = null;
			}
		}
	}
}
