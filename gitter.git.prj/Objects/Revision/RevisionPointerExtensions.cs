namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Extension methods for <see cref="IRevisionPointer"/>.</summary>
	public static class RevisionPointerExtensions
	{
		private static void ValidateRevisionPointer(IRevisionPointer revision, string argName = "revision")
		{
			if(revision == null) throw new ArgumentNullException(argName);
			if(revision.IsDeleted)
			{
				throw new ArgumentException(
					Resources.ExcSuppliedRevisionIsDeleted, argName);
			}
		}

		private static void ValidateRevisionPointers(
			IRevisionPointer revision1, IRevisionPointer revision2,
			string argName1 = "revision1", string argName2 = "revision2")
		{
			if(revision1 == null) throw new ArgumentNullException(argName1);
			if(revision1.IsDeleted)
			{
				throw new ArgumentException(
					Resources.ExcSuppliedRevisionIsDeleted, argName1);
			}
			if(revision2 == null) throw new ArgumentNullException(argName2);
			if(revision2.IsDeleted)
			{
				throw new ArgumentException(
					Resources.ExcSuppliedRevisionIsDeleted, argName2);
			}
			if(revision2.Repository != revision1.Repository)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcAllObjectsMustBeHandledByThisRepository, "revisions"), argName2);
			}
		}

		#region checkout

		/// <summary>Checks out <paramref name="revision"/>. Checking out a non-branch revision will result in detached HEAD.</summary>
		/// <param name="revision">Reference to checkout.</param>
		/// <param name="force">Throw away local changes.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="revision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="revision"/> is deleted.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="revision"/> or failed to checkout.</exception>
		public static void Checkout(this IRevisionPointer revision, bool force = false)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			if(repository.Head == revision) return;
			bool revisionIsLocalBranch = (revision is Branch);
			if(!revisionIsLocalBranch && repository.Head.IsDetached)
			{
				var rev = revision.Dereference();
				if(rev == repository.Head.Revision) return;
			}

			var pointer = revisionIsLocalBranch ? revision.Pointer : revision.FullName;

			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.CheckoutNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.IndexUpdatedNotification))
			{
				repository.Accessor.Checkout(
					new CheckoutParameters(pointer, force));
			}

			repository.Head.Pointer = revision;
			repository.Head.NotifyRelogRecordAdded();

			repository.Status.Refresh();
		}

		public static void CheckoutPath(this IRevisionPointer revision, string path)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.WorktreeUpdatedNotification))
			{
				repository.Accessor.CheckoutFiles(
					new CheckoutFilesParameters(revision.Pointer, path)
					{
					});
			}

			repository.Status.Refresh();
		}

		public static void CheckoutPaths(this IRevisionPointer revision, IList<string> paths)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.WorktreeUpdatedNotification))
			{
				repository.Accessor.CheckoutFiles(
					new CheckoutFilesParameters(revision.Pointer, paths)
					{
					});
			}

			repository.Status.Refresh();
		}

		#endregion

		#region cherry-pick

		/// <summary>Performs a cherry-pick operation on <paramref name="revisions"/>.</summary>
		/// <param name="revisions">Commits to cherry-pick.</param>
		public static void CherryPick(this IEnumerable<IRevisionPointer> revisions)
		{
			if(revisions == null) throw new ArgumentNullException("revision");
			var list = new List<string>();
			Repository repository = null;
			foreach(var rev in revisions)
			{
				if(rev == null) throw new ArgumentException("revisions");
				if(rev.IsDeleted) throw new ArgumentException("revisions");
				list.Add(rev.Pointer);
				if(repository == null)
				{
					repository = rev.Repository;
				}
				else if(repository != rev.Repository)
				{
					throw new ArgumentNullException("revisions");
				}
			}
			if(list.Count == 0) throw new ArgumentException("revisions");
			try
			{
				repository.Accessor.CherryPick(new CherryPickParameters(list));
				repository.InvokeUpdated();
				repository.Head.NotifyRelogRecordAdded();
			}
			catch(AutomaticCherryPickFailedException)
			{
				repository.Status.Refresh();
				throw;
			}
		}

		/// <summary>Performs a cherry-pick operation on <paramref name="revision"/>.</summary>
		/// <param name="revision">Commit to cherry-pick.</param>
		public static void CherryPick(this IRevisionPointer revision)
		{
			CherryPick(revision, false);
		}

		/// <summary>Performs a cherry-pick operation on <paramref name="revision"/>.</summary>
		/// <param name="revision">Commit to cherry-pick.</param>
		/// <param name="noCommit">Do not commit.</param>
		public static void CherryPick(this IRevisionPointer revision, bool noCommit)
		{
			ValidateRevisionPointer(revision);
			var repository = revision.Repository;
			if(repository.Head.IsEmpty)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcCantDoOnEmptyRepository, "cherry-pick"));
			}

			var rev = repository.Head.Revision;
			var cb = repository.Head.Pointer as Branch;
			try
			{
				using(repository.Monitor.BlockNotifications(
					RepositoryNotifications.IndexUpdatedNotification,
					RepositoryNotifications.WorktreeUpdatedNotification,
					RepositoryNotifications.BranchChangedNotification,
					RepositoryNotifications.CheckoutNotification))
				{
					repository.Accessor.CherryPick(
						new CherryPickParameters(revision.Pointer, noCommit));
					if(cb != null)
					{
						cb.Refresh();
						var branchTip = cb.Revision;
						if(branchTip != rev)
						{
							repository.InvokeCommitCreated(branchTip);
						}
					}
					else
					{
						repository.Head.Refresh();
						var headRev = repository.Head.Revision;
						if(headRev != rev)
						{
							repository.InvokeCommitCreated(headRev);
						}
					}
					repository.Head.NotifyRelogRecordAdded();
				}
			}
			catch(GitException)
			{
				repository.InvokeStateChanged();
				throw;
			}
			finally
			{
				repository.Status.Refresh();
			}
		}

		#endregion

		#region reset

		/// <summary>Reset HEAD to <paramref name="revision"/>.</summary>
		/// <param name="mode">Reset mode</param>
		/// <param name="revision">HEAD's new position.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="revision"/> == null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="revision"/> is not handled by this <see cref="Repository"/> or it is deleted.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">
		/// Failed to dereference <paramref name="revision"/> or git reset failed.
		/// </exception>
		public static void ResetHeadHere(this IRevisionPointer revision, ResetMode mode = ResetMode.Mixed)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			repository.Head.Reset(revision, mode);
		}

		#endregion

		#region revert

		public static void Revert(this IRevisionPointer revision)
		{
			Revert(revision, false);
		}

		public static void Revert(this IRevisionPointer revision, bool noCommit)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;

			var rev = repository.Head.Revision;
			var currentBranch = repository.Head.CurrentBranch;
			var notifications = noCommit ?
				new[]
				{
					RepositoryNotifications.IndexUpdatedNotification,
					RepositoryNotifications.WorktreeUpdatedNotification,
				} :
				new[]
				{
					RepositoryNotifications.BranchChangedNotification,
					RepositoryNotifications.CheckoutNotification,
					RepositoryNotifications.IndexUpdatedNotification,
					RepositoryNotifications.WorktreeUpdatedNotification,
				};
			using(repository.Monitor.BlockNotifications(notifications))
			{
				try
				{
					repository.Accessor.Revert(new RevertParameters(revision.Pointer, noCommit));
					if(!noCommit)
					{
						if(currentBranch != null)
						{
							currentBranch.Refresh();
						}
						else
						{
							repository.Head.Refresh();
						}
						var headRev = repository.Head.Revision;
						if(headRev != rev)
						{
							repository.InvokeCommitCreated(headRev);
							repository.Head.NotifyRelogRecordAdded();
						}
					}
				}
				finally
				{
					repository.Status.Refresh();
				}
			}
		}

		public static void Revert(this IEnumerable<IRevisionPointer> revisions)
		{
			Revert(revisions, false);
		}

		public static void Revert(this IEnumerable<IRevisionPointer> revisions, bool noCommit)
		{
			if(revisions == null) throw new ArgumentNullException("revisions");

			var list = new List<string>();
			Repository repository = null;
			foreach(var rev in revisions)
			{
				if(rev == null) throw new ArgumentException("revisions");
				if(rev.IsDeleted) throw new ArgumentException("revisions");
				list.Add(rev.Pointer);
				if(repository == null)
				{
					repository = rev.Repository;
				}
				else if(repository != rev.Repository)
				{
					throw new ArgumentNullException("revisions");
				}
			}
			var oldHeadRev = repository.Head.Revision;
			try
			{
				repository.Accessor.Revert(
					new RevertParameters(list, noCommit));
				if(!noCommit)
				{
					var currentBranch = repository.Head.Pointer as Branch;
					if(currentBranch != null)
					{
						currentBranch.Refresh();
					}
					else
					{
						repository.Head.Refresh();
					}
					var newHeadRev = repository.Head.Revision;
					if(newHeadRev != oldHeadRev)
					{
						repository.InvokeUpdated();
						repository.Head.NotifyRelogRecordAdded();
					}
				}
			}
			finally
			{
				repository.Status.Refresh();
			}
		}

		#endregion

		#region merge

		public static void Merge(this IRevisionPointer revision, bool noCommit, bool noFastForward, bool squash, string message)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			repository.Head.Merge(revision, noCommit, noFastForward, squash, message);
		}

		public static void Merge(this IRevisionPointer revision, bool noCommit, bool noFastForward, bool squash)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			repository.Head.Merge(revision, noCommit, noFastForward, squash);
		}

		public static void Merge(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			repository.Head.Merge(revision);
		}

		#endregion

		#region rebase

		public static void RebaseHeadHere(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			var cb = repository.Head.CurrentBranch;
			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification,
				RepositoryNotifications.CheckoutNotification,
				RepositoryNotifications.IndexUpdatedNotification,
				RepositoryNotifications.WorktreeUpdatedNotification))
			{
				try
				{
					repository.Accessor.Rebase(
						new RebaseParameters(revision.Pointer));
				}
				finally
				{
					if(cb != null)
					{
						cb.Refresh();
					}
					else
					{
						repository.Head.Refresh();
					}
					repository.Status.Refresh();
				}
				repository.InvokeUpdated();
			}
		}

		public static IAsyncAction RebaseHeadHereAsync(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			return AsyncAction.Create(revision,
				(rev, monitor) =>
				{
					RebaseHeadHere(rev);
				},
				Resources.StrRebase,
				Resources.StrsRebaseIsInProcess.AddEllipsis());
		}

		#endregion

		#region describe

		public static Tag Describe(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;

			var tag = repository.Accessor.Describe(
				new DescribeParameters(revision.Pointer));
			if(tag != null) return repository.Refs.Tags.TryGetItem(tag);
			return null;
		}

		#endregion

		#region branch

		/// <summary>Create <see cref="Branch"/>, starting at this revision.</summary>
		/// <param name="revision">Revision to start branch from.</param>
		/// <param name="name">Branch name.</param>
		/// <returns>Created <see cref="Branch"/>.</returns>
		public static Branch CreateBranch(this IRevisionPointer revision, string name)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Refs.Heads.Create(name, revision);
		}

		/// <summary>Create <see cref="Branch"/>, starting at this revision.</summary>
		/// <param name="revision">Revision to start branch from.</param>
		/// <param name="name">Branch name.</param>
		/// <param name="checkout">Checkout branch after creation.</param>
		/// <returns>Created <see cref="Branch"/>.</returns>
		public static Branch CreateBranch(this IRevisionPointer revision, string name, bool checkout)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Refs.Heads.Create(name, revision, checkout);
		}

		/// <summary>Create <see cref="Branch"/>, starting at this revision.</summary>
		/// <param name="revision">Revision to start branch from.</param>
		/// <param name="name">Branch name.</param>
		/// <param name="tracking">Branch tracking mode.</param>
		/// <returns>Created <see cref="Branch"/>.</returns>
		public static Branch CreateBranch(this IRevisionPointer revision, string name, BranchTrackingMode tracking)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Refs.Heads.Create(name, revision, tracking);
		}

		/// <summary>Create <see cref="Branch"/>, starting at this revision.</summary>
		/// <param name="revision">Revision to start branch from.</param>
		/// <param name="name">Branch name.</param>
		/// <param name="tracking">Branch tracking mode.</param>
		/// <param name="checkout">Checkout branch after creation.</param>
		/// <returns>Created <see cref="Branch"/>.</returns>
		public static Branch CreateBranch(this IRevisionPointer revision, string name, BranchTrackingMode tracking, bool checkout)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Refs.Heads.Create(name, revision, tracking, checkout);
		}

		#endregion

		#region tag

		/// <summary>Create lightweight <see cref="Tag"/> at this revision.</summary>
		/// <param name="revision">Revision to create tag at.</param>
		/// <param name="name">Tag name.</param>
		/// <returns>Created <see cref="Tag"/>.</returns>
		public static Tag CreateTag(this IRevisionPointer revision, string name)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Refs.Tags.Create(name, revision);
		}

		/// <summary>Create annotated <see cref="Tag"/> at this revision.</summary>
		/// <param name="revision">Revision to create tag at.</param>
		/// <param name="name">Tag name.</param>
		/// <param name="message">Tag message.</param>
		/// <param name="sign">Signed by default key.</param>
		/// <returns>Created <see cref="Tag"/>.</returns>
		public static Tag CreateTag(this IRevisionPointer revision, string name, string message, bool sign)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Refs.Tags.Create(name, revision, message, sign);
		}

		/// <summary>Create signed <see cref="Tag"/> at this revision.</summary>
		/// <param name="revision">Revision to create tag at.</param>
		/// <param name="name">Tag name.</param>
		/// <param name="message">Tag message.</param>
		/// <param name="keyId">Key ID for signing tag..</param>
		/// <returns>Created <see cref="Tag"/>.</returns>
		public static Tag CreateTag(this IRevisionPointer revision, string name, string message, string keyId)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Refs.Tags.Create(name, revision, message, keyId);
		}

		#endregion

		#region note

		/// <summary>Add note for this revision.</summary>
		/// <param name="revision">Revision to add note for.</param>
		/// <param name="message">Note message to add.</param>
		/// <returns>Created <see cref="Note"/>.</returns>
		public static Note AddNote(this IRevisionPointer revision, string message)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Notes.Add(revision, message);
		}

		#endregion

		#region tree

		/// <summary>Gets <see cref="Tree"/> pointed by the specified <paramref name="revision"/>.</summary>
		/// <param name="revision">The revision.</param>
		/// <returns><see cref="Tree"/> pointed by the specified <paramref name="revision"/>..</returns>
		public static Tree GetTree(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			return new Tree(revision.Repository, revision.FullName);
		}

		#endregion

		#region diff

		/// <summary>Get diff for this revision.</summary>
		/// <param name="revision">Revision to get diff for.</param>
		/// <returns><see cref="Diff"/> for this revision.</returns>
		/// <exception cref="T:gitter.Git.GitException">Failed to get diff.</exception>
		public static string FormatPatch(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Accessor.QueryRevisionPatch(
				new QueryRevisionDiffParameters(revision.Pointer)
				{
					Binary = true
				});
		}

		/// <summary>Get diff for this revision.</summary>
		/// <param name="revision">Revision to get diff for.</param>
		/// <returns><see cref="Diff"/> for this revision.</returns>
		/// <exception cref="T:gitter.Git.GitException">Failed to get diff.</exception>
		public static Diff GetDiff(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			return repository.Accessor.QueryRevisionDiff(
				new QueryRevisionDiffParameters(revision.Pointer));
		}

		/// <summary>Get diff for this revision.</summary>
		/// <param name="revision">Revision to get diff for.</param>
		/// <returns><see cref="Diff"/> for this revision.</returns>
		/// <exception cref="T:gitter.Git.GitException">Failed to get diff.</exception>
		public static IAsyncFunc<Diff> GetDiffAsync(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;

			return AsyncFunc.Create(
				new
				{
					Accessor = repository.Accessor,
					Parameters =
						new QueryRevisionDiffParameters()
						{
							Revision = revision.Pointer,
						},
				},
				(data, monitor) =>
				{
					return data.Accessor.QueryRevisionDiff(data.Parameters);
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				"");
		}

		/// <summary>Compare this revision with <paramref name="revision2"/>.</summary>
		/// <param name="revision1">Base revision.</param>
		/// <param name="revision2">Revision to compare with.</param>
		/// <returns><see cref="Diff"/>, representing difference between <paramref name="revision1"/> and <paramref name="revision2"/>.</returns>
		public static Diff CompareWith(this IRevisionPointer revision1, IRevisionPointer revision2)
		{
			ValidateRevisionPointers(revision1, revision2);

			var repository = revision1.Repository;

			return repository.Accessor.QueryDiff(new QueryDiffParameters()
				{
					Revision1 = revision1.Pointer,
					Revision2 = revision2.Pointer,
				});
		}

		/// <summary>Compare this revision with <paramref name="revision2"/>.</summary>
		/// <param name="revision1">Base revision.</param>
		/// <param name="revision2">Revision to compare with.</param>
		/// <returns><see cref="Diff"/>, representing difference between <paramref name="revision1"/> and <paramref name="revision2"/>.</returns>
		public static IAsyncFunc<Diff> CompareWithAsync(this IRevisionPointer revision1, IRevisionPointer revision2)
		{
			ValidateRevisionPointers(revision1, revision2);

			var repository = revision1.Repository;

			return AsyncFunc.Create(
				new
				{
					Accessor = repository.Accessor,
					Parameters =
						new QueryDiffParameters()
						{
							Revision1 = revision1.Pointer,
							Revision2 = revision2.Pointer,
						},
				},
				(data, monitor) =>
				{
					return data.Accessor.QueryDiff(data.Parameters);
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				"");
		}

		#endregion

		#region blame

		public static BlameFile GetBlameFile(this IRevisionPointer revision, string fileName)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;

			var blame = repository.Accessor.QueryBlame(
				new QueryBlameParameters()
				{
					Revision = revision.Pointer,
					FileName = fileName,
				});

			return blame;
		}

		#endregion
	}
}
