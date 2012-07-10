namespace gitter.Git.Gui
{
	public enum RevisionGraphItemType
	{
		/// <summary>Generic revision.</summary>
		Generic,
		/// <summary>Current revision.</summary>
		Current,
		/// <summary>Uncommitted staged changes (fake item).</summary>
		Uncommitted,
		/// <summary>Unstaged local changes (fake item).</summary>
		Unstaged,
	}
}
