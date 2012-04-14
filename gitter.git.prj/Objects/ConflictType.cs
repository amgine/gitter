namespace gitter.Git
{
	/// <summary>Conflict type.</summary>
	public enum ConflictType
	{
		/// <summary>No conflict.</summary>
		None = 0,

		/// <summary>DD</summary>
		BothDeleted,
		/// <summary>AA</summary>
		BothAdded,
		/// <summary>UU</summary>
		BothModified,

		/// <summary>AU</summary>
		AddedByUs,
		/// <summary>UA</summary>
		AddedByThem,
		/// <summary>DU</summary>
		DeletedByUs,
		/// <summary>UD</summary>
		DeletedByThem,
	}
}
