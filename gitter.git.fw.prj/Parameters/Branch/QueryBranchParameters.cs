namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IBranchAccessor.QueryBranch"/> operation.</summary>
	public sealed class QueryBranchParameters
	{
		/// <summary>Create <see cref="QueryBranchParameters"/>.</summary>
		public QueryBranchParameters()
		{
		}

		/// <summary>Create <see cref="QueryBranchParameters"/>.</summary>
		/// <param name="branchName">Requested branch name.</param>
		/// <param name="isRemote">Branch is remote tracking branch.</param>
		public QueryBranchParameters(string branchName, bool isRemote)
		{
			BranchName = branchName;
			IsRemote = isRemote;
		}

		/// <summary>Name of requested branch.</summary>
		public string BranchName { get; set; }

		/// <summary>Branch is remote tracking branch.</summary>
		public bool IsRemote { get; set; }
	}
}
