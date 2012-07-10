namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	/// <summary>Repository hook.</summary>
	public sealed class Hook : GitLifeTimeNamedObject
	{
		#region Data

		private readonly string _fullPath;
		private readonly string _relativePath;

		#endregion

		/// <summary>Create <see cref="Hook"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Hook name.</param>
		internal Hook(Repository repository, string name)
			: base(repository, name)
		{
			_relativePath = "hooks" + Path.DirectorySeparatorChar + name;
			_fullPath = Path.Combine(repository.GitDirectory, _relativePath);
			if(!File.Exists(_fullPath))
			{
				MarkAsDeleted();
			}
		}

		public string RelativePath
		{
			get { return _relativePath; }
		}

		public string FullPath
		{
			get { return _fullPath; }
		}

		public bool IsAvailable
		{
			get { return File.Exists(_fullPath); }
		}

		public void Set(string value)
		{
			File.WriteAllText(_fullPath, value);
		}

		public void Delete()
		{
			if(IsAvailable)
			{
				try
				{
					File.Delete(_fullPath);
				}
				catch
				{
				}
			}
		}
	}
}
