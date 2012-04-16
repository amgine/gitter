namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	/// <summary>Represents a git configuration ini-style file.</summary>
	public class ConfigurationFile : IEnumerable<ConfigParameter>
	{
		#region Static

		/// <summary>Open config file with system-wide settings.</summary>
		public static ConfigurationFile OpenSystemFile()
		{
			return new ConfigurationFile(ConfigFile.System, true);
		}

		/// <summary>Open config file with user-specific settings.</summary>
		public static ConfigurationFile OpenCurrentUserFile()
		{
			return new ConfigurationFile(ConfigFile.User, true);
		}

		#endregion

		#region Events

		/// <summary>New parameter created/detected.</summary>
		public event EventHandler<ConfigParameterEventArgs> ParameterCreated;

		/// <summary>Parameter removed/lost.</summary>
		public event EventHandler<ConfigParameterEventArgs> ParameterDeleted;

		/// <summary>Invokes <see cref="ParameterCreated"/> event.</summary>
		/// <param name="parameter">New branch.</param>
		private void InvokeCreated(ConfigParameter parameter)
		{
			if(parameter == null) throw new ArgumentNullException("parameter");
			var handler = ParameterCreated;
			if(handler != null) handler(this, new ConfigParameterEventArgs(parameter));
		}

		/// <summary>Invokes <see cref="Deleted"/> & other related events.</summary>
		/// <param name="branch">Deleted branch.</param>
		private void InvokeDeleted(ConfigParameter parameter)
		{
			if(parameter == null) throw new ArgumentNullException("parameter");
			parameter.MarkAsDeleted();
			var handler = ParameterDeleted;
			if(handler != null) handler(this, new ConfigParameterEventArgs(parameter));
		}

		#endregion

		#region Data

		private readonly Repository _repository;
		private readonly string _fileName;
		private readonly ConfigFile _configFile;
		private readonly Dictionary<string, ConfigParameter> _parameters;

		#endregion

		#region .ctor

		private ConfigurationFile(Repository repository, bool load)
		{
			_parameters = new Dictionary<string, ConfigParameter>();
			_repository = repository;
			_configFile = ConfigFile.Repository;
			if(load) Refresh();
		}

		public ConfigurationFile(Repository repository, string fileName, bool load)
		{
			_parameters = new Dictionary<string, ConfigParameter>();
			_repository = repository;
			_configFile = ConfigFile.Other;
			_fileName = fileName;
			if(load) Refresh();
		}

		private ConfigurationFile(ConfigFile configFile, bool load)
		{
			_parameters = new Dictionary<string, ConfigParameter>();
			_configFile = configFile;
			if(load) Refresh();
		}

		/// <summary>Create <see cref="ConfigurationFile"/>.</summary>
		/// <param name="fileName">Name of config file.</param>
		public ConfigurationFile(string fileName)
			: this(fileName, true)
		{
		}

		/// <summary>Create <see cref="ConfigurationFile"/>.</summary>
		/// <param name="fileName">Name of config file.</param>
		/// <param name="load">Immediately load file contents.</param>
		public ConfigurationFile(string fileName, bool load)
		{
			_fileName = fileName;
			_parameters = new Dictionary<string, ConfigParameter>();
			_configFile = ConfigFile.Other;
			if(load) Refresh();
		}

		#endregion

		#region Properties

		public IEnumerable<string> Names
		{
			get { return _parameters.Keys; }
		}

		public ConfigParameter this[string name]
		{
			get
			{
				ConfigParameter res;
				lock(_parameters)
				{
					if(!_parameters.TryGetValue(name, out res))
						throw new ArgumentException("Parameter not found.", "name");
				}
				return res;
			}
		}

		public bool Exists(string name)
		{
			lock(_parameters)
			{
				return _parameters.ContainsKey(name);
			}
		}

		public int Count
		{
			get
			{
				lock(_parameters)
				{
					return _parameters.Count;
				}
			}
		}

		/// <summary>Object used for cross-thread synchronization.</summary>
		public object SyncRoot
		{
			get { return _parameters; }
		}

		#endregion

		public ConfigParameter CreateParameter(string name, string value)
		{
			if(name == null) throw new ArgumentNullException("name");
			if(value == null) throw new ArgumentNullException("value");
			ConfigParameter p;
			lock(_parameters)
			{
				if(!_parameters.ContainsKey(name))
				{
					switch(_configFile)
					{
						case ConfigFile.Other:
							if(_repository == null)
								RepositoryProvider.Git.SetConfigValue(new SetConfigValueParameters(name, value)
								{
									ConfigFile = _configFile,
									FileName = _fileName,
								});
							else
								_repository.Accessor.SetConfigValue(new SetConfigValueParameters(name, value)
									{
										ConfigFile = _configFile,
										FileName = _fileName,
									});
							break;
						case ConfigFile.System:
						case ConfigFile.User:
							RepositoryProvider.Git.SetConfigValue(new SetConfigValueParameters(name, value)
							{
								ConfigFile = _configFile,
							});
							break;
						case ConfigFile.Repository:
							_repository.Accessor.SetConfigValue(new SetConfigValueParameters(name, value));
							break;
					}
					p = new ConfigParameter(_fileName, name, value);
					_parameters.Add(name, p);
					InvokeCreated(p);
				}
				else
				{
					throw new ArgumentException("Parameter already exists.", "name");
				}
			}
			return p;
		}

		internal void Unset(ConfigParameter parameter)
		{
			if(parameter == null) throw new ArgumentNullException("parameter");
			switch(_configFile)
			{
				case ConfigFile.Other:
					if(_repository == null)
						RepositoryProvider.Git.UnsetConfigValue(
							new UnsetConfigValueParameters(parameter.Name)
							{
								FileName = _fileName,
								ConfigFile = ConfigFile.Other,
							});
					else
						_repository.Accessor.UnsetConfigValue(
							new UnsetConfigValueParameters(parameter.Name)
							{
								FileName = _fileName,
								ConfigFile = _configFile,
							});
					break;
				case ConfigFile.System:
				case ConfigFile.User:
					RepositoryProvider.Git.UnsetConfigValue(
						new UnsetConfigValueParameters(parameter.Name)
						{
							ConfigFile = _configFile,
						});
					break;
				case ConfigFile.Repository:
					_repository.Accessor.UnsetConfigValue(
						new UnsetConfigValueParameters(parameter.Name));
					break;
			}
			lock(_parameters)
			{
				_parameters.Remove(parameter.Name);
				InvokeDeleted(parameter);
			}
		}

		public bool TryGetParameter(string name, out ConfigParameter parameter)
		{
			lock(_parameters)
			{
				return _parameters.TryGetValue(name, out parameter);
			}
		}

		public ConfigParameter TryGetParameter(string name)
		{
			ConfigParameter parameter;
			lock(_parameters)
			{
				if(_parameters.TryGetValue(name, out parameter))
					return parameter;
			}
			return null;
		}

		public ConfigParameter SetValue(string name, string value)
		{
			ConfigParameter p;
			lock(_parameters)
			{
				if(_parameters.TryGetValue(name, out p))
					p.Value = value;
				else
					p = CreateParameter(name, value);
			}
			return p;
		}

		#region Refresh()

		/// <summary>Synchronize cached information with actual data.</summary>
		public void Refresh()
		{
			IList<ConfigParameterData> config;
			switch(_configFile)
			{
				case ConfigFile.Other:
					if(_repository == null)
						config = RepositoryProvider.Git.QueryConfig(
							new QueryConfigParameters(_fileName));
					else
						config = _repository.Accessor.QueryConfig(
							new QueryConfigParameters(_fileName));
					break;
				case ConfigFile.Repository:
					config = _repository.Accessor.QueryConfig(
						new QueryConfigParameters());
					break;
				case ConfigFile.System:
				case ConfigFile.User:
					config = RepositoryProvider.Git.QueryConfig(
						new QueryConfigParameters(_configFile));
					break;
				default:
					throw new InvalidOperationException();
			}

			lock(_parameters)
			{
				CacheUpdater.UpdateObjectDictionary<ConfigParameter, ConfigParameterData>(
					_parameters,
					null,
					null,
					config,
					configParameterData => ObjectFactories.CreateConfigParameter(_repository, configParameterData),
					ObjectFactories.UpdateConfigParameter,
					InvokeCreated,
					InvokeDeleted,
					true);
			}
		}

		#endregion

		#region IEnumerable<ConfigParameter>

		public IEnumerator<ConfigParameter> GetEnumerator()
		{
			return _parameters.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _parameters.Values.GetEnumerator();
		}

		#endregion
	}
}
