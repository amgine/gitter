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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public class TreeFileListItem : TreeItemListItem<TreeFile>
	{
		private bool _showFullPath;

		public TreeFileListItem(TreeFile file, bool showFullPath)
			: base(file, showFullPath)
		{
			_showFullPath = showFullPath;
		}

		/// <summary>Called when item is attached to listbox.</summary>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.StagedStatusChanged += OnStagedStatusChanged;
			DataContext.StatusChanged += OnStatusChanged; 
		}

		/// <summary>Called when item is detached from listbox.</summary>
		protected override void OnListBoxDetached()
		{
			DataContext.StagedStatusChanged -= OnStagedStatusChanged;
			DataContext.StatusChanged -= OnStatusChanged;
			base.OnListBoxDetached();
		}

		protected virtual void OnStatusChanged(object sender, EventArgs e)
		{
			InvalidateSubItemSafe((int)ColumnId.Name);
		}

		protected virtual void OnStagedStatusChanged(object sender, EventArgs e)
		{
			InvalidateSubItemSafe((int)ColumnId.Name);
		}

		//protected override Icon GetIcon()
		//{
		//    if(Data.Status == FileStatus.Removed)
		//    {
		//        return Utility.ExtractAssociatedFileIcon16ByExt(Data.FullPath);
		//    }
		//    else
		//    {
		//        return Utility.ExtractAssociatedFileIcon16(Data.FullPath);
		//    }
		//}

		protected override Bitmap GetBitmapIcon()
		{
			var path = DataContext.RelativePath;
			if(path.EndsWith('/'))
			{
				return CachedResources.Bitmaps["ImgSubmodule"];
			}
			else
			{
				return GraphicsUtility.QueryIcon(DataContext.FullPath);
			}
		}

		protected override FileSize? GetSize()
		{
			if(DataContext.Status == FileStatus.Cached)
			{
				return new FileSize(DataContext.Size);
			}
			if(DataContext.Status == FileStatus.Removed)
			{
				return null;
			}
			try
			{
				var fi = new System.IO.FileInfo(DataContext.FullPath);
				if(fi.Exists)
				{
					var size = fi.Length;
					return new FileSize(size);
				}
				else
				{
					return null;
				}
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				return null;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(DataContext.Status == FileStatus.Unmerged)
			{
				var mnu = new ConflictedFileMenu(DataContext);
				Utility.MarkDropDownForAutoDispose(mnu);
				return mnu;
			}
			else
			{
				return base.GetContextMenu(requestEventArgs);
			}
		}

		protected override string GetItemType()
		{
			return "";// Utility.GetFileType(Data.FullPath, false, Data.Status == GitObjectStatus.Removed);
		}
	}

	public class WorktreeConflictedFileItem : TreeFileListItem
	{
		public WorktreeConflictedFileItem(TreeFile file, bool showFullPath)
			: base(file, showFullPath)
		{
		}

		protected override void OnStatusChanged(object sender, EventArgs e)
		{
			if(DataContext.Status != FileStatus.Unmerged)
			{
				RemoveSafe();
			}
		}
	}
}
