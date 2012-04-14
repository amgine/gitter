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

	sealed class RepositoryStashListItem : RepositoryExplorerItemBase
	{
		public RepositoryStashListItem()
			: base(CachedResources.Bitmaps["ImgStash"], Resources.StrStash)
		{
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			RepositoryProvider.Environment.ViewDockService.ShowView(Guids.StashViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(Repository != null)
			{
				var menu = new StashMenu(Repository);
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			else
			{
				return null;
			}
		}
	}
}
