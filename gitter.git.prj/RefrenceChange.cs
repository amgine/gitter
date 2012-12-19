namespace gitter.Git
{
	public enum ReferenceChangeType
	{
		Added,
		Removed,
		Moved,
	}

	public sealed class ReferenceChange
	{
		#region Data

		private readonly ReferenceType _referenceType;
		private readonly string _fullName;
		private readonly string _name;
		private readonly string _oldHash;
		private readonly string _newHash;
		private readonly ReferenceChangeType _changeType;

		#endregion

		public ReferenceChange(
			ReferenceType referenceType,
			string fullName, string name,
			string oldHash, string newHash,
			ReferenceChangeType changeType)
		{
			_referenceType = referenceType;
			_fullName = fullName;
			_name = name;
			_oldHash = oldHash;
			_newHash = newHash;
			_changeType = changeType;
		}

		public ReferenceType ReferenceType
		{
			get { return _referenceType; }
		}

		public string FullName
		{
			get { return _fullName;}
		}

		public string Name
		{
			get { return _name;}
		}

		public string OldHash
		{
			get { return _oldHash; }
		}

		public string NewHash
		{
			get { return _newHash; }
		}

		public ReferenceChangeType ChangeType
		{
			get { return _changeType; }
		}
	}
}
