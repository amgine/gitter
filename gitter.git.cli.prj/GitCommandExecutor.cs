namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;

	using gitter.Framework.Services;

	sealed class GitCommandExecutor : ICommandExecutor
	{
		private static readonly LoggingService Log = new LoggingService("Global CLI");

		private readonly GitCLI _gitCLI;

		public GitCommandExecutor(GitCLI gitCLI)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");

			_gitCLI = gitCLI;
		}

		public GitOutput ExecCommand(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.Exec(new GitInput(command));
		}

		public GitOutput ExecCommand(Command command, Encoding encoding)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.Exec(new GitInput(command, encoding));
		}

		public GitAsync ExecAsync(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.ExecAsync(new GitInput(command));
		}

		public GitAsync ExecAsync(Command command, Encoding encoding)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.ExecAsync(new GitInput(command, encoding));
		}
	}
}
