namespace gitter.Git
{
	public enum FileStatus
	{
		Unknown		= 0,
		Cached		= (int)'H',
		Unmerged	= (int)'M',
		Removed		= (int)'R',
		Modified	= (int)'C',
		Killed		= (int)'K',
		Added		= (int)'?',
		Renamed		= (int)'r',
		Copied		= (int)'c',
		ModeChanged	= (int)'x',
	}
}
