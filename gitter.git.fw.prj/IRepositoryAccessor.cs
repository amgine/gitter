namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Interface of repository accessor object.</summary>
	public interface IRepositoryAccessor :
		IIndexAccessor,
		IBranchAccessor,
		ITagAccessor,
		IStashAccessor,
		INotesAccessor,
		IRemoteAccessor,
		ISubmoduleAccessor,
		ITreeAccessor,
		IConfigAccessor
	{
		/// <summary>Returns git accessor.</summary>
		/// <value>git accessor.</value>
		IGitAccessor GitAccessor { get; }

		/// <summary>Create an archive of files from a named tree.</summary>
		/// <param name="parameters"><see cref="ArchiveParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void Archive(ArchiveParameters parameters);

		/// <summary>Get symbolic reference target.</summary>
		/// <param name="parameters"><see cref="QuerySymbolicReferenceParameters"/>.</param>
		/// <returns>Symbolic reference data.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		SymbolicReferenceData QuerySymbolicReference(QuerySymbolicReferenceParameters parameters);

		/// <summary>Get referenced revision.</summary>
		/// <param name="parameters"><see cref="DereferenceParameters"/>.</param>
		/// <returns>Corresponding <see cref="RevisionData"/>.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		RevisionData Dereference(DereferenceParameters parameters);

		///	<summary>Describes revision with tag.</summary>
		/// <param name="parameters"><see cref="DescribeParameters"/>.</param>
		/// <returns>Tag name.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		string Describe(DescribeParameters parameters);

		/// <summary>Get contents of requested objects.</summary>
		/// <param name="parameters"><see cref="QueryObjectsParameters"/>.</param>
		/// <returns>Objects contents.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		string QueryObjects(QueryObjectsParameters parameters);

		/// <summary>Get list of references.</summary>
		/// <param name="parameters"><see cref="QueryReferencesParameters"/>.</param>
		/// <returns>Lists of references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		ReferencesData QueryReferences(QueryReferencesParameters parameters);

		/// <summary>Get revision list.</summary>
		/// <param name="parameters"><see cref="QueryRevisionsParameters"/>.</param>
		/// <returns>List of revisions.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<RevisionData> QueryRevisions(QueryRevisionsParameters parameters);

		/// <summary>Get revision graph.</summary>
		/// <param name="parameters"><see cref="QueryRevisionsParameters"/>.</param>
		/// <returns>Revision graph.</returns>
		IList<RevisionGraphData> QueryRevisionGraph(QueryRevisionsParameters parameters);

		/// <summary>Get reflog.</summary>
		/// <param name="parameters"><see cref="QueryReflogParameters"/>.</param>
		/// <returns>List of reflog records.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<ReflogRecordData> QueryReflog(QueryReflogParameters parameters);
		
		/// <summary>Get revision information.</summary>
		/// <param name="parameters"><see cref="QueryRevisionParameters"/>.</param>
		/// <returns>Revision data.</returns>
		RevisionData QueryRevision(QueryRevisionParameters parameters);

		/// <summary>Get user list.</summary>
		/// <param name="parameters"><see cref="QueryUsersParameters"/>.</param>
		/// <returns>List of committers and authors.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<UserData> QueryUsers(QueryUsersParameters parameters);

		/// <summary>Checkout branch/revision.</summary>
		/// <param name="parameters"><see cref="CheckoutParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void Checkout(CheckoutParameters parameters);

		/// <summary>Reset HEAD and/or index.</summary>
		/// <param name="parameters"><see cref="ResetParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void Reset(ResetParameters parameters);

		/// <summary>Performs a cherry-pick operation.</summary>
		/// <param name="parameters"><see cref="CherryPickParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, cherry-pick is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, cherry-pick cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, unable to cherry-pick.</exception>
		/// <exception cref="T:gitter.Git.CherryPickIsEmptyException">Resulting cherry-pick is empty.</exception>
		/// <exception cref="T:gitter.Git.AutomaticCherryPickFailedException">Cherry-pick was not finished because of conflicts.</exception>
		void CherryPick(CherryPickParameters parameters);

		/// <summary>Performs a cherry-pick operation.</summary>
		/// <param name="control">Sequencer command to execute.</param>
		void CherryPick(CherryPickControl control);

		/// <summary>Performs a revert operation.</summary>
		/// <param name="parameters"><see cref="RevertParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, revert is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, revert cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, unable to revert.</exception>
		void Revert(RevertParameters parameters);

		/// <summary>Executes revert sequencer subcommand.</summary>
		/// <param name="control">Operation to execute.</param>
		void Revert(RevertControl control);

		/// <summary>Forward-port local commits to the updated upstream head.</summary>
		/// <param name="parameters"><see cref="RebaseParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void Rebase(RebaseParameters parameters);

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Rebase control option.</param>
		void Rebase(RebaseControl control);

		/// <summary>Prepare merge message.</summary>
		/// <param name="parameters"><see cref="FormatMergeMessageParameters"/>.</param>
		/// <returns>Merge message.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		string FormatMergeMessage(FormatMergeMessageParameters parameters);

		/// <summary>Merge development histories together.</summary>
		/// <param name="parameters"><see cref="MergeParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.AutomaticMergeFailedException">Merge resulted in conflicts.</exception>
		void Merge(MergeParameters parameters);

		/// <summary>Get patch representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns>Patch, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		string QueryRevisionPatch(QueryRevisionDiffParameters parameters);

		/// <summary>Get <see cref="Diff"/>, representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Diff QueryRevisionDiff(QueryRevisionDiffParameters parameters);

		/// <summary>Get <see cref="Diff"/>, representing difference between specified objects.</summary>
		/// <param name="parameters"><see cref="QueryDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing difference between requested objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Diff QueryDiff(QueryDiffParameters parameters);

		/// <summary>Get <see cref="BlameFile"/>, annotating each line of file with commit information.</summary>
		/// <param name="parameters"><see cref="QueryBlameParameters"/>.</param>
		/// <returns><see cref="BlameFile"/>, annotating each line of file with commit information.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		BlameFile QueryBlame(QueryBlameParameters parameters);

		/// <summary>Cleanup unnecessary files and optimize the local repository.</summary>
		/// <param name="parameters"><see cref="GarbageCollectParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void GarbageCollect(GarbageCollectParameters parameters);
	}
}
