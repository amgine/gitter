namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui;
	using gitter.Git.Gui.Dialogs;
	using gitter.Git.Integration;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git <see cref="Repository"/> provider.</summary>
	public sealed class RepositoryProvider : IRepositoryProvider
	{
		#region Static Data

		private static readonly IGitAccessorProvider[] _gitProviders = new[]
			{
				new gitter.Git.AccessLayer.CLI.MSysGitAccessorProvider(),
			};

		private static readonly Version _minVersion = new Version(1,7,0,2);
		private static readonly IGitAccessor _git;
		private static readonly IntegrationFeatures _integrationFeatures;
		private static IWorkingEnvironment _environment;

		internal static IWorkingEnvironment Environment
		{
			get { return _environment; }
		}

		#endregion

		/// <summary>Initializes the <see cref="RepositoryProvider"/> class.</summary>
		static RepositoryProvider()
		{
			_integrationFeatures = new IntegrationFeatures();
			_git = _gitProviders[0].CreateAccessor();
		}

		public IEnumerable<IGitAccessorProvider> AccessorProviders
		{
			get { return _gitProviders; }
		}

		/// <summary>Create <see cref="RepositoryProvider"/>.</summary>
		public RepositoryProvider()
		{
		}

		internal static IGitAccessor Git
		{
			get { return _git; }
		}

		public static IntegrationFeatures Integration
		{
			get { return _integrationFeatures; }
		}

		public static Version MinimumRequiredGitVersion
		{
			get { return _minVersion; }
		}

		public string Name
		{
			get { return "git"; }
		}

		public bool LoadFor(IWorkingEnvironment environment, Section section)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			if(section != null)
			{
				var msysgitNode = section.TryGetSection("MSysGit");
				if(msysgitNode != null)
				{
					_git.LoadFrom(msysgitNode);
				}
			}
			Version gitVersion;
			try
			{
				gitVersion = _git.GitVersion;
			}
			catch
			{
				gitVersion = null;
			}
			if(gitVersion == null || gitVersion < _minVersion)
			{
				using(var dlg = new VersionCheckDialog(environment, _minVersion, gitVersion))
				{
					dlg.Run(environment.MainForm);
					gitVersion = dlg.InstalledVersion;
					if(gitVersion == null || gitVersion < _minVersion)
					{
						return false;
					}
				}
			}
			if(section != null)
			{
				var integrationNode = section.TryGetSection("Integration");
				if(integrationNode != null)
				{
					_integrationFeatures.LoadFrom(integrationNode);
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
					IntegrationOptionsPage.Guid,
					Resources.StrIntegration,
					null,
					GitOptionsPage.Guid,
					env => new IntegrationOptionsPage()));
			GlobalOptions.RegisterPropertyPageFactory(
				new PropertyPageFactory(
					ConfigurationPage.Guid,
					Resources.StrConfig,
					null,
					GitOptionsPage.Guid,
					env => new ConfigurationPage(env)));
			_environment = environment;
			return true;
		}

		/// <summary>Save configuration to <paramref name="section"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		public void SaveTo(Section section)
		{
			if(section == null) throw new ArgumentNullException("section");

			var msysgitNode = section.GetCreateSection("MSysGit");
			_git.SaveTo(msysgitNode);

			var integrationNode = section.GetCreateSection("Integration");
			_integrationFeatures.SaveTo(integrationNode);
		}

		public bool IsValidFor(string workingDirectory)
		{
			return _git.IsValidRepository(workingDirectory);
		}

		public IRepository OpenRepository(string workingDirectory)
		{
			return new Repository(workingDirectory);
		}

		public IAsyncFunc<IRepository> OpenRepositoryAsync(string workingDirectory)
		{
			return Repository.LoadAsync(workingDirectory);
		}

		public void OnRepositoryLoaded(IWorkingEnvironment environment, IRepository repository)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			if(repository == null) throw new ArgumentNullException("repository");
			var gitRepository = repository as Repository;
			if(gitRepository == null) throw new ArgumentException("repository");

			if(!gitRepository.Configuration.Exists(GitConstants.UserNameParameter))
			{
				using(var dlg = new UserIdentificationDialog(environment, gitRepository))
				{
					dlg.Run(environment.MainForm);
				}
			}
		}

		public void CloseRepository(IRepository repository)
		{
			var gitRepository = (Repository)repository;
			gitRepository.Monitor.Shutdown();
			try
			{
				var cfgFileName = Path.Combine(gitRepository.GitDirectory, "gitter-config");
				using(var fs = new FileStream(cfgFileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					gitRepository.ConfigurationManager.Save(new XmlAdapter(fs));
				}
			}
			catch
			{
			}
		}

		public IRepositoryGuiProvider CreateGuiProvider(IRepository repository)
		{
			return new GuiProvider((Repository)repository);
		}

		public static DialogResult RunInitDialog()
		{
			return RunInitDialog(_environment);
		}

		public static DialogResult RunInitDialog(IWorkingEnvironment environment)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			DialogResult res;
			string path = "";
			using(var dlg = new InitDialog())
			{
				dlg.RepositoryPath = environment.RecentRepositoryPath;
				res = dlg.Run(environment.MainForm);
				if(res == DialogResult.OK)
				{
					path = Path.GetFullPath(dlg.RepositoryPath.Trim());
				}
			}
			if(res == DialogResult.OK)
			{
				environment.OpenRepository(path);
			}
			return res;
		}

		public static DialogResult RunCloneDialog()
		{
			return RunCloneDialog(_environment);
		}

		public static DialogResult RunCloneDialog(IWorkingEnvironment environment)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			DialogResult res;
			string path = "";
			using(var dlg = new CloneDialog())
			{
				dlg.RepositoryPath = environment.RecentRepositoryPath;
				res = dlg.Run(environment.MainForm);
				if(res == DialogResult.OK)
				{
					path = Path.GetFullPath(dlg.TargetPath.Trim());
				}
			}
			if(res == DialogResult.OK)
			{
				environment.OpenRepository(path);
			}
			return res;
		}

		public IEnumerable<StaticRepositoryAction> GetStaticActions()
		{
			yield return new StaticRepositoryAction(
				"init",
				Resources.StrInit.AddEllipsis(),
				CachedResources.Bitmaps["ImgInit"],
				env => RunInitDialog(env));
			yield return new StaticRepositoryAction(
				"clone",
				Resources.StrClone.AddEllipsis(),
				CachedResources.Bitmaps["ImgClone"],
				env => RunCloneDialog(env));
		}
	}
}
