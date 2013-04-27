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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using gitter.Framework.Controls;

	public sealed class TreeDirectoriesBinding : IDisposable
	{
		#region Data

		private readonly CustomListBoxItemsCollection _itemHost;
		private readonly TreeDirectory _root;
		private TreeDirectoryListItem _rootItem;

		#endregion

		#region Events

		public event EventHandler<BoundItemActivatedEventArgs<TreeItem>> ItemActivated;

		private void InvokeItemActivated(CustomListBoxItem listItem, TreeItem treeItem)
		{
			var handler = ItemActivated;
			if(handler != null) handler(this,
				new BoundItemActivatedEventArgs<TreeItem>(listItem, treeItem));
		}

		#endregion

		#region .ctor

		public TreeDirectoriesBinding(CustomListBoxItemsCollection itemHost, TreeDirectory root, bool showRoot)
		{
			Verify.Argument.IsNotNull(itemHost, "itemHost");
			Verify.Argument.IsNotNull(root, "root");

			_itemHost = itemHost;
			_root = root;

			InitTree(showRoot);
		}

		private void InitTree(bool showRoot)
		{
			_itemHost.Clear();
			_itemHost.Comparison = TreeItemListItem.CompareByName;
			if(showRoot)
			{
				_rootItem = new TreeDirectoryListItem(
					_root,
					TreeDirectoryListItemType.ShowNothing,
					OnSubItemActivated);
				_rootItem.Expand();
				foreach(var folder in _root.Directories)
				{
					var item = new TreeDirectoryListItem(
						folder,
						TreeDirectoryListItemType.ShowFoldersOnly,
						OnSubItemActivated);
					item.Activated += OnItemActivated;
					_rootItem.Items.Add(item);
				}
				_itemHost.Add(_rootItem);
				_root.DirectoryAdded += OnNewFolderAddedRooted;
			}
			else
			{
				foreach(var folder in _root.Directories)
				{
					var item = new TreeDirectoryListItem(
						folder,
						TreeDirectoryListItemType.ShowFoldersOnly,
						OnSubItemActivated);
					item.Activated += OnItemActivated;
					_itemHost.Add(item);
				}
				_root.DirectoryAdded += OnNewFolderAdded;
			}
		}

		private void OnItemActivated(object sender, EventArgs e)
		{
			var handler = ItemActivated;
			if(handler != null)
			{
				var listItem = (CustomListBoxItem)(sender);
				var data = ((ITreeItemListItem)listItem).TreeItem;
				handler(this, new BoundItemActivatedEventArgs<TreeItem>(listItem, data));
			}
		}

		private void OnSubItemActivated(object sender, BoundItemActivatedEventArgs<TreeItem> e)
		{
			InvokeItemActivated(e.Item, e.Object);
		}

		public TreeDirectory Root
		{
			get { return _root; }
		}

		private void OnNewFolderAdded(object sender, TreeDirectoryEventArgs e)
		{
			var item = new TreeDirectoryListItem(e.Folder, TreeDirectoryListItemType.ShowFoldersOnly);
			_itemHost.AddSafe(item);
			item.Activated += OnItemActivated;
		}

		private void OnNewFolderAddedRooted(object sender, TreeDirectoryEventArgs e)
		{
			var item = new TreeDirectoryListItem(e.Folder, TreeDirectoryListItemType.ShowFoldersOnly);
			_rootItem.Items.AddSafe(item);
			item.Activated += OnItemActivated;
		}

		#endregion

		public void Dispose()
		{
			_root.DirectoryAdded -= OnNewFolderAdded;
			_root.DirectoryAdded -= OnNewFolderAddedRooted;
			_itemHost.Clear();
		}
	}
}
