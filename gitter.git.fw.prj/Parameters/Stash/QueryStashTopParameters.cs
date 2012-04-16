namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IStashAccessor.QueryStashTop()"/> operation.</summary>
	public sealed class QueryStashTopParameters
	{
		/// <summary>Create <see cref="QueryStashTopParameters"/>.</summary>
		public QueryStashTopParameters()
		{
		}

		/// <summary>Create <see cref="QueryStashTopParameters"/>.</summary>
		/// <param name="loadCommitInfo">Load full commit information, not just SHA-1.</param>
		public QueryStashTopParameters(bool loadCommitInfo)
		{
			LoadCommitInfo = loadCommitInfo;
		}

		/// <summary>Load full commit information, not just SHA-1.</summary>
		public bool LoadCommitInfo { get; set; }
	}
}
