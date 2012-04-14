namespace gitter.Git.AccessLayer
{
	/// <summary>Determines how untracked files are represented in status.</summary>
	public enum StatusUntrackedFilesMode
	{
		/// <summary>Show no untracked files.</summary>
		No,
		/// <summary>Shows untracked files and directories.</summary>
		Normal,
		/// <summary>Show individual files in untracked directories.</summary>
		All,
	}
}
