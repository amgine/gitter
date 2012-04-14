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

	sealed class RepositoryHistoryListItem : RepositoryExplorerItemBase
	{
		public RepositoryHistoryListItem()
			: base(CachedResources.Bitmaps["ImgHistory"], Resources.StrHistory)
		{
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			RepositoryProvider.Environment.ViewDockService.ShowView(Guids.HistoryViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}
	}
}
