namespace gitter.Git.Gui.Views
{
	using System;
	using System.Linq;
	using System.IO;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	partial class TreeView : GitViewBase
	{
		private ITreeSource _treeSource;
		private Tree _wTree;
		private TreeDirectory _currentDirectory;

		private TreeToolbar _toolBar;

		public event EventHandler CurrentDirectoryChanged;

		private sealed class TreeMenu : ContextMenuStrip
		{
			public TreeMenu(TreeDirectoryListItem item)
			{
				if(item == null) throw new ArgumentNullException("item");

				Items.Add(GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(item));
				Items.Add(GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(item));
			}
		}

		public TreeView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.TreeViewGuid, gui, parameters)
		{
			InitializeComponent();

			_directoryTree.Columns.ShowAll(column => column.Id == (int)ColumnId.Name);
			_directoryTree.Columns[0].SizeMode = ColumnSizeMode.Auto;
			_treeContent.Columns.ShowAll(column => column.Id == (int)ColumnId.Name);
			_treeContent.Columns[0].SizeMode = ColumnSizeMode.Auto;

			_directoryTree.SelectionChanged += OnDirectoryTreeSelectionChanged;
			_directoryTree.ItemContextMenuRequested += (sender, e) =>
				{
					var menu = new TreeMenu((TreeDirectoryListItem)e.Item);
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
				};
			_directoryTree.PreviewKeyDown += OnKeyDown;

			_treeContent.ItemContextMenuRequested += OnContextMenuRequested;
			_treeContent.SelectionChanged += OnTreeContentSelectionChanged;
			_treeContent.PreviewKeyDown += OnKeyDown;

			Text = Resources.StrTree + " " + ((ITreeSource)parameters["tree"]).DisplayName;

			AddTopToolStrip(_toolBar = new TreeToolbar(this));
		}

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

		private void OnContextMenuRequested(object sender, ItemContextMenuRequestEventArgs e)
		{
			var rts = Parameters["tree"] as RevisionTreeSource;
			if(rts != null)
			{
				var file = ((ITreeItemListItem)e.Item).TreeItem as TreeFile;
				if(file != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.Add(GuiItemFactory.GetExtractAndOpenFileItem<ToolStripMenuItem>(_wTree, file.RelativePath));
					menu.Items.Add(GuiItemFactory.GetExtractAndOpenFileWithItem<ToolStripMenuItem>(_wTree, file.RelativePath));
					menu.Items.Add(new ToolStripSeparator());
					var c = new ToolStripMenuItem(Resources.StrCopyToClipboard);
					c.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFileName, file.Name));
					c.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, file.RelativePath));
					c.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, file.FullPath));
					menu.Items.Add(c);
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(GuiItemFactory.GetBlameItem<ToolStripMenuItem>(rts.Revision, file));
					menu.Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(rts.Revision, file.RelativePath));
					menu.Items.Add(GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(rts.Revision, file.RelativePath));
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is document.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is document; otherwise, <c>false</c>.
		/// </value>
		public override bool IsDocument
		{
			get { return true; }
		}

		public TreeDirectory CurrentDirectory
		{
			get { return _currentDirectory; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				if(_currentDirectory != value)
				{
					var item = FindDirectoryEntry(value);
					if(item == null) throw new ArgumentException("value");
					item.FocusAndSelect();
					_currentDirectory = value;
					CurrentDirectoryChanged.Raise(this);
				}
			}
		}

		private void OnDirectoryTreeSelectionChanged(object sender, EventArgs e)
		{
			if(_directoryTree.SelectedItems.Count != 0)
			{
				var treeItem = (_directoryTree.SelectedItems[0] as TreeDirectoryListItem);
				if(_currentDirectory != treeItem.Data)
				{
					_currentDirectory = treeItem.Data;
					_treeContent.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryContent);
					CurrentDirectoryChanged.Raise(this);
				}
			}
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			_treeSource = (ITreeSource)parameters["tree"];
			_wTree = _treeSource.GetTree();
			_currentDirectory = _wTree.Root;
			CurrentDirectoryChanged.Raise(this);
			_directoryTree.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryTree);
			_treeContent.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryContent);
			Text = Resources.StrTree + " " + _treeSource.DisplayName;
		}

		private TreeDirectoryListItem FindDirectoryEntry(TreeDirectory folder)
		{
			return FindDirectoryEntry((TreeDirectoryListItem)_directoryTree.Items[0], folder);
		}

		private static TreeDirectoryListItem FindDirectoryEntry(TreeDirectoryListItem root, TreeDirectory folder)
		{
			if(root.Data == folder) return root;
			foreach(TreeDirectoryListItem item in root.Items)
			{
				var subSearch = FindDirectoryEntry(item, folder);
				if(subSearch != null) return subSearch;
			}
			return null;
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as TreeFileListItem;
			if(item != null)
			{
				_wTree.OpenFile(item.Data.RelativePath);
			}
			else
			{
				var folderItem = e.Item as TreeDirectoryListItem;
				if(folderItem != null)
				{
					var directoryEntry = FindDirectoryEntry(folderItem.Data);
					if(directoryEntry != null)
					{
						if(directoryEntry.IsSelected)
							_treeContent.SetTree(folderItem.Data, TreeListBoxMode.ShowDirectoryContent);
						else
							directoryEntry.FocusAndSelect();
					}
					else
					{
						_treeContent.SetTree(folderItem.Data, TreeListBoxMode.ShowDirectoryContent);
					}
					_currentDirectory = folderItem.Data;
					CurrentDirectoryChanged.Raise(this);
				}
			}
		}

		protected override void AttachToRepository(Repository repository)
		{
			ApplyParameters(Parameters);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_directoryTree.Clear();
			_treeContent.Clear();
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgFolderTree"]; }
		}

		public override void RefreshContent()
		{
			if(_wTree != null) _wTree.Refresh();
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
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
			if(listNode != null)
				_treeContent.LoadViewFrom(listNode);
		}
	}
}
