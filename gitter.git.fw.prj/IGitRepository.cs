namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public interface IGitRepository : IRepository, IDisposable
	{
		string GitDirectory { get; }

		string GetGitFileName(string fileName);
	}
}
