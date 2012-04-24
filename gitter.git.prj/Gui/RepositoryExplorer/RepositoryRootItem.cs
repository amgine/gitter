namespace gitter.Git.Gui
{
	using System;
	using System.Linq;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	sealed class RepositoryRootItem : RepositoryExplorerItemBase
	{
		public RepositoryRootItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgGit"], Resources.StrGit)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			Items.Add(new RepositoryHistoryListItem(environment));
			Items.Add(new RepositoryCommitListItem(environment));
			Items.Add(new RepositoryStashListItem(environment));
			Items.Add(new RepositoryReferencesListItem(environment));
			Items.Add(new RepositoryRemotesListItem(environment));
			Items.Add(new RepositorySubmodulesListItem(environment));
			Items.Add(new RepositoryWorkingDirectoryListItem());
			Items.Add(new RepositoryConfigurationListItem(environment));
			Items.Add(new RepositoryUsersListItem(environment));
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
