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

namespace gitter.Framework;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using gitter.Framework.Controls;

sealed class MSVS2012CustomListBoxRenderer : CustomListBoxRenderer
{
	#region Static Data

	private static readonly Dictionary<CheckedState, IImageProvider> ImgCheckedState =
		new()
		{
			[CheckedState.Checked]       = CommonIcons.CheckBox.Checked,
			[CheckedState.Unchecked]     = CommonIcons.CheckBox.Unchecked,
			[CheckedState.Indeterminate] = CommonIcons.CheckBox.Indeterminate,
		};

	private static readonly Dictionary<CheckedState, IImageProvider> ImgCheckedStateHovered =
		new()
		{
			[CheckedState.Checked]       = CommonIcons.CheckBox.CheckedHover,
			[CheckedState.Unchecked]     = CommonIcons.CheckBox.UncheckedHover,
			[CheckedState.Indeterminate] = CommonIcons.CheckBox.IndeterminateHover,
		};

	private static readonly PointF[] _triangle = new PointF[3];

	#endregion

	#region Color Tables

	public static readonly IColorTable DarkColors  = new DarkColorTable();
	public static readonly IColorTable LightColors = new LightColorTable();

	public interface IColorTable
	{
		Color Background { get; }
		Color Text { get; }
		Color SelectionBackground { get; }
		Color FocusBorder { get; }
		Color SelectionBackgroundNoFocus { get; }
		Color HoverBackground { get; }
		Color ExtenderForeground { get; }
		Color HoverExtenderForeground { get; }
		Color PlusMinusForeground { get; }
		Color AccentPlusMinusForeground { get; }
	}

	private sealed class DarkColorTable : IColorTable
	{
		private static readonly Color TEXT							= MSVS2012DarkColors.WINDOW_TEXT;
		private static readonly Color BACKGROUND					= MSVS2012DarkColors.WINDOW;
		private static readonly Color SELECTION_BACKGROUND			= MSVS2012DarkColors.HIGHLIGHT;
		private static readonly Color FOCUS_BORDER					= MSVS2012DarkColors.HIGHLIGHT;
		private static readonly Color SELECTION_BACKGROUND_NO_FOCUS	= MSVS2012DarkColors.HIDDEN_HIGHLIGHT;
		private static readonly Color HOVER_BACKGROUND				= MSVS2012DarkColors.HOT_TRACK;
		private static readonly Color EXTENDER_FOREGROUND			= MSVS2012DarkColors.WINDOW_TEXT;
		private static readonly Color HOVER_EXTENDER_FOREGROUND		= Color.FromArgb(28, 151, 234);
		private static readonly Color PLUS_MINUS_FOREGROUND			= Color.FromArgb(255, 255, 255);
		private static readonly Color ACCENT_PLUS_MINUS_FOREGROUND	= Color.FromArgb(0, 122, 204);

		public Color Background => BACKGROUND;

		public Color Text => TEXT;

		public Color SelectionBackground => SELECTION_BACKGROUND;

		public Color FocusBorder => FOCUS_BORDER;

		public Color SelectionBackgroundNoFocus => SELECTION_BACKGROUND_NO_FOCUS;

		public Color HoverBackground => HOVER_BACKGROUND;

		public Color ExtenderForeground => EXTENDER_FOREGROUND;

		public Color HoverExtenderForeground => HOVER_EXTENDER_FOREGROUND;

		public Color PlusMinusForeground => PLUS_MINUS_FOREGROUND;

		public Color AccentPlusMinusForeground => ACCENT_PLUS_MINUS_FOREGROUND;
	}

