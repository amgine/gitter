#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer;
	using gitter.Git.Gui;
	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>git <see cref="Repository"/> provider.</summary>
	public sealed class RepositoryProvider : IGitRepositoryProvider
	{
		#region Static Data

		private static readonly IGitAccessorProvider[] _gitAccessorProviders = new[]
			{
				new gitter.Git.AccessLayer.CLI.MSysGitAccessorProvider(),
			};

		private static readonly Version _minVersion = new Version(1,7,0,2);
		private static IGitAccessorProvider _gitAccessorProvider;
		private static IGitAccessor _gitAccessor;

		#endregion

		#region Data

		private IWorkingEnvironment _environment;
		private GuiProvider _guiProvider;
		private Section _configSection;

		#endregion

		#region .ctor

		/// <summary>Initializes the <see cref="RepositoryProvider"/> class.</summary>
		static RepositoryProvider()
		{
		}

		/// <summary>Create <see cref="RepositoryProvider"/>.</summary>
		public RepositoryProvider()
		{
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return "git"; }
		}

		public string DisplayName
		{
			get { return "Git"; }
		}

		public Image Icon
		{
			get { return CachedResources.Bitmaps["ImgGit"]; }
		}

		public bool IsLoaded
		{
			get { return _environment != null; }
		}

		#endregion

		public IEnumerable<IGitAccessorProvider> GitAccessorProviders
		{
			get { return _gitAccessorProviders; }
		}

		public IGitAccessorProvider ActiveGitAccessorProvider
		{
			get { return _gitAccessorProvider; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_gitAccessorProvider != value)
				{
					if(_gitAccessorProvider != null && _gitAccessor != null && _configSection != null)
					{
						var gitAccessorSection = _configSection.GetCreateSection(_gitAccessorProvider.Name);
						_gitAccessor.SaveTo(gitAccessorSection);
					}

					_gitAccessorProvider = value;
					_gitAccessor = value.CreateAccessor();

					if(_gitAccessor != null && _configSection != null)
					{
						var gitAccessorSection = _configSection.TryGetSection(value.Name);
						_gitAccessor.LoadFrom(gitAccessorSection);
					}
				}
			}
		}

		public IGitAccessor GitAccessor
		{
			get { return _gitAccessor; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_gitAccessor != value)
				{
					if(_gitAccessorProvider != null && _gitAccessor != null && _configSection != null)
					{
						var gitAccessorSection = _configSection.GetCreateSection(_gitAccessorProvider.Name);
						_gitAccessor.SaveTo(gitAccessorSection);
					}

					_gitAccessorProvider = _gitAccessor.Provider;
					_gitAccessor = value;
				}
			}
		}

		public Version MinimumRequiredGitVersion
		{
			get { return _minVersion; }
		}

		public bool LoadFor(IWorkingEnvironment environment, Section section)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			if(section != null)
			{
				var providerName = section.GetValue<string>("AccessorProvider", string.Empty);
				if(!string.IsNullOrWhiteSpace(providerName))
				{
					ActiveGitAccessorProvider = GitAccessorProviders.FirstOrDefault(
						prov => prov.Name == providerName);
				}
				if(ActiveGitAccessorProvider == null)
				{
					ActiveGitAccessorProvider = GitAccessorProviders.First();
				}
				var gitAccessorSection = section.TryGetSection(ActiveGitAccessorProvider.Name);
				if(gitAccessorSection != null)
				{
					GitAccessor.LoadFrom(gitAccessorSection);
				}
			}
			else
			{
				ActiveGitAccessorProvider = GitAccessorProviders.First();
			}
			Version gitVersion;
			try
			{
				gitVersion = _gitAccessor.GitVersion;
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				gitVersion = null;
			}
			if(gitVersion == null || gitVersion < MinimumRequiredGitVersion)
			{
				using(var dlg = new VersionCheckDialog(environment, this, MinimumRequiredGitVersion, gitVersion))
				{
					dlg.Run(environment.MainForm);
					gitVersion = dlg.InstalledVersion;
					if(gitVersion == null || gitVersion < _minVersion)
					{
						return false;
					}
				}
			}
			GlobalOptions.RegisterPropertyPageFactory(
				new PropertyPageFactory(
					GitOptionsPage.Guid,
					Resources.StrGit,
					null,
					PropertyPageFactory.RootGroupGuid,
					env => new GitOptionsPage(env)));
			GlobalOptions.RegisterPropertyPageFactory(
				new PropertyPageFactory(
					ConfigurationPage.Guid,
					Resources.StrConfig,
					null,
					GitOptionsPage.Guid,
					env => new ConfigurationPage(env)));
			_environment = environment;
			_configSection = section;
			return true;
		}

		/// <summary>Save configuration to <paramref name="section"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			if(ActiveGitAccessorProvider != null)
			{
				section.SetValue<string>("AccessorProvider", ActiveGitAccessorProvider.Name);
				if(GitAccessor != null)
				{
					var gitAccessorSection = section.GetCreateSection(ActiveGitAccessorProvider.Name);
					GitAccessor.SaveTo(gitAccessorSection);
				}
			}
			_configSection = section;
		}

		public bool IsValidFor(string workingDirectory)
		{
			if(GitAccessor != null)
			{
				return GitAccessor.IsValidRepository(workingDirectory);
			}
			else
			{
				return false;
			}
		}

		public IRepository OpenRepository(string workingDirectory)
		{
			return Repository.Load(GitAccessor, workingDirectory);
		}

		public Task<IRepository> OpenRepositoryAsync(string workingDirectory, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return Repository.LoadAsync(GitAccessor, workingDirectory, progress, cancellationToken)
				.ContinueWith(
				t =>
				{
					var repository = TaskUtility.UnwrapResult(t);
					return (IRepository)repository;
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		public void OnRepositoryLoaded(IRepository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			var gitRepository = repository as Repository;
			Verify.Argument.IsTrue(gitRepository != null, "repository");

			if(gitRepository.UserIdentity == null)
			{
				using(var dlg = new UserIdentificationDialog(_environment, gitRepository))
				{
					dlg.Run(_environment.MainForm);
				}
			}
		}

		public void CloseRepository(IRepository repository)
		{
			var gitRepository = (Repository)repository;
			try
			{
				var cfgFileName = Path.Combine(gitRepository.GitDirectory, "gitter-config");
				using(var fs = new FileStream(cfgFileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					gitRepository.ConfigurationManager.Save(new XmlAdapter(fs));
				}
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
			}
			finally
			{
				gitRepository.Dispose();
			}
		}

		public IRepositoryGuiProvider GuiProvider
		{
			get
			{
				if(_guiProvider == null)
				{
					_guiProvider = new GuiProvider(this);
				}
				return _guiProvider;
			}
		}

		public Control CreateInitDialog()
		{
			return new InitDialog(this);
		}

		public Control CreateCloneDialog()
		{
			return new CloneDialog(this);
		}

		public DialogResult RunInitDialog()
		{
			Verify.State.IsTrue(IsLoaded, string.Format("{0} is not loaded.", GetType().FullName));

			DialogResult res;
			string path = "";
			using(var dlg = new InitDialog(this))
			{
				dlg.RepositoryPath.Value = _environment.RecentRepositoryPath;
				res = dlg.Run(_environment.MainForm);
				if(res == DialogResult.OK)
				{
					path = Path.GetFullPath(dlg.RepositoryPath.Value);
				}
			}
			if(res == DialogResult.OK)
			{
				_environment.OpenRepository(path);
			}
			return res;
		}

		bool IGitRepositoryProvider.RunInitDialog()
		{
			return RunInitDialog() == DialogResult.OK;
		}

		public DialogResult RunCloneDialog()
		{
			Verify.State.IsTrue(IsLoaded, string.Format("{0} is not loaded.", GetType().FullName));

			DialogResult res;
			var path = default(string);
			using(var dlg = new CloneDialog(this))
			{
				dlg.RepositoryPath.Value = _environment.RecentRepositoryPath;
				res = dlg.Run(_environment.MainForm);
				if(res == DialogResult.OK)
				{
					path = dlg.RepositoryPath.Value;
					if(!string.IsNullOrWhiteSpace(path))
					{
						path = Path.GetFullPath(path);
					}
				}
			}
			if(!string.IsNullOrWhiteSpace(path))
			{
				_environment.OpenRepository(path);
			}
			return res;
		}

		bool IGitRepositoryProvider.RunCloneDialog()
		{
			return RunCloneDialog() == DialogResult.OK;
		}

		public IEnumerable<GuiCommand> GetRepositoryCommands(string workingDirectory)
		{
			yield return new GuiCommand(
				"gui",
				Resources.StrlGui,
				CachedResources.Bitmaps["ImgGit"],
				env => StandardTools.StartGitGui(workingDirectory));
			yield return new GuiCommand(
				"gitk",
				Resources.StrlGitk,
				CachedResources.Bitmaps["ImgGit"],
				env => StandardTools.StartGitk(workingDirectory));
			yield return new GuiCommand(
				"bash",
				Resources.StrlBash,
				CachedResources.Bitmaps["ImgTerminal"],
				env => StandardTools.StartBash(workingDirectory));
		}
	}
}
