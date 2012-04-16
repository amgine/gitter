namespace gitter.Git
{
	using System;

	using gitter.Framework;

	public sealed class TreeFileData : TreeItemData
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
	}
}
