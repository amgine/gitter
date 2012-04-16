namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRemoteAccessor.Pull"/> operation.</summary>
	public sealed class PullParameters : FetchParameters
	{
		/// <summary>Create <see cref="PullParameters"/>.</summary>
		public PullParameters()
			: base()
		{
		}

		/// <summary>Create <see cref="PullParameters"/>.</summary>
		/// <param name="all">Fetch all remotes.</param>
		public PullParameters(bool all)
			: base(all)
		{
		}

		/// <summary>Create <see cref="PullParameters"/>.</summary>
		/// <param name="repository">Repository to fetch from.</param>
		public PullParameters(string repository)
			: base(repository)
		{
		}

		/// <summary>Instead of a merge, perform a rebase after fetching.</summary>
		public bool Rebase { get; set; }

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
