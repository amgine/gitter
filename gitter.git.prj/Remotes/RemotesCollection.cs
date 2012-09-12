namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository remotes collection.</summary>
	public sealed class RemotesCollection : GitObjectsCollection<Remote, RemoteEventArgs>
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
			Assert.IsNotNull(remote);

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
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsFalse(ContainsObjectName(name), "name",
				Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat("Remote"));
			Verify.Argument.IsNeitherNullNorWhitespace(url, "url");

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
			Verify.Argument.IsValidGitObject(remote, Repository, "remote");
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsFalse(ContainsObjectName(name), "name",
				Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat("Remote"));

			var oldName = remote.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.RemoteRemoved,
				RepositoryNotifications.RemoteCreated,
				RepositoryNotifications.BranchChanged))
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
			Assert.IsNotNull(remote);
			Assert.IsNeitherNullNorWhitespace(oldName);

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
			Verify.Argument.IsNull(remote, "remote");
			Verify.Argument.IsValidGitObject(remote, Repository, "remote");

			var name = remote.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.RemoteRemoved,
				RepositoryNotifications.BranchChanged))
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
					ObjectStorage,
					null,
					null,
					remotes,
					remoteData => ObjectFactories.CreateRemote(Repository, remoteData),
					ObjectFactories.UpdateRemote,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}

		#endregion

		#region fetch

		public void Fetch()
		{
			Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.TagChanged))
			{
				Repository.Accessor.Fetch(
					new FetchParameters());
			}
			Repository.Refs.Refresh(ReferenceType.RemoteBranch | ReferenceType.Tag);
			Repository.InvokeUpdated();
		}

		public IAsyncAction FetchAsync()
		{
			Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

			return AsyncAction.Create(
				Repository,
				(repository, monitor) =>
				{
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChanged,
						RepositoryNotifications.TagChanged))
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
			Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.TagChanged))
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
			Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

			return AsyncAction.Create(
				Repository,
				(repository, monitor) =>
				{
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChanged,
						RepositoryNotifications.TagChanged))
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
			Verify.Argument.IsNeitherNullNorWhitespace(url, "url");
			Verify.Argument.IsValidRevisionPointerSequence(branches, Repository, "branches");
			Verify.Argument.IsTrue(branches.Count != 0, "branches",
				Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("branch"));

			var branchNames = new List<string>(branches.Count);
			foreach(var branch in branches)
			{
				branchNames.Add(branch.Name);
			}
			IList<ReferencePushResult> res;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged))
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
			Verify.Argument.IsNeitherNullNorWhitespace(url, "url");
			Verify.Argument.IsValidRevisionPointerSequence(branches, Repository, "branches");
			Verify.Argument.IsTrue(branches.Count != 0, "branches",
				Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("branch"));

			var branchNames = new List<string>(branches.Count);
			foreach(var branch in branches)
			{
				branchNames.Add(branch.Name);
			}
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
						RepositoryNotifications.BranchChanged))
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
