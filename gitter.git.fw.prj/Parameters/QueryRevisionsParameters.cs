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
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryRevisions"/> method.</summary>
	public sealed class QueryRevisionsParameters
	{
		/// <summary>Create <see cref="QueryRevisionsParameters"/>.</summary>
		public QueryRevisionsParameters()
		{
		}

		public string Since { get; set; }

		public string Until { get; set; }

		public IReadOnlyList<string> References { get; set; }

		public string ReferencesGlob { get; set; }

		public string Branches { get; set; }

		public string Tags { get; set; }

		public string Remotes { get; set; }

		public DateTime? SinceDate { get; set; }

		public DateTime? UntilDate { get; set; }

		public IList<string> Paths { get; set; }

		/// <summary>Continue listing the history of a file beyond renames (works only for a single file).</summary>
		public bool Follow { get; set; }

		/// <summary>Stop when a given path disappears from the tree.</summary>
		public bool RemoveEmpty { get; set; }

		public int MaxCount { get; set; }

		public int Skip { get; set; }

		public string AuthorPattern { get; set; }

		public string CommitterPattern { get; set; }

		public string MessagePattern { get; set; }

		public bool AllMatch { get; set; }

		public bool RegexpIgnoreCase { get; set; }

		public bool RegexpExtended { get; set; }

		public bool RegexpFixedStrings { get; set; }

		public bool All { get; set; }

		public bool Not { get; set; }

		public bool SimplifyByDecoration { get; set; }

		public bool FirstParent { get; set; }

		public bool EnableParentsRewriting { get; set; }

		public RevisionMergesQueryMode MergesMode { get; set; }

		public RevisionQueryOrder Order { get; set; }

		public bool Reverse { get; set; }
	}
}
