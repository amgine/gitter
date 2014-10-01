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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer.CLI;

	/// <summary>Accesses repository through git command line interface.</summary>
	internal sealed partial class RepositoryCLI : IRepositoryAccessor
	{
		#region Data

		private readonly GitCLI _gitCLI;
		private readonly IGitRepository _repository;
		private readonly ICommandExecutor _executor;

		private readonly IGitAction   < AddConfigValueParameters                                     > _addConfigValue;
		private readonly IGitAction   < AddFilesParameters                                           > _addFiles;
		private readonly IGitAction   < AddRemoteParameters                                          > _addRemote;
		private readonly IGitAction   < AddSubmoduleParameters                                       > _addSubmodule;
		private readonly IGitAction   < AppendNoteParameters                                         > _appendNote;
		private readonly IGitAction   < ApplyPatchParameters                                         > _applyPatch;
		private readonly IGitAction   < ArchiveParameters                                            > _archive;
		private readonly IGitAction   < CleanFilesParameters                                         > _cleanFiles;
		private readonly IGitAction   < CheckoutParameters                                           > _checkout;
		private readonly IGitAction   < CheckoutFilesParameters                                      > _checkoutFiles;
		private readonly IGitAction   < CherryPickParameters                                         > _cherryPick;
		private readonly IGitAction   < CommitParameters                                             > _commit;
		private readonly IGitFunction < CountObjectsParameters,           ObjectCountData            > _countObjects;
		private readonly IGitAction   < CreateBranchParameters                                       > _createBranch;
		private readonly IGitAction   < CreateTagParameters                                          > _createTag;
		private readonly IGitAction   < DeleteBranchParameters                                       > _deleteBranch;
		private readonly IGitAction   < DeleteConfigSectionParameters                                > _deleteConfigSection;
		private readonly IGitAction   < DeleteTagParameters                                          > _deleteTag;
		private readonly IGitFunction < DereferenceParameters,            RevisionData               > _dereference;
		private readonly IGitFunction < DescribeParameters,               string                     > _describe;
		private readonly IGitAction   < FetchParameters                                              > _fetch;
		private readonly IGitFunction < FormatMergeMessageParameters,     string                     > _formatMergeMessage;
		private readonly IGitAction   < GarbageCollectParameters                                     > _garbageCollect;
		private readonly IGitAction   < MergeParameters                                              > _merge;
		private readonly IGitAction   < PruneNotesParameters                                         > _pruneNotes;
		private readonly IGitAction   < PruneRemoteParameters                                        > _pruneRemote;
		private readonly IGitAction   < PullParameters                                               > _pull;
		private readonly IGitFunction < PushParameters,                   IList<ReferencePushResult> > _push;
		private readonly IGitFunction < QueryBlameParameters,             BlameFile                  > _queryBlame;
		private readonly IGitFunction < QueryBlobBytesParameters,         byte[]                     > _queryBlobBytes;
		private readonly IGitFunction < QueryBranchParameters,            BranchData                 > _queryBranch;
		private readonly IGitFunction < QueryBranchesParameters,          BranchesData               > _queryBranches;
		private readonly IGitFunction < PruneRemoteParameters,            IList<string>              > _queryBranchesToPrune;
		private readonly IGitFunction < QueryConfigParameters,            IList<ConfigParameterData> > _queryConfig;
		private readonly IGitFunction < QueryConfigParameterParameters,   ConfigParameterData        > _queryConfigParameter;
		private readonly IGitFunction < QueryDiffParameters,              Diff                       > _queryDiff;
		private readonly IGitFunction < AddFilesParameters,               IList<TreeFileData>        > _queryFilesToAdd;
		private readonly IGitFunction < RemoveFilesParameters,            IList<string>              > _queryFilesToRemove;
		private readonly IGitFunction < CleanFilesParameters,             IList<string>              > _queryFilesToClean;
		private readonly IGitFunction < QueryNotesParameters,             IList<NoteData>            > _queryNotes;
		private readonly IGitFunction < QueryObjectsParameters,           string                     > _queryObjects;
		private readonly IGitFunction < QueryRevisionParameters,          RevisionData               > _queryRevision;
		private readonly IGitFunction < QueryRevisionsParameters,         IList<RevisionData>        > _queryRevisions;
		private readonly IGitFunction < QueryRevisionsParameters,         IList<RevisionGraphData>   > _queryRevisionGraph;
		private readonly IGitFunction < QueryRevisionDiffParameters,      byte[]                     > _queryRevisionPatch;
		private readonly IGitFunction < QueryReferencesParameters,        ReferencesData             > _queryReferences;
		private readonly IGitFunction < QueryReflogParameters,            IList<ReflogRecordData>    > _queryReflog;
		private readonly IGitFunction < QueryRemoteParameters,            RemoteData                 > _queryRemote;
		private readonly IGitFunction < QueryRemoteReferencesParameters,  IList<RemoteReferenceData> > _queryRemoteReferences;
		private readonly IGitFunction < QueryRemotesParameters,           IList<RemoteData>          > _queryRemotes;
		private readonly IGitFunction < QueryRevisionDiffParameters,      Diff                       > _queryRevisionDiff;
		private readonly IGitFunction < QueryStashParameters,             IList<StashedStateData>    > _queryStash;
		private readonly IGitFunction < QueryRevisionDiffParameters,      byte[]                     > _queryStashPatch;
		private readonly IGitFunction < QueryStashTopParameters,          RevisionData               > _queryStashTop;
		private readonly IGitFunction < QueryRevisionDiffParameters,      Diff                       > _queryStashDiff;
		private readonly IGitFunction < QueryStatusParameters,            StatusData                 > _queryStatus;
		private readonly IGitFunction < QuerySymbolicReferenceParameters, SymbolicReferenceData      > _querySymbolicReference;
		private readonly IGitFunction < QueryTreeContentParameters,       IList<TreeContentData>     > _queryTreeContent;
		private readonly IGitFunction < QueryTagParameters,               TagData                    > _queryTag;
		private readonly IGitFunction < QueryTagMessageParameters,        string                     > _queryTagMessage;
		private readonly IGitFunction < QueryTagsParameters,              IList<TagData>             > _queryTags;
		private readonly IGitFunction < QueryUsersParameters,             IList<UserData>            > _queryUsers;
		private readonly IGitAction   < RemoveFilesParameters                                        > _removeFiles;
		private readonly IGitAction   < RemoveRemoteReferencesParameters                             > _removeRemoteReferences;
		private readonly IGitAction   < RebaseParameters                                             > _rebase;
		private readonly IGitAction   < RemoveRemoteParameters                                       > _removeRemote;
		private readonly IGitAction   < RenameConfigSectionParameters                                > _renameConfigSection;
		private readonly IGitAction   < RenameBranchParameters                                       > _renameBranch;
		private readonly IGitAction   < RenameRemoteParameters                                       > _renameRemote;
		private readonly IGitAction   < ResetParameters                                              > _reset;
		private readonly IGitAction   < ResetFilesParameters                                         > _resetFiles;
		private readonly IGitAction   < ResetBranchParameters                                        > _resetBranch;
		private readonly IGitAction   < RevertParameters                                             > _revert;
		private readonly IGitAction   < RunMergeToolParameters                                       > _runMergeTool;
		private readonly IGitAction   < SetConfigValueParameters                                     > _setConfigValue;
		private readonly IGitAction   < StashApplyParameters                                         > _stashApply;
		private readonly IGitAction   < StashDropParameters                                          > _stashDrop;
		private readonly IGitAction   < StashClearParameters                                         > _stashClear;
		private readonly IGitAction   < StashPopParameters                                           > _stashPop;
		private readonly IGitFunction < StashSaveParameters,              bool                       > _stashSave;
		private readonly IGitAction   < StashToBranchParameters                                      > _stashToBranch;
		private readonly IGitAction   < UnsetConfigValueParameters                                   > _unsetConfigValue;
		private readonly IGitAction   < SubmoduleUpdateParameters                                    > _updateSubmodule;
		private readonly IGitAction   < VerifyTagsParameters                                         > _verifyTags;

		#endregion

		#region .ctor

		public RepositoryCLI(GitCLI gitCLI, IGitRepository repository)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");
			Verify.Argument.IsNotNull(repository, "repository");

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
			GitCliMethod.Create(out _commit,                 CommandExecutor, CommandBuilder.GetCommitCommand);
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
			_querySymbolicReference = new QuerySymbolicReferenceImpl(repository);
			GitCliMethod.Create(out _queryBranchesToPrune,   CommandExecutor, CommandBuilder.GetQueryPrunedBranchesCommand,    OutputParser.ParsePrunedBranches);
			GitCliMethod.Create(out _queryRemoteReferences,  CommandExecutor, CommandBuilder.GetQueryRemoteReferencesCommand,  OutputParser.ParseRemoteReferences);
			GitCliMethod.Create(out _queryReferences,        CommandExecutor, CommandBuilder.GetQueryReferencesCommand,        OutputParser.ParseReferences);
			GitCliMethod.Create(out _queryReflog,            CommandExecutor, CommandBuilder.GetQueryReflogCommand);
			GitCliMethod.Create(out _queryStash,             CommandExecutor, CommandBuilder.GetQueryStashCommand);
			GitCliMethod.Create(out _queryStashPatch,        CommandExecutor, CommandBuilder.GetQueryStashDiffCommand);
			GitCliMethod.Create(out _queryStashTop,          CommandExecutor, CommandBuilder.GetQueryStashTopCommand,          OutputParser.ParseQueryStashTopOutput);
			GitCliMethod.Create(out _queryStashDiff,         CommandExecutor, CommandBuilder.GetQueryStashDiffCommand,         OutputParser.ParseRevisionDiff);
			_queryStatus = new QueryStatusImpl(CommandExecutor, CommandBuilder);
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
			GitCliMethod.Create(out _verifyTags,             CommandExecutor, CommandBuilder.GetVerifyTagsCommand);
		}

		#endregion

		#region Properties

		public IGitAccessor GitAccessor
		{
			get { return _gitCLI; }
		}

		private CommandBuilder CommandBuilder
		{
			get { return _gitCLI.CommandBuilder; }
		}

		private OutputParser OutputParser
		{
			get { return _gitCLI.OutputParser; }
		}

		private ICommandExecutor CommandExecutor
		{
			get { return _executor; }
		}

		public IGitAction<AddFilesParameters> AddFiles
		{
			get { return _addFiles; }
		}

		public IGitAction<AddRemoteParameters> AddRemote
		{
			get { return _addRemote; }
		}

		public IGitAction<AddSubmoduleParameters> AddSubmodule
		{
			get { return _addSubmodule; }
		}

		public IGitAction<AppendNoteParameters> AppendNote
		{
			get { return _appendNote; }
		}

		public IGitAction<ApplyPatchParameters> ApplyPatch
		{
			get { return _applyPatch; }
		}

		public IGitAction<ArchiveParameters> Archive
		{
			get { return _archive; }
		}

		public IGitAction<CleanFilesParameters> CleanFiles
		{
			get { return _cleanFiles; }
		}

		public IGitAction<CheckoutParameters> Checkout
		{
			get { return _checkout; }
		}

		public IGitAction<CheckoutFilesParameters> CheckoutFiles
		{
			get { return _checkoutFiles; }
		}

		public IGitAction<CherryPickParameters> CherryPick
		{
			get { return _cherryPick; }
		}

		public IGitAction<CommitParameters> Commit
		{
			get { return _commit; }
		}

		public IGitFunction<CountObjectsParameters, ObjectCountData> CountObjects
		{
			get { return _countObjects; }
		}

		public IGitAction<CreateBranchParameters> CreateBranch
		{
			get { return _createBranch; }
		}

		public IGitAction<CreateTagParameters> CreateTag
		{
			get { return _createTag; }
		}

		public IGitAction<DeleteBranchParameters> DeleteBranch
		{
			get { return _deleteBranch; }
		}

		public IGitAction<DeleteTagParameters> DeleteTag
		{
			get { return _deleteTag; }
		}

		public IGitFunction<DereferenceParameters, RevisionData> Dereference
		{
			get { return _dereference; }
		}

		public IGitFunction<DescribeParameters, string> Describe
		{
			get { return _describe; }
		}

		public IGitAction<FetchParameters> Fetch
		{
			get { return _fetch; }
		}

		public IGitFunction<FormatMergeMessageParameters, string> FormatMergeMessage
		{
			get { return _formatMergeMessage; }
		}

		public IGitAction<GarbageCollectParameters> GarbageCollect
		{
			get { return _garbageCollect; }
		}

		public IGitAction<MergeParameters> Merge
		{
			get { return _merge; }
		}

		public IGitAction<PruneRemoteParameters> PruneRemote
		{
			get { return _pruneRemote; }
		}

		public IGitAction<PullParameters> Pull
		{
			get { return _pull; }
		}

		public IGitFunction<PushParameters, IList<ReferencePushResult>> Push
		{
			get { return _push; }
		}

		public IGitFunction<QueryBlameParameters, BlameFile> QueryBlame
		{
			get { return _queryBlame; }
		}

		public IGitFunction<QueryBlobBytesParameters, byte[]> QueryBlobBytes
		{
			get { return _queryBlobBytes; }
		}

		public IGitFunction<QueryBranchParameters, BranchData> QueryBranch
		{
			get { return _queryBranch; }
		}

		public IGitFunction<QueryBranchesParameters, BranchesData> QueryBranches
		{
			get { return _queryBranches; }
		}

		public IGitFunction<PruneRemoteParameters, IList<string>> QueryBranchesToPrune
		{
			get { return _queryBranchesToPrune; }
		}

		public IGitFunction<QueryDiffParameters, Diff> QueryDiff
		{
			get { return _queryDiff; }
		}

		public IGitFunction<AddFilesParameters, IList<TreeFileData>> QueryFilesToAdd
		{
			get { return _queryFilesToAdd; }
		}

		public IGitFunction<RemoveFilesParameters, IList<string>> QueryFilesToRemove
		{
			get { return _queryFilesToRemove; }
		}

		public IGitFunction<CleanFilesParameters, IList<string>> QueryFilesToClean
		{
			get { return _queryFilesToClean; }
		}

		public IGitFunction<QueryNotesParameters, IList<NoteData>> QueryNotes
		{
			get { return _queryNotes; }
		}

		public IGitFunction<QueryObjectsParameters, string> QueryObjects
		{
			get { return _queryObjects; }
		}

		public IGitFunction<QueryRevisionParameters, RevisionData> QueryRevision
		{
			get { return _queryRevision; }
		}

		public IGitFunction<QueryRevisionsParameters, IList<RevisionData>> QueryRevisions
		{
			get { return _queryRevisions; }
		}

		public IGitFunction<QueryRevisionsParameters, IList<RevisionGraphData>> QueryRevisionGraph
		{
			get { return _queryRevisionGraph; }
		}

		public IGitFunction<QueryRevisionDiffParameters, byte[]> QueryRevisionPatch
		{
			get { return _queryRevisionPatch; }
		}

		public IGitFunction<QueryReferencesParameters, ReferencesData> QueryReferences
		{
			get { return _queryReferences; }
		}

		public IGitFunction<QueryReflogParameters, IList<ReflogRecordData>> QueryReflog
		{
			get { return _queryReflog; }
		}

		public IGitFunction<QueryRemoteParameters, RemoteData> QueryRemote
		{
			get { return _queryRemote; }
		}

		public IGitFunction<QueryRemoteReferencesParameters, IList<RemoteReferenceData>> QueryRemoteReferences
		{
			get { return _queryRemoteReferences; }
		}

		public IGitFunction<QueryRemotesParameters, IList<RemoteData>> QueryRemotes
		{
			get { return _queryRemotes; }
		}

		public IGitFunction<QueryRevisionDiffParameters, Diff> QueryRevisionDiff
		{
			get { return _queryRevisionDiff; }
		}

		public IGitFunction<QueryStashParameters, IList<StashedStateData>> QueryStash
		{
			get { return _queryStash; }
		}

		public IGitFunction<QueryRevisionDiffParameters, byte[]> QueryStashPatch
		{
			get { return _queryStashPatch; }
		}

		public IGitFunction<QueryStashTopParameters, RevisionData> QueryStashTop
		{
			get { return _queryStashTop; }
		}

		public IGitFunction<QueryRevisionDiffParameters, Diff> QueryStashDiff
		{
			get { return _queryStashDiff; }
		}

		public IGitFunction<QueryStatusParameters, StatusData> QueryStatus
		{
			get { return _queryStatus; }
		}

		public IGitFunction<QuerySymbolicReferenceParameters, SymbolicReferenceData> QuerySymbolicReference
		{
			get { return _querySymbolicReference; }
		}

		public IGitFunction<QueryTreeContentParameters, IList<TreeContentData>> QueryTreeContent
		{
			get { return _queryTreeContent; }
		}

		public IGitFunction<QueryTagParameters, TagData> QueryTag
		{
			get { return _queryTag; }
		}

		public IGitFunction<QueryTagMessageParameters, string> QueryTagMessage
		{
			get { return _queryTagMessage; }
		}

		public IGitFunction<QueryTagsParameters, IList<TagData>> QueryTags
		{
			get { return _queryTags; }
		}

		public IGitFunction<QueryUsersParameters, IList<UserData>> QueryUsers
		{
			get { return _queryUsers; }
		}

		public IGitAction<RemoveFilesParameters> RemoveFiles
		{
			get { return _removeFiles; }
		}

		public IGitAction<RemoveRemoteReferencesParameters> RemoveRemoteReferences
		{
			get { return _removeRemoteReferences; }
		}

		public IGitAction<RebaseParameters> Rebase
		{
			get { return _rebase; }
		}

		public IGitAction<RemoveRemoteParameters> RemoveRemote
		{
			get { return _removeRemote; }
		}

		public IGitAction<RenameBranchParameters> RenameBranch
		{
			get { return _renameBranch; }
		}

		public IGitAction<RenameRemoteParameters> RenameRemote
		{
			get { return _renameRemote; }
		}

		public IGitAction<ResetParameters> Reset
		{
			get { return _reset; }
		}

		public IGitAction<ResetFilesParameters> ResetFiles
		{
			get { return _resetFiles; }
		}

		public IGitAction<ResetBranchParameters> ResetBranch
		{
			get { return _resetBranch; }
		}

		public IGitAction<RevertParameters> Revert
		{
			get { return _revert; }
		}

		public IGitAction<RunMergeToolParameters> RunMergeTool
		{
			get { return _runMergeTool; }
		}

		public IGitAction<StashApplyParameters> StashApply
		{
			get { return _stashApply; }
		}

		public IGitAction<StashDropParameters> StashDrop
		{
			get { return _stashDrop; }
		}

		public IGitAction<StashClearParameters> StashClear
		{
			get { return _stashClear; }
		}

		public IGitAction<StashPopParameters> StashPop
		{
			get { return _stashPop; }
		}

		public IGitFunction<StashSaveParameters, bool> StashSave
		{
			get { return _stashSave; }
		}

		public IGitAction<StashToBranchParameters> StashToBranch
		{
			get { return _stashToBranch; }
		}

		public IGitAction<SubmoduleUpdateParameters> UpdateSubmodule
		{
			get { return _updateSubmodule; }
		}

		public IGitAction<VerifyTagsParameters> VerifyTags
		{
			get { return _verifyTags; }
		}

		public IGitFunction<QueryConfigParameters, IList<ConfigParameterData>> QueryConfig
		{
			get { return _queryConfig; }
		}

		public IGitFunction<QueryConfigParameterParameters, ConfigParameterData> QueryConfigParameter
		{
			get { return _queryConfigParameter; }
		}

		public IGitAction<AddConfigValueParameters> AddConfigValue
		{
			get { return _addConfigValue; }
		}

		public IGitAction<SetConfigValueParameters> SetConfigValue
		{
			get { return _setConfigValue; }
		}

		public IGitAction<UnsetConfigValueParameters> UnsetConfigValue
		{
			get { return _unsetConfigValue; }
		}

		public IGitAction<RenameConfigSectionParameters> RenameConfigSection
		{
			get { return _renameConfigSection; }
		}

		public IGitAction<DeleteConfigSectionParameters> DeleteConfigSection
		{
			get { return _deleteConfigSection; }
		}

		#endregion
	}
}
