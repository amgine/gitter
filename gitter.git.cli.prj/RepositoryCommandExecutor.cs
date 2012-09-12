namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;

	using gitter.Framework.Services;

	/// <summary>Executes commands for specific repository.</summary>
	sealed class RepositoryCommandExecutor : ICommandExecutor
	{
		private static readonly LoggingService Log = new LoggingService("CLI");

		private readonly GitCLI _gitCLI;
		private readonly string _workingDirectory;

		/// <summary>Initializes a new instance of the <see cref="RepositoryCommandExecutor"/> class.</summary>
		/// <param name="workingDirectory">Repository working directory.</param>
		public RepositoryCommandExecutor(GitCLI gitCLI, string workingDirectory)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");
			Verify.Argument.IsNotNull(workingDirectory, "workingDirectory");

			_gitCLI = gitCLI;
			_workingDirectory = workingDirectory;
		}

		#region ICommandExecutor

		public GitOutput ExecCommand(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.Exec(
				new GitInput(_workingDirectory, command, GitProcess.DefaultEncoding));
		}

		public GitOutput ExecCommand(Command command, Encoding encoding)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.Exec(
				new GitInput(_workingDirectory, command, encoding));
		}

		public GitAsync ExecAsync(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.ExecAsync(
				new GitInput(_workingDirectory, command, GitProcess.DefaultEncoding));
		}

		public GitAsync ExecAsync(Command command, Encoding encoding)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.ExecAsync(
				new GitInput(_workingDirectory, command, encoding));
		}

		#endregion
	}
}
