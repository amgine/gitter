namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IBranchAccessor.DeleteBranch"/> operation.</summary>
	public sealed class DeleteBranchParameters
	{
		/// <summary>Create <see cref="DeleteBranchParameters"/>.</summary>
		public DeleteBranchParameters()
		{
		}

		/// <summary>Create <see cref="DeleteBranchParameters"/>.</summary>
		/// <param name="branchName">Name of the branch to delete.</param>
		/// <param name="force">Delete branch irrespective its merged status.</param>
		/// <param name="remote">Branch is remote tracking branch.</param>
		public DeleteBranchParameters(string branchName, bool remote, bool force)
		{
			BranchName = branchName;
			Remote = remote;
			Force = force;
		}

		/// <summary>Branch name.</summary>
		public string BranchName { get; set; }

		/// <summary>Branch is remote.</summary>
		public bool Remote { get; set; }

		/// <summary>Delete branch irrespective its merged status.</summary>
		public bool Force { get; set; }
	}
}
