namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class TreeListBox : CustomListBox
	{
		#region Data

		private IDisposable _binding;
		private Repository _repository;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="TreeListBox"/> class.</summary>
		public TreeListBox()
		{
			Columns.Add(new NameColumn());
			Columns.Add(new CustomListBoxColumn((int)ColumnId.Type, Resources.StrType) { Width = 50 });
			Columns.Add(new CustomListBoxColumn((int)ColumnId.Size, Resources.StrSize) { Width = 50 });
		}

		public void SetTree(TreeDirectory root, TreeListBoxMode mode)
		{
			BeginUpdate();
			ShowTreeLines =
				mode != TreeListBoxMode.ShowPlainFileList &&
				mode != TreeListBoxMode.ShowDirectoryContent;
			ShowRootTreeLines = mode != TreeListBoxMode.ShowDirectoryTree;
			if(_binding != null)
			{
				_binding.Dispose();
				_binding = null;
				_repository = null;
			}
			if(root != null)
			{
				switch(mode)
				{
					case TreeListBoxMode.ShowFullTree:
						_binding = new TreeBinding(Items, root, false);
						break;
					case TreeListBoxMode.ShowPlainFileList:
						_binding = new TreeBinding(Items, root, true);
						break;
					case TreeListBoxMode.ShowDirectoryContent:
						_binding = new TreeBinding(Items, root, false, true);
						break;
					case TreeListBoxMode.ShowDirectoryTree:
						_binding = new TreeDirectoriesBinding(Items, root, true);
						break;
					default:
						throw new ArgumentException("Invalid mode.", "mode");
				}
				_repository = root.Repository;
			}
			EndUpdate();
		}

		public void Clear()
		{
			if(_binding != null)
			{
				_binding.Dispose();
				_binding = null;
				_repository = null;
			}
		}

		private static bool HasRevertableItems(TreeDirectory directory)
		{
			foreach(var file in directory.Files)
			{
				if(file.Status == FileStatus.Removed || file.Status == FileStatus.Modified)
				{
					return true;
				}
			}
			foreach(var dir in directory.Directories)
			{
				if(HasRevertableItems(dir))
				{
					return true;
				}
			}
			return false;
		}

		private static bool HasRevertableItems(IEnumerable<TreeItem> items)
		{
			foreach(var item in items)
			{
				var dir = item as TreeDirectory;
				if(dir != null && HasRevertableItems(dir))
				{
					return true;
				}
				var file = item as TreeFile;
				if(file != null && (file.Status == FileStatus.Removed || file.Status == FileStatus.Modified))
				{
					return true;
				}
			}
			return false;
		}

		protected override ContextMenuStrip GetMultiselectContextMenu(ItemsContextMenuRequestEventArgs requestEventArgs)
		{
			var stagedStatus = StagedStatus.None;
			var items = new TreeItem[requestEventArgs.Items.Count];
			int id = 0;
			foreach(var item in requestEventArgs.Items)
			{
				var treeItem = ((ITreeItemListItem)item).TreeItem;
				items[id++] = treeItem;
				stagedStatus |= treeItem.StagedStatus;
			}
			ContextMenuStrip menu;
			switch(stagedStatus)
			{
				case StagedStatus.Staged:
					menu = new ContextMenuStrip();
					menu.Items.Add(GuiItemFactory.GetUnstageItem<ToolStripMenuItem>(_repository, items));
					break;
				case StagedStatus.Unstaged:
					menu = new ContextMenuStrip();
					menu.Items.Add(GuiItemFactory.GetStageItem<ToolStripMenuItem>(_repository, items));
					if(HasRevertableItems(items))
					{
						menu.Items.Add(GuiItemFactory.GetRevertPathsItem<ToolStripMenuItem>(items));
					}
					break;
				default:
					menu = null;
					break;
			}
			if(menu != null)
			{
				Utility.MarkDropDownForAutoDispose(menu);
			}
			return menu;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_binding != null)
				{
					_binding.Dispose();
					_binding = null;
				}
				_repository = null;
			}
			base.Dispose(disposing);
		}
	}
}
