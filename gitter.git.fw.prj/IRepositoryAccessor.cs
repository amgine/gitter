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

namespace gitter.Git.AccessLayer
{
	using System.Collections.Generic;

	/// <summary>Interface of repository accessor object.</summary>
	public interface IRepositoryAccessor : IConfigAccessor
	{
		/// <summary>Returns git accessor.</summary>
		/// <value>git accessor.</value>
		IGitAccessor GitAccessor { get; }

		/// <summary>Add file to index.</summary>
		IGitAction<AddFilesParameters> AddFiles { get; }

		/// <summary>Add remote repository.</summary>
		IGitAction<AddRemoteParameters> AddRemote { get; }

		/// <summary>Add new submodule.</summary>
		IGitAction<AddSubmoduleParameters> AddSubmodule { get; }

		/// <summary>Append new note to object.</summary>
		IGitAction<AppendNoteParameters> AppendNote { get; }

		/// <summary>Apply patches to working directory and/or index.</summary>
		IGitAction<ApplyPatchParameters> ApplyPatch { get; }

		/// <summary>Create an archive of files from a named tree.</summary>
		IGitAction<ArchiveParameters> Archive { get; }

		/// <summary>Remove untracked files from the working tree.</summary>
		IGitAction<CleanFilesParameters> CleanFiles { get; }

		/// <summary>Checkout branch/revision.</summary>
		IGitAction<CheckoutParameters> Checkout { get; }

		/// <summary>Checkout files from tree object to working directory.</summary>
		IGitAction<CheckoutFilesParameters> CheckoutFiles { get; }

		/// <summary>Performs a cherry-pick operation.</summary>
		IGitAction<CherryPickParameters> CherryPick { get; }

		/// <summary>Commit changes.</summary>
		IGitAction<CommitParameters> Commit { get; }

		/// <summary>Calculate object count.</summary>
		IGitFunction<CountObjectsParameters, ObjectCountData> CountObjects { get; }

		/// <summary>Create local branch.</summary>
		IGitAction<CreateBranchParameters> CreateBranch { get; }

		/// <summary>Create new tag object.</summary>
		IGitAction<CreateTagParameters> CreateTag { get; }

		/// <summary>Remove local branch.</summary>
		IGitAction<DeleteBranchParameters> DeleteBranch { get; }

		/// <summary>Delete tag.</summary>
		IGitAction<DeleteTagParameters> DeleteTag { get; }

		/// <summary>Dereference valid ref.</summary>
		IGitFunction<DereferenceParameters, RevisionData> Dereference { get; }

		/// <summary>Describe revision.</summary>
		IGitFunction<DescribeParameters, string> Describe { get; }

		/// <summary>Download objects and refs from another repository.</summary>
		IGitAction<FetchParameters> Fetch { get; }

		/// <summary>Formats merge message using commit messages.</summary>
		IGitFunction<FormatMergeMessageParameters, string> FormatMergeMessage { get; }

		/// <summary>Cleanup unnecessary files and optimize the local repository.</summary>
		IGitAction<GarbageCollectParameters> GarbageCollect { get; }

		/// <summary>Merge development histories together.</summary>
		IGitAction<MergeParameters> Merge { get; }

		/// <summary>Remove stale remote tracking branches.</summary>
		IGitAction<PruneRemoteParameters> PruneRemote { get; }

		/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
		IGitAction<PullParameters> Pull { get; }

		/// <summary>Update remote refs along with associated objects.</summary>
		IGitFunction<PushParameters, IList<ReferencePushResult>> Push { get; }

		/// <summary>Annotate each line of file with commit information.</summary>
		IGitFunction<QueryBlameParameters, BlameFile> QueryBlame { get; }

		/// <summary>Queries the BLOB bytes.</summary>
		IGitFunction<QueryBlobBytesParameters, byte[]> QueryBlobBytes { get; }

		/// <summary>Check if branch exists and get its position.</summary>
		IGitFunction<QueryBranchParameters, BranchData> QueryBranch { get; }

		/// <summary>Query branch list.</summary>
		IGitFunction<QueryBranchesParameters, BranchesData> QueryBranches { get; }

		/// <summary>Get list of stale remote tracking branches that are subject to pruninig.</summary>
		IGitFunction<PruneRemoteParameters, IList<string>> QueryBranchesToPrune { get; }

		/// <summary>Get <see cref="Diff"/>, representing difference between specified objects.</summary>
		IGitFunction<QueryDiffParameters, Diff> QueryDiff { get; }

		/// <summary>Get the list of files that can be added.</summary>
		IGitFunction<AddFilesParameters, IList<TreeFileData>> QueryFilesToAdd { get; }

		/// <summary>Get list of files which will be removed by a remove files call.</summary>
		IGitFunction<RemoveFilesParameters, IList<string>> QueryFilesToRemove { get; }

		/// <summary>Get list of files and directories which will be removed by a clean call.</summary>
		IGitFunction<CleanFilesParameters, IList<string>> QueryFilesToClean { get; }

		/// <summary>Get list of all note objects.</summary>
		IGitFunction<QueryNotesParameters, IList<NoteData>> QueryNotes { get; }

		/// <summary>Get contents of requested objects.</summary>
		IGitFunction<QueryObjectsParameters, string> QueryObjects { get; }

		/// <summary>Get revision information.</summary>
		IGitFunction<QueryRevisionParameters, RevisionData> QueryRevision { get; }

