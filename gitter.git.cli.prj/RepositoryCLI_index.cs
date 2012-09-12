namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : IIndexAccessor
	{
		private static FileStatus CharToFileStatus(char c)
		{
			switch(c)
			{
				case 'M':
					return FileStatus.Modified;
				case 'A':
					return FileStatus.Added;
				case 'D':
					return FileStatus.Removed;
				case '?':
					return FileStatus.Added;
				case 'U':
					return FileStatus.Unmerged;
				case ' ':
					return FileStatus.Unknown;
				case '!':
					return FileStatus.Ignored;
				default:
					return FileStatus.Unknown;
			}
		}

		private static ConflictType GetConflictType(char x, char y)
		{
			if(IsUnmergedState(x, y))
			{
				if(x == y)
				{
					if(x == 'A') return ConflictType.BothAdded;
					if(x == 'D') return ConflictType.BothDeleted;
					if(x == 'U') return ConflictType.BothModified;
					return ConflictType.Unknown;
				}
				else
				{
					switch(x)
					{
						case 'U':
							switch(y)
							{
								case 'A': return ConflictType.AddedByThem;
								case 'D': return ConflictType.DeletedByThem;
								default: return ConflictType.Unknown;
							}
						case 'A':
							switch(y)
							{
								case 'U': return ConflictType.AddedByUs;
								default: return ConflictType.Unknown;
							}
						case 'D':
							switch(y)
							{
								case 'U': return ConflictType.DeletedByUs;
								default: return ConflictType.Unknown;
							}
						default:
							return ConflictType.Unknown;
					}
				}
			}
			else
			{
				return ConflictType.None;
			}
		}

		private static bool IsUnmergedState(char x, char y)
		{
			return (x == 'U' || y == 'U') || (x == 'A' && y == 'A') || (x == 'D' && y == 'D');
		}

		private static void InsertAddFilesParameters(AddFilesParameters parameters, IList<CommandArgument> args)
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

		private static void InsertRemoveFilesParameters(RemoveFilesParameters parameters, IList<CommandArgument> args)
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

		private void InsertCleanFilesParameters(CleanFilesParameters parameters, IList<CommandArgument> args)
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
				args.Add(CleanCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
		}

		private static readonly Version SubmodulesStatusArgVersion = new Version(1, 7, 2, 3);

		private static string ReadStatusFileName(GitParser parser)
		{
			var eol = parser.FindNullOrEndOfString();
			return parser.String[eol - 1] == '/' ?
				parser.ReadStringUpTo(eol - 1, 2) :
				parser.ReadStringUpTo(eol, 1);
		}

		/// <summary>Get working directory status information.</summary>
		/// <param name="parameters"><see cref="QueryStatusParameters"/>.</param>
		/// <returns>Working directory status information.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public StatusData QueryStatus(QueryStatusParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(7);

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
			var cmd = new StatusCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var stagedFiles = new Dictionary<string, TreeFileData>();
			var unstagedFiles = new Dictionary<string, TreeFileData>();

			int unstagedUntrackedCount = 0;
			int unstagedRemovedCount = 0;
			int unstagedModifiedCount = 0;
			int unmergedCount = 0;
			int stagedAddedCount = 0;
			int stagedModifiedCount = 0;
			int stagedRemovedCount = 0;

			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfString)
			{
				var x = parser.ReadChar();
				var y = parser.ReadChar();
				parser.Skip();

				bool staged = false;
				bool unstaged = false;
				var conflictType = ConflictType.None;
				var stagedFileStatus = FileStatus.Unknown;
				var unstagedFileStatus = FileStatus.Unknown;

				var to = ReadStatusFileName(parser);
				string from = string.Empty;

				if(x == '?')
				{
					staged = false;
					unstaged = true;
					unstagedFileStatus = FileStatus.Added;
					++unstagedUntrackedCount;
				}
				else
				{
					if(x == 'C' || x == 'R')
					{
						from = ReadStatusFileName(parser);
						if(x == 'C')
						{
							x = 'A';
							stagedFileStatus = FileStatus.Added;
						}
						else
						{
							if(!stagedFiles.ContainsKey(from))
							{
								var file = new TreeFileData(from, FileStatus.Removed, ConflictType.None, StagedStatus.Staged);
								stagedFiles.Add(from, file);
								++stagedRemovedCount;
							}
							x = 'A';
							stagedFileStatus = FileStatus.Added;
						}
					}
					conflictType = GetConflictType(x, y);
					if(conflictType != ConflictType.None)
					{
						staged = false;
						unstaged = true;
						unstagedFileStatus = FileStatus.Unmerged;
						++unmergedCount;
					}
					else
					{
						if(x != ' ')
						{
							staged = true;
							stagedFileStatus = CharToFileStatus(x);
							switch(stagedFileStatus)
							{
								case FileStatus.Added:
									++stagedAddedCount;
									break;
								case FileStatus.Removed:
									++stagedRemovedCount;
									break;
								case FileStatus.Modified:
									++stagedModifiedCount;
									break;
							}
						}
						if(y != ' ')
						{
							unstaged = true;
							unstagedFileStatus = CharToFileStatus(y);
							switch(unstagedFileStatus)
							{
								case FileStatus.Added:
									++unstagedUntrackedCount;
									break;
								case FileStatus.Removed:
									++unstagedRemovedCount;
									break;
								case FileStatus.Modified:
									++unstagedModifiedCount;
									break;
							}
						}
					}
				}

				if(staged)
				{
					var file = new TreeFileData(to, stagedFileStatus, ConflictType.None, StagedStatus.Staged);
					if(!stagedFiles.ContainsKey(to))
					{
						stagedFiles.Add(to, file);
					}
					else
					{
						stagedFiles[to] = file;
					}
				}
				if(unstaged)
				{
					var file = new TreeFileData(to, unstagedFileStatus, conflictType, StagedStatus.Unstaged);
					TreeFileData existing;
					if(unstagedFiles.TryGetValue(to, out existing))
					{
						if(existing.FileStatus == FileStatus.Removed)
						{
							unstagedFiles[to] = file;
						}
					}
					else
					{
						unstagedFiles.Add(to, file);
					}
				}	
			}
			return new StatusData(
				stagedFiles, unstagedFiles,
				unstagedUntrackedCount,
				unstagedRemovedCount,
				unstagedModifiedCount,
				unmergedCount,
				stagedAddedCount,
				stagedModifiedCount,
				stagedRemovedCount);
		}

		/// <summary>Get the list of files that can be added.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <returns>List of files which will be added by call to <see cref="AddFiles"/>(<paramref name="parameters"/>).</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<TreeFileData> QueryFilesToAdd(AddFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 5);
			args.Add(AddCommand.DryRun());
			InsertAddFilesParameters(parameters, args);

			var cmd = new AddCommand(args);
			var output = _executor.ExecCommand(cmd, Encoding.Default);
			if(output.ExitCode != 0 && output.ExitCode != 128)
			{
				return new List<TreeFileData>(0);
			}

			var files = output.Output;
			var l = files.Length;
			var pos = 0;
			var res = new List<TreeFileData>();
			while(pos < l)
			{
				int eol = files.IndexOf('\n', pos);
				if(eol == -1) eol = l;
				var status = FileStatus.Cached;
				string filePath = null;
				switch(files[pos])
				{
					case 'a':
						status = FileStatus.Added;
						filePath = files.Substring(pos + 5, eol - pos - 6);
						break;
					case 'r':
						status = FileStatus.Removed;
						filePath = files.Substring(pos + 8, eol - pos - 9);
						break;
					case 'T':
						eol = l;
						break;
				}
				if(filePath != null)
				{
					var slashPos = filePath.LastIndexOf('/');
					var fileName = slashPos != -1 ?
						filePath.Substring(slashPos + 1) :
						filePath;
					var file = new TreeFileData(
						filePath, status, ConflictType.None, StagedStatus.Unstaged);
					res.Add(file);
					pos = eol + 1;
				}
				else
				{
					pos = eol + 1;
				}
			}
			return res;
		}

		/// <summary>Add file to index.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AddFiles(AddFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(
				(parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 5);
			InsertAddFilesParameters(parameters, args);

			var cmd = new AddCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>
		/// Get list of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <returns>List of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.</returns>
		public IList<string> QueryFilesToRemove(RemoveFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 5);

			args.Add(RmCommand.DryRun());
			InsertRemoveFilesParameters(parameters, args);

			var cmd = new RmCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var files = output.Output;
			var l = files.Length;
			var pos = 0;
			var res = new List<string>();
			while(pos < l)
			{
				var eol = files.IndexOf('\n', pos);
				if(eol == -1) eol = files.Length;
				if(CheckValue(files, pos, "rm '"))
				{
					res.Add(files.Substring(pos + 4, eol - pos - 5));
				}
				pos = eol + 1;
			}
			return res;
		}

		/// <summary>Remove file from index and/or working directory.</summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RemoveFiles(RemoveFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 4);

			InsertRemoveFilesParameters(parameters, args);

			var cmd = new RmCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>
		/// Get list of files and directories which will be removed
		/// by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <returns>List of files and directories which will be removed by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<string> QueryFilesToClean(CleanFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(
				(parameters.ExcludePatterns != null ? parameters.ExcludePatterns.Count : 0) +
				(parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 4);

			args.Add(CleanCommand.DryRun());
			InsertCleanFilesParameters(parameters, args);

			var cmd = new CleanCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var files = output.Output;
			var res = new List<string>();
			var parser = new GitParser(files);
			while(!parser.IsAtEndOfString)
			{
				if(parser.CheckValue("Would remove "))
				{
					parser.Skip(13);
					res.Add(parser.DecodeEscapedString(parser.FindNewLineOrEndOfString(), 1));
				}
				else
				{
					parser.SkipLine();
				}
			}
			return res;
		}

		/// <summary>Remove untracked files from the working tree.</summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CleanFiles(CleanFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(
				(parameters.ExcludePatterns != null ? parameters.ExcludePatterns.Count : 0) +
				(parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 3);

			InsertCleanFilesParameters(parameters, args);

			var cmd = new CleanCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Resets files.</summary>
		/// <param name="parameters"><see cref="ResetFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void ResetFiles(ResetFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 1);

			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandArgument(parameters.Revision));
			}
			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(ResetCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}

			var cmd = new ResetCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnError();
		}

		/// <summary>Apply patches to working directory and/or index.</summary>
		/// <param name="parameters"><see cref="ApplyPatchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void ApplyPatch(ApplyPatchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			InsertApplyPatchParameters(parameters, args);
			var cmd = new ApplyCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		private static void InsertApplyPatchParameters(ApplyPatchParameters parameters, IList<CommandArgument> args)
		{
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
		}

		/// <summary>Commit changes.</summary>
		/// <param name="parameters"><see cref="CommitParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Commit(CommitParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(12);
			InsertCommitParameters(parameters, args);
			var cmd = new CommitCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		private static void InsertCommitParameters(CommitParameters parameters, IList<CommandArgument> args)
		{
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
		}

		/// <summary>Run merge tool to resolve conflicts.</summary>
		/// <param name="parameters"><see cref="RunMergeToolParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RunMergeTool(RunMergeToolParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>((parameters.Files != null ? parameters.Files.Count : 0) + 2);
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

			var cmd = new MergeToolCommand(args);

			if(parameters.Monitor == null)
			{
				var output = _executor.ExecCommand(cmd);
				output.ThrowOnBadReturnCode();
			}
			else
			{
				var mon = parameters.Monitor;
				var output = _executor.ExecAsync(cmd);
				mon.Cancelled += (sender, e) => output.Kill();
				output.Start();
				output.WaitForExit();
				output.ThrowOnBadReturnCode();
			}
		}
	}
}
