namespace gitter.Git
{
	using gitter.Framework;

	public interface ITreeSource
	{
		string DisplayName { get; }

		Repository Repository { get; }

		IRevisionPointer Revision { get; }

		Tree GetTree();

		IAsyncFunc<Tree> GetTreeAsync();
	}
}