		/// <summary>Get revision list.</summary>
		IGitFunction<QueryRevisionsParameters, IList<RevisionData>> QueryRevisions { get; }

		/// <summary>Get <see cref="Diff"/>, representing changes made by specified commit.</summary>
		IGitFunction<QueryRevisionDiffParameters, Diff> QueryRevisionDiff { get; }

		/// <summary>Get revision graph.</summary>
		IGitFunction<QueryRevisionsParameters, IList<RevisionGraphData>> QueryRevisionGraph { get; }

		/// <summary>Get patch representing changes made by specified commit.</summary>
		IGitFunction<QueryRevisionDiffParameters, byte[]> QueryRevisionPatch { get; }

		/// <summary>Get list of references.</summary>
		IGitFunction<QueryReferencesParameters, ReferencesData> QueryReferences { get; }

		/// <summary>Get reference reflog.</summary>
		IGitFunction<QueryReflogParameters, IList<ReflogRecordData>> QueryReflog { get; }

		/// <summary>Get information about remote.</summary>
		IGitFunction<QueryRemoteParameters, RemoteData> QueryRemote { get; }

		/// <summary>Get list of references on remote repository.</summary>
		IGitFunction<QueryRemoteReferencesParameters, IList<RemoteReferenceData>> QueryRemoteReferences { get; }

		/// <summary>Query list of remotes.</summary>
		IGitFunction<QueryRemotesParameters, IList<RemoteData>> QueryRemotes { get; }

		/// <summary>Query all stashed states.</summary>
		IGitFunction<QueryStashParameters, IList<StashedStateData>> QueryStash { get; }

		/// <summary>Get patch representing stashed changes.</summary>
		IGitFunction<QueryRevisionDiffParameters, byte[]> QueryStashPatch { get; }

		/// <summary>Query most recent stashed state.</summary>
		IGitFunction<QueryStashTopParameters, RevisionData> QueryStashTop { get; }

		/// <summary>Get patch representing stashed changes.</summary>
		IGitFunction<QueryRevisionDiffParameters, Diff> QueryStashDiff { get; }

		/// <summary>Get working directory status information.</summary>
		IGitFunction<QueryStatusParameters, StatusData> QueryStatus { get; }

		/// <summary>Get symbolic reference target.</summary>
		IGitFunction<QuerySymbolicReferenceParameters, SymbolicReferenceData> QuerySymbolicReference { get; }

		/// <summary>Get objects contained in a tree.</summary>
		IGitFunction<QueryTreeContentParameters, IList<TreeContentData>> QueryTreeContent { get; }

		/// <summary>Check if tag exists and get its position.</summary>
		IGitFunction<QueryTagParameters, TagData> QueryTag { get; }

		/// <summary>Query tag message.</summary>
		IGitFunction<QueryTagMessageParameters, string> QueryTagMessage { get; }

		/// <summary>Query tag list.</summary>
		IGitFunction<QueryTagsParameters, IList<TagData>> QueryTags { get; }

		/// <summary>Get user list.</summary>
		IGitFunction<QueryUsersParameters, IList<UserData>> QueryUsers { get; }

		/// <summary>Remove file from index and/or working directory.</summary>
		IGitAction<RemoveFilesParameters> RemoveFiles { get; }

		/// <summary>Remove reference on remote repository.</summary>
		IGitAction<RemoveRemoteReferencesParameters> RemoveRemoteReferences { get; }

		/// <summary>Forward-port local commits to the updated upstream head.</summary>
		IGitAction<RebaseParameters> Rebase { get; }

		/// <summary>Remove remote repository.</summary>
		IGitAction<RemoveRemoteParameters> RemoveRemote { get; }

		/// <summary>Rename local branch.</summary>
		IGitAction<RenameBranchParameters> RenameBranch { get; }

		/// <summary>Rename remote repository.</summary>
		IGitAction<RenameRemoteParameters> RenameRemote { get; }

		/// <summary>Reset HEAD.</summary>
		IGitAction<ResetParameters> Reset { get; }

		/// <summary>Resets files.</summary>
		IGitAction<ResetFilesParameters> ResetFiles { get; }

		/// <summary>Reset local branch.</summary>
		IGitAction<ResetBranchParameters> ResetBranch { get; }

		/// <summary>Performs a revert operation.</summary>
		IGitAction<RevertParameters> Revert { get; }

		/// <summary>Run merge tool to resolve conflicts.</summary>
		IGitAction<RunMergeToolParameters> RunMergeTool { get; }

		/// <summary>Apply stashed changes and do not remove stashed state.</summary>
		IGitAction<StashApplyParameters> StashApply { get; }

		/// <summary>Remove stashed state.</summary>
		IGitAction<StashDropParameters> StashDrop { get; }

		/// <summary>Clear stash.</summary>
		IGitAction<StashClearParameters> StashClear { get; }

		/// <summary>Apply stashed changes and remove stashed state.</summary>
		IGitAction<StashPopParameters> StashPop { get; }

		/// <summary>Stash changes in working directory.</summary>
		IGitFunction<StashSaveParameters, bool> StashSave { get; }

		/// <summary>Create new branch, checkout that branch and pop stashed state.</summary>
		IGitAction<StashToBranchParameters> StashToBranch { get; }

		/// <summary>Updates submodule.</summary>
		IGitAction<SubmoduleUpdateParameters> UpdateSubmodule { get; }

		/// <summary>Verify tags GPG signatures.</summary>
		IGitAction<VerifyTagsParameters> VerifyTags { get; }
	}
}
