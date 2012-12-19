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
