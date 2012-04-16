namespace gitter.Git.AccessLayer
{
	/// <summary>Git accessor factory.</summary>
	public interface IGitAccessorProvider
	{
		/// <summary>Returns string used to identify git accessor.</summary>
		string Name { get; }

		/// <summary>Returns string to represent accessor in GUI.</summary>
		string DisplayName { get; }

		/// <summary>Creates git accessor.</summary>
		/// <returns>Created git accessor.</returns>
		IGitAccessor CreateAccessor();
	}
}
