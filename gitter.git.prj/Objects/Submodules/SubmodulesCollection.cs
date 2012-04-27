namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository submodules collection.</summary>
	public sealed class SubmodulesCollection : GitObjectCollection<Submodule, SubmoduleEventArgs>
	{
		#region .ctor

		internal SubmodulesCollection(Repository repository)
			: base(repository)
		{
		}

		#endregion

		protected override SubmoduleEventArgs CreateEventArgs(Submodule item)
		{
			return new SubmoduleEventArgs(item);
		}

		public void Update()
		{
			Repository.Accessor.UpdateSubmodule(
				new SubmoduleUpdateParameters()
				{
					Recursive = true,
					Init = true,
				});
		}

		public IAsyncAction UpdateAsync()
		{
			return AsyncAction.Create(
				new
				{
					Repository = Repository,
					Parameters =
						new SubmoduleUpdateParameters()
						{
							Recursive = true,
							Init = true,
						},
				},
				(data, monitor) =>
				{
					data.Repository.Accessor.UpdateSubmodule(data.Parameters);
				},
				Resources.StrUpdate,
				Resources.StrFetchingDataFromRemoteRepository);
		}

		public bool ExistsPath(string path)
		{
			if(path == null) throw new ArgumentNullException("path");
			lock(SyncRoot)
			{
				foreach(var s in ObjectStorage.Values)
				{
					if(s.Path == path) return true;
				}
			}
			return false;
		}

		public bool ExistsUrl(string url)
		{
			if(url == null) throw new ArgumentNullException("url");
			lock(SyncRoot)
			{
				foreach(var s in ObjectStorage.Values)
				{
					if(s.Url == url) return true;
				}
			}
			return false;
		}

		public bool TryGetSubmoduleByPath(string path, out Submodule submodule)
		{
			if(path == null) throw new ArgumentNullException("path");
			lock(SyncRoot)
			{
				foreach(var s in ObjectStorage.Values)
				{
					if(s.Path == path)
					{
						submodule = s;
						return true;
					}
				}
			}
			submodule = null;
			return false;
		}

		public Submodule TryGetSubmoduleByPath(string path)
		{
			if(path == null) throw new ArgumentNullException("path");
			lock(SyncRoot)
			{
				foreach(var s in ObjectStorage.Values)
				{
					if(s.Path == path)
					{
						return s;
					}
				}
			}
			return null;
		}

		public bool TryGetSubmoduleByUrl(string url, out Submodule submodule)
		{
			if(url == null) throw new ArgumentNullException("url");
			lock(SyncRoot)
			{
				foreach(var s in ObjectStorage.Values)
				{
					if(s.Url == url)
					{
						submodule = s;
						return true;
					}
				}
			}
			submodule = null;
			return false;
		}

		public Submodule TryGetSubmoduleByUrl(string url)
		{
			if(url == null) throw new ArgumentNullException("url");
			lock(SyncRoot)
			{
				foreach(var s in ObjectStorage.Values)
				{
					if(s.Url == url)
					{
						return s;
					}
				}
			}
			return null;
		}

		public Submodule Create(string path, string url)
		{
			return Create(path, url, null);
		}

		public Submodule Create(string path, string url, string branch)
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.SubmodulesChanged))
			{
				lock(SyncRoot)
				{
					try
					{
						Repository.Accessor.AddSubmodule(
							new AddSubmoduleParameters()
							{
								Branch = branch,
								Path = "./" + path,
								Repository = url,
							});
					}
					finally
					{
						Repository.Status.Refresh();
					}
					var submodule = new Submodule(Repository, path, path, url);
					AddObject(submodule);
					return submodule;
				}
			}
		}

		public void Refresh()
		{
			var submodules = new Dictionary<string, SubmoduleData>();
			var cfg = Path.Combine(Repository.WorkingDirectory, GitConstants.SubmodulesConfigFile);
			bool skipUpdate = false;
			if(File.Exists(cfg))
			{
				ConfigurationFile cfgFile = null;
				try
				{
					cfgFile = new ConfigurationFile(Repository, GitConstants.SubmodulesConfigFile, true);
				}
				catch
				{
					skipUpdate = true;
				}
				if(cfgFile != null)
				{
					foreach(var param in cfgFile)
					{
						if(param.Name.StartsWith("submodule."))
						{
							int p = param.Name.LastIndexOf('.', param.Name.Length - 1, param.Name.Length - "submodule.".Length);
							if(p != -1 && p != param.Name.Length - 1)
							{
								var p3 = param.Name.Substring(p + 1);
								bool valueIsPath = false;
								bool valueIsUrl = false;
								switch(p3)
								{
									case "path":
										valueIsPath = true;
										break;
									case "url":
										valueIsUrl = true;
										break;
									default:
										continue;
								}
								var name = param.Name.Substring("submodule.".Length, p - "submodule.".Length);
								SubmoduleData info;
								if(!submodules.TryGetValue(name, out info))
								{
									info = new SubmoduleData(name);
									submodules.Add(name, info);
								}
								if(valueIsPath)
								{
									info.Path = param.Value;
								}
								if(valueIsUrl)
								{
									info.Url = param.Value;
								}
							}
						}
					}
				}
			}

			if(!skipUpdate)
			{
				lock(SyncRoot)
				{
					CacheUpdater.UpdateObjectDictionary<Submodule, SubmoduleData>(
						ObjectStorage,
						null,
						null,
						submodules,
						submoduleData => ObjectFactories.CreateSubmodue(Repository, submoduleData),
						ObjectFactories.UpdateSubmodule,
						InvokeObjectAdded,
						InvokeObjectRemoved,
						true);
				}
			}
		}
	}
}
