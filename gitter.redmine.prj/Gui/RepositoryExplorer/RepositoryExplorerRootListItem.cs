namespace gitter.Redmine.Gui
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	sealed class RepositoryExplorerRootListItem : RepositoryExplorerItemBase
	{
		public RepositoryExplorerRootListItem(IWorkingEnvironment env, RedmineGuiProvider guiProvider)
			: base(env, guiProvider, CachedResources.Bitmaps["ImgRedmine"], Resources.StrRedmine)
		{
			Items.Add(new RepositoryExplorerNewsListItem(env, guiProvider));
			Items.Add(new RepositoryExplorerIssuesListItem(env, guiProvider));
			Items.Add(new RepositoryExplorerVersionsListItem(env, guiProvider));
			Expand();
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new RedmineMenu(WorkingEnvironment, GuiProvider);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
