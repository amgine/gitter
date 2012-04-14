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

	sealed class RepositoryExplorerRootListItem : RepositoryExplorerItemBase
	{
		public RepositoryExplorerRootListItem(IWorkingEnvironment env, RedmineServiceContext service)
			: base(env, service, CachedResources.Bitmaps["ImgRedmine"], Resources.StrRedmine)
		{
			Items.Add(new RepositoryExplorerNewsListItem(env, service));
			Items.Add(new RepositoryExplorerIssuesListItem(env, service));
			Items.Add(new RepositoryExplorerVersionsListItem(env, service));
			Expand();
		}
	}
}
