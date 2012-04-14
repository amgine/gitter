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

	sealed class RepositoryUsersListItem : RepositoryExplorerItemBase
	{
		public RepositoryUsersListItem()
			: base(CachedResources.Bitmaps["ImgUsers"], Resources.StrUsers)
		{
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			RepositoryProvider.Environment.ViewDockService.ShowView(Guids.UsersViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}
	}
}
