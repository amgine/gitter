namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

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

		protected override Bitmap GetBitmapIcon()
		{
			return ImgIcon;
		}

		protected override FileSize? GetSize()
		{
			return null;
		}

		protected override string GetItemType()
		{
			return "";// Utility.GetFileType(Data.FullPath, true, Data.Status == GitObjectStatus.Removed);
		}
	}
}
