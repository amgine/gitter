namespace gitter.Git.AccessLayer
{
	using gitter.Framework;

	public interface IGitRepository : IRepository
	{
		string GitDirectory { get; }

		string GetGitFileName(string fileName);
	}
}