	private sealed class LightColorTable : IColorTable
	{
		private static readonly Color BACKGROUND					= MSVS2012LightColors.WINDOW;
		private static readonly Color TEXT							= MSVS2012LightColors.WINDOW_TEXT;
		private static readonly Color SELECTION_BACKGROUND			= MSVS2012LightColors.HIGHLIGHT;
		private static readonly Color FOCUS_BORDER					= MSVS2012LightColors.HIGHLIGHT;
		private static readonly Color SELECTION_BACKGROUND_NO_FOCUS	= MSVS2012LightColors.HIDDEN_HIGHLIGHT;
		private static readonly Color HOVER_BACKGROUND				= MSVS2012LightColors.HOT_TRACK;
		private static readonly Color EXTENDER_FOREGROUND			= MSVS2012LightColors.WINDOW_TEXT;
		private static readonly Color HOVER_EXTENDER_FOREGROUND		= Color.FromArgb(28, 151, 234);
		private static readonly Color PLUS_MINUS_FOREGROUND			= Color.FromArgb(255, 255, 255);
		private static readonly Color ACCENT_PLUS_MINUS_FOREGROUND	= Color.FromArgb(0, 122, 204);

		public Color Background => BACKGROUND;

		public Color Text => TEXT;

		public Color SelectionBackground => SELECTION_BACKGROUND;

		public Color FocusBorder => FOCUS_BORDER;

		public Color SelectionBackgroundNoFocus => SELECTION_BACKGROUND_NO_FOCUS;

		public Color HoverBackground => HOVER_BACKGROUND;

		public Color ExtenderForeground => EXTENDER_FOREGROUND;

		public Color HoverExtenderForeground => HOVER_EXTENDER_FOREGROUND;

		public Color PlusMinusForeground => PLUS_MINUS_FOREGROUND;

		public Color AccentPlusMinusForeground => ACCENT_PLUS_MINUS_FOREGROUND;
	}

	#endregion

	#region .ctor

	public MSVS2012CustomListBoxRenderer(IColorTable colorTable)
	{
		Verify.Argument.IsNotNull(colorTable);

		ColorTable = colorTable;
	}

	#endregion

	private IColorTable ColorTable { get; }

	public override Color BackColor => ColorTable.Background;

	public override Color ForeColor => ColorTable.Text;

	public override Color ColumnHeaderForeColor => ColorTable.Text;

	public override void OnPaintColumnBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
	{
		switch(paintEventArgs.State)
		{
			case ItemState.None:
				RenderColumnNormalBackground(paintEventArgs);
				break;
			case ItemState.Pressed:
				RenderColumnPressedBackground(column, paintEventArgs);
				break;
			default:
				RenderColumnHoverBackground(column, paintEventArgs);
				break;
		}
	}

	private void PaintColumnExtender(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
	{
		var bounds = paintEventArgs.Bounds;
		var w = CustomListBoxColumn.ExtenderButtonWidth.GetValue(paintEventArgs.Dpi);
		if(column.Extender is not null && bounds.Width > w)
		{
			var graphics = paintEventArgs.Graphics;
			using(var brush = SolidBrushCache.Get(ColorTable.Background))
			{
				graphics.FillRectangle(brush,
					bounds.Right - w - 0.5f, bounds.Y,
					1, bounds.Height);
			}
			var conv = DpiConverter.FromDefaultTo(paintEventArgs.Dpi);
			var arrowSize = conv.ConvertX(4);
			var p1 = new Point(
				bounds.Right - w + w / 2,
				bounds.Y + bounds.Height - (bounds.Height - arrowSize) / 2);
			var p2 = new Point(p1.X + arrowSize, p1.Y - arrowSize);
			var p3 = new Point(p1.X - arrowSize + 1, p1.Y - arrowSize);
			_triangle[0] = p1;
			_triangle[1] = p2;
			_triangle[2] = p3;
			var foregroundColor = paintEventArgs.HoveredPart == ColumnHitTestResults.Extender ?
				ColorTable.HoverExtenderForeground : ColorTable.ExtenderForeground;
			using(var brush = SolidBrushCache.Get(foregroundColor))
			{
				graphics.FillPolygon(brush, _triangle);
			}
		}
	}

	private static void RenderColumnNormalBackground(ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var bounds = paintEventArgs.Bounds;
		var c1 = Color.FromArgb(63, 63, 70);
		using var pen = new Pen(c1);
		graphics.DrawLine(pen, bounds.Right - 1, 0, bounds.Right - 1, bounds.Bottom - 1);
		graphics.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
	}

	private void RenderColumnPressedBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var bounds = paintEventArgs.Bounds;
		var c1 = Color.FromArgb(63, 63, 70);
		var c2 = Color.FromArgb(0, 122, 204);
		using(var pen = new Pen(c1))
		{
			graphics.DrawLine(pen, bounds.Right - 1, 0, bounds.Right - 1, bounds.Bottom - 1);
			graphics.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
		}
		var rc = bounds;
		rc.Width -= 1;
		rc.Height -= 1;
		graphics.GdiFill(c2, rc);
		PaintColumnExtender(column, paintEventArgs);
	}

