namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository remotes collection.</summary>
	public sealed class RemotesCollection : GitObjectCollection<Remote, RemoteEventArgs>
	{
		#region Events

		protected override RemoteEventArgs CreateEventArgs(Remote item)
		{
			return new RemoteEventArgs(item);
		}

		/// <summary>Remote renamed.</summary>
		public event EventHandler<RemoteEventArgs> Renamed;

		/// <summary>Invoke <see cref="Renamed"/>.</summary>
		private void InvokeRenamed(Remote remote)
		{
			if(remote == null) throw new ArgumentNullException("remote");
			var handler = Renamed;
			if(handler != null) handler(this, new RemoteEventArgs(remote));
		}

		#endregion

		#region .ctor

		internal RemotesCollection(Repository repository)
			: base(repository)
		{
		}

		#endregion

		#region Add()

		public Remote AddRemote(string name, string url, bool fetch, bool mirror, TagFetchMode tagFetchMode)
		{
			#region validate arguments

			if(name == null) throw new ArgumentNullException("name");
			if(name.Length == 0) throw new ArgumentException("name");
			if(url == null) throw new ArgumentNullException("url");
			if(url.Length == 0) throw new ArgumentException("url");
			if(ContainsObjectName(name))
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectWithThisNameAlreadyExists, "Remote"), "name");
			}

			#endregion

			Repository.Accessor.AddRemote(
				new AddRemoteParameters(name, url)
				{
					Fetch = fetch,
					Mirror = mirror,
					TagFetchMode = tagFetchMode,
				});

			var remote = new Remote(Repository, name, url, url);
			AddObject(remote);

			Repository.Refs.Remotes.Refresh();

			return remote;
		}

		#endregion

		#region Rename()

		/// <summary>Rename remote repository reference.</summary>
		/// <param name="remote">Remote to rename.</param>
		/// <param name="name">New remote name.</param>
		internal void RenameRemote(Remote remote, string name)
		{
			#region validate arguments

			if(name == null) throw new ArgumentNullException("remote");
			ValidateObject(remote, "remote");
			if(ContainsObjectName(name))
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectWithThisNameAlreadyExists, "Remote"), "name");
			}

			#endregion

			var oldName = remote.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.RemoteRemovedNotification,
				RepositoryNotifications.RemoteCreatedNotification,
				RepositoryNotifications.BranchChangedNotification))
			{
				Repository.Accessor.RenameRemote(
					new RenameRemoteParameters(oldName, name));
			}
		}

		/// <summary>Notify that <paramref name="remote"/> is renamed.</summary>
		/// <param name="remote">Renamed remote.</param>
		/// <param name="oldName">Old name.</param>
		internal void NotifyRenamed(Remote remote, string oldName)
		{
			lock(SyncRoot)
			{
				ObjectStorage.Remove(oldName);
				ObjectStorage.Add(remote.Name, remote);
				InvokeRenamed(remote);
			}
			Repository.Refs.Remotes.Refresh();
		}

		#endregion

		#region Remove()

		/// <summary>Delete reference to remote repository <paramref name="remote"/>.</summary>
		/// <param name="remote">Removed remote.</param>
		internal void RemoveRemote(Remote remote)
		{
			#region validate arguments

			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.Repository != Repository)
			{
				throw new ArgumentNullException(string.Format(
					Resources.ExcSuppliedObjectIsNotHandledByThisRepository, "remote"), "remote");
			}
			if(remote.IsDeleted)
			{
				throw new ArgumentNullException(string.Format(
					Resources.ExcSuppliedObjectIsDeleted, "remote"), "remote");
			}

			#endregion

			var name = remote.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.RemoteRemovedNotification,
				RepositoryNotifications.BranchChangedNotification))
			{
				Repository.Accessor.RemoveRemote(
					new RemoveRemoteParameters(name));
			}
			RemoveObject(remote);
			Repository.Refs.Remotes.Refresh();
		}

		#endregion

		#region Refresh()

		/// <summary>Sync information on remotes: removes non-existent, adds new, updates url.</summary>
		public void Refresh()
		{
			var remotes = Repository.Accessor.QueryRemotes(
				new QueryRemotesParameters());
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<Remote, RemoteData>(
					Repository,
					ObjectStorage,
					null,
					null,
					remotes,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}

		#endregion

		#region fetch

		public void Fetch()
		{
			if(Count == 0) throw new InvalidOperationException();

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification,
				RepositoryNotifications.TagChangedNotification))
			{
				Repository.Accessor.Fetch(
					new FetchParameters());
			}
			Repository.Refs.Refresh(ReferenceType.RemoteBranch | ReferenceType.Tag);
			Repository.InvokeUpdated();
		}

		public IAsyncAction FetchAsync()
		{
			if(Count == 0) throw new InvalidOperationException();

			return AsyncAction.Create(
				Repository,
				(repository, monitor) =>
				{
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChangedNotification,
						RepositoryNotifications.TagChangedNotification))
					{
						if(repository.Accessor.Fetch(
							new FetchParameters()
							{
								Monitor = monitor,
							}))
						{
							monitor.SetAction(Resources.StrRefreshingReferences.AddEllipsis());
							Repository.Refs.Refresh(ReferenceType.RemoteBranch | ReferenceType.Tag);
							repository.InvokeUpdated();
						}
					}
				},
				Resources.StrFetch,
				Resources.StrFetchingDataFromRemoteRepository,
				true);
		}

		#endregion

		#region pull

		public void Pull()
		{
			if(Count == 0) throw new InvalidOperationException();

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification,
				RepositoryNotifications.TagChangedNotification))
			{
				try
				{
					Repository.Accessor.Pull(
						new PullParameters());
				}
				finally
				{
					Repository.Refs.Refresh();
					Repository.InvokeUpdated();
				}
			}
		}

		public IAsyncAction PullAsync()
		{
			if(Count == 0) throw new InvalidOperationException();

			return AsyncAction.Create(
				Repository,
				(repository, monitor) =>
				{
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChangedNotification,
						RepositoryNotifications.TagChangedNotification))
					{
						try
						{
							if(repository.Accessor.Pull(
								new PullParameters()
								{
									Monitor = monitor,
								}))
							{
								monitor.SetAction(Resources.StrRefreshingReferences.AddEllipsis());
								repository.Refs.Refresh();
							}
						}
						catch
						{
							monitor.SetAction(Resources.StrRefreshingReferences.AddEllipsis());
							repository.Refs.Refresh();
							throw;
						}
						finally
						{
							repository.InvokeUpdated();
						}
					}
				},
				Resources.StrPull,
				Resources.StrFetchingDataFromRemoteRepository,
				true);
		}

		#endregion

		#region push

		/// <summary>Send local objects to remote repository.</summary>
		public void PushTo(string url, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			#region validate arguments

			if(url == null) throw new ArgumentNullException("url");
			if(branches == null) throw new ArgumentNullException("branches");
			if(branches.Count == 0)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcCollectionMustContainAtLeastOneObject, "branch"), "branches");
			}

			var branchNames = new List<string>(branches.Count);
			foreach(var branch in branches)
			{
				if(branch == null)
				{
					throw new ArgumentException(
						Resources.ExcCollectionMustNotContainNullElements, "branches");
				}
				if(branch.Repository != Repository)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAllObjectsMustBeHandledByThisRepository, "branches"), "branches");
				}
				if(branch.IsDeleted)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAtLeastOneOfSuppliedObjectIsDeleted, "branches"), "branches");
				}
				branchNames.Add(branch.Name);
			}

			#endregion

			IList<ReferencePushResult> res;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification))
			{
				res = Repository.Accessor.Push(
					new PushParameters(url, sendTags ? PushMode.Tags : PushMode.Default, branchNames)
					{
						Force = forceOverwrite,
						ThinPack = thinPack,
					});
			}
			bool changed = false;
			for(int i = 0; i < res.Count; ++i)
			{
				if(res[i].Type != PushResultType.UpToDate && res[i].Type != PushResultType.Rejected)
				{
					changed = true;
					break;
				}
			}
			if(changed)
			{
				Repository.Refs.Remotes.Refresh();
			}
		}

		/// <summary>Send local objects to remote repository.</summary>
		public IAsyncAction PushToAsync(string url, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			#region validate arguments

			if(url == null) throw new ArgumentNullException("url");
			if(branches == null) throw new ArgumentNullException("branches");
			if(branches.Count == 0)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcCollectionMustContainAtLeastOneObject, "branch"), "branches");
			}

			var branchNames = new List<string>(branches.Count);
			foreach(var b in branches)
			{
				if(b == null)
				{
					throw new ArgumentException(
						Resources.ExcCollectionMustNotContainNullElements, "branches");
				}
				if(b.Repository != Repository)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAllObjectsMustBeHandledByThisRepository, "branches"), "branches");
				}
				if(b.IsDeleted)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAtLeastOneOfSuppliedObjectIsDeleted, "branches"), "branches");
				}
				branchNames.Add(b.Name);
			}

			#endregion

			return AsyncAction.Create(
				new
				{
					Repository	= Repository,
					Url			= url,
					BranchNames	= branchNames,
					Force		= forceOverwrite,
					ThinPack	= thinPack,
					SendTags	= sendTags,
				},
				(data, monitor) =>
				{
					var repository = data.Repository;
					IList<ReferencePushResult> res;
					using(repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChangedNotification))
					{
						res = repository.Accessor.Push(
							new PushParameters(data.Url, data.SendTags ? PushMode.Tags : PushMode.Default, data.BranchNames)
							{
								Force = data.Force,
								ThinPack = data.ThinPack,
								Monitor = monitor,
							});
					}
					bool changed = false;
					for(int i = 0; i < res.Count; ++i)
					{
						if(res[i].Type != PushResultType.UpToDate && res[i].Type != PushResultType.Rejected)
						{
							changed = true;
							break;
						}
					}
					if(changed)
					{
						repository.Refs.Remotes.Refresh();
					}
				},
				Resources.StrPush,
				string.Format(Resources.StrSendingDataTo.AddEllipsis(), url),
				true);
		}

		#endregion
	}
}
