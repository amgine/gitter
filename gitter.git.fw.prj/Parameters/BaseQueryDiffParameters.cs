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

	/// <summary>Base class for all diff parameters.</summary>
	public abstract class BaseQueryDiffParameters
	{
		/// <summary>Create <see cref="BaseQueryDiffParameters"/>.</summary>
		protected BaseQueryDiffParameters()
		{
			Context = -1;
		}

		/// <summary>Requested context lines (-1 = default).</summary>
		public int Context { get; set; }

		/// <summary>Compare width index.</summary>
		public bool Cached { get; set; }

		/// <summary>Generate binary patch.</summary>
		public bool Binary { get; set; }

		/// <summary>Use "patience diff" algorithm.</summary>
		public bool Patience { get; set; }

		public bool IgnoreSpaceChange { get; set; }

		public bool IgnoreSpaceAtEOL { get; set; }

		public bool IgnoreAllSpace { get; set; }

		public bool SwapInputs { get; set; }

		/// <summary>Paths to query diff for.</summary>
		public IList<string> Paths { get; set; }

		public bool? EnableTextConvFilters { get; set; }

		public bool? EnableExternalDiffDrivers { get; set; }

		public bool TreatAllFilesAsText { get; set; }

		public SimilaritySpecification? FindRenames { get; set; }

		public SimilaritySpecification? FindCopies { get; set; }
	}
}
