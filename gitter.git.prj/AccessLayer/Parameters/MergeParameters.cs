namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.Merge"/> opearation.</summary>
	public sealed class MergeParameters
	{
		/// <summary>Create <see cref="MergeParameters"/>.</summary>
		public MergeParameters()
		{
		}

		/// <summary>Create <see cref="MergeParameters"/>.</summary>
		/// <param name="revisions">Branch to merge with.</param>
		public MergeParameters(string revisions)
		{
			Revisions = new[] { revisions };
		}

		/// <summary>Create <see cref="MergeParameters"/>.</summary>
		/// <param name="revisions">Branches to merge with.</param>
		public MergeParameters(IList<string> revisions)
		{
			Revisions = revisions;
		}

		/// <summary>Branches to merge with.</summary>
		public IList<string> Revisions { get; set; }

		/// <summary>Commit message.</summary>
		public string Message { get; set; }

		/// <summary>Create merge commit even if merge can be performed by fast-forwarding current branch.</summary>
		public bool NoFastForward { get; set; }

		/// <summary>Prepare index but do not commit.</summary>
		public bool NoCommit { get; set; }

		/// <summary>Merge strategy.</summary>
		public MergeStrategy Strategy { get; set; }

		/// <summary>Starategy option.</summary>
		public string StrategyOption { get; set; }

		/// <summary>Perform merge but do not consider this a merge.</summary>
		public bool Squash { get; set; }
	}
}
