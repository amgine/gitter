namespace gitter.Git
{
	/// <summary>Conflict resolution type.</summary>
	public enum ConflictResolution
	{
		KeepModifiedFile,
		DeleteFile,
		UseTheirs,
		UseOurs,
	}
}
