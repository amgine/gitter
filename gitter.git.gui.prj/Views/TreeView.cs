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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
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

		private static readonly object CurrentDirectoryChangedEvent = new object();

		public event EventHandler CurrentDirectoryChanged
		{
			add { Events.AddHandler(CurrentDirectoryChangedEvent, value); }
			remove { Events.RemoveHandler(CurrentDirectoryChangedEvent, value); }
		}

		protected virtual void OnCurrentDirectoryChanged()
		{
			var handler = (EventHandler)Events[CurrentDirectoryChangedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Helpers

		private sealed class TreeMenu : ContextMenuStrip
		{
			public TreeMenu(ITreeSource treeSource, TreeDirectoryListItem item)
			{
				Verify.Argument.IsNotNull(item, "item");

				Items.Add(GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(item));
				Items.Add(GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(item));

				if(treeSource != null)
				{
					Items.Add(new ToolStripSeparator());
					Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(treeSource.Revision, item.DataContext));
					Items.Add(new ToolStripSeparator());
					Items.Add(GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(treeSource.Revision, item.DataContext));
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
					var vm = ViewModel as TreeViewModel;
					if(vm != null && vm.TreeSource != null)
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
			get { return _dataSource; }
			set
			{
				if(_dataSource != value)
				{
					if(_dataSource != null)
					{
						_dataSource.DataChanged -= OnTreeChanged;
						_dataSource.Dispose();
					}
					_dataSource = value;
					if(_dataSource != null)
					{
						_dataSource.DataChanged += OnTreeChanged;
						_dataSource.ReloadData();
					}
				}
			}
		}

		/// <summary>Gets a value indicating whether this instance is document.</summary>
		/// <value><c>true</c> if this instance is document; otherwise, <c>false</c>.</value>
		public override bool IsDocument
		{
			get { return true; }
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgFolderTree"]; }
		}

		public TreeDirectory CurrentDirectory
		{
			get { return _currentDirectory; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_currentDirectory != value)
				{
					var item = FindDirectoryEntry(value);
					if(item == null)
						throw new ArgumentException("value");
					item.FocusAndSelect();
					_currentDirectory = value;
					OnCurrentDirectoryChanged();
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

		private void OnContextMenuRequested(object sender, ItemContextMenuRequestEventArgs e)
		{
			var vm = ViewModel as TreeViewModel;
			var rts = vm != null ? vm.TreeSource : null;
			if(rts != null)
			{
				var item = ((ITreeItemListItem)e.Item);
				var file = item.TreeItem as TreeFile;
				if(file != null)
				{
					var menu = new ContextMenuStrip();
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
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFileName, file.Name),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, file.RelativePath),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, file.FullPath),
								}),
							new ToolStripSeparator(),
							GuiItemFactory.GetBlameItem<ToolStripMenuItem>(rts.Revision, file),
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(rts.Revision, file),
							new ToolStripSeparator(),
							GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(rts.Revision, file),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
					return;
				}
				var directory = item.TreeItem as TreeDirectory;
				if(directory != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							new ToolStripMenuItem(Resources.StrOpen, null, (s, args) => e.Item.Activate()),
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(rts.Revision, directory),
							new ToolStripSeparator(),
							GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(rts.Revision, directory),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
					return;
				}
				var commit = ((ITreeItemListItem)e.Item).TreeItem as TreeCommit;
				if(commit != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(rts.Revision, commit),
							new ToolStripSeparator(),
							new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
								new ToolStripItem[]
								{
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, commit.Name),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, commit.RelativePath),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, commit.FullPath),
								}),
							new ToolStripSeparator(),
							GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(rts.Revision, commit),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
					return;
				}
			}
		}

		private void UpdateCurrentDirectory(TreeDirectory directory)
		{
			if(_currentDirectory != directory)
			{
				_currentDirectory = directory;
				OnCurrentDirectoryChanged();
			}
		}

		private void OnDirectoryTreeSelectionChanged(object sender, EventArgs e)
		{
			if(_directoryTree.SelectedItems.Count != 0)
			{
				var treeItem = (_directoryTree.SelectedItems[0] as TreeDirectoryListItem);
				if(_currentDirectory != treeItem.DataContext)
				{
					_currentDirectory = treeItem.DataContext;
					_treeContent.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryContent);
					OnCurrentDirectoryChanged();
				}
			}
		}

		protected override void AttachViewModel(object viewModel)
		{
			base.AttachViewModel(viewModel);

			var vm = viewModel as TreeViewModel;
			if(vm != null)
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

			var vm = viewModel as TreeViewModel;
			if(vm != null)
			{
				_treeSource = null;
				Text = Resources.StrTree;
				DataSource = null;
			}
		}

		private void OnTreeChanged(object sender, EventArgs e)
		{
			if(DataSource != null && DataSource.Data != null)
			{
				UpdateCurrentDirectory(DataSource.Data.Root);
			}
			else
			{
				UpdateCurrentDirectory(null);
			}
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
				if(subSearch != null) return subSearch;
			}
			return null;
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as TreeFileListItem;
			if(item != null)
			{
				var fileName = DataSource.Data.ExtractBlobToTemporaryFile(item.DataContext.RelativePath);
				if(!string.IsNullOrWhiteSpace(fileName))
				{
					var process = Utility.CreateProcessFor(fileName);
					process.EnableRaisingEvents = true;
					process.Exited += OnFileViewerProcessExited;
					process.Start();
				}
			}
			else
			{
				var folderItem = e.Item as TreeDirectoryListItem;
				if(folderItem != null)
				{
					var directoryEntry = FindDirectoryEntry(folderItem.DataContext);
					if(directoryEntry != null)
					{
						if(directoryEntry.IsSelected)
						{
							_treeContent.SetTree(folderItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
						}
						else
						{
							directoryEntry.FocusAndSelect();
						}
					}
					else
					{
						_treeContent.SetTree(folderItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
					}
					_currentDirectory = folderItem.DataContext;
					OnCurrentDirectoryChanged();
				}
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
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				LoggingService.Global.Warning(exc, "Failed to remove temporary file: '{0}'", path);
			}
			process.Dispose();
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_directoryTree.Clear();
			_treeContent.Clear();
		}

		public override void RefreshContent()
		{
			if(DataSource != null)
			{
				DataSource.ReloadData();
			}
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
			{
				_treeContent.LoadViewFrom(listNode);
			}
		}

		#endregion
	}
}
