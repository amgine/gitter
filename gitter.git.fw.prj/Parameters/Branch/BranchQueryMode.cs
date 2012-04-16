namespace gitter.Git.AccessLayer
{
	/// <summary>Advanced query options.</summary>
	public enum BranchQueryMode
	{
		/// <summary>Restrict returned branches only by type.</summary>
		Default,
		/// <summary>Only list branches whose tips are reachable from the specified commit (HEAD if not specified).</summary>
		Merged,
		/// <summary>Only list branches whose tips are not reachable from the specified commit (HEAD if not specified).</summary>
		NoMerged,
		/// <summary>Only list branches which contain the specified commit.</summary>
		Contains,
	}
}
