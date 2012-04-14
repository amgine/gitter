namespace gitter.Git.AccessLayer
{
	/// <summary>Branch tracking mode.</summary>
	public enum BranchTrackingMode
	{
		/// <summary>Do as written in configs.</summary>
		Default,
		/// <summary>Force tracking.</summary>
		Tracking,
		/// <summary>Disable tracking.</summary>
		NotTracking,
	}
}
