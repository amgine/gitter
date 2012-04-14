namespace gitter.Git
{
	using System;

	[Flags]
	public enum DiffLineState
	{
		Invalid = 0,

		NotPresent	= (1 << 0),
		Added		= (1 << 1),
		Removed		= (1 << 2),
		Context		= (1 << 3),
		Header		= (1 << 4),
	}
}
