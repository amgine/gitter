namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class RemoteReferenceData
	{
		#region Data

		private readonly string _hash;
		private readonly string _name;
		private TagType _tagType;

		#endregion

		#region .ctor

		public RemoteReferenceData(string name, string hash)
		{
			if(name == null) throw new ArgumentNullException("name");
			if(hash == null) throw new ArgumentNullException("hash");
			if(hash.Length != 40) throw new ArgumentException("hash");

			_name = name;
			_hash = hash;
		}

		#endregion

		#region Properties

		public TagType TagType
		{
			get { return _tagType; }
			set { _tagType = value; }
		}

		public ReferenceType ReferenceType
		{
			get
			{
				if(_name.StartsWith(GitConstants.LocalBranchPrefix))
					return ReferenceType.LocalBranch;
				if(_name.StartsWith(GitConstants.TagPrefix))
					return ReferenceType.Tag;
				return ReferenceType.Reference;
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public string Hash
		{
			get { return _hash; }
		}

		#endregion

		public override string ToString()
		{
			return _name;
		}
	}
}
