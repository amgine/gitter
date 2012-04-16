namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IBranchAccessor.RenameBranch"/> operation.</summary>
	public sealed class RenameBranchParameters
	{
		/// <summary>Create <see cref="RenameBranchParameters"/>.</summary>
		public RenameBranchParameters()
		{
		}

		/// <summary>Create <see cref="RenameBranchParameters"/>.</summary>
		/// <param name="oldName">Name of the branch to rename.</param>
		/// <param name="newName">New branch name.</param>
		public RenameBranchParameters(string oldName, string newName)
		{
			OldName = oldName;
			NewName = newName;
		}

		/// <summary>Name of the branch to rename.</summary>
		public string OldName { get; set; }

		/// <summary>New branch name.</summary>
		public string NewName { get; set; }

		/// <summary>Force-overwrite existing branch.</summary>
		public bool Force { get; set; }
	}
}
