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

		/// <summary>Paths to query diff for.</summary>
		public IList<string> Paths { get; set; }
	}
}
