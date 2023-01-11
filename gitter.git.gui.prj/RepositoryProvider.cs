#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autofac;

using gitter.Framework;
using gitter.Framework.Configuration;

using gitter.Git.AccessLayer;
using gitter.Git.Gui;
using gitter.Git.Gui.Dialogs;

using Resources = gitter.Git.Gui.Properties.Resources;

#nullable enable

/// <summary>git <see cref="Repository"/> provider.</summary>
sealed class RepositoryProvider : IGitRepositoryProvider
{
	#region Static Data

	private static readonly Version _minVersion = new(1,7,0,2);

	#endregion

	#region Data

	private IWorkingEnvironment? _environment;
	private GuiProvider? _guiProvider;
	private Section? _configSection;

	private IGitAccessorProvider? _gitAccessorProvider;
	private IGitAccessor? _gitAccessor;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="RepositoryProvider"/>.</summary>
	public RepositoryProvider(
		IReadOnlyList<IGitAccessorProvider> accessorProviders,
		IFactory<VersionCheckDialog>        versionCheckDialogFactory,
		IFactory<InitDialog>                initDialogFactory,
		IFactory<CloneDialog>               cloneDialogFactory,
		ILifetimeScope                      lifetimeScope)
	{
		Verify.Argument.IsNotNull(accessorProviders);
		Verify.Argument.IsNotNull(versionCheckDialogFactory);
		Verify.Argument.IsNotNull(initDialogFactory);
		Verify.Argument.IsNotNull(cloneDialogFactory);
		Verify.Argument.IsNotNull(lifetimeScope);

		GitAccessorProviders      = accessorProviders;
		VersionCheckDialogFactory = versionCheckDialogFactory;
		InitDialogFactory         = initDialogFactory;
		CloneDialogFactory        = cloneDialogFactory;
		LifetimeScope             = lifetimeScope;
	}

	#endregion

	#region Properties

	public string Name => "git";

	public string DisplayName => "Git";

	public IImageProvider Icon => Icons.Git;

#if NET5_0_OR_GREATER
	[System.Diagnostics.CodeAnalysis.MemberNotNullWhen(returnValue: true, nameof(_environment))]
#endif
	public bool IsLoaded => _environment is not null;

	#endregion

	public IReadOnlyList<IGitAccessorProvider> GitAccessorProviders { get; }

	public Section? ConfigSection => _configSection;

	public IFactory<VersionCheckDialog> VersionCheckDialogFactory { get; }

	private IFactory<InitDialog> InitDialogFactory { get; }

	private IFactory<CloneDialog> CloneDialogFactory { get; }

	private ILifetimeScope LifetimeScope { get; }

	public IGitAccessorProvider? ActiveGitAccessorProvider
	{
		get => _gitAccessorProvider;
		set
		{
			if(_gitAccessorProvider != value)
			{
				if(_gitAccessorProvider is not null && _gitAccessor is not null && _configSection is not null)
				{
					var gitAccessorSection = _configSection.GetCreateSection(_gitAccessorProvider.Name);
					_gitAccessor.SaveTo(gitAccessorSection);
				}

				_gitAccessorProvider = value;
				_gitAccessor = value?.CreateAccessor();

				if(_gitAccessor is not null && _configSection is not null)
				{
					var gitAccessorSection = _configSection.TryGetSection(value.Name);
					_gitAccessor.LoadFrom(gitAccessorSection);
				}
			}
		}
	}

	public IGitAccessor? GitAccessor
	{
		get => _gitAccessor;
		set
		{
			if(_gitAccessor != value)
			{
				if(_gitAccessorProvider is not null && _gitAccessor is not null && _configSection is not null)
				{
					var gitAccessorSection = _configSection.GetCreateSection(_gitAccessorProvider.Name);
					_gitAccessor.SaveTo(gitAccessorSection);
				}

				_gitAccessorProvider = _gitAccessor?.Provider;
				_gitAccessor = value;
			}
		}
	}

	public Version MinimumRequiredGitVersion => _minVersion;

