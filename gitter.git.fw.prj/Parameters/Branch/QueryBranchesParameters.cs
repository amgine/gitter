#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
