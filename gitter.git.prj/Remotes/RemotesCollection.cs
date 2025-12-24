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

namespace gitter.Git;

using System;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

/// <summary>Repository remotes collection.</summary>
public sealed class RemotesCollection : GitObjectsCollection<Remote, RemoteEventArgs>
{
	#region Events

	/// <inheritdoc/>
	protected override RemoteEventArgs CreateEventArgs(Remote item) => new(item);

	/// <summary>Remote renamed.</summary>
	public event EventHandler<RemoteEventArgs>? Renamed;

	/// <summary>Invoke <see cref="Renamed"/>.</summary>
	private void InvokeRenamed(Remote remote)
	{
		Assert.IsNotNull(remote);

		Renamed?.Invoke(this, new RemoteEventArgs(remote));
	}

	/// <summary>Fetch completed.</summary>
	public event EventHandler<FetchCompletedEventArgs>? FetchCompleted;

	/// <summary>Invokes <see cref="FetchCompleted"/>.</summary>
	/// <param name="remote">Remote.</param>
	/// <param name="changes">Reference changes.</param>
	internal void OnFetchCompleted(Remote? remote, Many<ReferenceChange> changes)
		=> FetchCompleted?.Invoke(this, new FetchCompletedEventArgs(remote, changes));

	/// <summary>Pull completed.</summary>
	public event EventHandler<PullCompletedEventArgs>? PullCompleted;

	/// <summary>Invokes <see cref="PullCompleted"/>.</summary>
	/// <param name="remote">Remote.</param>
	/// <param name="changes">Reference changes.</param>
	internal void OnPullCompleted(Remote? remote, Many<ReferenceChange> changes)
		=> PullCompleted?.Invoke(this, new PullCompletedEventArgs(remote, changes));

	/// <summary>Prune completed.</summary>
	public event EventHandler<PruneCompletedEventArgs>? PruneCompleted;

	/// <summary>Invokes <see cref="PruneCompleted"/>.</summary>
	/// <param name="remote">Remote.</param>
	/// <param name="changes">Reference changes.</param>
	internal void OnPruneCompleted(Remote? remote, Many<ReferenceChange> changes)
		=> PruneCompleted?.Invoke(this, new PruneCompletedEventArgs(remote, changes));

	#endregion

	#region .ctor

	internal RemotesCollection(Repository repository)
		: base(repository)
	{
	}

	#endregion

	#region Add()

	private static AddRemoteRequest GetAddRemoteRequest(string name, string url,
		bool fetch = false, bool mirror = false, TagFetchMode tagFetchMode = TagFetchMode.Default)
		=> new(name, url)
		{
			Fetch        = fetch,
			Mirror       = mirror,
			TagFetchMode = tagFetchMode,
		};

