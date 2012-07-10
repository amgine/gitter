namespace gitter.Git
{
	public interface IIndexDiffSource : IDiffSource
	{
		bool Cached { get; }
	}
}
