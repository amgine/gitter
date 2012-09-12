namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : IRemoteAccessor
	{
		private static RemoteData ParseRemote(string strRemote, ref int pos)
		{
			var posName = strRemote.IndexOf('\t', pos);
			var pos2 = strRemote.IndexOf('\n', posName + 1);

			var strName = strRemote.Substring(pos, posName - pos);

			++posName;
			while(strRemote[posName] == '\t') ++posName;
			var posUrl = strRemote.IndexOf(' ', posName);

			string fetchUrl = strRemote.Substring(posName, posUrl - posName);
			string pushUrl = fetchUrl;

			pos = pos2 + 1;

			if(pos < strRemote.Length)
			{
				posName = strRemote.IndexOf('\t', pos);
				var name2 = strRemote.Substring(pos, posName - pos);
				if(name2 == strName)
				{
					++posName;
					while(strRemote[posName] == '\t') ++posName;

					posUrl = strRemote.IndexOf(' ', posName);
					pushUrl = strRemote.Substring(posName, posUrl - posName);
				}

				pos = strRemote.IndexOf('\n', posName + 1) + 1;
				if(pos < 0) pos = strRemote.Length;
			}

			return new RemoteData(strName, fetchUrl, pushUrl);
		}

		private static ReferencePushResult ParsePushResult(string output, ref int pos)
		{
			PushResultType type;
			switch(output[pos])
			{
				case ' ':
					type = PushResultType.FastForwarded;
					break;
				case '+':
					type = PushResultType.ForceUpdated;
					break;
				case '-':
					type = PushResultType.DeletedReference;
					break;
				case '*':
					type = PushResultType.CreatedReference;
					break;
				case '!':
					type = PushResultType.Rejected;
					break;
				case '=':
					type = PushResultType.UpToDate;
					break;
				default:
					pos = output.IndexOf('\n') + 1;
					if(pos == 0) pos = output.Length + 1;
					return null;
			}
			pos += 2;
			int end = output.IndexOf('\n', pos);
			if(end == -1) end = output.Length;
			var t = output.IndexOf('\t', pos, end - pos);
			var c = output.IndexOf(':', pos, t - pos);
			var from = output.Substring(pos, c - pos);
			var to = output.Substring(c + 1, t - c - 1);
			var summary = output.Substring(t + 1, end - t - 1);
			pos = end + 1;
			return new ReferencePushResult(type, from, to, summary);
		}

		private static RemoteReferenceData ParseRemoteReference(string output, ref int pos)
		{
			var hash = output.Substring(pos, 40);
			pos += 41;
			while(output[pos] == ' ' || output[pos] == '\t') ++pos;
			int end = output.IndexOf('\n', pos);
			if(end == -1) end = output.Length;
			var name = output.Substring(pos, end - pos);
			pos = end + 1;

			if(name.StartsWith(GitConstants.TagPrefix) && pos < output.Length)
			{
				end = output.IndexOf('\n', pos);
				if(end == -1) end = output.Length;
				var hash2 = output.Substring(pos, 40);
				int pos2 = pos + 41;
				while(output[pos2] == ' ' || output[pos2] == '\t') ++pos2;
				int l = end - pos2;
				if(l == name.Length + GitConstants.DereferencedTagPostfix.Length)
				{
					if(CheckValues(output, pos2, name, GitConstants.DereferencedTagPostfix))
					{
						pos = end + 1;
						return new RemoteReferenceData(name, hash2) { TagType = TagType.Annotated };
					}
				}
			}

			return new RemoteReferenceData(name, hash);
		}

		private void InsertPullParameters(PullParameters parameters, IList<CommandArgument> args)
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

		private void InsertFetchParameters(FetchParameters parameters, IList<CommandArgument> args)
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
			if(parameters.Monitor != null && GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				args.Add(FetchCommand.Progress());
			}
			if(!string.IsNullOrEmpty(parameters.UploadPack))
			{
				args.Add(FetchCommand.UploadPack(parameters.UploadPack));
			}
			if(!string.IsNullOrEmpty(parameters.Repository))
			{
				args.Add(new CommandArgument(parameters.Repository));
			}
		}

		private void InsertPushparameters(PushParameters parameters, IList<CommandArgument> args)
		{
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
			if(parameters.Monitor != null && GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				args.Add(PushCommand.Progress());
			}
			if(!string.IsNullOrEmpty(parameters.Repository))
			{
				args.Add(new CommandArgument(parameters.Repository));
			}
			if(parameters.Refspecs != null && parameters.Refspecs.Count != 0)
			{
				foreach(var refspec in parameters.Refspecs)
				{
					args.Add(new CommandArgument(refspec));
				}
			}
		}

		/// <summary>Get information about remote.</summary>
		/// <param name="parameters"><see cref="QueryRemoteParameters"/>.</param>
		/// <returns>Requested remote.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public RemoteData QueryRemote(QueryRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new RemoteCommand(
				RemoteCommand.Show(),
				RemoteCommand.Cached(),
				new CommandArgument(parameters.RemoteName));
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var info = output.Output;
			int pos = info.IndexOf('\n') + 1;
			int pos2 = info.IndexOf('\n', pos);
			string fetchUrl = info.Substring(pos + 13, pos2 - pos - 13);
			pos = pos2 + 1;
			pos = info.IndexOf('\n', pos);
			string pushUrl = info.Substring(pos + 13, pos2 - pos - 13);

			return new RemoteData(parameters.RemoteName, fetchUrl, pushUrl);
		}

		/// <summary>Query list of remotes.</summary>
		/// <param name="parameters"><see cref="QueryRemotesParameters"/>.</param>
		/// <returns>List of remotes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<RemoteData> QueryRemotes(QueryRemotesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new RemoteCommand(RemoteCommand.Verbose());
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			var remotes = output.Output;
			int pos = 0;
			int l = remotes.Length;
			var res = new List<RemoteData>();
			while(pos < l)
			{
				var r = ParseRemote(remotes, ref pos);
				if(r != null) res.Add(r);
			}
			return res;
		}

		/// <summary>Add remote repository.</summary>
		/// <param name="parameters"><see cref="AddRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AddRemote(AddRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>((parameters.Branches != null ? parameters.Branches.Count : 0) + 6);
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
			args.Add(new CommandArgument(parameters.RemoteName));
			args.Add(new CommandArgument(parameters.Url));

			var cmd = new RemoteCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Rename remote repository.</summary>
		/// <param name="parameters"><see cref="RenameRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RenameRemote(RenameRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new RemoteCommand(
				RemoteCommand.Rename(),
				new CommandArgument(parameters.OldName),
				new CommandArgument(parameters.NewName));

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Get list of stale remote tracking branches that are subject to pruninig.</summary>
		/// <param name="parameters"><see cref="PruneRemoteParameters"/>.</param>
		/// <returns>List of stale remote tracking branches that are subject to pruninig.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<string> QueryPrunedBranches(PruneRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new RemoteCommand(
				RemoteCommand.Prune(),
				RemoteCommand.DryRun(),
				new CommandArgument(parameters.RemoteName));

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var res = new List<string>();

			var branches = output.Output;
			var pos = 0;
			var l = branches.Length;
			while(pos < l)
			{
				int end = branches.IndexOf('\n', pos);
				if(end == -1) end = l;

				if(CheckValue(branches, pos, " * [would prune] "))
				{
					res.Add(branches.Substring(pos + 17, end - pos - 17));
				}

				pos = end + 1;
			}
			return res;
		}

		/// <summary>Remove stale remote tracking branches.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void PruneRemote(PruneRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new RemoteCommand(
				RemoteCommand.Prune(),
				new CommandArgument(parameters.RemoteName));

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Remove remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RemoveRemote(RemoveRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = RemoteCommand.FormatRemoveCommand(parameters.RemoteName);

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Get list of references on remote repository.</summary>
		/// <param name="parameters"><see cref="QueryRemoteReferencesParameters"/>.</param>
		/// <returns>List of remote references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<RemoteReferenceData> QueryRemoteReferences(QueryRemoteReferencesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(4);
			if(parameters.Heads)
			{
				args.Add(LsRemoteCommand.Heads());
			}
			if(parameters.Tags)
			{
				args.Add(LsRemoteCommand.Tags());
			}
			args.Add(new CommandArgument(parameters.RemoteName));
			if(!string.IsNullOrEmpty(parameters.Pattern))
			{
				args.Add(new CommandArgument(parameters.Pattern));
			}

			var cmd = new LsRemoteCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var srefs = output.Output;
			var l = srefs.Length;
			int pos = 0;
			var refs = new List<RemoteReferenceData>();
			while(pos != -1 && pos < srefs.Length)
			{
				var rrinfo = ParseRemoteReference(srefs, ref pos);
				refs.Add(rrinfo);
			}
			return refs;
		}

		/// <summary>Remove reference on remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteReferencesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RemoveRemoteReferences(RemoveRemoteReferencesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(1 + parameters.References.Count);
			args.Add(new CommandArgument(parameters.RemoteName));
			foreach(var reference in parameters.References)
			{
				args.Add(new CommandArgument(":" + reference));
			}

			var cmd = new PushCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Download objects and refs from another repository.</summary>
		/// <param name="parameters"><see cref="FetchParameters"/>.</param>
		/// <returns>true if any objects were downloaded.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public bool Fetch(FetchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(10);
			InsertFetchParameters(parameters, args);
			var cmd = new FetchCommand(args);
			if(parameters.Monitor == null || !GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				var output = _executor.ExecCommand(cmd);
				output.ThrowOnBadReturnCode();
				return output.Error.Length != 0; // TODO: needs a better parser
			}
			else
			{
				var mon = parameters.Monitor;
				bool isCancelled = false;
				using(var async = _executor.ExecAsync(cmd))
				{
					mon.Cancelled += (sender, e) =>
						{
							async.Kill();
							isCancelled = true;
						};
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
								mon.SetProgressIntermediate();
							}
						};
					async.Start();
					async.WaitForExit();
					if(isCancelled)
					{
						return true;
					}
					if(async.ExitCode != 0)
					{
						throw new GitException(async.StdErr);
					}
					return true; // TODO: needs a better parser
				}
			}
		}

		/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
		/// <param name="parameters"><see cref="PullParameters"/>.</param>
		/// <returns>true if any objects were downloaded.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public bool Pull(PullParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();

			InsertPullParameters(parameters, args);
			var cmd = new PullCommand(args);
			if(parameters.Monitor == null || !GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				var output = _executor.ExecCommand(cmd);
				output.ThrowOnBadReturnCode();
				return output.Error.Length != 0; // TODO: needs a better parser
			}
			else
			{
				var mon = parameters.Monitor;
				bool isCancelled = false;
				using(var async = _executor.ExecAsync(cmd))
				{
					mon.Cancelled += (sender, e) =>
						{
							async.Kill();
							isCancelled = true;
						};
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
							mon.SetProgressIntermediate();
						}
					};
					async.Start();
					async.WaitForExit();
					if(isCancelled)
					{
						return true;
					}
					if(async.ExitCode != 0)
					{
						throw new GitException("git pull exited with code " + async.ExitCode);
					}
					return true; // TODO: needs a better parser
				}
			}
		}

		///	<summary>Update remote refs along with associated objects.</summary>
		/// <param name="parameters"><see cref="PushParameters"/>.</param>
		///	<returns>List of pushed references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<ReferencePushResult> Push(PushParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();
			InsertPushparameters(parameters, args);
			var cmd = new PushCommand(args);
			string refs;
			if(parameters.Monitor == null || !GitFeatures.ProgressFlag.IsAvailableFor(_gitCLI))
			{
				var output = _executor.ExecCommand(cmd);
				output.ThrowOnBadReturnCode();
				refs = output.Output;
			}
			else
			{
				var mon = parameters.Monitor;
				bool isCancelled = false;
				using(var async = _executor.ExecAsync(cmd))
				{
					mon.Cancelled += (sender, e) =>
						{
							async.Kill();
							isCancelled = true;
						};
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
								mon.SetProgressIntermediate();
							}
						};
					async.Start();
					async.WaitForExit();
					if(isCancelled)
					{
						return new ReferencePushResult[0];
					}
					if(async.ExitCode != 0)
					{
						throw new GitException(async.StdErr);
					}
					refs = async.StdOut;
				}
			}
			int pos = 0;
			int l = refs.Length;
			var res = new List<ReferencePushResult>();
			if(refs.StartsWith("To "))
			{
				pos = refs.IndexOf('\n');
				if(pos == -1)
					pos = l;
				else
					++pos;
			}
			while(pos < l)
			{
				if(CheckValue(refs, pos, "Done\n")) break;
				var refPushResult = ParsePushResult(refs, ref pos);
				if(refPushResult != null)
				{
					res.Add(refPushResult);
				}
			}
			return res;
		}
	}
}
