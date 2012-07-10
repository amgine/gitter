namespace gitter.Git
{
	using System;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.AccessLayer;

	interface IRepositoryChangedNotification
	{
		object NotificationType { get; }

		bool Apply(Repository repository);
	}

	abstract class RepositoryChangedNotification : IRepositoryChangedNotification
	{
		protected RepositoryChangedNotification()
		{
		}

		public abstract object NotificationType { get; }

		public abstract bool Apply(Repository repository);
	}

	sealed class CheckoutNotification : RepositoryChangedNotification
	{
		public CheckoutNotification()
		{
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.Checkout; }
		}

		public override bool Apply(Repository repository)
		{
			var orig = repository.Head;
			repository.Head.Refresh();
			return (orig != repository.Head) ;
		}
	}

	sealed class SubmodulesChangedNotification : RepositoryChangedNotification
	{
		public SubmodulesChangedNotification()
		{
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.SubmodulesChanged; }
		}

		public override bool Apply(Repository repository)
		{
			repository.Submodules.Refresh();
			return true;
		}
	}

	sealed class RepositoryRemovedNotification : RepositoryChangedNotification
	{
		public RepositoryRemovedNotification()
		{
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.RepositoryRemoved; }
		}

		public override bool Apply(Repository repository)
		{
			/*
			RepositoryProvider.Environment.BeginInvoke(
				new Action(()=>
				{
					GitterApplication.MessageBoxService.Show(
						RepositoryProvider.Environment.MainForm,
						"Repository was removed externally and will be closed.",
						repository.WorkingDirectory,
						MessageBoxButton.Close,
						System.Windows.Forms.MessageBoxIcon.Warning);
					RepositoryProvider.Environment.CloseRepository();
				}), null);
			*/
			return true;
		}
	}

	sealed class ConfigUpdatedNotification : RepositoryChangedNotification
	{
		public ConfigUpdatedNotification()
		{
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.ConfigUpdated; }
		}

		public override bool Apply(Repository repository)
		{
			repository.Configuration.Refresh();
			return true;
		}
	}

	sealed class IndexUpdatedNotification : RepositoryChangedNotification
	{
		public IndexUpdatedNotification()
		{
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.IndexUpdated; }
		}

		public override bool Apply(Repository repository)
		{
			repository.Status.Refresh();
			return true;
		}
	}

	sealed class WorktreeUpdatedNotification : RepositoryChangedNotification
	{
		private readonly string _path;

		public WorktreeUpdatedNotification(string path)
		{
			_path = path;
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.WorktreeUpdated; }
		}

		public override bool Apply(Repository repository)
		{
			repository.Status.Refresh();
			return true;
		}
	}

	/// <summary>Branch created/deleted/reset.</summary>
	sealed class BranchChangedNotification : RepositoryChangedNotification
	{
		private readonly string _name;
		private readonly bool _remote;

		public BranchChangedNotification(string name, bool remote)
		{
			_name = name;
			_remote = remote;
		}

		public string Name
		{
			get { return _name; }
		}

		public bool IsRemote
		{
			get { return _remote; }
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.BranchChanged; }
		}

		public override bool Apply(Repository repository)
		{
			var branchInformation = repository.Accessor.QueryBranch(
				new QueryBranchParameters(_name, _remote));
			if(branchInformation == null)
			{
				if(_remote)
				{
					var refs = repository.Refs.Remotes;
					lock(refs.SyncRoot)
					{
						if(refs.Contains(_name))
							refs.NotifyRemoved(_name);
						else
							return false;
					}
				}
				else
				{
					var refs = repository.Refs.Heads;
					lock(refs.SyncRoot)
					{
						if(refs.Contains(_name))
							refs.NotifyRemoved(_name);
						else
							return false;
					}
				}
			}
			else
			{
				if(_remote)
				{
					var refs = repository.Refs.Remotes;
					lock(refs.SyncRoot)
					{
						var branch = refs.TryGetItem(_name);
						if(branch == null)
						{
							refs.NotifyCreated(branchInformation);
						}
						else
						{
							if(branch.Revision.Name != branchInformation.SHA1)
								branch.NotifyReset(branchInformation);
							else
								return false;
						}
					}
				}
				else
				{
					var refs = repository.Refs.Heads;
					lock(refs.SyncRoot)
					{
						var branch = refs.TryGetItem(_name);
						if(branch == null)
						{
							refs.NotifyCreated(branchInformation);
						}
						else
						{
							if(branch.Revision.Name != branchInformation.SHA1)
								branch.NotifyReset(branchInformation);
							else
								return false;
						}
					}
				}
			}
			return true;
		}
	}

	/// <summary>Tag created/deleted.</summary>
	sealed class TagChangedNotification : RepositoryChangedNotification
	{
		private readonly string _name;

		public TagChangedNotification(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.TagChanged; }
		}

		public override bool Apply(Repository repository)
		{
			var tagInformation = repository.Accessor.QueryTag(
				new QueryTagParameters(_name));
			if(tagInformation == null)
			{
				var refs = repository.Refs.Tags;
				lock(refs.SyncRoot)
				{
					if(refs.Contains(_name))
					{
						refs.NotifyRemoved(_name);
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				var refs = repository.Refs.Tags;
				lock(refs.SyncRoot)
				{
					var tag = refs.TryGetItem(_name);
					if(tag == null)
					{
						refs.NotifyCreated(tagInformation);
					}
					else
					{
						if(tag.Revision.Name != tagInformation.SHA1)
						{
							refs.NotifyRemoved(_name);
							refs.NotifyCreated(tagInformation);
						}
						else
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}

	/// <summary>Stash removed/updated.</summary>
	sealed class StashChangedNotification : RepositoryChangedNotification
	{
		public StashChangedNotification()
		{
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.StashChanged; }
		}

		public override bool Apply(Repository repository)
		{
			var top = repository.Accessor.QueryStashTop(
				new QueryStashTopParameters(false));
			if(top == null)
			{
				repository.Stash.NotifyCleared();
			}
			else
			{
				repository.Stash.Refresh();
			}
			return true;
		}
	}

	/// <summary>Remote deleted.</summary>
	sealed class RemoteRemovedNotification : RepositoryChangedNotification
	{
		private readonly string _name;

		public RemoteRemovedNotification(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.RemoteRemoved; }
		}

		public override bool Apply(Repository repository)
		{
			return false;
		}
	}

	/// <summary>Remote created.</summary>
	sealed class RemoteCreatedNotification : RepositoryChangedNotification
	{
		private readonly string _name;

		public RemoteCreatedNotification(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public override object NotificationType
		{
			get { return RepositoryNotifications.RemoteCreated; }
		}

		public override bool Apply(Repository repository)
		{
			return false;
		}
	}
}
