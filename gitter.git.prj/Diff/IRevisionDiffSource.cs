namespace gitter.Git
{
	public interface IRevisionDiffSource : IDiffSource
	{
		IRevisionPointer Revision { get; }
	}
}
