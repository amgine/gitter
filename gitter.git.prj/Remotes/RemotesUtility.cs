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
using gitter.Git.AccessLayer;
using gitter.Framework;

using Resources = gitter.Git.Properties.Resources;

static class RemotesUtility
{
	public static void FetchOrPull(Repository repository, Remote? remote, bool pull)
	{
		var affectedReferences = ReferenceType.RemoteBranch | ReferenceType.Tag;
		if(pull)
		{
			affectedReferences |= ReferenceType.LocalBranch;
		}
		Many<ReferenceChange> changes;
		var state1 = RefsState.Capture(repository, affectedReferences);
		using(repository.Monitor.BlockNotifications(
			RepositoryNotifications.BranchChanged,
			RepositoryNotifications.TagChanged))
		{
			try
			{
				if(pull)
				{
					var request = new PullRequest();
					request.Repository = remote?.Name;
					repository.Accessor.Pull.Invoke(request);
				}
				else
				{
					var request = new FetchRequest();
					request.Repository = remote?.Name;
					repository.Accessor.Fetch.Invoke(request);
				}
			}
			finally
			{
				repository.Refs.Refresh(affectedReferences);
				var state2 = RefsState.Capture(repository, affectedReferences);
				changes = RefsDiff.Calculate(state1, state2);
				if(!changes.IsEmpty)
				{
					repository.OnUpdated();
				}
			}
		}
		if(pull)
		{
			repository.Remotes.OnPullCompleted(remote, changes);
		}
		else
		{
			repository.Remotes.OnFetchCompleted(remote, changes);
		}
	}

	private static Task GetFetchOrPullTask(Repository repository, Remote? remote, bool pull,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		if(pull)
		{
			var request = new PullRequest();
			request.Repository = remote?.Name;
			return repository.Accessor.Pull.InvokeAsync(request, progress, cancellationToken);
		}
		else
		{
			var request = new FetchRequest();
			request.Repository = remote?.Name;
			return repository.Accessor.Fetch.InvokeAsync(request, progress, cancellationToken);
		}
	}

	public static async Task FetchOrPullAsync(
		Repository repository, Remote? remote, bool pull,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		var affectedReferences = ReferenceType.RemoteBranch | ReferenceType.Tag;
		if(pull)
		{
			affectedReferences |= ReferenceType.LocalBranch;
		}
		var suppressedNotifications = repository.Monitor.BlockNotifications(
			RepositoryNotifications.BranchChanged, RepositoryNotifications.TagChanged);
		var state1 = RefsState.Capture(repository, affectedReferences);
		await GetFetchOrPullTask(repository, remote, pull, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
			
		progress?.Report(new OperationProgress(Resources.StrRefreshingReferences.AddEllipsis()));
		repository.Refs.Refresh(affectedReferences);
		var state2 = RefsState.Capture(repository, affectedReferences);
		var changes = RefsDiff.Calculate(state1, state2);
		suppressedNotifications.Dispose();
		if(!changes.IsEmpty)
		{
			repository.OnUpdated();
		}
			
		if(pull)
		{
			repository.Remotes.OnPullCompleted(remote, changes);
		}
		else
		{
			repository.Remotes.OnFetchCompleted(remote, changes);
		}
	}

	private static PushRequest CreatePushRequest(
		string       remoteRepository,
		Many<Branch> branches,
		bool         forceOverwrite,
		bool         thinPack,
		bool         sendTags)
		=> new()
		{
			Repository = remoteRepository,
			PushMode   = sendTags ? PushMode.Tags : PushMode.Default,
			Force      = forceOverwrite,
			ThinPack   = thinPack,
			Refspecs   = branches.ConvertAll(static b => b.Name),
		};

	private static async Task PushAsync(
		Repository                    repository,
		PushRequest                   request,
		IProgress<OperationProgress>? progress,
		CancellationToken             cancellationToken)
	{
		Many<ReferencePushResult> res;
		using(var notificationsBlock = repository.Monitor.BlockNotifications(RepositoryNotifications.BranchChanged))
		{
			res = await repository
				.Accessor
				.Push
				.InvokeAsync(request, progress, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
			
		var changed = res.Any(static r
			=> r.Type != PushResultType.UpToDate
			&& r.Type != PushResultType.Rejected);
		if(changed)
		{
			repository.Refs.Remotes.Refresh();
		}
	}

	public static Task PushAsync(Repository repository, string url, Many<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		var request = CreatePushRequest(url, branches, forceOverwrite, thinPack, sendTags);
		return PushAsync(repository, request, progress, cancellationToken);
	}

	public static Task PushAsync(Repository repository, Remote remote, Many<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		var request = CreatePushRequest(remote.Name, branches, forceOverwrite, thinPack, sendTags);
		return PushAsync(repository, request, progress, cancellationToken);
	}
}
