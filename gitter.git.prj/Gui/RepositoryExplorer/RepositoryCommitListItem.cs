namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Properties.Resources;

	sealed class RepositoryCommitListItem : RepositoryExplorerItemBase
	{
		public RepositoryCommitListItem()
			: base(CachedResources.Bitmaps["ImgCommit"], Resources.StrCommit)
		{
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			RepositoryProvider.Environment.ViewDockService.ShowView(Guids.CommitViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}
	}
}
