namespace gitter.Git.AccessLayer
{
	using gitter.Framework;
	using gitter.Git.AccessLayer.CLI;

	public static class StandardTools
	{
		public static void StartGitGui(string workingDirectory)
		{
			GitProcess.ExecNormal(new GitInput(workingDirectory, new Command("gui"))).Dispose();
		}

		public static bool CanStartGitk
		{
			get { return GitProcess.CanExecGitk; }
		}

		public static void StartGitk(string workingDirectory)
		{
			GitProcess.ExecGitk(workingDirectory, "--all").Dispose();
		}

		public static bool CanStartBash
		{
			get { return GitProcess.CanExecSh; }
		}

		public static void StartBash(string workingDirectory)
		{
			GitProcess.ExecSh(workingDirectory, "--login -i").Dispose();
		}
	}
}
