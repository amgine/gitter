namespace gitter.Git
{
	using System;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git configuration parameter.</summary>
	public sealed class ConfigParameter : GitLifeTimeNamedObject
	{
		#region Events

		public event EventHandler ValueChanged;

		private void InvokeValueChanged()
		{
			var handler = ValueChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Data

		private readonly ConfigFile _configFile;
		private readonly string _fileName;
		private string _value;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ConfigParameter"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Paramater name.</param>
		/// <param name="value">Parameter value.</param>
		internal ConfigParameter(Repository repository, ConfigFile configFile, string name, string value)
			: base(repository, name)
		{
			_value = value;
		}

		/// <summary>Create <see cref="ConfigParameter"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Paramater name.</param>
		/// <param name="value">Parameter value.</param>
		internal ConfigParameter(ConfigFile configFile, string name, string value)
			: base(name)
		{
			_configFile = configFile;
			_value = value;
		}

		/// <summary>Create <see cref="ConfigParameter"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Paramater name.</param>
		/// <param name="value">Parameter value.</param>
		internal ConfigParameter(string fileName, string name, string value)
			: base(name)
		{
			_configFile = Git.ConfigFile.Other;
			_fileName = fileName;
			_value = value;
		}

		#endregion

		#region Properties

		/// <summary>Returns or sets parameter value.</summary>
		/// <value>Parameter value.</value>
		public string Value
		{
			get { return _value; }
			set
			{
				if(IsDeleted)
				{
					throw new InvalidOperationException(
						Resources.ExcObjectIsDeleted.UseAsFormat("ConfigParameter"));
				}
				if(_value != value)
				{
					switch(_configFile)
					{
						case Git.ConfigFile.Other:
							{
								if(Repository == null)
								{
									RepositoryProvider.Git.SetConfigValue(
										new SetConfigValueParameters(Name, value)
										{
											FileName = _fileName,
											ConfigFile = Git.ConfigFile.Other,
										});
								}
								else
								{
									Repository.Accessor.SetConfigValue(
										new SetConfigValueParameters(Name, value)
										{
											ConfigFile = Git.ConfigFile.Other,
											FileName = _fileName,
										});
								}
							}
							break;
						case Git.ConfigFile.Repository:
							{
								Repository.Accessor.SetConfigValue(
									new SetConfigValueParameters(Name, value));
							}
							break;
						default:
							{
								RepositoryProvider.Git.SetConfigValue(
									new SetConfigValueParameters(Name, value)
									{
										FileName = _fileName,
										ConfigFile = Git.ConfigFile.Other,
									});
							}
							break;
					}
					_value = value;
					InvokeValueChanged();
				}
			}
		}

		public ConfigFile ConfigFile
		{
			get { return _configFile; }
		}

		public string FileName
		{
			get { return _fileName; }
		}

		#endregion

		public void Unset()
		{
			if(_configFile == ConfigFile.Repository)
			{
				Repository.Configuration.Unset(this);
			}
			else
			{
				RepositoryProvider.Git.UnsetConfigValue(new UnsetConfigValueParameters(Name)
					{
						ConfigFile = _configFile,
					});
				Refresh();
			}
		}

		/// <summary>Update parameter value.</summary>
		public void Refresh()
		{
			if(_configFile == ConfigFile.Repository)
			{
				Repository.Configuration.Refresh(this);
			}
			else
			{
				ConfigParameterData p;
				if(_configFile == Git.ConfigFile.Other)
				{
					p = RepositoryProvider.Git.QueryConfigParameter(
						new QueryConfigParameterParameters(_fileName, Name));
				}
				else
				{
					p = RepositoryProvider.Git.QueryConfigParameter(
						new QueryConfigParameterParameters(_configFile, Name));
				}
				if(p == null)
				{
					MarkAsDeleted();
				}
				else
				{
					ObjectFactories.UpdateConfigParameter(this, p);
				}
			}
		}

		internal void SetValue(string value)
		{
			if(_value != value)
			{
				_value = value;
				InvokeValueChanged();
			}
		}

		public override string ToString()
		{
			return string.Format("{0} = {1}", Name, _value);
		}
	}
}