	public bool LoadFor(IWorkingEnvironment environment, Section section)
	{
		Verify.Argument.IsNotNull(environment);

		if(section is not null)
		{
			var providerName = section.GetValue<string>("AccessorProvider", string.Empty);
			if(!string.IsNullOrWhiteSpace(providerName))
			{
				ActiveGitAccessorProvider = GitAccessorProviders.FirstOrDefault(
					prov => prov.Name == providerName);
			}
			ActiveGitAccessorProvider ??= GitAccessorProviders.First();
			var gitAccessorSection = section.TryGetSection(ActiveGitAccessorProvider.Name);
			if(gitAccessorSection is not null)
			{
				GitAccessor?.LoadFrom(gitAccessorSection);
			}
		}
		else
		{
			ActiveGitAccessorProvider = GitAccessorProviders.First();
		}
		Version? gitVersion;
		try
		{
			gitVersion = GitAccessor?.GitVersion;
		}
		catch(Exception exc) when (!exc.IsCritical())
		{
			gitVersion = null;
		}
		if(gitVersion is null || gitVersion < MinimumRequiredGitVersion)
		{
			using var dlg = VersionCheckDialogFactory.Create();
			dlg.RequiredVersion  = MinimumRequiredGitVersion;
			dlg.InstalledVersion = gitVersion;
			dlg.Run(environment.MainForm);
			gitVersion = dlg.InstalledVersion;
			if(gitVersion is null || gitVersion < _minVersion)
			{
				return false;
			}
		}
		_environment = environment;
		_configSection = section;
		return true;
	}

	/// <summary>Save configuration to <paramref name="section"/>.</summary>
	/// <param name="section"><see cref="Section"/> for storing configuration.</param>
	public void SaveTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		if(ActiveGitAccessorProvider is not null)
		{
			section.SetValue<string>("AccessorProvider", ActiveGitAccessorProvider.Name);
			if(GitAccessor is not null)
			{
				var gitAccessorSection = section.GetCreateSection(ActiveGitAccessorProvider.Name);
				GitAccessor.SaveTo(gitAccessorSection);
			}
		}
		_configSection = section;
	}

	public bool IsValidFor(string workingDirectory)
		=> GitAccessor is not null && GitAccessor.IsValidRepository(workingDirectory);

	public IRepository OpenRepository(string workingDirectory)
	{
		if(GitAccessor is null) throw new InvalidOperationException($"Must initialize {nameof(GitAccessor)} first.");

		return Repository.Load(GitAccessor, workingDirectory);
	}

	public async Task<IRepository> OpenRepositoryAsync(string workingDirectory,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		if(GitAccessor is null) throw new InvalidOperationException($"Must initialize {nameof(GitAccessor)} first.");

		return await Repository
			.LoadAsync(GitAccessor, workingDirectory, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	public void OnRepositoryLoaded(IRepository repository)
	{
		Verify.Argument.IsNotNull(repository);

		if(_environment is not null && repository is Repository { UserIdentity: null } gitRepository)
		{
			using var dlg = new UserIdentificationDialog(_environment, gitRepository);
			dlg.Run(_environment.MainForm);
		}
	}

	public void CloseRepository(IRepository repository)
	{
		var gitRepository = (Repository)repository;
		try
		{
			var cfgFileName = Path.Combine(gitRepository.GitDirectory, @"gitter-config");
			using var fs = new FileStream(cfgFileName, FileMode.Create, FileAccess.Write, FileShare.None);
			gitRepository.ConfigurationManager.Save(new XmlAdapter(fs));
		}
		catch(Exception exc) when (!exc.IsCritical())
		{
		}
		finally
		{
			gitRepository.Dispose();
		}
	}

	public IRepositoryGuiProvider GuiProvider
		=> _guiProvider ??= new GuiProvider(this, LifetimeScope);

	public Control CreateInitDialog() => InitDialogFactory.Create();

	public Control CreateCloneDialog() => CloneDialogFactory.Create();

	public DialogResult RunInitDialog()
	{
		Verify.State.IsTrue(IsLoaded, $"{GetType().FullName} is not loaded.");

		DialogResult res;
		string path = "";
		using(var dlg = InitDialogFactory.Create())
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
		=> RunInitDialog() == DialogResult.OK;

	public DialogResult RunCloneDialog()
	{
		Verify.State.IsTrue(IsLoaded, string.Format("{0} is not loaded.", GetType().FullName));

		DialogResult res;
		var path = default(string);
		using(var dlg = CloneDialogFactory.Create())
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
		=> RunCloneDialog() == DialogResult.OK;

	public IEnumerable<GuiCommand> GetRepositoryCommands(string workingDirectory)
	{
		Verify.Argument.IsNotNull(workingDirectory);

		yield return new GuiCommand(@"gui",
			Resources.StrlGui,
			Icons.Git,
			env => StandardTools.StartGitGui(workingDirectory));
		yield return new GuiCommand(@"gitk",
			Resources.StrlGitk,
			Icons.Git,
			env => StandardTools.StartGitk(workingDirectory))
		{
			IsEnabled = StandardTools.CanStartGitk,
		};
		yield return new GuiCommand(@"bash",
			Resources.StrlBash,
			CommonIcons.Terminal,
			env => StandardTools.StartBash(workingDirectory))
		{
			IsEnabled = StandardTools.CanStartBash,
		};
	}
}
