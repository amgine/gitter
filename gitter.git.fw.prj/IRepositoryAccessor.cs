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

namespace gitter.Git.AccessLayer;

using System.Collections.Generic;

using gitter.Framework;

/// <summary>Interface of repository accessor object.</summary>
public interface IRepositoryAccessor : IConfigAccessor
{
	/// <summary>Returns git accessor.</summary>
	/// <value>git accessor.</value>
	IGitAccessor GitAccessor { get; }

	/// <summary>Add file to index.</summary>
	IGitAction<AddFilesRequest> AddFiles { get; }

	/// <summary>Add remote repository.</summary>
	IGitAction<AddRemoteRequest> AddRemote { get; }

	/// <summary>Add new submodule.</summary>
	IGitAction<AddSubmoduleRequest> AddSubmodule { get; }

	/// <summary>Append new note to object.</summary>
	IGitAction<AppendNoteRequest> AppendNote { get; }

	/// <summary>Apply patches to working directory and/or index.</summary>
	IGitAction<ApplyPatchRequest> ApplyPatch { get; }

	/// <summary>Create an archive of files from a named tree.</summary>
	IGitAction<ArchiveRequest> Archive { get; }

	/// <summary>Remove untracked files from the working tree.</summary>
	IGitAction<CleanFilesRequest> CleanFiles { get; }

	/// <summary>Checkout branch/revision.</summary>
	IGitAction<CheckoutRequest> Checkout { get; }

	/// <summary>Checkout files from tree object to working directory.</summary>
	IGitAction<CheckoutFilesRequest> CheckoutFiles { get; }

	/// <summary>Performs a cherry-pick operation.</summary>
	IGitAction<CherryPickRequest> CherryPick { get; }

	/// <summary>Commit changes.</summary>
	IGitFunction<CommitRequest, string> Commit { get; }

	/// <summary>Calculate object count.</summary>
	IGitFunction<CountObjectsRequest, ObjectCountData> CountObjects { get; }

	/// <summary>Create local branch.</summary>
	IGitAction<CreateBranchRequest> CreateBranch { get; }

	/// <summary>Create new tag object.</summary>
	IGitAction<CreateTagRequest> CreateTag { get; }

	/// <summary>Remove local branch.</summary>
	IGitAction<DeleteBranchRequest> DeleteBranch { get; }

	/// <summary>Delete tag.</summary>
	IGitAction<DeleteTagRequest> DeleteTag { get; }

	/// <summary>Dereference valid ref.</summary>
	IGitFunction<DereferenceRequest, RevisionData> Dereference { get; }

	/// <summary>Describe revision.</summary>
	IGitFunction<DescribeRequest, string?> Describe { get; }

	/// <summary>Download objects and refs from another repository.</summary>
	IGitAction<FetchRequest> Fetch { get; }

	/// <summary>Formats merge message using commit messages.</summary>
	IGitFunction<FormatMergeMessageRequest, string> FormatMergeMessage { get; }

	/// <summary>Cleanup unnecessary files and optimize the local repository.</summary>
	IGitAction<GarbageCollectRequest> GarbageCollect { get; }

	/// <summary>Merge development histories together.</summary>
	IGitAction<MergeRequest> Merge { get; }

	/// <summary>Remove stale remote tracking branches.</summary>
	IGitAction<PruneRemoteRequest> PruneRemote { get; }

	/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
	IGitAction<PullRequest> Pull { get; }

	/// <summary>Update remote refs along with associated objects.</summary>
	IGitFunction<PushRequest, Many<ReferencePushResult>> Push { get; }

	/// <summary>Annotate each line of file with commit information.</summary>
	IGitFunction<QueryBlameRequest, BlameFile> QueryBlame { get; }

	/// <summary>Queries the BLOB bytes.</summary>
	IGitFunction<QueryBlobBytesRequest, byte[]> QueryBlobBytes { get; }

	/// <summary>Check if branch exists and get its position.</summary>
	IGitFunction<QueryBranchRequest, BranchData?> QueryBranch { get; }

	/// <summary>Query branch list.</summary>
	IGitFunction<QueryBranchesRequest, BranchesData> QueryBranches { get; }

	/// <summary>Get list of stale remote tracking branches that are subject to pruning.</summary>
	IGitFunction<PruneRemoteRequest, IList<string>> QueryBranchesToPrune { get; }

	/// <summary>Get <see cref="Diff"/>, representing difference between specified objects.</summary>
	IGitFunction<QueryDiffRequest, Diff> QueryDiff { get; }

	/// <summary>Get the list of files that can be added.</summary>
	IGitFunction<AddFilesRequest, IList<TreeFileData>> QueryFilesToAdd { get; }

	/// <summary>Get list of files which will be removed by a remove files call.</summary>
	IGitFunction<RemoveFilesRequest, IList<string>> QueryFilesToRemove { get; }

	/// <summary>Get list of files and directories which will be removed by a clean call.</summary>
	IGitFunction<CleanFilesRequest, IList<string>> QueryFilesToClean { get; }

	/// <summary>Get list of all note objects.</summary>
	IGitFunction<QueryNotesRequest, IList<NoteData>> QueryNotes { get; }

	/// <summary>Get contents of requested objects.</summary>
	IGitFunction<QueryObjectsRequest, string> QueryObjects { get; }

	/// <summary>Get revision information.</summary>
	IGitFunction<QueryRevisionRequest, RevisionData> QueryRevision { get; }

	/// <summary>Get revision list.</summary>
	IGitFunction<QueryRevisionsRequest, IList<RevisionData>> QueryRevisions { get; }

