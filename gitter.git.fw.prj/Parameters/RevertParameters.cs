namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.Revert"/> opearation.</summary>
	public sealed class RevertParameters
	{
		/// <summary>Create <see cref="CherryPickParameters"/>.</summary>
		public RevertParameters()
		{
		}

		/// <summary>Create <see cref="RevertParameters"/>.</summary>
		/// <param name="revision">Revision to revert.</param>
		public RevertParameters(string revision)
		{
			Revisions = new string[] { revision };
		}

		/// <summary>Create <see cref="RevertParameters"/>.</summary>
		/// <param name="revisions">Revisions to revert.</param>
		public RevertParameters(IList<string> revisions)
		{
			Revisions = revisions;
		}

		/// <summary>Create <see cref="RevertParameters"/>.</summary>
		/// <param name="revision">Revision to revert.</param>
		/// <param name="noCommit">>Don't create commit.</param>
		public RevertParameters(string revision, bool noCommit)
		{
			Revisions = new string[] { revision };
			NoCommit = noCommit;
		}

		/// <summary>Create <see cref="RevertParameters"/>.</summary>
		/// <param name="revisions">Revisions to revert.</param>
		/// <param name="noCommit">>Don't create commit.</param>
		public RevertParameters(IList<string> revisions, bool noCommit)
		{
			Revisions = revisions;
			NoCommit = noCommit;
		}

		/// <summary>Revisions to revert.</summary>
		public IList<string> Revisions { get; set; }

		/// <summary>Don't create commit.</summary>
		public bool NoCommit { get; set; }

		public int Mainline { get; set; }
	}
}
