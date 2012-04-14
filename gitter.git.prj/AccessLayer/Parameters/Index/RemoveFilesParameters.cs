namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IIndexAccessor.RemoveFiles"/> operation.</summary>
	public sealed class RemoveFilesParameters
	{
		/// <summary>Create <see cref="RemoveFilesParameters"/>.</summary>
		public RemoveFilesParameters()
		{
		}

		/// <summary>Create <see cref="RemoveFilesParameters"/>.</summary>
		public RemoveFilesParameters(string path)
		{
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="RemoveFilesParameters"/>.</summary>
		public RemoveFilesParameters(IList<string> paths)
		{
			Paths = paths;
		}

		/// <summary>Paths to remove.</summary>
		public IList<string> Paths { get; set; }

		/// <summary>Remove file irrespective its modified status.</summary>
		public bool Force { get; set; }

		/// <summary>Recursively remove files in subdirectories.</summary>
		public bool Recursive { get; set; }

		/// <summary>Remove file from index only.</summary>
		public bool Cached { get; set; }

		/// <summary>Dont fail if no files mathc paths.</summary>
		public bool IgnoreUnmatch { get; set; }
	}
}
