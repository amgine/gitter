namespace gitter.Git
{
	using System;

	/// <summary>Reset modes.</summary>
	[Flags]
	public enum ResetMode
	{
		Mixed	= (1 << 1),
		Soft	= (1 << 2),
		Hard	= (1 << 3),
		Merge	= (1 << 4),
		Keep	= (1 << 5),
	}
}
