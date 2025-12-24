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
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

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
				case TreeItemType.Commit when data2.ItemType == TreeItemType.Blob:
					return -1;
				case TreeItemType.Tree   when data2.ItemType == TreeItemType.Blob:
					return -1;
			}
		}
		return string.Compare(data1.Name, data2.Name);
	}

	public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is ITreeItemListItem i1 && item2 is ITreeItemListItem i2
				? CompareByName(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
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
				case TreeItemType.Commit when data2.ItemType == TreeItemType.Blob:
					return -1;
				case TreeItemType.Tree   when data2.ItemType == TreeItemType.Blob:
					return -1;
			}
		}
		return string.Compare(data1.RelativePath, data2.RelativePath);
	}

	public static int CompareByRelativePath(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is ITreeItemListItem i1 && item2 is ITreeItemListItem i2
				? CompareByRelativePath(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return 0;
		}
	}
}

public abstract class TreeItemListItem<T> : CustomListBoxItem<T>, ITreeItemListItem
	where T : TreeItem
{
	#region Data

	private string? _type;
	private FileSize? _size;
	private bool _cachedInfo;
	private bool _showFullPath;

	private Size _cachedSize;

	#endregion

	protected TreeItemListItem(T item, bool showFullPath)
		: base(item)
	{
		Verify.Argument.IsNotNull(item);

		_showFullPath = showFullPath;
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		DataContext.Deleted += OnDeleted;
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DataContext.Deleted -= OnDeleted;
		base.OnListBoxDetached(listBox);
	}

	private void OnDeleted(object? sender, EventArgs e)
	{
		RemoveSafe();
	}

	protected virtual Icon? GetIcon() => null;

	protected virtual Image? GetBitmapIcon(Dpi dpi) => null;

	protected abstract FileSize? GetSize();

	protected abstract string GetItemType();

	private string GetItemText()
		=> _showFullPath ? DataContext.RelativePath : DataContext.Name;

	private Bitmap? GetItemOverlay(Dpi dpi)
	{
		var name = DataContext.Status switch
		{
			FileStatus.Modified => DataContext.StagedStatus == StagedStatus.Unstaged ? @"overlays.edit"   : @"overlays.edit.staged",
			FileStatus.Added    => DataContext.StagedStatus == StagedStatus.Unstaged ? @"overlays.add"    : @"overlays.add.staged",
			FileStatus.Removed  => DataContext.StagedStatus == StagedStatus.Unstaged ? @"overlays.delete" : @"overlays.delete.staged",
			FileStatus.Unmerged => @"overlays.conflict",
			_ => null,
		};
		if(name is null) return default;

		return CachedResources.ScaledBitmaps[name, DpiConverter.FromDefaultTo(dpi).ConvertX(16)];
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				{
					if(_cachedSize.IsEmpty)
					{
						_cachedSize = measureEventArgs.MeasureImageAndText(GetBitmapIcon(measureEventArgs.Dpi), GetItemText());
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

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(!_cachedInfo)
		{
			_size = GetSize();
			_type = GetItemType();
			_cachedInfo = true;
		}
		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
				paintEventArgs.PaintImageOverlayAndText(GetBitmapIcon(paintEventArgs.Dpi), GetItemOverlay(paintEventArgs.Dpi), GetItemText());
				break;
			case ColumnId.Type:
				paintEventArgs.PaintText(_type);
				break;
			case ColumnId.Size when _size.HasValue:
				var rect     = paintEventArgs.Bounds;
				var graphics = paintEventArgs.Graphics;
				var conv     = paintEventArgs.DpiConverter;
				paintEventArgs.PrepareContentRectangle(ref rect);
				paintEventArgs.PrepareTextRectangle(paintEventArgs.Font, ref rect);
				var r2 = rect;
				rect.Width -= conv.ConvertX(23);
				r2.X = r2.Right - conv.ConvertX(20);
				r2.Width = conv.ConvertX(20);
				GitterApplication.TextRenderer.DrawText(
					graphics, _size.Value.ShortSize, paintEventArgs.Font, paintEventArgs.Brush, rect, GitterApplication.TextRenderer.RightAlign);
				GitterApplication.TextRenderer.DrawText(
					graphics, _size.Value.ShortSizeUnits, paintEventArgs.Font, paintEventArgs.Brush, r2);
				break;
		}
	}

	/// <summary>Gets the context menu.</summary>
	/// <param name="requestEventArgs">Request parameters.</param>
	/// <returns>Context menu for specified location.</returns>
	public override ContextMenuStrip? GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		if(DataContext.StagedStatus == StagedStatus.Staged)
		{
			var menu = new StagedItemMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
		if(DataContext.StagedStatus == StagedStatus.Unstaged)
		{
			var menu = new UnstagedItemMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
		return base.GetContextMenu(requestEventArgs);
	}

	#region IWorktreeListItem Members

	TreeItem ITreeItemListItem.TreeItem => DataContext;

	#endregion
}
