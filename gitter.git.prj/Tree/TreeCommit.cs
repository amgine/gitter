namespace gitter.Git
{
	using System;
	using System.IO;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Represents a submodule commit.</summary>
	public sealed class TreeCommit : TreeItem
	{
		#region .ctor

		public TreeCommit(Repository repository, string relativePath, TreeDirectory parent, FileStatus status, string name)
			: base(repository, relativePath, parent, status, name)
		{
		}

		#endregion

		public override TreeItemType ItemType
		{
			get { return TreeItemType.Commit; }
		}
	}
}
