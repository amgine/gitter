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
	using System.Globalization;
	using System.IO;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Configuration;
	using gitter.Framework.CLI;

	using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

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
			var gitOutput = _executor.ExecuteCommand(new Command("--version"));
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

		/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
		/// <param name="parameters"><see cref="InitRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void InitRepository(InitRepositoryParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command  = CommandBuilder.GetInitCommand(parameters);
			var executor = new RepositoryCommandExecutor(this, parameters.Path);
			var output   = executor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
		/// <param name="parameters"><see cref="InitRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task InitRepositoryAsync(InitRepositoryParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command  = CommandBuilder.GetInitCommand(parameters);
			var executor = new RepositoryCommandExecutor(this, parameters.Path);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
				t =>
				{
					TaskUtility.UnwrapResult(t).ThrowOnBadReturnCode();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		/// <summary>Clone existing repository.</summary>
		/// <param name="parameters"><see cref="CloneRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CloneRepository(CloneRepositoryParameters parameters)
		{
			/*
			 * git clone [--template=<template_directory>] [-l] [-s] [--no-hardlinks]
			 * [-q] [-n] [--bare] [--mirror] [-o <name>] [-b <name>] [-u <upload-pack>]
			 * [--reference <repository>] [--depth <depth>] [--recursive] [--] <repository> [<directory>]
			*/
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetCloneCommand(parameters, false);
			if(!Directory.Exists(parameters.Path))
			{
				Directory.CreateDirectory(parameters.Path);
			}
			var output = _executor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Clone existing repository.</summary>
		/// <param name="parameters"><see cref="CloneRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task CloneRepositoryAsync(CloneRepositoryParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetCloneCommand(parameters, true);
			if(!Directory.Exists(parameters.Path))
			{
				Directory.CreateDirectory(parameters.Path);
			}
			if(progress == null)
			{
				progress = NullProgress.Instance;
			}
			progress.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));

			List<string> errorMessages = null;
			var stdOutReceiver = new NullReader();
			var stdErrReceiver = new NotifyingAsyncTextReader();
			stdErrReceiver.TextLineReceived += (s, e) =>
				{
					if(!string.IsNullOrWhiteSpace(e.Text))
					{
						var parser = new GitParser(e.Text);
						var operationProgress = parser.ParseProgress();
						progress.Report(operationProgress);
						if(operationProgress.IsIndeterminate)
						{
							if(!string.IsNullOrWhiteSpace(operationProgress.ActionName))
							{
								if(errorMessages == null)
								{
									errorMessages = new List<string>();
								}
								errorMessages.Add(operationProgress.ActionName);
							}
						}
						else
						{
							if(errorMessages != null)
							{
								errorMessages.Clear();
							}
						}
					}
				};
			return _executor.ExecuteCommandAsync(command, stdOutReceiver, stdErrReceiver, cancellationToken)
							.ContinueWith(
							t =>
							{
								var exitCode = TaskUtility.UnwrapResult(t);
								if(exitCode != 0)
								{
									string errorMessage;
									if(errorMessages != null && errorMessages.Count != 0)
									{
										errorMessage = string.Join(Environment.NewLine, errorMessages);
									}
									else
									{
										errorMessage = string.Format(CultureInfo.InvariantCulture, "git process exited with code {0}", exitCode);
									}
									throw new GitException(errorMessage);
								}
							},
							cancellationToken,
							TaskContinuationOptions.ExecuteSynchronously,
							TaskScheduler.Default);
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
