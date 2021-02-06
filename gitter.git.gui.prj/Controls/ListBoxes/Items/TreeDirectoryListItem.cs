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
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class TreeDirectoryListItem : TreeItemListItem<TreeDirectory>
	{
		private static readonly Bitmap ImgIcon = CachedResources.Bitmaps["ImgFolder"];

		private IDisposable _binding;
		private TreeDirectoryListItemType _includeFiles;
		private EventHandler<BoundItemActivatedEventArgs<TreeItem>> _itemActivated;
		private EventHandler<ItemContextMenuRequestEventArgs> _itemContextMenuRequested;

		public TreeDirectoryListItem(
			TreeDirectory folder,
			TreeDirectoryListItemType includeFiles,
			EventHandler<BoundItemActivatedEventArgs<TreeItem>> onItemActivated = null,
			EventHandler<ItemContextMenuRequestEventArgs> onItemContextMenuRequested = null)
			: base(folder, false)
		{
			_includeFiles = includeFiles;
			_itemActivated = onItemActivated;
			_itemContextMenuRequested = onItemContextMenuRequested;
		}

		public TreeDirectoryListItem(
			TreeDirectory folder,
			EventHandler<BoundItemActivatedEventArgs<TreeItem>> onItemActivated = null,
			EventHandler<ItemContextMenuRequestEventArgs> onItemContextMenuRequested = null)
			: this(folder, TreeDirectoryListItemType.ShowFilesAndFolders, onItemActivated, onItemContextMenuRequested)
		{
		}

		protected override void OnListBoxAttached()
		{
			switch(_includeFiles)
			{
				case TreeDirectoryListItemType.ShowFilesAndFolders:
					{
						var binding = new TreeBinding(Items, DataContext, false);
						if(_itemActivated != null)
						{
							binding.ItemActivated += _itemActivated;
							binding.ItemContextMenuRequested += _itemContextMenuRequested;
						}
						_binding = binding;
					}
					break;
				case TreeDirectoryListItemType.ShowFoldersOnly:
					{
						var binding = new TreeDirectoriesBinding(Items, DataContext, false);
						if(_itemActivated != null)
						{
							binding.ItemActivated += _itemActivated;
						}
						_binding = binding;
					}
					break;
			}
			base.OnListBoxAttached();
		}

		protected override void OnListBoxDetached()
		{
			if(_binding != null)
			{
				_binding.Dispose();
				_binding = null;
			}
			base.OnListBoxDetached();
		}

		protected override Bitmap GetBitmapIcon() => ImgIcon;

		protected override FileSize? GetSize() => null;

		protected override string GetItemType() => "";
	}
}
