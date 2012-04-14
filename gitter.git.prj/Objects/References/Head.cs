namespace gitter.Git
{
	using System;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;
	using System.Collections.Generic;

	/// <summary>Repository's HEAD reference.</summary>
	public sealed class Head : Reference
	{
		#region Events

		/// <summary>Occurs when HEAD gets detached.</summary>
		public event EventHandler Detached;

		/// <summary>Occurs when HEAD gets attached.</summary>
		public event EventHandler Attached;

		/// <summary>Invokes <see cref="Detached"/> event.</summary>
		private void InvokeDetached()
		{
			var handler = Detached;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		/// <summary>Invokes <see cref="Attached"/> event.</summary>
		private void InvokeAttached()
		{
			var handler = Attached;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Data

		private bool _wasDetached;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="Head"/> class.</summary>
		/// <param name="repository">Host repository.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		internal Head(Repository repository)
			: base(repository, GitConstants.HEAD, GetHeadPointer(repository))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="Head"/> class.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="pointer">Target of this reference.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		internal Head(Repository repository, IRevisionPointer pointer)
			: base(repository, GitConstants.HEAD, pointer)
		{
		}

		#endregion

		/// <summary>Gets the current branch.</summary>
		/// <value>Current branch or <c>null</c> if HEAD is detached.</value>
		public Branch CurrentBranch
		{
			get { return Pointer as Branch; }
		}

		/// <summary>Returns object pointed by HEAD.</summary>
		/// <param name="repository">Repository to get HEAD from.</param>
		/// <returns>Object pointed by HEAD of the specified repository.</returns>
		private static IRevisionPointer GetHeadPointer(Repository repository)
		{
			var head = repository.Accessor.QuerySymbolicReference(
				new QuerySymbolicReferenceParameters(GitConstants.HEAD));

			switch(head.TargetType)
			{
				case ReferenceType.LocalBranch:
					Branch branch;
					lock(repository.Refs.Heads.SyncRoot)
					{
						branch = repository.Refs.Heads.TryGetItem(head.TargetObject);
						if(branch == null)
						{
							var info = repository.Accessor.QueryBranch(
								new QueryBranchParameters(head.TargetObject, false));
							if(info != null)
							{
								branch = repository.Refs.Heads.NotifyCreated(info);
							}
						}
					}
					if(branch == null)
					{
						return new NowherePointer(repository, head.TargetObject);
					}
					else
					{
						return branch;
					}
				case ReferenceType.Revision:
					lock(repository.Revisions.SyncRoot)
					{
						return repository.Revisions.GetOrCreateRevision(head.TargetObject);
					}
				default:
					return new NowherePointer(repository, head.TargetObject);
			}
		}

		/// <summary>Filter <see cref="IRevisionPointer"/> to types supported by this <see cref="Reference"/>.</summary>
		/// <param name="pointer">Raw pointer.</param>
		/// <returns>Valid pointer.</returns>
		protected override IRevisionPointer PrepareInputPointer(IRevisionPointer pointer)
		{
			switch(pointer.Type)
			{
				case ReferenceType.None:
				case ReferenceType.LocalBranch:
					return pointer;
				default:
					return pointer.Dereference();
			}
		}

		/// <summary>Gets a value indicating whether HEAD is detached.</summary>
		/// <value><c>true</c> if HEAD is detached; otherwise, <c>false</c>.</value>
		public bool IsDetached
		{
			get { return Pointer.Type != ReferenceType.LocalBranch; }
		}

		/// <summary>Gets a value indicating whether HEAD is pointing to non-existent object.</summary>
		/// <value><c>true</c> if HEAD is pointing to non-existent object; otherwise, <c>false</c>.</value>
		/// <remarks>It is typical to newly created repository whith Head.IsEmpty == <c>true</c>.</remarks>
		public bool IsEmpty
		{
			get { return Pointer.Type == ReferenceType.None; }
		}

		/// <summary>Called when this <see cref="Reference"/> is moved away from <paramref name="pointer"/>.</summary>
		/// <param name="pointer">Object, which this <see cref="Reference"/> was pointing to.</param>
		protected override void LeavePointer(IRevisionPointer pointer)
		{
			var branch = pointer as Branch;
			_wasDetached = branch == null;
			if(branch != null)
			{
				branch.PositionChanged -= OnBranchPositionChanged;
			}
		}

		/// <summary>Called when this <see cref="Reference"/> is moved to <paramref name="pointer"/>.</summary>
		/// <param name="pointer">Object, which this <see cref="Reference"/> will be pointing to.</param>
		protected override void EnterPointer(IRevisionPointer pointer)
		{
			var branch = pointer as Branch;
			bool detached = branch == null;
			if(_wasDetached != detached)
			{
				if(detached)
				{
					InvokeDetached();
				}
				else
				{
					InvokeAttached();
				}
			}
			if(branch != null)
			{
				branch.PositionChanged += OnBranchPositionChanged;
			}
		}

		/// <summary>Notifies about reflog modification.</summary>
		internal override void NotifyRelogRecordAdded()
		{
			base.NotifyRelogRecordAdded();
			var reference = Pointer as Reference;
			if(reference != null) reference.NotifyRelogRecordAdded();
		}

		private void OnBranchPositionChanged(object sender, RevisionChangedEventArgs e)
		{
			LeaveRevision(e.OldValue);
			EnterRevision(e.NewValue);
			InvokePositionChanged(e.OldValue, e.NewValue);
		}

		/// <summary>Reset HEAD to <paramref name="pointer"/>.</summary>
		/// <param name="mode">Reset mode</param>
		/// <param name="pointer">HEAD's new position.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="pointer"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="pointer"/> is not handled by this <see cref="Repository"/> or it is deleted.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">
		/// Failed to dereference <paramref name="pointer"/> or git reset failed.
		/// </exception>
		public void Reset(IRevisionPointer pointer, ResetMode mode)
		{
			#region validate args

			if(pointer == null) throw new ArgumentNullException("revision");
			if(pointer.IsDeleted)
			{
				throw new ArgumentException(
					Resources.ExcSuppliedRevisionIsDeleted, "revision");
			}

			#endregion

			var pos = Pointer.Dereference();
			var rev = pointer.Dereference();

			var currentBranch = Pointer as Branch;

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification,
				RepositoryNotifications.CheckoutNotification,
				RepositoryNotifications.IndexUpdatedNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.SubmodulesChangedNotification))
			{
				Repository.Accessor.Reset(
					new ResetParameters(rev.Name, mode));
			}

			if(currentBranch != null)
			{
				currentBranch.Pointer = rev;
			}
			else
			{
				Pointer = rev;
			}

			if(pos != rev)
			{
				NotifyRelogRecordAdded();
			}

			Repository.Status.Refresh();
			Repository.Submodules.Refresh();
			Repository.InvokeStateChanged();
		}

		/// <summary>Reset HEAD to <paramref name="pointer"/>.</summary>
		/// <param name="pointer">HEAD's new position.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="pointer"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="pointer"/> is not handled by this <see cref="Repository"/> or it is deleted.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="pointer"/> or git reset failed.</exception>
		public void Reset(IRevisionPointer pointer)
		{
			Reset(pointer, ResetMode.Mixed);
		}

		/// <summary>Updates cached information.</summary>
		public void Refresh()
		{
			Pointer = GetHeadPointer(Repository);
		}

		#region merge

		public string FormatMergeMessage(IRevisionPointer revision)
		{
			#region validate arguments

			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.Repository != Repository)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcSuppliedObjectIsNotHandledByThisRepository, "revision"), "revision");
			}
			if(revision.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcSuppliedObjectIsDeleted, "revision"), "revision");
			}
			if(IsEmpty)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcCantDoOnEmptyRepository, "format merge message"));
			}

			#endregion

			return Repository.Accessor.FormatMergeMessage(
				new FormatMergeMessageParameters(revision.Pointer, Pointer.Pointer));
		}

		public string FormatMergeMessage(ICollection<IRevisionPointer> revisions)
		{
			#region validate arguments

			if(revisions == null) throw new ArgumentNullException("revisions");
			if(revisions.Count == 0)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcCollectionMustContainAtLeastOneObject, "revisions"), "branches");
			}
			var names = new List<string>(revisions.Count);

			foreach(var branch in revisions)
			{
				if(branch == null)
				{
					throw new ArgumentException(
						Resources.ExcCollectionMustNotContainNullElements, "revisions");
				}
				if(branch.Repository != Repository)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAllObjectsMustBeHandledByThisRepository, "revisions"), "revisions");
				}
				if(branch.IsDeleted)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAtLeastOneOfSuppliedObjectIsDeleted, "revisions"), "revisions");
				}
				names.Add(branch.Pointer);
			}

			#endregion

			return Repository.Accessor.FormatMergeMessage(
				new FormatMergeMessageParameters(names, Pointer.Pointer));
		}

		public Revision Merge(IRevisionPointer branch, bool noCommit, bool noFastForward, bool squash, string message)
		{
			#region validate arguments

			if(branch == null) throw new ArgumentNullException("branch");
			if(branch.Repository != Repository)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcSuppliedObjectIsNotHandledByThisRepository, "branch"), "branch");
			}
			if(branch.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcSuppliedObjectIsDeleted, "branch"), "branch");
			}

			#endregion

			var oldRev = branch.Dereference();

			var currentBranch = CurrentBranch;

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.CheckoutNotification,
				RepositoryNotifications.IndexUpdatedNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.BranchChangedNotification))
			{
				try
				{
					Repository.Accessor.Merge(
						new MergeParameters(branch.FullName)
						{
							NoCommit = noCommit,
							NoFastForward = noFastForward,
							Squash = squash,
							Message = message,
						});
				}
				catch(AutomaticMergeFailedException)
				{
					Repository.InvokeStateChanged();
					Repository.Status.Refresh();
					throw;
				}
			}

			if(currentBranch != null)
			{
				currentBranch.Refresh();
			}
			else
			{
				Refresh();
			}

			var headRev = Revision;
			if(noCommit)
			{
				Repository.InvokeStateChanged();
				Repository.Status.Refresh();
			}
			else
			{
				if(noFastForward || headRev != oldRev) //not fast-forwarded
				{
					Repository.InvokeCommitCreated(headRev);
				}
			}
			NotifyRelogRecordAdded();
			return headRev;
		}

		public Revision Merge(IRevisionPointer branch, bool noCommit, bool noFastForward, bool squash)
		{
			return Merge(branch, noCommit, noFastForward, squash, null);
		}

		public Revision Merge(IRevisionPointer branch)
		{
			return Merge(branch, false, false, false, null);
		}

		public Revision Merge(ICollection<IRevisionPointer> branches, bool noCommit, bool noFastForward, bool squash, string message)
		{
			#region validate arguments

			if(branches == null) throw new ArgumentNullException("branches");
			if(branches.Count == 0)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcCollectionMustContainAtLeastOneObject, "branch"), "branches");
			}
			if(branches.Count == 1)
			{
				foreach(var branch in branches)
				{
					return Merge(branch, noCommit, noFastForward, squash, message);
				}
			}
			var currentBranch = CurrentBranch;

			var oldRevs = new List<Revision>(branches.Count);
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
				oldRevs.Add(branch.Dereference());
				branchNames.Add(branch.FullName);
			}

			#endregion

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.CheckoutNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.IndexUpdatedNotification,
				RepositoryNotifications.BranchChangedNotification))
			{
				try
				{
					Repository.Accessor.Merge(
						new MergeParameters(branchNames)
						{
							NoCommit = noCommit,
							NoFastForward = noFastForward,
							Squash = squash,
							Message = message,
						});
				}
				catch(AutomaticMergeFailedException)
				{
					Repository.InvokeStateChanged();
					Repository.Status.Refresh();
					throw;
				}
			}

			if(currentBranch != null)
			{
				currentBranch.Refresh();
			}
			else
			{
				Refresh();
			}

			var headRev = Revision;
			if(noCommit)
			{
				Repository.InvokeStateChanged();
				Repository.Status.Refresh();
			}
			else
			{
				if(noFastForward || !oldRevs.Contains(headRev)) //not fast-forwarded
				{
					Repository.InvokeCommitCreated(headRev);
				}
			}
			NotifyRelogRecordAdded();
			return headRev;
		}

		public Revision Merge(ICollection<IRevisionPointer> branches, bool noCommit, bool noFastForward, bool squash)
		{
			return Merge(branches, noCommit, noFastForward, squash, null);
		}

		public Revision Merge(ICollection<IRevisionPointer> branches)
		{
			return Merge(branches, false, false, false, null);
		}

		#endregion
	}
}
