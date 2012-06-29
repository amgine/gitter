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

			Items.AddRange(
				new CustomListBoxItem[]
				{
					new RepositoryHistoryListItem(environment),
					new RepositoryCommitListItem(environment),
					new RepositoryStashListItem(environment),
					new RepositoryReferencesListItem(environment),
					new RepositoryRemotesListItem(environment),
					new RepositorySubmodulesListItem(environment),
					new RepositoryWorkingDirectoryListItem(),
					new RepositoryConfigurationListItem(environment),
					new RepositoryContributorsListItem(environment),
				});
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
