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
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using gitter.Git.AccessLayer;
	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	static class RemotesUtility
	{
		public static void FetchOrPull(Repository repository, Remote remote, bool pull)
		{
			var affectedReferences = ReferenceType.RemoteBranch | ReferenceType.Tag;
			if(pull)
			{
				affectedReferences |= ReferenceType.LocalBranch;
			}
			ReferenceChange[] changes;
			var state1 = RefsState.Capture(repository, affectedReferences);
			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.TagChanged))
			{
				try
				{
					if(pull)
					{
						var p = new PullParameters();
						if(remote != null)
						{
							p.Repository = remote.Name;
						}
						repository.Accessor.Pull.Invoke(p);
					}
					else
					{
						var p = new FetchParameters();
						if(remote != null)
						{
							p.Repository = remote.Name;
						}
						repository.Accessor.Fetch.Invoke(p);
					}
				}
				finally
				{
					repository.Refs.Refresh(affectedReferences);
					var state2 = RefsState.Capture(repository, affectedReferences);
					changes = RefsDiff.Calculate(state1, state2);
					if(changes != null && changes.Length != 0)
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

		public static Task FetchOrPullAsync(
			Repository repository, Remote remote, bool pull,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			var affectedReferences = ReferenceType.RemoteBranch | ReferenceType.Tag;
			if(pull)
			{
				affectedReferences |= ReferenceType.LocalBranch;
			}
			var suppressedNotifications = repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged, RepositoryNotifications.TagChanged);
			var state1 = RefsState.Capture(repository, affectedReferences);
			Task task;
			if(pull)
			{
				var p = new PullParameters();
				if(remote != null)
				{
					p.Repository = remote.Name;
				}
				task = repository.Accessor.Pull.InvokeAsync(p, progress, cancellationToken);
			}
			else
			{
				var p = new FetchParameters();
				if(remote != null)
				{
					p.Repository = remote.Name;
				}
				task = repository.Accessor.Fetch.InvokeAsync(p, progress, cancellationToken);
			}
			return task.ContinueWith(
				t =>
				{
					progress.Report(new OperationProgress(Resources.StrRefreshingReferences.AddEllipsis()));
					repository.Refs.Refresh(affectedReferences);
					var state2 = RefsState.Capture(repository, affectedReferences);
					var changes = RefsDiff.Calculate(state1, state2);
					suppressedNotifications.Dispose();
					if(changes != null && changes.Length != 0)
					{
						repository.OnUpdated();
					}
					TaskUtility.PropagateFaultedStates(t);
					if(pull)
					{
						repository.Remotes.OnPullCompleted(remote, changes);
					}
					else
					{
						repository.Remotes.OnFetchCompleted(remote, changes);
					}
				});
		}

		private static PushParameters GetPushParameters(string remoteRepository, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			var names = new List<string>(branches.Count);
			foreach(var branch in branches)
			{
				names.Add(branch.Name);
			}
			var parameters = new PushParameters
			{
				Repository	= remoteRepository,
				PushMode	= sendTags ? PushMode.Tags : PushMode.Default,
				Force		= forceOverwrite,
				ThinPack	= thinPack,
				Refspecs	= names,
			};
			return parameters;
		}

		private static Task PushAsync(Repository repository, PushParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			var notificationsBlock = repository.Monitor.BlockNotifications(RepositoryNotifications.BranchChanged);
			return repository.Accessor
							 .Push.InvokeAsync(parameters, progress, cancellationToken)
							 .ContinueWith(task =>
								{
									notificationsBlock.Dispose();
									var res = TaskUtility.UnwrapResult(task);
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
								cancellationToken);
		}

		public static Task PushAsync(Repository repository, string url, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			var parameters = GetPushParameters(url, branches, forceOverwrite, thinPack, sendTags);
			return PushAsync(repository, parameters, progress, cancellationToken);
		}

		public static Task PushAsync(Repository repository, Remote remote, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			var parameters = GetPushParameters(remote.Name, branches, forceOverwrite, thinPack, sendTags);
			return PushAsync(repository, parameters, progress, cancellationToken);
		}
	}
}
