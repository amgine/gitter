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

	sealed class RepositoryExplorerIssuesListItem : RepositoryExplorerItemBase
	{
		public RepositoryExplorerIssuesListItem(IWorkingEnvironment env, RedmineServiceContext service)
			: base(env, service, CachedResources.Bitmaps["ImgBug"], Resources.StrIssues)
		{
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			ShowView(Guids.IssuesViewGuid);
		}
	}
}
