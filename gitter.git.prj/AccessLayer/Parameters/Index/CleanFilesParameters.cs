namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IIndexAccessor.CleanFiles"/> operation.</summary>
	public sealed class CleanFilesParameters
	{
		/// <summary>Create <see cref="CleanFilesParameters"/>.</summary>
		public CleanFilesParameters()
		{
		}

		/// <summary>Create <see cref="CleanFilesParameters"/>.</summary>
		/// <param name="path">Path to clean.</param>
		public CleanFilesParameters(string path)
		{
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="CleanFilesParameters"/>.</summary>
		/// <param name="paths">Paths to clean.</param>
		public CleanFilesParameters(IList<string> paths)
		{
			Paths = paths;
		}

		/// <summary>
		/// If the git configuration variable clean.requireForce is not set to false,
		/// git clean will refuse to run unless given -f or -n. 
		/// </summary>
		public bool Force { get; set; }

		/// <summary>Clean files mode.</summary>
		public CleanFilesMode Mode { get; set; }

		/// <summary>Allow removing directories.</summary>
		public bool RemoveDirectories { get; set; }

		/// <summary>Paths to clean.</summary>
		public IList<string> Paths { get; set; }

		/// <summary>Exception paths that should not be cleaned.</summary>
		public IList<string> ExcludePatterns { get; set; }
	}
}
