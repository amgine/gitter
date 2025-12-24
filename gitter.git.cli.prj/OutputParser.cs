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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using gitter.Framework;

sealed class OutputParser(GitCLI cli)
{
	public GitCLI CLI { get; } = cli;

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

	private static bool IsUntrackedFileWouldBeOverwrittenError(string error,
		[MaybeNullWhen(returnValue: false)] out string fileName)
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

	private static bool IsHaveLocalChangesError(string error,
		[MaybeNullWhen(returnValue: false)] out string fileName)
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
		fileName = null!;
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

	public RevisionData ParseSingleRevision(QueryRevisionRequest parameters, GitOutput output)
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
		parser.ParseRevisionData(rev, cache: null);
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
				result.Add(new RevisionGraphData(sha1, Many<string>.None));
			}
			else
			{
				var parents = new string[numParents];
				for(int i = 0; i < numParents; ++i)
				{
					parents[i] = parser.ReadString(40, 1);
				}
				result.Add(new RevisionGraphData(sha1, parents));
			}
		}
		return result;
	}

	public RevisionData ParseDereferenceOutput(DereferenceRequest parameters, GitOutput output)
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
		return new(Sha1Hash.Parse(output.Output));
	}

	public IList<TreeFileData> ParseFilesToAdd(GitOutput output)
	{
		Assert.IsNotNull(output);

		if(output.ExitCode != 0 && output.ExitCode != 128)
		{
			return [];
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
			string filePath = null!;
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

	public Diff ParseDiff(QueryDiffRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		if(output.ExitCode != 0)
		{
			if(request.Cached && IsNoHEADCommitToCompareWithError(output.Error))
			{
				throw new RepositoryIsEmptyException(output.Error);
			}
			output.Throw();
		}
		var parser = new DiffParser(output.Output);
		var diffType = GetDiffType(request);
		return parser.ReadDiff(diffType);
	}

	public Diff ParseRevisionDiff(GitOutput output)
	{
		Assert.IsNotNull(output);

		output.ThrowOnBadReturnCode();
		var parser = new DiffParser(output.Output);
		return parser.ReadDiff(DiffType.CommittedChanges);
	}

	private static DiffType GetDiffType(QueryDiffRequest parameters)
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
			#if NETCOREAPP
			int commitsCount = int.Parse(parser.ReadSpanUpTo(tab, skip: 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
			#else
			int commitsCount = int.Parse(parser.ReadStringUpTo(tab, skip: 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
			#endif
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
				count = parser.ReadLineAsInt32();
			}
			else if(parser.CheckValueAndSkip("size: "))
			{
				size = parser.ReadLineAsInt32();
			}
			else if(parser.CheckValueAndSkip("in-pack: "))
			{
				inPack = parser.ReadLineAsInt32();
			}
			else if(parser.CheckValueAndSkip("packs: "))
			{
				packs = parser.ReadLineAsInt32();
			}
			else if(parser.CheckValueAndSkip("size-pack: "))
			{
				sizePack = parser.ReadLineAsInt32();
			}
			else if(parser.CheckValueAndSkip("prune-packable: "))
			{
				prunePackable = parser.ReadLineAsInt32();
			}
			else if(parser.CheckValueAndSkip("garbage: "))
			{
				garbage = parser.ReadLineAsInt32();
			}
			else
			{
				parser.SkipLine();
			}
		}
		return new ObjectCountData(count, size, inPack, packs, sizePack, prunePackable, garbage);
	}

	public RemoteData ParseSingleRemote(QueryRemoteRequest parameters, GitOutput output)
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
			if(ParseRemote(remotes, ref pos) is { } remote)
			{
				res.Add(remote);
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

		var hash = Sha1Hash.Parse(output, pos);
		pos += Sha1Hash.HexStringLength + 1;
		while(output[pos] is ' ' or '\t') ++pos;
		int end = output.IndexOf('\n', pos);
		if(end == -1) end = output.Length;
		var name = output.Substring(pos, end - pos);
		pos = end + 1;
		if(name.StartsWith(GitConstants.TagPrefix) && pos < output.Length)
		{
			end = output.IndexOf('\n', pos);
			if(end == -1) end = output.Length;
			var hash2 = Sha1Hash.Parse(output, pos);
			int pos2 = pos + Sha1Hash.HexStringLength + 1;
			while(output[pos2] is ' ' or '\t') ++pos2;
			int l = end - pos2;
			if(l == name.Length + GitConstants.DereferencedTagPostfix.Length)
			{
				if(StringUtility.CheckValues(output, pos2, name, GitConstants.DereferencedTagPostfix))
				{
					pos = end + 1;
					return new RemoteReferenceData(name, hash2) { TagType = TagType.Annotated };
				}
			}
		}
		return new RemoteReferenceData(name, hash);
	}

	public IList<string> ParsePrunedBranches(GitOutput output)
	{
		Assert.IsNotNull(output);

		output.ThrowOnBadReturnCode();
		var res = new List<string>();
		var branches = output.Output;
		var pos = 0;
		while(pos < branches.Length)
		{
			int end = branches.IndexOf('\n', pos);
			if(end == -1) end = branches.Length;

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
		if(notes.Length > Sha1Hash.HexStringLength * 2 + 1)
		{
			var parser = new GitParser(notes);
			while(!parser.IsAtEndOfString)
			{
				var noteSHA1   = parser.ReadString(Sha1Hash.HexStringLength, 1);
				var objectSHA1 = parser.ReadString(Sha1Hash.HexStringLength, 1);
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

	private static void SkipSpaces(string content, ref int pos)
	{
		while(pos < content.Length && content[pos] == ' ') ++pos;
	}

	public IList<TreeContentData> ParseTreeContent(GitOutput output)
	{
		Assert.IsNotNull(output);

		static TreeContentType GetTreeObjectType(string content, ref int pos)
		{
			static bool Check(string content, string type, ref int pos)
			{
				if(!StringUtility.CheckValue(content, pos, type)) return false;
				var space = pos + type.Length;
				if(space >= content.Length || content[space] != ' ') return false;
				pos = space + 1;
				SkipSpaces(content, ref pos);
				return true;
			}

			if(Check(content, GitConstants.TreeObjectType, ref pos))
			{
				return TreeContentType.Tree;
			}
			if(Check(content, GitConstants.BlobObjectType, ref pos))
			{
				return TreeContentType.Blob;
			}
			if(Check(content, GitConstants.CommitObjectType, ref pos))
			{
				return TreeContentType.Commit;
			}
			if(Check(content, GitConstants.TagObjectType, ref pos))
			{
				return (TreeContentType)(-1);
			}
			return (TreeContentType)(-1);
		}

		static int ParseMode(string content, ref int pos)
		{
			int delimeter = content.IndexOf(' ', pos);
			#if NETCOREAPP
			int mode = int.Parse(content.AsSpan(pos, delimeter - pos), NumberStyles.Integer, CultureInfo.InvariantCulture);
			#else
			int mode = int.Parse(content.Substring(pos, delimeter - pos), CultureInfo.InvariantCulture);
			#endif
			pos = delimeter + 1;
			SkipSpaces(content, ref pos);
			return mode;
		}

		static Sha1Hash ParseHash(string content, ref int pos)
		{
			var delimeter = content.IndexOf(' ', pos);
			var hash = Sha1Hash.Parse(content, pos);
			pos += Sha1Hash.HexStringLength + 1;
			SkipSpaces(content, ref pos);
			return hash;
		}

		output.ThrowOnBadReturnCode();
		var content = output.Output;
		int pos = 0;
		var res = new List<TreeContentData>();
		// <mode> SP <type> SP <object> SP <object size> TAB <file>
		while(pos < content.Length)
		{
			var end = content.IndexOf('\0', pos);
			if(end == -1) end = content.Length;

			var mode = ParseMode        (content, ref pos);
			var type = GetTreeObjectType(content, ref pos);
			var hash = ParseHash        (content, ref pos);

			var delimeter = content.IndexOf('\t', pos);
			long size = 0;
			if(type == TreeContentType.Blob)
			{
				#if NETCOREAPP
				size = long.Parse(content.AsSpan(pos, delimeter - pos), NumberStyles.Integer, CultureInfo.InvariantCulture);
				#else
				size = long.Parse(content.Substring(pos, delimeter - pos), NumberStyles.Integer, CultureInfo.InvariantCulture);
				#endif
			}
			pos = delimeter + 1;
			var name = content.Substring(pos, end - pos);
			TreeContentData? data = type switch
			{
				TreeContentType.Tree   => new TreeData(hash, mode, name),
				TreeContentType.Blob   => new BlobData(hash, mode, name, size),
				TreeContentType.Commit => new TreeCommitData(hash, mode, name),
				_ => default,
			};
			if(data is not null)
			{
				res.Add(data);
			}
			pos = end + 1;
		}
		return res;
	}

	public void HandleStashApplyResult(GitOutput output)
	{
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

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

	public void HandleStashPopResult(GitOutput output)
	{
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

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

	public void HandleCheckoutResult(CheckoutRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

		if(IsUnknownRevisionError(output.Error, request.Revision))
		{
			throw new UnknownRevisionException(request.Revision);
		}
		if(IsRefrerenceIsNotATreeError(output.Error, request.Revision))
		{
			throw new UnknownRevisionException(request.Revision);
		}
		if(IsUnknownPathspecError(output.Error, request.Revision))
		{
			throw new UnknownRevisionException(request.Revision);
		}
		if(!request.Force)
		{
			string? fileName;
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

		if(output.ExitCode == 0) return;

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

	public void HandleMergeResult(GitOutput output)
	{
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

		if(IsAutomaticMergeFailedError(output.Output))
		{
			throw new AutomaticMergeFailedException();
		}
		output.Throw();
	}

	public BlameFile ParseBlame(QueryBlameRequest parameters, GitOutput output)
	{
		Assert.IsNotNull(parameters);
		Assert.IsNotNull(output);

		output.ThrowOnBadReturnCode();
		var parser = new BlameParser(output.Output);
		return parser.ParseBlameFile(parameters.FileName);
	}

	public ReferencesData ParseReferences(QueryReferencesRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		var refTypes = request.ReferenceTypes;

		bool needHeads		= (refTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch;
		bool needRemotes	= (refTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch;
		bool needTags		= (refTypes & ReferenceType.Tag) == ReferenceType.Tag;
		bool needStash		= (refTypes & ReferenceType.Stash) == ReferenceType.Stash;

		var heads	= needHeads   ? new List<BranchData>() : null;
		var remotes	= needRemotes ? new List<BranchData>() : null;
		var tags	= needTags    ? new List<TagData>()    : null;
		var stash   = default(RevisionData);

		bool encounteredRemoteBranch = false;
		bool encounteredStash = false;
		bool encounteredTag = false;

		var refs = output.Output;
		int pos = 0;
		int l = refs.Length;
		while(pos < l)
		{
			var hash = Sha1Hash.Parse(refs, pos);
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
					heads!.Add(branch);
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
						remotes!.Add(branch);
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
								hash = Sha1Hash.Parse(refs, s2);
								end = end2;
							}
						}
					}
					var tag = new TagData(name, hash, type);
					tags!.Add(tag);
				}
				else break;
			}
			pos = end + 1;
		}
		return new ReferencesData(heads, remotes, tags, stash);
	}

	#region Branches

	public BranchData? ParseSingleBranch(QueryBranchRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		if(output.ExitCode == 0)
		{
			var hash = Sha1Hash.Parse(output.Output);
			return new BranchData(request.BranchName, hash, false, request.IsRemote, false);
		}
		else
		{
			return null;
		}
	}

	public BranchesData ParseBranches(QueryBranchesRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		output.ThrowOnBadReturnCode();
		var parser = new GitParser(output.Output);
		return parser.ParseBranches(request.Restriction, request.AllowFakeBranch);
	}

	public void HandleCreateBranchResult(CreateBranchRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

		if(IsUnknownRevisionError(output.Error, request.StartingRevision))
		{
			throw new UnknownRevisionException(request.StartingRevision);
		}
		if(IsBranchAlreadyExistsError(output.Error, request.BranchName))
		{
			throw new BranchAlreadyExistsException(request.BranchName);
		}
		if(IsInvalidBranchNameError(output.Error, request.BranchName))
		{
			throw new InvalidBranchNameException(request.BranchName);
		}
		output.Throw();
	}

	public void HandleDeleteBranchResult(DeleteBranchRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

		if(IsBranchNotFoundError(output.Error, request.Remote, request.BranchName))
		{
			throw new BranchNotFoundException(request.BranchName);
		}
		if(!request.Force)
		{
			if(IsBranchNotFullyMergedError(output.Error, request.BranchName))
			{
				throw new BranchIsNotFullyMergedException(request.BranchName);
			}
		}
		output.Throw();
	}

	public void HandleRenameBranchResult(RenameBranchRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

		if(IsBranchAlreadyExistsError(output.Error, request.NewName))
		{
			throw new BranchAlreadyExistsException(request.NewName);
		}
		if(IsInvalidBranchNameError(output.Error, request.NewName))
		{
			throw new InvalidBranchNameException(request.NewName);
		}
		output.Throw();
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

	public TagData? ParseTag(QueryTagRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		var tag = output.Output;
		if(output.ExitCode == 0 && tag.Length >= Sha1Hash.HexStringLength)
		{
			Sha1Hash hash;
			TagType type;
			if(tag.Length >= Sha1Hash.HexStringLength * 2 + 1)
			{
				hash = Sha1Hash.Parse(output.Output, Sha1Hash.HexStringLength + 1);
				type = TagType.Annotated;
			}
			else
			{
				hash = Sha1Hash.Parse(output.Output);
				type = TagType.Lightweight;
			}
			return new TagData(request.TagName, hash, type);
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
			return Preallocated<TagData>.EmptyArray;
		}
		var tags = output.Output;
		int pos = 0;
		int l = tags.Length;
		var res = new List<TagData>();
		while(pos < l)
		{
			var tag = TryParseTag(tags, ref pos);
			if(tag is not null) res.Add(tag);
		}
		return res;
	}

	private static TagData TryParseTag(string strTag, ref int pos)
	{
		Assert.IsNotNull(strTag);

		var strHash = strTag.Substring(pos, Sha1Hash.HexStringLength);
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
					strHash = strTag.Substring(pos, Sha1Hash.HexStringLength);
					pos = pos2 + 1;
				}
			}
		}
		return new TagData(strName, Sha1Hash.Parse(strHash), type);
	}

	public string? ParseDescribeResult(DescribeRequest parameters, GitOutput output)
	{
		Assert.IsNotNull(parameters);
		Assert.IsNotNull(output);

		if(output.ExitCode != 0)
		{
			if(parameters.Revision is not null)
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

	public string ParseCommitResult(CommitRequest parameters, GitOutput output)
	{
		return output.Output;
	}

	public Many<ReferencePushResult> ParsePushResults(string output)
	{
		Assert.IsNotNull(output);

		int pos = 0;
		int l = output.Length;
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
		var builder = new Many<ReferencePushResult>.Builder();
		while(pos < l)
		{
			if(StringUtility.CheckValue(output, pos, "Done\n")) break;
			if(ParsePushResult(output, ref pos) is { } result)
			{
				builder.Add(result);
			}
		}
		return builder;
	}

	private static PushResultType? GetPushResultType(string output, int pos)
	{
		if((output.Length <= pos + 1) || (output[pos + 1] != '\t')) return default;
		return output[pos] switch
		{
			' ' => PushResultType.FastForwarded,
			'+' => PushResultType.ForceUpdated,
			'-' => PushResultType.DeletedReference,
			'*' => PushResultType.CreatedReference,
			'!' => PushResultType.Rejected,
			'=' => PushResultType.UpToDate,
			 _  => default,
		};
	}

	private static ReferencePushResult? ParsePushResult(string output, ref int pos)
	{
		var type = GetPushResultType(output, pos);
		if(!type.HasValue)
		{
			pos = output.IndexOf('\n', pos) + 1;
			if(pos == 0) pos = output.Length + 1;
			return default;
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
		return new ReferencePushResult(type.Value, from, to, summary);
	}

	public void HandleCreateTagResult(CreateTagRequest request, GitOutput output)
	{
		Assert.IsNotNull(request);
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

		if(IsUnknownRevisionError(output.Error, request.TaggedObject))
		{
			throw new UnknownRevisionException(request.TaggedObject);
		}
		if(IsTagAlreadyExistsError(output.Error, request.TagName))
		{
			throw new TagAlreadyExistsException(request.TagName);
		}
		if(IsInvalidTagNameError(output.Error, request.TagName))
		{
			throw new InvalidTagNameException(request.TagName);
		}
		output.Throw();
	}

	public void HandleDeleteTagResult(DeleteTagRequest parameters, GitOutput output)
	{
		Assert.IsNotNull(parameters);
		Assert.IsNotNull(output);

		if(output.ExitCode == 0) return;

		if(IsTagNotFoundError(output.Error, parameters.TagName))
		{
			throw new TagNotFoundException(parameters.TagName);
		}
		output.Throw();
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

	public RevisionData? ParseQueryStashTopOutput(QueryStashTopRequest parameters, GitOutput output)
	{
		Assert.IsNotNull(parameters);
		Assert.IsNotNull(output);

		if(parameters.LoadCommitInfo)
		{
			return new GitParser(output.Output).ParseRevision();
		}
		if(output.ExitCode != 0 || output.Output.Length < 40)
		{
			return null;
		}
		var hash = Sha1Hash.Parse(output.Output);
		return new RevisionData(hash);
	}

	public string ParseObjects(GitOutput output)
		=> output.Output;

	#region Config

	public void HandleConfigResults(GitOutput output)
	{
		Assert.IsNotNull(output);

		switch(output.ExitCode)
		{
			case 0: return;
			case 1: throw new InvalidConfigFileException(output.Error);
			case 2: throw new CannotWriteConfigFileException(output.Error);
			case 3: throw new NoSectionProvidedException(output.Error);
			case 4: throw new InvalidSectionOrKeyException(output.Error);
			case 5: throw new ConfigParameterDoesNotExistException(output.Error);
			default:
				output.Throw();
				break;
		}
	}

	public ConfigParameterData? ParseQueryConfigParameterResult(QueryConfigParameterRequest parameters, GitOutput output)
	{
		Assert.IsNotNull(parameters);
		Assert.IsNotNull(output);

		if(output.ExitCode != 0) return default;

		var value = output.Output.TrimEnd('\n');
		return new ConfigParameterData(parameters.ParameterName, value, parameters.ConfigFile, parameters.FileName);
	}

	public IList<ConfigParameterData> ParseQueryConfigResults(QueryConfigRequest parameters, GitOutput output)
	{
		Assert.IsNotNull(parameters);
		Assert.IsNotNull(output);

		if(output.ExitCode != 0 && parameters.ConfigFile != ConfigFile.Other)
		{
			return Preallocated<ConfigParameterData>.EmptyArray;
		}
		HandleConfigResults(output);
		var res = new List<ConfigParameterData>();
		var parser = new GitParser(output.Output);
		while(!parser.IsAtEndOfString)
		{
			var name  = parser.ReadStringUpTo(parser.FindNewLineOrEndOfString(), skip: 1);
			var value = parser.ReadStringUpTo(parser.FindNullOrEndOfString(), skip: 1);
			res.Add(parameters.ConfigFile != ConfigFile.Other
				? new ConfigParameterData(name, value, parameters.ConfigFile)
				: new ConfigParameterData(name, value, parameters.FileName!));
		}
		return res;
	}

	#endregion
}
