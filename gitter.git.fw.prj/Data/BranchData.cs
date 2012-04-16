namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>Branch description.</summary>
	public sealed class BranchData : INamedObject
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
	}
}
