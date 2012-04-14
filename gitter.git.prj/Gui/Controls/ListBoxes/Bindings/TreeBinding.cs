namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public sealed class TreeBinding : IDisposable
	{
		#region Data

		private readonly CustomListBoxItemsCollection _itemHost;
		private readonly TreeDirectory _root;
		private HashSet<TreeDirectory> _trackedDirectories;
		private bool _plain;

		#endregion

		#region Events

		public event EventHandler<ItemContextMenuRequestEventArgs> ItemContextMenuRequested;

		public event EventHandler<BoundItemActivatedEventArgs<TreeItem>> ItemActivated;

		private void InvokeItemContextMenuRequested(ItemContextMenuRequestEventArgs e)
		{
			var handler = ItemContextMenuRequested;
			if(handler != null)
			{
				handler(this, e);
			}
		}

		private void InvokeItemActivated(CustomListBoxItem item, TreeItem treeItem)
		{
			var handler = ItemActivated;
			if(handler != null)
			{
				handler(this, new BoundItemActivatedEventArgs<TreeItem>(item, treeItem));
			}
		}

		#endregion

		#region .ctor

		public TreeBinding(CustomListBoxItemsCollection itemHost, TreeDirectory root, bool filesOnly, bool oneLevel)
		{
			if(itemHost == null) throw new ArgumentNullException("itemHost");
			if(root == null) throw new ArgumentNullException("root");

			_itemHost = itemHost;
			_root = root;
			_plain = filesOnly;

			if(filesOnly)
			{
				InitPlain(oneLevel);
			}
			else
			{
				InitTree(oneLevel);
			}
		}

		public TreeBinding(CustomListBoxItemsCollection itemHost, TreeDirectory root, bool filesOnly)
			: this(itemHost, root, filesOnly, false)
		{
		}


		private void InitTree(bool oneLevel)
		{
			_itemHost.Clear();
			_itemHost.Comparison = TreeItemListItem.CompareByName;
			foreach(var folder in _root.Directories)
			{
				var item = new TreeDirectoryListItem(
					folder,
					oneLevel ?
						TreeDirectoryListItemType.ShowNothing :
						TreeDirectoryListItemType.ShowFilesAndFolders,
					OnSubItemActivated,
					OnItemContextMenuRequested);
				item.Activated += OnItemActivated;
				item.ContextMenuRequested += OnItemContextMenuRequested;
				_itemHost.Add(item);
			}
			foreach(var file in _root.Files)
			{
				var item = new TreeFileListItem(file, false);
				item.Activated += OnItemActivated;
				item.ContextMenuRequested += OnItemContextMenuRequested;
				_itemHost.Add(item);
			}
			_root.DirectoryAdded += OnDirectoryAdded;
			_root.FileAdded += OnFileAdded;
		}

		private void InitPlain(bool oneLevel)
		{
			_trackedDirectories = new HashSet<TreeDirectory>();
			_itemHost.Clear();
			_itemHost.Comparison = TreeItemListItem.CompareByRelativePath;
			InsertFiles(_root, oneLevel);
		}

		private void InsertFiles(TreeDirectory folder, bool oneLevel)
		{
			if(!oneLevel)
			{
				foreach(var subFolder in folder.Directories)
				{
					InsertFiles(subFolder, false);
				}
			}
			foreach(var file in folder.Files)
			{
				var item = new TreeFileListItem(file, true);
				item.Activated += OnItemActivated;
				item.ContextMenuRequested += OnItemContextMenuRequested;
				_itemHost.AddSafe(item);
			}
			_trackedDirectories.Add(folder);
			if(!oneLevel)
			{
				folder.DirectoryAdded += OnDirectoryAdded2;
			}
			folder.FileAdded += OnFileAdded;
			folder.Deleted += OnDirectoryDeleted;
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

		private void OnItemContextMenuRequested(object sender, ItemContextMenuRequestEventArgs e)
		{
			InvokeItemContextMenuRequested(e);
		}

		private void OnSubItemActivated(object sender, BoundItemActivatedEventArgs<TreeItem> e)
		{
			InvokeItemActivated(e.Item, e.Object);
		}

		public TreeDirectory Root
		{
			get { return _root; }
		}

		private void OnDirectoryDeleted(object sender, EventArgs e)
		{
			var directory = (TreeDirectory)sender;
			directory.DirectoryAdded -= OnDirectoryAdded2;
			directory.FileAdded -= OnFileAdded;
			directory.Deleted -= OnDirectoryDeleted;
			_trackedDirectories.Remove(directory);
		}

		private void OnDirectoryAdded(object sender, TreeDirectoryEventArgs e)
		{
			var item = new TreeDirectoryListItem(e.Folder, OnSubItemActivated, OnItemContextMenuRequested);
			item.Activated += OnItemActivated;
			item.ContextMenuRequested += OnItemContextMenuRequested;
			_itemHost.AddSafe(item);
		}

		private void OnDirectoryAdded2(object sender, TreeDirectoryEventArgs e)
		{
			InsertFiles(e.Folder, false);
		}

		private void OnFileAdded(object sender, TreeFileEventArgs e)
		{
			var item = new TreeFileListItem(e.File, _plain);
			item.Activated += OnItemActivated;
			item.ContextMenuRequested += OnItemContextMenuRequested;
			_itemHost.AddSafe(item);
		}

		#endregion

		public void Dispose()
		{
			_root.FileAdded -= OnFileAdded;
			_root.DirectoryAdded -= OnDirectoryAdded;
			_root.DirectoryAdded -= OnDirectoryAdded2;
			_root.Deleted -= OnDirectoryDeleted;
			if(_trackedDirectories != null)
			{
				foreach(var folder in _trackedDirectories)
				{
					folder.DirectoryAdded -= OnDirectoryAdded2;
					folder.FileAdded -= OnFileAdded;
					folder.Deleted -= OnDirectoryDeleted;
				}
				_trackedDirectories = null;
			}
			_itemHost.Clear();
		}
	}
}
