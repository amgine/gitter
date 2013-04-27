#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.Gui
{
	using System;
	using System.Linq;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RepositoryRootItem : RepositoryExplorerItemBase
	{
		public RepositoryRootItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgGit"], Resources.StrGit)
		{
			Verify.Argument.IsNotNull(environment, "environment");

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
