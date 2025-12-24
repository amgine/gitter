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

public class SubmoduleListItem : CustomListBoxItem<Submodule>
{
	public static int CompareByName(SubmoduleListItem item1, SubmoduleListItem item2)
	{
		var data1 = item1.DataContext.Name;
		var data2 = item2.DataContext.Name;
		return string.Compare(data1, data2);
	}

	public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is SubmoduleListItem i1 && item2 is SubmoduleListItem i2
				? CompareByName(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return 0;
		}
	}

	public static int CompareByPath(SubmoduleListItem item1, SubmoduleListItem item2)
	{
		var data1 = item1.DataContext.Path;
		var data2 = item2.DataContext.Path;
		return string.Compare(data1, data2);
	}

	public static int CompareByPath(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is SubmoduleListItem i1 && item2 is SubmoduleListItem i2
				? CompareByPath(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return 0;
		}
	}

	public static int CompareByUrl(SubmoduleListItem item1, SubmoduleListItem item2)
	{
		var data1 = item1.DataContext.Url;
		var data2 = item2.DataContext.Url;
		return string.Compare(data1, data2);
	}

	public static int CompareByUrl(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is SubmoduleListItem i1 && item2 is SubmoduleListItem i2
				? CompareByUrl(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return 0;
		}
	}

	public SubmoduleListItem(Submodule submodule)
		: base(submodule)
	{
	}

	private void OnDeleted(object? sender, EventArgs e)
	{
		RemoveSafe();
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		DataContext.Deleted += OnDeleted;
		base.OnListBoxAttached(listBox);
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DataContext.Deleted -= OnDeleted;
		base.OnListBoxDetached(listBox);
	}

	private static Image? GetImage(Dpi dpi) => Icons.Submodule.GetImage(16 * dpi.X / 96);

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				return measureEventArgs.MeasureImageAndText(GetImage(measureEventArgs.Dpi), DataContext.Name);
			case ColumnId.Path:
				return measureEventArgs.MeasureText(DataContext.Path);
			case ColumnId.Url:
				return measureEventArgs.MeasureText(DataContext.Url);
			default:
				return Size.Empty;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
				paintEventArgs.PaintImageAndText(GetImage(paintEventArgs.Dpi), DataContext.Name);
				break;
			case ColumnId.Path:
				paintEventArgs.PaintText(DataContext.Path);
				break;
			case ColumnId.Url:
				paintEventArgs.PaintText(DataContext.Url);
				break;
		}
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		var menu = new SubmoduleMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
