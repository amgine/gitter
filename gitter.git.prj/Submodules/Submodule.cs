namespace gitter.Git
{
	using System;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Git submodule.</summary>
	public sealed class Submodule : GitNamedObjectWithLifetime
	{
		#region Data

		private string _path;
		private string _url;

		#endregion

		#region Events

		/// <summary><see cref="M:Path"/> property value changed.</summary>
		public event EventHandler PathChanged;

		/// <summary><see cref="M:Url"/> property value changed.</summary>
		public event EventHandler UrlChanged;

		private void OnPathChanged()
		{
			var handler = PathChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		private void OnUrlChanged()
		{
			var handler = UrlChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

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
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");
			Verify.Argument.IsNotNull(path, "url");

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
			private set
			{
				if(_path != value)
				{
					_path = value;
					OnPathChanged();
				}
			}
		}

		/// <summary>Submodule URL.</summary>
		public string Url
		{
			get { return _url; }
			private set
			{
				if(_url != value)
				{
					_url = value;
					OnUrlChanged();
				}
			}
		}

		#endregion

		#region Methods

		public void Update()
		{
			Verify.State.IsNotDeleted(this);

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
			Verify.State.IsNotDeleted(this);

			return AsyncAction.Create(
				new SubmoduleUpdateParameters()
				{
					Path = _path,
					Recursive = true,
					Init = true,
				},
				(data, monitor) =>
				{
					Repository.Accessor.UpdateSubmodule(data);
				},
				Resources.StrUpdate,
				Resources.StrFetchingDataFromRemoteRepository);
		}

		internal void UpdateInfo(string path, string url)
		{
			Assert.IsNotNull(path);
			Assert.IsNotNull(url);

			Path = path;
			Url = url;
		}

		#endregion
	}
}
