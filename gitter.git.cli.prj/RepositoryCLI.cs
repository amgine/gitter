#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System;
using System.Collections.Generic;

using gitter.Framework;
using gitter.Git.AccessLayer.CLI;

/// <summary>Accesses repository through git command line interface.</summary>
internal sealed partial class RepositoryCLI : IRepositoryAccessor
{
	#region Data

	private readonly GitCLI _gitCLI;
	private readonly IGitRepository _repository;
	private readonly ICommandExecutor _executor;

	private readonly IGitAction   < AddConfigValueRequest                                     > _addConfigValue;
	private readonly IGitAction   < AddFilesRequest                                           > _addFiles;
	private readonly IGitAction   < AddRemoteRequest                                          > _addRemote;
	private readonly IGitAction   < AddSubmoduleRequest                                       > _addSubmodule;
	private readonly IGitAction   < AppendNoteRequest                                         > _appendNote;
	private readonly IGitAction   < ApplyPatchRequest                                         > _applyPatch;
	private readonly IGitAction   < ArchiveRequest                                            > _archive;
	private readonly IGitAction   < CleanFilesRequest                                         > _cleanFiles;
	private readonly IGitAction   < CheckoutRequest                                           > _checkout;
	private readonly IGitAction   < CheckoutFilesRequest                                      > _checkoutFiles;
	private readonly IGitAction   < CherryPickRequest                                         > _cherryPick;
	private readonly IGitFunction < CommitRequest,                 string                     > _commit;
	private readonly IGitFunction < CountObjectsRequest,           ObjectCountData            > _countObjects;
	private readonly IGitAction   < CreateBranchRequest                                       > _createBranch;
	private readonly IGitAction   < CreateTagRequest                                          > _createTag;
	private readonly IGitAction   < DeleteBranchRequest                                       > _deleteBranch;
	private readonly IGitAction   < DeleteConfigSectionRequest                                > _deleteConfigSection;
	private readonly IGitAction   < DeleteTagRequest                                          > _deleteTag;
	private readonly IGitFunction < DereferenceRequest,            RevisionData               > _dereference;
	private readonly IGitFunction < DescribeRequest,               string?                    > _describe;
	private readonly IGitAction   < FetchRequest                                              > _fetch;
	private readonly IGitFunction < FormatMergeMessageRequest,     string                     > _formatMergeMessage;
	private readonly IGitAction   < GarbageCollectRequest                                     > _garbageCollect;
	private readonly IGitAction   < MergeRequest                                              > _merge;
	private readonly IGitAction   < PruneNotesRequest                                         > _pruneNotes;
	private readonly IGitAction   < PruneRemoteRequest                                        > _pruneRemote;
	private readonly IGitAction   < PullRequest                                               > _pull;
	private readonly IGitFunction < PushRequest,                   Many<ReferencePushResult>  > _push;
	private readonly IGitFunction < QueryBlameRequest,             BlameFile                  > _queryBlame;
	private readonly IGitFunction < QueryBlobBytesRequest,         byte[]                     > _queryBlobBytes;
	private readonly IGitFunction < QueryBranchRequest,            BranchData?                > _queryBranch;
	private readonly IGitFunction < QueryBranchesRequest,          BranchesData               > _queryBranches;
	private readonly IGitFunction < PruneRemoteRequest,            IList<string>              > _queryBranchesToPrune;
	private readonly IGitFunction < QueryConfigRequest,            IList<ConfigParameterData> > _queryConfig;
	private readonly IGitFunction < QueryConfigParameterRequest,   ConfigParameterData?       > _queryConfigParameter;
	private readonly IGitFunction < QueryDiffRequest,              Diff                       > _queryDiff;
	private readonly IGitFunction < AddFilesRequest,               IList<TreeFileData>        > _queryFilesToAdd;
	private readonly IGitFunction < RemoveFilesRequest,            IList<string>              > _queryFilesToRemove;
	private readonly IGitFunction < CleanFilesRequest,             IList<string>              > _queryFilesToClean;
	private readonly IGitFunction < QueryNotesRequest,             IList<NoteData>            > _queryNotes;
	private readonly IGitFunction < QueryObjectsRequest,           string                     > _queryObjects;
	private readonly IGitFunction < QueryRevisionRequest,          RevisionData               > _queryRevision;
	private readonly IGitFunction < QueryRevisionsRequest,         IList<RevisionData>        > _queryRevisions;
	private readonly IGitFunction < QueryRevisionsRequest,         IList<RevisionGraphData>   > _queryRevisionGraph;
	private readonly IGitFunction < QueryRevisionDiffRequest,      byte[]                     > _queryRevisionPatch;
	private readonly IGitFunction < QueryReferencesRequest,        ReferencesData             > _queryReferences;
	private readonly IGitFunction < QueryReflogRequest,            IList<ReflogRecordData>    > _queryReflog;
	private readonly IGitFunction < QueryRemoteRequest,            RemoteData                 > _queryRemote;
	private readonly IGitFunction < QueryRemoteReferencesRequest,  IList<RemoteReferenceData> > _queryRemoteReferences;
	private readonly IGitFunction < QueryRemotesRequest,           IList<RemoteData>          > _queryRemotes;
	private readonly IGitFunction < QueryRevisionDiffRequest,      Diff                       > _queryRevisionDiff;
	private readonly IGitFunction < QueryStashRequest,             IList<StashedStateData>    > _queryStash;
	private readonly IGitFunction < QueryRevisionDiffRequest,      byte[]                     > _queryStashPatch;
	private readonly IGitFunction < QueryStashTopRequest,          RevisionData?              > _queryStashTop;
	private readonly IGitFunction < QueryRevisionDiffRequest,      Diff                       > _queryStashDiff;
	private readonly IGitFunction < QueryStatusRequest,            StatusData                 > _queryStatus;
	private readonly IGitFunction < QuerySymbolicReferenceRequest, SymbolicReferenceData      > _querySymbolicReference;
	private readonly IGitFunction < QueryTreeContentRequest,       IList<TreeContentData>     > _queryTreeContent;
	private readonly IGitFunction < QueryTagRequest,               TagData?                   > _queryTag;
	private readonly IGitFunction < QueryTagMessageRequest,        string                     > _queryTagMessage;
	private readonly IGitFunction < QueryTagsRequest,              IList<TagData>             > _queryTags;
	private readonly IGitFunction < QueryUsersRequest,             IList<UserData>            > _queryUsers;
	private readonly IGitAction   < RemoveFilesRequest                                        > _removeFiles;
	private readonly IGitAction   < RemoveRemoteReferencesRequest                             > _removeRemoteReferences;
	private readonly IGitAction   < RebaseRequest                                             > _rebase;
	private readonly IGitAction   < RemoveRemoteRequest                                       > _removeRemote;
	private readonly IGitAction   < RenameConfigSectionRequest                                > _renameConfigSection;
	private readonly IGitAction   < RenameBranchRequest                                       > _renameBranch;
	private readonly IGitAction   < RenameRemoteRequest                                       > _renameRemote;
	private readonly IGitAction   < ResetRequest                                              > _reset;
	private readonly IGitAction   < ResetFilesRequest                                         > _resetFiles;
	private readonly IGitAction   < ResetBranchRequest                                        > _resetBranch;
	private readonly IGitAction   < RevertRequest                                             > _revert;
	private readonly IGitAction   < RunMergeToolRequest                                       > _runMergeTool;
	private readonly IGitAction   < SetConfigValueRequest                                     > _setConfigValue;
	private readonly IGitAction   < StashApplyRequest                                         > _stashApply;
	private readonly IGitAction   < StashDropRequest                                          > _stashDrop;
	private readonly IGitAction   < StashClearRequest                                         > _stashClear;
	private readonly IGitAction   < StashPopRequest                                           > _stashPop;
	private readonly IGitFunction < StashSaveRequest,              bool                       > _stashSave;
	private readonly IGitAction   < StashToBranchRequest                                      > _stashToBranch;
	private readonly IGitAction   < UnsetConfigValueRequest                                   > _unsetConfigValue;
	private readonly IGitAction   < UpdateSubmoduleRequest                                    > _updateSubmodule;
	private readonly IGitAction   < SyncSubmoduleRequest                                      > _syncSubmodule;
	private readonly IGitAction   < VerifyTagsRequest                                         > _verifyTags;

