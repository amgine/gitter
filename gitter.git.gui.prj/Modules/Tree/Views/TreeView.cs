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
using gitter.Framework.Layout;
using gitter.Framework.Services;

using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Controls.ListBoxes;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
partial class TreeView : GitViewBase
{
	readonly struct ViewControls
	{
		public readonly TreeListBox _treeContent;
		public readonly TreeListBox _directoryTree;
		public readonly TreeToolbar _toolBar;

		public ViewControls(IGitterStyle? style, TreeView view)
		{
			style ??= GitterApplication.Style;

			_toolBar = new(view);
			_treeContent = new()
			{
				Style         = style,
				BorderStyle   = BorderStyle.None,
				HeaderStyle   = HeaderStyle.Hidden,
				ShowTreeLines = true
			};
			_directoryTree = new()
			{
				Style         = style,
				BorderStyle   = BorderStyle.None,
				HeaderStyle   = HeaderStyle.Hidden,
				ShowTreeLines = true
			};
		}

		public void Localize()
		{
			_treeContent.Text = "Tree is empty";
			_directoryTree.Text = "Tree is empty";
		}

		public void Layout(Control parent)
		{
			Grid grid;
			Panel grip;

			_ = new ControlLayout(parent)
			{
				Content = grid = new(
					rows:
					[
						LayoutConstants.ToolbarHeight,
						SizeSpec.Nothing(),
						SizeSpec.Everything(),
					],
					columns:
					[
						SizeSpec.Absolute(200),
						LayoutConstants.RowSpacing,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_toolBar,       marginOverride: LayoutConstants.NoMargin), column: 0, row: 2, columnSpan: 3),
						new GridContent(new ControlContent(_directoryTree, marginOverride: LayoutConstants.NoMargin), column: 0, row: 2),
						new GridContent(new ControlContent(grip = new(),   marginOverride: LayoutConstants.NoMargin), column: 1, row: 2),
						new GridContent(new ControlContent(_treeContent,   marginOverride: LayoutConstants.NoMargin), column: 2, row: 2),
					]),
			};

			_toolBar.Parent       = parent;
			_directoryTree.Parent = parent;
			grip.Parent           = parent;
			_treeContent.Parent   = parent;

			var tabIndex = 0;
			_toolBar.TabIndex       = tabIndex++;
			_directoryTree.TabIndex = tabIndex++;
			grip.TabIndex           = tabIndex++;
			_treeContent.TabIndex   = tabIndex++;

			grip.Cursor = Cursors.SizeWE;

