namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository branches collection ($GIT_DIR/refs/heads cache).</summary>
	public sealed class RefsHeadsCollection : GitObjectCollection<Branch, BranchEventArgs>
	{
		#region Events

		/// <summary>Occurs when branch is renamed.</summary>
		public event EventHandler<BranchRenamedEventArgs> BranchRenamed;

		/// <summary>Invokes <see cref="BranchRenamed"/> event.</summary>
		/// <param name="branch">Renamed branch.</param>
		/// <param name="oldName">Old branch name.</param>
		private void InvokeBranchRenamed(Branch branch, string oldName)
		{
			var handler = BranchRenamed;
			if(handler != null) handler(this, new BranchRenamedEventArgs(branch, oldName));
		}

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="RefsHeadsCollection"/> class.</summary>
		/// <param name="repository">Host repository.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		internal RefsHeadsCollection(Repository repository)
			: base(repository)
		{
		}

		#endregion

		#region Create()

		/// <summary>Create branch.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <param name="tracking">Tracking mode.</param>
		/// <param name="createRefLog">Create branch's reflog.</param>
		/// <param name="checkout">Set to <c>true</c> to checkout branch after creation.</param>
		/// <param name="orphan">Set to <c>true</c> to create orphan branch.</param>
		/// <returns>Created branch.</returns>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="startingRevision"/> or create a branch.</exception>
		private Branch CreateBranchCore(string name, IRevisionPointer startingRevision, BranchTrackingMode tracking, bool createRefLog, bool checkout, bool orphan)
		{
			var rev = startingRevision.Dereference();

			var notifications = checkout ?
				new[] { RepositoryNotifications.CheckoutNotification, RepositoryNotifications.BranchChangedNotification } :
				new[] { RepositoryNotifications.BranchChangedNotification };

			using(Repository.Monitor.BlockNotifications(notifications))
			{
				Repository.Accessor.CreateBranch(new CreateBranchParameters(
					name, startingRevision.Pointer, checkout, orphan, createRefLog, tracking));
			}

			var branch = new Branch(Repository, name, rev);

			AddObject(branch);

			if(checkout)
			{
				Repository.Head.Pointer = branch;
			}

			return branch;
		}

		/// <summary>Create orphan branch and checkout.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <param name="tracking">Tracking mode.</param>
		/// <param name="createRefLog">Create branch's reflog.</param>
		/// <returns>Created branch.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> == null or <paramref name="startingRevision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="startingRevision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// Branch '<paramref name="name"/>' already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="startingRevision"/> or create a branch.</exception>
		public Branch CreateOrphan(string name, IRevisionPointer startingRevision, BranchTrackingMode tracking, bool createRefLog)
		{
			#region validate arguments

			if(name == null) throw new ArgumentNullException("name");
			ValidateRevisionPointer(startingRevision, "startingRevision");
			Branch.ValidateName(name);
			if(ContainsObjectName(name))
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectWithThisNameAlreadyExists, "Branch"), "name");
			}

			#endregion

			return CreateBranchCore(name, startingRevision, tracking, createRefLog, true, true);
		}

		/// <summary>Create local branch.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <param name="tracking">Tracking mode.</param>
		/// <param name="checkout">Set to <c>true</c> to checkout branch after creation.</param>
		/// <param name="createRefLog">Create branch's reflog.</param>
		/// <returns>Created branch.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> == null or <paramref name="startingRevision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="startingRevision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// Branch '<paramref name="name"/>' already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="startingRevision"/> or create a branch.</exception>
		public Branch Create(string name, IRevisionPointer startingRevision, BranchTrackingMode tracking, bool checkout, bool createRefLog)
		{
			#region validate arguments

			if(name == null) throw new ArgumentNullException("name");
			ValidateRevisionPointer(startingRevision, "startingRevision");
			Branch.ValidateName(name);
			if(ContainsObjectName(name))
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectWithThisNameAlreadyExists, "Branch"), "name");
			}

			#endregion

			return CreateBranchCore(name, startingRevision, tracking, createRefLog, checkout, false);
		}

		/// <summary>Create local branch.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <param name="tracking">Tracking mode.</param>
		/// <param name="checkout">Set to true to checkout branch after creation.</param>
		/// <returns>Created branch.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> == null or <paramref name="startingRevision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="startingRevision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// Branch '<paramref name="name"/>' already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="startingRevision"/> or create a branch.</exception>
		public Branch Create(string name, IRevisionPointer startingRevision, BranchTrackingMode tracking, bool checkout)
		{
			return Create(name, startingRevision, tracking, checkout, false);
		}

		/// <summary>Create local branch.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <param name="tracking">Tracking mode.</param>
		/// <returns>Created branch.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> == null or <paramref name="startingRevision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="startingRevision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// Branch '<paramref name="name"/>' already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="startingRevision"/> or create a branch.</exception>
		public Branch Create(string name, IRevisionPointer startingRevision, BranchTrackingMode tracking)
		{
			return Create(name, startingRevision, tracking, false, false);
		}

		/// <summary>Create local branch.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <param name="checkout">Set to true to checkout branch after creation.</param>
		/// <returns>Created branch.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> == null or <paramref name="startingRevision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="startingRevision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// Branch '<paramref name="name"/>' already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="startingRevision"/> or create a branch.</exception>
		public Branch Create(string name, IRevisionPointer startingRevision, bool checkout)
		{
			return Create(name, startingRevision, BranchTrackingMode.Default, checkout, false);
		}

		/// <summary>Create local branch.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <returns>Created branch.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> == null or <paramref name="startingRevision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="startingRevision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// Branch '<paramref name="name"/>' already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="startingRevision"/> or create a branch.</exception>
		public Branch Create(string name, IRevisionPointer startingRevision)
		{
			return Create(name, startingRevision, BranchTrackingMode.Default, false, false);
		}

		#endregion

		#region Rename()

		/// <summary>Rename branch.</summary>
		/// <param name="branch">Branch to rename.</param>
		/// <param name="name">Branch's new name.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="branch"/> == null or <paramref name="name"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="branch"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// Branch '<paramref name="name"/>' already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to rename <paramref name="branch"/>.</exception>
		internal void Rename(Branch branch, string name)
		{
			#region validate arguments

			ValidateObject(branch, "branch");
			if(name == null) throw new ArgumentNullException("name");
			Branch.ValidateName(name);
			if(ContainsObjectName(name))
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectWithThisNameAlreadyExists, "Branch"), "name");
			}

			#endregion

			string oldName = branch.Name;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification))
			{
				Repository.Accessor.RenameBranch(
					new RenameBranchParameters(branch.Name, name));
			}
		}

		/// <summary>Notify that <paramref name="branch"/> is renamed.</summary>
		/// <param name="branch">Renamed branch.</param>
		/// <param name="oldName">Old name.</param>
		internal void NotifyRenamed(Branch branch, string oldName)
		{
			branch.Revision.RenameRef(GitConstants.LocalBranchPrefix + oldName, branch);
			lock(SyncRoot)
			{
				ObjectStorage.Remove(oldName);
				ObjectStorage.Add(branch.Name, branch);
				InvokeBranchRenamed(branch, oldName);
			}
		}

		#endregion

		#region Delete()

		/// <summary>Delete branch.</summary>
		/// <param name="branch">Branch to delete.</param>
		/// <param name="force">Delete branch irrespective of its merged status.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException"><paramref name="branch"/> is not handled by this repository or deleted.</exception>
		/// <exception cref="T:git.BranchIsNotFullyMergedException">Branch is not fully merged and can only be deleted if <paramref name="force"/> == true.</exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to delete <paramref name="branch"/>.</exception>
		internal void Delete(Branch branch, bool force)
		{
			ValidateObject(branch, "branch");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification))
			{
				Repository.Accessor.DeleteBranch(
					new DeleteBranchParameters(branch.Name, false, force));
			}
			RemoveObject(branch);
		}

		#endregion

		#region Refresh()

		/// <summary>Updates branch cache.</summary>
		/// <param name="branches">Actual branch data.</param>
		private void RefreshInternal(IEnumerable<BranchData> branches)
		{
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary(
					ObjectStorage,
					null,
					null,
					branches,
					branchData => ObjectFactories.CreateBranch(Repository, branchData),
					ObjectFactories.UpdateBranch,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}

		/// <summary>Refresh local branches.</summary>
		public void Refresh()
		{
			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Local));
			RefreshInternal(refs.Heads);
		}

		/// <summary>Refresh local branches.</summary>
		internal void Refresh(IEnumerable<BranchData> branches)
		{
			if(branches == null) throw new ArgumentNullException("branches");

			RefreshInternal(branches);
		}

		/// <summary>Refresh branch's position (and remove branch if it doesn't exist anymore).</summary>
		/// <param name="branch">Branch to refresh.</param>
		internal void Refresh(Branch branch)
		{
			ValidateObject(branch, "branch");

			var branchData = Repository.Accessor.QueryBranch(
				new QueryBranchParameters(branch.Name, branch.IsRemote));
			if(branchData != null)
			{
				ObjectFactories.UpdateBranch(branch, branchData);
			}
			else
			{
				RemoveObject(branch);
			}
		}

		#endregion

		#region Get()

		/// <summary>Gets the list of unmerged local branches.</summary>
		/// <returns>List of unmerged local branches.</returns>
		public IList<Branch> GetUnmerged()
		{
			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Local, BranchQueryMode.NoMerged));
			var heads = refs.Heads;
			var remotes = refs.Remotes;
			if(heads.Count == 0)
			{
				return new Branch[0];
			}
			var res = new List<Branch>(heads.Count);
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

		/// <summary>Gets the list of merged local branches.</summary>
		/// <returns>List of merged local branches.</returns>
		public IList<Branch> GetMerged()
		{
			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Local, BranchQueryMode.Merged));
			var heads = refs.Heads;
			var remotes = refs.Remotes;
			if(heads.Count == 0)
			{
				return new Branch[0];
			}
			var res = new List<Branch>(heads.Count);
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

		/// <summary>Gets the list of local branches, containing specified <paramref name="revision"/>.</summary>
		/// <param name="revision">Revision which must be present in any resulting local branch.</param>
		/// <returns>List of local branches, containing specified <paramref name="revision"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="revision"/> == <c>null</c>.</exception>
		public IList<Branch> GetContaining(IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision, "revision");

			var refs = Repository.Accessor.QueryBranches(
				new QueryBranchesParameters(QueryBranchRestriction.Local, BranchQueryMode.Contains, revision.Pointer));
			var heads = refs.Heads;
			var remotes = refs.Remotes;
			if(heads.Count == 0)
			{
				return new Branch[0];
			}
			var res = new List<Branch>(heads.Count);
			lock(SyncRoot)
			{
				foreach(var head in heads)
				{
					var branch = TryGetItem(head.Name);
					if(branch != null)
						res.Add(branch);
				}
			}
			return res;
		}

		#endregion

		#region Load()

		/// <summary>Perform initial load of branches.</summary>
		/// <param name="branchDataList">List of branch data containers.</param>
		internal void Load(IEnumerable<BranchData> branchDataList)
		{
			if(branchDataList == null) throw new ArgumentNullException("branchDataList");

			ObjectStorage.Clear();
			if(branchDataList != null)
			{
				foreach(var branchData in branchDataList)
				{
					AddObject(ObjectFactories.CreateBranch(Repository, branchData));
				}
			}
		}

		#endregion

		#region Notify()

		/// <summary>Notifies that branch was created externally.</summary>
		/// <param name="branchData">Created branch data.</param>
		/// <returns>Created branch.</returns>
		internal Branch NotifyCreated(BranchData branchData)
		{
			var branch = ObjectFactories.CreateBranch(Repository, branchData);
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
			if(name.StartsWith(GitConstants.LocalBranchPrefix) && !ContainsObjectName(name))
			{
				return name.Substring(GitConstants.LocalBranchPrefix.Length);
			}
			return name;
		}

		/// <summary>Creates the event args for specified <paramref name="item"/>.</summary>
		/// <param name="item">Item to create event args for.</param>
		/// <returns>Created event args.</returns>
		protected override BranchEventArgs CreateEventArgs(Branch item)
		{
			return new BranchEventArgs(item);
		}

		#endregion
	}
}
