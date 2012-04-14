namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI
	{
		#region log

		private static CommandArgument GetRevisionFormatArgument()
		{
			if(GitFeatures.LogFormatBTag.IsAvailable)
			{
				return LogCommand.Format("%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%B");
			}
			else
			{
				return LogCommand.Format("%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%s%n%n%b");
			}
		}

		private static CommandArgument GetReflogFormatArgument()
		{
			if(GitFeatures.LogFormatBTag.IsAvailable)
			{
				return LogCommand.Format("%gd%n%gs%n%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%B");
			}
			else
			{
				return LogCommand.Format("%gd%n%gs%n%H%n%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%s%n%n%b");
			}
		}

		private static CommandArgument GetRevisionDataFormatArgument()
		{
			if(GitFeatures.LogFormatBTag.IsAvailable)
			{
				return LogCommand.Format("%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%B");
			}
			else
			{
				return LogCommand.Format("%T%n%P%n%ct%n%cN%n%cE%n%at%n%aN%n%aE%n%s%n%n%b");
			}
		}

		private static void InsertQueryRevisionsParameters(QueryRevisionsParameters parameters, IList<CommandArgument> args)
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
			args.Add(GetRevisionFormatArgument());

			#endregion

			if(parameters.Since != null && parameters.Until != null)
			{
				args.Add(new CommandArgument(parameters.Since + ".." + parameters.Until));
			}

			if(parameters.References != null)
			{
				foreach(var reference in parameters.References)
				{
					args.Add(new CommandArgument(reference));
				}
			}

			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(CommandArgument.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}
		}


		/// <summary>Get revision list.</summary>
		/// <param name="parameters"><see cref="QueryRevisionsParameters"/>.</param>
		/// <returns>List of revisions.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<RevisionData> QueryRevisions(QueryRevisionsParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(30);
			InsertQueryRevisionsParameters(parameters, args);
			var cmd = new LogCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var cache = new Dictionary<string, RevisionData>();
			var list = new List<RevisionData>();
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfString)
			{
				var sha1 = parser.ReadString(40, 1);
				RevisionData rev;
				if(!cache.TryGetValue(sha1, out rev))
				{
					rev = new RevisionData(sha1);
					cache.Add(sha1, rev);
				}
				parser.ParseRevisionData(rev, cache);
				list.Add(rev);
			}

			return list;
		}

		public RevisionData QueryRevision(QueryRevisionParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");
			if(!Repository.ValidateSHA(parameters.SHA1))
				throw new ArgumentException("Provided expression is not a valid SHA-1.", "sha1");

			var cmd = new LogCommand(
				LogCommand.MaxCount(1),
				new CommandArgument(parameters.SHA1),
				GetRevisionDataFormatArgument());

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsUnknownRevisionError(output.Error, parameters.SHA1))
				{
					throw new UnknownRevisionException(parameters.SHA1);
				}
				output.Throw();
			}
			var parser = new GitParser(output.Output);
			var rev = new RevisionData(parameters.SHA1);
			parser.ParseRevisionData(rev, null);
			return rev;
		}

		public RevisionData GetRevisionByRefName(string refName)
		{
			if(refName == null) throw new ArgumentNullException("refName");

			var cmd = new LogCommand(
				LogCommand.MaxCount(1),
				new CommandArgument(refName),
				GetRevisionFormatArgument());

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			return parser.ParseRevision();
		}

		/// <summary>Get patch representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public string QueryRevisionPatch(QueryRevisionDiffParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>();
			args.Add(LogCommand.MaxCount(1));
			InsertDiffParameters(args, parameters);
			args.Add(new CommandArgument("-c"));
			args.Add(new CommandArgument(parameters.Revision));

			var cmd = new LogCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			return output.Output;
		}

		/// <summary>Get <see cref="Diff"/>, representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Diff QueryRevisionDiff(QueryRevisionDiffParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>();
			args.Add(LogCommand.MaxCount(1));
			InsertDiffParameters(args, parameters);
			args.Add(new CommandArgument("-c"));
			args.Add(new CommandArgument(parameters.Revision));

			var cmd = new LogCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			var parser = new DiffParser(output.Output);
			return parser.ReadDiff(DiffType.CommittedChanges);
		}

		/// <summary>Dereference valid ref.</summary>
		/// <param name="parameters"><see cref="DereferenceParameters"/>.</param>
		/// <returns>Corresponding <see cref="RevisionData"/>.</returns>
		public RevisionData Dereference(DereferenceParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var cmd = new LogCommand(
				new CommandArgument(parameters.Reference),
				LogCommand.MaxCount(1),
				parameters.LoadRevisionData?
					GetRevisionFormatArgument() : LogCommand.Format("%H"));

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsUnknownRevisionError(output.Error, parameters.Reference))
				{
					throw new UnknownRevisionException(parameters.Reference);
				}
				if(IsBadObjectError(output.Error, parameters.Reference))
				{
					throw new UnknownRevisionException(parameters.Reference);
				}
				output.Throw();
			}

			if(parameters.LoadRevisionData)
			{
				var parser = new GitParser(output.Output);
				return parser.ParseRevision();
			}
			else
			{
				var hash = output.Output;
				return new RevisionData(hash);
			}
		}

		#endregion

		#region revert

		/// <summary>Performs a revert operation.</summary>
		/// <param name="parameters"><see cref="RevertParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, revert is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, revert cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, unable to revert.</exception>
		public void Revert(RevertParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>();
			if(parameters.NoCommit)
			{
				args.Add(RevertCommand.NoCommit());
			}
			foreach(var rev in parameters.Revisions)
			{
				args.Add(new CommandArgument(rev));
			}
			var cmd = new RevertCommand(args);

			var output = _executor.ExecCommand(cmd);
			if(output.Error != "Finished one revert.\n") // TODO: needs a better parser.
			{
				output.ThrowOnBadReturnCode();
			}
		}

		#endregion

		#region cherry-pick

		/// <summary>Performs a cherry-pick operation.</summary>
		/// <param name="parameters"><see cref="CherryPickParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, cherry-pick is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, cherry-pick cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, cannot cherry-pick.</exception>
		/// <exception cref="T:gitter.Git.CherryPickIsEmptyException">Resulting cherry-pick is empty.</exception>
		/// <exception cref="T:gitter.Git.AutomaticCherryPickFailedException">Cherry-pick was not finished because of conflicts.</exception>
		public void CherryPick(CherryPickParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>();
			if(parameters.NoCommit)
			{
				args.Add(CherryPickCommand.NoCommit());
			}
			foreach(var rev in parameters.Revisions)
			{
				args.Add(new CommandArgument(rev));
			}
			var cmd = new CherryPickCommand(args);

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				string fileName;
				if(IsAutomaticCherryPickFailedError(output.Error))
				{
					throw new AutomaticCherryPickFailedException();
				}
				if(IsCherryPickIsEmptyError(output.Error))
				{
					throw new CherryPickIsEmptyException(output.Error);
				}
				if(IsHaveLocalChangesMergeError(output.Error, out fileName))
				{
					throw new HaveLocalChangesException(fileName);
				}
				if(IsCherryPickNotPossibleBecauseOfMergeCommit(output.Error))
				{
					throw new CommitIsMergeException();
				}
				if(IsCherryPickNotPossibleBecauseOfConflictsError(output.Error))
				{
					throw new HaveConflictsException();
				}
				output.Throw();
			}
		}

		#endregion
	}
}
