namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class RemoteReferencesCollection
	{
		#region Events

		public event EventHandler<RemoteReferenceEventArgs> TagDeleted;
		public event EventHandler<RemoteReferenceEventArgs> BranchDeleted;

		private void InvokeTagDeleted(RemoteRepositoryTag tag)
		{
			var handler = TagDeleted;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(tag));
		}

		private void InvokeBranchDeleted(RemoteRepositoryBranch branch)
		{
			var handler = BranchDeleted;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(branch));
		}

		public event EventHandler<RemoteReferenceEventArgs> TagCreated;
		public event EventHandler<RemoteReferenceEventArgs> BranchCreated;

		private void InvokeTagCreated(RemoteRepositoryTag tag)
		{
			var handler = TagCreated;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(tag));
		}

		private void InvokeBranchCreated(RemoteRepositoryBranch branch)
		{
			var handler = BranchCreated;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(branch));
		}

		#endregion

		#region Data

		private readonly Repository _repository;
		private readonly Remote _remote;
		private readonly Dictionary<string, RemoteRepositoryBranch> _remoteBranches;
		private readonly Dictionary<string, RemoteRepositoryTag> _remoteTags;

		#endregion

		#region .ctor

		internal RemoteReferencesCollection(Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");

			_remote = remote;
			_repository = remote.Repository;
			_remoteBranches = new Dictionary<string, RemoteRepositoryBranch>();
			_remoteTags = new Dictionary<string, RemoteRepositoryTag>();
		}

		#endregion

		internal void RemoveTag(RemoteRepositoryTag tag)
		{
			Verify.Argument.IsNotNull(tag, "tag");
			Verify.Argument.IsFalse(tag.IsDeleted, "tag",
				Resources.ExcSuppliedObjectIsDeleted.UseAsFormat("tag"));

			_remote.Repository.Accessor.RemoveRemoteReferences(
				new RemoveRemoteReferencesParameters(_remote.Name, tag.FullName));

			_remoteTags.Remove(tag.Name);
			tag.MarkAsDeleted();
			InvokeTagDeleted(tag);
		}

		internal void RemoveBranch(RemoteRepositoryBranch branch)
		{
			Verify.Argument.IsNotNull(branch, "branch");
			Verify.Argument.IsFalse(branch.IsDeleted, "branch",
				Resources.ExcSuppliedObjectIsDeleted.UseAsFormat("branch"));

			_remote.Repository.Accessor.RemoveRemoteReferences(
				new RemoveRemoteReferencesParameters(_remote.Name, branch.FullName));

			_remoteBranches.Remove(branch.Name);
			branch.MarkAsDeleted();
			InvokeBranchDeleted(branch);
		}

		public IEnumerable<RemoteRepositoryBranch> Branches
		{
			get { return _remoteBranches.Values; }
		}

		public int BranchCount
		{
			get { return _remoteBranches.Count; }
		}

		public IEnumerable<RemoteRepositoryTag> Tags
		{
			get { return _remoteTags.Values; }
		}

		public int TagCount
		{
			get { return _remoteTags.Count; }
		}

		public Remote Remote
		{
			get { return _remote; }
		}

		public void Refresh()
		{
			var refs = _repository.Accessor.QueryRemoteReferences(
				new QueryRemoteReferencesParameters(_remote.Name, true, true));
			var branches = new Dictionary<string, RemoteReferenceData>(refs.Count);
			var tags = new Dictionary<string, RemoteReferenceData>(refs.Count);
			foreach(var r in refs)
			{
				switch(r.ReferenceType)
				{
					case ReferenceType.LocalBranch:
						branches.Add(r.Name.Substring(GitConstants.LocalBranchPrefix.Length), r);
						break;
					case ReferenceType.Tag:
						tags.Add(r.Name.Substring(GitConstants.TagPrefix.Length), r);
						break;
				}
			}

			List<RemoteRepositoryBranch> deletedBranches;
			if(_remoteBranches.Count != 0)
			{
				deletedBranches = new List<RemoteRepositoryBranch>(_remoteBranches.Count);
				if(branches.Count == 0)
				{
					deletedBranches.AddRange(_remoteBranches.Values);
				}
				else
				{
					foreach(var b in _remoteBranches)
					{
						if(!branches.ContainsKey(b.Key))
						{
							deletedBranches.Add(b.Value);
						}
						else
						{
							branches.Remove(b.Key);
						}
					}
				}
			}
			else
			{
				deletedBranches = null;
				foreach(var b in _remoteBranches)
				{
					branches.Remove(b.Key);
				}
			}
			if(branches.Count != 0)
			{
				foreach(var b in branches)
				{
					var branch = new RemoteRepositoryBranch(this, b.Key, b.Value.Hash);
					_remoteBranches.Add(branch.Name, branch);
					InvokeBranchCreated(branch);
				}
			}
			if(deletedBranches != null && deletedBranches.Count != 0)
			{
				foreach(var b in deletedBranches)
				{
					_remoteBranches.Remove(b.Name);
					b.MarkAsDeleted();
					InvokeBranchDeleted(b);
				}
			}

			List<RemoteRepositoryTag> deletedTags;
			if(_remoteTags.Count != 0)
			{
				deletedTags = new List<RemoteRepositoryTag>(_remoteTags.Count);
				if(tags.Count == 0)
				{
					deletedTags.AddRange(_remoteTags.Values);
				}
				else
				{
					foreach(var t in _remoteTags)
					{
						if(!tags.ContainsKey(t.Key))
						{
							deletedTags.Add(t.Value);
						}
						else
						{
							tags.Remove(t.Key);
						}
					}
				}
			}
			else
			{
				deletedTags = null;
				foreach(var t in _remoteTags)
				{
					if(tags.ContainsKey(t.Key))
					{
						tags.Remove(t.Key);
					}
				}
			}
			if(tags.Count != 0)
			{
				foreach(var t in tags)
				{
					var tag = new RemoteRepositoryTag(this, t.Key, t.Value.TagType, t.Value.Hash);
					_remoteTags.Add(tag.Name, tag);
					InvokeTagCreated(tag);
				}
			}
			if(deletedTags != null && deletedTags.Count != 0)
			{
				foreach(var t in deletedTags)
				{
					_remoteTags.Remove(t.Name);
					t.MarkAsDeleted();
					InvokeTagDeleted(t);
				}
			}
		}
	}
}