	#endregion

	#region .ctor

	public RepositoryCLI(GitCLI gitCLI, IGitRepository repository)
	{
		Verify.Argument.IsNotNull(gitCLI);
		Verify.Argument.IsNotNull(repository);

		_gitCLI     = gitCLI;
		_repository = repository;
		_executor   = new RepositoryCommandExecutor(gitCLI, repository.WorkingDirectory);

		GitCliMethod.Create(out _addConfigValue,         CommandExecutor, CommandBuilder.GetAddConfigValueCommand,         OutputParser.HandleConfigResults);
		GitCliMethod.Create(out _addFiles,               CommandExecutor, CommandBuilder.GetAddFilesCommand);
		GitCliMethod.Create(out _addRemote,              CommandExecutor, CommandBuilder.GetAddRemoteCommand);
		GitCliMethod.Create(out _addSubmodule,           CommandExecutor, CommandBuilder.GetAddSubmoduleCommand);
		GitCliMethod.Create(out _appendNote,             CommandExecutor, CommandBuilder.GetAppendNoteCommand);
		GitCliMethod.Create(out _applyPatch,             CommandExecutor, CommandBuilder.GetApplyPatchCommand);
		GitCliMethod.Create(out _archive,                CommandExecutor, CommandBuilder.GetArchiveCommand);
		GitCliMethod.Create(out _cleanFiles,             CommandExecutor, CommandBuilder.GetCleanFilesCommand);
		GitCliMethod.Create(out _checkout,               CommandExecutor, CommandBuilder.GetCheckoutCommand,               OutputParser.HandleCheckoutResult);
		GitCliMethod.Create(out _checkoutFiles,          CommandExecutor, CommandBuilder.GetCheckoutFilesCommand);
		GitCliMethod.Create(out _cherryPick,             CommandExecutor, CommandBuilder.GetCherryPickCommand,             OutputParser.HandleCherryPickResult);
		GitCliMethod.Create(out _commit,                 CommandExecutor, CommandBuilder.GetCommitCommand,                 OutputParser.ParseCommitResult);
		GitCliMethod.Create(out _countObjects,           CommandExecutor, CommandBuilder.GetCountObjectsCommand,           OutputParser.ParseObjectCountData);
		GitCliMethod.Create(out _createBranch,           CommandExecutor, CommandBuilder.GetCreateBranchCommand,           OutputParser.HandleCreateBranchResult);
		GitCliMethod.Create(out _createTag,              CommandExecutor, CommandBuilder.GetCreateTagCommand,              OutputParser.HandleCreateTagResult);
		GitCliMethod.Create(out _deleteBranch,           CommandExecutor, CommandBuilder.GetDeleteBranchCommand,           OutputParser.HandleDeleteBranchResult);
		GitCliMethod.Create(out _deleteConfigSection,    CommandExecutor, CommandBuilder.GetDeleteConfigSectionCommand,    OutputParser.HandleConfigResults);
		GitCliMethod.Create(out _deleteTag,              CommandExecutor, CommandBuilder.GetDeleteTagCommand,              OutputParser.HandleDeleteTagResult);
		GitCliMethod.Create(out _dereference,            CommandExecutor, CommandBuilder.GetDereferenceCommand,            OutputParser.ParseDereferenceOutput);
		GitCliMethod.Create(out _describe,               CommandExecutor, CommandBuilder.GetDescribeCommand,               OutputParser.ParseDescribeResult);
		GitCliMethod.Create(out _fetch,                  CommandExecutor, CommandBuilder.GetFetchCommand);
		GitCliMethod.Create(out _formatMergeMessage,     CommandExecutor);
		GitCliMethod.Create(out _garbageCollect,         CommandExecutor, CommandBuilder.GetGarbageCollectCommand);
		GitCliMethod.Create(out _merge,                  CommandExecutor, CommandBuilder.GetMergeCommand,                  OutputParser.HandleMergeResult);
		GitCliMethod.Create(out _pruneNotes,             CommandExecutor, CommandBuilder.GetPruneNotesCommand);
		GitCliMethod.Create(out _pruneRemote,            CommandExecutor, CommandBuilder.GetPruneRemoteCommand);
		GitCliMethod.Create(out _pull,                   CommandExecutor, CommandBuilder.GetPullCommand);
		GitCliMethod.Create(out _push,                   CommandExecutor, CommandBuilder.GetPushCommand,                   OutputParser.ParsePushResults);
		GitCliMethod.Create(out _queryBlame,             CommandExecutor, CommandBuilder.GetQueryBlameCommand,             OutputParser.ParseBlame);
		GitCliMethod.Create(out _queryBlobBytes,         CommandExecutor, CommandBuilder.GetQueryBlobBytesCommand);
		GitCliMethod.Create(out _queryBranch,            CommandExecutor, CommandBuilder.GetQueryBranchCommand,            OutputParser.ParseSingleBranch);
		GitCliMethod.Create(out _queryBranches,          CommandExecutor, CommandBuilder.GetQueryBranchesCommand,          OutputParser.ParseBranches);
		GitCliMethod.Create(out _queryConfig,            CommandExecutor, CommandBuilder.GetQueryConfigCommand,            OutputParser.ParseQueryConfigResults);
		GitCliMethod.Create(out _queryConfigParameter,   CommandExecutor, CommandBuilder.GetQueryConfigParameterCommand,   OutputParser.ParseQueryConfigParameterResult);
		GitCliMethod.Create(out _queryDiff,              CommandExecutor, CommandBuilder.GetDiffCommand,                   OutputParser.ParseDiff);
		GitCliMethod.Create(out _queryObjects,           CommandExecutor, CommandBuilder.GetQueryObjectsCommand,           OutputParser.ParseObjects);
		GitCliMethod.Create(out _queryRevision,          CommandExecutor, CommandBuilder.GetQueryRevisionCommand,          OutputParser.ParseSingleRevision);
		GitCliMethod.Create(out _queryRevisions,         CommandExecutor, CommandBuilder);
		GitCliMethod.Create(out _queryRevisionGraph,     CommandExecutor, CommandBuilder.GetQueryRevisionGraphCommand,     OutputParser.ParseRevisionGraph);
		GitCliMethod.Create(out _queryNotes,             CommandExecutor, CommandBuilder.GetQueryNotesCommand,             OutputParser.ParseNotes);
		GitCliMethod.Create(out _queryFilesToAdd,        CommandExecutor, CommandBuilder.GetQueryFilesToAddCommand,        OutputParser.ParseFilesToAdd);
		GitCliMethod.Create(out _queryFilesToRemove,     CommandExecutor, CommandBuilder.GetQueryFilesToRemoveCommand,     OutputParser.ParseFilesToRemove);
		GitCliMethod.Create(out _queryFilesToClean,      CommandExecutor, CommandBuilder.GetQueryFilesToCleanCommand,      OutputParser.ParseFilesToClean);
		GitCliMethod.Create(out _queryRemote,            CommandExecutor, CommandBuilder.GetQueryRemoteCommand,            OutputParser.ParseSingleRemote);
		GitCliMethod.Create(out _queryRemotes,           CommandExecutor, CommandBuilder.GetQueryRemotesCommand,           OutputParser.ParseRemotesOutput);
		_querySymbolicReference = new QuerySymbolicReferenceFunction(repository);
		GitCliMethod.Create(out _queryBranchesToPrune,   CommandExecutor, CommandBuilder.GetQueryPrunedBranchesCommand,    OutputParser.ParsePrunedBranches);
		GitCliMethod.Create(out _queryRemoteReferences,  CommandExecutor, CommandBuilder.GetQueryRemoteReferencesCommand,  OutputParser.ParseRemoteReferences);
		GitCliMethod.Create(out _queryReferences,        CommandExecutor, CommandBuilder.GetQueryReferencesCommand,        OutputParser.ParseReferences);
		GitCliMethod.Create(out _queryReflog,            CommandExecutor, CommandBuilder.GetQueryReflogCommand);
		GitCliMethod.Create(out _queryStash,             CommandExecutor, CommandBuilder.GetQueryStashCommand);
		GitCliMethod.Create(out _queryStashPatch,        CommandExecutor, CommandBuilder.GetQueryStashDiffCommand);
		GitCliMethod.Create(out _queryStashTop,          CommandExecutor, CommandBuilder.GetQueryStashTopCommand,          OutputParser.ParseQueryStashTopOutput);
		GitCliMethod.Create(out _queryStashDiff,         CommandExecutor, CommandBuilder.GetQueryStashDiffCommand,         OutputParser.ParseRevisionDiff);
		_queryStatus = new QueryStatusFunction(CommandExecutor, CommandBuilder);
		GitCliMethod.Create(out _queryRevisionDiff,      CommandExecutor, CommandBuilder.GetQueryRevisionDiffCommand,      OutputParser.ParseRevisionDiff);
		GitCliMethod.Create(out _queryRevisionPatch,     CommandExecutor, CommandBuilder.GetQueryRevisionDiffCommand);
		GitCliMethod.Create(out _queryTreeContent,       CommandExecutor, CommandBuilder.GetQueryTreeContentCommand,       OutputParser.ParseTreeContent);
		GitCliMethod.Create(out _queryUsers,             CommandExecutor, CommandBuilder.GetQueryUsersCommand,             OutputParser.ParseUsers);
		GitCliMethod.Create(out _queryTag,               CommandExecutor, CommandBuilder.GetQueryTagCommand,               OutputParser.ParseTag);
		GitCliMethod.Create(out _queryTagMessage,        CommandExecutor, CommandBuilder.GetQueryTagMessageCommand);
		GitCliMethod.Create(out _queryTags,              CommandExecutor, CommandBuilder.GetQueryTagsCommand,              OutputParser.ParseTags);
		GitCliMethod.Create(out _removeFiles,            CommandExecutor, CommandBuilder.GetRemoveFilesCommand);
		GitCliMethod.Create(out _removeRemoteReferences, CommandExecutor, CommandBuilder.GetRemoveRemoteReferencesCommand);
		GitCliMethod.Create(out _rebase,                 CommandExecutor, CommandBuilder.GetRebaseCommand);
		GitCliMethod.Create(out _removeRemote,           CommandExecutor, CommandBuilder.GetRemoveRemoteCommand);
		GitCliMethod.Create(out _renameBranch,           CommandExecutor, CommandBuilder.GetRenameBranchCommand,           OutputParser.HandleRenameBranchResult);
		GitCliMethod.Create(out _renameConfigSection,    CommandExecutor, CommandBuilder.GetRenameConfigSectionCommand,    OutputParser.HandleConfigResults);
		GitCliMethod.Create(out _renameRemote,           CommandExecutor, CommandBuilder.GetRenameRemoteCommand);
		GitCliMethod.Create(out _reset,                  CommandExecutor, CommandBuilder.GetResetCommand);
		GitCliMethod.Create(out _resetFiles,             CommandExecutor, CommandBuilder.GetResetFilesCommand);
		GitCliMethod.Create(out _resetBranch,            CommandExecutor, CommandBuilder.GetResetBranchCommand);
		GitCliMethod.Create(out _revert,                 CommandExecutor, CommandBuilder.GetRevertCommand,                 OutputParser.HandleRevertResult);
		GitCliMethod.Create(out _runMergeTool,           CommandExecutor, CommandBuilder.GetRunMergeToolCommand);
		GitCliMethod.Create(out _setConfigValue,         CommandExecutor, CommandBuilder.GetSetConfigValueCommand,         OutputParser.HandleConfigResults);
		GitCliMethod.Create(out _stashApply,             CommandExecutor, CommandBuilder.GetStashApplyCommand, 			   OutputParser.HandleStashApplyResult);
		GitCliMethod.Create(out _stashDrop,              CommandExecutor, CommandBuilder.GetStashDropCommand);
		GitCliMethod.Create(out _stashClear,             CommandExecutor, CommandBuilder.GetStashClearCommand);
		GitCliMethod.Create(out _stashPop,               CommandExecutor, CommandBuilder.GetStashPopCommand,               OutputParser.HandleStashPopResult);
		GitCliMethod.Create(out _stashSave,              CommandExecutor, CommandBuilder.GetStashSaveCommand,              OutputParser.ParseStashSaveResult);
		GitCliMethod.Create(out _stashToBranch,          CommandExecutor, CommandBuilder.GetStashToBranchCommand);
		GitCliMethod.Create(out _unsetConfigValue,       CommandExecutor, CommandBuilder.GetUnsetConfigValueCommand,       OutputParser.HandleConfigResults);
		GitCliMethod.Create(out _updateSubmodule,        CommandExecutor, CommandBuilder.GetUpdateSubmoduleCommand);
		GitCliMethod.Create(out _syncSubmodule,          CommandExecutor, CommandBuilder.GetSyncSubmoduleCommand);
		GitCliMethod.Create(out _verifyTags,             CommandExecutor, CommandBuilder.GetVerifyTagsCommand);
	}

