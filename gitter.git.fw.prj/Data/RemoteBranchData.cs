namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>RemoteBranch description.</summary>
	public sealed class RemoteBranchData : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _sha1;

		#endregion

		#region .ctor

		public RemoteBranchData(string name, string sha1)
		{
			if(name == null) throw new ArgumentNullException("name");
			if(name.Length == 0) throw new ArgumentException("name");
			if(sha1 == null) throw new ArgumentNullException("sha1");
			if(sha1.Length != 40) throw new ArgumentException("sha1");

			_name = name;
			_sha1 = sha1;
		}

		#endregion

		#region Properties

		/// <summary>Branche's name (short format, excluding /refs/remotes/).</summary>
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
			get { return false; }
		}

		/// <summary>It is a remote tracking branch.</summary>
		public bool IsRemote
		{
			get { return true; }
		}

		/// <summary>This branch is current HEAD.</summary>
		public bool IsCurrent
		{
			get { return false; }
		}

		#endregion

		public override string ToString()
		{
			return _name;
		}
	}
}
