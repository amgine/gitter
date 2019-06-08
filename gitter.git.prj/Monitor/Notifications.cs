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

		public override object NotificationType => RepositoryNotifications.Checkout;

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

		public override object NotificationType => RepositoryNotifications.SubmodulesChanged;

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

		public override object NotificationType => RepositoryNotifications.RepositoryRemoved;

		public override bool Apply(Repository repository)
		{
			repository.OnDeleted();
			return true;
		}
	}

	sealed class ConfigUpdatedNotification : RepositoryChangedNotification
	{
		public ConfigUpdatedNotification()
		{
		}

		public override object NotificationType => RepositoryNotifications.ConfigUpdated;

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

		public override object NotificationType => RepositoryNotifications.IndexUpdated;

		public override bool Apply(Repository repository)
		{
			repository.Status.Refresh();
			return true;
		}
	}

	sealed class WorktreeUpdatedNotification : RepositoryChangedNotification
	{
		public WorktreeUpdatedNotification(string path)
		{
			Path = path;
		}

		public string Path { get; }

		public override object NotificationType => RepositoryNotifications.WorktreeUpdated;

		public override bool Apply(Repository repository)
		{
			repository.Status.Refresh();
			return true;
		}
	}

	/// <summary>Branch created/deleted/reset.</summary>
	sealed class BranchChangedNotification : RepositoryChangedNotification
	{
		public BranchChangedNotification(string name, bool remote)
		{
			Name     = name;
			IsRemote = remote;
		}

		public string Name { get; }

		public bool IsRemote { get; }

		public override object NotificationType => RepositoryNotifications.BranchChanged;

		public override bool Apply(Repository repository)
		{
			var branchInformation = repository.Accessor.QueryBranch.Invoke(
				new QueryBranchParameters(Name, IsRemote));
			if(branchInformation == null)
			{
				if(IsRemote)
				{
					var refs = repository.Refs.Remotes;
					lock(refs.SyncRoot)
					{
						if(refs.Contains(Name))
						{
							refs.NotifyRemoved(Name);
						}
						else
						{
							return false;
						}
					}
				}
				else
				{
					var refs = repository.Refs.Heads;
					lock(refs.SyncRoot)
					{
						if(refs.Contains(Name))
						{
							refs.NotifyRemoved(Name);
						}
						else
						{
							return false;
						}
					}
				}
			}
			else
			{
				if(IsRemote)
				{
					var refs = repository.Refs.Remotes;
					lock(refs.SyncRoot)
					{
						var branch = refs.TryGetItem(Name);
						if(branch == null)
						{
							refs.NotifyCreated(branchInformation);
						}
						else
						{
							if(branch.Revision.Hash != branchInformation.SHA1)
							{
								branch.NotifyReset(branchInformation);
							}
							else
							{
								return false;
							}
						}
					}
				}
				else
				{
					var refs = repository.Refs.Heads;
					lock(refs.SyncRoot)
					{
						var branch = refs.TryGetItem(Name);
						if(branch == null)
						{
							refs.NotifyCreated(branchInformation);
						}
						else
						{
							if(branch.Revision.Hash != branchInformation.SHA1)
							{
								branch.NotifyReset(branchInformation);
							}
							else
							{
								return false;
							}
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
		public TagChangedNotification(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public override object NotificationType => RepositoryNotifications.TagChanged;

		public override bool Apply(Repository repository)
		{
			var tagInformation = repository.Accessor.QueryTag.Invoke(
				new QueryTagParameters(Name));
			if(tagInformation == null)
			{
				var refs = repository.Refs.Tags;
				lock(refs.SyncRoot)
				{
					if(refs.Contains(Name))
					{
						refs.NotifyRemoved(Name);
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
					var tag = refs.TryGetItem(Name);
					if(tag == null)
					{
						refs.NotifyCreated(tagInformation);
					}
					else
					{
						if(tag.Revision.Hash != tagInformation.SHA1)
						{
							refs.NotifyRemoved(Name);
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

		public override object NotificationType => RepositoryNotifications.StashChanged;

		public override bool Apply(Repository repository)
		{
			var top = repository.Accessor.QueryStashTop.Invoke(
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
		public RemoteRemovedNotification(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public override object NotificationType => RepositoryNotifications.RemoteRemoved;

		public override bool Apply(Repository repository) => false;
	}

	/// <summary>Remote created.</summary>
	sealed class RemoteCreatedNotification : RepositoryChangedNotification
	{
		public RemoteCreatedNotification(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public override object NotificationType => RepositoryNotifications.RemoteCreated;

		public override bool Apply(Repository repository) => false;
	}
}
