namespace gitter.Git
{
	using System;

	public sealed class TreeCommitData : TreeContentData
	{
		public TreeCommitData(string hash, int mode, string name)
			: base(hash, mode, name)
		{
		}

		public override TreeContentType Type
		{
			get { return TreeContentType.Commit; }
		}
	}
}
