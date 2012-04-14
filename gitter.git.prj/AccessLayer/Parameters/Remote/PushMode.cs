namespace gitter.Git.AccessLayer
{
	/// <summary>Push mode.</summary>
	public enum PushMode
	{
		/// <summary>Push only specified refspecs.</summary>
		Default,
		/// <summary>Push all local branches.</summary>
		AllLocalBranches,
		/// <summary>Push all refs.</summary>
		Mirror,
		/// <summary>Push all tags.</summary>
		Tags,
	}
}
