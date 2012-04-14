namespace gitter.Git
{
	using System;

	using gitter.Framework;

	public abstract class TreeItemData : INamedObject
	{
		private readonly string _name;
		private string _shortName;
		private FileStatus _fileStatus;
		private StagedStatus _stagedStatus;

		protected TreeItemData(string name, FileStatus fileStatus, StagedStatus stagedStatus)
		{
			_name = name;
			_fileStatus = fileStatus;
			_stagedStatus = stagedStatus;
		}

		public string ShortName
		{
			get
			{
				if(!string.IsNullOrEmpty(_shortName))
					return _shortName;
				var pos = Name.LastIndexOf('/');
				if(pos == -1)
					_shortName = Name;
				else
					_shortName = Name.Substring(pos + 1);
				return _shortName;
			}
			set { _shortName = value; }
		}

		public FileStatus FileStatus
		{
			get { return _fileStatus; }
			set { _fileStatus = value; }
		}

		public StagedStatus StagedStatus
		{
			get { return _stagedStatus; }
			set { _stagedStatus = value; }
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
