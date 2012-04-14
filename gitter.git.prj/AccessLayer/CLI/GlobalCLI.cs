namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.IO;
	using System.Collections.Generic;

	using gitter.Framework.Services;

	/// <summary>Performs repository-independent git operations.</summary>
	internal sealed partial class GlobalCLI : IGitAccessor
	{
		private static readonly LoggingService Log = new LoggingService("Global CLI");

		private readonly ICommandExecutor _executor;

		/// <summary>Initializes a new instance of the <see cref="GlobalCLI"/> class.</summary>
		public GlobalCLI()
		{
			_executor = new GitCommandExecutor();
		}

		public Version QueryVersion()
		{
			var output = _executor.ExecCommand(new Command("--version"));
			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			return parser.ReadVersion();
		}

		public IRepositoryAccessor CreateRepositoryAccessor(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			return new RepositoryCLI(repository);
		}

		public bool IsValidRepository(string path)
		{
			var gitPath = Path.Combine(path, GitConstants.GitDir);
			if(Directory.Exists(gitPath) || File.Exists(gitPath))
			{
				var gitOutput = GitProcess.Exec(new GitInput(path, new RevParseCommand(RevParseCommand.GitDir())));
				return gitOutput.ExitCode == 0;
			}
			return false;
		}

		/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
		/// <param name="parameters"><see cref="InitRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void InitRepository(InitRepositoryParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(3);
			if(parameters.Bare)
			{
				args.Add(InitCommand.Bare());
			}
			if(!string.IsNullOrEmpty(parameters.Template))
			{
				args.Add(InitCommand.Template(parameters.Template));
			}
			args.Add(new PathCommandArgument(parameters.Path));
			var cmd = new InitCommand(args);
			var output = GitProcess.Exec(new GitInput(parameters.Path, cmd));
			output.ThrowOnBadReturnCode();
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
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>();

			if(!string.IsNullOrEmpty(parameters.Template))
			{
				args.Add(CloneCommand.Template(parameters.Template));
			}
			if(parameters.NoCheckout)
			{
				args.Add(CloneCommand.NoCheckout());
			}
			if(parameters.Bare)
			{
				args.Add(CloneCommand.Bare());
			}
			if(parameters.Mirror)
			{
				args.Add(CloneCommand.Mirror());
			}
			if(!string.IsNullOrEmpty(parameters.RemoteName))
			{
				args.Add(CloneCommand.Origin(parameters.RemoteName));
			}
			if(parameters.Shallow)
			{
				args.Add(CloneCommand.Depth(parameters.Depth));
			}
			if(parameters.Recursive)
			{
				args.Add(CloneCommand.Recursive());
			}
			if(parameters.Monitor != null && GitFeatures.ProgressFlag.IsAvailable)
			{
				args.Add(CloneCommand.Progress());
			}

			args.Add(CloneCommand.NoMoreOptions());
			args.Add(new PathCommandArgument(parameters.Url));
			args.Add(new PathCommandArgument(parameters.Path));

			var cmd = new CloneCommand(args);

			if(parameters.Monitor == null || !GitFeatures.ProgressFlag.IsAvailable)
			{
				var output = _executor.ExecCommand(cmd);
				output.ThrowOnBadReturnCode();
			}
			else
			{
				using(var async = _executor.ExecAsync(cmd))
				{
					var mon = parameters.Monitor;
					mon.Cancelled += (sender, e) => async.Kill();
					async.ErrorReceived += (sender, e) =>
					{
						if(e.Data != null && e.Data.Length != 0)
						{
							var parser = new GitParser(e.Data);
							var progress = parser.ParseProgress();
							progress.Apply(mon);
						}
						else
						{
							mon.SetProgressIntermediate();
						}
					};
					async.Start();
					async.WaitForExit();
					if(async.ExitCode != 0)
					{
						throw new GitException(async.StdErr);
					}
				}
			}
		}
	}
}
