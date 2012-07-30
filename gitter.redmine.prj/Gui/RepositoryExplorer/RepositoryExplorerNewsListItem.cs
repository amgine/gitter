namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	sealed class RepositoryExplorerNewsListItem : RepositoryExplorerItemBase
	{
		public RepositoryExplorerNewsListItem(IWorkingEnvironment env, RedmineGuiProvider guiProvider)
			: base(env, guiProvider, CachedResources.Bitmaps["ImgNews"], Resources.StrNews)
		{
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			ShowView(Guids.NewsViewGuid);
		}
	}
}
