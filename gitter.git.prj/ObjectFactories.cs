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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	static class ObjectFactories
	{
		public static Branch CreateBranch(Repository repository, BranchData branchData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(branchData, "branchData");
			Verify.Argument.IsFalse(branchData.IsRemote, "branchData", "Cannot create remote branch.");

			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(branchData.SHA1);
			}
			var branch = new Branch(repository, branchData.Name, revision);
			if(branchData.IsCurrent) repository.Head.Pointer = branch;
			return branch;
		}

		public static void UpdateBranch(Branch branch, BranchData branchData)
		{
			Verify.Argument.IsNotNull(branch, "branch");
			Verify.Argument.IsNotNull(branchData, "branchData");
			Verify.Argument.IsFalse(branchData.IsRemote, "branchData", "Cannot update remote branch.");

			var repo = branch.Repository;
			if(branch.Revision.Hash != branchData.SHA1)
			{
				lock(repo.Revisions.SyncRoot)
				{
					branch.Pointer = repo.Revisions.GetOrCreateRevision(branchData.SHA1);
				}
			}
			if(branchData.IsCurrent)
			{
				repo.Head.Pointer = branch;
			}
		}

		public static RemoteBranch CreateRemoteBranch(Repository repository, BranchData branchData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(branchData, "branchData");
			Verify.Argument.IsTrue(branchData.IsRemote, "branchData", "Cannot create local branch.");

			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(branchData.SHA1);
			}
			return new RemoteBranch(repository, branchData.Name, revision);
		}

		public static void UpdateRemoteBranch(RemoteBranch remoteBranch, BranchData branchData)
		{
			Verify.Argument.IsNotNull(remoteBranch, "remoteBranch");
			Verify.Argument.IsNotNull(branchData, "branchData");
			Verify.Argument.IsTrue(branchData.IsRemote, "branchData", "Cannot update local branch.");

			if(remoteBranch.Revision.Hash != branchData.SHA1)
			{
				var revisionCache = remoteBranch.Repository.Revisions;
				lock(revisionCache.SyncRoot)
				{
					remoteBranch.Pointer = revisionCache.GetOrCreateRevision(branchData.SHA1);
				}
			}
		}

		public static RemoteBranch CreateRemoteBranch(Repository repository, RemoteBranchData branchData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(branchData, "branchData");

			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(branchData.SHA1);
			}
			return new RemoteBranch(repository, branchData.Name, revision);
		}

		public static void UpdateRemoteBranch(RemoteBranch remoteBranch, RemoteBranchData branchData)
		{
			Verify.Argument.IsNotNull(remoteBranch, "remoteBranch");
			Verify.Argument.IsNotNull(branchData, "branchData");

			if(remoteBranch.Revision.Hash != branchData.SHA1)
			{
				remoteBranch.Pointer = remoteBranch.Repository.Revisions.GetOrCreateRevision(branchData.SHA1);
			}
		}

		public static Tag CreateTag(Repository repository, TagData tagData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(tagData, "tagData");

			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(tagData.SHA1);
			}
			return new Tag(repository, tagData.Name, revision, tagData.TagType);
		}

		public static void UpdateTag(Tag tag, TagData tagData)
		{
			Verify.Argument.IsNotNull(tag, "tag");
			Verify.Argument.IsNotNull(tagData, "tagData");

			if(tag.Revision.Hash != tagData.SHA1)
			{
				var repo = tag.Repository;
				Revision revision;
				lock(repo.Revisions.SyncRoot)
				{
					revision = repo.Revisions.GetOrCreateRevision(tagData.SHA1);
				}
				tag.Pointer = revision;
			}
			tag.TagType = tagData.TagType;
		}

		public static Note CreateNote(Repository repository, NoteData noteData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(noteData, "noteData");

			return new Note(repository, noteData.Name, noteData.ObjectName, noteData.Message);
		}

		public static void UpdateNode(Note note, NoteData noteData)
		{
			Verify.Argument.IsNotNull(note, "note");
			Verify.Argument.IsNotNull(noteData, "noteData");

			note.Object = noteData.ObjectName;
			if(noteData.Message != null)
			{
				note.Message = noteData.Message;
			}
		}

		public static TreeDirectory CreateTreeDirectory(Repository repository, TreeDirectoryData treeDirectoryData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(treeDirectoryData, "treeDirectoryData");

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
			Verify.Argument.IsNotNull(treeDirectory, "treeDirectory");
			Verify.Argument.IsNotNull(treeDirectoryData, "treeDirectoryData");

		}

		public static ConfigParameter CreateConfigParameter(IConfigAccessor configAccessor, ConfigParameterData configParameterData)
		{
			Verify.Argument.IsNotNull(configAccessor, "configAccessor");
			Verify.Argument.IsNotNull(configParameterData, "configParameterData");

			switch(configParameterData.ConfigFile)
			{
				case ConfigFile.Repository:
					throw new ArgumentException("Config file cannot be 'Repository'.", "configParameterData");
				case ConfigFile.Other:
					return new ConfigParameter(
						configAccessor,
						configParameterData.SpecifiedFile,
						configParameterData.Name,
						configParameterData.Value);
				default:
					return new ConfigParameter(
						configAccessor,
						configParameterData.ConfigFile,
						configParameterData.Name,
						configParameterData.Value);
			}
		}

		public static void UpdateConfigParameter(ConfigParameter configParameter, ConfigParameterData configParameterData)
		{
			Verify.Argument.IsNotNull(configParameter, "configParameter");
			Verify.Argument.IsNotNull(configParameterData, "configParameterData");

			configParameter.SetValue(configParameterData.Value);
		}

		public static ConfigParameter CreateConfigParameter(Repository repository, ConfigParameterData configParameterData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(configParameterData, "configParameterData");

			return new ConfigParameter(
				repository,
				configParameterData.ConfigFile,
				configParameterData.Name,
				configParameterData.Value);
		}

		public static void UpdateReflogRecord(ReflogRecord reflogRecord, ReflogRecordData reflogRecordData)
		{
			Verify.Argument.IsNotNull(reflogRecord, "reflogRecord");
			Verify.Argument.IsNotNull(reflogRecordData, "reflogRecordData");

			reflogRecord.Index = reflogRecordData.Index;
			reflogRecord.Message = reflogRecordData.Message;
			Revision revision;
			lock(reflogRecord.Repository.Revisions.SyncRoot)
			{
				revision = reflogRecord.Repository.Revisions.GetOrCreateRevision(reflogRecordData.Revision.SHA1);
			}
			if(!revision.IsLoaded)
			{
				UpdateRevision(revision, reflogRecordData.Revision);
			}
			reflogRecord.Revision = revision;
		}

		public static ReflogRecord CreateReflogRecord(Reflog reflog, ReflogRecordData reflogRecordData)
		{
			Verify.Argument.IsNotNull(reflog, "reflog");
			Verify.Argument.IsNotNull(reflogRecordData, "reflogRecordData");

			var repository = reflog.Repository;
			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(reflogRecordData.Revision.SHA1);
			}
			if(!revision.IsLoaded)
			{
				UpdateRevision(revision, reflogRecordData.Revision);
			}
			return new ReflogRecord(repository, reflog, revision, reflogRecordData.Message, reflogRecordData.Index);
		}

		public static void UpdateStashedState(StashedState stashedState, StashedStateData stashedStateData)
		{
			Verify.Argument.IsNotNull(stashedState, "stashedState");
			Verify.Argument.IsNotNull(stashedStateData, "stashedStateData");

			stashedState.Index = stashedStateData.Index;
		}

		public static StashedState CreateStashedState(Repository repository, StashedStateData stashedStateData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(stashedStateData, "stashedStateData");

			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(stashedStateData.Revision.SHA1);
			}
			if(!revision.IsLoaded)
			{
				UpdateRevision(revision, stashedStateData.Revision);
			}
			return new StashedState(repository, stashedStateData.Index, revision);
		}

		public static void UpdateSubmodule(Submodule submodule, SubmoduleData submoduleData)
		{
			Verify.Argument.IsNotNull(submodule, "submodule");
			Verify.Argument.IsNotNull(submoduleData, "submoduleData");

			submodule.UpdateInfo(submoduleData.Path, submoduleData.Url);
		}

		public static Submodule CreateSubmodue(Repository repository, SubmoduleData submoduleData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(submoduleData, "submoduleData");

			return new Submodule(repository, submoduleData.Name, submoduleData.Path, submoduleData.Url);
		}

		public static void UpdateRevision(Revision revision, RevisionData revisionData, bool updateParents)
		{
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsNotNull(revisionData, "revisionData");

			var fields = revisionData.Fields;
			if(fields != RevisionField.SHA1)
			{
				var repository = revision.Repository;
				if((fields & RevisionField.Subject) == RevisionField.Subject)
				{
					revision.Subject = revisionData.Subject;
				}
				if((fields & RevisionField.Body) == RevisionField.Body)
				{
					revision.Body = revisionData.Body;
				}
				if((fields & RevisionField.TreeHash) == RevisionField.TreeHash)
				{
					revision.TreeHash = revisionData.TreeHash;
				}
				if(updateParents && ((fields & RevisionField.Parents) == RevisionField.Parents))
				{
					HashSet<Revision> hset = null;
					if(revision.Parents.Count != 0)
					{
						hset = new HashSet<Revision>(revision.Parents);
					}
					int id = 0;
					foreach(var info in revisionData.Parents)
					{
						bool found = false;
						for(int i = id; i < revision.Parents.Count; ++i)
						{
							if(revision.Parents[i].Hash == info.SHA1)
							{
								if(i != id)
								{
									var temp = revision.Parents[i];
									revision.Parents[i] = revision.Parents[id];
									revision.Parents[id] = temp;
								}
								if(hset != null)
								{
									hset.Remove(revision.Parents[id]);
								}
								found = true;
								break;
							}
						}
						if(!found)
						{
							var obj = CreateRevision(repository, info);
							revision.Parents.InsertInternal(id, obj);
						}
						++id;
					}
					if(hset != null && hset.Count != 0)
					{
						foreach(var obj in hset)
						{
							revision.Parents.RemoveInternal(obj);
						}
					}
				}
				if((fields & RevisionField.CommitDate) == RevisionField.CommitDate)
				{
					revision.CommitDate = revisionData.CommitDate;
				}
				if((fields & (RevisionField.CommitterName | RevisionField.CommitterEmail)) ==
					(RevisionField.CommitterName | RevisionField.CommitterEmail))
				{
					revision.Committer = repository.Users.GetOrCreateUser(
						revisionData.CommitterName, revisionData.CommitterEmail);
				}
				if((fields & RevisionField.AuthorDate) == RevisionField.AuthorDate)
				{
					revision.AuthorDate = revisionData.AuthorDate;
				}
				if((fields & (RevisionField.AuthorName | RevisionField.AuthorEmail)) ==
					(RevisionField.AuthorName | RevisionField.AuthorEmail))
				{
					revision.Author = repository.Users.GetOrCreateUser(
						revisionData.AuthorName, revisionData.AuthorEmail);
				}

				revision.IsLoaded = true;
			}
		}

		public static void UpdateRevision(Revision obj, RevisionData revisionData)
		{
			UpdateRevision(obj, revisionData, true);
		}

		public static Revision CreateRevision(Repository repository, RevisionData revisionData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(revisionData, "revisionData");

			var revisions = repository.Revisions;
			var revision  = revisions.GetOrCreateRevision(revisionData.SHA1);
			var fields    = revisionData.Fields;
			if(!revision.IsLoaded && (fields != RevisionField.SHA1))
			{
				if((fields & RevisionField.Subject) == RevisionField.Subject)
				{
					revision.Subject = revisionData.Subject;
				}
				if((fields & RevisionField.Body) == RevisionField.Body)
				{
					revision.Body = revisionData.Body;
				}
				if((fields & RevisionField.TreeHash) == RevisionField.TreeHash)
				{
					revision.TreeHash = revisionData.TreeHash;
				}
				if((fields & RevisionField.Parents) == RevisionField.Parents)
				{
					foreach(var parentData in revisionData.Parents)
					{
						var parent = revisions.GetOrCreateRevision(parentData.SHA1);
						revision.Parents.AddInternal(parent);
					}
				}
				if((fields & RevisionField.CommitDate) == RevisionField.CommitDate)
				{
					revision.CommitDate = revisionData.CommitDate;
				}
				if((fields & (RevisionField.CommitterName | RevisionField.CommitterEmail)) ==
					(RevisionField.CommitterName | RevisionField.CommitterEmail))
				{
					revision.Committer = repository.Users.GetOrCreateUser(revisionData.CommitterName, revisionData.CommitterEmail);
				}
				if((fields & RevisionField.AuthorDate) == RevisionField.AuthorDate)
				{
					revision.AuthorDate = revisionData.AuthorDate;
				}
				if((fields & (RevisionField.AuthorName | RevisionField.AuthorEmail)) ==
					(RevisionField.AuthorName | RevisionField.AuthorEmail))
				{
					revision.Author = repository.Users.GetOrCreateUser(revisionData.AuthorName, revisionData.AuthorEmail);
				}
				revision.IsLoaded = true;
			}
			return revision;
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
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(treeFileData, "treeFileData");

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
			Verify.Argument.IsNotNull(treeFile, "treeFile");
			Verify.Argument.IsNotNull(treeFileData, "treeFileData");

			treeFile.ConflictType = treeFileData.ConflictType;
			treeFile.Status = treeFileData.FileStatus;
			treeFile.StagedStatus = treeFileData.StagedStatus;
		}

		public static User CreateUser(Repository repository, UserData userData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(userData, "userData");

			return new User(repository, userData.UserName, userData.Email, userData.Commits);
		}

		public static void UpdateUser(User user, UserData userData)
		{
			Verify.Argument.IsNotNull(user, "user");
			Verify.Argument.IsNotNull(userData, "userData");

			user.Commits = userData.Commits;
		}

		public static Remote CreateRemote(Repository repository, RemoteData remoteData)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(remoteData, "remoteData");

			return new Remote(repository, remoteData.Name, remoteData.FetchUrl, remoteData.PushUrl);
		}

		public static void UpdateRemote(Remote remote, RemoteData remoteData)
		{
			Verify.Argument.IsNotNull(remote, "remote");
			Verify.Argument.IsNotNull(remoteData, "remoteData");

			remote.SetPushUrl(remoteData.PushUrl);
			remote.SetFetchUrl(remoteData.FetchUrl);
		}
	}
}
