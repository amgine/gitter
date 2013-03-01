namespace gitter.Git
{
	using System;

	[Flags]
	public enum FileStatus
	{
		Unknown		= 0,
		Cached		= 1 << 0,
		Unmerged	= 1 << 1,
		Removed		= 1 << 2,
		Modified	= 1 << 3,
		Killed		= 1 << 4,
		Added		= 1 << 5,
		Renamed		= 1 << 6,
		Copied		= 1 << 7,
		Ignored		= 1 << 8,
		ModeChanged	= 1 << 9,
	}
}
