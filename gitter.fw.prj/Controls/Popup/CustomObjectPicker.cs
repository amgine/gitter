﻿#region Copyright Notice
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

namespace gitter.Framework.Controls;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
public abstract class CustomObjectPicker<TListBox, TItem, TValue> : CustomPopupComboBox, IPicker<TValue>
	where TListBox : CustomListBox, new()
	where TItem : CustomListBoxItem
{
	#region Static

	protected static readonly StringFormat DefaultStringFormat = new(StringFormat.GenericTypographic)
	{
		LineAlignment = StringAlignment.Center,
	};

	#endregion

	#region Data

	private TItem _selectedItem;

	#endregion

	#region .ctor

	protected CustomObjectPicker()
	{
		var listBox = new TListBox()
		{
			Style = GitterApplication.DefaultStyle,
			HeaderStyle = HeaderStyle.Hidden,
			ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
			DisableContextMenus = true,
			Font = LicenseManager.UsageMode == LicenseUsageMode.Runtime?
				GitterApplication.FontManager.UIFont.Font:
				SystemFonts.MessageBoxFont,
		};
		listBox.Size = new Size(Width, 2 + 2 + listBox.CurrentItemHeight * 5);
		listBox.BorderStyle = BorderStyle.FixedSingle;
		if(listBox.Columns.Count == 0)
		{
			listBox.Columns.Add(
				new CustomListBoxColumn(0, Resources.StrName)
				{
					SizeMode = ColumnSizeMode.Auto,
				});
		}
		listBox.ItemActivated += OnListBoxItemActivated;

		DropDownStyle = ComboBoxStyle.DropDownList;
		DrawMode      = DrawMode.OwnerDrawFixed;
		ItemHeight    = SystemInformation.SmallIconSize.Height + 1;

		base.DropDownControl = listBox;
	}

	#endregion

	#region Properties

	protected abstract TValue GetValue(TItem item);

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new TListBox DropDownControl => base.DropDownControl as TListBox;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public CustomListBoxItemsCollection DropDownItems => DropDownControl.Items;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new TItem SelectedItem
	{
		get => _selectedItem;
		set
		{
			if(_selectedItem != value)
			{
				if(value != null && value.ListBox != DropDownControl)
				{
					throw new ArgumentException();
				}
				_selectedItem = value;
				if(_selectedItem != null)
				{
					_selectedItem.FocusAndSelect();
				}
				OnSelectedItemChanged(EventArgs.Empty);
				OnSelectedIndexChanged(EventArgs.Empty);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new TValue SelectedValue
	{
		get
		{
			var selectedItem = SelectedItem;
			if(selectedItem is not null)
			{
				return GetValue(selectedItem);
			}
			return default;
		}
		set
		{
			SelectedItem = FindItemByValue(DropDownControl.Items, value);
		}
	}

	#endregion

	#region Methods

	protected virtual TItem FindItemByValue(CustomListBoxItemsCollection items, TValue value)
	{
		Assert.IsNotNull(items);

		foreach(var item in items)
		{
			if(item is TItem i)
			{
				var v = GetValue(i);
				if(EqualityComparer<TValue>.Default.Equals(v, value))
				{
					return i;
				}
			}
			if(item.Items.Count != 0)
			{
				var subItem = FindItemByValue(item.Items, value);
				if(subItem is not null)
				{
					return subItem;
				}
			}
		}
		return null;
	}

	/// <inheritdoc/>
	protected override void OnDrawItem(DrawItemEventArgs e)
	{
		Assert.IsNotNull(e);

		e.DrawBackground();
		e.Graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
		e.Graphics.TextContrast      = GraphicsUtility.TextContrast;

		var selectedItem = SelectedItem;
		if(selectedItem is null)
		{
			OnPaintNullItem(e);
		}
		else
		{
			OnPaintItem(selectedItem, e);
		}
	}

	protected virtual void OnPaintItem(TItem item, DrawItemEventArgs e)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(e);

		var itemState = ItemState.None;
		if((e.State & DrawItemState.Selected) == DrawItemState.Selected)
		{
			itemState |= ItemState.Selected;
		}
		if((e.State & DrawItemState.Focus) == DrawItemState.Focus)
		{
			itemState |= ItemState.Focused;
		}

		var column = DropDownControl.Columns[0];
		if((e.State & DrawItemState.Selected) == DrawItemState.Selected)
		{
			var oldBrush = column.ContentBrush;
			try
			{
				using var brush = new SolidBrush(e.ForeColor);
				column.ContentBrush = brush;
				var args = new SubItemPaintEventArgs(e.Graphics, Dpi.FromControl(this),
					e.Bounds,
					e.Bounds,
					0,
					itemState,
					0,
					Focused,
					item,
					0,
					column);
				item.PaintSubItem(args);
			}
			finally
			{
				column.ContentBrush = oldBrush;
			}
		}
		else
		{
			var args = new SubItemPaintEventArgs(e.Graphics, Dpi.FromControl(this),
				e.Bounds,
				e.Bounds,
				0,
				itemState,
				0,
				Focused,
				item,
				0,
				column);
			item.PaintSubItem(args);
		}
	}

	protected virtual void OnPaintNullItem(DrawItemEventArgs e)
	{
		Assert.IsNotNull(e);

		var bounds = e.Bounds;
		var iconBounds = e.Bounds;
		iconBounds.Width = 16;
		var d = (iconBounds.Height - 16);
		iconBounds.Y += d / 2;
		iconBounds.Height = 16;
		bounds.X += iconBounds.Width + 3;
		bounds.Width -= iconBounds.Width + 3;

		GitterApplication.TextRenderer.DrawText(
			e.Graphics, "<none>", Font,
			((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
			SystemBrushes.HighlightText : SystemBrushes.GrayText, bounds, DefaultStringFormat);
	}

	private void OnListBoxItemActivated(object sender, ItemEventArgs e)
	{
		Assert.IsNotNull(e);

		SelectedItem = e.Item as TItem;
		HideDropDown();
	}

	/// <inheritdoc/>
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if(DropDownItems is not { Count: > 1 } items)
		{
			return;
		}
		var index = SelectedItem is null ? -1 : items.IndexOf(SelectedItem);
		if(e.Delta > 0)
		{
			if(--index < 0) return;
		}
		else
		{
			if(++index >= items.Count) return;
		}
		var item = items[index];
		DropDownControl.FocusAndSelectItem(item);
		SelectedItem = item as TItem;
		Invalidate();
		base.OnMouseWheel(e);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			var listBox = DropDownControl;
			if(listBox != null)
			{
				listBox.ItemActivated -= OnListBoxItemActivated;
				listBox.Dispose();
			}
		}
		base.Dispose(disposing);
	}

	#endregion
}
