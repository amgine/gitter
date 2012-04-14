namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IBranchAccessor.QueryBranches"/> operation.</summary>
	public sealed class QueryBranchesParameters
	{
		/// <summary>Create <see cref="QueryBranchesParameters"/>.</summary>
		public QueryBranchesParameters()
		{
			Restriction = QueryBranchRestriction.All;
			Mode = BranchQueryMode.Default;
		}

		/// <summary>Create <see cref="QueryBranchesParameters"/>.</summary>
		/// <param name="restriction">Restriction on types of branches to return.</param>
		public QueryBranchesParameters(QueryBranchRestriction restriction)
		{
			Restriction = restriction;
			Mode = BranchQueryMode.Default;
		}

		/// <summary>Create <see cref="QueryBranchesParameters"/>.</summary>
		/// <param name="restriction">Restriction on types of branches to return.</param>
		/// <param name="mode">Relation between returned branches and specified <see cref="M:Revision"/> (HEAD if not specified).</param>
		public QueryBranchesParameters(QueryBranchRestriction restriction, BranchQueryMode mode)
		{
			Restriction = restriction;
			Mode = mode;
		}

		/// <summary>Create <see cref="QueryBranchesParameters"/>.</summary>
		/// <param name="restriction">Restriction on types of branches to return.</param>
		/// <param name="mode">Relation between returned branches and specified <see cref="M:Revision"/> (HEAD if not specified).</param>
		/// <summary>Revision to check according to <see cref="M:Mode"/>.</summary>
		public QueryBranchesParameters(QueryBranchRestriction restriction, BranchQueryMode mode, string revision)
		{
			Restriction = restriction;
			Mode = mode;
			Revision = revision;
		}

		/// <summary>Restriction on types of branches to return.</summary>
		public QueryBranchRestriction Restriction { get; set; }

		/// <summary>Relation between returned branches and specified <see cref="M:Revision"/> (HEAD if not specified).</summary>
		public BranchQueryMode Mode { get; set; }

		/// <summary>Revision to check according to <see cref="M:Mode"/>.</summary>
		public string Revision { get; set; }

		/// <summary>Allow to return a branch "(no branch)", representing detached HEAD.</summary>
		public bool AllowFakeBranch { get; set; }
	}
}
