namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository remote branches collection ($GIT_DIR/refs/remotes cache).</summary>
	public sealed class RefsRemotesCollection : GitObjectCollection<RemoteBranch, RemoteBranchEventArgs>
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
			ValidateObject(branch, "branch");

			var name = branch.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged))
			{
				Repository.Accessor.DeleteBranch(
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
			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Remote));
			RefreshInternal(refs.Remotes);
		}

		/// <summary>Refreshes the specified branches.</summary>
		/// <param name="branches">Actual remote branch data.</param>
		internal void Refresh(IEnumerable<RemoteBranchData> branches)
		{
			if(branches == null) throw new ArgumentNullException("branches");
			RefreshInternal(branches);
		}

		/// <summary>Refreshes the specified branches.</summary>
		/// <param name="branches">Actual remote branch data.</param>
		internal void Refresh(IEnumerable<BranchData> branches)
		{
			if(branches == null) throw new ArgumentNullException("branches");
			RefreshInternal(branches);
		}

		/// <summary>Refresh branch's position (and remove branch if it doesn't exist anymore).</summary>
		/// <param name="branch">Branch to refresh.</param>
		internal void Refresh(RemoteBranch branch)
		{
			ValidateObject(branch, "branch");

			var remoteBranchData = Repository.Accessor.QueryBranch(
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

		/// <summary>Gets the list of unmerged remote branches.</summary>
		/// <returns>List of unmerged remote branches.</returns>
		public IList<RemoteBranch> GetUnmerged()
		{
			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Remote, BranchQueryMode.NoMerged));
			var heads = refs.Heads;
			var remotes = refs.Remotes;
			if(heads.Count == 0)
			{
				return new RemoteBranch[0];
			}
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

		/// <summary>Gets the list of merged remote branches.</summary>
		/// <returns>List of merged remote branches.</returns>
		public IList<RemoteBranch> GetMerged()
		{
			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Remote, BranchQueryMode.Merged));
			var heads = refs.Heads;
			var remotes = refs.Remotes;
			if(heads.Count == 0)
			{
				return new RemoteBranch[0];
			}
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

		/// <summary>Gets the list of remote branches, containing specified <paramref name="revision"/>.</summary>
		/// <param name="revision">Revision which must be present in any resulting remote branch.</param>
		/// <returns>List of remote branches, containing specified <paramref name="revision"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="revision"/> == <c>null</c>.</exception>
		public IList<RemoteBranch> GetContaining(IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision, "revision");

			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Remote, BranchQueryMode.Contains, revision.Pointer));
			var heads = refs.Heads;
			var remotes = refs.Remotes;
			if(heads.Count == 0)
			{
				return new RemoteBranch[0];
			}
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

		#endregion

		#region Load()

		/// <summary>Perform initial load of remote branches.</summary>
		/// <param name="branchDataList">List of remote branch data containers.</param>
		internal void Load(IEnumerable<RemoteBranchData> branchDataList)
		{
			if(branchDataList == null) throw new ArgumentNullException("branchDataList");

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
			if(branchDataList == null) throw new ArgumentNullException("branchDataList");

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
			if(name == null) throw new ArgumentNullException("name");
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