			_ = new HorizontalResizer(grip, grid.Columns[0], 1, minWidth: DpiBoundValue.ScaleX(100));
		}
	}

	#region Data

	private readonly ViewControls _controls;
	private ITreeSource? _treeSource;
	private TreeListsBinding? _dataSource;
	private TreeDirectory? _currentDirectory;

	#endregion

	#region Events

	private static readonly object CurrentDirectoryChangedEvent = new();

	public event EventHandler CurrentDirectoryChanged
	{
		add    => Events.AddHandler    (CurrentDirectoryChangedEvent, value);
		remove => Events.RemoveHandler (CurrentDirectoryChangedEvent, value);
	}

	protected virtual void OnCurrentDirectoryChanged(EventArgs e)
		=> ((EventHandler?)Events[CurrentDirectoryChangedEvent])?.Invoke(this, e);

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
		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		Name                = nameof(TreeView);
		_controls = new(GitterApplication.Style, this);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._treeContent.ItemActivated += OnItemActivated;

		_controls._directoryTree.Columns.ShowAll(static column => column.Id is (int)ColumnId.Name);
		_controls._directoryTree.Columns[0].SizeMode = ColumnSizeMode.Auto;
		_controls._treeContent.Columns.ShowAll(static column => column.Id is (int)ColumnId.Name or (int)ColumnId.Size);
		_controls._treeContent.Columns.GetById((int)ColumnId.Name)!.SizeMode = ColumnSizeMode.Auto;

		_controls._directoryTree.SelectionChanged += OnDirectoryTreeSelectionChanged;
		_controls._directoryTree.ItemContextMenuRequested +=
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
		_controls._directoryTree.PreviewKeyDown += OnKeyDown;

		_controls._treeContent.ItemContextMenuRequested += OnContextMenuRequested;
		_controls._treeContent.SelectionChanged += OnTreeContentSelectionChanged;
		_controls._treeContent.PreviewKeyDown += OnKeyDown;
	}

	#endregion

	#region Properties

	private TreeListsBinding? DataSource
	{
		get => _dataSource;
		set
		{
			if(_dataSource == value) return;

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

	/// <inheritdoc/>
	public override bool IsDocument => true;

	public override IImageProvider ImageProvider => Icons.FolderTree;

	public TreeDirectory? CurrentDirectory
	{
		get => _currentDirectory;
		set
		{
			if(_currentDirectory == value) return;

			var item = value is not null ? FindDirectoryEntry(value) : default;
			item?.FocusAndSelect();
			_currentDirectory = value;
			OnCurrentDirectoryChanged(EventArgs.Empty);
		}
	}

	#endregion

	#region Methods

	private void OnTreeContentSelectionChanged(object? sender, EventArgs e)
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

	private ContextMenuStrip? GetFileContextMenu(IRevisionPointer revision, TreeFile file)
	{
		Assert.IsNotNull(revision);
		Assert.IsNotNull(file);

		if(DataSource?.Data is null) return default;

		var menu = new ContextMenuStrip
		{
			Renderer = GitterApplication.Style.ToolStripRenderer,
		};
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			[
				GuiItemFactory.GetExtractAndOpenFileItem<ToolStripMenuItem>(DataSource.Data, file.RelativePath),
				GuiItemFactory.GetExtractAndOpenFileWithItem<ToolStripMenuItem>(DataSource.Data, file.RelativePath),
				GuiItemFactory.GetSaveAsItem<ToolStripMenuItem>(DataSource.Data, file.RelativePath),
				new ToolStripSeparator(),
				new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
					[
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFileName, file.Name),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, file.RelativePath),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, file.FullPath),
					]),
				new ToolStripSeparator(),
				factory.GetBlameItem<ToolStripMenuItem>(revision, file),
				factory.GetPathHistoryItem<ToolStripMenuItem>(revision, file),
				new ToolStripSeparator(),
				factory.GetCheckoutPathItem<ToolStripMenuItem>(revision, file),
			]);
		return menu;
	}

	private ContextMenuStrip GetDirectoryContextMenu(CustomListBoxItem item, IRevisionPointer revision, TreeDirectory directory)
	{
		Assert.IsNotNull(revision);
		Assert.IsNotNull(directory);

		var menu = new ContextMenuStrip
		{
			Renderer = GitterApplication.Style.ToolStripRenderer,
		};
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			[
				new ToolStripMenuItem(Resources.StrOpen, null, (_, _) => item.Activate()),
				factory.GetPathHistoryItem<ToolStripMenuItem>(revision, directory),
				new ToolStripSeparator(),
				factory.GetCheckoutPathItem<ToolStripMenuItem>(revision, directory),
			]);
		return menu;
	}

	private ContextMenuStrip GetCommitContextMenu(IRevisionPointer revision, TreeCommit commit)
	{
		Assert.IsNotNull(revision);
		Assert.IsNotNull(commit);

		var menu = new ContextMenuStrip
		{
			Renderer = GitterApplication.Style.ToolStripRenderer,
		};
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			[
				factory.GetPathHistoryItem<ToolStripMenuItem>(revision, commit),
				new ToolStripSeparator(),
				new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
					[
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, commit.Name),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, commit.RelativePath),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, commit.FullPath),
					]),
				new ToolStripSeparator(),
				factory.GetCheckoutPathItem<ToolStripMenuItem>(revision, commit),
			]);
		return menu;
	}

	private void OnContextMenuRequested(object? sender, ItemContextMenuRequestEventArgs e)
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
		if(menu is not null)
		{
			Utility.MarkDropDownForAutoDispose(menu);
			e.ContextMenu = menu;
			e.OverrideDefaultMenu = true;
		}
	}

	private void UpdateCurrentDirectory(TreeDirectory? directory)
	{
		if(_currentDirectory != directory)
		{
			_currentDirectory = directory;
			OnCurrentDirectoryChanged(EventArgs.Empty);
		}
	}

	private void OnDirectoryTreeSelectionChanged(object? sender, EventArgs e)
	{
		if(_controls._directoryTree.SelectedItems.Count != 0)
		{
			var treeItem = (TreeDirectoryListItem)_controls._directoryTree.SelectedItems[0];
			if(_currentDirectory != treeItem.DataContext)
			{
				_currentDirectory = treeItem.DataContext;
				_controls._treeContent.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryContent);
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
				DataSource = new TreeListsBinding(_treeSource, _controls._directoryTree, _controls._treeContent);
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

	private void OnTreeChanged(object? sender, EventArgs e)
	{
		UpdateCurrentDirectory(DataSource?.Data?.Root);
	}

	private TreeDirectoryListItem? FindDirectoryEntry(TreeDirectory folder)
	{
		return FindDirectoryEntry((TreeDirectoryListItem)_controls._directoryTree.Items[0], folder);
	}

	private static TreeDirectoryListItem? FindDirectoryEntry(TreeDirectoryListItem root, TreeDirectory folder)
	{
		if(root.DataContext == folder) return root;
		foreach(TreeDirectoryListItem item in root.Items)
		{
			var subSearch = FindDirectoryEntry(item, folder);
			if(subSearch is not null) return subSearch;
		}
		return null;
	}

	private void OnItemActivated(object? sender, ItemEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.Item)
		{
			case TreeFileListItem fileItem when DataSource is not null:
				var fileName = DataSource.Data?.ExtractBlobToTemporaryFile(fileItem.DataContext.RelativePath);
				if(fileName is { Length: not 0 })
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
						_controls._treeContent.SetTree(directoryItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
					}
					else
					{
						directoryEntry.FocusAndSelect();
					}
				}
				else
				{
					_controls._treeContent.SetTree(directoryItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
				}
				_currentDirectory = directoryItem.DataContext;
				OnCurrentDirectoryChanged(EventArgs.Empty);
				break;
		}
	}

	private static void OnFileViewerProcessExited(object? sender, EventArgs e)
	{
		var process = (Process)sender!;
		var path = process.StartInfo.FileName;
		try
		{
			File.Delete(path);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			LoggingService.Global.Warning(exc, "Failed to remove temporary file: '{0}'", path);
		}
		process.Dispose();
	}

	protected override void DetachFromRepository(Repository repository)
	{
		_controls._directoryTree.Clear();
		_controls._treeContent.Clear();
	}

	public override void RefreshContent() => DataSource?.ReloadData();

	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		OnKeyDown(this, e);
		base.OnPreviewKeyDown(e);
	}

	private void OnKeyDown(object? sender, PreviewKeyDownEventArgs e)
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
		_controls._treeContent.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);
		var listNode = section.TryGetSection("TreeList");
		if(listNode is not null)
		{
			_controls._treeContent.LoadViewFrom(listNode);
		}
	}

	#endregion
}
