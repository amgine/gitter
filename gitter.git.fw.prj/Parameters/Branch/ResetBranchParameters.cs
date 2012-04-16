namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IBranchAccessor.ResetBranch"/> operation.</summary>
	public sealed class ResetBranchParameters
	{
		/// <summary>Create <see cref="ResetBranchParameters"/>.</summary>
		public ResetBranchParameters()
		{
		}

		/// <summary>Create <see cref="ResetBranchParameters"/>.</summary>
		/// <param name="branchName">Branch to reset.</param>
		/// <param name="revision">New branch position.</param>
		public ResetBranchParameters(string branchName, string revision)
		{
			BranchName = branchName;
			Revision = revision;
		}

		/// <summary>Branch to reset.</summary>
		public string BranchName { get; set; }

		/// <summary>New branch positiion.</summary>
		public string Revision { get; set; }
	}
}
