namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IStashAccessor.StashToBranch()"/> operation.</summary>
	public sealed class StashToBranchParameters
	{
		/// <summary>Create <see cref="StashToBranchParameters"/>.</summary>
		public StashToBranchParameters()
		{
		}

		/// <summary>Create <see cref="StashToBranchParameters"/>.</summary>
		/// <param name="stashName">Stash to convert.</param>
		/// <param name="branchName">Branch name.</param>
		public StashToBranchParameters(string stashName, string branchName)
		{
			StashName = stashName;
			BranchName = branchName;
		}

		/// <summary>Stash to convert.</summary>
		public string StashName { get; set; }

		/// <summary>Branch name.</summary>
		public string BranchName { get; set; }
	}
}
