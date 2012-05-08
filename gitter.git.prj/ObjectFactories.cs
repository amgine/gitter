namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Git.AccessLayer;

	static class ObjectFactories
	{
		public static Branch CreateBranch(Repository repository, BranchData branchData)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(branchData == null) throw new ArgumentNullException("branchData");
			if(branchData.IsRemote) throw new ArgumentException();

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
			if(branch == null) throw new ArgumentNullException("branch");
			if(branchData == null) throw new ArgumentNullException("branchData");
			if(branchData.IsRemote) throw new ArgumentException();

			var repo = branch.Repository;
			if(branch.Revision.Name != branchData.SHA1)
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
			if(repository == null) throw new ArgumentNullException("repository");
			if(branchData == null) throw new ArgumentNullException("branchData");
			if(!branchData.IsRemote) throw new ArgumentException();

			var repo = (Repository)repository;
			Revision revision;
			lock(repo.Revisions.SyncRoot)
			{
				revision = repo.Revisions.GetOrCreateRevision(branchData.SHA1);
			}
			return new RemoteBranch(repo, branchData.Name, revision);
		}

		public static void UpdateRemoteBranch(RemoteBranch remoteBranch, BranchData branchData)
		{
			if(remoteBranch == null) throw new ArgumentNullException("remoteBranch");
			if(branchData == null) throw new ArgumentNullException("branchData");
			if(!branchData.IsRemote) throw new ArgumentException();

			var repo = remoteBranch.Repository;
			if(remoteBranch.Revision.Name != branchData.SHA1)
			{
				lock(repo.Revisions.SyncRoot)
				{
					remoteBranch.Pointer = repo.Revisions.GetOrCreateRevision(branchData.SHA1);
				}
			}
		}

		public static RemoteBranch CreateRemoteBranch(Repository repository, RemoteBranchData branchData)
		{
			if(repository == null) throw new ArgumentNullException("repository");

			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(branchData.SHA1);
			}
			return new RemoteBranch(repository, branchData.Name, revision);
		}

		public static void UpdateRemoteBranch(RemoteBranch remoteBranch, RemoteBranchData branchData)
		{
			if(remoteBranch == null) throw new ArgumentNullException("remoteBranch");
			if(branchData == null) throw new ArgumentNullException("branchData");

			var repository = remoteBranch.Repository;
			if(remoteBranch.Revision.Name != branchData.SHA1)
			{
				remoteBranch.Pointer = repository.Revisions.GetOrCreateRevision(branchData.SHA1);
			}
		}

		public static Tag CreateTag(Repository repository, TagData tagData)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(tagData == null) throw new ArgumentNullException("tagData");

			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(tagData.SHA1);
			}
			return new Tag(repository, tagData.Name, revision, tagData.TagType);
		}

		public static void UpdateTag(Tag tag, TagData tagData)
		{
			if(tag == null) throw new ArgumentNullException("tag");
			if(tagData == null) throw new ArgumentNullException("tagData");

			if(tag.Revision.Name != tagData.SHA1)
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
			if(repository == null) throw new ArgumentNullException("repository");
			if(noteData == null) throw new ArgumentNullException("noteData");

			return new Note(repository, noteData.Name, noteData.ObjectName, noteData.Message);
		}

		public static void UpdateNode(Note note, NoteData noteData)
		{
			if(note == null) throw new ArgumentNullException("note");
			if(noteData == null) throw new ArgumentNullException("noteData");

			note.Object = noteData.ObjectName;
			if(noteData.Message != null)
			{
				note.Message = noteData.Message;
			}
		}

		public static TreeDirectory CreateTreeDirectory(Repository repository, TreeDirectoryData treeDirectoryData)
		{
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
			if(treeDirectory == null) throw new ArgumentNullException("treeDirectory");
			if(treeDirectoryData == null) throw new ArgumentNullException("treeDirectoryData");

		}

		public static ConfigParameter CreateConfigParameter(IConfigAccessor configAccessor, ConfigParameterData configParameterData)
		{
			if(configAccessor == null) throw new ArgumentNullException("configAccessor");
			if(configParameterData == null) throw new ArgumentNullException("configParameterData");

			switch(configParameterData.ConfigFile)
			{
				case ConfigFile.Repository:
					throw new ArgumentException();
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
			configParameter.SetValue(configParameterData.Value);
		}

		public static ConfigParameter CreateConfigParameter(Repository repository, ConfigParameterData configParameterData)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(configParameterData == null) throw new ArgumentNullException("configParameterData");

			return new ConfigParameter(
				repository,
				configParameterData.ConfigFile,
				configParameterData.Name,
				configParameterData.Value);
		}

		public static void UpdateReflogRecord(ReflogRecord reflogRecord, ReflogRecordData reflogRecordData)
		{
			if(reflogRecord == null) throw new ArgumentNullException("reflogRecord");
			if(reflogRecordData == null) throw new ArgumentNullException("reflogRecordData");

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

		public static ReflogRecord CreateReflogRecord(Repository repository, Reflog reflog, ReflogRecordData reflogRecordData)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(reflog == null) throw new ArgumentNullException("reflog");
			if(reflogRecordData == null) throw new ArgumentNullException("reflogRecordData");

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
			if(stashedState == null) throw new ArgumentNullException("stashedState");
			if(stashedStateData == null) throw new ArgumentNullException("stashedStateData");

			stashedState.Index = stashedStateData.Index;
		}

		public static StashedState CreateStashedState(Repository repository, StashedStateData stashedStateData)
		{
			if(repository == null) throw new ArgumentNullException("repository");

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
			if(submodule == null) throw new ArgumentNullException("submodule");
			if(submoduleData == null) throw new ArgumentNullException("submoduleData");

			submodule.UpdateInfo(submoduleData.Path, submoduleData.Url);
		}

		public static Submodule CreateSubmodue(Repository repository, SubmoduleData submoduleData)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(submoduleData == null) throw new ArgumentNullException("submoduleData");

			return new Submodule(repository, submoduleData.Name, submoduleData.Path, submoduleData.Url);
		}

		public static void UpdateRevision(Revision revision, RevisionData revisionData, bool updateParents)
		{
			if(revision == null) throw new ArgumentNullException("revision");
			if(revisionData == null) throw new ArgumentNullException("revisionData");

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
							if(revision.Parents[i].SHA1 == info.SHA1)
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
			if(repository == null) throw new ArgumentNullException("repository");
			if(revisionData == null) throw new ArgumentNullException("revisionData");

			var revisions = repository.Revisions;
			var obj = revisions.GetOrCreateRevision(revisionData.SHA1);
			var fields = revisionData.Fields;
			if(!obj.IsLoaded && (fields != RevisionField.SHA1))
			{
				if((fields & RevisionField.Subject) == RevisionField.Subject)
				{
					obj.Subject = revisionData.Subject;
				}
				if((fields & RevisionField.Body) == RevisionField.Body)
				{
					obj.Body = revisionData.Body;
				}
				if((fields & RevisionField.TreeHash) == RevisionField.TreeHash)
				{
					obj.TreeHash = revisionData.TreeHash;
				}
				if((fields & RevisionField.Parents) == RevisionField.Parents)
				{
					foreach(var parentData in revisionData.Parents)
					{
						var parent = revisions.GetOrCreateRevision(parentData.SHA1);
						obj.Parents.AddInternal(parent);
					}
				}
				if((fields & RevisionField.CommitDate) == RevisionField.CommitDate)
				{
					obj.CommitDate = revisionData.CommitDate;
				}
				if((fields & (RevisionField.CommitterName | RevisionField.CommitterEmail)) ==
					(RevisionField.CommitterName | RevisionField.CommitterEmail))
				{
					obj.Committer = repository.Users.GetOrCreateUser(revisionData.CommitterName, revisionData.CommitterEmail);
				}
				if((fields & RevisionField.AuthorDate) == RevisionField.AuthorDate)
				{
					obj.AuthorDate = revisionData.AuthorDate;
				}
				if((fields & (RevisionField.AuthorName | RevisionField.AuthorEmail)) ==
					(RevisionField.AuthorName | RevisionField.AuthorEmail))
				{
					obj.Author = repository.Users.GetOrCreateUser(revisionData.AuthorName, revisionData.AuthorEmail);
				}
				obj.IsLoaded = true;
			}
			return obj;
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
			if(repository == null) throw new ArgumentNullException("repository");
			if(treeFileData == null) throw new ArgumentNullException("treeFileData");

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
			if(treeFile == null) throw new ArgumentNullException("treeFile");
			if(treeFileData == null) throw new ArgumentNullException("treeFileData");

			treeFile.ConflictType = treeFileData.ConflictType;
			treeFile.Status = treeFileData.FileStatus;
			treeFile.StagedStatus = treeFileData.StagedStatus;
		}

		public static User CreateUser(Repository repository, UserData userData)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(userData == null) throw new ArgumentNullException("userData");

			return new User(repository, userData.UserName, userData.Email, userData.Commits);
		}

		public static void UpdateUser(User user, UserData userData)
		{
			if(user == null) throw new ArgumentNullException("user");
			if(userData == null) throw new ArgumentNullException("userData");

			user.Commits = userData.Commits;
		}

		public static Remote CreateRemote(Repository repository, RemoteData remoteData)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(remoteData == null) throw new ArgumentNullException("remoteData");

			return new Remote(repository, remoteData.Name, remoteData.FetchUrl, remoteData.PushUrl);
		}

		public static void UpdateRemote(Remote remote, RemoteData remoteData)
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remoteData == null) throw new ArgumentNullException("remoteData");

			remote.SetPushUrl(remoteData.PushUrl);
			remote.SetFetchUrl(remoteData.FetchUrl);
		}
	}
}
