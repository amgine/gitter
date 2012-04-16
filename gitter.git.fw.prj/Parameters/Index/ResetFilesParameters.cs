namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IIndexAccessor.ResetFiles"/> operation.</summary>
	public sealed class ResetFilesParameters
	{
		/// <summary>Create <see cref="ResetFilesParameters"/>.</summary>
		public ResetFilesParameters()
		{
		}

		/// <summary>Create <see cref="ResetFilesParameters"/>.</summary>
		/// <param name="revision">Commit containing desired file states.</param>
		/// <param name="path">Path to reset.</param>
		public ResetFilesParameters(string revision, string path)
		{
			Revision = revision;
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="ResetFilesParameters"/>.</summary>
		/// <param name="path">Path to reset.</param>
		public ResetFilesParameters(string path)
		{
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="ResetFilesParameters"/>.</summary>
		/// <param name="revision">Commit containing desired file states.</param>
		/// <param name="paths">Paths to reset.</param>
		public ResetFilesParameters(string revision, IList<string> paths)
		{
			Revision = revision;
			Paths = paths;
		}

		/// <summary>Create <see cref="ResetFilesParameters"/>.</summary>
		/// <param name="paths">Paths to reset.</param>
		public ResetFilesParameters(IList<string> paths)
		{
			Paths = paths;
		}

		/// <summary>Commit containing desired file states.</summary>
		public string Revision { get; set; }

		/// <summary>Paths to reset.</summary>
		public IList<string> Paths { get; set; }
	}
}
