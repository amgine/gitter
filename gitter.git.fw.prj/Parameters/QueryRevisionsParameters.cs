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

		public IList<string> References { get; set; }

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
