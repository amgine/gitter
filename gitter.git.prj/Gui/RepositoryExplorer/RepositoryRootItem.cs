namespace gitter.Git.Gui
{
	using System;
	using System.Linq;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Properties.Resources;

	sealed class RepositoryRootItem : RepositoryExplorerItemBase
	{
		public RepositoryRootItem()
			: base(CachedResources.Bitmaps["ImgGit"], Resources.StrGit)
		{
			Items.Add(new RepositoryHistoryListItem());
			Items.Add(new RepositoryCommitListItem());
			Items.Add(new RepositoryStashListItem());
			Items.Add(new RepositoryReferencesListItem());
			Items.Add(new RepositoryRemotesListItem());
			Items.Add(new RepositorySubmodulesListItem());
			Items.Add(new RepositoryWorkingDirectoryListItem());
			Items.Add(new RepositoryConfigurationListItem());
			Items.Add(new RepositoryUsersListItem());
		}

		protected override void AttachToRepository()
		{
			foreach(var item in Items.OfType<RepositoryExplorerItemBase>())
			{
				item.Repository = Repository;
			}
		}

		protected override void DetachFromRepository()
		{
			foreach(var item in Items.OfType<RepositoryExplorerItemBase>())
			{
				item.Repository = null;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(Repository != null)
			{
				var menu = new RepositoryMenu(Repository);
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
