namespace gitter.Git
{
	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	public abstract class TreeSourceBase : ITreeSource
	{
		protected abstract Tree GetTreeCore();

		public abstract string DisplayName { get; }

		public virtual Repository Repository
		{
			get { return Revision.Repository; }
		}

		public abstract IRevisionPointer Revision
		{
			get;
		}

		public Tree GetTree()
		{
			return GetTreeCore();
		}

		public IAsyncFunc<Tree> GetTreeAsync()
		{
			return AsyncFunc.Create(
				this,
				(data, monitor) =>
				{
					return data.GetTreeCore();
				},
				string.Empty,
				Resources.StrsFetchingTree.AddEllipsis());
		}
	}
}
