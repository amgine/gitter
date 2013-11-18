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
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Extension methods for <see cref="IRevisionPointer"/>.</summary>
	public static class RevisionPointerExtensions
	{
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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
				repository.Accessor.Checkout.Invoke(
					new CheckoutParameters(pointer, force));
			}

			repository.Head.Pointer = revision;
			repository.Head.NotifyRelogRecordAdded();

			repository.Status.Refresh();
		}

		public static void CheckoutPath(this IRevisionPointer revision, string path)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.WorktreeUpdated))
			{
				repository.Accessor.CheckoutFiles.Invoke(
					new CheckoutFilesParameters(revision.Pointer, path)
					{
					});
			}

			repository.Status.Refresh();
		}

		public static void CheckoutPaths(this IRevisionPointer revision, IList<string> paths)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.WorktreeUpdated))
			{
				repository.Accessor.CheckoutFiles.Invoke(
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
			Verify.Argument.IsValidRevisionPointerSequence(revisions, "revisions");

			var list = new List<string>();
			Repository repository = null;
			foreach(var rev in revisions)
			{
				list.Add(rev.Pointer);
				repository = rev.Repository;
			}
			Verify.Argument.IsTrue(list.Count != 0, "revisions",
				Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("revision"));
			try
			{
				repository.Accessor.CherryPick.Invoke(new CherryPickParameters(list));
				repository.OnUpdated();
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
			CherryPick(revision, 0, false);
		}

		/// <summary>Performs a cherry-pick operation on <paramref name="revision"/>.</summary>
		/// <param name="revision">Commit to cherry-pick.</param>
		/// <param name="noCommit">Do not commit.</param>
		public static void CherryPick(this IRevisionPointer revision, bool noCommit)
		{
			CherryPick(revision, 0, noCommit);
		}

		/// <summary>Performs a cherry-pick operation on <paramref name="revision"/>.</summary>
		/// <param name="revision">Commit to cherry-pick.</param>
		/// <param name="mainline">Mainline parent commit.</param>
		/// <param name="noCommit">Do not commit.</param>
		public static void CherryPick(this IRevisionPointer revision, int mainline, bool noCommit)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");
			Verify.Argument.IsNotNegative(mainline, "mainline");
			var repository = revision.Repository;
			Verify.State.IsFalse(repository.Head.IsEmpty,
				Resources.ExcCantDoOnEmptyRepository.UseAsFormat("cherry-pick"));

			var rev = repository.Head.Revision;
			var cb = repository.Head.Pointer as Branch;
			var parameters = new CherryPickParameters(revision.Pointer, noCommit);
			if(mainline > 0)
			{
				parameters.Mainline = mainline;
			}
			try
			{
				using(repository.Monitor.BlockNotifications(
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated,
					RepositoryNotifications.BranchChanged,
					RepositoryNotifications.Checkout))
				{
					repository.Accessor.CherryPick.Invoke(parameters);
					if(cb != null)
					{
						cb.Refresh();
						var branchTip = cb.Revision;
						if(branchTip != rev)
						{
							repository.OnCommitCreated(branchTip);
						}
					}
					else
					{
						repository.Head.Refresh();
						var headRev = repository.Head.Revision;
						if(headRev != rev)
						{
							repository.OnCommitCreated(headRev);
						}
					}
					repository.Head.NotifyRelogRecordAdded();
				}
			}
			catch(GitException)
			{
				repository.OnStateChanged();
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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			repository.Head.Reset(revision, mode);
		}

		#endregion

		#region revert

		public static void Revert(this IRevisionPointer revision)
		{
			Revert(revision, 0, false);
		}

		public static void Revert(this IRevisionPointer revision, bool noCommit)
		{
			Revert(revision, 0, noCommit);
		}

		public static void Revert(this IRevisionPointer revision, int mainline, bool noCommit)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");
			Verify.Argument.IsNotNegative(mainline, "mainline");

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
				var parameters = new RevertParameters(revision.Pointer, noCommit);
				if(mainline > 0)
				{
					parameters.Mainline = mainline;
				}
				try
				{
					repository.Accessor.Revert.Invoke(parameters);
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
							repository.OnCommitCreated(headRev);
							repository.Head.NotifyRelogRecordAdded();
						}
					}
				}
				catch(GitException)
				{
					repository.OnStateChanged();
					throw;
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
			Verify.Argument.IsValidRevisionPointerSequence(revisions, "revisions");

			var list = new List<string>();
			Repository repository = null;
			foreach(var rev in revisions)
			{
				list.Add(rev.Pointer);
				repository = rev.Repository;
			}
			Verify.Argument.IsTrue(list.Count != 0, "revisions",
				Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("revision"));

			var oldHeadRev = repository.Head.Revision;
			try
			{
				repository.Accessor.Revert.Invoke(
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
						repository.OnUpdated();
						repository.Head.NotifyRelogRecordAdded();
					}
				}
			}
			catch(GitException)
			{
				repository.OnStateChanged();
				throw;
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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			repository.Head.Merge(revision, noCommit, noFastForward, squash, message);
		}

		public static void Merge(this IRevisionPointer revision, bool noCommit, bool noFastForward, bool squash)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			repository.Head.Merge(revision, noCommit, noFastForward, squash);
		}

		public static void Merge(this IRevisionPointer revision)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			repository.Head.Merge(revision);
		}

		#endregion

		#region rebase

		private static RebaseParameters GetRebaseParameters(IRevisionPointer revision)
		{
			Assert.IsNotNull(revision);

			return new RebaseParameters(revision.Pointer);
		}

		public static void RebaseHeadHere(this IRevisionPointer revision)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			var oldHead = repository.Head.CurrentBranch;
			var parameters = GetRebaseParameters(revision);
			using(repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				try
				{
					repository.Accessor.Rebase.Invoke(parameters);
				}
				finally
				{
					if(oldHead != null)
					{
						oldHead.Refresh();
					}
					else
					{
						repository.Head.Refresh();
					}
					repository.Status.Refresh();
				}
				repository.OnUpdated();
			}
		}

		public static Task RebaseHeadHereAsync(this IRevisionPointer revision, IProgress<OperationProgress> progress)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;
			var oldHead = repository.Head.CurrentBranch;
			var parameters = GetRebaseParameters(revision);
			var block = repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated);

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsRebaseIsInProcess.AddEllipsis()));
			}
			return repository.Accessor
				.Rebase.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					if(oldHead != null)
					{
						oldHead.Refresh();
					}
					else
					{
						repository.Head.Refresh();
					}
					TaskUtility.PropagateFaultedStates(t);
					repository.OnUpdated();
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region describe

		public static Tag Describe(this IRevisionPointer revision)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var repository = revision.Repository;

			var tag = repository.Accessor.Describe.Invoke(
				new DescribeParameters(revision.Pointer));
			if(tag != null)
			{
				return repository.Refs.Tags.TryGetItem(tag);
			}
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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");
			Verify.Argument.IsNeitherNullNorEmpty(message, "message");

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
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			return new Tree(revision.Repository, revision.FullName);
		}

		/// <summary>Gets <see cref="Tree"/> pointed by the specified <paramref name="revision"/>.</summary>
		/// <param name="revision">The revision.</param>
		/// <returns>Function which retrieves <see cref="Tree"/> pointed by the specified <paramref name="revision"/>.</returns>
		public static Task<Tree> GetTreeAsync(this IRevisionPointer revision, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			return Tree.GetAsync(revision.Repository, revision.FullName, progress, cancellationToken);
		}

		#endregion

		#region diff

		public static IRevisionDiffSource GetDiffSource(this IRevisionPointer revision, IEnumerable<string> paths = null)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var stashedState = revision as StashedState;
			if(stashedState != null)
			{
				if(paths == null)
				{
					return new StashedChangesDiffSource(stashedState);
				}
				else
				{
					return new StashedChangesDiffSource(stashedState, paths.ToList());
				}
			}
			else
			{
				if(paths == null)
				{
					return new RevisionChangesDiffSource(revision);
				}
				else
				{
					return new RevisionChangesDiffSource(revision, paths.ToList());
				}
			}
		}

		public static IDiffSource GetCompareDiffSource(this IRevisionPointer revision1, IRevisionPointer revision2, IEnumerable<string> paths = null)
		{
			Verify.Argument.AreValidRevisionPointers(revision1, revision2);

			if(paths == null)
			{
				return new RevisionCompareDiffSource(revision1, revision2);
			}
			else
			{
				return new RevisionCompareDiffSource(revision1, revision2, paths.ToList());
			}
		}

		#endregion

		#region FormatPatch

		private static QueryRevisionDiffParameters GetFormatPatchParameters(IRevisionPointer revision)
		{
			Assert.IsNotNull(revision);

			return new QueryRevisionDiffParameters(revision.Pointer)
				{
					EnableTextConvFilters = false,
					Binary = true
				};
		}

		private static IGitFunction<QueryRevisionDiffParameters, byte[]> GetFormatPatchFunction(IRevisionPointer revision)
		{
			Assert.IsNotNull(revision);

			if(revision.Type == ReferenceType.Stash)
			{
				return revision.Repository.Accessor.QueryStashPatch;
			}
			else
			{
				return revision.Repository.Accessor.QueryRevisionPatch;
			}
		}

		/// <summary>Get diff for this revision.</summary>
		/// <param name="revision">Revision to get diff for.</param>
		/// <returns><see cref="Diff"/> for this revision.</returns>
		/// <exception cref="T:gitter.Git.GitException">Failed to get diff.</exception>
		public static byte[] FormatPatch(this IRevisionPointer revision)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			var parameters = GetFormatPatchParameters(revision);
			var function   = GetFormatPatchFunction(revision);

			return function.Invoke(parameters);
		}

		/// <summary>Get diff for this revision.</summary>
		/// <param name="revision">Revision to get diff for.</param>
		/// <returns><see cref="Diff"/> for this revision.</returns>
		/// <exception cref="T:gitter.Git.GitException">Failed to get diff.</exception>
		public static Task<byte[]> FormatPatchAsync(this IRevisionPointer revision,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsFetchingPatch.AddEllipsis()));
			}
			var parameters = GetFormatPatchParameters(revision);
			var function   = GetFormatPatchFunction(revision);

			return function.InvokeAsync(parameters, progress, cancellationToken);
		}

		#endregion

		#region blame

		public static IBlameSource GetBlameSource(this IRevisionPointer revision, string fileName)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");

			return new RevisionFileBlameSource(revision, fileName);
		}

		#endregion

		#region archive

		private static ArchiveParameters GetArchiveParameters(IRevisionPointer revision, string outputFile, string path, string format)
		{
			return new ArchiveParameters()
			{
				Tree       = revision.FullName,
				Path       = path,
				OutputFile = outputFile,
				Format     = format,
			};
		}

		public static void Archive(this IRevisionPointer revision, string outputFile, string path, string format)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");
			Verify.Argument.IsNeitherNullNorWhitespace(outputFile, "outputFile");

			var parameters = GetArchiveParameters(revision, outputFile, path, format);
			revision.Repository.Accessor.Archive.Invoke(parameters);
		}

		public static Task ArchiveAsync(this IRevisionPointer revision, string outputFile, string path, string format, IProgress<OperationProgress> progress)
		{
			Verify.Argument.IsValidRevisionPointer(revision, "revision");
			Verify.Argument.IsNeitherNullNorWhitespace(outputFile, "outputFile");

			var parameters = GetArchiveParameters(revision, outputFile, path, format);
			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrfCreatingArchiveFrom.UseAsFormat(parameters.Tree).AddEllipsis()));
			}
			return revision.Repository.Accessor.Archive.InvokeAsync(
				parameters, progress, CancellationToken.None);
		}

		#endregion
	}
}
