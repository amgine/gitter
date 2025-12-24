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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using gitter.Framework;
using gitter.Git.AccessLayer;

static class ObjectFactories
{
	static bool ReferenceTargetChanged<T>(T reference, Sha1Hash hash)
		where T : Reference
	{
		var revision = reference.Revision;
		return revision is null || revision.Hash != hash;
	}

	static bool UpdateReferenceTarget<T>(T reference, Sha1Hash hash)
		where T : Reference
	{
		if(!ReferenceTargetChanged(reference, hash)) return false;
		var sitory = reference.Repository;
		lock(sitory.Revisions.SyncRoot)
		{
			reference.Pointer = sitory.Revisions.GetOrCreateRevision(hash);
		}
		return true;
	}

	public static Branch CreateBranch(Repository repository, BranchData branchData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(branchData);
		Verify.Argument.IsFalse(branchData.IsRemote, nameof(branchData), "Cannot create remote branch.");

		Revision revision;
		lock(repository.Revisions.SyncRoot)
		{
			revision = repository.Revisions.GetOrCreateRevision(branchData.Hash);
		}
		var branch = new Branch(repository, branchData.Name, revision);
		if(branchData.IsCurrent) repository.Head.Pointer = branch;
		return branch;
	}

	public static void UpdateBranch(Branch branch, BranchData branchData)
	{
		Verify.Argument.IsNotNull(branch);
		Verify.Argument.IsNotNull(branchData);
		Verify.Argument.IsFalse(branchData.IsRemote, nameof(branchData), "Cannot update remote branch.");

		UpdateReferenceTarget(branch, branchData.Hash);
		if(branchData.IsCurrent)
		{
			branch.Repository.Head.Pointer = branch;
		}
	}

	public static RemoteBranch CreateRemoteBranch(Repository repository, BranchData branchData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(branchData);
		Verify.Argument.IsTrue(branchData.IsRemote, nameof(branchData), "Cannot create local branch.");

		Revision revision;
		lock(repository.Revisions.SyncRoot)
		{
			revision = repository.Revisions.GetOrCreateRevision(branchData.Hash);
		}
		return new RemoteBranch(repository, branchData.Name, revision);
	}

	public static void UpdateRemoteBranch(RemoteBranch remoteBranch, BranchData branchData)
	{
		Verify.Argument.IsNotNull(remoteBranch);
		Verify.Argument.IsNotNull(branchData);
		Verify.Argument.IsTrue(branchData.IsRemote, nameof(branchData), "Cannot update local branch.");

		UpdateReferenceTarget(remoteBranch, branchData.Hash);
	}

	public static RemoteBranch CreateRemoteBranch(Repository repository, RemoteBranchData branchData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(branchData);

		Revision revision;
		lock(repository.Revisions.SyncRoot)
		{
			revision = repository.Revisions.GetOrCreateRevision(branchData.Hash);
		}
		return new RemoteBranch(repository, branchData.Name, revision);
	}

	public static void UpdateRemoteBranch(RemoteBranch remoteBranch, RemoteBranchData branchData)
	{
		Verify.Argument.IsNotNull(remoteBranch);
		Verify.Argument.IsNotNull(branchData);

		UpdateReferenceTarget(remoteBranch, branchData.Hash);
	}

	public static Tag CreateTag(Repository repository, TagData tagData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(tagData);

		Revision revision;
		lock(repository.Revisions.SyncRoot)
		{
			revision = repository.Revisions.GetOrCreateRevision(tagData.Hash);
		}
		return new Tag(repository, tagData.Name, revision, tagData.TagType);
	}

	public static void UpdateTag(Tag tag, TagData tagData)
	{
		Verify.Argument.IsNotNull(tag);
		Verify.Argument.IsNotNull(tagData);

		UpdateReferenceTarget(tag, tagData.Hash);
		tag.TagType = tagData.TagType;
	}

	public static Note CreateNote(Repository repository, NoteData noteData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(noteData);

		return new Note(repository, noteData.Name, noteData.ObjectName, noteData.Message);
	}

	public static void UpdateNode(Note note, NoteData noteData)
	{
		Verify.Argument.IsNotNull(note);
		Verify.Argument.IsNotNull(noteData);

		note.Object = noteData.ObjectName;
		if(noteData.Message is not null)
		{
			note.Message = noteData.Message;
		}
	}

	public static TreeDirectory CreateTreeDirectory(Repository repository, TreeDirectoryData treeDirectoryData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(treeDirectoryData);

		var directory = new TreeDirectory(repository, treeDirectoryData.Name, null, treeDirectoryData.ShortName)
		{
			StagedStatus = treeDirectoryData.StagedStatus,
		};
		foreach(var subDirectory in treeDirectoryData.Directories)
		{
			directory.AddDirectory(CreateTreeDirectory(repository, subDirectory));
		}
		foreach(var file in treeDirectoryData.Files)
		{
			directory.AddFile(ObjectFactories.CreateTreeFile(repository, file));
		}
		return directory;
	}

