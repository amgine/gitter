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
		private readonly bool _isFake;
		private readonly bool _isRemote;
		private readonly bool _isCurrent;

		#endregion

		#region .ctor

		public BranchData(string name, string sha1, bool isFake, bool isRemote, bool isCurrent)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(sha1, "sha1");
			Verify.Argument.IsTrue(sha1.Length == 40, "sha1");

			_name = name;
			_sha1 = sha1;
			_isFake = isFake;
			_isRemote = isRemote;
			_isCurrent = isCurrent;
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
			get { return _isFake; }
		}

		/// <summary>It is a remote tracking branch.</summary>
		public bool IsRemote
		{
			get { return _isRemote; }
		}

		/// <summary>This branch is current HEAD.</summary>
		public bool IsCurrent
		{
			get { return _isCurrent; }
		}

		#endregion

		public override string ToString()
		{
			return _name;
		}
	}
}
