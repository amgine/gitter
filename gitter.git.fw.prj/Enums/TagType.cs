namespace gitter.Git
{
	/// <summary>Tag type.</summary>
	public enum TagType
	{
		/// <summary>Tag which points directly to a <see cref="Revision"/>.</summary>
		Lightweight,
		/// <summary>Tag which points to git tag object.</summary>
		Annotated,
	}
}
