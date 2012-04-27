namespace gitter.Git
{
	/// <summary>Checkout file mode.</summary>
	public enum CheckoutFileMode
	{
		/// <summary>Default.</summary>
		Default,
		/// <summary>When checking out paths from the index, check out stage #2 (ours) for unmerged paths.</summary>
		Ours,
		/// <summary>When checking out paths from the index, check out stage #3 (theirs) for unmerged paths.</summary>
		Theirs,
		/// <summary>This option lets you recreate the conflicted merge in the specified paths.</summary>
		Merge,
		/// <summary>Do not fail upon unmerged entries; instead, unmerged entries are ignored.</summary>
		IgnoreUnmergedEntries,
	}
}