	/// <summary>Get <see cref="Diff"/>, representing changes made by specified commit.</summary>
	IGitFunction<QueryRevisionDiffRequest, Diff> QueryRevisionDiff { get; }

	/// <summary>Get revision graph.</summary>
	IGitFunction<QueryRevisionsRequest, IList<RevisionGraphData>> QueryRevisionGraph { get; }

	/// <summary>Get patch representing changes made by specified commit.</summary>
	IGitFunction<QueryRevisionDiffRequest, byte[]> QueryRevisionPatch { get; }

	/// <summary>Get list of references.</summary>
	IGitFunction<QueryReferencesRequest, ReferencesData> QueryReferences { get; }

	/// <summary>Get reference reflog.</summary>
	IGitFunction<QueryReflogRequest, IList<ReflogRecordData>> QueryReflog { get; }

	/// <summary>Get information about remote.</summary>
	IGitFunction<QueryRemoteRequest, RemoteData> QueryRemote { get; }

	/// <summary>Get list of references on remote repository.</summary>
	IGitFunction<QueryRemoteReferencesRequest, IList<RemoteReferenceData>> QueryRemoteReferences { get; }

	/// <summary>Query list of remotes.</summary>
	IGitFunction<QueryRemotesRequest, IList<RemoteData>> QueryRemotes { get; }

	/// <summary>Query all stashed states.</summary>
	IGitFunction<QueryStashRequest, IList<StashedStateData>> QueryStash { get; }

	/// <summary>Get patch representing stashed changes.</summary>
	IGitFunction<QueryRevisionDiffRequest, byte[]> QueryStashPatch { get; }

	/// <summary>Query most recent stashed state.</summary>
	IGitFunction<QueryStashTopRequest, RevisionData?> QueryStashTop { get; }

	/// <summary>Get patch representing stashed changes.</summary>
	IGitFunction<QueryRevisionDiffRequest, Diff> QueryStashDiff { get; }

	/// <summary>Get working directory status information.</summary>
	IGitFunction<QueryStatusRequest, StatusData> QueryStatus { get; }

	/// <summary>Get symbolic reference target.</summary>
	IGitFunction<QuerySymbolicReferenceRequest, SymbolicReferenceData> QuerySymbolicReference { get; }

	/// <summary>Get objects contained in a tree.</summary>
	IGitFunction<QueryTreeContentRequest, IList<TreeContentData>> QueryTreeContent { get; }

	/// <summary>Check if tag exists and get its position.</summary>
	IGitFunction<QueryTagRequest, TagData?> QueryTag { get; }

	/// <summary>Query tag message.</summary>
	IGitFunction<QueryTagMessageRequest, string> QueryTagMessage { get; }

	/// <summary>Query tag list.</summary>
	IGitFunction<QueryTagsRequest, IList<TagData>> QueryTags { get; }

	/// <summary>Get user list.</summary>
	IGitFunction<QueryUsersRequest, IList<UserData>> QueryUsers { get; }

	/// <summary>Remove file from index and/or working directory.</summary>
	IGitAction<RemoveFilesRequest> RemoveFiles { get; }

	/// <summary>Remove reference on remote repository.</summary>
	IGitAction<RemoveRemoteReferencesRequest> RemoveRemoteReferences { get; }

	/// <summary>Forward-port local commits to the updated upstream head.</summary>
	IGitAction<RebaseRequest> Rebase { get; }

	/// <summary>Remove remote repository.</summary>
	IGitAction<RemoveRemoteRequest> RemoveRemote { get; }

	/// <summary>Rename local branch.</summary>
	IGitAction<RenameBranchRequest> RenameBranch { get; }

	/// <summary>Rename remote repository.</summary>
	IGitAction<RenameRemoteRequest> RenameRemote { get; }

	/// <summary>Reset HEAD.</summary>
	IGitAction<ResetRequest> Reset { get; }

	/// <summary>Resets files.</summary>
	IGitAction<ResetFilesRequest> ResetFiles { get; }

	/// <summary>Reset local branch.</summary>
	IGitAction<ResetBranchRequest> ResetBranch { get; }

	/// <summary>Performs a revert operation.</summary>
	IGitAction<RevertRequest> Revert { get; }

	/// <summary>Run merge tool to resolve conflicts.</summary>
	IGitAction<RunMergeToolRequest> RunMergeTool { get; }

	/// <summary>Apply stashed changes and do not remove stashed state.</summary>
	IGitAction<StashApplyRequest> StashApply { get; }

	/// <summary>Remove stashed state.</summary>
	IGitAction<StashDropRequest> StashDrop { get; }

	/// <summary>Clear stash.</summary>
	IGitAction<StashClearRequest> StashClear { get; }

	/// <summary>Apply stashed changes and remove stashed state.</summary>
	IGitAction<StashPopRequest> StashPop { get; }

	/// <summary>Stash changes in working directory.</summary>
	IGitFunction<StashSaveRequest, bool> StashSave { get; }

	/// <summary>Create new branch, checkout that branch and pop stashed state.</summary>
	IGitAction<StashToBranchRequest> StashToBranch { get; }

	/// <summary>Updates submodule.</summary>
	IGitAction<UpdateSubmoduleRequest> UpdateSubmodule { get; }

	/// <summary>Synchronizes submodule.</summary>
	IGitAction<SyncSubmoduleRequest> SyncSubmodule { get; }

	/// <summary>Verify tags GPG signatures.</summary>
	IGitAction<VerifyTagsRequest> VerifyTags { get; }
}
