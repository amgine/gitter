namespace gitter.Git
{
	using System;

	public sealed class TreeData : TreeContentData
	{
		public TreeData(string hash, int mode, string name)
			: base(hash, mode, name)
		{
		}

		public override TreeContentType Type
		{
			get { return TreeContentType.Tree; }
		}
	}
}
