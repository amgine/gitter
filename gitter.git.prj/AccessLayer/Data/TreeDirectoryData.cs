namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	public sealed class TreeDirectoryData : TreeItemData, IObjectData<TreeDirectory>
	{
		private readonly TreeDirectoryData _parent;
		private readonly IList<TreeDirectoryData> _directories;
		private readonly IList<TreeFileData> _files;

		public TreeDirectoryData(string name, string shortName, TreeDirectoryData parent, FileStatus fileStatus, StagedStatus stagedStatus)
			: base(name, fileStatus, stagedStatus)
		{
			_parent = parent;
			ShortName = shortName;
			_files = new List<TreeFileData>();
			_directories = new List<TreeDirectoryData>();
		}

		public TreeDirectoryData Parent
		{
			get { return _parent; }
		}

		public IList<TreeDirectoryData> Directories
		{
			get { return _directories; }
		}

		public IList<TreeFileData> Files
		{
			get { return _files; }
		}

		public void AddDirectory(TreeDirectoryData directory)
		{
			_directories.Add(directory);
		}

		public void AddFile(TreeFileData file)
		{
			_files.Add(file);
		}

		#region IObjectInformation<WorkingTreeFolder> Members

		public void Update(TreeDirectory obj)
		{
			
		}

		public TreeDirectory Construct(IRepository repository)
		{
			var directory = new TreeDirectory((Repository)repository, Name, null, ShortName)
			{
				StagedStatus = this.StagedStatus,
			};
			foreach(var subDirectory in _directories)
			{
				directory.AddDirectory(subDirectory.Construct(repository));
			}
			foreach(var file in _files)
			{
				directory.AddFile(file.Construct(repository));
			}
			return directory;
		}

		#endregion
	}
}