	public static void UpdateTreeDirectory(TreeDirectory treeDirectory, TreeDirectoryData treeDirectoryData)
	{
		Verify.Argument.IsNotNull(treeDirectory);
		Verify.Argument.IsNotNull(treeDirectoryData);

	}

	public static ConfigParameter CreateConfigParameter(IConfigAccessor configAccessor, ConfigParameterData configParameterData)
	{
		Verify.Argument.IsNotNull(configAccessor);
		Verify.Argument.IsNotNull(configParameterData);

		return configParameterData.ConfigFile switch
		{
			ConfigFile.Repository => throw new ArgumentException(
				"Config file cannot be 'Repository'.", nameof(configParameterData)),
			ConfigFile.Other => new ConfigParameter(
				configAccessor,
				configParameterData.SpecifiedFile!,
				configParameterData.Name,
				configParameterData.Value),
			_ => new ConfigParameter(
				configAccessor,
				configParameterData.ConfigFile,
				configParameterData.Name,
				configParameterData.Value),
		};
	}

	public static void UpdateConfigParameter(ConfigParameter configParameter, ConfigParameterData configParameterData)
	{
		Verify.Argument.IsNotNull(configParameter);
		Verify.Argument.IsNotNull(configParameterData);

		configParameter.SetValue(configParameterData.Value);
	}

	public static ConfigParameter CreateConfigParameter(Repository repository, ConfigParameterData configParameterData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(configParameterData);

		return new ConfigParameter(
			repository,
			configParameterData.ConfigFile,
			configParameterData.Name,
			configParameterData.Value);
	}

	public static void UpdateReflogRecord(ReflogRecord reflogRecord, ReflogRecordData reflogRecordData)
	{
		Verify.Argument.IsNotNull(reflogRecord);
		Verify.Argument.IsNotNull(reflogRecordData);

		reflogRecord.Index = reflogRecordData.Index;
		reflogRecord.Message = reflogRecordData.Message;
		Revision revision;
		lock(reflogRecord.Repository.Revisions.SyncRoot)
		{
			revision = reflogRecord.Repository.Revisions.GetOrCreateRevision(reflogRecordData.Revision.CommitHash);
		}
		if(!revision.IsLoaded)
		{
			UpdateRevision(revision, reflogRecordData.Revision);
		}
		reflogRecord.Revision = revision;
	}

	public static ReflogRecord CreateReflogRecord(Reflog reflog, ReflogRecordData reflogRecordData)
	{
		Verify.Argument.IsNotNull(reflog);
		Verify.Argument.IsNotNull(reflogRecordData);

		var repository = reflog.Repository;
		Revision revision;
		lock(repository.Revisions.SyncRoot)
		{
			revision = repository.Revisions.GetOrCreateRevision(reflogRecordData.Revision.CommitHash);
		}
		if(!revision.IsLoaded)
		{
			UpdateRevision(revision, reflogRecordData.Revision);
		}
		return new ReflogRecord(repository, reflog, revision, reflogRecordData.Message, reflogRecordData.Index);
	}

	public static void UpdateStashedState(StashedState stashedState, StashedStateData stashedStateData)
	{
		Verify.Argument.IsNotNull(stashedState);
		Verify.Argument.IsNotNull(stashedStateData);

		stashedState.Index = stashedStateData.Index;
	}

	public static StashedState CreateStashedState(Repository repository, StashedStateData stashedStateData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(stashedStateData);

		Revision revision;
		lock(repository.Revisions.SyncRoot)
		{
			revision = repository.Revisions.GetOrCreateRevision(stashedStateData.Revision.CommitHash);
		}
		if(!revision.IsLoaded)
		{
			UpdateRevision(revision, stashedStateData.Revision);
		}
		return new StashedState(repository, stashedStateData.Index, revision);
	}

	public static void UpdateSubmodule(Submodule submodule, SubmoduleData submoduleData)
	{
		Verify.Argument.IsNotNull(submodule);
		Verify.Argument.IsNotNull(submoduleData);

		submodule.UpdateInfo(submoduleData.Path, submoduleData.Url);
	}