	#endregion

	#region Properties

	public IGitAccessor GitAccessor => _gitCLI;

	private CommandBuilder CommandBuilder => _gitCLI.CommandBuilder;

	private OutputParser OutputParser => _gitCLI.OutputParser;

	private ICommandExecutor CommandExecutor => _executor;

	public IGitAction<AddFilesRequest> AddFiles => _addFiles;

	public IGitAction<AddRemoteRequest> AddRemote => _addRemote;

	public IGitAction<AddSubmoduleRequest> AddSubmodule => _addSubmodule;

	public IGitAction<AppendNoteRequest> AppendNote => _appendNote;

	public IGitAction<ApplyPatchRequest> ApplyPatch => _applyPatch;

	public IGitAction<ArchiveRequest> Archive => _archive;

	public IGitAction<CleanFilesRequest> CleanFiles => _cleanFiles;

	public IGitAction<CheckoutRequest> Checkout => _checkout;

	public IGitAction<CheckoutFilesRequest> CheckoutFiles => _checkoutFiles;

	public IGitAction<CherryPickRequest> CherryPick => _cherryPick;

	public IGitFunction<CommitRequest, string> Commit => _commit;

	public IGitFunction<CountObjectsRequest, ObjectCountData> CountObjects => _countObjects;

	public IGitAction<CreateBranchRequest> CreateBranch => _createBranch;

