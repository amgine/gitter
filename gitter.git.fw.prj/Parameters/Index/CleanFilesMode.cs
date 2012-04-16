namespace gitter.Git.AccessLayer
{
	/// <summary>Clean files mode.</summary>
	public enum CleanFilesMode
	{
		/// <summary>Remove untracked files.</summary>
		Default,
		/// <summary>Remove untracked and ignored files.</summary>
		IncludeIgnored,
		/// <summary>Remove only ignored files.</summary>
		OnlyIgnored,
	}
}
