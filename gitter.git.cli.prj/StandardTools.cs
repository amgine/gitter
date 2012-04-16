namespace gitter.Git.AccessLayer
{
	using gitter.Framework;
	using gitter.Git.AccessLayer.CLI;

	public static class StandardTools
	{
		public static void StartGitGui(IRepository repository)
		{
			GitProcess.ExecNormal(new GitInput(repository.WorkingDirectory, new Command("gui"))).Dispose();
		}

		public static void StartGitk(IRepository repository)
		{
			GitProcess.ExecGitk(repository.WorkingDirectory, "--all").Dispose();
		}

		public static void StartBash(IRepository repository)
		{
			GitProcess.ExecSh(repository.WorkingDirectory, "--login -i").Dispose();
		}
	}
}
