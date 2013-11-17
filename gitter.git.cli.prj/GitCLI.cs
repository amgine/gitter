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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.IO;

	using gitter.Framework.Configuration;

	/// <summary>Performs repository-independent git operations.</summary>
	internal sealed partial class GitCLI : IGitAccessor
	{
		private static readonly Version _minVersion = new Version(1, 7, 0, 2);

		#region Data

		private readonly IGitAccessorProvider _provider;
		private readonly ICommandExecutor _executor;
		private readonly CommandBuilder _commandBuilder;
		private readonly OutputParser _outputParser;
		private Version _gitVersion;
		private bool _autodetectGitExePath;
		private string _gitExePath;

		private readonly IGitAction<InitRepositoryParameters> _init;
		private readonly IGitAction<CloneRepositoryParameters> _clone;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="GitCLI"/> class.</summary>
		/// <param name="provider">Provider of this accessor.</param>
		public GitCLI(IGitAccessorProvider provider)
		{
			Verify.Argument.IsNotNull(provider, "provider");

			_provider				= provider;
			_executor				= new GitCommandExecutor(this);
			_commandBuilder			= new CommandBuilder(this);
			_outputParser			= new OutputParser(this);
			_autodetectGitExePath	= true;
			_gitExePath				= string.Empty;

			GitProcess.UpdateGitExePath(this);

			GitCliMethod.Create(out _init,  this, CommandBuilder.GetInitCommand);
			GitCliMethod.Create(out _clone, CommandExecutor, CommandBuilder.GetCloneCommand);
		}

		#endregion

		#region Properties

		/// <summary>Returns provider of this accessor.</summary>
		/// <value>Provider of this accessor</value>
		public IGitAccessorProvider Provider
		{
			get { return _provider; }
		}

		internal OutputParser OutputParser
		{
			get { return _outputParser; }
		}

		internal CommandBuilder CommandBuilder
		{
			get { return _commandBuilder; }
		}

		private ICommandExecutor CommandExecutor
		{
			get { return _executor; }
		}

		public bool LogCLICalls
		{
			get;
			set;
		}

		public Version MinimumRequiredGitVersion
		{
			get { return _minVersion; }
		}

		public bool EnableAnsiCodepageFallback
		{
			get { return GitProcess.EnableAnsiCodepageFallback; }
			set { GitProcess.EnableAnsiCodepageFallback = value; }
		}

		public bool AutodetectGitExePath
		{
			get { return _autodetectGitExePath; }
			set
			{
				if(_autodetectGitExePath != value)
				{
					_autodetectGitExePath = value;
					GitProcess.UpdateGitExePath(this);
				}
			}
		}

		public string ManualGitExePath
		{
			get { return _gitExePath; }
			set
			{
				if(_gitExePath != value)
				{
					_gitExePath = value;
					if(!AutodetectGitExePath)
					{
						GitProcess.UpdateGitExePath(this);
					}
				}
			}
		}

		/// <summary>Returns git version.</summary>
		/// <value>git version.</value>
		public Version GitVersion
		{
			get
			{
				var gitVersion = _gitVersion;
				if(gitVersion == null)
				{
					gitVersion = QueryVersion();
					_gitVersion = gitVersion;
				}
				return gitVersion;
			}
		}

		/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
		public IGitAction<InitRepositoryParameters> InitRepository
		{
			get { return _init; }
		}

		/// <summary>Clone existing repository.</summary>
		public IGitAction<CloneRepositoryParameters> CloneRepository
		{
			get { return _clone; }
		}

		#endregion

		/// <summary>Forces re-check of git version.</summary>
		public void InvalidateGitVersion()
		{
			_gitVersion = null;
		}

		/// <summary>Returns git version.</summary>
		/// <returns>git version.</returns>
		private Version QueryVersion()
		{
			var gitOutput = CommandExecutor.ExecuteCommand(new Command("--version"));
			gitOutput.ThrowOnBadReturnCode();
			var parser = new GitParser(gitOutput.Output);
			return parser.ReadVersion();
		}

		public IRepositoryAccessor CreateRepositoryAccessor(IGitRepository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			return new RepositoryCLI(this, repository);
		}

		public bool IsValidRepository(string path)
		{
			var gitPath = Path.Combine(path, GitConstants.GitDir);
			if(Directory.Exists(gitPath) || File.Exists(gitPath))
			{
				var executor = new RepositoryCommandExecutor(this, path);
				var gitOutput = executor.ExecuteCommand(new RevParseCommand(RevParseCommand.GitDir()));
				return gitOutput.ExitCode == 0;
			}
			return false;
		}

		/// <summary>Save parameters to the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to store parameters.</param>
		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			section.SetValue("Path", ManualGitExePath);
			section.SetValue("Autodetect", AutodetectGitExePath);
			section.SetValue("LogCLICalls", LogCLICalls);
			section.SetValue("EnableAnsiCodepageFallback", EnableAnsiCodepageFallback);
		}

		/// <summary>Load parameters from the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to look for parameters.</param>
		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			ManualGitExePath			= section.GetValue("Path", string.Empty);
			AutodetectGitExePath		= section.GetValue("Autodetect", true);
			LogCLICalls					= section.GetValue("LogCLICalls", false);
			EnableAnsiCodepageFallback	= section.GetValue("EnableAnsiCodepageFallback", false);

			GitProcess.UpdateGitExePath(this);
		}
	}
}
