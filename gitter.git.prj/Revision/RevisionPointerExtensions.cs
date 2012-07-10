namespace gitter.Git
{
	using System;
	using System.Linq;
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
				RepositoryNotifications.Checkout,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
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
				RepositoryNotifications.WorktreeUpdated))
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
				RepositoryNotifications.WorktreeUpdated))
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
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated,
					RepositoryNotifications.BranchChanged,
					RepositoryNotifications.Checkout))
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
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated,
				} :
				new[]
				{
					RepositoryNotifications.BranchChanged,
					RepositoryNotifications.Checkout,
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated,
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
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
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
		/// <returns><see cref="Tree"/> pointed by the specified <paramref name="revision"/>.</returns>
		public static Tree GetTree(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			return new Tree(revision.Repository, revision.FullName);
		}

		/// <summary>Gets <see cref="Tree"/> pointed by the specified <paramref name="revision"/>.</summary>
		/// <param name="revision">The revision.</param>
		/// <returns>Function which retrieves <see cref="Tree"/> pointed by the specified <paramref name="revision"/>.</returns>
		public static IAsyncFunc<Tree> GetTreeAsync(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			return AsyncFunc.Create(
				revision,
				(mon, rev) =>
				{
					return new Tree(revision.Repository, revision.FullName);
				},
				"",
				"");
		}

		#endregion

		#region diff

		public static IRevisionDiffSource GetDiffSource(this IRevisionPointer revision, IEnumerable<string> paths = null)
		{
			ValidateRevisionPointer(revision);

			if(paths == null)
			{
				return new RevisionChangesDiffSource(revision);
			}
			else
			{
				return new RevisionChangesDiffSource(revision, paths.ToList());
			}
		}

		public static IDiffSource GetCompareDiffSource(this IRevisionPointer revision1, IRevisionPointer revision2, IEnumerable<string> paths = null)
		{
			ValidateRevisionPointers(revision1, revision2);

			if(paths == null)
			{
				return new RevisionCompareDiffSource(revision1, revision2);
			}
			else
			{
				return new RevisionCompareDiffSource(revision1, revision2, paths.ToList());
			}
		}

		/// <summary>Get diff for this revision.</summary>
		/// <param name="revision">Revision to get diff for.</param>
		/// <returns><see cref="Diff"/> for this revision.</returns>
		/// <exception cref="T:gitter.Git.GitException">Failed to get diff.</exception>
		public static string FormatPatch(this IRevisionPointer revision)
		{
			ValidateRevisionPointer(revision);

			var repository = revision.Repository;
			if(revision.Type == ReferenceType.Stash)
			{
				return repository.Accessor.QueryStashPatch(
					new QueryRevisionDiffParameters(revision.Pointer)
					{
						Binary = true
					});
			}
			else
			{
				return repository.Accessor.QueryRevisionPatch(
					new QueryRevisionDiffParameters(revision.Pointer)
					{
						Binary = true
					});
			};
		}

		#endregion

		#region blame

		public static IBlameSource GetBlameSource(this IRevisionPointer revision, string fileName)
		{
			ValidateRevisionPointer(revision);

			return new RevisionFileBlameSource(revision, fileName);
		}

		#endregion

		#region archive

		public static void Archive(this IRevisionPointer pointer, string outputFile, string path = null, string format = null)
		{
			ValidateRevisionPointer(pointer);

			pointer.Repository.Accessor.Archive(
				new ArchiveParameters()
				{
					Tree = pointer.FullName,
					Path = path,
					OutputFile = outputFile,
					Format = format,
				});
		}

		public static IAsyncAction ArchiveAsync(this IRevisionPointer pointer, string outputFile, string path = null, string format = null)
		{
			ValidateRevisionPointer(pointer);

			return AsyncAction.Create(
				new
				{
					Repository = pointer.Repository,
					Parameters = new ArchiveParameters()
					{
						Tree = pointer.FullName,
						Path = path,
						OutputFile = outputFile,
						Format = format,
					}
				},
				(data, mon) =>
				{
					data.Repository.Accessor.Archive(data.Parameters);
				},
				"Archive",
				"Creating archive from '{0}'...".UseAsFormat(pointer.Pointer));
		}

		#endregion
	}
}
