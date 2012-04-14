namespace gitter.Git.AccessLayer
{
	using gitter.Git.AccessLayer.CLI;

	static class StandardTools
	{
		#region default tools

		public static void StartGitGui(Repository repository)
		{
			GitProcess.ExecNormal(new GitInput(repository.WorkingDirectory, new Command("gui"))).Dispose();
		}

		public static void StartGitk(Repository repository)
		{
			GitProcess.ExecGitk(repository.WorkingDirectory, "--all").Dispose();
		}

		public static void StartBash(Repository repository)
		{
			GitProcess.ExecSh(repository.WorkingDirectory, "--login -i").Dispose();
		}

		#endregion
	}
}