	public IGitAction<CreateTagRequest> CreateTag => _createTag;

	public IGitAction<DeleteBranchRequest> DeleteBranch => _deleteBranch;

	public IGitAction<DeleteTagRequest> DeleteTag => _deleteTag;

	public IGitFunction<DereferenceRequest, RevisionData> Dereference => _dereference;

	public IGitFunction<DescribeRequest, string?> Describe => _describe;

	public IGitAction<FetchRequest> Fetch => _fetch;

	public IGitFunction<FormatMergeMessageRequest, string> FormatMergeMessage => _formatMergeMessage;

	public IGitAction<GarbageCollectRequest> GarbageCollect => _garbageCollect;

	public IGitAction<MergeRequest> Merge => _merge;

	public IGitAction<PruneRemoteRequest> PruneRemote => _pruneRemote;

	public IGitAction<PullRequest> Pull => _pull;

	public IGitFunction<PushRequest, Many<ReferencePushResult>> Push => _push;

	public IGitFunction<QueryBlameRequest, BlameFile> QueryBlame => _queryBlame;

	public IGitFunction<QueryBlobBytesRequest, byte[]> QueryBlobBytes => _queryBlobBytes;

	public IGitFunction<QueryBranchRequest, BranchData?> QueryBranch => _queryBranch;

