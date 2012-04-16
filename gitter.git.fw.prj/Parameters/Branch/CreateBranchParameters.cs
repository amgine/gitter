namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IBranchAccessor.CreateBranch"/> operation.</summary>
	public sealed class CreateBranchParameters
	{
		/// <summary>Create <see cref="CreateBranchParameters"/>.</summary>
		public CreateBranchParameters()
		{
		}

		/// <summary>Create <see cref="CreateBranchParameters"/>.</summary>
		/// <param name="branchName">Branch's name</param>
		/// <param name="startingRevision">Starting revision.</param>
		/// <param name="tracking">Tracking mode.</param>
		/// <param name="checkout">Set to true to checkout branch after creation.</param>
		/// <param name="createRefLog">Create branch's reflog.</param>
		public CreateBranchParameters(string branchName, string startingRevision, bool checkout, bool orphan, bool createReflog, BranchTrackingMode trackingMode)
		{
			BranchName = branchName;
			StartingRevision = startingRevision;
			Checkout = checkout;
			Orphan = orphan;
			CreateReflog = createReflog;
			TrackingMode = trackingMode;
		}
		/// <summary>Branch's name.</summary>
		public string BranchName { get; set; }

		/// <summary>Starting revision.</summary>
		public string StartingRevision { get; set; }

		/// <summary>Set to true to checkout branch after creation.</summary>
		public bool Checkout { get; set; }

		/// <summary>Create a new orphan branch.</summary>
		public bool Orphan { get; set; }

		/// <summary>Create branch's reflog.</summary>
		public bool CreateReflog { get; set; }

		/// <summary>Tracking mode.</summary>
		public BranchTrackingMode TrackingMode { get; set; }

		/// <summary>Force-overwrite existing branch.</summary>
		public bool Force { get; set; }
	}
}
