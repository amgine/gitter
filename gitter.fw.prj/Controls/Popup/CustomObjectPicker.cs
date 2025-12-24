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

namespace gitter.Framework.Controls;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
public abstract class CustomObjectPicker<TListBox, TItem, TValue> : BorderControl, IPicker<TValue>
	where TListBox : CustomListBox, new()
	where TItem : CustomListBoxItem
{
	const int MaxDisplayedItems = 6;

	protected static readonly StringFormat DefaultStringFormat = new(StringFormat.GenericTypographic)
	{
		LineAlignment = StringAlignment.Center,
	};

	private static readonly object SelectedValueChangedEvent = new();

	public event EventHandler SelectedValueChanged
	{
		add    => Events.AddHandler    (SelectedValueChangedEvent, value);
		remove => Events.RemoveHandler (SelectedValueChangedEvent, value);
	}

	protected virtual void OnSelectedValueChanged(EventArgs e)
		=> Events.Raise(SelectedValueChangedEvent, this, e);

	private TListBox _dropDownControl = default!;
	private TItem? _selectedItem;
	private Popup? _dropDown;
	private long _lastHidden;

	protected CustomObjectPicker()
	{
		SetStyle(ControlStyles.Selectable, true);
		SetStyle(ControlStyles.StandardDoubleClick | ControlStyles.ContainerControl, false);

		var listBox = new TListBox()
		{
			HeaderStyle = HeaderStyle.Hidden,
			ItemActivation = ItemActivation.SingleClick,
			DisableContextMenus = true,
			Font = LicenseManager.UsageMode == LicenseUsageMode.Runtime ?
				GitterApplication.FontManager.UIFont.Font :
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
		listBox.ItemActivated              += OnListBoxItemActivated;
		listBox.DisplayedItemsCountChanged += OnDropDownControlDisplayedItemsCountChanged;

		DropDownControl = listBox;
	}

	private void OnDropDownControlDisplayedItemsCountChanged(object? sender, EventArgs e)
	{
		if(sender is not TListBox listBox) return;
		if(_dropDown is not { Visible: true }) return;
		var count = Math.Min(MaxDisplayedItems, Math.Max(listBox.DisplayedItemsCount, 1));
		_dropDown.Height = listBox.GetHeightToFitItems(count, Dpi.FromControl(this));
	}

	public TListBox DropDownControl
	{
		get => _dropDownControl;
		private set
		{
			if(_dropDownControl == value) return;

			_dropDownControl = value;
			if(_dropDown is not null)
			{
				_dropDown.Closed -= OnDropDownClosed;
				_dropDown.Dispose();
				_dropDown = default;
			}
			_dropDown = new Popup(value)
			{
				PopupAnimation = PopupAnimations.Slide | PopupAnimations.TopToBottom,
				AutoClose = true,
			};
			_dropDown.Closed += OnDropDownClosed;
		}
	}

	public bool DroppedDown
	{
		get => _dropDown is { Visible: true };
		set
		{
			if(value)
			{
				ShowDropDownCore();
			}
			else
			{
				HideDropDownCore();
			}
		}
	}

	private void ShowDropDownCore()
	{
		if(_dropDown is null) return;
		if(DropDownControl.DisplayedItemsCount == 0) return;
		var count = Math.Min(6, DropDownControl.DisplayedItemsCount);
		_dropDown.Height = DropDownControl.GetHeightToFitItems(count, Dpi.FromControl(this));
		_dropDown.Show(this);
	}

	private void HideDropDownCore(ToolStripDropDownCloseReason reason = ToolStripDropDownCloseReason.ItemClicked)
		=> _dropDown?.Close(reason);

	private void OnDropDownClosed(object? sender, ToolStripDropDownClosedEventArgs e)
#if NETCOREAPP
		=> _lastHidden = Environment.TickCount64;
#else
		=> _lastHidden = Environment.TickCount;
#endif

	/// <summary>Extracts value from the specified list item.</summary>
	/// <param name="item">List item.</param>
	/// <returns>Extracted value.</returns>
	protected abstract TValue GetValue(TItem item);

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public CustomListBoxItemsCollection DropDownItems => DropDownControl.Items;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public TItem? SelectedItem
	{
		get => _selectedItem;
		set
		{
			if(_selectedItem == value) return;

			if(value is not null && value.ListBox != DropDownControl)
			{
				throw new ArgumentException($"Specified item does not belong to {nameof(DropDownControl)}", nameof(value));
			}
			_selectedItem = value;
			_selectedItem?.FocusAndSelect();
			Invalidate();
			OnSelectedValueChanged(EventArgs.Empty);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public TValue? SelectedValue
	{
		get => SelectedItem is { } item ? GetValue(item) : default;
		set => SelectedItem = FindItemByValue(DropDownControl.Items, value);
	}

	protected override bool IsFocused => Focused || _dropDown is { Visible: true };

	protected virtual TItem? FindItemByValue(CustomListBoxItemsCollection items, TValue? value)
	{
		Assert.IsNotNull(items);

		foreach(var item in items)
		{
			if(item is TItem typedItem)
			{
				if(EqualityComparer<TValue?>.Default.Equals(GetValue(typedItem), value))
				{
					return typedItem;
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
		return default;
	}

	protected override void PaintContent(Graphics graphics, Rectangle bounds, Rectangle clip, Colors colors)
	{
		var conv        = DpiConverter.FromDefaultTo(this);
		var glyphColor  = colors.ForeColor;
		var glyphSize   = conv.ConvertX(8);
		var glyphMargin = conv.ConvertX(4);

		var glyphBounds = new Rectangle(
			bounds.Right - (glyphMargin + glyphSize + BorderThickness.GetValue(conv.To)),
			bounds.Y + (bounds.Height - glyphSize) / 2,
			glyphSize, glyphSize);

		using(var pen = new Pen(glyphColor, conv.ConvertX(1.5f)))
		{
			var h  = glyphBounds.Height / 4;
			var y0 = glyphBounds.Y + h;
			var y1 = glyphBounds.Y + h * 3;
			var x0 = glyphBounds.X;
			var x1 = glyphBounds.X + glyphBounds.Width / 2;
			var x2 = glyphBounds.Right;
#if NET9_0_OR_GREATER
			Span<Point> points = stackalloc Point[3];
#else
			var points = new Point[3];
#endif
			points[0] = new(x0, y0);
			points[1] = new(x1, y1);
			points[2] = new(x2, y0);
			using(graphics.SwitchSmoothingMode(System.Drawing.Drawing2D.SmoothingMode.HighQuality))
			{
				graphics.DrawLines(pen, points);
			}
		}

		bounds.Width -= glyphMargin + glyphSize;

		if(SelectedItem is { } selectedItem)
		{
			OnPaintItem(selectedItem, graphics, bounds, clip, colors);
		}
		else
		{
			OnPaintNullItem(graphics, bounds, clip, colors);
		}
	}

	/// <inheritdoc/>
	protected virtual void OnPaintItem(TItem item, Graphics graphics, Rectangle bounds, Rectangle clip, Colors colors)
	{
		Assert.IsNotNull(item);

		var itemState = ItemState.None;
		var column = DropDownControl.Columns[0];
		var args = new SubItemPaintEventArgs(graphics, Dpi.FromControl(this),
			bounds,
			bounds,
			0,
			itemState,
			0,
			Focused,
			item,
			0,
			column)
		{
			Font = Font,
		};
		item.PaintSubItem(args);
	}

	protected virtual void OnPaintNullItem(Graphics graphics, Rectangle bounds, Rectangle clip, Colors colors)
	{
		var iconBounds = bounds;
		iconBounds.Width = 16;
		var d = (iconBounds.Height - 16);
		iconBounds.Y += d / 2;
		iconBounds.Height = 16;
		bounds.X += iconBounds.Width + 3;
		bounds.Width -= iconBounds.Width + 3;

		GitterApplication.TextRenderer.DrawText(
			graphics, "<none>", Font,
			colors.ForeColor, bounds, DefaultStringFormat);
	}

	private void OnListBoxItemActivated(object? sender, ItemEventArgs e)
	{
		Assert.IsNotNull(e);

		SelectedItem = e.Item as TItem;
		HideDropDownCore();
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if(e.Button == MouseButtons.Left)
		{
			Focus();
#if NETCOREAPP
			var delta = Environment.TickCount64 - _lastHidden;
#else
			var delta = Environment.TickCount - _lastHidden;
#endif
			if(delta < 0 || delta > 20)
			{
				if(_dropDown is { Visible: true })
				{
					HideDropDownCore();
				}
				else
				{
					ShowDropDownCore();
				}
			}
			Invalidate();
		}
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
		base.OnMouseWheel(e);
	}

	/// <inheritdoc/>
	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		if(_dropDown is not null)
		{
			_dropDown.Width = Width;
		}
	}

	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		UpdateColors();
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		UpdateColors();
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_dropDown is not null)
			{
				_dropDown.Closed -= OnDropDownClosed;
				_dropDown.Dispose();
				_dropDown = default;
			}
			var listBox = DropDownControl;
			if(listBox is not null)
			{
				listBox.DisplayedItemsCountChanged -= OnDropDownControlDisplayedItemsCountChanged;
				listBox.ItemActivated              -= OnListBoxItemActivated;
				listBox.Dispose();
			}
		}
		base.Dispose(disposing);
	}
}
