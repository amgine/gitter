namespace gitter.Git
{
	using System;
	/// <summary>Reference types.</summary>
	[Flags]
	public enum ReferenceType
	{
		/// <summary>Invalid (none) reference.</summary>
		None = (0),

		/// <summary><see cref="Revision"/>.</summary>
		Revision = (1 << 0),
		/// <summary>Local <see cref="Branch"/>.</summary>
		LocalBranch = (1 << 1),
		/// <summary>Remote <see cref="Branch"/>.</summary>
		RemoteBranch = (1 << 2),
		/// <summary><see cref="Tag"/>.</summary>
		Tag = (1 << 3),
		/// <summary><see cref="StashedState"/>.</summary>
		Stash = (1 << 4),
		/// <summary><see cref="Remote"/>.</summary>
		Remote = (1 << 5),
		/// <summary><see cref="ReflogRecord"/>.</summary>
		ReflogRecord = (1 << 6),

		/// <summary>Local or remote <see cref="Branch"/>.</summary>
		Branch = LocalBranch | RemoteBranch,
		/// <summary>Tag or branch.</summary>
		Reference = Branch | Tag,

		/// <summary>Any valid reference.</summary>
		All = Revision | Branch | Tag,
	}
}