	private void RenderColumnHoverBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var bounds = paintEventArgs.Bounds;
		var c1 = Color.FromArgb(63, 63, 70);
		var c2 = Color.FromArgb(62, 62, 64);
		using(var pen = new Pen(c1))
		{
			graphics.DrawLine(pen, bounds.Right - 1, 0, bounds.Right - 1, bounds.Bottom - 1);
			graphics.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
		}
		var rc = bounds;
		rc.Width -= 1;
		rc.Height -= 1;
		graphics.GdiFill(c2, rc);
		PaintColumnExtender(column, paintEventArgs);
	}

	public override void OnPaintColumnContent(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var rect = paintEventArgs.Bounds;
		var font = column.HeaderFont;

		paintEventArgs.PrepareContentRectangle(ref rect);
		paintEventArgs.PrepareTextRectangle(font, font, ref rect);
		if(column.Extender is not null && ((paintEventArgs.State & (ItemState.Hovered | ItemState.Pressed)) != ItemState.None))
		{
			rect.Width -= CustomListBoxColumn.ExtenderButtonWidth.GetValue(paintEventArgs.Dpi);
			if(rect.Width <= 0) return;
		}
		var format = column.HeaderAlignment switch
		{
			StringAlignment.Near   => GitterApplication.TextRenderer.LeftAlign,
			StringAlignment.Far    => GitterApplication.TextRenderer.RightAlign,
			StringAlignment.Center => GitterApplication.TextRenderer.CenterAlign,
			_ => GitterApplication.TextRenderer.LeftAlign,
		};
		GitterApplication.TextRenderer.DrawText(graphics, column.Name, font, column.HeaderBrush, rect, format);
	}

	public override void OnPaintItemBackground(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs)
	{
		var state = paintEventArgs.State;

		if(state == ItemState.None) return;

		bool isHovered	= (state & ItemState.Hovered) == ItemState.Hovered;
		bool isSelected	= (state & ItemState.Selected) == ItemState.Selected;
		bool isFocused	= (state & ItemState.Focused) == ItemState.Focused;

		var backColor = Color.Transparent;
		var pen = default(Pen);

		if(isSelected)
		{
			backColor = paintEventArgs.IsHostControlFocused
				? ColorTable.SelectionBackground
				: ColorTable.SelectionBackgroundNoFocus;
		}
		else
		{
			if(isHovered)
			{
				backColor = ColorTable.HoverBackground;
			}
			if(isFocused && paintEventArgs.IsHostControlFocused)
			{
				pen = new Pen(ColorTable.FocusBorder);
			}
		}
		if(backColor != Color.Transparent)
		{
			var itemBounds = Rectangle.Intersect(paintEventArgs.ClipRectangle, paintEventArgs.Bounds);
			if(itemBounds is { Width: > 0, Height: > 0 })
			{
				paintEventArgs.Graphics.GdiFill(backColor, itemBounds);
			}
		}
		if(pen is not null)
		{
			var rect = paintEventArgs.Bounds;
			rect.Width  -= 1;
			rect.Height -= 1;
			paintEventArgs.Graphics.DrawRectangle(pen, rect);
			pen.Dispose();
		}
	}

	private static void CacheMinusTrianglePolygon1(int x, int y, Dpi dpi)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var p0 = new PointF(x + conv.ConvertX(10.5f), y + conv.ConvertY(3.5f));
		_triangle[0] = p0;
		_triangle[1] = new PointF(p0.X + conv.ConvertX(0.0f), p0.Y + conv.ConvertY(7f));
		_triangle[2] = new PointF(p0.X - conv.ConvertX(7.0f), p0.Y + conv.ConvertY(7f));
	}

	private static void CacheMinusTrianglePolygon2(int x, int y, Dpi dpi)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var p0 = new PointF(x + conv.ConvertX(9.5f), y + conv.ConvertY(6f));
		_triangle[0] = p0;
		_triangle[1] = new PointF(p0.X + conv.ConvertX(0.0f), p0.Y + conv.ConvertY(3.5f));
		_triangle[2] = new PointF(p0.X - conv.ConvertX(3.5f), p0.Y + conv.ConvertY(3.5f));
	}

	private static void CachePlusTrianglePolygon1(int x, int y, Dpi dpi)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var p0 = new PointF(x + conv.ConvertX(5.5f), y + conv.ConvertY(2.5f));
		_triangle[0] = p0;
		_triangle[1] = new PointF(p0.X + conv.ConvertX(5.0f), p0.Y + conv.ConvertY( 5.5f));
		_triangle[2] = new PointF(p0.X + conv.ConvertX(0.0f), p0.Y + conv.ConvertY(10.5f));
	}

	private static void CachePlusTrianglePolygon2(int x, int y, Dpi dpi)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var p0 = new PointF(x + conv.ConvertX(6.5f), y + conv.ConvertY(4.5f));
		_triangle[0] = p0;
		_triangle[1] = new PointF(p0.X + conv.ConvertX(3.0f), p0.Y + conv.ConvertY(3.5f));
		_triangle[2] = new PointF(p0.X + conv.ConvertX(0.0f), p0.Y + conv.ConvertY(6.5f));
	}

	private void RenderPlusMinus(Graphics graphics, Dpi dpi, int x, int y, bool isExpanded, bool isSelected, bool isPlusMinusHovered, bool isHostControlFocused)
	{
		Assert.IsNotNull(graphics);

		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			if(isExpanded)
			{
				if(isPlusMinusHovered)
				{
					if(isHostControlFocused && isSelected)
					{
						CacheMinusTrianglePolygon1(x, y, dpi);
						using(var brush = SolidBrushCache.Get(ColorTable.PlusMinusForeground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
						CacheMinusTrianglePolygon2(x, y, dpi);
						using(var brush = SolidBrushCache.Get(ColorTable.SelectionBackground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
					}
					else
					{
						CacheMinusTrianglePolygon1(x, y, dpi);
						using var brush = SolidBrushCache.Get(ColorTable.AccentPlusMinusForeground);
						graphics.FillPolygon(brush, _triangle);
					}
				}
				else
				{
					CacheMinusTrianglePolygon1(x, y, dpi);
					using var brush = SolidBrushCache.Get(ColorTable.PlusMinusForeground);
					graphics.FillPolygon(brush, _triangle);
				}
			}
			else
			{
				if(isPlusMinusHovered)
				{
					if(isHostControlFocused && isSelected)
					{
						CachePlusTrianglePolygon1(x, y, dpi);
						using var brush = SolidBrushCache.Get(ColorTable.PlusMinusForeground);
						graphics.FillPolygon(brush, _triangle);
					}
					else
					{
						CachePlusTrianglePolygon1(x, y, dpi);
						using(var brush = SolidBrushCache.Get(ColorTable.AccentPlusMinusForeground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
						CachePlusTrianglePolygon2(x, y, dpi);
						if(isSelected)
						{
							if(isHostControlFocused)
							{
								using var brush = SolidBrushCache.Get(ColorTable.SelectionBackground);
								graphics.FillPolygon(brush, _triangle);
							}
							else
							{
								using var brush = SolidBrushCache.Get(ColorTable.SelectionBackgroundNoFocus);
								graphics.FillPolygon(brush, _triangle);
							}
						}
						else
						{
							using var brush = SolidBrushCache.Get(ColorTable.Background);
							graphics.FillPolygon(brush, _triangle);
						}
					}
				}
				else
				{
					CachePlusTrianglePolygon1(x, y, dpi);
					using(var brush = SolidBrushCache.Get(ColorTable.PlusMinusForeground))
					{
						graphics.FillPolygon(brush, _triangle);
					}
					CachePlusTrianglePolygon2(x, y, dpi);
					if(isSelected)
					{
						if(isHostControlFocused)
						{
							using var brush = SolidBrushCache.Get(ColorTable.SelectionBackground);
							graphics.FillPolygon(brush, _triangle);
						}
						else
						{
							using var brush = SolidBrushCache.Get(ColorTable.SelectionBackgroundNoFocus);
							graphics.FillPolygon(brush, _triangle);
						}
					}
					else
					{
						using var brush = SolidBrushCache.Get(ColorTable.Background);
						graphics.FillPolygon(brush, _triangle);
					}
				}
			}
		}
	}

	private void PaintTreeLines(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs, int x, ref int offset, ref int w2)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(paintEventArgs);

		var listBox = item.ListBox;
		var level   = item.Level;

		if(!listBox.ShowRootTreeLines && level != 0)
		{
			var levelMargin = ListBoxConstants.LevelMargin.GetValue(paintEventArgs.Dpi);
			offset -= levelMargin;
			w2     += levelMargin;
		}

		if(level != 0 || listBox.ShowRootTreeLines)
		{
			var spaceBefore = ListBoxConstants.SpaceBeforePlusMinus.GetValue(paintEventArgs.Dpi);
			var imageSize   = ListBoxConstants.PlusMinusImageSize.GetValue(paintEventArgs.Dpi);

			if(w2 > spaceBefore && item.Items.Count != 0)
			{
				var bounds = paintEventArgs.Bounds;

				RenderPlusMinus(
					paintEventArgs.Graphics, paintEventArgs.Dpi,
					x + offset, bounds.Y + (bounds.Height - imageSize.Height) / 2,
					item.IsExpanded,
					(paintEventArgs.State & ItemState.Selected) == ItemState.Selected,
					paintEventArgs.HoveredPart == ItemHitTestResults.PlusMinus,
					paintEventArgs.IsHostControlFocused);
			}

			var spaceAfter = ListBoxConstants.SpaceAfterPlusMinus.GetValue(paintEventArgs.Dpi);
			var dx = spaceBefore + spaceAfter + imageSize.Width;

			offset += dx;
			w2     -= dx;
		}
	}

	private static void PaintCheckBox(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs, int x, ref int offset, ref int w2)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(paintEventArgs);

		var lookup = paintEventArgs.HoveredPart == ItemHitTestResults.CheckBox
			? ImgCheckedStateHovered
			: ImgCheckedState;
		var image = lookup.TryGetValue(item.CheckedState, out var imageProvider)
			? imageProvider.GetImage(16 * paintEventArgs.Dpi.X / 96)
			: default;

		var spaceBefore = ListBoxConstants.SpaceBeforeCheckbox.GetValue(paintEventArgs.Dpi);
		var imageSize   = ListBoxConstants.CheckboxImageSize.GetValue(paintEventArgs.Dpi);

		if(image is not null && w2 > spaceBefore)
		{
			var bounds = paintEventArgs.Bounds;
			Rectangle destRect, srcRect;
			if(w2 < imageSize.Width + spaceBefore)
			{
				destRect = new Rectangle(
					x + offset + spaceBefore,
					bounds.Y + (bounds.Height - imageSize.Height) / 2,
					w2 - spaceBefore,
					imageSize.Height);
				srcRect = new Rectangle(
					0, 0,
					imageSize.Width * (w2 - spaceBefore) / imageSize.Width,
					imageSize.Height);
			}
			else
			{
				destRect = new Rectangle(
					x + offset + spaceBefore,
					bounds.Y + (bounds.Height - imageSize.Height) / 2,
					imageSize.Width,
					imageSize.Height);
				srcRect = new Rectangle(
					0, 0,
					imageSize.Width,
					imageSize.Height);
			}
			paintEventArgs.Graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
		}

		var spaceAfter = ListBoxConstants.SpaceAfterCheckbox.GetValue(paintEventArgs.Dpi);
		var dx = spaceBefore + spaceAfter + imageSize.Width;

		offset += dx;
		w2     -= dx;
	}

	public override void OnPaintItemContent(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var rect     = paintEventArgs.Bounds;

		#region clip invisible subitems

		var clip = paintEventArgs.ClipRectangle;
		var clipX1 = clip.X;
		var clipX2 = clip.Right;
		var columns = item.ListBox.Columns;
		int columnsCount = columns.Count;
		int x = rect.X;

		int firstColumnId;
		int startColumnId;
		int endColumnId;
		int startX;

		if(clipX1 <= rect.X && clipX2 >= rect.Right)
		{
			// all subitems should be painted
			startColumnId = 0;
			firstColumnId = 0;
			endColumnId = columnsCount - 1;
			startX = x;
		}
		else
		{
			firstColumnId = -1;
			startColumnId = -1;
			endColumnId = -1;
			startX = -1;
			// skip clipped subitems
			int prev = -1;
			for(int i = 0; i < columnsCount; ++i)
			{
				var column = columns[i];
				if(column.IsVisible)
				{
					if(firstColumnId == -1)
					{
						firstColumnId = i;
					}

					int x2 = x + column.CurrentWidth;

					if(startColumnId == -1 && x2 > clipX1)
					{
						if(prev != -1 && columns[prev].ExtendsToRight)
						{
							startColumnId = prev;
							startX = x - columns[prev].CurrentWidth;
						}
						else
						{
							startColumnId = i;
							startX = x;
						}
					}

					if(startColumnId != -1 && endColumnId == -1 && x2 >= clipX2)
					{
						endColumnId = i++;
						for(; i < columnsCount; ++i)
						{
							if(columns[i].IsVisible)
							{
								if(columns[i].ExtendsToLeft)
								{
									endColumnId = i;
								}
								break;
							}
						}
						break;
					}

					x = x2;
					prev = i;
				}
			}
			// no visible columns found
			if(startColumnId == -1) return;
			if(endColumnId == -1) endColumnId = prev;
		}

		#endregion

		x = startX;
		bool first = startColumnId == firstColumnId;
		var subrect = new Rectangle(0, rect.Y, 0, rect.Height);

		int hoveredPart = paintEventArgs.HoveredPart;

		for(int i = startColumnId; i <= endColumnId; ++i)
		{
			var column = columns[i];
			if(column.IsVisible)
			{
				int columnWidth = column.CurrentWidth;

				if(first)
				{
					first = false;
					var listBox = item.ListBox;
					int offset = item.Level * ListBoxConstants.LevelMargin.GetValue(paintEventArgs.Dpi) + ListBoxConstants.RootMargin;
					int w2 = columnWidth - offset;

					if(listBox.ShowTreeLines)
					{
						PaintTreeLines(item, paintEventArgs, x, ref offset, ref w2);
					}

					if(listBox.ShowCheckBoxes && item.CheckedState != CheckedState.Unavailable)
					{
						PaintCheckBox(item, paintEventArgs, x, ref offset, ref w2);
					}

					subrect.X     = x + offset;
					subrect.Width = w2;
					x += columnWidth;
					if(w2 <= 0) continue;
				}
				else
				{
					subrect.X     = x;
					subrect.Width = columnWidth;
					x += columnWidth;
				}

				item.PaintSubItem(new SubItemPaintEventArgs(paintEventArgs.Graphics, paintEventArgs.Dpi, clip, subrect, paintEventArgs.Index,
					paintEventArgs.State, hoveredPart, paintEventArgs.IsHostControlFocused, item, i, column));
			}
		}
	}
}