	public IGitFunction<QueryBranchesRequest, BranchesData> QueryBranches => _queryBranches;

	public IGitFunction<PruneRemoteRequest, IList<string>> QueryBranchesToPrune => _queryBranchesToPrune;

	public IGitFunction<QueryDiffRequest, Diff> QueryDiff => _queryDiff;

	public IGitFunction<AddFilesRequest, IList<TreeFileData>> QueryFilesToAdd => _queryFilesToAdd;

	public IGitFunction<RemoveFilesRequest, IList<string>> QueryFilesToRemove => _queryFilesToRemove;

	public IGitFunction<CleanFilesRequest, IList<string>> QueryFilesToClean => _queryFilesToClean;

	public IGitFunction<QueryNotesRequest, IList<NoteData>> QueryNotes => _queryNotes;

	public IGitFunction<QueryObjectsRequest, string> QueryObjects => _queryObjects;

	public IGitFunction<QueryRevisionRequest, RevisionData> QueryRevision => _queryRevision;

	public IGitFunction<QueryRevisionsRequest, IList<RevisionData>> QueryRevisions => _queryRevisions;

	public IGitFunction<QueryRevisionsRequest, IList<RevisionGraphData>> QueryRevisionGraph => _queryRevisionGraph;

	public IGitFunction<QueryRevisionDiffRequest, byte[]> QueryRevisionPatch => _queryRevisionPatch;

