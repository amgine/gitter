namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>Git repository.</summary>
	public interface IGitRepository : IRepository, IDisposable
	{
		/// <summary>Returns object which provides raw access to this repository.</summary>
		/// <value>Object which provides raw access to this repository.</value>
		IRepositoryAccessor Accessor { get; }

		/// <summary>GIT_DIR.</summary>
		string GitDirectory { get; }

		/// <summary>Returns full file name for a file in GIT_DIR.</summary>
		/// <param name="fileName">File name.</param>
		/// <returns>Full file name.</returns>
		string GetGitFileName(string fileName);
	}
}