	public static Submodule CreateSubmodule(Repository repository, SubmoduleData submoduleData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(submoduleData);

		return new Submodule(repository, submoduleData.Name, submoduleData.Path, submoduleData.Url);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool HasFlag(RevisionField flags, RevisionField flag)
		=> (flags & flag) == flag;

	private static Many<Revision> CreateAndLoadRevisions(Repository repository, Many<RevisionData> data)
	{
		if(data.Count == 0) return Many<Revision>.None;
		if(data.Count == 1) return CreateRevision(repository, data.First());
		var revisions = new Revision[data.Count];
		for(int i = 0; i < revisions.Length; ++i)
		{
			revisions[i] = CreateRevision(repository, data[i]);
		}
		return revisions;
	}

	private static Many<Revision> GetOrCreateRevisions(Repository repository, Many<RevisionData> data)
	{
		if(data.Count == 0) return Many<Revision>.None;
		if(data.Count == 1) return GetOrCreateRevision(repository, data.First());
		var revisions = new Revision[data.Count];
		for(int i = 0; i < revisions.Length; ++i)
		{
			revisions[i] = GetOrCreateRevision(repository, data[i]);
		}
		return revisions;
	}

	private static void LoadRevision(Revision revision, RevisionData revisionData)
	{
		var fields = revisionData.Fields;
		if(HasFlag(fields, RevisionField.Subject))
		{
			revision.Subject = revisionData.Subject;
		}
		if(HasFlag(fields, RevisionField.Body))
		{
			revision.Body = revisionData.Body;
		}
		if(HasFlag(fields, RevisionField.TreeHash))
		{
			revision.TreeHash = revisionData.TreeHash;
		}
		if(HasFlag(fields, RevisionField.Parents))
		{
			revision.Parents = GetOrCreateRevisions(revision.Repository, revisionData.Parents);
		}
		if(HasFlag(fields, RevisionField.CommitDate))
		{
			revision.CommitDate = revisionData.CommitDate;
		}
		if(HasFlag(fields, RevisionField.CommitterName | RevisionField.CommitterEmail))
		{
			revision.Committer = revision.Repository.Users.GetOrCreateUser(revisionData.CommitterName, revisionData.CommitterEmail);
		}
		if(HasFlag(fields, RevisionField.AuthorDate))
		{
			revision.AuthorDate = revisionData.AuthorDate;
		}
		if(HasFlag(fields, RevisionField.AuthorName | RevisionField.AuthorEmail))
		{
			revision.Author = revision.Repository.Users.GetOrCreateUser(revisionData.AuthorName, revisionData.AuthorEmail);
		}
	}

	public static void UpdateRevision(Revision revision, RevisionData revisionData)
	{
		Verify.Argument.IsNotNull(revision);
		Verify.Argument.IsNotNull(revisionData);

		if(revisionData.Fields != RevisionField.CommitHash)
		{
			revision.IsLoaded = true;
			LoadRevision(revision, revisionData);
		}
	}

	public static Revision CreateRevision(Repository repository, RevisionData revisionData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(revisionData);

		var revisions = repository.Revisions;
		lock(revisions.SyncRoot)
		{
			var revision = revisions.GetOrCreateRevision(revisionData.CommitHash);
			var fields = revisionData.Fields;
			if(!revision.IsLoaded && (fields != RevisionField.CommitHash))
			{
				revision.IsLoaded = true;
				LoadRevision(revision, revisionData);
			}
			return revision;
		}
	}

	public static Revision GetOrCreateRevision(Repository repository, RevisionData revisionData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(revisionData);

		var revisions = repository.Revisions;
		lock(revisions.SyncRoot)
		{
			return revisions.GetOrCreateRevision(revisionData.CommitHash);
		}
	}

	private static string GetShortName(string name)
	{
		bool isSubmodule = false;
		int i = name.Length;
		while(i > 0)
		{
			i = name.LastIndexOf('/', i - 1);
			if(i == name.Length - 1)
			{
				isSubmodule = true;
				continue;
			}
			var s = i + 1;
			var l = name.Length - s;
			if(isSubmodule) --l;
			return name.Substring(i + 1, l);
		}
		return string.Empty;
	}

	public static TreeFile CreateTreeFile(Repository repository, TreeFileData treeFileData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(treeFileData);

		var shortName = treeFileData.ShortName.Length == 0 ?
			GetShortName(treeFileData.Name) : treeFileData.ShortName;
		return new TreeFile(repository, treeFileData.Name, null, treeFileData.FileStatus, shortName)
		{
			ConflictType = treeFileData.ConflictType,
			StagedStatus = treeFileData.StagedStatus,
		};
	}

	public static void UpdateTreeFile(TreeFile treeFile, TreeFileData treeFileData)
	{
		Verify.Argument.IsNotNull(treeFile);
		Verify.Argument.IsNotNull(treeFileData);

		treeFile.ConflictType = treeFileData.ConflictType;
		treeFile.Status       = treeFileData.FileStatus;
		treeFile.StagedStatus = treeFileData.StagedStatus;
	}

	public static User CreateUser(Repository repository, UserData userData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(userData);

		return new User(repository, userData.Name, userData.Email, userData.Commits);
	}

	public static void UpdateUser(User user, UserData userData)
	{
		Verify.Argument.IsNotNull(user);
		Verify.Argument.IsNotNull(userData);

		user.Commits = userData.Commits;
	}

	public static Remote CreateRemote(Repository repository, RemoteData remoteData)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(remoteData);

		return new Remote(repository, remoteData.Name, remoteData.FetchUrl, remoteData.PushUrl);
	}

	public static void UpdateRemote(Remote remote, RemoteData remoteData)
	{
		Verify.Argument.IsNotNull(remote);
		Verify.Argument.IsNotNull(remoteData);

		remote.SetPushUrl(remoteData.PushUrl);
		remote.SetFetchUrl(remoteData.FetchUrl);
	}
}
