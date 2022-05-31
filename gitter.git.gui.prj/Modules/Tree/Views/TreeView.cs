#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;
using gitter.Framework.Services;

using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Controls.ListBoxes;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
partial class TreeView : GitViewBase
{
	#region Data

	private ITreeSource _treeSource;
	private TreeListsBinding _dataSource;
	private TreeDirectory _currentDirectory;
	private TreeToolbar _toolBar;

	#endregion

	#region Events

	private static readonly object CurrentDirectoryChangedEvent = new();

	public event EventHandler CurrentDirectoryChanged
	{
		add    => Events.AddHandler    (CurrentDirectoryChangedEvent, value);
		remove => Events.RemoveHandler (CurrentDirectoryChangedEvent, value);
	}

	protected virtual void OnCurrentDirectoryChanged(EventArgs e)
		=> ((EventHandler)Events[CurrentDirectoryChangedEvent])?.Invoke(this, e);

	#endregion

	#region Helpers

	private sealed class TreeMenu : ContextMenuStrip
	{
		public TreeMenu(ITreeSource treeSource, TreeDirectoryListItem item)
		{
			Verify.Argument.IsNotNull(item);

			var dpiBindings = new DpiBindings(this);
			var factory     = new GuiItemFactory(dpiBindings);

			Items.Add(GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(item));
			Items.Add(GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(item));

			if(treeSource is not null)
			{
				Items.Add(new ToolStripSeparator());
				Items.Add(factory.GetPathHistoryItem<ToolStripMenuItem>(treeSource.Revision, item.DataContext));
				Items.Add(new ToolStripSeparator());
				Items.Add(factory.GetCheckoutPathItem<ToolStripMenuItem>(treeSource.Revision, item.DataContext));
			}
		}
	}

	#endregion

	#region .ctor

	public TreeView(GuiProvider gui)
		: base(Guids.TreeViewGuid, gui)
	{
		InitializeComponent();

		_splitContainer.BackColor = GitterApplication.Style.Colors.WorkArea;
		_splitContainer.Panel1.BackColor = GitterApplication.Style.Colors.Window;
		_splitContainer.Panel2.BackColor = GitterApplication.Style.Colors.Window;

		_directoryTree.Columns.ShowAll(column => column.Id == (int)ColumnId.Name);
		_directoryTree.Columns[0].SizeMode = ColumnSizeMode.Auto;
		_treeContent.Columns.ShowAll(column => column.Id == (int)ColumnId.Name || column.Id == (int)ColumnId.Size);
		_treeContent.Columns.GetById((int)ColumnId.Name).SizeMode = ColumnSizeMode.Auto;

		_directoryTree.SelectionChanged += OnDirectoryTreeSelectionChanged;
		_directoryTree.ItemContextMenuRequested +=
			(sender, e) =>
			{
				if(ViewModel is TreeViewModel vm && vm.TreeSource is not null)
				{
					var menu = new TreeMenu(vm.TreeSource, (TreeDirectoryListItem)e.Item);
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
				}
			};
		_directoryTree.PreviewKeyDown += OnKeyDown;

		_treeContent.ItemContextMenuRequested += OnContextMenuRequested;
		_treeContent.SelectionChanged += OnTreeContentSelectionChanged;
		_treeContent.PreviewKeyDown += OnKeyDown;

		AddTopToolStrip(_toolBar = new TreeToolbar(this));
	}

	#endregion

	#region Properties

	private TreeListsBinding DataSource
	{
		get => _dataSource;
		set
		{
			if(_dataSource != value)
			{
				if(_dataSource is not null)
				{
					_dataSource.DataChanged -= OnTreeChanged;
					_dataSource.Dispose();
				}
				_dataSource = value;
				if(_dataSource is not null)
				{
					_dataSource.DataChanged += OnTreeChanged;
					_dataSource.ReloadData();
				}
			}
		}
	}

	/// <inheritdoc/>
	public override bool IsDocument => true;

	public override IImageProvider ImageProvider => Icons.FolderTree;

