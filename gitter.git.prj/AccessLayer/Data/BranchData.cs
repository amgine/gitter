namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>Branch description.</summary>
	public sealed class BranchData : IObjectData<Branch>, IObjectData<RemoteBranch>, INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _sha1;
		private readonly bool _fake;
		private readonly bool _remote;
		private readonly bool _current;

		#endregion

		#region .ctor

		public BranchData(string name, string sha1, bool fake, bool remote, bool current)
		{
			if(name == null) throw new ArgumentNullException("name");
			if(name.Length == 0) throw new ArgumentException("name");
			if(sha1 == null) throw new ArgumentNullException("sha1");
			if(sha1.Length != 40) throw new ArgumentException("sha1");
			_name = name;
			_sha1 = sha1;
			_fake = fake;
			_remote = remote;
			_current = current;
		}

		public BranchData(string name, string sha1, bool remote, bool current)
			: this(name, sha1, false, remote, current)
		{
		}

		#endregion

		#region Properties

		/// <summary>Branche's name (short format, excluding refs/heads/ or /refs/%remote%/).</summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>SHA1 of commit, which is pointed by branch.</summary>
		public string SHA1
		{
			get { return _sha1; }
		}

		/// <summary>It's not actually a branch, just a representation of detached HEAD.</summary>
		public bool IsFake
		{
			get { return _fake; }
		}

		/// <summary>It is a remote tracking branch.</summary>
		public bool IsRemote
		{
			get { return _remote; }
		}

		/// <summary>This branch is current HEAD.</summary>
		public bool IsCurrent
		{
			get { return _current; }
		}

		#endregion

		public override string ToString()
		{
			return _name;
		}

		#region IObjectData<Branch>

		void IObjectData<Branch>.Update(Branch branch)
		{
			var repo = branch.Repository;
			if(branch.Revision.Name != _sha1)
			{
				lock(repo.Revisions.SyncRoot)
				{
					branch.Pointer = repo.Revisions.GetOrCreateRevision(_sha1);
				}
			}
			if(_current)
			{
				repo.Head.Pointer = branch;
			}
		}

		Branch IObjectData<Branch>.Construct(IRepository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var repo = (Repository)repository;
			Revision revision;
			lock(repo.Revisions.SyncRoot)
			{
				revision = repo.Revisions.GetOrCreateRevision(_sha1);
			}
			var branch = new Branch(repo, _name, revision);
			if(_current) repo.Head.Pointer = branch;
			return branch;
		}

		#endregion

		#region IObjectData<RemoteBranch>

		void IObjectData<RemoteBranch>.Update(RemoteBranch remoteBranch)
		{
			if(!_remote) throw new InvalidOperationException();

			var repo = remoteBranch.Repository;
			if(remoteBranch.Revision.Name != _sha1)
			{
				lock(repo.Revisions.SyncRoot)
				{
					remoteBranch.Pointer = repo.Revisions.GetOrCreateRevision(_sha1);
				}
			}
		}

		RemoteBranch IObjectData<RemoteBranch>.Construct(IRepository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(!_remote) throw new InvalidOperationException();

			var repo = (Repository)repository;
			Revision revision;
			lock(repo.Revisions.SyncRoot)
			{
				revision = repo.Revisions.GetOrCreateRevision(_sha1);
			}
			return new RemoteBranch(repo, _name, revision);
		}

		#endregion
	}
}
