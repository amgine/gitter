namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryRevisionDiff"/> opearation.</summary>
	public class QueryRevisionDiffParameters : BaseQueryDiffParameters
	{
		/// <summary>Create <see cref="QueryRevisionDiffParameters"/>.</summary>
		public QueryRevisionDiffParameters()
		{
		}

		/// <summary>Create <see cref="QueryRevisionDiffParameters"/>.</summary>
		/// <param name="revision">Revision to query diff for.</param>
		public QueryRevisionDiffParameters(string revision)
		{
			Revision = revision;
		}

		/// <summary>Revision to query diff for.</summary>
		public string Revision { get; set; }
	}
}
