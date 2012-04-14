namespace gitter.Git.AccessLayer
{
	/// <summary>What happened with single pushed reference.</summary>
	public enum PushResultType
	{
		/// <summary>Successfully pushed fast-forward.</summary>
		FastForwarded,
		/// <summary>Successful forced update.</summary>
		ForceUpdated,
		/// <summary>Successfully pushed new ref.</summary>
		CreatedReference,
		/// <summary>Successfully deleted ref.</summary>
		DeletedReference,
		/// <summary>Ref was rejected or failed to push.</summary>
		Rejected,
		/// <summary>Ref was up to date and did not need pushing.</summary>
		UpToDate,
	}
}
