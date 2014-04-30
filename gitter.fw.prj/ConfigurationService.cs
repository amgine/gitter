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

namespace gitter.Framework
{
	using System;
	using System.IO;

	using gitter.Framework.Configuration;
	using gitter.Framework.Services;

	public sealed class ConfigurationService : IDisposable
	{
		#region Constants

		private const string ConfigFileName = "gitter.xml";
		private const string AppFolderName = "gitter";

		#endregion

		#region Data

		private readonly string _configPath;
		private readonly string _configFileName;
		private ConfigurationManager _configuration;

		private Section _rootSection;
		private Section _guiSection;
		private Section _globalSection;
		private Section _viewsSection;
		private Section _providersSection;
		private Section _repositoryManagerSection;

		private bool _isDisposed;

		#endregion

		#region .ctor & finalizer

		internal ConfigurationService()
		{
			var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			_configPath = Path.Combine(appData, AppFolderName);
			_configFileName = Path.Combine(_configPath, ConfigFileName);

			if(!Directory.Exists(_configPath))
			{
				try
				{
					Directory.CreateDirectory(_configPath);
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
					LoggingService.Global.Error(exc);
				}
			}

			_configuration            = LoadConfig(ConfigFileName, "Configuration");
			_rootSection              = _configuration.RootSection;
			_guiSection               = _rootSection.GetCreateSection("Gui");
			_globalSection            = _rootSection.GetCreateSection("Global");
			_viewsSection             = _rootSection.GetCreateSection("Tools");
			_providersSection         = _rootSection.GetCreateSection("Providers");
			_repositoryManagerSection = _rootSection.GetCreateSection("RepositoryManager");
		}

		~ConfigurationService()
		{
			Dispose(false);
		}

		#endregion

		public ConfigurationManager Configuration
		{
			get { return _configuration; }
		}

		public Stream CreateFile(string fileName)
		{
			return new FileStream(Path.Combine(_configPath, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
		}

		public Stream OpenFile(string fileName)
		{
			return new FileStream(Path.Combine(_configPath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public bool FileExists(string fileName)
		{
			return File.Exists(Path.Combine(_configPath, fileName));
		}

		public Section RootSection
		{
			get { return _rootSection; }
		}

		public Section GuiSection
		{
			get { return _guiSection; }
		}

		public Section GlobalSection
		{
			get { return _globalSection; }
		}

		public Section ViewsSection
		{
			get { return _viewsSection; }
		}

		public Section RepositoryManagerSection
		{
			get { return _repositoryManagerSection; }
		}

		public Section GetSectionForProvider(IRepositoryProvider provider)
		{
			Verify.Argument.IsNotNull(provider, "provider");

			return _providersSection.GetCreateSection(provider.Name);
		}

		public Section GetSectionForProvider(IRepositoryServiceProvider provider)
		{
			Verify.Argument.IsNotNull(provider, "provider");

			return _providersSection.GetCreateSection(provider.Name);
		}

		public Section GetSectionForProviderGui(IRepositoryProvider provider)
		{
			Verify.Argument.IsNotNull(provider, "provider");

			var section = _providersSection.GetCreateSection(provider.Name);
			return section.GetCreateSection("Gui");
		}

		public Section GetSectionForProviderGui(IRepositoryServiceProvider provider)
		{
			Verify.Argument.IsNotNull(provider, "provider");

			var section = _providersSection.GetCreateSection(provider.Name);
			return section.GetCreateSection("Gui");
		}

		public void Save()
		{
			SaveConfig(ConfigFileName, _configuration);
		}

		private ConfigurationManager LoadConfig(string configFile, string configName)
		{
			ConfigurationManager config = null;
			if(FileExists(configFile))
			{
				try
				{
					using(var stream = OpenFile(configFile))
					{
						if(stream.Length != 0)
						{
							using(var adapter = new XmlAdapter(stream))
							{
								try
								{
									config = new ConfigurationManager(adapter);
								}
								catch(Exception exc)
								{
									if(exc.IsCritical())
									{
										throw;
									}
									LoggingService.Global.Error(exc);
								}
							}
						}
					}
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
					LoggingService.Global.Error(exc);
				}
			}
			if(config == null)
			{
				config = new ConfigurationManager(configName);
			}
			return config;
		}

		private void SaveConfig(string configFile, ConfigurationManager config)
		{
			try
			{
				using(var stream = CreateFile(configFile))
				using(var adapter = new XmlAdapter(stream))
				{
					config.Save(adapter);
				}
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				LoggingService.Global.Error(exc);
			}
		}

		#region IDisposable

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		private void Dispose(bool disposing)
		{
			if(disposing)
			{
			}
		}

		public void Dispose()
		{
			if(!IsDisposed)
			{
				GC.SuppressFinalize(this);
				Dispose(true);
				IsDisposed = true;
			}
		}

		#endregion
	}
}
