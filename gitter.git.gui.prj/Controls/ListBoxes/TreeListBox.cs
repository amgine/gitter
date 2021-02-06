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
			Columns.Add(new CustomListBoxColumn((int)ColumnId.Size, Resources.StrSize) { Width = 75 });
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
				_binding = mode switch
				{
					TreeListBoxMode.ShowFullTree         => new TreeBinding(Items, root, false),
					TreeListBoxMode.ShowPlainFileList    => new TreeBinding(Items, root, true),
					TreeListBoxMode.ShowDirectoryContent => new TreeBinding(Items, root, false, true),
					TreeListBoxMode.ShowDirectoryTree    => new TreeDirectoriesBinding(Items, root, true),
					_ => throw new ArgumentException("Invalid mode.", "mode"),
				};
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
				switch(item)
				{
					case TreeDirectory dir when HasRevertableItems(dir):
						return true;
					case TreeFile file when file.Status == FileStatus.Removed || file.Status == FileStatus.Modified:
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