	public IGitFunction<QueryReferencesRequest, ReferencesData> QueryReferences => _queryReferences;

	public IGitFunction<QueryReflogRequest, IList<ReflogRecordData>> QueryReflog => _queryReflog;

	public IGitFunction<QueryRemoteRequest, RemoteData> QueryRemote => _queryRemote;

	public IGitFunction<QueryRemoteReferencesRequest, IList<RemoteReferenceData>> QueryRemoteReferences => _queryRemoteReferences;

	public IGitFunction<QueryRemotesRequest, IList<RemoteData>> QueryRemotes => _queryRemotes;

	public IGitFunction<QueryRevisionDiffRequest, Diff> QueryRevisionDiff => _queryRevisionDiff;

	public IGitFunction<QueryStashRequest, IList<StashedStateData>> QueryStash => _queryStash;

	public IGitFunction<QueryRevisionDiffRequest, byte[]> QueryStashPatch => _queryStashPatch;

	public IGitFunction<QueryStashTopRequest, RevisionData?> QueryStashTop => _queryStashTop;

	public IGitFunction<QueryRevisionDiffRequest, Diff> QueryStashDiff => _queryStashDiff;

	public IGitFunction<QueryStatusRequest, StatusData> QueryStatus => _queryStatus;

	public IGitFunction<QuerySymbolicReferenceRequest, SymbolicReferenceData> QuerySymbolicReference => _querySymbolicReference;

