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
	using System.Globalization;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

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

		/// <summary>Fetch completed.</summary>
		public event EventHandler<FetchCompletedEventArgs> FetchCompleted;

		/// <summary>Invokes <see cref="FetchCompleted"/>.</summary>
		/// <param name="remote">Remote.</param>
		/// <param name="changes">Reference changes.</param>
		internal void OnFetchCompleted(Remote remote, ReferenceChange[] changes)
		{
			var handler = FetchCompleted;
			if(handler != null) handler(this, new FetchCompletedEventArgs(remote, changes));
		}

		/// <summary>Pull completed.</summary>
		public event EventHandler<PullCompletedEventArgs> PullCompleted;

		/// <summary>Invokes <see cref="PullCompleted"/>.</summary>
		/// <param name="remote">Remote.</param>
		/// <param name="changes">Reference changes.</param>
		internal void OnPullCompleted(Remote remote, ReferenceChange[] changes)
		{
			var handler = PullCompleted;
			if(handler != null) handler(this, new PullCompletedEventArgs(remote, changes));
		}

		/// <summary>Prune completed.</summary>
		public event EventHandler<PruneCompletedEventArgs> PruneCompleted;

		/// <summary>Invokes <see cref="PruneCompleted"/>.</summary>
		/// <param name="remote">Remote.</param>
		/// <param name="changes">Reference changes.</param>
		internal void OnPruneCompleted(Remote remote, ReferenceChange[] changes)
		{
			var handler = PruneCompleted;
			if(handler != null) handler(this, new PruneCompletedEventArgs(remote, changes));
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

			Repository.Accessor.AddRemote.Invoke(
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
				Repository.Accessor.RenameRemote.Invoke(
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
			Verify.Argument.IsNotNull(remote, "remote");
			Verify.Argument.IsValidGitObject(remote, Repository, "remote");

			var name = remote.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.RemoteRemoved,
				RepositoryNotifications.BranchChanged))
			{
				Repository.Accessor.RemoveRemote.Invoke(
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
			var remotes = Repository.Accessor.QueryRemotes.Invoke(
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

			RemotesUtility.FetchOrPull(Repository, null, false);
		}

		public Task FetchAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(progress, "progress");
			Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

			return RemotesUtility.FetchOrPullAsync(Repository, null, false, progress, cancellationToken);
		}

		#endregion

		#region pull

		public void Pull()
		{
			Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

			RemotesUtility.FetchOrPull(Repository, null, true);
		}

		public Task PullAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(progress, "progress");
			Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

			return RemotesUtility.FetchOrPullAsync(Repository, null, true, progress, cancellationToken);
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
				res = Repository.Accessor.Push.Invoke(
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
		public Task PushToAsync(string url, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(url, "url");

			return RemotesUtility.PushAsync(Repository, url, branches, forceOverwrite, thinPack, sendTags, progress, cancellationToken);
		}

		#endregion
	}
}
