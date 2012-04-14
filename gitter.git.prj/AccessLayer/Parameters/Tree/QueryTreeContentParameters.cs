namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="ITreeAccessor.QueryTreeContent"/> operation.</summary>
	public sealed class QueryTreeContentParameters
	{
		/// <summary>Create <see cref="QueryTreeContentParameters"/>.</summary>
		public QueryTreeContentParameters()
		{
		}

		/// <summary>Create <see cref="QueryTreeContentParameters"/>.</summary>
		/// <param name="treeId">>Tree-ish.</param>
		public QueryTreeContentParameters(string treeId)
		{
			TreeId = treeId;
		}

		/// <summary>Create <see cref="QueryTreeContentParameters"/>.</summary>
		/// <param name="treeId">>Tree-ish.</param>
		/// <param name="recurse">Recurse into sub-trees.</param>
		public QueryTreeContentParameters(string treeId, bool recurse)
		{
			TreeId = treeId;
			Recurse = recurse;
		}

		/// <summary>Create <see cref="QueryTreeContentParameters"/>.</summary>
		/// <param name="treeId">>Tree-ish.</param>
		/// <param name="recurse">Recurse into sub-trees.</param>
		/// <param name="onlyTrees">Exclude files (blobs) from output.</param>
		public QueryTreeContentParameters(string treeId, bool recurse, bool onlyTrees)
		{
			TreeId = treeId;
			Recurse = recurse;
			OnlyTrees = onlyTrees;
		}

		/// <summary>Exclude files (blobs) from output.</summary>
		public bool OnlyTrees { get; set; }

		/// <summary>Recurse into sub-trees.</summary>
		public bool Recurse { get; set; }

		/// <summary>Tree-ish.</summary>
		public string TreeId { get; set; }

		/// <summary>Limiting paths.</summary>
		public IList<string> Paths { get; set; }
	}
}
