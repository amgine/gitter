namespace gitter.Git
{
	using System;

	[Flags]
	public enum StagedStatus
	{
		None		= 0,

		Committed	= (1 << 0),
		Unstaged	= (1 << 1),
		Staged		= (1 << 2),
	}
}
