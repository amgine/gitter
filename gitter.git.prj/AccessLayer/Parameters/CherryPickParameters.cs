namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.CherryPick"/> opearation.</summary>
	public sealed class CherryPickParameters
	{
		/// <summary>Create <see cref="CherryPickParameters"/>.</summary>
		public CherryPickParameters()
		{
		}

		/// <summary>Create <see cref="CherryPickParameters"/>.</summary>
		/// <param name="revision">Revision to cherry-pick.</param>
		public CherryPickParameters(string revision)
		{
			Revisions = new string[] { revision };
		}

		/// <summary>Create <see cref="CherryPickParameters"/>.</summary>
		/// <param name="revisions">Revisions to cherry-pick.</param>
		public CherryPickParameters(IList<string> revisions)
		{
			Revisions = revisions;
		}

		/// <summary>Create <see cref="CherryPickParameters"/>.</summary>
		/// <param name="revision">Revision to cherry-pick.</param>
		/// <param name="noCommit">>Don't create commit.</param>
		public CherryPickParameters(string revision, bool noCommit)
		{
			Revisions = new string[] { revision };
			NoCommit = noCommit;
		}

		/// <summary>Create <see cref="CherryPickParameters"/>.</summary>
		/// <param name="revisions">Revisions to cherry-pick.</param>
		/// <param name="noCommit">>Don't create commit.</param>
		public CherryPickParameters(IList<string> revisions, bool noCommit)
		{
			Revisions = revisions;
			NoCommit = noCommit;
		}

		/// <summary>Revisions to cherry-pick.</summary>
		public IList<string> Revisions { get; set; }

		/// <summary>Don't create commit.</summary>
		public bool NoCommit { get; set; }
	}
}
