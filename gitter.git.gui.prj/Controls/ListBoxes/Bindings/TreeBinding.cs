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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;

using gitter.Framework.Controls;

public sealed class TreeBinding : IDisposable
{
	#region Data

	private readonly CustomListBoxItemsCollection _itemHost;
	private HashSet<TreeDirectory>? _trackedDirectories;
	private bool _plain;

	#endregion

	#region Events

	public event EventHandler<ItemContextMenuRequestEventArgs>? ItemContextMenuRequested;

	public event EventHandler<BoundItemActivatedEventArgs<TreeItem>>? ItemActivated;

	private void OnItemContextMenuRequested(ItemContextMenuRequestEventArgs e)
		=> ItemContextMenuRequested?.Invoke(this, e);

	private void OnItemActivated(CustomListBoxItem item, TreeItem treeItem)
		=> ItemActivated?.Invoke(this, new BoundItemActivatedEventArgs<TreeItem>(item, treeItem));

	#endregion

	#region .ctor

	public TreeBinding(CustomListBoxItemsCollection itemHost, TreeDirectory root, bool filesOnly, bool oneLevel)
	{
		Verify.Argument.IsNotNull(itemHost);
		Verify.Argument.IsNotNull(root);

		_itemHost = itemHost;
		Root = root;
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
		foreach(var folder in Root.Directories)
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
		foreach(var commit in Root.Commits)
		{
			var item = new TreeCommitListItem(commit, false);
			item.Activated += OnItemActivated;
			item.ContextMenuRequested += OnItemContextMenuRequested;
			_itemHost.Add(item);
		}
		foreach(var file in Root.Files)
		{
			var item = new TreeFileListItem(file, false);
			item.Activated += OnItemActivated;
			item.ContextMenuRequested += OnItemContextMenuRequested;
			_itemHost.Add(item);
		}
		Root.DirectoryAdded += OnDirectoryAdded;
		Root.CommitAdded += OnCommitAdded;
		Root.FileAdded += OnFileAdded;
	}

	private void InitPlain(bool oneLevel)
	{
		_trackedDirectories = [];
		_itemHost.Clear();
		_itemHost.Comparison = TreeItemListItem.CompareByRelativePath;
		InsertFiles(Root, oneLevel);
	}

	private void InsertFiles(TreeDirectory directory, bool oneLevel)
	{
		if(!oneLevel)
		{
			foreach(var subFolder in directory.Directories)
			{
				InsertFiles(subFolder, false);
			}
		}
		foreach(var commit in directory.Commits)
		{
			var item = new TreeCommitListItem(commit, true);
			item.Activated += OnItemActivated;
			item.ContextMenuRequested += OnItemContextMenuRequested;
			_itemHost.AddSafe(item);
		}
		foreach(var file in directory.Files)
		{
			var item = new TreeFileListItem(file, true);
			item.Activated += OnItemActivated;
			item.ContextMenuRequested += OnItemContextMenuRequested;
			_itemHost.AddSafe(item);
		}
		_trackedDirectories?.Add(directory);
		if(!oneLevel)
		{
			directory.DirectoryAdded += OnDirectoryAdded2;
		}
		directory.FileAdded += OnFileAdded;
		directory.CommitAdded += OnCommitAdded;
		directory.Deleted += OnDirectoryDeleted;
	}

	private void OnItemActivated(object? sender, EventArgs e)
	{
		var handler = ItemActivated;
		if(handler is not null)
		{
			var listItem = (CustomListBoxItem)sender!;
			var data     = ((ITreeItemListItem)listItem).TreeItem;
			handler(this, new BoundItemActivatedEventArgs<TreeItem>(listItem, data));
		}
	}

	private void OnItemContextMenuRequested(object? sender, ItemContextMenuRequestEventArgs e)
	{
		OnItemContextMenuRequested(e);
	}

	private void OnSubItemActivated(object? sender, BoundItemActivatedEventArgs<TreeItem> e)
	{
		OnItemActivated(e.Item, e.Object);
	}

	public TreeDirectory Root { get; }

	private void OnDirectoryDeleted(object? sender, EventArgs e)
	{
		var directory = (TreeDirectory)sender!;
		directory.DirectoryAdded -= OnDirectoryAdded2;
		directory.FileAdded -= OnFileAdded;
		directory.CommitAdded -= OnCommitAdded;
		directory.Deleted -= OnDirectoryDeleted;
		_trackedDirectories?.Remove(directory);
	}

	private void OnDirectoryAdded(object? sender, TreeDirectoryEventArgs e)
	{
		var item = new TreeDirectoryListItem(e.Folder, OnSubItemActivated, OnItemContextMenuRequested);
		item.Activated += OnItemActivated;
		item.ContextMenuRequested += OnItemContextMenuRequested;
		_itemHost.AddSafe(item);
	}

	private void OnDirectoryAdded2(object? sender, TreeDirectoryEventArgs e)
	{
		InsertFiles(e.Folder, false);
	}

	private void OnFileAdded(object? sender, TreeFileEventArgs e)
	{
		var item = new TreeFileListItem(e.File, _plain);
		item.Activated += OnItemActivated;
		item.ContextMenuRequested += OnItemContextMenuRequested;
		_itemHost.AddSafe(item);
	}

	private void OnCommitAdded(object? sender, TreeCommitEventArgs e)
	{
		var item = new TreeCommitListItem(e.Object, _plain);
		item.Activated += OnItemActivated;
		item.ContextMenuRequested += OnItemContextMenuRequested;
		_itemHost.AddSafe(item);
	}

	#endregion

	public void Dispose()
	{
		Root.FileAdded -= OnFileAdded;
		Root.CommitAdded -= OnCommitAdded;
		Root.DirectoryAdded -= OnDirectoryAdded;
		Root.DirectoryAdded -= OnDirectoryAdded2;
		Root.Deleted -= OnDirectoryDeleted;
		if(_trackedDirectories is not null)
		{
			foreach(var directory in _trackedDirectories)
			{
				directory.DirectoryAdded -= OnDirectoryAdded2;
				directory.FileAdded -= OnFileAdded;
				directory.CommitAdded -= OnCommitAdded;
				directory.Deleted -= OnDirectoryDeleted;
			}
			_trackedDirectories = null;
		}
		_itemHost.Clear();
	}
}
