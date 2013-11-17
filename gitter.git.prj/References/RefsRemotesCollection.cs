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

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository remote branches collection ($GIT_DIR/refs/remotes cache).</summary>
	public sealed class RefsRemotesCollection : GitObjectsCollection<RemoteBranch, RemoteBranchEventArgs>
	{
		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="RefsRemotesCollection"/> class.</summary>
		/// <param name="repository">Host repository.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		internal RefsRemotesCollection(Repository repository)
			: base(repository)
		{
		}

		#endregion

		#region Delete()

		/// <summary>Delete branch.</summary>
		/// <param name="branch">Branch to delete.</param>
		/// <param name="force">Delete branch irrespective of its merged status.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="branch"/> == <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="branch"/> is not handled by this repository or deleted.
		/// </exception>
		/// <exception cref="BranchIsNotFullyMergedException">
		/// Branch is not fully merged and can only be deleted if <paramref name="force"/> == <c>true</c>.
		/// </exception>
		/// <exception cref="GitException">
		/// Failed to delete <paramref name="branch"/>.
		/// </exception>
		internal void Delete(RemoteBranch branch, bool force)
		{
			Verify.Argument.IsValidGitObject(branch, Repository, "branch");

			var name = branch.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged))
			{
				Repository.Accessor.DeleteBranch.Invoke(
					new DeleteBranchParameters(name, true, force));
			}
			RemoveObject(branch);
		}

		#endregion

		#region Refresh()

		/// <summary>Updates remote branch cache.</summary>
		/// <param name="branches">Actual remote branch data.</param>
		private void RefreshInternal(IEnumerable<BranchData> branches)
		{
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<RemoteBranch, BranchData>(
					ObjectStorage,
					null,
					branchData => branchData.IsRemote,
					branches,
					remoteBranchData => ObjectFactories.CreateRemoteBranch(Repository, remoteBranchData),
					ObjectFactories.UpdateRemoteBranch,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}

		/// <summary>Updates remote branch cache.</summary>
		/// <param name="branches">Actual remote branch data.</param>
		private void RefreshInternal(IEnumerable<RemoteBranchData> branches)
		{
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<RemoteBranch, RemoteBranchData>(
					ObjectStorage,
					null,
					null,
					branches,
					remoteBranchData => ObjectFactories.CreateRemoteBranch(Repository, remoteBranchData),
					ObjectFactories.UpdateRemoteBranch,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}

		/// <summary>Refresh remote branches.</summary>
		public void Refresh()
		{
			var refs = Repository.Accessor.QueryBranches.Invoke(
				new QueryBranchesParameters(QueryBranchRestriction.Remote));
			RefreshInternal(refs.Remotes);
		}

		/// <summary>Refreshes the specified branches.</summary>
		/// <param name="branches">Actual remote branch data.</param>
		internal void Refresh(IEnumerable<RemoteBranchData> branches)
		{
			Verify.Argument.IsNotNull(branches, "branches");

			RefreshInternal(branches);
		}

		/// <summary>Refreshes the specified branches.</summary>
		/// <param name="branches">Actual remote branch data.</param>
		internal void Refresh(IEnumerable<BranchData> branches)
		{
			Verify.Argument.IsNotNull(branches, "branches");

			RefreshInternal(branches);
		}

		/// <summary>Refresh branch's position (and remove branch if it doesn't exist anymore).</summary>
		/// <param name="branch">Branch to refresh.</param>
		internal void Refresh(RemoteBranch branch)
		{
			Verify.Argument.IsValidGitObject(branch, Repository, "branch");

			var remoteBranchData = Repository.Accessor.QueryBranch.Invoke(
				new QueryBranchParameters(branch.Name, branch.IsRemote));
			if(remoteBranchData != null)
			{
				ObjectFactories.UpdateRemoteBranch(branch, remoteBranchData);
			}
			else
			{
				RemoveObject(branch);
			}
		}

		#endregion

		#region Get()

		private IList<RemoteBranch> GetRemotes(BranchesData refs)
		{
			var heads = refs.Heads;
			var remotes = refs.Remotes;
			if(heads.Count == 0)
			{
				return new RemoteBranch[0];
			}
			else
			{
				var res = new List<RemoteBranch>(heads.Count);
				lock(SyncRoot)
				{
					foreach(var head in heads)
					{
						var branch = TryGetItem(head.Name);
						if(branch != null) res.Add(branch);
					}
				}
				return res;
			}
		}

		/// <summary>Gets the list of unmerged remote branches.</summary>
		/// <returns>List of unmerged remote branches.</returns>
		public IList<RemoteBranch> GetUnmerged()
		{
			var refs = Repository.Accessor.QueryBranches.Invoke(
				new QueryBranchesParameters(QueryBranchRestriction.Remote, BranchQueryMode.NoMerged));
			return GetRemotes(refs);
		}

		/// <summary>Gets the list of merged remote branches.</summary>
		/// <returns>List of merged remote branches.</returns>
		public IList<RemoteBranch> GetMerged()
		{
			var refs = Repository.Accessor.QueryBranches.Invoke(
				new QueryBranchesParameters(QueryBranchRestriction.Remote, BranchQueryMode.Merged));
			return GetRemotes(refs);
		}

		/// <summary>Gets the list of remote branches, containing specified <paramref name="revision"/>.</summary>
		/// <param name="revision">Revision which must be present in any resulting remote branch.</param>
		/// <returns>List of remote branches, containing specified <paramref name="revision"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="revision"/> == <c>null</c>.</exception>
		public IList<RemoteBranch> GetContaining(IRevisionPointer revision)
		{
			Verify.Argument.IsValidRevisionPointer(revision, Repository, "revision");

			var refs = Repository.Accessor.QueryBranches.Invoke(
				new QueryBranchesParameters(QueryBranchRestriction.Remote, BranchQueryMode.Contains, revision.Pointer));
			return GetRemotes(refs);
		}

		#endregion

		#region Load()

		/// <summary>Perform initial load of remote branches.</summary>
		/// <param name="branchDataList">List of remote branch data containers.</param>
		internal void Load(IEnumerable<RemoteBranchData> branchDataList)
		{
			ObjectStorage.Clear();
			if(branchDataList != null)
			{
				foreach(var remoteBranchData in branchDataList)
				{
					AddObject(ObjectFactories.CreateRemoteBranch(Repository, remoteBranchData));
				}
			}
		}

		/// <summary>Perform initial load of remote branches.</summary>
		/// <param name="branchDataList">List of remote branch data containers.</param>
		internal void Load(IEnumerable<BranchData> branchDataList)
		{
			ObjectStorage.Clear();
			if(branchDataList != null)
			{
				foreach(var remoteBranchData in branchDataList)
				{
					AddObject(ObjectFactories.CreateRemoteBranch(Repository, remoteBranchData));
				}
			}
		}

		#endregion

		#region Notify()

		/// <summary>Notifies that remote branch was created externally.</summary>
		/// <param name="remoteBranchData">Created remote branch data.</param>
		/// <returns>Created remote branch.</returns>
		internal RemoteBranch NotifyCreated(RemoteBranchData remoteBranchData)
		{
			var branch = ObjectFactories.CreateRemoteBranch(Repository, remoteBranchData);
			AddObject(branch);
			return branch;
		}

		/// <summary>Notifies that remote branch was created externally.</summary>
		/// <param name="branchData">Created remote branch data.</param>
		/// <returns>Created remote branch.</returns>
		internal RemoteBranch NotifyCreated(BranchData branchData)
		{
			var branch = ObjectFactories.CreateRemoteBranch(Repository, branchData);
			AddObject(branch);
			return branch;
		}

		#endregion

		#region Overrides

		/// <summary>Fixes the input branch name.</summary>
		/// <param name="name">Input value.</param>
		/// <returns>Fixed name value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> == <c>null</c>.</exception>
		protected override string FixInputName(string name)
		{
			Verify.Argument.IsNotNull(name, "name");

			if(name.StartsWith(GitConstants.RemoteBranchPrefix) && !ContainsObjectName(name))
			{
				return name.Substring(GitConstants.RemoteBranchPrefix.Length);
			}
			return name;
		}

		/// <summary>Creates the event args for specified <paramref name="item"/>.</summary>
		/// <param name="item">Item to create event args for.</param>
		/// <returns>Created event args.</returns>
		protected override RemoteBranchEventArgs CreateEventArgs(RemoteBranch item)
		{
			return new RemoteBranchEventArgs(item);
		}

		#endregion
	}
}
