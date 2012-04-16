namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IIndexAccessor.QueryStatus"/> operation.</summary>
	public sealed class QueryStatusParameters
	{
		/// <summary>Create <see cref="QueryStatusParameters"/>.</summary>
		public QueryStatusParameters()
		{
			UntrackedFilesMode = StatusUntrackedFilesMode.All;
		}

		/// <summary>Create <see cref="QueryStatusParameters"/>.</summary>
		/// <param name="path">Path to check for changes.</param>
		public QueryStatusParameters(string path)
		{
			Path = path;
			UntrackedFilesMode = StatusUntrackedFilesMode.All;
		}

		/// <summary>Path to check for changes.</summary>
		public string Path { get; set; }

		/// <summary>Determines how untracked files are represented in status.</summary>
		public StatusUntrackedFilesMode UntrackedFilesMode { get; set; }

		/// <summary>Method of ignoring submodule changes.</summary>
		public StatusIgnoreSubmodulesMode IgnoreSubmodulesMode { get; set; }
	}
}
