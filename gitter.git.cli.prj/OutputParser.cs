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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	sealed class OutputParser
	{
		#region Data

		private readonly GitCLI _gitCLI;

		#endregion

		#region .ctor

		public OutputParser(GitCLI gitCLI)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");

			_gitCLI = gitCLI;
		}

		#endregion

		#region Generic Errors

		private static bool IsUnknownRevisionError(string err, string revision)
		{
			const string errPrefix =
				"fatal: ambiguous argument '";
			const string errPostfix =
				"': unknown revision or path not in the working tree.\n" +
				"Use '--' to separate paths from revisions\n";

			return (err.Length == errPrefix.Length + errPostfix.Length + revision.Length) &&
				StringUtility.CheckValues(err, errPrefix, revision, errPostfix);
		}

		private static bool IsAutomaticMergeFailedError(string output)
		{
			const string error = "Automatic merge failed; fix conflicts and then commit the result.\n";

			return output.EndsWith(error);
		}

		private static bool IsRefrerenceIsNotATreeError(string error, string reference)
		{
			const string errPrefix = "fatal: reference is not a tree: ";
			const string errPostfix = "\n";

			return StringUtility.CheckValues(error, errPrefix, reference, errPostfix);
		}

		private static bool IsUnknownPathspecError(string error, string pathspec)
		{
			const string errPrefix = "error: pathspec '";
			const string errPostfix = "' did not match any file(s) known to git.\n";

			return StringUtility.CheckValues(error, errPrefix, pathspec, errPostfix);
		}

		private static bool IsUntrackedFileWouldBeOverwrittenError(string error, out string fileName)
		{
			const string errPrefix = "error: Untracked working tree file '";
			const string errPostfix = "' would be overwritten by merge.\n";

			if((error.Length > errPrefix.Length + errPostfix.Length) &&
				error.StartsWith(errPrefix) && errPrefix.EndsWith(errPrefix))
			{
				fileName = error.Substring(errPrefix.Length, error.Length - errPrefix.Length - errPostfix.Length);
				return true;
			}
			else
			{
				fileName = null;
				return false;
			}
		}

		private static bool IsHaveLocalChangesError(string error, out string fileName)
		{
			const string errPrefix = "error: You have local changes to '";
			const string errPostfix = "'; cannot switch branches.\n";

			if((error.Length > errPrefix.Length + errPostfix.Length) &&
				error.StartsWith(errPrefix) && errPrefix.EndsWith(errPrefix))
			{
				fileName = error.Substring(errPrefix.Length, error.Length - errPrefix.Length - errPostfix.Length);
				return true;
			}
			else
			{
				fileName = null;
				return false;
			}
		}

		private static bool IsHaveConflictsError(string error)
		{
			const string err = "error: you need to resolve your current index first\n";

			return error == err;
		}

		private static bool IsCherryPickIsEmptyError(string error)
		{
			const string err =
				"The previous cherry-pick is now empty, possibly due to conflict resolution.\n" +
				"If you wish to commit it anyway, use:\n" +
				"\n" +
				"    git commit --allow-empty\n" +
				"\n" +
				"Otherwise, please use 'git reset'\n";

			return error == err;
		}

		private static bool IsAutomaticCherryPickFailedError(string error)
		{
			const string errPrefix =
				"Automatic cherry-pick failed.  After resolving the conflicts,\n" +
				"mark the corrected paths with 'git add <paths>' or 'git rm <paths>'\n" +
				"and commit the result with: \n\n" +
				"        git commit -c ";
			const string errPostfix = "\n\n";

			return error.Length > errPrefix.Length + errPostfix.Length &&
				error.StartsWith(errPrefix) && error.EndsWith(errPostfix);
		}

		private static bool IsHaveLocalChangesMergeError(string error, out string fileName)
		{
			const string errPrefix =
				"error: Your local changes to '";

			const string errPostfix = "' would be overwritten by merge.  Aborting.\n" +
				"Please, commit your changes or stash them before you can merge.\n";

			if(error.Length > errPrefix.Length + errPostfix.Length)
			{
				if(error.StartsWith(errPrefix) && error.EndsWith(errPostfix))
				{
					fileName = error.Substring(errPrefix.Length, error.Length - errPrefix.Length - errPostfix.Length);
					return true;
				}
			}
			fileName = null;
			return false;
		}

		private static bool IsCherryPickNotPossibleBecauseOfConflictsError(string error)
		{
			const string err =
				"fatal: 'cherry-pick' is not possible because you have unmerged files.\n" +
				"Please, fix them up in the work tree, and then use 'git add/rm <file>' as\n" +
				"appropriate to mark resolution and make a commit, or use 'git commit -a'.\n";

			return error == err;
		}

		private static bool IsCherryPickNotPossibleBecauseOfMergeCommit(string err, string revision)
		{
			const string errPrefix = "fatal: Commit ";
			const string errPostfix = " is a merge but no -m option was given.\n";

			return (err.Length == errPrefix.Length + errPostfix.Length + revision.Length) &&
				StringUtility.CheckValues(err, errPrefix, revision, errPostfix);
		}

		private static bool IsCherryPickNotPossibleBecauseOfMergeCommit(string err)
		{
			const string errPrefix = "fatal: Commit ";
			const string errPostfix = " is a merge but no -m option was given.\n";

			return (err.Length > errPrefix.Length + errPostfix.Length) &&
				err.StartsWith(errPrefix) && err.EndsWith(errPostfix);
		}

		private static bool IsBadObjectError(string error, string obj)
		{
			const string errPrefix = "fatal: bad object ";
			const string errPostfix = "\n";

			return StringUtility.CheckValues(error, errPrefix, obj, errPostfix);
		}

		private static bool IsNoHEADCommitToCompareWithError(string error)
		{
			const string err = "fatal: No HEAD commit to compare with (yet)\n";

			return error == err;
		}

		private static bool IsNothingToApplyError(string error)
		{
			const string err = "Nothing to apply\n";

			return error == err;
		}

		private static bool IsCannotApplyToDirtyWorkingTreeError(string error)
		{
			const string err = "Cannot apply to a dirty working tree, please stage your changes\n";

			return error == err;
		}

		private static bool IsCantStashToEmptyRepositoryError(string error)
		{
			const string err =
				"fatal: bad revision 'HEAD'\n" +
				"fatal: bad revision 'HEAD'\n" +
				"fatal: Needed a single revision\n" +
				"You do not have the initial commit yet\n";

			return error == err;
		}

		#endregion

		public RevisionData ParseSingleRevision(QueryRevisionParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				var revName = parameters.SHA1.ToString();
				if(IsUnknownRevisionError(output.Error, revName))
				{
					throw new UnknownRevisionException(revName);
				}
				output.Throw();
			}
			var parser = new GitParser(output.Output);
			var rev = new RevisionData(parameters.SHA1);
			parser.ParseRevisionData(rev, null);
			return rev;
		}

		public IList<RevisionGraphData> ParseRevisionGraph(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			var result = new List<RevisionGraphData>();
			while(!parser.IsAtEndOfString)
			{
				var sha1 = parser.ReadString(40, 1);
				int end = parser.FindNullOrEndOfString();
				int numParents = (end - parser.Position + 1) / 41;
				if(numParents == 0)
				{
					parser.Position = end + 1;
					result.Add(new RevisionGraphData(sha1, new string[0]));
				}
				else
				{
					var parents = new List<string>(numParents);
					for(int i = 0; i < numParents; ++i)
					{
						parents.Add(parser.ReadString(40, 1));
					}
					result.Add(new RevisionGraphData(sha1, parents));
				}
			}
			return result;
		}

		public RevisionData ParseDereferenceOutput(DereferenceParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

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
				var hash = new Hash(output.Output);
				return new RevisionData(hash);
			}
		}

		public IList<TreeFileData> ParseFilesToAdd(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		public IList<string> ParseFilesToRemove(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var files = output.Output;
			var l = files.Length;
			var pos = 0;
			var res = new List<string>();
			while(pos < l)
			{
				var eol = files.IndexOf('\n', pos);
				if(eol == -1) eol = files.Length;
				if(StringUtility.CheckValue(files, pos, "rm '"))
				{
					res.Add(files.Substring(pos + 4, eol - pos - 5));
				}
				pos = eol + 1;
			}
			return res;
		}

		public IList<string> ParseFilesToClean(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		public Diff ParseDiff(QueryDiffParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(parameters.Cached && IsNoHEADCommitToCompareWithError(output.Error))
				{
					throw new RepositoryIsEmptyException(output.Error);
				}
				output.Throw();
			}
			var parser = new DiffParser(output.Output);
			var diffType = GetDiffType(parameters);
			return parser.ReadDiff(diffType);
		}

		public Diff ParseRevisionDiff(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var parser = new DiffParser(output.Output);
			return parser.ReadDiff(DiffType.CommittedChanges);
		}

		private static DiffType GetDiffType(QueryDiffParameters parameters)
		{
			Assert.IsNotNull(parameters);

			if(!string.IsNullOrWhiteSpace(parameters.Revision2))
			{
				return DiffType.CommitCompare;
			}
			if(parameters.Cached)
			{
				return DiffType.StagedChanges;
			}
			else
			{
				return DiffType.UnstagedChanges;
			}
		}

		public IList<UserData> ParseUsers(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		public ObjectCountData ParseObjectCountData(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			int count = 0;
			int size = 0;
			int inPack = 0;
			int packs = 0;
			int sizePack = 0;
			int prunePackable = 0;
			int garbage = 0;
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfString)
			{
				if(parser.CheckValueAndSkip("count: "))
				{
					int.TryParse(parser.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out count);
				}
				else if(parser.CheckValueAndSkip("size: "))
				{
					int.TryParse(parser.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out size);
				}
				else if(parser.CheckValueAndSkip("in-pack: "))
				{
					int.TryParse(parser.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out inPack);
				}
				else if(parser.CheckValueAndSkip("packs: "))
				{
					int.TryParse(parser.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out packs);
				}
				else if(parser.CheckValueAndSkip("size-pack: "))
				{
					int.TryParse(parser.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out sizePack);
				}
				else if(parser.CheckValueAndSkip("prune-packable: "))
				{
					int.TryParse(parser.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out prunePackable);
				}
				else if(parser.CheckValueAndSkip("garbage: "))
				{
					int.TryParse(parser.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out garbage);
				}
				else
				{
					parser.SkipLine();
				}
			}
			return new ObjectCountData(count, size, inPack, packs, sizePack, prunePackable, garbage);
		}

		public RemoteData ParseSingleRemote(QueryRemoteParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

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

		public IList<RemoteData> ParseRemotesOutput(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var remotes = output.Output;
			int pos = 0;
			int l = remotes.Length;
			var res = new List<RemoteData>();
			while(pos < l)
			{
				var r = ParseRemote(remotes, ref pos);
				if(r != null)
				{
					res.Add(r);
				}
			}
			return res;
		}

		private static RemoteData ParseRemote(string strRemote, ref int pos)
		{
			Assert.IsNotNull(strRemote);

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

		public IList<RemoteReferenceData> ParseRemoteReferences(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		private static RemoteReferenceData ParseRemoteReference(string output, ref int pos)
		{
			Assert.IsNotNull(output);

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
					if(StringUtility.CheckValues(output, pos2, name, GitConstants.DereferencedTagPostfix))
					{
						pos = end + 1;
						return new RemoteReferenceData(name, new Hash(hash2)) { TagType = TagType.Annotated };
					}
				}
			}
			return new RemoteReferenceData(name, new Hash(hash));
		}

		public IList<string> ParsePrunedBranches(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var res = new List<string>();
			var branches = output.Output;
			var pos = 0;
			var l = branches.Length;
			while(pos < l)
			{
				int end = branches.IndexOf('\n', pos);
				if(end == -1) end = l;

				if(StringUtility.CheckValue(branches, pos, " * [would prune] "))
				{
					res.Add(branches.Substring(pos + 17, end - pos - 17));
				}

				pos = end + 1;
			}
			return res;
		}

		public IList<NoteData> ParseNotes(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var res = new List<NoteData>();
			var notes = output.Output;
			if(notes.Length > 81)
			{
				var parser = new GitParser(notes);
				while(!parser.IsAtEndOfString)
				{
					var noteSHA1 = parser.ReadString(40, 1);
					var objectSHA1 = parser.ReadString(40, 1);
					res.Add(new NoteData(noteSHA1, objectSHA1, null));
				}
			}
			return res;
		}

		public bool ParseStashSaveResult(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		public IList<TreeContentData> ParseTreeContent(GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var content = output.Output;
			int pos = 0;
			int l = content.Length;
			var res = new List<TreeContentData>();
			// <mode> SP <type> SP <object> SP <object size> TAB <file>
			while(pos < l)
			{
				var end = content.IndexOf('\0', pos);
				if(end == -1) end = l;

				int delimeter = content.IndexOf(' ', pos);
				int mode = int.Parse(content.Substring(pos, delimeter - pos), CultureInfo.InvariantCulture);
				pos = delimeter + 1;
				while(content[pos] == ' ')
				{
					++pos;
				}

				bool isTree		= StringUtility.CheckValue(content, pos, GitConstants.TreeObjectType);
				bool isBlob		= !isTree && StringUtility.CheckValue(content, pos, GitConstants.BlobObjectType);
				bool isCommit	= !isTree && !isBlob && StringUtility.CheckValue(content, pos, GitConstants.CommitObjectType);
				bool isTag		= !isTree && !isBlob && !isCommit && StringUtility.CheckValue(content, pos, GitConstants.TagObjectType);

				pos += 5;
				delimeter = content.IndexOf(' ', pos);
				var hash = content.Substring(pos, delimeter - pos);
				pos += 41;
				while(content[pos] == ' ')
				{
					++pos;
				}
				delimeter = content.IndexOf('\t', pos);
				long size = 0;
				if(isBlob)
				{
					size = long.Parse(content.Substring(pos, delimeter - pos), CultureInfo.InvariantCulture);
				}
				pos = delimeter + 1;
				var name = content.Substring(pos, end - pos);
				if(isBlob)
				{
					res.Add(new BlobData(hash, mode, name, size));
				}
				else if(isTree)
				{
					res.Add(new TreeData(hash, mode, name));
				}
				else if(isCommit)
				{
					res.Add(new TreeCommitData(hash, mode, name));
				}
				pos = end + 1;
			}
			return res;
		}

		public void HandleStashApplyResult(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		public void HandleStashPopResult(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		public void HandleCheckoutResult(CheckoutParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

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

		public void HandleRevertResult(GitOutput output)
		{
			Assert.IsNotNull(output);

			if(output.Error != "Finished one revert.\n") // TODO: needs a better parser.
			{
				output.ThrowOnBadReturnCode();
			}
		}

		public void HandleCherryPickResult(GitOutput output)
		{
			Assert.IsNotNull(output);

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

		public void HandleMergeResult(GitOutput output)
		{
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(IsAutomaticMergeFailedError(output.Output))
				{
					throw new AutomaticMergeFailedException();
				}
				output.Throw();
			}
		}

		public BlameFile ParseBlame(QueryBlameParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var parser = new BlameParser(output.Output);
			return parser.ParseBlameFile(parameters.FileName);
		}

		public ReferencesData ParseReferences(QueryReferencesParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			var refTypes = parameters.ReferenceTypes;

			bool needHeads		= (refTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch;
			bool needRemotes	= (refTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch;
			bool needTags		= (refTypes & ReferenceType.Tag) == ReferenceType.Tag;
			bool needStash		= (refTypes & ReferenceType.Stash) == ReferenceType.Stash;

			var heads	= needHeads   ? new List<BranchData>() : null;
			var remotes	= needRemotes ? new List<BranchData>() : null;
			var tags	= needTags    ? new List<TagData>()    : null;
			RevisionData stash = null;

			bool encounteredRemoteBranch = false;
			bool encounteredStash = false;
			bool encounteredTag = false;

			var refs = output.Output;
			int pos = 0;
			int l = refs.Length;
			while(pos < l)
			{
				var hash = new Hash(refs, pos);
				pos += 41;
				var end = refs.IndexOf('\n', pos);
				if(end == -1) end = l;

				if(!encounteredRemoteBranch && StringUtility.CheckValue(refs, pos, GitConstants.LocalBranchPrefix))
				{
					if(needHeads)
					{
						pos += GitConstants.LocalBranchPrefix.Length;
						var name = refs.Substring(pos, end - pos);
						var branch = new BranchData(name, hash, false, false, false);
						heads.Add(branch);
					}
				}
				else if(!encounteredStash && StringUtility.CheckValue(refs, pos, GitConstants.RemoteBranchPrefix))
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
				else if(!encounteredTag && !encounteredStash && StringUtility.CheckValue(refs, pos, GitConstants.StashFullName))
				{
					encounteredRemoteBranch = true;
					encounteredStash = true;
					if(needStash)
					{
						stash = new RevisionData(hash);
					}
				}
				else if(StringUtility.CheckValue(refs, pos, GitConstants.TagPrefix))
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
								if(StringUtility.CheckValue(refs, pos2, name) && StringUtility.CheckValue(refs, pos2 + name.Length, GitConstants.DereferencedTagPostfix))
								{
									type = TagType.Annotated;
									hash = new Hash(refs, s2);
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

		#region Branches

		public BranchData ParseSingleBranch(QueryBranchParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode == 0)
			{
				var hash = new Hash(output.Output);
				return new BranchData(parameters.BranchName, hash, false, parameters.IsRemote, false);
			}
			else
			{
				return null;
			}
		}

		public BranchesData ParseBranches(QueryBranchesParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			return parser.ParseBranches(parameters.Restriction, parameters.AllowFakeBranch);
		}

		public void HandleCreateBranchResult(CreateBranchParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(IsUnknownRevisionError(output.Error, parameters.StartingRevision))
				{
					throw new UnknownRevisionException(parameters.StartingRevision);
				}
				if(IsBranchAlreadyExistsError(output.Error, parameters.BranchName))
				{
					throw new BranchAlreadyExistsException(parameters.BranchName);
				}
				if(IsInvalidBranchNameError(output.Error, parameters.BranchName))
				{
					throw new InvalidBranchNameException(parameters.BranchName);
				}
				output.Throw();
			}
		}

		public void HandleDeleteBranchResult(DeleteBranchParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(IsBranchNotFoundError(output.Error, parameters.Remote, parameters.BranchName))
				{
					throw new BranchNotFoundException(parameters.BranchName);
				}
				if(!parameters.Force)
				{
					if(IsBranchNotFullyMergedError(output.Error, parameters.BranchName))
					{
						throw new BranchIsNotFullyMergedException(parameters.BranchName);
					}
				}
				output.Throw();
			}
		}

		public void HandleRenameBranchResult(RenameBranchParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(IsBranchAlreadyExistsError(output.Error, parameters.NewName))
				{
					throw new BranchAlreadyExistsException(parameters.NewName);
				}
				if(IsInvalidBranchNameError(output.Error, parameters.NewName))
				{
					throw new InvalidBranchNameException(parameters.NewName);
				}
				output.Throw();
			}
		}

		private static bool IsBranchNotFullyMergedError(string error, string branchName)
		{
			const string errNotMerged0 =
				"warning: not deleting branch '";
			const string errNotMerged1 =
				"' that is not yet merged to\n         '";
			const string errNotMerged2 =
				"', even though it is merged to HEAD.\n";
			const string errNotMerged3 =
				"error: The branch '";
			const string errNotMerged4 =
				"' is not fully merged.\n" +
				"If you are sure you want to delete it, run 'git branch -D ";
			const string errNotMerged5 =
				"'.\n";

			if(error.Length == errNotMerged3.Length + errNotMerged4.Length + errNotMerged5.Length + 2 * branchName.Length)
			{
				return StringUtility.CheckValues(error, errNotMerged3, branchName, errNotMerged4, branchName, errNotMerged5);
			}
			else
			{
				if(StringUtility.CheckValues(error, 0, errNotMerged0, branchName, errNotMerged1))
				{
					int pos = error.IndexOf(errNotMerged2, errNotMerged0.Length + errNotMerged1.Length + branchName.Length);
					if(pos != -1)
					{
						pos += errNotMerged2.Length;
						return StringUtility.CheckValues(error, pos, errNotMerged3, branchName, errNotMerged4, branchName, errNotMerged5);
					}
				}
				return false;
			}
		}

		private static bool IsBranchAlreadyExistsError(string error, string branchName)
		{
			const string errPrefix = "fatal: A branch named '";
			const string errPostfix = "' already exists.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
				StringUtility.CheckValues(error, errPrefix, branchName, errPostfix);
		}

		private static bool IsBranchNotFoundError(string error, bool remote, string branchName)
		{
			if(remote)
			{
				const string errPrefix = "error: remote branch '";
				const string errPostfix = "' not found.\n";

				return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
					StringUtility.CheckValues(error, errPrefix, branchName, errPostfix);
			}
			else
			{
				const string errPrefix = "error: branch '";
				const string errPostfix = "' not found.\n";

				return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
					StringUtility.CheckValues(error, errPrefix, branchName, errPostfix);
			}
		}

		private static bool IsInvalidBranchNameError(string error, string branchName)
		{
			const string errPrefix = "fatal: '";
			const string errPostfix = "' is not a valid branch name.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
				StringUtility.CheckValues(error, errPrefix, branchName, errPostfix);
		}

		#endregion

		#region Tags

		public TagData ParseTag(QueryTagParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			var tag = output.Output;
			if(output.ExitCode == 0 && tag.Length >= 40)
			{
				Hash hash;
				TagType type;
				if(tag.Length >= 81)
				{
					hash = new Hash(output.Output, 41);
					type = TagType.Annotated;
				}
				else
				{
					hash = new Hash(output.Output);
					type = TagType.Lightweight;
				}
				return new TagData(parameters.TagName, hash, type);
			}
			else
			{
				return null;
			}
		}

		public IList<TagData> ParseTags(GitOutput output)
		{
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				return new TagData[0];
			}
			var tags = output.Output;
			int pos = 0;
			int l = tags.Length;
			var res = new List<TagData>();
			while(pos < l)
			{
				var tag = TryParseTag(tags, ref pos);
				if(tag != null) res.Add(tag);
			}
			return res;
		}

		private static TagData TryParseTag(string strTag, ref int pos)
		{
			Assert.IsNotNull(strTag);

			var strHash = strTag.Substring(pos, 40);
			int pos2 = strTag.IndexOf('\n', pos);
			if(pos2 < 0) pos2 = strTag.Length;
			pos += 41;
			var strName = strTag.Substring(pos + 10, pos2 - pos - 10);
			pos = pos2 + 1;
			var type = TagType.Lightweight;
			if(pos < strTag.Length)
			{
				pos2 = strTag.IndexOf('\n', pos);
				if(pos2 != -1)
				{
					if(strTag[pos2 - 3] == '^')
					{
						type = TagType.Annotated;
						strHash = strTag.Substring(pos, 40);
						pos = pos2 + 1;
					}
				}
			}
			return new TagData(strName, new Hash(strHash), type);
		}

		public string ParseDescribeResult(DescribeParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(parameters.Revision != null)
				{
					if(IsUnknownRevisionError(output.Error, parameters.Revision))
					{
						throw new UnknownRevisionException(parameters.Revision);
					}
				}
				output.Throw();
			}
			if(string.IsNullOrWhiteSpace(output.Output))
			{
				return null;
			}
			return output.Output;
		}

		public IList<ReferencePushResult> ParsePushResults(string output)
		{
			Assert.IsNotNull(output);

			int pos = 0;
			int l = output.Length;
			var res = new List<ReferencePushResult>();
			if(output.StartsWith("To "))
			{
				pos = output.IndexOf('\n');
				if(pos == -1)
				{
					pos = l;
				}
				else
				{
					++pos;
				}
			}
			while(pos < l)
			{
				if(StringUtility.CheckValue(output, pos, "Done\n")) break;
				var refPushResult = ParsePushResult(output, ref pos);
				if(refPushResult != null)
				{
					res.Add(refPushResult);
				}
			}
			return res;
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

		public void HandleCreateTagResult(CreateTagParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(IsUnknownRevisionError(output.Error, parameters.TaggedObject))
				{
					throw new UnknownRevisionException(parameters.TaggedObject);
				}
				if(IsTagAlreadyExistsError(output.Error, parameters.TagName))
				{
					throw new TagAlreadyExistsException(parameters.TagName);
				}
				if(IsInvalidTagNameError(output.Error, parameters.TagName))
				{
					throw new InvalidTagNameException(parameters.TagName);
				}
				output.Throw();
			}
		}

		public void HandleDeleteTagResult(DeleteTagParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0)
			{
				if(IsTagNotFoundError(output.Error, parameters.TagName))
				{
					throw new TagNotFoundException(parameters.TagName);
				}
				output.Throw();
			}
		}

		private static bool IsTagAlreadyExistsError(string error, string tagName)
		{
			const string errPrefix = "fatal: tag '";
			const string errPostfix = "' already exists\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + tagName.Length) &&
				StringUtility.CheckValues(error, errPrefix, tagName, errPostfix);
		}

		private static bool IsTagNotFoundError(string error, string tagName)
		{
			const string errPrefix = "error: tag '";
			const string errPostfix = "' not found.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + tagName.Length) &&
				StringUtility.CheckValues(error, errPrefix, tagName, errPostfix);
		}

		private static bool IsInvalidTagNameError(string error, string tagName)
		{
			const string errPrefix = "fatal: '";
			const string errPostfix = "' is not a valid tag name.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + tagName.Length) &&
				StringUtility.CheckValues(error, errPrefix, tagName, errPostfix);
		}

		#endregion

		public RevisionData ParseQueryStashTopOutput(QueryStashTopParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(parameters.LoadCommitInfo)
			{
				return new GitParser(output.Output).ParseRevision();
			}
			else
			{
				if(output.ExitCode != 0 || output.Output.Length < 40)
				{
					return null;
				}

				var hash = new Hash(output.Output);
				return new RevisionData(hash);
			}
		}

		public string ParseObjects(GitOutput output)
		{
			return output.Output;
		}

		#region Config

		public void HandleConfigResults(GitOutput output)
		{
			Assert.IsNotNull(output);

			switch(output.ExitCode)
			{
				case 0:
					return;
				case 1:
					throw new InvalidConfigFileException(output.Error);
				case 2:
					throw new CannotWriteConfigFileException(output.Error);
				case 3:
					throw new NoSectionProvidedException(output.Error);
				case 4:
					throw new InvalidSectionOrKeyException(output.Error);
				case 5:
					throw new ConfigParameterDoesNotExistException(output.Error);
				default:
					output.Throw();
					break;
			}
		}

		public ConfigParameterData ParseQueryConfigParameterResult(QueryConfigParameterParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode == 0)
			{
				var value = output.Output.TrimEnd('\n');
				return new ConfigParameterData(parameters.ParameterName, value, parameters.ConfigFile, parameters.FileName);
			}
			else
			{
				return null;
			}
		}

		public IList<ConfigParameterData> ParseQueryConfigResults(QueryConfigParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0 && parameters.ConfigFile != ConfigFile.Other)
			{
				return new ConfigParameterData[0];
			}
			HandleConfigResults(output);
			var res = new List<ConfigParameterData>();
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfString)
			{
				var name = parser.ReadStringUpTo(parser.FindNewLineOrEndOfString(), 1);
				var value = parser.ReadStringUpTo(parser.FindNullOrEndOfString(), 1);
				if(parameters.ConfigFile != ConfigFile.Other)
				{
					res.Add(new ConfigParameterData(name, value, parameters.ConfigFile));
				}
				else
				{
					res.Add(new ConfigParameterData(name, value, parameters.FileName));
				}
			}
			return res;
		}

		#endregion
	}
}
