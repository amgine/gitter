namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository configuration.</summary>
	public sealed class ConfigParametersCollection : GitObject, IEnumerable<ConfigParameter>
	{
		#region Events

		/// <summary>New parameter created/detected.</summary>
		public event EventHandler<ConfigParameterEventArgs> ParameterCreated;

		/// <summary>Parameter removed/lost.</summary>
		public event EventHandler<ConfigParameterEventArgs> ParameterDeleted;

		/// <summary>Invokes <see cref="ParameterCreated"/> event.</summary>
		/// <param name="parameter">New branch.</param>
		private void InvokeParameterCreated(ConfigParameter parameter)
		{
			var handler = ParameterCreated;
			if(handler != null) handler(this, new ConfigParameterEventArgs(parameter));
		}

		/// <summary>Invokes <see cref="Deleted"/> & other related events.</summary>
		/// <param name="branch">Deleted branch.</param>
		private void InvokeParameterDeleted(ConfigParameter parameter)
		{
			parameter.MarkAsDeleted();
			var handler = ParameterDeleted;
			if(handler != null) handler(this, new ConfigParameterEventArgs(parameter));
		}

		#endregion

		#region Data

		private readonly Dictionary<string, ConfigParameter> _parameters;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ConfigParametersCollection"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		internal ConfigParametersCollection(Repository repository)
			: base(repository)
		{
			_parameters = new Dictionary<string, ConfigParameter>();
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
				lock(SyncRoot)
				{
					if(!_parameters.TryGetValue(name, out res))
					{
						throw new ArgumentException("Parameter not found.", "name");
					}
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

		/// <summary>Returns parameter count.</summary>
		/// <value>Parameter count.</value>
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
		public object SyncRoot
		{
			get { return _parameters; }
		}

		/// <summary>Gets/sets default merge tool.</summary>
		public MergeTool MergeTool
		{
			get
			{
				var toolName = TryGetParameterValue(GitConstants.MergeToolParameter);
				MergeTool mergeTool;
				if(toolName == null)
				{
					mergeTool = null;
				}
				else
				{
					mergeTool = MergeTool.GetCreateByName(toolName);
				}
				return mergeTool;
			}
			set
			{
				var toolName = value == null ? "" : value.Name;
				ConfigParameter parameter;
				if(TryGetParameter(GitConstants.MergeToolParameter, out parameter))
				{
					parameter.Value = toolName;
				}
				else
				{
					CreateParameter(GitConstants.MergeToolParameter, toolName);
				}
			}
		}

		#endregion

		/// <summary>Create new configuration parameter.</summary>
		/// <param name="name">Parameter name.</param>
		/// <param name="value">Praameter value.</param>
		/// <returns>Created parameter.</returns>
		public ConfigParameter CreateParameter(string name, string value)
		{
			if(name == null) throw new ArgumentNullException("name");
			if(value == null) throw new ArgumentNullException("value");

			ConfigParameter p;
			lock(SyncRoot)
			{
				if(!_parameters.ContainsKey(name))
				{
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.ConfigUpdatedNotification))
					{
						Repository.Accessor.AddConfigValue(
							new AddConfigValueParameters(name, value));
					}
					p = new ConfigParameter(Repository, ConfigFile.Repository, name, value);
					_parameters.Add(name, p);
					InvokeParameterCreated(p);
				}
				else
				{
					throw new ArgumentException("Parameter already exists.", "name");
				}
			}
			return p;
		}

		/// <summary>Set value of a parameter or create a new one if it does not exist.</summary>
		/// <param name="name">Parameter name.</param>
		/// <param name="value">Parameter value.</param>
		/// <returns>Modified or created parameter.</returns>
		public ConfigParameter SetValue(string name, string value)
		{
			ConfigParameter p;
			lock(SyncRoot)
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

		internal void Unset(ConfigParameter parameter)
		{
			if(parameter == null) throw new ArgumentNullException("parameter");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.ConfigUpdatedNotification))
			{
				Repository.Accessor.UnsetConfigValue(
					new UnsetConfigValueParameters(parameter.Name));
				Refresh(parameter);
			}
		}

		public bool TryGetParameter(string name, out ConfigParameter parameter)
		{
			lock(SyncRoot)
			{
				return _parameters.TryGetValue(name, out parameter);
			}
		}

		public bool TryGetParameterValue(string name, out string value)
		{
			lock(SyncRoot)
			{
				ConfigParameter parameter;
				var res = _parameters.TryGetValue(name, out parameter);
				if(res)
				{
					value = parameter.Value;
					return true;
				}
				else
				{
					value = null;
					return false;
				}
			}
		}

		public ConfigParameter TryGetParameter(string name)
		{
			ConfigParameter parameter;
			lock(SyncRoot)
			{
				if(_parameters.TryGetValue(name, out parameter))
					return parameter;
			}
			return null;
		}

		public string TryGetParameterValue(string name)
		{
			ConfigParameter parameter;
			lock(SyncRoot)
			{
				if(_parameters.TryGetValue(name, out parameter))
					return parameter.Value;
			}
			return null;
		}

		#region Refresh()

		public void Refresh()
		{
			var config = Repository.Accessor.QueryConfig(
				new QueryConfigParameters());

			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<ConfigParameter, ConfigParameterData>(
					Repository,
					_parameters,
					null,
					null,
					config,
					InvokeParameterCreated,
					InvokeParameterDeleted,
					true);
			}
		}

		internal void Refresh(ConfigParameter configParameter)
		{
			if(configParameter == null) throw new ArgumentNullException("configFile");

			string name = configParameter.Name;
			var p = Repository.Accessor.QueryConfigParameter(
				new QueryConfigParameterParameters(configParameter.Name));
			if(p == null)
			{
				lock(SyncRoot)
				{
					_parameters.Remove(name.ToLowerInvariant());
					InvokeParameterDeleted(configParameter);
				}
			}
			else
			{
				p.Update(configParameter);
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
