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
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public static class TreeItemListItem
	{
		public static int CompareByName(ITreeItemListItem item1, ITreeItemListItem item2)
		{
			var data1 = item1.TreeItem;
			var data2 = item2.TreeItem;
			if(data1.ItemType != data2.ItemType)
			{
				switch(data1.ItemType)
				{
					case TreeItemType.Blob:
						return 1;
					case TreeItemType.Commit:
						if(data2.ItemType == TreeItemType.Blob)
							return -1;
						break;
					case TreeItemType.Tree:
						if(data2.ItemType == TreeItemType.Blob)
							return -1;
						break;
				}
			}
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as ITreeItemListItem;
			if(i1 == null) return 0;
			var i2 = item2 as ITreeItemListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByName(i1, i2);
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				return 0;
			}
		}

		public static int CompareByRelativePath(ITreeItemListItem item1, ITreeItemListItem item2)
		{
			var data1 = item1.TreeItem;
			var data2 = item2.TreeItem;
			if(data1.ItemType != data2.ItemType)
			{
				switch(data1.ItemType)
				{
					case TreeItemType.Blob:
						return 1;
					case TreeItemType.Commit:
						if(data2.ItemType == TreeItemType.Blob)
							return -1;
						break;
					case TreeItemType.Tree:
						if(data2.ItemType == TreeItemType.Blob)
							return -1;
						break;
				}
			}
			return string.Compare(data1.RelativePath, data2.RelativePath);
		}

		public static int CompareByRelativePath(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as ITreeItemListItem;
			if(i1 == null) return 0;
			var i2 = item2 as ITreeItemListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByRelativePath(i1, i2);
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				return 0;
			}
		}
	}

	public abstract class TreeItemListItem<T> : CustomListBoxItem<T>, ITreeItemListItem
		where T : TreeItem
	{
		#region Static Data

		private static readonly Bitmap ImgOverlayEdit		= CachedResources.Bitmaps["ImgOverlayEdit"];
		private static readonly Bitmap ImgOverlayEditStaged	= CachedResources.Bitmaps["ImgOverlayEditStaged"];
		private static readonly Bitmap ImgOverlayAdd		= CachedResources.Bitmaps["ImgOverlayAdd"];
		private static readonly Bitmap ImgOverlayAddStaged	= CachedResources.Bitmaps["ImgOverlayAddStaged"];
		private static readonly Bitmap ImgOverlayDel		= CachedResources.Bitmaps["ImgOverlayDel"];
		private static readonly Bitmap ImgOverlayDelStaged	= CachedResources.Bitmaps["ImgOverlayDelStaged"];
		private static readonly Bitmap ImgOverlayConflict	= CachedResources.Bitmaps["ImgOverlayConflict"];

		#endregion

		#region Data

		private Icon _icon;
		private Bitmap _bmpIcon;
		private string _type;
		private FileSize? _size;
		private bool _cachedInfo;
		private bool _showFullPath;

		private Size _cachedSize;

		#endregion

		protected TreeItemListItem(T item, bool showFullPath)
			: base(item)
		{
			Verify.Argument.IsNotNull(item, "item");

			_showFullPath = showFullPath;
		}

		/// <summary>
		/// Called when item is attached to listbox.
		/// </summary>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
		}

		/// <summary>
		/// Called when item is detached from listbox.
		/// </summary>
		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			base.OnListBoxDetached();
		}

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		protected virtual Icon GetIcon()
		{
			return null;
		}

		protected virtual Bitmap GetBitmapIcon()
		{
			return null;
		}

		protected abstract FileSize? GetSize();

		protected abstract string GetItemType();

		private string GetItemText()
		{
			return _showFullPath ? DataContext.RelativePath : DataContext.Name;
		}

		private Bitmap GetItemOverlay()
		{
			switch(DataContext.Status)
			{
				case FileStatus.Modified:
					return (DataContext.StagedStatus == StagedStatus.Unstaged) ? ImgOverlayEdit : ImgOverlayEditStaged;
				case FileStatus.Added:
					return (DataContext.StagedStatus == StagedStatus.Unstaged) ? ImgOverlayAdd : ImgOverlayAddStaged;
				case FileStatus.Removed:
					return (DataContext.StagedStatus == StagedStatus.Unstaged) ? ImgOverlayDel : ImgOverlayDelStaged;
				case FileStatus.Unmerged:
					return ImgOverlayConflict;
				default:
					return null;
			}
		}

		/// <summary>
		/// Override this to provide subitem measurement.
		/// </summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		/// <returns></returns>
		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					{
						if(_cachedSize.IsEmpty)
						{
							_cachedSize = measureEventArgs.MeasureImageAndText(_bmpIcon, GetItemText());
						}
						return _cachedSize;
					}
				case ColumnId.Type:
					return measureEventArgs.MeasureText(_type);
				case ColumnId.Size:
					if(_size.HasValue)
					{
						var w = measureEventArgs.MeasureText(_size.Value.ShortSize);
						w.Width += 20;
						return w;
					}
					else
					{
						return Size.Empty;
					}
				default:
					return Size.Empty;
			}
		}

		/// <summary>
		/// Override this to paint part of your item.
		/// </summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(!_cachedInfo)
			{
				_icon = GetIcon();
				if(_icon == null)
				{
					_bmpIcon = GetBitmapIcon();
				}
				_size = GetSize();
				_type = GetItemType();
				_cachedInfo = true;
			}
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageOverlayAndText(_bmpIcon, GetItemOverlay(), GetItemText());
					break;
				case ColumnId.Type:
					paintEventArgs.PaintText(_type);
					break;
				case ColumnId.Size:
					if(_size.HasValue)
					{
						var rect = paintEventArgs.Bounds;
						var graphics = paintEventArgs.Graphics;
						SubItemPaintEventArgs.PrepareContentRectangle(ref rect);
						paintEventArgs.PrepareTextRectangle(paintEventArgs.Font, ref rect);
						var r2 = rect;
						rect.Width -= 23;
						r2.X = r2.Right - 20;
						r2.Width = 20;
						GitterApplication.TextRenderer.DrawText(
							graphics, _size.Value.ShortSize, paintEventArgs.Font, paintEventArgs.Brush, rect, GitterApplication.TextRenderer.RightAlign);
						GitterApplication.TextRenderer.DrawText(
							graphics, _size.Value.ShortSizeUnits, paintEventArgs.Font, paintEventArgs.Brush, r2);
					}
					break;
			}
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(DataContext.StagedStatus == StagedStatus.Staged)
			{
				var menu = new StagedItemMenu(DataContext);
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			else if(DataContext.StagedStatus == StagedStatus.Unstaged)
			{
				var menu = new UnstagedItemMenu(DataContext);
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			return base.GetContextMenu(requestEventArgs);
		}

		#region IWorktreeListItem Members

		TreeItem ITreeItemListItem.TreeItem
		{
			get { return DataContext; }
		}

		#endregion
	}
}