	public IGitFunction<QueryTreeContentRequest, IList<TreeContentData>> QueryTreeContent => _queryTreeContent;

	public IGitFunction<QueryTagRequest, TagData?> QueryTag => _queryTag;

	public IGitFunction<QueryTagMessageRequest, string> QueryTagMessage => _queryTagMessage;

	public IGitFunction<QueryTagsRequest, IList<TagData>> QueryTags => _queryTags;

	public IGitFunction<QueryUsersRequest, IList<UserData>> QueryUsers => _queryUsers;

	public IGitAction<RemoveFilesRequest> RemoveFiles => _removeFiles;

	public IGitAction<RemoveRemoteReferencesRequest> RemoveRemoteReferences => _removeRemoteReferences;

	public IGitAction<RebaseRequest> Rebase => _rebase;

	public IGitAction<RemoveRemoteRequest> RemoveRemote => _removeRemote;

	public IGitAction<RenameBranchRequest> RenameBranch => _renameBranch;

	public IGitAction<RenameRemoteRequest> RenameRemote => _renameRemote;

	public IGitAction<ResetRequest> Reset => _reset;

	public IGitAction<ResetFilesRequest> ResetFiles => _resetFiles;

	public IGitAction<ResetBranchRequest> ResetBranch => _resetBranch;

	public IGitAction<RevertRequest> Revert => _revert;

	public IGitAction<RunMergeToolRequest> RunMergeTool => _runMergeTool;

	public IGitAction<StashApplyRequest> StashApply => _stashApply;

	public IGitAction<StashDropRequest> StashDrop => _stashDrop;

	public IGitAction<StashClearRequest> StashClear => _stashClear;

	public IGitAction<StashPopRequest> StashPop => _stashPop;

	public IGitFunction<StashSaveRequest, bool> StashSave => _stashSave;

	public IGitAction<StashToBranchRequest> StashToBranch => _stashToBranch;

	public IGitAction<UpdateSubmoduleRequest> UpdateSubmodule => _updateSubmodule;

	public IGitAction<SyncSubmoduleRequest> SyncSubmodule => _syncSubmodule;

	public IGitAction<VerifyTagsRequest> VerifyTags => _verifyTags;

	public IGitFunction<QueryConfigRequest, IList<ConfigParameterData>> QueryConfig => _queryConfig;

	public IGitFunction<QueryConfigParameterRequest, ConfigParameterData?> QueryConfigParameter => _queryConfigParameter;

	public IGitAction<AddConfigValueRequest> AddConfigValue => _addConfigValue;

	public IGitAction<SetConfigValueRequest> SetConfigValue => _setConfigValue;

	public IGitAction<UnsetConfigValueRequest> UnsetConfigValue => _unsetConfigValue;

	public IGitAction<RenameConfigSectionRequest> RenameConfigSection => _renameConfigSection;

	public IGitAction<DeleteConfigSectionRequest> DeleteConfigSection => _deleteConfigSection;

	#endregion
}
