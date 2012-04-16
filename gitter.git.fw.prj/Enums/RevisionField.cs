namespace gitter.Git.AccessLayer
{
	using System;

	[Flags]
	public enum RevisionField
	{
		None = 0,

		SHA1			= (1 << 0),
		TreeHash		= (1 << 1),
		Parents			= (1 << 2),
		Children		= (1 << 3),
		Boundary		= (1 << 4),
		Subject			= (1 << 5),
		Body			= (1 << 6),
		CommitDate		= (1 << 7),
		CommitterName	= (1 << 8),
		CommitterEmail	= (1 << 9),
		AuthorDate		= (1 << 10),
		AuthorName		= (1 << 11),
		AuthorEmail		= (1 << 12),
	}
}