	public TreeDirectory CurrentDirectory
	{
		get => _currentDirectory;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_currentDirectory != value)
			{
				var item = FindDirectoryEntry(value);
				if(item is null) throw new ArgumentException(nameof(value));
				item.FocusAndSelect();
				_currentDirectory = value;
				OnCurrentDirectoryChanged(EventArgs.Empty);
			}
		}
	}

	#endregion

	#region Methods

	private void OnTreeContentSelectionChanged(object sender, EventArgs e)
	{
		//var rev = Parameters["tree"] as RevisionTreeSource;
		//if(rev != null)
		//{
		//    var items = _treeContent.SelectedItems;
		//    if(items.Count == 0)
		//    {
		//        ShowContextualDiffTool(null);
		//    }
		//    else
		//    {
		//        ShowContextualDiffTool(new RevisionChangesDiffSource(rev.Revision,
		//            items.Cast<IWorktreeListItem>().Select(item => item.WorktreeItem.RelativePath).ToList()));
		//    }
		//}
	}

	private ContextMenuStrip GetFileContextMenu(IRevisionPointer revision, TreeFile file)
	{
		Assert.IsNotNull(revision);
		Assert.IsNotNull(file);

		var menu        = new ContextMenuStrip();
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			new ToolStripItem[]
			{
				GuiItemFactory.GetExtractAndOpenFileItem<ToolStripMenuItem>(DataSource.Data, file.RelativePath),
				GuiItemFactory.GetExtractAndOpenFileWithItem<ToolStripMenuItem>(DataSource.Data, file.RelativePath),
				GuiItemFactory.GetSaveAsItem<ToolStripMenuItem>(DataSource.Data, file.RelativePath),
				new ToolStripSeparator(),
				new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
					new ToolStripItem[]
					{
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFileName, file.Name),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, file.RelativePath),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, file.FullPath),
					}),
				new ToolStripSeparator(),
				factory.GetBlameItem<ToolStripMenuItem>(revision, file),
				factory.GetPathHistoryItem<ToolStripMenuItem>(revision, file),
				new ToolStripSeparator(),
				factory.GetCheckoutPathItem<ToolStripMenuItem>(revision, file),
			});
		return menu;
	}

	private ContextMenuStrip GetDirectoryContextMenu(CustomListBoxItem item, IRevisionPointer revision, TreeDirectory directory)
	{
		Assert.IsNotNull(revision);
		Assert.IsNotNull(directory);

		var menu        = new ContextMenuStrip();
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			new ToolStripItem[]
			{
				new ToolStripMenuItem(Resources.StrOpen, null, (_, _) => item.Activate()),
				factory.GetPathHistoryItem<ToolStripMenuItem>(revision, directory),
				new ToolStripSeparator(),
				factory.GetCheckoutPathItem<ToolStripMenuItem>(revision, directory),
			});
		return menu;
	}

	private ContextMenuStrip GetCommitContextMenu(IRevisionPointer revision, TreeCommit commit)
	{
		Assert.IsNotNull(revision);
		Assert.IsNotNull(commit);

		var menu        = new ContextMenuStrip();
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			new ToolStripItem[]
			{
				factory.GetPathHistoryItem<ToolStripMenuItem>(revision, commit),
				new ToolStripSeparator(),
				new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
					new ToolStripItem[]
					{
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, commit.Name),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, commit.RelativePath),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, commit.FullPath),
					}),
				new ToolStripSeparator(),
				factory.GetCheckoutPathItem<ToolStripMenuItem>(revision, commit),
			});
		return menu;
	}

	private void OnContextMenuRequested(object sender, ItemContextMenuRequestEventArgs e)
	{
		Assert.IsNotNull(e);

		var rts = (ViewModel as TreeViewModel)?.TreeSource;
		if(rts == null) return;

		var menu = ((ITreeItemListItem)e.Item).TreeItem switch
		{
			TreeFile      file      => GetFileContextMenu(rts.Revision, file),
			TreeDirectory directory => GetDirectoryContextMenu(e.Item, rts.Revision, directory),
			TreeCommit    commit    => GetCommitContextMenu(rts.Revision, commit),
			_ => default,
		};
		if(menu != null)
		{
			Utility.MarkDropDownForAutoDispose(menu);
			e.ContextMenu = menu;
			e.OverrideDefaultMenu = true;
		}
	}

	private void UpdateCurrentDirectory(TreeDirectory directory)
	{
		if(_currentDirectory != directory)
		{
			_currentDirectory = directory;
			OnCurrentDirectoryChanged(EventArgs.Empty);
		}
	}

	private void OnDirectoryTreeSelectionChanged(object sender, EventArgs e)
	{
		if(_directoryTree.SelectedItems.Count != 0)
		{
			var treeItem = (TreeDirectoryListItem)_directoryTree.SelectedItems[0];
			if(_currentDirectory != treeItem.DataContext)
			{
				_currentDirectory = treeItem.DataContext;
				_treeContent.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryContent);
				OnCurrentDirectoryChanged(EventArgs.Empty);
			}
		}
	}

	protected override void AttachViewModel(object viewModel)
	{
		base.AttachViewModel(viewModel);

		if(viewModel is TreeViewModel vm)
		{
			_treeSource = vm.TreeSource;
			if(_treeSource != null)
			{
				Text = Resources.StrTree + " " + _treeSource.DisplayName;
				DataSource = new TreeListsBinding(_treeSource, _directoryTree, _treeContent);
			}
			else
			{
				Text = Resources.StrTree;
				DataSource = null;
			}
		}
	}

	protected override void DetachViewModel(object viewModel)
	{
		base.DetachViewModel(viewModel);

		if(viewModel is TreeViewModel)
		{
			_treeSource = null;
			Text = Resources.StrTree;
			DataSource = null;
		}
	}

	private void OnTreeChanged(object sender, EventArgs e)
	{
		UpdateCurrentDirectory(DataSource?.Data?.Root);
	}

	private TreeDirectoryListItem FindDirectoryEntry(TreeDirectory folder)
	{
		return FindDirectoryEntry((TreeDirectoryListItem)_directoryTree.Items[0], folder);
	}

	private static TreeDirectoryListItem FindDirectoryEntry(TreeDirectoryListItem root, TreeDirectory folder)
	{
		if(root.DataContext == folder) return root;
		foreach(TreeDirectoryListItem item in root.Items)
		{
			var subSearch = FindDirectoryEntry(item, folder);
			if(subSearch is not null) return subSearch;
		}
		return null;
	}

	private void OnItemActivated(object sender, ItemEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.Item)
		{
			case TreeFileListItem fileItem:
				var fileName = DataSource.Data.ExtractBlobToTemporaryFile(fileItem.DataContext.RelativePath);
				if(!string.IsNullOrWhiteSpace(fileName))
				{
					var process = Utility.CreateProcessFor(fileName);
					process.EnableRaisingEvents = true;
					process.Exited += OnFileViewerProcessExited;
					process.Start();
				}
				break;
			case TreeDirectoryListItem directoryItem:
				var directoryEntry = FindDirectoryEntry(directoryItem.DataContext);
				if(directoryEntry is not null)
				{
					if(directoryEntry.IsSelected)
					{
						_treeContent.SetTree(directoryItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
					}
					else
					{
						directoryEntry.FocusAndSelect();
					}
				}
				else
				{
					_treeContent.SetTree(directoryItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
				}
				_currentDirectory = directoryItem.DataContext;
				OnCurrentDirectoryChanged(EventArgs.Empty);
				break;
		}
	}

	private static void OnFileViewerProcessExited(object sender, EventArgs e)
	{
		var process = (Process)sender;
		var path = process.StartInfo.FileName;
		try
		{
			File.Delete(path);
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
			LoggingService.Global.Warning(exc, "Failed to remove temporary file: '{0}'", path);
		}
		process.Dispose();
	}

	protected override void DetachFromRepository(Repository repository)
	{
		_directoryTree.Clear();
		_treeContent.Clear();
	}

	public override void RefreshContent() => DataSource?.ReloadData();

	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		OnKeyDown(this, e);
		base.OnPreviewKeyDown(e);
	}

	private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.KeyCode)
		{
			case Keys.F5:
				RefreshContent();
				e.IsInputKey = true;
				break;
		}
	}

	protected override void SaveMoreViewTo(Section section)
	{
		base.SaveMoreViewTo(section);
		var listNode = section.GetCreateSection("TreeList");
		_treeContent.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);
		var listNode = section.TryGetSection("TreeList");
		if(listNode is not null)
		{
			_treeContent.LoadViewFrom(listNode);
		}
	}

	#endregion
}
