#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2018  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;
using System.IO;

sealed class CommandBuilder(GitCLI gitCLI)
{
	private static readonly Version SubmodulesStatusArgVersion = new(1, 7, 2, 3);

	private static readonly Version MergeFileArgVersion = new(2, 9, 0);

	public Command GetInitCommand(InitRepositoryRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new InitCommand.Builder();
		if(request.Bare)
		{
			builder.Bare();
		}
		if(request.Template is { Length: not 0 } template)
		{
			builder.Template(template);
		}
		if(request.InitialBranch is { Length: not 0 } initialBranch)
		{
			builder.InitialBranch(initialBranch);
		}
		return builder.Build();
	}

	public Command GetCloneCommand(CloneRepositoryRequest request, bool isAsync)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		if(!string.IsNullOrEmpty(request.Template))
		{
			args.Add(CloneCommand.Template(request.Template!));
		}
		if(request.NoCheckout)
		{
			args.Add(CloneCommand.NoCheckout);
		}
		if(request.Bare)
		{
			args.Add(CloneCommand.Bare);
		}
		if(request.Mirror)
		{
			args.Add(CloneCommand.Mirror);
		}
		if(!string.IsNullOrEmpty(request.RemoteName))
		{
			args.Add(CloneCommand.Origin(request.RemoteName!));
		}
		if(request.Shallow)
		{
			args.Add(CloneCommand.Depth(request.Depth));
		}
		if(request.Recursive)
		{
			args.Add(CloneCommand.Recursive);
		}
		if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(gitCLI))
		{
			args.Add(CloneCommand.Progress);
		}
		args.Add(CloneCommand.NoMoreOptions);
		args.Add(new PathCommandArgument(request.Url));
		args.Add(new PathCommandArgument(request.Path));

		return new CloneCommand(args);
	}

	public Command GetDereferenceByNameCommand(string reference)
	{
		var builder = new LogCommand.Builder();
		builder.MaxCount(1);
		builder.SpecifyReference(reference);
		builder.AddArgument(GetRevisionFormatArgument());
		return builder.Build();
	}

	public Command GetQueryStashTopCommand(QueryStashTopRequest request)
	{
		Assert.IsNotNull(request);

		return request.LoadCommitInfo
			? GetDereferenceByNameCommand(GitConstants.StashFullName)
			: new ShowRefCommand(
				ShowRefCommand.Verify(),
				new CommandParameter(GitConstants.StashFullName));
	}

	public Command GetQueryStatusCommand(QueryStatusRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(7);
		args.Add(StatusCommand.Porcelain());
		args.Add(StatusCommand.UntrackedFiles(request.UntrackedFilesMode));
		args.Add(StatusCommand.NullTerminate());
		if(gitCLI.GitVersion >= SubmodulesStatusArgVersion)
		{
			if(request.IgnoreSubmodulesMode != StatusIgnoreSubmodulesMode.Default)
			{
				args.Add(StatusCommand.IgnoreSubmodules(request.IgnoreSubmodulesMode));
			}
		}
		if(!string.IsNullOrEmpty(request.Path))
		{
			args.Add(StatusCommand.NoMoreOptions());
			args.Add(new PathCommandArgument(request.Path!));
		}
		return new StatusCommand(args);
	}

	public Command GetQueryObjectsCommand(QueryObjectsRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(request.Objects.Count);
		foreach(var obj in request.Objects)
		{
			args.Add(new CommandParameter(obj));
		}
		return new ShowCommand(args);
	}

	public Command GetQueryRevisionsCommand(QueryRevisionsRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new LogCommand.Builder();
		InsertQueryRevisionsParameters(request, builder, GetRevisionFormatArgument());
		return builder.Build();
	}

	public Command GetQueryRevisionGraphCommand(QueryRevisionsRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new LogCommand.Builder();
		InsertQueryRevisionsParameters(request, builder, LogCommand.KnownArguments.Format("%H%n%P"));
		return builder.Build();
	}

	public Command GetQueryRevisionCommand(QueryRevisionRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new LogCommand.Builder();
		builder.MaxCount(1);
		builder.SpecifyReference(request.SHA1);
		builder.AddArgument(GetRevisionDataFormatArgument());
		return builder.Build();
	}

	public Command GetQueryReflogCommand(QueryReflogRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new LogCommand.Builder();
		builder.WalkReflogs();
		builder.MaxCount(request.MaxCount);
		builder.NullTerminate();
		builder.AddArgument(GetReflogFormatArgument());
		builder.SpecifyOptionalReference(request.Reference);
		return builder.Build();
	}

	private ICommandArgument GetReflogFormatArgument()
		=> GitFeatures.LogFormatBTag.IsAvailableFor(gitCLI)
			? LogCommand.KnownArguments.Format(LogFormatPlaceholders.ReflogFormat)
			: LogCommand.KnownArguments.Format(LogFormatPlaceholders.ReflogFormatOld);

	private ICommandArgument GetRevisionFormatArgument()
		=> GitFeatures.LogFormatBTag.IsAvailableFor(gitCLI)
			? LogCommand.KnownArguments.Format(LogFormatPlaceholders.RevisionFormat)
			: LogCommand.KnownArguments.Format(LogFormatPlaceholders.RevisionFormatOld);

	private ICommandArgument GetRevisionDataFormatArgument()
		=> GitFeatures.LogFormatBTag.IsAvailableFor(gitCLI)
			? LogCommand.KnownArguments.Format(LogFormatPlaceholders.RevisionDataFormat)
			: LogCommand.KnownArguments.Format(LogFormatPlaceholders.RevisionDataFormatOld);

	private static void InsertQueryRevisionsParameters(QueryRevisionsRequest request, LogCommand.Builder builder, ICommandArgument format)
	{
		#region Commit Limiting

		builder.MaxCount(request.MaxCount);
		builder.Skip(request.Skip);

		if(request.SinceDate.HasValue)
		{
			builder.Since(request.SinceDate.Value);
		}
		if(request.UntilDate.HasValue)
		{
			builder.Until(request.UntilDate.Value);
		}

		if(request.AuthorPattern is not null)
		{
			builder.Author(request.AuthorPattern);
		}
		if(request.CommitterPattern is not null)
		{
			builder.Committer(request.CommitterPattern);
		}
		if(request.MessagePattern is not null)
		{
			builder.Grep(request.MessagePattern);
		}
		if(request.AllMatch)           builder.AllMatch();
		if(request.RegexpIgnoreCase)   builder.RegexpIgnoreCase();
		if(request.RegexpExtended)     builder.ExtendedRegexp();
		if(request.RegexpFixedStrings) builder.FixedStrings();
		if(request.RemoveEmpty)        builder.RemoveEmpty();
		switch(request.MergesMode)
		{
			case RevisionMergesQueryMode.MergesOnly:
				builder.Merges();
				break;
			case RevisionMergesQueryMode.NoMerges:
				builder.NoMerges();
				break;
		}
		if(request.Follow) builder.Follow();

		if(request.Not) builder.Not();
		if(request.All) builder.All();

		if(request.ReferencesGlob is not null) builder.Glob     (request.ReferencesGlob);
		if(request.Branches       is not null) builder.Branches (request.Branches);
		if(request.Tags           is not null) builder.Tags     (request.Tags);
		if(request.Remotes        is not null) builder.Remotes  (request.Remotes);

		#endregion

		#region History Simplification

		if(request.FirstParent)            builder.FirstParent();
		if(request.SimplifyByDecoration)   builder.SimplifyByDecoration();
		if(request.EnableParentsRewriting) builder.Parents();

		#endregion

		#region Ordering

		builder.Order(request.Order);
		if(request.Reverse) builder.Reverse();

		#endregion

		#region Formatting

		builder.NullTerminate();
		if(format is not null)
		{
			builder.AddArgument(format);
		}

		#endregion

		if(request.Since is not null && request.Until is not null)
		{
			builder.AddArgument(new ReferenceRangeCommandParameter(request.Since, request.Until));
		}

		builder.SpecifyReferences (request.References);
		builder.SpecifyPaths      (request.Paths);
	}

	public Command GetQueryNotesCommand(QueryNotesRequest request)
	{
		return new NotesCommand(NotesCommand.List());
	}

	public Command GetDereferenceCommand(DereferenceRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new LogCommand.Builder();
		builder.AddArgument(new CommandParameter(request.Reference));
		builder.MaxCount(1);
		if(request.LoadRevisionData)
		{
			builder.AddArgument(GetRevisionFormatArgument());
		}
		else
		{
			builder.Format("%H");
		}
		return builder.Build();
	}

	public Command GetCommitCommand(CommitRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(12);
		if(request.All)
		{
			args.Add(CommitCommand.All());
		}
		if(request.SignOff)
		{
			args.Add(CommitCommand.SignOff());
		}
		if(request.Amend)
		{
			args.Add(CommitCommand.Amend());
		}
		if(!string.IsNullOrEmpty(request.ReuseMessageFrom))
		{
			args.Add(CommitCommand.ReuseMessage(request.ReuseMessageFrom!));
		}
		var message = request.Message;
		switch(message.Type)
		{
			case MessageSpecificationType.Text when message.Text is { Length: not 0 } text:
				args.Add(CommitCommand.Message(text));
				break;
			case MessageSpecificationType.File when message.File is { Length: not 0 } file:
				args.Add(CommitCommand.File(file));
				break;
		}
		if(request.ResetAuthor)
		{
			args.Add(CommitCommand.ResetAuthor());
		}
		if(request.AllowEmpty)
		{
			args.Add(CommitCommand.AllowEmpty());
		}
		if(request.AllowEmptyMessage)
		{
			args.Add(CommitCommand.AllowEmptyMessage());
		}
		if(request.NoVerify)
		{
			args.Add(CommitCommand.NoVerify());
		}
		if(!string.IsNullOrEmpty(request.Author))
		{
			args.Add(CommitCommand.Author(request.Author!));
		}
		if(request.AuthorDate.HasValue)
		{
			args.Add(CommitCommand.Date(request.AuthorDate.Value));
		}
		return new CommitCommand(args);
	}

	public Command GetQueryFilesToAddCommand(AddFilesRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new AddCommand.Builder();
		builder.DryRun();
		InsertAddFilesParameters(request, builder);
		return builder.Build();
	}

	public Command GetQueryFilesToRemoveCommand(RemoveFilesRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(request.Paths.Count + 1 + 5);
		args.Add(RmCommand.DryRun());
		InsertRemoveFilesParameters(request, args);
		return new RmCommand(args);
	}

	public Command GetQueryFilesToCleanCommand(CleanFilesRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new CleanCommand.Builder();
		builder.DryRun();
		InsertCleanFilesParameters(request, builder);
		return builder.Build();
	}

	public Command GetAddFilesCommand(AddFilesRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new AddCommand.Builder();
		InsertAddFilesParameters(request, builder);
		return builder.Build();
	}

	private static void InsertAddFilesParameters(AddFilesRequest request, AddCommand.Builder builder)
	{
		if(request.Force)
		{
			builder.Force();
		}
		switch(request.Mode)
		{
			case AddFilesMode.All:
				builder.All();
				break;
			case AddFilesMode.Update:
				builder.Update();
				break;
		}
		if(request.IntentToAdd)
		{
			builder.IntentToAdd();
		}
		if(request.Refresh)
		{
			builder.Refresh();
		}
		if(request.IgnoreErrors)
		{
			builder.IgnoreErrors();
		}
		builder.SpecifyPaths(request.Paths);
	}

	public Command GetRemoveFilesCommand(RemoveFilesRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(request.Paths.Count + 1 + 4);
		InsertRemoveFilesParameters(request, args);
		return new RmCommand(args);
	}

	private static void InsertRemoveFilesParameters(RemoveFilesRequest request, List<ICommandArgument> args)
	{
		if(request.Force)
		{
			args.Add(RmCommand.Force());
		}
		if(request.Recursive)
		{
			args.Add(RmCommand.Recursive());
		}
		if(request.Cached)
		{
			args.Add(RmCommand.Cached());
		}
		if(request.IgnoreUnmatch)
		{
			args.Add(RmCommand.IgnoreUnmatch());
		}
		if(request.Paths is { Count: not 0 } paths)
		{
			args.Add(RmCommand.NoMoreOptions());
			foreach(var path in paths)
			{
				args.Add(new PathCommandArgument(path));
			}
		}
	}

	public Command GetCleanFilesCommand(CleanFilesRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new CleanCommand.Builder();
		InsertCleanFilesParameters(request, builder);
		return builder.Build();
	}

	private void InsertCleanFilesParameters(CleanFilesRequest request, CleanCommand.Builder builder)
	{
		if(request.Force)
		{
			builder.Force();
		}
		if(request.RemoveDirectories)
		{
			builder.Directories();
		}
		switch(request.Mode)
		{
			case CleanFilesMode.IncludeIgnored: builder.IncludeIgnored();   break;
			case CleanFilesMode.OnlyIgnored:    builder.ExcludeUntracked(); break;
		}
		if(!request.ExcludePatterns.IsEmpty && GitFeatures.CleanExcludeOption.IsAvailableFor(gitCLI))
		{
			foreach(var pattern in request.ExcludePatterns)
			{
				if(!string.IsNullOrEmpty(pattern))
				{
					builder.Exclude(pattern);
				}
			}
		}
		builder.SpecifyPaths(request.Paths);
	}

	public Command GetResetFilesCommand(ResetFilesRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new ResetCommand.Builder();
		if(request.Revision is { Length: not 0 } revision)
		{
			builder.AddArgument(new CommandParameter(revision));
		}
		builder.SpecifyPaths(request.Paths);
		return builder.Build();
	}

	public Command GetAppendNoteCommand(AppendNoteRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(capacity: 3);
		args.Add(NotesCommand.Append());
		var message = request.Message;
		switch(message.Type)
		{
			case MessageSpecificationType.Text when message.Text is { Length: not 0 } text:
				args.Add(NotesCommand.Message(text));
				break;
			case MessageSpecificationType.File when message.File is { Length: not 0 } file:
				args.Add(NotesCommand.File(file));
				break;
		}
		args.Add(new CommandParameter(request.Revision));
		return new NotesCommand(args);
	}

	public Command GetPruneNotesCommand(PruneNotesRequest request)
	{
		Assert.IsNotNull(request);

		return new NotesCommand(NotesCommand.Prune());
	}

	public Command GetDiffCommand(QueryDiffRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new DiffCommand.Builder();
		InsertDiffParameters1(request, builder);
		builder.SpecifyOptionalReference(request.Revision1);
		builder.SpecifyOptionalReference(request.Revision2);
		InsertDiffParameters2(request, builder);
		return builder.Build();
	}

	public Command GetQueryRevisionDiffCommand(QueryRevisionDiffRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new LogCommand.Builder();
		builder.MaxCount(1);
		InsertDiffParameters1(request, builder);
		builder.CombinedDiff();
		builder.SpecifyReference(request.Revision);
		InsertDiffParameters2(request, builder);
		return builder.Build();
	}

	public Command GetQueryStashDiffCommand(QueryRevisionDiffRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new StashCommand.Builder();
		builder.Show();
		InsertDiffParameters1(request, builder);
		builder.AddArgument(new CommandParameter(request.Revision));
		InsertDiffParameters2(request, builder);
		return builder.Build();
	}

	private static void InsertDiffParameters1<T>(BaseQueryDiffRequest request, T builder)
		where T : IDiffCommandBuilder
	{
		Assert.IsNotNull(request);

		builder.Patch();
		builder.FullIndex();
		builder.NoColor();
		if(request.Context >= 0)
		{
			builder.Unified(request.Context);
		}
		if(request.Cached)            builder.Cached();
		if(request.Binary)            builder.Binary();
		if(request.Patience)          builder.Patience();
		if(request.IgnoreAllSpace)    builder.IgnoreAllSpace();
		if(request.IgnoreSpaceAtEOL)  builder.IgnoreSpaceAtEOL();
		if(request.IgnoreSpaceChange) builder.IgnoreSpaceChange();
		if(request.SwapInputs)        builder.SwapInputs();
		if(request.EnableTextConvFilters.HasValue)
		{
			if(request.EnableTextConvFilters.Value)
			{
				builder.TextConv();
			}
			else
			{
				builder.NoTextConv();
			}
		}
		if(request.EnableExternalDiffDrivers.HasValue)
		{
			if(request.EnableExternalDiffDrivers.Value)
			{
				builder.ExtDiff();
			}
			else
			{
				builder.NoExtDiff();
			}
		}
		if(request.TreatAllFilesAsText)
		{
			builder.Text();
		}
		if(request.FindRenames.HasValue)
		{
			if(request.FindRenames.Value.IsSpecified)
			{
				builder.FindRenames(request.FindRenames.Value.Similarity);
			}
			else
			{
				builder.FindRenames();
			}
		}
		if(request.FindCopies.HasValue)
		{
			if(request.FindCopies.Value.IsSpecified)
			{
				builder.FindCopies(request.FindCopies.Value.Similarity);
			}
			else
			{
				builder.FindCopies();
			}
		}
	}

	private static void InsertDiffParameters2<T>(BaseQueryDiffRequest request, T builder)
		where T : IDiffCommandBuilder
	{
		Assert.IsNotNull(request);

		builder.SpecifyPaths(request.Paths);
	}

	public Command GetQueryStashCommand(QueryStashRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new LogCommand.Builder();
		builder.WalkReflogs();
		builder.NullTerminate();
		builder.AddArgument(GetRevisionFormatArgument());
		builder.AddArgument(new CommandParameter(GitConstants.StashFullName));
		return builder.Build();
	}

	public Command GetStashSaveCommand(StashSaveRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new StashCommand.Builder();
		builder.Save();
		if(request.KeepIndex)
		{
			builder.KeepIndex();
		}
		else
		{
			builder.NoKeepIndex();
		}
		if(request.IncludeUntracked && GitFeatures.StashIncludeUntrackedOption.IsAvailableFor(gitCLI))
		{
			builder.IncludeUntracked();
		}
		if(!string.IsNullOrWhiteSpace(request.Message))
		{
			builder.AddArgument(new CommandParameter(request.Message!.SurroundWithDoubleQuotes()));
		}

		return builder.Build();
	}

	public Command GetStashApplyCommand(StashApplyRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new StashCommand.Builder();
		builder.Apply();
		if(request.RestoreIndex)
		{
			builder.Index();
		}
		if(request.StashName is not null)
		{
			builder.AddArgument(new CommandParameter(request.StashName));
		}
		return builder.Build();
	}

	public Command GetStashPopCommand(StashPopRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new StashCommand.Builder();
		builder.Pop();
		if(request.RestoreIndex)
		{
			builder.Index();
		}
		if(request.StashName != null)
		{
			builder.AddArgument(new CommandParameter(request.StashName));
		}
		return builder.Build();
	}

	public Command GetStashToBranchCommand(StashToBranchRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new StashCommand.Builder();
		builder.Branch();
		builder.AddArgument(new CommandParameter(request.BranchName));
		if(request.StashName is not null)
		{
			builder.AddArgument(new CommandParameter(request.StashName));
		}

		return builder.Build();
	}

	public Command GetStashDropCommand(StashDropRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new StashCommand.Builder();
		builder.Drop();
		if(!string.IsNullOrWhiteSpace(request.StashName))
		{
			builder.AddArgument(new CommandParameter(request.StashName));
		}
		return builder.Build();
	}

	public Command GetStashClearCommand(StashClearRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new StashCommand.Builder();
		builder.Clear();
		return builder.Build();
	}

	public Command GetQueryTreeContentCommand(QueryTreeContentRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		if(request.OnlyTrees)
		{
			args.Add(LsTreeCommand.Directories());
		}
		if(request.Recurse)
		{
			args.Add(LsTreeCommand.Recurse());
		}
		args.Add(LsTreeCommand.Tree());
		args.Add(LsTreeCommand.FullName());
		args.Add(LsTreeCommand.Long());
		args.Add(LsTreeCommand.NullTerminate());
		args.Add(new CommandParameter(request.TreeId));
		foreach(var path in request.Paths)
		{
			args.Add(new PathCommandArgument(path));
		}
		return new LsTreeCommand(args);
	}

	public Command GetApplyPatchCommand(ApplyPatchRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		switch(request.ApplyTo)
		{
			case ApplyPatchTo.WorkingDirectory:
				break;
			case ApplyPatchTo.Index:
				args.Add(ApplyCommand.Cached());
				break;
			case ApplyPatchTo.IndexAndWorkingDirectory:
				args.Add(ApplyCommand.Index());
				break;
			default:
				throw new ArgumentException();
		}
		if(request.Reverse)
		{
			args.Add(ApplyCommand.Reverse());
		}
		args.Add(ApplyCommand.UnidiffZero());
		foreach(var patch in request.Patches)
		{
			args.Add(new PathCommandArgument(patch));
		}
		return new ApplyCommand(args);
	}

	public Command GetCheckoutCommand(CheckoutRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new CheckoutCommand.Builder();
		if(request.Force)
		{
			builder.Force();
		}
		if(request.Merge)
		{
			builder.Merge();
		}
		builder.AddArgument(request.Revision);
		return builder.Build();
	}

	public Command GetCheckoutFilesCommand(CheckoutFilesRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new CheckoutCommand.Builder();
		switch(request.Mode)
		{
			case CheckoutFileMode.Ours:
				builder.Ours();
				break;
			case CheckoutFileMode.Theirs:
				builder.Theirs();
				break;
			case CheckoutFileMode.Merge:
				builder.Merge();
				break;
			case CheckoutFileMode.IgnoreUnmergedEntries:
				builder.Force();
				break;
		}
		if(!string.IsNullOrEmpty(request.Revision))
		{
			builder.AddArgument(request.Revision);
		}
		if(request.Paths is { Count: not 0 } paths)
		{
			builder.NoMoreOptions();
			foreach(var path in paths)
			{
				builder.AddArgument(new PathCommandArgument(path));
			}
		}
		return builder.Build();
	}

	public Command GetQueryBlobBytesCommand(QueryBlobBytesRequest request)
	{
		return new CatFileCommand(
			new CommandParameter(GitConstants.BlobObjectType),
			new CommandParameter(request.Treeish + ":" + request.ObjectName));
	}

	public Command GetRevertCommand(RevertRequest request)
	{
		Assert.IsNotNull(request);

		if(request.Control.HasValue)
		{
			return GetRevertCommand(request.Control.Value);
		}
		else
		{
			var builder = new RevertCommand.Builder();
			if(request.NoCommit)
			{
				builder.NoCommit();
			}
			if(request.Mainline > 0)
			{
				builder.Mainline(request.Mainline);
			}
			foreach(var rev in request.Revisions)
			{
				builder.AddArgument(new CommandParameter(rev));
			}
			return builder.Build();
		}
	}

	public Command GetRevertCommand(RevertControl control)
	{
		var builder = new RevertCommand.Builder();
		switch(control)
		{
			case RevertControl.Continue:
				builder.AddOption(new ConfigurationValueOverride("core.editor", "true"));
				builder.Continue();
				break;
			case RevertControl.Quit:
				builder.Quit();
				break;
			case RevertControl.Abort:
				builder.Abort();
				break;
			default:
				throw new ArgumentException("Unknown enum value.", nameof(control));
		}
		return builder.Build();
	}

	public Command GetCherryPickCommand(CherryPickRequest request)
	{
		Assert.IsNotNull(request);

		if(request.Control.HasValue)
		{
			return GetCherryPickCommand(request.Control.Value);
		}
		var builder = new CherryPickCommand.Builder();
		if(request.NoCommit)             builder.NoCommit();
		if(request.Mainline > 0)         builder.Mainline(request.Mainline);
		if(request.SignOff)              builder.SignOff();
		if(request.FastForward)          builder.FastForward();
		if(request.AllowEmpty)           builder.AllowEmpty();
		if(request.AllowEmptyMessage)    builder.AllowEmptyMessage();
		if(request.KeepRedundantCommits) builder.KeepRedundantCommits();
		foreach(var rev in request.Revisions)
		{
			builder.AddArgument(new CommandParameter(rev));
		}
		return builder.Build();
	}

	public Command GetCherryPickCommand(CherryPickControl control)
	{
		var builder = new CherryPickCommand.Builder();
		switch(control)
		{
			case CherryPickControl.Continue:
				builder.AddOption(new ConfigurationValueOverride("core.editor", "true"));
				builder.Continue();
				break;
			case CherryPickControl.Quit:
				builder.Quit();
				break;
			case CherryPickControl.Abort:
				builder.Abort();
				break;
			default:
				throw new ArgumentException("Unknown enum value.", nameof(control));
		}
		return builder.Build();
	}

	public Command GetResetCommand(ResetRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new ResetCommand.Builder();
		builder.Mode(request.Mode);
		if(!string.IsNullOrEmpty(request.Revision))
		{
			builder.AddArgument(new CommandParameter(request.Revision!));
		}
		return builder.Build();
	}

	public Command GetMergeCommand(MergeRequest request)
	{
		Assert.IsNotNull(request);

		static string EscapeMessageText(string text)
			=> text.Replace("\"", "\\\"");

		var args = new List<ICommandArgument>();
		if(request.NoCommit)
		{
			args.Add(MergeCommand.NoCommit());
		}
		if(request.Squash)
		{
			args.Add(MergeCommand.Squash());
		}
		if(request.NoFastForward)
		{
			args.Add(MergeCommand.NoFastForward());
		}
		var message = request.Message;
		switch(message.Type)
		{
			case MessageSpecificationType.Text when message.Text is { Length: not 0 } text:
				args.Add(MergeCommand.Message(EscapeMessageText(text)));
				break;
			case MessageSpecificationType.File when message.File is { Length: not 0 } file:
				if(gitCLI.GitVersion >= MergeFileArgVersion)
				{
					args.Add(MergeCommand.File(file));
				}
				else
				{
					var text = EscapeMessageText(File.ReadAllText(file));
					args.Add(MergeCommand.Message(text));
				}
				break;
			default:
				args.Add(MergeCommand.NoEdit());
				break;
		}
		foreach(var rev in request.Revisions)
		{
			args.Add(new CommandParameter(rev));
		}
		return new MergeCommand(args);
	}

	public Command GetRebaseCommand(RebaseRequest request)
	{
		Assert.IsNotNull(request);

		if(request.Control.HasValue)
		{
			ICommandArgument arg = request.Control.Value switch
			{
				RebaseControl.Abort    => RebaseCommand.Abort(),
				RebaseControl.Continue => RebaseCommand.Continue(),
				RebaseControl.Skip     => RebaseCommand.Skip(),
				_ => throw new ArgumentException("control"),
			};
			return new RebaseCommand(arg);
		}
		else
		{
			var args = new List<ICommandArgument>();
			if(!string.IsNullOrEmpty(request.NewBase))
			{
				args.Add(RebaseCommand.Onto(request.NewBase));
			}
			args.Add(new CommandParameter(request.Upstream));
			if(!string.IsNullOrEmpty(request.Branch))
			{
				args.Add(new CommandParameter(request.Branch));
			}
			return new RebaseCommand(args);
		}
	}

	public Command GetRebaseCommand(RebaseControl control)
	{
		ICommandArgument arg = control switch
		{
			RebaseControl.Abort    => RebaseCommand.Abort(),
			RebaseControl.Continue => RebaseCommand.Continue(),
			RebaseControl.Skip     => RebaseCommand.Skip(),
			_ => throw new ArgumentException(nameof(control)),
		};
		return new RebaseCommand(arg);
	}

	public Command GetQueryUsersCommand(QueryUsersRequest request)
	{
		Assert.IsNotNull(request);

		return new ShortLogCommand(
			ShortLogCommand.All(),
			ShortLogCommand.Numbered(),
			ShortLogCommand.Summary(),
			ShortLogCommand.Email());
	}

	public Command GetCountObjectsCommand(CountObjectsRequest request)
	{
		Assert.IsNotNull(request);

		return new CountObjectsCommand(CountObjectsCommand.Verbose());
	}

	public Command GetGarbageCollectCommand(GarbageCollectRequest request)
	{
		Assert.IsNotNull(request);

		return new GcCommand();
	}

	public Command GetArchiveCommand(ArchiveRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(5);
		if(request.Format is { Length: not 0 } format)
		{
			args.Add(ArchiveCommand.Format(format));
		}
		if(!string.IsNullOrEmpty(request.OutputFile))
		{
			args.Add(ArchiveCommand.Output(request.OutputFile));
		}
		if(!string.IsNullOrEmpty(request.Remote))
		{
			args.Add(ArchiveCommand.Remote(request.Remote));
		}
		args.Add(new CommandParameter(request.Tree));
		if(request.Path is { Length: not 0 } path)
		{
			args.Add(new PathCommandArgument(path));
		}
		return new ArchiveCommand(args);
	}

	public Command GetQueryBlameCommand(QueryBlameRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		args.Add(BlameCommand.Porcelain);
		if(!string.IsNullOrEmpty(request.Revision))
		{
			args.Add(new CommandParameter(request.Revision));
		}
		args.Add(CommandFlag.NoMoreOptions);
		args.Add(new PathCommandArgument(request.FileName));
		return new BlameCommand(args);
	}

	public Command GetFetchCommand(FetchRequest request, bool isAsync)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(10);
		InsertFetchParameters(request, args);
		if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(gitCLI))
		{
			args.Add(FetchCommand.Progress());
		}
		return new FetchCommand(args);
	}

	private static void InsertFetchParameters(FetchRequest request, IList<ICommandArgument> args)
	{
		if(request.All)
		{
			args.Add(FetchCommand.All());
		}
		if(request.Append)
		{
			args.Add(FetchCommand.Append());
		}
		if(request.Prune)
		{
			args.Add(FetchCommand.Prune());
		}
		if(request.Depth != 0)
		{
			args.Add(FetchCommand.Depth(request.Depth));
		}
		switch(request.TagFetchMode)
		{
			case TagFetchMode.Default:
				break;
			case TagFetchMode.AllTags:
				args.Add(FetchCommand.Tags());
				break;
			case TagFetchMode.NoTags:
				args.Add(FetchCommand.NoTags());
				break;
		}
		if(request.KeepDownloadedPack)
		{
			args.Add(FetchCommand.Keep());
		}
		if(request.Force)
		{
			args.Add(FetchCommand.Force());
		}
		if(!string.IsNullOrWhiteSpace(request.UploadPack))
		{
			args.Add(FetchCommand.UploadPack(request.UploadPack!));
		}
		if(!string.IsNullOrWhiteSpace(request.Repository))
		{
			args.Add(new CommandParameter(request.Repository!));
		}
	}

	public Command GetPullCommand(PullRequest request, bool isAsync)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		InsertPullParameters(request, args);
		if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(gitCLI))
		{
			args.Add(FetchCommand.Progress());
		}
		return new PullCommand(args);
	}

	private static void InsertPullParameters(PullRequest request, IList<ICommandArgument> args)
	{
		if(request.NoFastForward)
		{
			args.Add(MergeCommand.NoFastForward());
		}
		if(request.NoCommit)
		{
			args.Add(MergeCommand.NoCommit());
		}
		if(request.Squash)
		{
			args.Add(MergeCommand.Squash());
		}
		var arg = MergeCommand.Strategy(request.Strategy);
		if(arg is not null)
		{
			args.Add(arg);
		}
		if(!string.IsNullOrEmpty(request.StrategyOption))
		{
			args.Add(MergeCommand.StrategyOption(request.StrategyOption!));
		}
		InsertFetchParameters(request, args);
	}

	public Command GetPushCommand(PushRequest request, bool isAsync)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		switch(request.PushMode)
		{
			case PushMode.Default:
				break;
			case PushMode.AllLocalBranches:
				args.Add(PushCommand.All());
				break;
			case PushMode.Mirror:
				args.Add(PushCommand.Mirror());
				break;
			case PushMode.Tags:
				args.Add(PushCommand.Tags());
				break;
		}
		if(!string.IsNullOrEmpty(request.ReceivePack))
		{
			args.Add(PushCommand.ReceivePack(request.ReceivePack!));
		}
		if(request.Force)
		{
			args.Add(PushCommand.Force());
		}
		if(request.Delete)
		{
			args.Add(PushCommand.Delete());
		}
		if(request.SetUpstream)
		{
			args.Add(PushCommand.SetUpstream());
		}
		args.Add(request.ThinPack ? PushCommand.Thin() : PushCommand.NoThin());
		args.Add(PushCommand.Porcelain());
		if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(gitCLI))
		{
			args.Add(PushCommand.Progress());
		}
		if(!string.IsNullOrWhiteSpace(request.Repository))
		{
			args.Add(new CommandParameter(request.Repository));
		}
		foreach(var refspec in request.Refspecs)
		{
			args.Add(new CommandParameter(refspec));
		}
		return new PushCommand(args);
	}

	public Command GetQueryRemoteCommand(QueryRemoteRequest request)
	{
		Assert.IsNotNull(request);

		return new RemoteCommand(
			RemoteCommand.Show(),
			RemoteCommand.Cached(),
			new CommandParameter(request.RemoteName));
	}

	public Command GetQueryRemotesCommand(QueryRemotesRequest request)
	{
		Assert.IsNotNull(request);

		return new RemoteCommand(RemoteCommand.Verbose());
	}

	public Command GetAddRemoteCommand(AddRemoteRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(request.Branches.Count + 6);
		args.Add(RemoteCommand.Add());
		foreach(var branch in request.Branches)
		{
			args.Add(RemoteCommand.TrackBranch(branch));
		}
		if(!string.IsNullOrEmpty(request.MasterBranch))
		{
			args.Add(RemoteCommand.Master(request.MasterBranch!));
		}
		if(request.Fetch)
		{
			args.Add(RemoteCommand.Fetch());
		}
		switch(request.TagFetchMode)
		{
			case TagFetchMode.Default:
				break;
			case TagFetchMode.AllTags:
				args.Add(RemoteCommand.Tags());
				break;
			case TagFetchMode.NoTags:
				args.Add(RemoteCommand.NoTags());
				break;
		}
		if(request.Mirror)
		{
			args.Add(RemoteCommand.Mirror());
		}
		args.Add(new CommandParameter(request.RemoteName));
		args.Add(new CommandParameter(request.Url));
		return new RemoteCommand(args);
	}

	public Command GetRenameRemoteCommand(RenameRemoteRequest request)
	{
		Assert.IsNotNull(request);

		return new RemoteCommand(
			RemoteCommand.Rename(),
			new CommandParameter(request.OldName),
			new CommandParameter(request.NewName));
	}

	public Command GetRemoveRemoteCommand(RemoveRemoteRequest request)
	{
		Assert.IsNotNull(request);

		return RemoteCommand.FormatRemoveCommand(request.RemoteName);
	}

	public Command GetQueryPrunedBranchesCommand(PruneRemoteRequest request)
	{
		Assert.IsNotNull(request);

		return new RemoteCommand(
			RemoteCommand.Prune(),
			RemoteCommand.DryRun(),
			new CommandParameter(request.RemoteName));
	}

	public Command GetPruneRemoteCommand(PruneRemoteRequest request)
	{
		Assert.IsNotNull(request);

		return new RemoteCommand(
			RemoteCommand.Prune(),
			new CommandParameter(request.RemoteName));
	}

	public Command GetQueryRemoteReferencesCommand(QueryRemoteReferencesRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(4);
		if(request.Heads)
		{
			args.Add(LsRemoteCommand.Heads());
		}
		if(request.Tags)
		{
			args.Add(LsRemoteCommand.Tags());
		}
		args.Add(new CommandParameter(request.RemoteName));
		if(!string.IsNullOrEmpty(request.Pattern))
		{
			args.Add(new CommandParameter(request.Pattern!));
		}
		return new LsRemoteCommand(args);
	}

	public Command GetRemoveRemoteReferencesCommand(RemoveRemoteReferencesRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(1 + request.References.Count);
		args.Add(new CommandParameter(request.RemoteName));
		foreach(var reference in request.References)
		{
			args.Add(new CommandParameter(":" + reference));
		}
		return new PushCommand(args);
	}

	public Command GetQueryReferencesCommand(QueryReferencesRequest request)
	{
		Assert.IsNotNull(request);

		switch(request.ReferenceTypes)
		{
			case ReferenceType.LocalBranch | ReferenceType.Tag:
				return new ShowRefCommand(
					ShowRefCommand.Heads(),
					ShowRefCommand.Tags(),
					ShowRefCommand.Dereference());
			case ReferenceType.LocalBranch:
				return new ShowRefCommand(
					ShowRefCommand.Heads(),
					ShowRefCommand.Dereference());
			case ReferenceType.Tag:
				return new ShowRefCommand(
					ShowRefCommand.Tags(),
					ShowRefCommand.Dereference());
			default:
				return new ShowRefCommand(
					ShowRefCommand.Dereference());
		}
	}

	#region Branches

	public Command GetQueryBranchCommand(QueryBranchRequest request)
	{
		Assert.IsNotNull(request);

		string fullName = (request.IsRemote ?
			GitConstants.RemoteBranchPrefix :
			GitConstants.LocalBranchPrefix) + request.BranchName;
		return new ShowRefCommand(
				ShowRefCommand.Verify(),
				ShowRefCommand.Heads(),
				ShowRefCommand.Hash(),
				ShowRefCommand.NoMoreOptions(),
				new CommandParameter(fullName));
	}

	public Command GetQueryBranchesCommand(QueryBranchesRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new BranchCommand.Builder(gitCLI.GitVersion);
		builder.NoColor();
		builder.Verbose();
		builder.NoAbbrev();
		switch(request.Restriction)
		{
			case QueryBranchRestriction.All:
				builder.All();
				break;
			case QueryBranchRestriction.Remote:
				builder.Remotes();
				break;
		}
		switch(request.Mode)
		{
			case BranchQueryMode.Contains:
				builder.Contains(request.Revision);
				break;
			case BranchQueryMode.Merged:
				builder.Merged(request.Revision);
				break;
			case BranchQueryMode.NoMerged:
				builder.NoMerged(request.Revision);
				break;
		}
		return builder.Build();
	}

	public Command GetCreateBranchCommand(CreateBranchRequest request)
	{
		Assert.IsNotNull(request);

		if(request.Checkout)
		{
			var builder = new CheckoutCommand.Builder();
			switch(request.TrackingMode)
			{
				case BranchTrackingMode.NotTracking:
					builder.NoTrack();
					break;
				case BranchTrackingMode.Tracking:
					builder.Track();
					break;
			}
			if(request.CreateReflog)
			{
				builder.CreateReflog();
			}
			if(request.Orphan)
			{
				builder.Orphan();
			}
			else
			{
				builder.Branch();
			}
			builder.AddArgument(new CommandParameter(request.BranchName));
			if(!string.IsNullOrEmpty(request.StartingRevision))
			{
				builder.AddArgument(new CommandParameter(request.StartingRevision));
			}
			return builder.Build();
		}
		else
		{
			var builder = new BranchCommand.Builder(gitCLI.GitVersion);
			switch(request.TrackingMode)
			{
				case BranchTrackingMode.NotTracking:
					builder.NoTrack();
					break;
				case BranchTrackingMode.Tracking:
					builder.Track();
					break;
			}
			if(request.CreateReflog)
			{
				builder.CreateReflog();
			}
			builder.AddArgument(new CommandParameter(request.BranchName));
			if(!string.IsNullOrEmpty(request.StartingRevision))
			{
				builder.AddArgument(new CommandParameter(request.StartingRevision));
			}
			return builder.Build();
		}
	}

	public Command GetResetBranchCommand(ResetBranchRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new BranchCommand.Builder(gitCLI.GitVersion);
		builder.Force();
		builder.AddArgument(request.BranchName);
		builder.AddArgument(request.Revision);
		return builder.Build();
	}

	public Command GetDeleteBranchCommand(DeleteBranchRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new BranchCommand.Builder(gitCLI.GitVersion);
		builder.Delete(request.Force);
		if(request.Remote)
		{
			builder.Remotes();
		}
		builder.AddArgument(request.BranchName);
		return builder.Build();
	}

	public Command GetRenameBranchCommand(RenameBranchRequest request)
	{
		Assert.IsNotNull(request);

		var builder = new BranchCommand.Builder(gitCLI.GitVersion);
		builder.Move(request.Force);
		builder.AddArgument(request.OldName);
		builder.AddArgument(request.NewName);
		return builder.Build();
	}

	#endregion

	#region Tags

	public Command GetQueryTagCommand(QueryTagRequest request)
	{
		Assert.IsNotNull(request);

		string fullTagName = GitConstants.TagPrefix + request.TagName;
		return new ShowRefCommand(
			ShowRefCommand.Tags(),
			ShowRefCommand.Dereference(),
			ShowRefCommand.Hash(),
			ShowRefCommand.NoMoreOptions(),
			new CommandParameter(fullTagName));
	}

	public Command GetQueryTagsCommand(QueryTagsRequest request)
	{
		Assert.IsNotNull(request);

		return new ShowRefCommand(
			ShowRefCommand.Dereference(),
			ShowRefCommand.Tags());
	}

	public Command GetQueryTagMessageCommand(QueryTagMessageRequest request)
	{
		Assert.IsNotNull(request);

		return new CatFileCommand(
			new CommandParameter("tag"),
			new CommandParameter(request.TagName));
	}

	public Command GetDescribeCommand(DescribeRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(2);
		args.Add(DescribeCommand.Tags());
		if(request.Revision != null)
		{
			args.Add(new CommandParameter(request.Revision));
		}
		return new DescribeCommand(args);
	}

	public Command GetCreateTagCommand(CreateTagRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(5);
		if(request.TagType == TagType.Annotated)
		{
			if(request.Signed)
			{
				if(request.KeyId is not null)
				{
					args.Add(TagCommand.SignByKey(request.KeyId));
				}
				else
				{
					args.Add(TagCommand.SignByEmail());
				}
			}
			else
			{
				args.Add(TagCommand.Annotate());
			}
		}
		if(request.Force)
		{
			args.Add(TagCommand.Force());
		}
		var message = request.Message;
		switch(message.Type)
		{
			case MessageSpecificationType.Text when message.Text is { Length: not 0 } text:
				args.Add(TagCommand.Message(text));
				break;
			case MessageSpecificationType.File when message.File is { Length: not 0 } file:
				args.Add(TagCommand.MessageFromFile(file));
				break;
		}
		args.Add(new CommandParameter(request.TagName));
		if(request.TaggedObject is not null)
		{
			args.Add(new CommandParameter(request.TaggedObject));
		}
		return new TagCommand(args);
	}

	public Command GetDeleteTagCommand(DeleteTagRequest request)
	{
		Assert.IsNotNull(request);

		return new TagCommand(
			TagCommand.Delete(),
			new CommandParameter(request.TagName));
	}

	public Command GetVerifyTagsCommand(VerifyTagsRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(1 + request.TagNames.Count);
		args.Add(TagCommand.Verify());
		foreach(var tagName in request.TagNames)
		{
			args.Add(new CommandParameter(tagName));
		}
		return new TagCommand(args);
	}

	public Command GetRunMergeToolCommand(RunMergeToolRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(request.Files.Count + 2);
		if(!string.IsNullOrEmpty(request.Tool))
		{
			args.Add(MergeToolCommand.Tool(request.Tool!));
		}
		args.Add(MergeToolCommand.NoPrompt());
		if(request.Files is { Count: not 0 } files)
		{
			foreach(var file in files)
			{
				args.Add(new PathCommandArgument(file));
			}
		}
		return new MergeToolCommand(args);
	}

	public Command GetAddSubmoduleCommand(AddSubmoduleRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		args.Add(SubmoduleCommand.Add());
		if(request.Branch is not null)
		{
			args.Add(SubmoduleCommand.Branch(request.Branch));
		}
		if(request.Force)
		{
			args.Add(SubmoduleCommand.Force());
		}
		if(request.ReferenceRepository is not null)
		{
			args.Add(SubmoduleCommand.Reference(request.ReferenceRepository));
		}
		args.Add(CommandFlag.NoMoreOptions);
		args.Add(new CommandParameter(request.Repository));
		if(request.Path is not null)
		{
			var path = request.Path.Replace('\\', '/').Trim('/');
			args.Add(new PathCommandArgument(path));
		}
		return new SubmoduleCommand(args);
	}

	public Command GetUpdateSubmoduleCommand(UpdateSubmoduleRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		args.Add(SubmoduleCommand.Update());
		if(request.Init)
		{
			args.Add(SubmoduleCommand.InitFlag());
		}
		if(request.NoFetch)
		{
			args.Add(SubmoduleCommand.NoFetch());
		}
		switch(request.Mode)
		{
			case SubmoduleUpdateMode.Merge:
				args.Add(SubmoduleCommand.Merge());
				break;
			case SubmoduleUpdateMode.Rebase:
				args.Add(SubmoduleCommand.Rebase());
				break;
		}
		if(request.Recursive)
		{
			args.Add(SubmoduleCommand.Recursive());
		}
		if(!string.IsNullOrEmpty(request.Path))
		{
			args.Add(SubmoduleCommand.NoMoreOptions());
			args.Add(new PathCommandArgument(request.Path));
		}
		return new SubmoduleCommand(args);
	}

	public Command GetSyncSubmoduleCommand(SyncSubmoduleRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>();
		args.Add(SubmoduleCommand.Sync());
		if(request.Recursive)
		{
			args.Add(SubmoduleCommand.Recursive());
		}
		if(request.Submodules is { Count: not 0 } submodules)
		{
			args.Add(SubmoduleCommand.NoMoreOptions());
			foreach(var submodule in submodules)
			{
				args.Add(new PathCommandArgument(submodule));
			}
		}

		return new SubmoduleCommand(args);
	}

	#endregion

	#region Config

	private static void InsertConfigFileSpecifier(IList<ICommandArgument> args, BaseConfigRequest request)
	{
		switch(request.ConfigFile)
		{
			case ConfigFile.Repository:
			case ConfigFile.Other:
				if(request.FileName is not null)
				{
					args.Add(ConfigCommand.File(request.FileName));
				}
				break;
			case ConfigFile.LocalSystem:
				args.Add(ConfigCommand.System());
				break;
			case ConfigFile.CurrentUser:
				args.Add(ConfigCommand.Global());
				break;
		}
	}

	public Command GetQueryConfigParameterCommand(QueryConfigParameterRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(2);
		InsertConfigFileSpecifier(args, request);
		args.Add(new CommandParameter(request.ParameterName));
		return new ConfigCommand(args);
	}

	public Command GetQueryConfigCommand(QueryConfigRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(3);
		args.Add(ConfigCommand.NullTerminate());
		args.Add(ConfigCommand.List());
		InsertConfigFileSpecifier(args, request);
		return new ConfigCommand(args);
	}

	public Command GetAddConfigValueCommand(AddConfigValueRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(4);
		InsertConfigFileSpecifier(args, request);
		args.Add(ConfigCommand.Add());
		args.Add(new CommandParameter(request.ParameterName));
		args.Add(new CommandParameter(request.ParameterValue.SurroundWith("\"", "\"")));
		return new ConfigCommand(args);
	}

	public Command GetSetConfigValueCommand(SetConfigValueRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(3);
		InsertConfigFileSpecifier(args, request);
		args.Add(new CommandFlag(request.ParameterName));
		args.Add(new CommandFlag(request.ParameterValue.SurroundWith("\"", "\"")));
		return new ConfigCommand(args);
	}

	public Command GetUnsetConfigValueCommand(UnsetConfigValueRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(3);
		InsertConfigFileSpecifier(args, request);
		args.Add(ConfigCommand.Unset());
		args.Add(new CommandParameter(request.ParameterName));
		return new ConfigCommand(args);
	}

	public Command GetRenameConfigSectionCommand(RenameConfigSectionRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(2);
		InsertConfigFileSpecifier(args, request);
		args.Add(ConfigCommand.RenameSection(request.OldName, request.NewName));
		return new ConfigCommand(args);
	}

	public Command GetDeleteConfigSectionCommand(DeleteConfigSectionRequest request)
	{
		Assert.IsNotNull(request);

		var args = new List<ICommandArgument>(2);
		InsertConfigFileSpecifier(args, request);
		args.Add(ConfigCommand.RemoveSection(request.SectionName));
		return new ConfigCommand(args);
	}

	#endregion
}
