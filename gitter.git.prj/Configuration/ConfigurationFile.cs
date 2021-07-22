#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	/// <summary>Represents a git configuration ini-style file.</summary>
	public class ConfigurationFile : IEnumerable<ConfigParameter>
	{
		#region Static

		/// <summary>Open config file with system-wide settings.</summary>
		/// <param name="configAccessor">Configuration file accessor.</param>
		/// <param name="load">Immediately load file contents.</param>
		public static ConfigurationFile OpenSystemFile(IConfigAccessor configAccessor, bool load = true)
			=> new(configAccessor, ConfigFile.System, load);

		/// <summary>Open config file with user-specific settings.</summary>
		/// <param name="configAccessor">Configuration file accessor.</param>
		/// <param name="load">Immediately load file contents.</param>
		public static ConfigurationFile OpenCurrentUserFile(IConfigAccessor configAccessor, bool load = true)
			=> new(configAccessor, ConfigFile.User, load);

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
			Assert.IsNotNull(parameter);

			ParameterCreated?.Invoke(this, new ConfigParameterEventArgs(parameter));
		}

		/// <summary>Invokes <see cref="ParameterDeleted"/> &amp; other related events.</summary>
		/// <param name="parameter">Deleted parameter.</param>
		private void InvokeDeleted(ConfigParameter parameter)
		{
			Assert.IsNotNull(parameter);

			parameter.MarkAsDeleted();
			ParameterDeleted?.Invoke(this, new ConfigParameterEventArgs(parameter));
		}

		#endregion

		#region Data

		private readonly IConfigAccessor _configAccessor;
		private readonly Repository _repository;
		private readonly string _fileName;
		private readonly ConfigFile _configFile;
		private readonly Dictionary<string, ConfigParameter> _parameters = new();

		#endregion

		#region .ctor

		private ConfigurationFile(Repository repository, bool load = true)
		{
			_configAccessor = repository.Accessor;
			_repository = repository;
			_configFile = ConfigFile.Repository;
			if(load) Refresh();
		}

		public ConfigurationFile(Repository repository, string fileName, bool load = true)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			_configAccessor = repository.Accessor;
			_repository = repository;
			_configFile = ConfigFile.Other;
			_fileName = fileName;
			if(load) Refresh();
		}

		private ConfigurationFile(IConfigAccessor configAccessor, ConfigFile configFile, bool load = true)
		{
			Verify.Argument.IsNotNull(configAccessor, nameof(configAccessor));

			_configAccessor = configAccessor;
			_configFile     = configFile;
			if(load) Refresh();
		}

		/// <summary>Create <see cref="ConfigurationFile"/>.</summary>
		/// <param name="configAccessor">Configuration file accessor.</param>
		/// <param name="fileName">Name of config file.</param>
		/// <param name="load">Immediately load file contents.</param>
		public ConfigurationFile(IConfigAccessor configAccessor, string fileName, bool load = true)
		{
			Verify.Argument.IsNotNull(configAccessor, nameof(configAccessor));

			_configAccessor = configAccessor;
			_fileName = fileName;
			_configFile = ConfigFile.Other;
			if(load) Refresh();
		}

		#endregion

		#region Properties

		public IEnumerable<string> Names => _parameters.Keys;

		public ConfigParameter this[string name]
		{
			get
			{
				ConfigParameter res;
				lock(SyncRoot)
				{
					Verify.Argument.IsTrue(_parameters.TryGetValue(name, out res), nameof(name),
						"Parameter not found.");
				}
				return res;
			}
		}

		public bool Exists(string name)
		{
			lock(SyncRoot)
			{
				return _parameters.ContainsKey(name);
			}
		}

		public int Count
		{
			get
			{
				lock(SyncRoot)
				{
					return _parameters.Count;
				}
			}
		}

		/// <summary>Object used for cross-thread synchronization.</summary>
		public object SyncRoot => _parameters;

		#endregion

		public ConfigParameter CreateParameter(string name, string value)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));
			Verify.Argument.IsNotNull(value, nameof(value));

			lock(SyncRoot)
			{
				Verify.Argument.IsFalse(_parameters.ContainsKey(name), nameof(name),
					"Parameter already exists.");
				switch(_configFile)
				{
					case ConfigFile.Other:
						_configAccessor.SetConfigValue.Invoke(new SetConfigValueParameters(name, value)
						{
							ConfigFile = _configFile,
							FileName = _fileName,
						});
						break;
					case ConfigFile.System:
					case ConfigFile.User:
						_configAccessor.SetConfigValue.Invoke(new SetConfigValueParameters(name, value)
						{
							ConfigFile = _configFile,
						});
						break;
					case ConfigFile.Repository:
						_configAccessor.SetConfigValue.Invoke(new SetConfigValueParameters(name, value));
						break;
				}
				var configParameter = _repository != null ?
					new ConfigParameter(_repository, _configFile, name, value) :
					new ConfigParameter(_configAccessor, _fileName, name, value);
				_parameters.Add(name, configParameter);
				InvokeCreated(configParameter);
				return configParameter;
			}
		}

		internal void Unset(ConfigParameter parameter)
		{
			Verify.Argument.IsNotNull(parameter, nameof(parameter));

			switch(_configFile)
			{
				case ConfigFile.Other:
					_configAccessor.UnsetConfigValue.Invoke(
						new UnsetConfigValueParameters(parameter.Name)
						{
							FileName = _fileName,
							ConfigFile = ConfigFile.Other,
						});
					break;
				case ConfigFile.System:
				case ConfigFile.User:
					_configAccessor.UnsetConfigValue.Invoke(
						new UnsetConfigValueParameters(parameter.Name)
						{
							ConfigFile = _configFile,
						});
					break;
				case ConfigFile.Repository:
					_configAccessor.UnsetConfigValue.Invoke(
						new UnsetConfigValueParameters(parameter.Name));
					break;
			}
			lock(SyncRoot)
			{
				if(_parameters.Remove(parameter.Name))
				{
					InvokeDeleted(parameter);
				}
			}
		}

		public bool TryGetParameter(string name, out ConfigParameter parameter)
		{
			lock(SyncRoot)
			{
				return _parameters.TryGetValue(name, out parameter);
			}
		}

		public ConfigParameter TryGetParameter(string name)
		{
			ConfigParameter parameter;
			lock(SyncRoot)
			{
				if(_parameters.TryGetValue(name, out parameter))
				{
					return parameter;
				}
			}
			return null;
		}

		public ConfigParameter SetValue(string name, string value)
		{
			ConfigParameter p;
			lock(_parameters)
			{
				if(_parameters.TryGetValue(name, out p))
				{
					p.Value = value;
				}
				else
				{
					p = CreateParameter(name, value);
				}
			}
			return p;
		}

		#region Refresh()

		private void Refresh(IList<ConfigParameterData> config)
		{
			lock(SyncRoot)
			{
				if(_repository is not null)
				{
					CacheUpdater.UpdateObjectDictionary(
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
				else
				{
					CacheUpdater.UpdateObjectDictionary(
						_parameters,
						null,
						null,
						config,
						configParameterData => ObjectFactories.CreateConfigParameter(_configAccessor, configParameterData),
						ObjectFactories.UpdateConfigParameter,
						InvokeCreated,
						InvokeDeleted,
						true);
				}
			}
		}

		private QueryConfigParameters GetQueryConfigParameters()
			=> _configFile switch
			{
				ConfigFile.Other      => new QueryConfigParameters(_fileName),
				ConfigFile.Repository => new QueryConfigParameters(),
				ConfigFile.System     => new QueryConfigParameters(ConfigFile.System),
				ConfigFile.User       => new QueryConfigParameters(ConfigFile.User),
				_ => throw new ApplicationException($"Unknown {nameof(ConfigFile)} value: {_configFile}"),
			};

		/// <summary>Synchronize cached information with actual data.</summary>
		public void Refresh()
		{
			var parameters = GetQueryConfigParameters();
			var config = _configAccessor.QueryConfig
				.Invoke(parameters);

			Refresh(config);
		}

		/// <summary>Synchronize cached information with actual data.</summary>
		public async Task RefreshAsync()
		{
			var parameters = GetQueryConfigParameters();
			var config = await _configAccessor.QueryConfig
				.InvokeAsync(parameters)
				.ConfigureAwait(continueOnCapturedContext: false);

			Refresh(config);
		}

		#endregion

		#region IEnumerable<ConfigParameter>

		public Dictionary<string, ConfigParameter>.ValueCollection.Enumerator GetEnumerator()
			=> _parameters.Values.GetEnumerator();

		IEnumerator<ConfigParameter> IEnumerable<ConfigParameter>.GetEnumerator()
			=> _parameters.Values.GetEnumerator();

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> _parameters.Values.GetEnumerator();

		#endregion
	}
}
