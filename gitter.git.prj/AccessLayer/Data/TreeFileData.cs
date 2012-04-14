namespace gitter.Git
{
	using System;

	using gitter.Framework;

	public sealed class TreeFileData : TreeItemData, IObjectData<TreeFile>
	{
		private ConflictType _conflictType;

		public TreeFileData(string name, FileStatus fileStatus, ConflictType conflictType, StagedStatus stagedStatus)
			: base(name, fileStatus, stagedStatus)
		{
			_conflictType = conflictType;
		}

		public ConflictType ConflictType
		{
			get { return _conflictType; }
			set { _conflictType = value; }
		}

		#region IObjectInformation<WorkingTreeFile> Members

		public void Update(TreeFile obj)
		{
			obj.ConflictType = _conflictType;
			obj.Status = FileStatus;
			obj.StagedStatus = StagedStatus;
		}

		private string GetShortName()
		{
			bool isSubmodule = false;
			int i = Name.Length;
			while(i > 0)
			{
				i = Name.LastIndexOf('/', i - 1);
				if(i == Name.Length - 1)
				{
					isSubmodule = true;
					continue;
				}
				var s = i + 1;
				var l = Name.Length - s;
				if(isSubmodule) --l;
				return Name.Substring(i + 1, l);
			}
			return string.Empty;
		}

		public TreeFile Construct(IRepository repository)
		{
			var shortName = ShortName.Length == 0 ? GetShortName() : ShortName;
			return new TreeFile((Repository)repository, Name, null, FileStatus, shortName)
			{
				ConflictType = _conflictType,
				StagedStatus = StagedStatus,
			};
		}

		#endregion
	}
}
