namespace gitter.Git.AccessLayer
{
	using System;
	using System.Linq;
	using System.IO;
	using System.Collections.Generic;
	using System.Text;
	using System.Globalization;

	using gitter.Framework.Services;

	using gitter.Git.AccessLayer.CLI;

	/// <summary>Accesses repository through git command line interface.</summary>
	internal sealed partial class RepositoryCLI : IRepositoryAccessor
	{
		#region Data

		private readonly GitCLI _gitCLI;
		private readonly IGitRepository _repository;
		private readonly ICommandExecutor _executor;

		#endregion

		public RepositoryCLI(GitCLI gitCLI, IGitRepository repository)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");
			Verify.Argument.IsNotNull(repository, "repository");

			_gitCLI = gitCLI;
			_repository = repository;
			_executor = new RepositoryCommandExecutor(
				gitCLI, repository.WorkingDirectory);
		}

		/// <summary>Returns git accessor.</summary>
		/// <value>git accessor.</value>
		public IGitAccessor GitAccessor
		{
			get { return _gitCLI; }
		}

		const string refPrefix = "ref: ";

		/// <summary>Get symbolic reference target.</summary>
		/// <param name="parameters"><see cref="QuerySymbolicReferenceParameters"/>.</param>
		/// <returns>Symbolic reference data.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public SymbolicReferenceData QuerySymbolicReference(QuerySymbolicReferenceParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var fileName = _repository.GetGitFileName(parameters.Name);
			if(File.Exists(fileName))
			{
				string pointer;
				using(var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if(fs.Length == 0)
					{
						return new SymbolicReferenceData(null, ReferenceType.None);
					}
					else
					{
						using(var sr = new StreamReader(fs))
						{
							pointer = sr.ReadLine();
							sr.Close();
						}
					}
					fs.Close();
				}
				if(pointer.Length >= 17 && pointer.StartsWith(refPrefix + GitConstants.LocalBranchPrefix))
				{
					return new SymbolicReferenceData(pointer.Substring(16), ReferenceType.LocalBranch);
				}
				else
				{
					if(GitUtils.IsValidSHA1(pointer))
					{
						return new SymbolicReferenceData(pointer, ReferenceType.Revision);
					}
				}
			}
			return new SymbolicReferenceData(null, ReferenceType.None);
		}

		/// <summary>Get HEAD of repository.</summary>
		/// <param name="refType">Type of the HEAD.</param>
		/// <returns>HEAD reference.</returns>
		public string GetHEAD(out ReferenceType refType)
		{
			var headFile = _repository.GetGitFileName(GitConstants.HEAD);
			if(File.Exists(headFile))
			{
				string head;
				using(var fs = new FileStream(headFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if(fs.Length == 0)
					{
						refType = ReferenceType.None;
						return null;
					}
					else
					{
						using(var sr = new StreamReader(fs))
						{
							head = sr.ReadLine();
							sr.Close();
						}
					}
					fs.Close();
				}
				if(head.StartsWith(refPrefix + GitConstants.LocalBranchPrefix) && head.Length >= 17)
				{
					refType = ReferenceType.LocalBranch;
					return head.Substring(16);
				}
				else
				{
					if(GitUtils.IsValidSHA1(head))
					{
						refType = ReferenceType.Revision;
						return head;
					}
				}
			}
			refType = ReferenceType.None;
			return null;
		}

		#region show

		/// <summary>Get contents of requested objects.</summary>
		/// <param name="parameters"><see cref="parameters"/>.</param>
		/// <returns>Objects contents.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public string QueryObjects(QueryObjectsParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(parameters.Objects.Count);
			foreach(var obj in parameters.Objects)
			{
				args.Add(new CommandArgument(obj));
			}
			var cmd = new ShowCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			return output.Output;
		}

		#endregion

		#region show-ref

		/// <summary>Get list of references.</summary>
		/// <param name="parameters"><see cref="QueryReferencesParameters"/>.</param>
		/// <returns>Lists of references.</returns>
		public ReferencesData QueryReferences(QueryReferencesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var refTypes = parameters.ReferenceTypes;

			bool needHeads		= (refTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch;
			bool needRemotes	= (refTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch;
			bool needTags		= (refTypes & ReferenceType.Tag) == ReferenceType.Tag;
			bool needStash		= (refTypes & ReferenceType.Stash) == ReferenceType.Stash;

			var heads	= needHeads		? new List<BranchData>()	: null;
			var remotes	= needRemotes	? new List<BranchData>()	: null;
			var tags	= needTags		? new List<TagData>()		: null;
			RevisionData stash = null;

			ShowRefCommand cmd;
			switch(refTypes)
			{
				case ReferenceType.LocalBranch | ReferenceType.Tag:
					cmd = new ShowRefCommand(
						ShowRefCommand.Heads(),
						ShowRefCommand.Tags(),
						ShowRefCommand.Dereference());
					break;
				case ReferenceType.LocalBranch:
					cmd = new ShowRefCommand(
						ShowRefCommand.Heads(),
						ShowRefCommand.Dereference());
					break;
				case ReferenceType.Tag:
					cmd = new ShowRefCommand(
						ShowRefCommand.Tags(),
						ShowRefCommand.Dereference());
					break;
				default:
					cmd = new ShowRefCommand(
						ShowRefCommand.Dereference());
					break;
			}

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(output.Error.Length == 0 && output.Output.Length == 0)
				{
					return new ReferencesData(null, null, null, null);
				}
				output.Throw();
			}

			bool encounteredRemoteBranch = false;
			bool encounteredStash = false;
			bool encounteredTag = false;

			var refs = output.Output;
			int pos = 0;
			int l = refs.Length;
			while(pos < l)
			{
				var hash = refs.Substring(pos, 40);
				pos += 41;
				var end = refs.IndexOf('\n', pos);
				if(end == -1) end = l;

				if(!encounteredRemoteBranch && CheckValue(refs, pos, GitConstants.LocalBranchPrefix))
				{
					if(needHeads)
					{
						pos += GitConstants.LocalBranchPrefix.Length;
						var name = refs.Substring(pos, end - pos);
						var branch = new BranchData(name, hash, false, false, false);
						heads.Add(branch);
					}
				}
				else if(!encounteredStash && CheckValue(refs, pos, GitConstants.RemoteBranchPrefix))
				{
					encounteredRemoteBranch = true;
					if(needRemotes)
					{
						pos += GitConstants.RemoteBranchPrefix.Length;
						var name = refs.Substring(pos, end - pos);
						if(!name.EndsWith("/HEAD"))
						{
							var branch = new BranchData(name, hash, false, true, false);
							remotes.Add(branch);
						}
					}
				}
				else if(!encounteredTag && !encounteredStash && CheckValue(refs, pos, GitConstants.StashFullName))
				{
					encounteredRemoteBranch = true;
					encounteredStash = true;
					if(needStash)
					{
						stash = new RevisionData(hash);
					}
				}
				else if(CheckValue(refs, pos, GitConstants.TagPrefix))
				{
					encounteredRemoteBranch = true;
					encounteredStash = true;
					encounteredTag = true;
					if(needTags)
					{
						pos += GitConstants.TagPrefix.Length;
						var name = refs.Substring(pos, end - pos);
						var type = TagType.Lightweight;
						if(end < l - 1)
						{
							int s2 = end + 1;
							int pos2 = s2 + 41 + GitConstants.TagPrefix.Length;
							var end2 = refs.IndexOf('\n', pos2);
							if(end2 == -1) end2 = l;
							if(end2 - pos2 == end - pos + 3)
							{
								if(CheckValue(refs, pos2, name) && CheckValue(refs, pos2 + name.Length, GitConstants.DereferencedTagPostfix))
								{
									type = TagType.Annotated;
									hash = refs.Substring(s2, 40);
									end = end2;
								}
							}
						}
						var tag = new TagData(name, hash, type);
						tags.Add(tag);
					}
					else break;
				}
				pos = end + 1;
			}
			return new ReferencesData(heads, remotes, tags, stash);
		}

		#endregion

		#region reflog

		/// <summary>Get reflog.</summary>
		/// <param name="parameters"><see cref="QueryReflogParameters"/>.</param>
		/// <returns>List of reflog records.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<ReflogRecordData> QueryReflog(QueryReflogParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(5);
			args.Add(LogCommand.WalkReflogs());
			if(parameters.MaxCount != 0)
			{
				args.Add(LogCommand.MaxCount(parameters.MaxCount));
			}
			args.Add(LogCommand.NullTerminate());
			args.Add(GetReflogFormatArgument());
			if(parameters.Reference != null)
			{
				args.Add(new CommandArgument(parameters.Reference));
			}

			var cmd = new LogCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var cache = new Dictionary<string, RevisionData>();
			var list = new List<ReflogRecordData>();
			if(output.Output.Length < 40) return new ReflogRecordData[0];
			var parser = new GitParser(output.Output);
			int index = 0;
			while(!parser.IsAtEndOfString)
			{
				var selector = parser.ReadLine();
				if(selector.Length == 0) break;
				var message = parser.ReadLine();
				var sha1 = parser.ReadString(40, 1);
				RevisionData rev;
				if(!cache.TryGetValue(sha1, out rev))
				{
					rev = new RevisionData(sha1);
					cache.Add(sha1, rev);
				}
				parser.ParseRevisionData(rev, cache);
				list.Add(new ReflogRecordData(index++, message, rev));
			}

			// get real commit parents
			args.Clear();
			args.Add(LogCommand.WalkReflogs());
			if(parameters.MaxCount != 0)
			{
				args.Add(LogCommand.MaxCount(parameters.MaxCount));
			}
			args.Add(LogCommand.NullTerminate());
			args.Add(LogCommand.FormatRaw());
			if(parameters.Reference != null)
			{
				args.Add(new CommandArgument(parameters.Reference));
			}
			cmd = new LogCommand(args);
			output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			parser = new GitParser(output.Output);
			parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);

			return list;
		}

		#endregion

		#region diff

		/// <summary>Get <see cref="Diff"/>, representing difference between specified objects.</summary>
		/// <param name="parameters"><see cref="QueryDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing difference between requested objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Diff QueryDiff(QueryDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			InsertDiffParameters1(parameters, args);

			bool comparison = false;
			if(!string.IsNullOrEmpty(parameters.Revision1))
			{
				args.Add(new CommandArgument(parameters.Revision1));
			}
			if(!string.IsNullOrEmpty(parameters.Revision2))
			{
				args.Add(new CommandArgument(parameters.Revision2));
				comparison = true;
			}

			DiffType type = DiffType.Patch;
			if(comparison)
			{
				type = DiffType.CommitCompare;
			}
			else
			{
				if(parameters.Cached)
				{
					type = DiffType.StagedChanges;
				}
				else
				{
					type = DiffType.UnstagedChanges;
				}
			}
			InsertDiffParameters2(parameters, args);

			var cmd = new DiffCommand(args);
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(parameters.Cached && IsNoHEADCommitToCompareWithError(output.Error))
				{
					throw new RepositoryIsEmptyException(output.Error);
				}
				output.Throw();
			}
			var parser = new DiffParser(output.Output);
			return parser.ReadDiff(type);
		}

		private static void InsertDiffParameters1(BaseQueryDiffParameters parameters, IList<CommandArgument> args)
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
		}

		private static void InsertDiffParameters2(BaseQueryDiffParameters parameters, IList<CommandArgument> args)
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

		public BlameFile QueryBlame(QueryBlameParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			InsertQueryBlameParameters(parameters, args);
			var cmd = new BlameCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			var parser = new BlameParser(output.Output);
			return parser.ParseBlameFile(parameters.FileName);
		}

		private static void InsertQueryBlameParameters(QueryBlameParameters parameters, IList<CommandArgument> args)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(args);

			args.Add(BlameCommand.Porcelain());
			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandArgument(parameters.Revision));
			}
			args.Add(CommandArgument.NoMoreOptions());
			args.Add(new PathCommandArgument(parameters.FileName));
		}

		#endregion

		#region checkout

		/// <summary>Checkout branch/revision.</summary>
		/// <param name="parameters"><see cref="CheckoutParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Checkout(CheckoutParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			InsertCheckoutParameters(parameters, args);
			var cmd = new CheckoutCommand(args);
			var output = _executor.ExecCommand(cmd);
			HandleCheckoutResults(parameters, output);
		}

		private static void InsertCheckoutParameters(CheckoutParameters parameters, IList<CommandArgument> args)
		{
			if(parameters.Force)
			{
				args.Add(CheckoutCommand.Force());
			}
			if(parameters.Merge)
			{
				args.Add(CheckoutCommand.Merge());
			}
			args.Add(new CommandArgument(parameters.Revision));
		}

		private static void HandleCheckoutResults(CheckoutParameters parameters, GitOutput output)
		{
			if(output.ExitCode != 0)
			{
				if(IsUnknownRevisionError(output.Error, parameters.Revision))
				{
					throw new UnknownRevisionException(parameters.Revision);
				}
				if(IsRefrerenceIsNotATreeError(output.Error, parameters.Revision))
				{
					throw new UnknownRevisionException(parameters.Revision);
				}
				if(IsUnknownPathspecError(output.Error, parameters.Revision))
				{
					throw new UnknownRevisionException(parameters.Revision);
				}
				if(!parameters.Force)
				{
					string fileName;
					if(IsUntrackedFileWouldBeOverwrittenError(output.Error, out fileName))
					{
						throw new UntrackedFileWouldBeOverwrittenException(fileName);
					}
					if(IsHaveLocalChangesError(output.Error, out fileName))
					{
						throw new HaveLocalChangesException(fileName);
					}
					if(IsHaveConflictsError(output.Error))
					{
						throw new HaveConflictsException();
					}
				}
				output.Throw();
			}
		}

		#endregion

		#region reset

		/// <summary>Reset HEAD.</summary>
		/// <param name="parameters"><see cref="ResetParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Reset(ResetParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			args.Add(ResetCommand.Mode(parameters.Mode));
			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandArgument(parameters.Revision));
			}
			var cmd = new ResetCommand(args);

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		#endregion

		#region merge

		/// <summary>Merge development histories together.</summary>
		/// <param name="parameters"><see cref="MergeParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.AutomaticMergeFailedException">Merge resulted in conflicts.</exception>
		public void Merge(MergeParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			InsertMergeParameters(parameters, args);
			var cmd = new MergeCommand(args);
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsAutomaticMergeFailedError(output.Output))
				{
					throw new AutomaticMergeFailedException();
				}
				output.Throw();
			}
		}

		private static void InsertMergeParameters(MergeParameters parameters, IList<CommandArgument> args)
		{
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
				args.Add(MergeCommand.Message(parameters.Message));
			}
			foreach(var rev in parameters.Revisions)
			{
				args.Add(new CommandArgument(rev));
			}
		}

		private string GetMergedCommits(string branch, string head, string lineFormat)
		{
			var cmd = new LogCommand(
				LogCommand.TFormat(lineFormat),
				new CommandArgument(head + ".." + branch));
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			return output.Output;
		}

		private const string changeLogFormat = "  * %s\r";

		public string FormatMergeMessage(FormatMergeMessageParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			if(parameters.Revisions.Count == 1)
			{
				var rev = parameters.Revisions[0];
				var commits = GetMergedCommits(rev, parameters.HeadReference, changeLogFormat);
				var msg = string.Format("Merge branch '{0}' into {1}\r\n\r\n", rev, parameters.HeadReference) + "Changes:\r\n" + commits;
				return msg;
			}
			else
			{
				var sb = new StringBuilder();
				sb.Append("Merge branches ");
				for(int i = 0; i < parameters.Revisions.Count; ++i)
				{
					sb.Append('\'');
					sb.Append(parameters.Revisions[i]);
					sb.Append('\'');
					if(i != parameters.Revisions.Count - 1)
					{
						sb.Append(", ");
					}
				}
				sb.Append(" into ");
				sb.Append(parameters.HeadReference);
				sb.Append("\r\n");
				for(int i = 0; i < parameters.Revisions.Count; ++i)
				{
					sb.Append("\r\nChanges from ");
					sb.Append(parameters.Revisions[i]);
					sb.Append(":\r\n");
					sb.Append(GetMergedCommits(parameters.Revisions[i], parameters.HeadReference, changeLogFormat));
				}
				return sb.ToString();
			}
		}

		#endregion

		#region rebase

		/// <summary>Forward-port local commits to the updated upstream head.</summary>
		/// <param name="parameters"><see cref="RebaseParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Rebase(RebaseParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			if(!string.IsNullOrEmpty(parameters.NewBase))
			{
				args.Add(RebaseCommand.Onto(parameters.NewBase));
			}
			args.Add(new CommandArgument(parameters.Upstream));
			if(!string.IsNullOrEmpty(parameters.Branch))
			{
				args.Add(new CommandArgument(parameters.Branch));
			}

			var cmd = new RebaseCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Rebase control option.</param>
		public void Rebase(RebaseControl control)
		{
			CommandArgument arg;
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
			var cmd = new RebaseCommand(arg);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		#endregion

		#region committers

		/// <summary>Get user list.</summary>
		/// <param name="parameters"><see cref="QueryUsersParameters"/>.</param>
		/// <returns>List of committers and authors.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<UserData> QueryUsers(QueryUsersParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new ShortLogCommand(
				ShortLogCommand.All(),
				ShortLogCommand.Numbered(),
				ShortLogCommand.Summary(),
				ShortLogCommand.Email());
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var res = new List<UserData>();
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfString)
			{
				var tab = parser.FindNoAdvance('\t');
				string commitsCountStr = parser.ReadStringUpTo(tab, 1);
				int commitsCount = int.Parse(commitsCountStr, NumberStyles.Integer, CultureInfo.InvariantCulture);
				var eol = parser.FindLfLineEnding();
				var emailSeparator = parser.String.LastIndexOf(" <", eol - 1, eol - tab - 1);
				string name = parser.ReadStringUpTo(emailSeparator, 2);
				string email = parser.ReadStringUpTo(eol - 1, 2);
				var userData = new UserData(name, email, commitsCount);
				res.Add(userData);
			}
			return res;
		}

		#endregion

		#region gc

		/// <summary>Cleanup unnecessary files and optimize the local repository.</summary>
		/// <param name="parameters"><see cref="GarbageCollectParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void GarbageCollect(GarbageCollectParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new GcCommand();

			if(parameters.Monitor == null)
			{
				var output = _executor.ExecCommand(cmd);
				output.ThrowOnBadReturnCode();
			}
			else
			{
				var mon = parameters.Monitor;
				using(var async = _executor.ExecAsync(cmd))
				{
					async.ErrorReceived += (sender, e) =>
					{
						if(e.Data != null && e.Data.Length != 0)
						{
							var parser = new GitParser(e.Data);
							var progress = parser.ParseProgress();
							progress.Notify(mon);
						}
						else
						{
							mon.SetProgressIndeterminate();
						}
					};
					async.Start();
					async.WaitForExit();
					if(async.ExitCode != 0)
					{
						throw new GitException(async.StdErr);
					}
				}
			}
		}

		#endregion
	}
}
