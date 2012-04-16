namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Restriction on branch type.</summary>
	[Flags]
	public enum QueryBranchRestriction
	{
		/// <summary>Local branches.</summary>
		Local = 1,
		/// <summary>Remote tracking branches.</summary>
		Remote = 2,

		/// <summary>All branches.</summary>
		All = Local | Remote,
	}
}
