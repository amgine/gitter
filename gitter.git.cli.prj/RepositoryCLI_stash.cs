namespace gitter.Git.AccessLayer
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : IStashAccessor
	{
		/// <summary>Query most recent stashed state.</summary>
		/// <param name="parameters"><see cref="QueryStashTopParameters"/>.</param>
		/// <returns>Most recent stashed state or null if stash is empty.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public RevisionData QueryStashTop(QueryStashTopParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			if(parameters.LoadCommitInfo)
			{
				return GetRevisionByRefName(GitConstants.StashFullName);
			}
			else
			{
				var cmd = new ShowRefCommand(
					ShowRefCommand.Verify(),
					new CommandArgument(GitConstants.StashFullName));

				var output = _executor.ExecCommand(cmd);
				if(output.ExitCode != 0 || output.Output.Length < 40)
					return null;

				var hash = output.Output.Substring(0, 40);
				return new RevisionData(hash);
			}
		}

		/// <summary>Query all stashed states.</summary>
		/// <param name="parameters"><see cref="QueryStashParameters"/>.</param>
		/// <returns>List of all stashed states.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<StashedStateData> QueryStash(QueryStashParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new LogCommand(
				LogCommand.WalkReflogs(),
				LogCommand.NullTerminate(),
				GetRevisionFormatArgument(),
				new CommandArgument(GitConstants.StashFullName));
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var cache = new Dictionary<string, RevisionData>();
			int index = 0;
			var parser = new GitParser(output.Output);
			var res = new List<StashedStateData>();
			while(!parser.IsAtEndOfString)
			{
				var sha1 = parser.ReadString(40, 1);
				var rev = new RevisionData(sha1);
				parser.ParseRevisionData(rev, cache);
				var state = new StashedStateData(index, rev);
				res.Add(state);
				++index;
			}

			// get real commit parents

			cmd = new LogCommand(
				LogCommand.WalkReflogs(),
				LogCommand.NullTerminate(),
				LogCommand.FormatRaw(),
				new CommandArgument(GitConstants.StashFullName));
			output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			parser = new GitParser(output.Output);
			parser.ParseCommitParentsFromRaw(res.Select(ssd => ssd.Revision), cache);

			return res;
		}

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns>Patch, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public string QueryStashPatch(QueryRevisionDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			args.Add(StashCommand.Show());
			InsertDiffParameters1(parameters, args);
			args.Add(new CommandArgument(parameters.Revision));
			InsertDiffParameters2(parameters, args);

			var cmd = new StashCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			return output.Output;
		}

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Diff QueryStashDiff(QueryRevisionDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			args.Add(StashCommand.Show());
			InsertDiffParameters1(parameters, args);
			args.Add(new CommandArgument(parameters.Revision));
			InsertDiffParameters2(parameters, args);

			var cmd = new StashCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			var parser = new DiffParser(output.Output);
			return parser.ReadDiff(DiffType.CommitCompare);
		}

		/// <summary>Stash changes in working directory.</summary>
		/// <param name="parameters"><see cref="StashSaveParameters"/>.</param>
		/// <returns>true if something was stashed, false otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public bool StashSave(StashSaveParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			args.Add(StashCommand.Save());
			args.Add(parameters.KeepIndex ? StashCommand.KeepIndex() : StashCommand.NoKeepIndex());
			if(parameters.IncludeUntracked && GitFeatures.StashIncludeUntrackedOption.IsAvailableFor(_gitCLI))
			{
				args.Add(StashCommand.IncludeUntracked());
			}
			if(!string.IsNullOrWhiteSpace(parameters.Message))
			{
				args.Add(new CommandArgument(parameters.Message.SurroundWithDoubleQuotes()));
			}

			var cmd = new StashCommand(args);
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsCantStashToEmptyRepositoryError(output.Error))
				{
					throw new RepositoryIsEmptyException();
				}
				output.Throw();
			}

			return output.Output != "No local changes to save\n";
		}

		/// <summary>Apply stashed changes and remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashPopParameters"/></param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashPop(StashPopParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(3);
			args.Add(StashCommand.Pop());
			if(parameters.RestoreIndex)
			{
				args.Add(StashCommand.Index());
			}
			if(parameters.StashName != null)
			{
				args.Add(new CommandArgument(parameters.StashName));
			}

			var cmd = new StashCommand(args);
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsNothingToApplyError(output.Error))
				{
					throw new StashIsEmptyException(output.Error);
				}
				if(IsCannotApplyToDirtyWorkingTreeError(output.Error))
				{
					throw new DirtyWorkingDirectoryException();
				}
				output.Throw();
			}
		}

		/// <summary>Apply stashed changes and do not remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashApplyParameters/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashApply(StashApplyParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(3);
			args.Add(StashCommand.Apply());
			if(parameters.RestoreIndex)
			{
				args.Add(StashCommand.Index());
			}
			if(parameters.StashName != null)
			{
				args.Add(new CommandArgument(parameters.StashName));
			}

			var cmd = new StashCommand(args);
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsNothingToApplyError(output.Error))
				{
					throw new StashIsEmptyException(output.Error);
				}
				if(IsCannotApplyToDirtyWorkingTreeError(output.Error))
				{
					throw new DirtyWorkingDirectoryException();
				}
				output.Throw();
			}
		}

		/// <summary>Create new branch, checkout that branch and pop stashed state.</summary>
		/// <param name="parameters"><see cref="StashToBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashToBranch(StashToBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(3);
			args.Add(StashCommand.Branch());
			args.Add(new CommandArgument(parameters.BranchName));
			if(parameters.StashName != null)
			{
				args.Add(new CommandArgument(parameters.StashName));
			}

			var cmd = new StashCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashDropParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashDrop(StashDropParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			StashCommand cmd;
			if(parameters.StashName != null)
			{
				cmd = new StashCommand(
					StashCommand.Drop(),
					new CommandArgument(parameters.StashName));
			}
			else
			{
				cmd = new StashCommand(
					StashCommand.Drop());
			}
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Clear stash.</summary>
		/// <param name="parameters"><see cref="StashClearParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashClear(StashClearParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new StashCommand(
				StashCommand.Clear());

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}
	}
}
