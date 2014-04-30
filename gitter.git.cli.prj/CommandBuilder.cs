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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	sealed class CommandBuilder
	{
		private readonly GitCLI _gitCLI;

		public CommandBuilder(GitCLI gitCLI)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");

			_gitCLI = gitCLI;
		}

		private static readonly Version SubmodulesStatusArgVersion = new Version(1, 7, 2, 3);

		public Command GetInitCommand(InitRepositoryParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(3);
			if(parameters.Bare)
			{
				args.Add(InitCommand.Bare());
			}
			if(!string.IsNullOrEmpty(parameters.Template))
			{
				args.Add(InitCommand.Template(parameters.Template));
			}
			return new InitCommand(args);
		}

		public Command GetCloneCommand(CloneRepositoryParameters parameters, bool isAsync)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			if(!string.IsNullOrEmpty(parameters.Template))
			{
				args.Add(CloneCommand.Template(parameters.Template));
			}
			if(parameters.NoCheckout)
			{
				args.Add(CloneCommand.NoCheckout());
			}
			if(parameters.Bare)
			{
				args.Add(CloneCommand.Bare());
			}
			if(parameters.Mirror)
			{
				args.Add(CloneCommand.Mirror());
			}
			if(!string.IsNullOrEmpty(parameters.RemoteName))
			{
				args.Add(CloneCommand.Origin(parameters.RemoteName));
			}
			if(parameters.Shallow)
			{
				args.Add(CloneCommand.Depth(parameters.Depth));
			}
			if(parameters.Recursive)
			{
				args.Add(CloneCommand.Recursive());
			}
			if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				args.Add(CloneCommand.Progress());
			}
			args.Add(CloneCommand.NoMoreOptions());
			args.Add(new PathCommandArgument(parameters.Url));
			args.Add(new PathCommandArgument(parameters.Path));

			return new CloneCommand(args);
		}

		public Command GetDereferenceByNameCommand(string refName)
		{
			return new LogCommand(
				LogCommand.MaxCount(1),
				new CommandParameter(refName),
				GetRevisionFormatArgument());
		}

		public Command GetQueryStashTopCommand(QueryStashTopParameters parameters)
		{
			Assert.IsNotNull(parameters);

			if(parameters.LoadCommitInfo)
			{
				return GetDereferenceByNameCommand(GitConstants.StashFullName);
			}
			else
			{
				return new ShowRefCommand(
					ShowRefCommand.Verify(),
					new CommandParameter(GitConstants.StashFullName));
			}
		}

		public Command GetQueryStatusCommand(QueryStatusParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(7);
			args.Add(StatusCommand.Porcelain());
			args.Add(StatusCommand.UntrackedFiles(parameters.UntrackedFilesMode));
			args.Add(StatusCommand.NullTerminate());
			if(_gitCLI.GitVersion >= SubmodulesStatusArgVersion)
			{
				if(parameters.IgnoreSubmodulesMode != StatusIgnoreSubmodulesMode.Default)
				{
					args.Add(StatusCommand.IgnoreSubmodules(parameters.IgnoreSubmodulesMode));
				}
			}
			if(!string.IsNullOrEmpty(parameters.Path))
			{
				args.Add(StatusCommand.NoMoreOptions());
				args.Add(new PathCommandArgument(parameters.Path));
			}
			return new StatusCommand(args);
		}

		public Command GetQueryObjectsCommand(QueryObjectsParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(parameters.Objects.Count);
			foreach(var obj in parameters.Objects)
			{
				args.Add(new CommandParameter(obj));
			}
			return new ShowCommand(args);
		}

		public Command GetQueryRevisionsCommand(QueryRevisionsParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(30);
			InsertQueryRevisionsParameters(parameters, args, GetRevisionFormatArgument());
			return new LogCommand(args);
		}

		public Command GetQueryRevisionGraphCommand(QueryRevisionsParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(30);
			InsertQueryRevisionsParameters(parameters, args, LogCommand.Format("%H%n%P"));
			return new LogCommand(args);
		}

		public Command GetQueryRevisionCommand(QueryRevisionParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new LogCommand(
				LogCommand.MaxCount(1),
				new CommandParameter(parameters.SHA1.ToString()),
				GetRevisionDataFormatArgument());
		}

		public Command GetQueryReflogCommand(QueryReflogParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(5);
			args.Add(LogCommand.WalkReflogs());
			if(parameters.MaxCount != 0)
			{
				args.Add(LogCommand.MaxCount(parameters.MaxCount));
			}
			args.Add(LogCommand.NullTerminate());
			args.Add(GetReflogFormatArgument());
			if(parameters.Reference != null)
			{
				args.Add(new CommandParameter(parameters.Reference));
			}
			return new LogCommand(args);
		}

		private ICommandArgument GetReflogFormatArgument()
		{
			if(GitFeatures.LogFormatBTag.IsAvailableFor(_gitCLI))
			{
				return LogCommand.Format("%gd%n%gs%n%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%B");
			}
			else
			{
				return LogCommand.Format("%gd%n%gs%n%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%s%n%n%b");
			}
		}

		private ICommandArgument GetRevisionFormatArgument()
		{
			if(GitFeatures.LogFormatBTag.IsAvailableFor(_gitCLI))
			{
				return LogCommand.Format("%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%B");
			}
			else
			{
				return LogCommand.Format("%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%s%n%n%b");
			}
		}

		private ICommandArgument GetRevisionDataFormatArgument()
		{
			if(GitFeatures.LogFormatBTag.IsAvailableFor(_gitCLI))
			{
				return LogCommand.Format("%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%B");
			}
			else
			{
				return LogCommand.Format("%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%s%n%n%b");
			}
		}

		private static void InsertQueryRevisionsParameters(QueryRevisionsParameters parameters, IList<ICommandArgument> args, ICommandArgument format)
		{
			#region Commit Limiting

			if(parameters.MaxCount != 0)
			{
				args.Add(LogCommand.MaxCount(parameters.MaxCount));
			}
			if(parameters.Skip != 0)
			{
				args.Add(LogCommand.Skip(parameters.Skip));
			}

			if(parameters.SinceDate.HasValue)
			{
				args.Add(LogCommand.Since(parameters.SinceDate.Value));
			}
			if(parameters.UntilDate.HasValue)
			{
				args.Add(LogCommand.Until(parameters.UntilDate.Value));
			}

			if(parameters.AuthorPattern != null)
			{
				args.Add(LogCommand.Author(parameters.AuthorPattern));
			}
			if(parameters.CommitterPattern != null)
			{
				args.Add(LogCommand.Committer(parameters.CommitterPattern));
			}
			if(parameters.MessagePattern != null)
			{
				args.Add(LogCommand.Grep(parameters.MessagePattern));
			}
			if(parameters.AllMatch)
			{
				args.Add(LogCommand.AllMatch());
			}
			if(parameters.RegexpIgnoreCase)
			{
				args.Add(LogCommand.RegexpIgnoreCase());
			}
			if(parameters.RegexpExtended)
			{
				args.Add(LogCommand.ExtendedRegexp());
			}
			if(parameters.RegexpFixedStrings)
			{
				args.Add(LogCommand.FixedStrings());
			}
			if(parameters.RemoveEmpty)
			{
				args.Add(LogCommand.RemoveEmpty());
			}
			switch(parameters.MergesMode)
			{
				case RevisionMergesQueryMode.MergesOnly:
					args.Add(LogCommand.Merges());
					break;
				case RevisionMergesQueryMode.NoMerges:
					args.Add(LogCommand.NoMerges());
					break;
			}
			if(parameters.Follow)
			{
				args.Add(LogCommand.Follow());
			}

			if(parameters.Not)
			{
				args.Add(LogCommand.Not());
			}
			if(parameters.All)
			{
				args.Add(LogCommand.All());
			}

			if(parameters.ReferencesGlob != null)
			{
				args.Add(LogCommand.Glob(parameters.ReferencesGlob));
			}
			if(parameters.Branches != null)
			{
				args.Add(LogCommand.Branches(parameters.Branches));
			}
			if(parameters.Tags != null)
			{
				args.Add(LogCommand.Tags(parameters.Tags));
			}
			if(parameters.Remotes != null)
			{
				args.Add(LogCommand.Remotes(parameters.Remotes));
			}

			#endregion

			#region History Simplification

			if(parameters.FirstParent)
			{
				args.Add(LogCommand.FirstParent());
			}
			if(parameters.SimplifyByDecoration)
			{
				args.Add(LogCommand.SimplifyByDecoration());
			}
			if(parameters.EnableParentsRewriting)
			{
				args.Add(LogCommand.Parents());
			}

			#endregion

			#region Ordering

			switch(parameters.Order)
			{
				case RevisionQueryOrder.DateOrder:
					args.Add(LogCommand.DateOrder());
					break;
				case RevisionQueryOrder.TopoOrder:
					args.Add(LogCommand.TopoOrder());
					break;
			}

			if(parameters.Reverse)
			{
				args.Add(LogCommand.Reverse());
			}

			#endregion

			#region Formatting

			args.Add(LogCommand.NullTerminate());
			if(format != null)
			{
				args.Add(format);
			}

			#endregion

			if(parameters.Since != null && parameters.Until != null)
			{
				args.Add(new CommandParameter(parameters.Since + ".." + parameters.Until));
			}

			if(parameters.References != null)
			{
				foreach(var reference in parameters.References)
				{
					args.Add(new CommandParameter(reference));
				}
			}

			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(CommandFlag.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
		}

		public Command GetQueryNotesCommand(QueryNotesParameters parameters)
		{
			return new NotesCommand(NotesCommand.List());
		}

		public Command GetDereferenceCommand(DereferenceParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new LogCommand(
				new CommandParameter(parameters.Reference),
				LogCommand.MaxCount(1),
				parameters.LoadRevisionData ?
					GetRevisionFormatArgument() : LogCommand.Format("%H"));
		}

		public Command GetCommitCommand(CommitParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(12);
			if(parameters.All)
			{
				args.Add(CommitCommand.All());
			}
			if(parameters.SignOff)
			{
				args.Add(CommitCommand.SignOff());
			}
			if(parameters.Amend)
			{
				args.Add(CommitCommand.Amend());
			}
			if(!string.IsNullOrEmpty(parameters.ReuseMessageFrom))
			{
				args.Add(CommitCommand.ReuseMessage(parameters.ReuseMessageFrom));
			}
			if(!string.IsNullOrEmpty(parameters.Message))
			{
				args.Add(CommitCommand.Message(parameters.Message));
			}
			if(!string.IsNullOrEmpty(parameters.MessageFileName))
			{
				args.Add(CommitCommand.File(parameters.MessageFileName));
			}
			if(parameters.ResetAuthor)
			{
				args.Add(CommitCommand.ResetAuthor());
			}
			if(parameters.AllowEmpty)
			{
				args.Add(CommitCommand.AllowEmpty());
			}
			if(parameters.AllowEmptyMessage)
			{
				args.Add(CommitCommand.AllowEmptyMessage());
			}
			if(parameters.NoVerify)
			{
				args.Add(CommitCommand.NoVerify());
			}
			if(!string.IsNullOrEmpty(parameters.Author))
			{
				args.Add(CommitCommand.Author(parameters.Author));
			}
			if(parameters.AuthorDate.HasValue)
			{
				args.Add(CommitCommand.Date(parameters.AuthorDate.Value));
			}
			return new CommitCommand(args);
		}

		public Command GetQueryFilesToAddCommand(AddFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 6);
			args.Add(AddCommand.DryRun());
			InsertAddFilesParameters(parameters, args);
			return new AddCommand(args);
		}

		public Command GetQueryFilesToRemoveCommand(RemoveFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 5);
			args.Add(RmCommand.DryRun());
			InsertRemoveFilesParameters(parameters, args);
			return new RmCommand(args);
		}

		public Command GetQueryFilesToCleanCommand(CleanFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(
				(parameters.ExcludePatterns != null ? parameters.ExcludePatterns.Count : 0) +
				(parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 4);
			args.Add(CleanCommand.DryRun());
			InsertCleanFilesParameters(parameters, args);
			return new CleanCommand(args);
		}

		public Command GetAddFilesCommand(AddFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 5);
			InsertAddFilesParameters(parameters, args);
			return new AddCommand(args);
		}

		private static void InsertAddFilesParameters(AddFilesParameters parameters, IList<ICommandArgument> args)
		{
			if(parameters.Force)
			{
				args.Add(AddCommand.Force());
			}
			switch(parameters.Mode)
			{
				case AddFilesMode.All:
					args.Add(AddCommand.All());
					break;
				case AddFilesMode.Update:
					args.Add(AddCommand.Update());
					break;
			}
			if(parameters.IntentToAdd)
			{
				args.Add(AddCommand.IntentToAdd());
			}
			if(parameters.Refresh)
			{
				args.Add(AddCommand.Refresh());
			}
			if(parameters.IgnoreErrors)
			{
				args.Add(AddCommand.IgnoreErrors());
			}
			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(AddCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
		}

		public Command GetRemoveFilesCommand(RemoveFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 4);
			InsertRemoveFilesParameters(parameters, args);
			return new RmCommand(args);
		}

		private static void InsertRemoveFilesParameters(RemoveFilesParameters parameters, IList<ICommandArgument> args)
		{
			if(parameters.Force)
			{
				args.Add(RmCommand.Force());
			}
			if(parameters.Recursive)
			{
				args.Add(RmCommand.Recursive());
			}
			if(parameters.Cached)
			{
				args.Add(RmCommand.Cached());
			}
			if(parameters.IgnoreUnmatch)
			{
				args.Add(RmCommand.IgnoreUnmatch());
			}
			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(RmCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
		}

		public Command GetCleanFilesCommand(CleanFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(
				(parameters.ExcludePatterns != null ? parameters.ExcludePatterns.Count : 0) +
				(parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 3);
			InsertCleanFilesParameters(parameters, args);
			return new CleanCommand(args);
		}

		private void InsertCleanFilesParameters(CleanFilesParameters parameters, IList<ICommandArgument> args)
		{
			if(parameters.Force)
			{
				args.Add(CleanCommand.Force());
			}
			if(parameters.RemoveDirectories)
			{
				args.Add(CleanCommand.Directories());
			}
			switch(parameters.Mode)
			{
				case CleanFilesMode.IncludeIgnored:
					args.Add(CleanCommand.IncludeIgnored());
					break;
				case CleanFilesMode.OnlyIgnored:
					args.Add(CleanCommand.ExcludeUntracked());
					break;
			}
			if(parameters.ExcludePatterns != null && GitFeatures.CleanExcludeOption.IsAvailableFor(_gitCLI))
			{
				foreach(var pattern in parameters.ExcludePatterns)
				{
					if(!string.IsNullOrEmpty(pattern))
					{
						args.Add(CleanCommand.Exclude(pattern));
					}
				}
			}
			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				bool addedAnyPath = false;
				foreach(var path in parameters.Paths)
				{
					if(string.IsNullOrWhiteSpace(path))
					{
						continue;
					}
					if(addedAnyPath)
					{
						args.Add(CleanCommand.NoMoreOptions());
						addedAnyPath = true;
					}
					args.Add(new PathCommandArgument(path));
				}
			}
		}

		public Command GetResetFilesCommand(ResetFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 1);
			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandParameter(parameters.Revision));
			}
			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(ResetCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
			return new ResetCommand(args);
		}

		public Command GetAppendNoteCommand(AppendNoteParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new NotesCommand(
				NotesCommand.Append(),
				NotesCommand.Message(parameters.Message),
				new CommandParameter(parameters.Revision));
		}

		public Command GetPruneNotesCommand(PruneNotesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new NotesCommand(NotesCommand.Prune());
		}

		public Command GetDiffCommand(QueryDiffParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			InsertDiffParameters1(parameters, args);
			if(!string.IsNullOrEmpty(parameters.Revision1))
			{
				args.Add(new CommandParameter(parameters.Revision1));
			}
			if(!string.IsNullOrEmpty(parameters.Revision2))
			{
				args.Add(new CommandParameter(parameters.Revision2));
			}
			InsertDiffParameters2(parameters, args);
			return new DiffCommand(args);
		}

		public Command GetQueryRevisionDiffCommand(QueryRevisionDiffParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			args.Add(LogCommand.MaxCount(1));
			InsertDiffParameters1(parameters, args);
			args.Add(new CommandFlag("-c"));
			args.Add(new CommandParameter(parameters.Revision));
			InsertDiffParameters2(parameters, args);
			return new LogCommand(args);
		}

		public Command GetQueryStashDiffCommand(QueryRevisionDiffParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			args.Add(StashCommand.Show());
			InsertDiffParameters1(parameters, args);
			args.Add(new CommandParameter(parameters.Revision));
			InsertDiffParameters2(parameters, args);
			return new StashCommand(args);
		}

		private static void InsertDiffParameters1(BaseQueryDiffParameters parameters, IList<ICommandArgument> args)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(args);

			args.Add(DiffCommand.Patch());
			args.Add(DiffCommand.FullIndex());
			args.Add(DiffCommand.NoColor());
			if(parameters.Context != -1)
			{
				args.Add(DiffCommand.Unified(parameters.Context));
			}
			if(parameters.Cached)
			{
				args.Add(DiffCommand.Cached());
			}
			if(parameters.Binary)
			{
				args.Add(DiffCommand.Binary());
			}
			if(parameters.Patience)
			{
				args.Add(DiffCommand.Patience());
			}
			if(parameters.IgnoreAllSpace)
			{
				args.Add(DiffCommand.IgnoreAllSpace());
			}
			if(parameters.IgnoreSpaceAtEOL)
			{
				args.Add(DiffCommand.IgnoreSpaceAtEOL());
			}
			if(parameters.IgnoreSpaceChange)
			{
				args.Add(DiffCommand.IgnoreSpaceChange());
			}
			if(parameters.SwapInputs)
			{
				args.Add(DiffCommand.SwapInputs());
			}
			if(parameters.EnableTextConvFilters.HasValue)
			{
				if(parameters.EnableTextConvFilters.Value)
				{
					args.Add(DiffCommand.TextConv());
				}
				else
				{
					args.Add(DiffCommand.NoTextConv());
				}
			}
			if(parameters.EnableExternalDiffDrivers.HasValue)
			{
				if(parameters.EnableExternalDiffDrivers.Value)
				{
					args.Add(DiffCommand.ExtDiff());
				}
				else
				{
					args.Add(DiffCommand.NoExtDiff());
				}
			}
			if(parameters.TreatAllFilesAsText)
			{
				args.Add(DiffCommand.Text());
			}
			if(parameters.FindRenames.HasValue)
			{
				if(parameters.FindRenames.Value.IsSpecified)
				{
					args.Add(DiffCommand.FindRenames(parameters.FindRenames.Value.Similarity));
				}
				else
				{
					args.Add(DiffCommand.FindRenames());
				}
			}
			if(parameters.FindCopies.HasValue)
			{
				if(parameters.FindCopies.Value.IsSpecified)
				{
					args.Add(DiffCommand.FindCopies(parameters.FindCopies.Value.Similarity));
				}
				else
				{
					args.Add(DiffCommand.FindCopies());
				}
			}
		}

		private static void InsertDiffParameters2(BaseQueryDiffParameters parameters, IList<ICommandArgument> args)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(args);

			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(DiffCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
		}

		public Command GetQueryStashCommand(QueryStashParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new LogCommand(
				LogCommand.WalkReflogs(),
				LogCommand.NullTerminate(),
				GetRevisionFormatArgument(),
				new CommandParameter(GitConstants.StashFullName));
		}

		public Command GetStashSaveCommand(StashSaveParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			args.Add(StashCommand.Save());
			args.Add(parameters.KeepIndex ? StashCommand.KeepIndex() : StashCommand.NoKeepIndex());
			if(parameters.IncludeUntracked && GitFeatures.StashIncludeUntrackedOption.IsAvailableFor(_gitCLI))
			{
				args.Add(StashCommand.IncludeUntracked());
			}
			if(!string.IsNullOrWhiteSpace(parameters.Message))
			{
				args.Add(new CommandParameter(parameters.Message.SurroundWithDoubleQuotes()));
			}

			return new StashCommand(args);
		}

		public Command GetStashApplyCommand(StashApplyParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(3);
			args.Add(StashCommand.Apply());
			if(parameters.RestoreIndex)
			{
				args.Add(StashCommand.Index());
			}
			if(parameters.StashName != null)
			{
				args.Add(new CommandParameter(parameters.StashName));
			}
			return new StashCommand(args);
		}

		public Command GetStashPopCommand(StashPopParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(3);
			args.Add(StashCommand.Pop());
			if(parameters.RestoreIndex)
			{
				args.Add(StashCommand.Index());
			}
			if(parameters.StashName != null)
			{
				args.Add(new CommandParameter(parameters.StashName));
			}
			return new StashCommand(args);
		}

		public Command GetStashToBranchCommand(StashToBranchParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(3);
			args.Add(StashCommand.Branch());
			args.Add(new CommandParameter(parameters.BranchName));
			if(parameters.StashName != null)
			{
				args.Add(new CommandParameter(parameters.StashName));
			}

			return new StashCommand(args);
		}

		public Command GetStashDropCommand(StashDropParameters parameters)
		{
			Assert.IsNotNull(parameters);

			if(!string.IsNullOrWhiteSpace(parameters.StashName))
			{
				return new StashCommand(
					StashCommand.Drop(),
					new CommandParameter(parameters.StashName));
			}
			else
			{
				return new StashCommand(
					StashCommand.Drop());
			}
		}

		public Command GetStashClearCommand(StashClearParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new StashCommand(StashCommand.Clear());
		}

		public Command GetQueryTreeContentCommand(QueryTreeContentParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			if(parameters.OnlyTrees)
			{
				args.Add(LsTreeCommand.Directories());
			}
			if(parameters.Recurse)
			{
				args.Add(LsTreeCommand.Recurse());
			}
			args.Add(LsTreeCommand.Tree());
			args.Add(LsTreeCommand.FullName());
			args.Add(LsTreeCommand.Long());
			args.Add(LsTreeCommand.NullTerminate());
			args.Add(new CommandParameter(parameters.TreeId));
			if(parameters.Paths != null)
			{
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
			return new LsTreeCommand(args);
		}

		public Command GetApplyPatchCommand(ApplyPatchParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			switch(parameters.ApplyTo)
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
			if(parameters.Reverse)
			{
				args.Add(ApplyCommand.Reverse());
			}
			args.Add(ApplyCommand.UnidiffZero());
			if(parameters.Patches != null)
			{
				foreach(var patch in parameters.Patches)
				{
					args.Add(new PathCommandArgument(patch));
				}
			}
			return new ApplyCommand(args);
		}

		public Command GetCheckoutCommand(CheckoutParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			if(parameters.Force)
			{
				args.Add(CheckoutCommand.Force());
			}
			if(parameters.Merge)
			{
				args.Add(CheckoutCommand.Merge());
			}
			args.Add(new CommandParameter(parameters.Revision));
			return new CheckoutCommand(args);
		}

		public Command GetCheckoutFilesCommand(CheckoutFilesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 2);
			switch(parameters.Mode)
			{
				case CheckoutFileMode.Ours:
					args.Add(CheckoutCommand.Ours());
					break;
				case CheckoutFileMode.Theirs:
					args.Add(CheckoutCommand.Theirs());
					break;
				case CheckoutFileMode.Merge:
					args.Add(CheckoutCommand.Merge());
					break;
				case CheckoutFileMode.IgnoreUnmergedEntries:
					args.Add(CheckoutCommand.Force());
					break;
			}
			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandParameter(parameters.Revision));
			}
			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(CheckoutCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
			return new CheckoutCommand(args);
		}

		public Command GetQueryBlobBytesCommand(QueryBlobBytesParameters parameters)
		{
			return new CatFileCommand(
				new CommandParameter(GitConstants.BlobObjectType),
				new CommandParameter(parameters.Treeish + ":" + parameters.ObjectName));
		}

		public Command GetRevertCommand(RevertParameters parameters)
		{
			Assert.IsNotNull(parameters);

			if(parameters.Control.HasValue)
			{
				switch(parameters.Control)
				{
					case RevertControl.Continue:
						return new RevertCommand(RevertCommand.Continue());
					case RevertControl.Quit:
						return new RevertCommand(RevertCommand.Quit());
					case RevertControl.Abort:
						return new RevertCommand(RevertCommand.Abort());
					default:
						throw new ArgumentException("Unknown enum value.", "control");
				}
			}
			else
			{
				var args = new List<ICommandArgument>();
				if(parameters.NoCommit)
				{
					args.Add(RevertCommand.NoCommit());
				}
				if(parameters.Mainline > 0)
				{
					args.Add(RevertCommand.Mainline(parameters.Mainline));
				}
				foreach(var rev in parameters.Revisions)
				{
					args.Add(new CommandParameter(rev));
				}
				return new RevertCommand(args);
			}
		}

		public Command GetRevertCommand(RevertControl control)
		{
			switch(control)
			{
				case RevertControl.Continue:
					return new RevertCommand(RevertCommand.Continue());
				case RevertControl.Quit:
					return new RevertCommand(RevertCommand.Quit());
				case RevertControl.Abort:
					return new RevertCommand(RevertCommand.Abort());
				default:
					throw new ArgumentException("Unknown enum value.", "control");
			}
		}

		public Command GetCherryPickCommand(CherryPickParameters parameters)
		{
			Assert.IsNotNull(parameters);

			if(parameters.Control.HasValue)
			{
				switch(parameters.Control.Value)
				{
					case CherryPickControl.Continue:
						return new CherryPickCommand(CherryPickCommand.Continue());
					case CherryPickControl.Quit:
						return new CherryPickCommand(CherryPickCommand.Quit());
					case CherryPickControl.Abort:
						return new CherryPickCommand(CherryPickCommand.Abort());
					default:
						throw new ArgumentException("Unknown enum value.", "control");
				}
			}
			else
			{
				var args = new List<ICommandArgument>();
				if(parameters.NoCommit)
				{
					args.Add(CherryPickCommand.NoCommit());
				}
				if(parameters.Mainline > 0)
				{
					args.Add(CherryPickCommand.Mainline(parameters.Mainline));
				}
				if(parameters.SignOff)
				{
					args.Add(CherryPickCommand.SignOff());
				}
				if(parameters.FastForward)
				{
					args.Add(CherryPickCommand.FastForward());
				}
				if(parameters.AllowEmpty)
				{
					args.Add(CherryPickCommand.AllowEmpty());
				}
				if(parameters.AllowEmptyMessage)
				{
					args.Add(CherryPickCommand.AllowEmptyMessage());
				}
				if(parameters.KeepRedundantCommits)
				{
					args.Add(CherryPickCommand.KeepRedundantCommits());
				}
				foreach(var rev in parameters.Revisions)
				{
					args.Add(new CommandParameter(rev));
				}
				return new CherryPickCommand(args);
			}
		}

		public Command GetCherryPickCommand(CherryPickControl control)
		{
			switch(control)
			{
				case CherryPickControl.Continue:
					return new CherryPickCommand(CherryPickCommand.Continue());
				case CherryPickControl.Quit:
					return new CherryPickCommand(CherryPickCommand.Quit());
				case CherryPickControl.Abort:
					return new CherryPickCommand(CherryPickCommand.Abort());
				default:
					throw new ArgumentException("Unknown enum value.", "control");
			}
		}

		public Command GetResetCommand(ResetParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			args.Add(ResetCommand.Mode(parameters.Mode));
			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandParameter(parameters.Revision));
			}
			return new ResetCommand(args);
		}

		public Command GetMergeCommand(MergeParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			if(parameters.NoCommit)
			{
				args.Add(MergeCommand.NoCommit());
			}
			if(parameters.Squash)
			{
				args.Add(MergeCommand.Squash());
			}
			if(parameters.NoFastForward)
			{
				args.Add(MergeCommand.NoFastForward());
			}
			if(!string.IsNullOrEmpty(parameters.Message))
			{
				var message = parameters.Message.Replace("\"", "\\\"");
				args.Add(MergeCommand.Message(message));
			}
			if(parameters.Revisions != null)
			{
				foreach(var rev in parameters.Revisions)
				{
					args.Add(new CommandParameter(rev));
				}
			}
			return new MergeCommand(args);
		}

		public Command GetRebaseCommand(RebaseParameters parameters)
		{
			Assert.IsNotNull(parameters);

			if(parameters.Control.HasValue)
			{
				ICommandArgument arg;
				switch(parameters.Control.Value)
				{
					case RebaseControl.Abort:
						arg = RebaseCommand.Abort();
						break;
					case RebaseControl.Continue:
						arg = RebaseCommand.Continue();
						break;
					case RebaseControl.Skip:
						arg = RebaseCommand.Skip();
						break;
					default:
						throw new ArgumentException("control");
				}
				return new RebaseCommand(arg);
			}
			else
			{
				var args = new List<ICommandArgument>();
				if(!string.IsNullOrEmpty(parameters.NewBase))
				{
					args.Add(RebaseCommand.Onto(parameters.NewBase));
				}
				args.Add(new CommandParameter(parameters.Upstream));
				if(!string.IsNullOrEmpty(parameters.Branch))
				{
					args.Add(new CommandParameter(parameters.Branch));
				}
				return new RebaseCommand(args);
			}
		}

		public Command GetRebaseCommand(RebaseControl control)
		{
			ICommandArgument arg;
			switch(control)
			{
				case RebaseControl.Abort:
					arg = RebaseCommand.Abort();
					break;
				case RebaseControl.Continue:
					arg = RebaseCommand.Continue();
					break;
				case RebaseControl.Skip:
					arg = RebaseCommand.Skip();
					break;
				default:
					throw new ArgumentException("control");
			}
			return new RebaseCommand(arg);
		}

		public Command GetQueryUsersCommand(QueryUsersParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new ShortLogCommand(
				ShortLogCommand.All(),
				ShortLogCommand.Numbered(),
				ShortLogCommand.Summary(),
				ShortLogCommand.Email());
		}

		public Command GetCountObjectsCommand(CountObjectsParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new CountObjectsCommand(CountObjectsCommand.Verbose());
		}

		public Command GetGarbageCollectCommand(GarbageCollectParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new GcCommand();
		}

		public Command GetArchiveCommand(ArchiveParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(5);
			if(!string.IsNullOrEmpty(parameters.Format))
			{
				args.Add(ArchiveCommand.Format(parameters.Format));
			}
			if(!string.IsNullOrEmpty(parameters.OutputFile))
			{
				args.Add(ArchiveCommand.Output(parameters.OutputFile));
			}
			if(!string.IsNullOrEmpty(parameters.Remote))
			{
				args.Add(ArchiveCommand.Remote(parameters.Remote));
			}
			args.Add(new CommandParameter(parameters.Tree));
			if(!string.IsNullOrEmpty(parameters.Path))
			{
				args.Add(new PathCommandArgument(parameters.Path));
			}
			return new ArchiveCommand(args);
		}

		public Command GetQueryBlameCommand(QueryBlameParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			args.Add(BlameCommand.Porcelain());
			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandParameter(parameters.Revision));
			}
			args.Add(CommandFlag.NoMoreOptions());
			args.Add(new PathCommandArgument(parameters.FileName));
			return new BlameCommand(args);
		}

		public Command GetFetchCommand(FetchParameters parameters, bool isAsync)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(10);
			InsertFetchParameters(parameters, args);
			if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				args.Add(FetchCommand.Progress());
			}
			return new FetchCommand(args);
		}

		private static void InsertFetchParameters(FetchParameters parameters, IList<ICommandArgument> args)
		{
			if(parameters.All)
			{
				args.Add(FetchCommand.All());
			}
			if(parameters.Append)
			{
				args.Add(FetchCommand.Append());
			}
			if(parameters.Prune)
			{
				args.Add(FetchCommand.Prune());
			}
			if(parameters.Depth != 0)
			{
				args.Add(FetchCommand.Depth(parameters.Depth));
			}
			switch(parameters.TagFetchMode)
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
			if(parameters.KeepDownloadedPack)
			{
				args.Add(FetchCommand.Keep());
			}
			if(parameters.Force)
			{
				args.Add(FetchCommand.Force());
			}
			if(!string.IsNullOrWhiteSpace(parameters.UploadPack))
			{
				args.Add(FetchCommand.UploadPack(parameters.UploadPack));
			}
			if(!string.IsNullOrWhiteSpace(parameters.Repository))
			{
				args.Add(new CommandParameter(parameters.Repository));
			}
		}

		public Command GetPullCommand(PullParameters parameters, bool isAsync)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			InsertPullParameters(parameters, args);
			if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				args.Add(FetchCommand.Progress());
			}
			return new PullCommand(args);
		}

		private static void InsertPullParameters(PullParameters parameters, IList<ICommandArgument> args)
		{
			if(parameters.NoFastForward)
			{
				args.Add(MergeCommand.NoFastForward());
			}
			if(parameters.NoCommit)
			{
				args.Add(MergeCommand.NoCommit());
			}
			if(parameters.Squash)
			{
				args.Add(MergeCommand.Squash());
			}
			var arg = MergeCommand.Strategy(parameters.Strategy);
			if(arg != null)
			{
				args.Add(arg);
			}
			if(!string.IsNullOrEmpty(parameters.StrategyOption))
			{
				args.Add(MergeCommand.StrategyOption(parameters.StrategyOption));
			}
			InsertFetchParameters(parameters, args);
		}

		public Command GetPushCommand(PushParameters parameters, bool isAsync)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			switch(parameters.PushMode)
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
			if(!string.IsNullOrEmpty(parameters.ReceivePack))
			{
				args.Add(PushCommand.ReceivePack(parameters.ReceivePack));
			}
			if(parameters.Force)
			{
				args.Add(PushCommand.Force());
			}
			if(parameters.Delete)
			{
				args.Add(PushCommand.Delete());
			}
			if(parameters.SetUpstream)
			{
				args.Add(PushCommand.SetUpstream());
			}
			args.Add(parameters.ThinPack ? PushCommand.Thin() : PushCommand.NoThin());
			args.Add(PushCommand.Porcelain());
			if(isAsync && GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				args.Add(PushCommand.Progress());
			}
			if(!string.IsNullOrWhiteSpace(parameters.Repository))
			{
				args.Add(new CommandParameter(parameters.Repository));
			}
			if(parameters.Refspecs != null && parameters.Refspecs.Count != 0)
			{
				foreach(var refspec in parameters.Refspecs)
				{
					args.Add(new CommandParameter(refspec));
				}
			}
			return new PushCommand(args);
		}

		public Command GetQueryRemoteCommand(QueryRemoteParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new RemoteCommand(
				RemoteCommand.Show(),
				RemoteCommand.Cached(),
				new CommandParameter(parameters.RemoteName));
		}

		public Command GetQueryRemotesCommand(QueryRemotesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new RemoteCommand(RemoteCommand.Verbose());
		}

		public Command GetAddRemoteCommand(AddRemoteParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Branches != null ? parameters.Branches.Count : 0) + 6);
			args.Add(RemoteCommand.Add());
			if(parameters.Branches != null && parameters.Branches.Count != 0)
			{
				foreach(var branch in parameters.Branches)
				{
					args.Add(RemoteCommand.TrackBranch(branch));
				}
			}
			if(!string.IsNullOrEmpty(parameters.MasterBranch))
			{
				args.Add(RemoteCommand.Master(parameters.MasterBranch));
			}
			if(parameters.Fetch)
			{
				args.Add(RemoteCommand.Fetch());
			}
			switch(parameters.TagFetchMode)
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
			if(parameters.Mirror)
			{
				args.Add(RemoteCommand.Mirror());
			}
			args.Add(new CommandParameter(parameters.RemoteName));
			args.Add(new CommandParameter(parameters.Url));
			return new RemoteCommand(args);
		}

		public Command GetRenameRemoteCommand(RenameRemoteParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new RemoteCommand(
				RemoteCommand.Rename(),
				new CommandParameter(parameters.OldName),
				new CommandParameter(parameters.NewName));
		}

		public Command GetRemoveRemoteCommand(RemoveRemoteParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return RemoteCommand.FormatRemoveCommand(parameters.RemoteName);
		}

		public Command GetQueryPrunedBranchesCommand(PruneRemoteParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new RemoteCommand(
				RemoteCommand.Prune(),
				RemoteCommand.DryRun(),
				new CommandParameter(parameters.RemoteName));
		}

		public Command GetPruneRemoteCommand(PruneRemoteParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new RemoteCommand(
				RemoteCommand.Prune(),
				new CommandParameter(parameters.RemoteName));
		}

		public Command GetQueryRemoteReferencesCommand(QueryRemoteReferencesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(4);
			if(parameters.Heads)
			{
				args.Add(LsRemoteCommand.Heads());
			}
			if(parameters.Tags)
			{
				args.Add(LsRemoteCommand.Tags());
			}
			args.Add(new CommandParameter(parameters.RemoteName));
			if(!string.IsNullOrEmpty(parameters.Pattern))
			{
				args.Add(new CommandParameter(parameters.Pattern));
			}
			return new LsRemoteCommand(args);
		}

		public Command GetRemoveRemoteReferencesCommand(RemoveRemoteReferencesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(1 + parameters.References.Count);
			args.Add(new CommandParameter(parameters.RemoteName));
			foreach(var reference in parameters.References)
			{
				args.Add(new CommandParameter(":" + reference));
			}
			return new PushCommand(args);
		}

		public Command GetQueryReferencesCommand(QueryReferencesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			switch(parameters.ReferenceTypes)
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

		public Command GetQueryBranchCommand(QueryBranchParameters parameters)
		{
			Assert.IsNotNull(parameters);

			string fullName = (parameters.IsRemote ?
				GitConstants.RemoteBranchPrefix :
				GitConstants.LocalBranchPrefix) + parameters.BranchName;
			return new ShowRefCommand(
					ShowRefCommand.Verify(),
					ShowRefCommand.Heads(),
					ShowRefCommand.Hash(),
					ShowRefCommand.NoMoreOptions(),
					new CommandParameter(fullName));
		}

		public Command GetQueryBranchesCommand(QueryBranchesParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(6);
			args.Add(BranchCommand.NoColor());
			args.Add(BranchCommand.Verbose());
			args.Add(BranchCommand.NoAbbrev());
			switch(parameters.Restriction)
			{
				case QueryBranchRestriction.All:
					args.Add(BranchCommand.All());
					break;
				case QueryBranchRestriction.Remote:
					args.Add(BranchCommand.Remote());
					break;
			}
			switch(parameters.Mode)
			{
				case BranchQueryMode.Contains:
					args.Add(BranchCommand.Contains());
					break;
				case BranchQueryMode.Merged:
					args.Add(BranchCommand.Merged());
					break;
				case BranchQueryMode.NoMerged:
					args.Add(BranchCommand.NoMerged());
					break;
			}
			if(parameters.Revision != null)
			{
				args.Add(new CommandParameter(parameters.Revision));
			}

			return new BranchCommand(args);
		}

		public Command GetCreateBranchCommand(CreateBranchParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(6);
			switch(parameters.TrackingMode)
			{
				case BranchTrackingMode.NotTracking:
					args.Add(BranchCommand.NoTrack());
					break;
				case BranchTrackingMode.Tracking:
					args.Add(BranchCommand.Track());
					break;
			}
			if(parameters.CreateReflog)
			{
				args.Add(BranchCommand.RefLog());
			}
			if(parameters.Checkout)
			{
				if(parameters.Orphan)
				{
					args.Add(CheckoutCommand.Orphan());
				}
				else
				{
					args.Add(CheckoutCommand.Branch());
				}
			}
			args.Add(new CommandParameter(parameters.BranchName));
			if(!string.IsNullOrEmpty(parameters.StartingRevision))
			{
				args.Add(new CommandParameter(parameters.StartingRevision));
			}
			if(parameters.Checkout)
			{
				return new CheckoutCommand(args);
			}
			else
			{
				return new BranchCommand(args);
			}
		}

		public Command GetResetBranchCommand(ResetBranchParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new BranchCommand(
				BranchCommand.Reset(),
				new CommandParameter(parameters.BranchName),
				new CommandParameter(parameters.Revision));
		}

		public Command GetDeleteBranchCommand(DeleteBranchParameters parameters)
		{
			Assert.IsNotNull(parameters);

			if(parameters.Remote)
			{
				return new BranchCommand(
					parameters.Force ? BranchCommand.DeleteForce() : BranchCommand.Delete(),
					BranchCommand.Remote(),
					new CommandParameter(parameters.BranchName));
			}
			else
			{
				return new BranchCommand(
					parameters.Force ? BranchCommand.DeleteForce() : BranchCommand.Delete(),
					new CommandParameter(parameters.BranchName));
			}
		}

		public Command GetRenameBranchCommand(RenameBranchParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new BranchCommand(
				parameters.Force ? BranchCommand.MoveForce() : BranchCommand.Move(),
				new CommandParameter(parameters.OldName),
				new CommandParameter(parameters.NewName));
		}

		#endregion

		#region Tags

		public Command GetQueryTagCommand(QueryTagParameters parameters)
		{
			Assert.IsNotNull(parameters);

			string fullTagName = GitConstants.TagPrefix + parameters.TagName;
			return new ShowRefCommand(
				ShowRefCommand.Tags(),
				ShowRefCommand.Dereference(),
				ShowRefCommand.Hash(),
				ShowRefCommand.NoMoreOptions(),
				new CommandParameter(fullTagName));
		}

		public Command GetQueryTagsCommand(QueryTagsParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new ShowRefCommand(
				ShowRefCommand.Dereference(),
				ShowRefCommand.Tags());
		}

		public Command GetQueryTagMessageCommand(QueryTagMessageParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new CatFileCommand(
				new CommandParameter("tag"),
				new CommandParameter(parameters.TagName));
		}

		public Command GetDescribeCommand(DescribeParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(2);
			args.Add(DescribeCommand.Tags());
			if(parameters.Revision != null)
			{
				args.Add(new CommandParameter(parameters.Revision));
			}
			return new DescribeCommand(args);
		}

		public Command GetCreateTagCommand(CreateTagParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(5);
			if(parameters.TagType == TagType.Annotated)
			{
				if(parameters.Signed)
				{
					if(parameters.KeyId != null)
					{
						args.Add(TagCommand.SignByKey(parameters.KeyId));
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
			if(parameters.Force)
			{
				args.Add(TagCommand.Force());
			}
			if(!string.IsNullOrEmpty(parameters.Message))
			{
				args.Add(TagCommand.Message(parameters.Message));
			}
			else if(!string.IsNullOrEmpty(parameters.MessageFile))
			{
				args.Add(TagCommand.MessageFromFile(parameters.MessageFile));
			}
			args.Add(new CommandParameter(parameters.TagName));
			if(parameters.TaggedObject != null)
			{
				args.Add(new CommandParameter(parameters.TaggedObject));
			}
			return new TagCommand(args);
		}

		public Command GetDeleteTagCommand(DeleteTagParameters parameters)
		{
			Assert.IsNotNull(parameters);

			return new TagCommand(
				TagCommand.Delete(),
				new CommandParameter(parameters.TagName));
		}

		public Command GetVerifyTagsCommand(VerifyTagsParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(1 + parameters.TagNames.Count);
			args.Add(TagCommand.Verify());
			foreach(var tagName in parameters.TagNames)
			{
				args.Add(new CommandParameter(tagName));
			}
			return new TagCommand(args);
		}

		public Command GetRunMergeToolCommand(RunMergeToolParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>((parameters.Files != null ? parameters.Files.Count : 0) + 2);
			if(!string.IsNullOrEmpty(parameters.Tool))
			{
				args.Add(MergeToolCommand.Tool(parameters.Tool));
			}
			args.Add(MergeToolCommand.NoPrompt());
			if(parameters.Files != null && parameters.Files.Count != 0)
			{
				foreach(var file in parameters.Files)
				{
					args.Add(new PathCommandArgument(file));
				}
			}
			return new MergeToolCommand(args);
		}

		public Command GetAddSubmoduleCommand(AddSubmoduleParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			args.Add(SubmoduleCommand.Add());
			if(parameters.Branch != null)
			{
				args.Add(SubmoduleCommand.Branch(parameters.Branch));
			}
			if(parameters.Force)
			{
				args.Add(SubmoduleCommand.Force());
			}
			if(parameters.ReferenceRepository != null)
			{
				args.Add(SubmoduleCommand.Reference(parameters.ReferenceRepository));
			}
			args.Add(CommandFlag.NoMoreOptions());
			args.Add(new CommandParameter(parameters.Repository));
			if(parameters.Path != null)
			{
				var path = parameters.Path.Replace('\\', '/').Trim('/');
				args.Add(new PathCommandArgument(path));
			}
			return new SubmoduleCommand(args);
		}

		public Command GetUpdateSubmoduleCommand(SubmoduleUpdateParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>();
			args.Add(SubmoduleCommand.Update());
			if(parameters.Init)
			{
				args.Add(SubmoduleCommand.InitFlag());
			}
			if(parameters.NoFetch)
			{
				args.Add(SubmoduleCommand.NoFetch());
			}
			switch(parameters.Mode)
			{
				case SubmoduleUpdateMode.Merge:
					args.Add(SubmoduleCommand.Merge());
					break;
				case SubmoduleUpdateMode.Rebase:
					args.Add(SubmoduleCommand.Rebase());
					break;
			}
			if(parameters.Recursive)
			{
				args.Add(SubmoduleCommand.Recursive());
			}
			if(!string.IsNullOrEmpty(parameters.Path))
			{
				args.Add(SubmoduleCommand.NoMoreOptions());
				args.Add(new PathCommandArgument(parameters.Path));
			}
			return new SubmoduleCommand(args);
		}

		#endregion

		#region Config

		private static void InsertConfigFileSpecifier(IList<ICommandArgument> args, BaseConfigParameters parameters)
		{
			switch(parameters.ConfigFile)
			{
				case ConfigFile.Repository:
				case ConfigFile.Other:
					if(parameters.FileName != null)
					{
						args.Add(ConfigCommand.File(parameters.FileName));
					}
					break;
				case ConfigFile.System:
					args.Add(ConfigCommand.System());
					break;
				case ConfigFile.User:
					args.Add(ConfigCommand.Global());
					break;
			}
		}

		public Command GetQueryConfigParameterCommand(QueryConfigParameterParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(2);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(new CommandParameter(parameters.ParameterName));
			return new ConfigCommand(args);
		}

		public Command GetQueryConfigCommand(QueryConfigParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(3);
			args.Add(ConfigCommand.NullTerminate());
			args.Add(ConfigCommand.List());
			InsertConfigFileSpecifier(args, parameters);
			return new ConfigCommand(args);
		}

		public Command GetAddConfigValueCommand(AddConfigValueParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(4);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.Add());
			args.Add(new CommandParameter(parameters.ParameterName));
			args.Add(new CommandParameter(parameters.ParameterValue.SurroundWith("\"", "\"")));
			return new ConfigCommand(args);
		}

		public Command GetSetConfigValueCommand(SetConfigValueParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(3);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(new CommandFlag(parameters.ParameterName));
			args.Add(new CommandFlag(parameters.ParameterValue.SurroundWith("\"", "\"")));
			return new ConfigCommand(args);
		}

		public Command GetUnsetConfigValueCommand(UnsetConfigValueParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(3);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.Unset());
			args.Add(new CommandParameter(parameters.ParameterName));
			return new ConfigCommand(args);
		}

		public Command GetRenameConfigSectionCommand(RenameConfigSectionParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(2);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.RenameSection(parameters.OldName, parameters.NewName));
			return new ConfigCommand(args);
		}

		public Command GetDeleteConfigSectionCommand(DeleteConfigSectionParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<ICommandArgument>(2);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.RemoveSection(parameters.SectionName));
			return new ConfigCommand(args);
		}

		#endregion
	}
}
