namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryDiff"/> opearation.</summary>
	public class QueryDiffParameters : BaseQueryDiffParameters
	{
		/// <summary>Create <see cref="QueryDiffParameters"/>.</summary>
		public QueryDiffParameters()
		{
		}

		/// <summary>Create <see cref="QueryDiffParameters"/>.</summary>
		public QueryDiffParameters(string revision1, string revision2)
		{
			Revision1 = revision1;
			Revision2 = revision2;
		}

		public string Revision1 { get; set; }

		public string Revision2 { get; set; }
	}
}
