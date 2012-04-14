namespace gitter.Git
{
	using System;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Git submodule.</summary>
	public sealed class Submodule : GitLifeTimeNamedObject
	{
		#region Data

		private string _path;
		private string _url;

		/// <summary><see cref="M:Path"/> property value changed.</summary>
		public event EventHandler PathChanged;

		/// <summary><see cref="M:Url"/> property value changed.</summary>
		public event EventHandler UrlChanged;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="Submodule"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Submodule name.</param>
		/// <param name="path">Submodule path.</param>
		/// <param name="url">Submodule URL.</param>
		internal Submodule(Repository repository, string name, string path, string url)
			: base(repository, name)
		{
			if(path == null) throw new ArgumentNullException("path");
			if(url == null) throw new ArgumentNullException("url");

			_path = path;
			_url = url;
		}

		#endregion

		#region Properties

		/// <summary>Submodule full path.</summary>
		public string FullPath
		{
			get { return System.IO.Path.Combine(Repository.WorkingDirectory, _path); }
		}

		/// <summary>Submodule path.</summary>
		public string Path
		{
			get { return _path; }
		}

		/// <summary>Submodule URL.</summary>
		public string Url
		{
			get { return _url; }
		}

		#endregion

		#region Methods

		public void Update()
		{
			Repository.Accessor.UpdateSubmodule(
				new SubmoduleUpdateParameters()
				{
					Path = _path,
					Recursive = true,
					Init = true,
				});
		}

		public IAsyncAction UpdateAsync()
		{
			return AsyncAction.Create(
				new
				{
					Accessor = Repository.Accessor,
					Parameters = 
						new SubmoduleUpdateParameters()
						{
							Path = _path,
							Recursive = true,
							Init = true,
						},
				},
				(data, monitor) =>
				{
					data.Accessor.UpdateSubmodule(data.Parameters);
				},
				Resources.StrUpdate,
				Resources.StrFetchingDataFromRemoteRepository);
		}

		internal void UpdateInfo(string path, string url)
		{
			if(_path != path)
			{
				_path = path;
				PathChanged.Raise(this);
			}
			if(_url != url)
			{
				_url = url;
				UrlChanged.Raise(this);
			}
		}

		#endregion
	}
}