	public Remote Add(string name, string url, bool fetch = false, bool mirror = false,
		TagFetchMode tagFetchMode = TagFetchMode.Default)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);
		Verify.Argument.IsFalse(ContainsObjectName(name), nameof(name),
			Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat(nameof(Remote)));
		Verify.Argument.IsNeitherNullNorWhitespace(url);

		var request = GetAddRemoteRequest(name, url, fetch, mirror, tagFetchMode);
		Repository.Accessor.AddRemote.Invoke(request);

		var remote = new Remote(Repository, name, url, url);
		AddObject(remote);

		if(fetch)
		{
			Repository.Refs.Remotes.Refresh();
		}

		return remote;
	}

	public async Task<Remote> AddAsync(string name, string url, bool fetch = false, bool mirror = false,
		TagFetchMode tagFetchMode = TagFetchMode.Default)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);
		Verify.Argument.IsFalse(ContainsObjectName(name), nameof(name),
			Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat(nameof(Remote)));
		Verify.Argument.IsNeitherNullNorWhitespace(url);

		var request = GetAddRemoteRequest(name, url, fetch, mirror, tagFetchMode);
		await Repository.Accessor.AddRemote
			.InvokeAsync(request)
			.ConfigureAwait(continueOnCapturedContext: false);

		var remote = new Remote(Repository, name, url, url);
		AddObject(remote);

		if(fetch)
		{
			await Repository.Refs.Remotes
				.RefreshAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		return remote;
	}

	#endregion

	#region Rename()

	/// <summary>Rename remote repository reference.</summary>
	/// <param name="remote">Remote to rename.</param>
	/// <param name="name">New remote name.</param>
	internal void RenameRemote(Remote remote, string name)
	{
		Verify.Argument.IsValidGitObject(remote, Repository);
		Verify.Argument.IsNeitherNullNorWhitespace(name);
		Verify.Argument.IsFalse(ContainsObjectName(name), nameof(name),
			Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat(nameof(Remote)));

		var oldName = remote.Name;
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.RemoteRemoved,
			RepositoryNotifications.RemoteCreated,
			RepositoryNotifications.BranchChanged))
		{
			Repository.Accessor.RenameRemote.Invoke(
				new RenameRemoteRequest(oldName, name));
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
		Verify.Argument.IsNotNull(remote);
		Verify.Argument.IsValidGitObject(remote, Repository);

		var name = remote.Name;
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.RemoteRemoved,
			RepositoryNotifications.BranchChanged))
		{
			Repository.Accessor.RemoveRemote.Invoke(
				new RemoveRemoteRequest(name));
		}
		RemoveObject(remote);
		Repository.Refs.Remotes.Refresh();
	}

	/// <summary>Delete reference to remote repository <paramref name="remote"/>.</summary>
	/// <param name="remote">Removed remote.</param>
	internal async Task RemoveRemoteAsync(Remote remote)
	{
		Verify.Argument.IsNotNull(remote);
		Verify.Argument.IsValidGitObject(remote, Repository);

		var name = remote.Name;
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.RemoteRemoved,
			RepositoryNotifications.BranchChanged))
		{
			await Repository.Accessor.RemoveRemote
				.InvokeAsync(new RemoveRemoteRequest(name))
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		RemoveObject(remote);
		await Repository.Refs.Remotes
			.RefreshAsync()
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	#endregion

	#region Refresh()

	/// <summary>Sync information on remotes: removes non-existent, adds new, updates URL.</summary>
	public void Refresh()
	{
		var remotes = Repository.Accessor.QueryRemotes.Invoke(
			new QueryRemotesRequest());
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

	/// <summary>Sync information on remotes: removes non-existent, adds new, updates URL.</summary>
	public async Task RefreshAsync()
	{
		var remotes = await Repository.Accessor.QueryRemotes
			.InvokeAsync(new QueryRemotesRequest())
			.ConfigureAwait(continueOnCapturedContext: false);
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

	public Task FetchAsync(IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
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

	public Task PullAsync(IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.State.IsTrue(Count != 0, "Repository contains no remotes.");

		return RemotesUtility.FetchOrPullAsync(Repository, null, true, progress, cancellationToken);
	}

	#endregion

	#region push

	/// <summary>Send local objects to remote repository.</summary>
	public void PushTo(string url, Many<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(url);
		Verify.Argument.IsValidRevisionPointerSequence(branches, Repository);
		Verify.Argument.IsFalse(branches.IsEmpty, nameof(branches),
			Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("branch"));

		var names = branches.ConvertAll(static b => b.Name);
		Many<ReferencePushResult> res;
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.BranchChanged))
		{
			res = Repository.Accessor.Push.Invoke(
				new PushRequest(url, sendTags ? PushMode.Tags : PushMode.Default, names)
				{
					Force    = forceOverwrite,
					ThinPack = thinPack,
				});
		}
		var changed = res.Any(static r
			=> r.Type != PushResultType.UpToDate
			&& r.Type != PushResultType.Rejected);
		if(changed)
		{
			Repository.Refs.Remotes.Refresh();
		}
	}

	/// <summary>Send local objects to remote repository.</summary>
	public Task PushToAsync(string url, Many<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(url);

		return RemotesUtility.PushAsync(Repository, url, branches, forceOverwrite, thinPack, sendTags, progress, cancellationToken);
	}

	#endregion
}
