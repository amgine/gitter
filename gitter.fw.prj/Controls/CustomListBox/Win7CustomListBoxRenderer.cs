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

namespace gitter.Framework.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

sealed class Win7CustomListBoxRenderer : CustomListBoxRenderer
{
	#region Static Data

	private static readonly Brush ExtenderHoveredBrush	= new SolidBrush(Color.FromArgb(210, 229, 253));
	private static readonly Brush ExtenderBorderBrush	= new LinearGradientBrush(Point.Empty, new Point(0, 23), Color.FromArgb(223, 234, 247), Color.FromArgb(238, 242, 249));

	private static readonly Pen ExtenderBorderPenHovered = new(Color.FromArgb(215, 227, 241));

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

	#endregion

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

	private static void RenderColumnNormalBackground(ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var rect = paintEventArgs.Bounds;
		var c1 = Color.FromArgb(223, 234, 247);
		var c2 = Color.FromArgb(255, 255, 255);
		var rc = new Rectangle(rect.Right - 1, 0, 1, rect.Height);
		using(var brush = new LinearGradientBrush(
			rc, c1, c2, LinearGradientMode.Vertical))
		{
			graphics.FillRectangle(brush, rc);
		}
	}

	private static void RenderColumnPressedBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var rect = paintEventArgs.Bounds;
		var c1 = Color.FromArgb(192, 203, 217);
		var c2 = Color.FromArgb(246, 247, 248);
		var c3 = Color.FromArgb(193, 204, 218);
		var c4 = Color.FromArgb(215, 222, 231);
		var c5 = Color.FromArgb(235, 238, 242);
		var conv = DpiConverter.FromDefaultTo(paintEventArgs.Dpi);
		using(var p = new Pen(c1))
		{
			var rc = rect;
			rc.Y -= conv.ConvertY(1);
			rc.X += conv.ConvertX(1);
			rc.Width -= conv.ConvertX(2);
			graphics.DrawRectangle(p, rc);
		}
		using(var b = new SolidBrush(c2))
		{
			var rc = rect;
			rc.Y += conv.ConvertY(3);
			rc.X += conv.ConvertX(2);
			rc.Width  -= conv.ConvertX(4);
			rc.Height -= conv.ConvertY(4);
			graphics.FillRectangle(b, rc);
		}
		using(var p = new Pen(c3))
		{
			var rc = rect;
			graphics.DrawLine(p, rc.X + 1, rc.Y + 0, rc.Right - 2, rc.Y + 0);
		}
		using(var p = new Pen(c4))
		{
			var rc = rect;
			graphics.DrawLine(p, rc.X + 1, rc.Y + 1, rc.Right - 2, rc.Y + 1);
		}
		using(var p = new Pen(c5))
		{
			var rc = rect;
			graphics.DrawLine(p, rc.X + 1, rc.Y + 2, rc.Right - 2, rc.Y + 2);
		}
		if(column.Extender is not null)
		{
			var w = CustomListBoxColumn.ExtenderButtonWidth.GetValue(paintEventArgs.Dpi);
			if(rect.Width > w)
			{
				var rc0 = new RectangleF(rect.Right - w - 0.5f, rect.Y, 1, rect.Height - 1);
				graphics.FillRectangle(ExtenderBorderBrush, rc0);
				var arrowSize = conv.ConvertX(4);
				var p1 = new PointF(rect.Right - w / 2, rect.Y + (rect.Height + arrowSize) / 2);
				var p2 = new PointF(p1.X + arrowSize, p1.Y - arrowSize);
				var p3 = new PointF(p1.X - arrowSize + 1, p1.Y - arrowSize);
				var triangle = new PointF[3] { p1, p2, p3, };
				using(var brush = new SolidBrush(Color.FromArgb(76, 96, 122)))
				{
					graphics.FillPolygon(brush, triangle);
				}
			}
		}
	}

	private static void RenderColumnHoverBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		var rect = paintEventArgs.Bounds;
		var c1 = Color.FromArgb(227, 232, 238);
		var c2 = Color.FromArgb(241, 245, 251);
		var dpi  = paintEventArgs.Dpi;
		var conv = DpiConverter.FromDefaultTo(dpi);
		using(var p = new Pen(c1))
		{
			var rc = rect;
			rc.Y     -= conv.ConvertX(1);
			rc.Width -= conv.ConvertX(1);
			graphics.DrawRectangle(p, rc);
		}
		using(var b = new SolidBrush(c2))
		{
			var rc = rect;
			rc.X += conv.ConvertX(2);
			rc.Y += conv.ConvertY(1);
			rc.Width  -= conv.ConvertX(4);
			rc.Height -= conv.ConvertY(3);
			graphics.FillRectangle(b, rc);
		}
		if(column.Extender is not null)
		{
			var w = CustomListBoxColumn.ExtenderButtonWidth.GetValue(paintEventArgs.Dpi);
			if(rect.Width > w)
			{
				if(paintEventArgs.HoveredPart == ColumnHitTestResults.Extender)
				{
					var rc1 = new RectangleF(rect.Right - w + 1.5f, rect.Y + 1.5f, w - 4, rect.Height - 4);
					var rc2 = new Rectangle (rect.Right - w, 0, w - 1, rect.Height - 1);
					graphics.FillRectangle(ExtenderHoveredBrush, rc1);
					graphics.DrawRectangle(ExtenderBorderPenHovered, rc2);
				}
				else
				{
					var rc1 = new RectangleF(rect.Right - w - 0.5f, rect.Y, conv.ConvertX(1), rect.Height);
					graphics.FillRectangle(ExtenderBorderBrush, rc1);
				}
				var arrowSize = conv.ConvertX(4);
				var p1 = new PointF(rect.Right - w / 2, rect.Y + (rect.Height + arrowSize) / 2);
				var p2 = new PointF(p1.X + arrowSize, p1.Y - arrowSize);
				var p3 = new PointF(p1.X - arrowSize + 1, p1.Y - arrowSize);
				var triangle = new PointF[3] { p1, p2, p3, };
				using(var brush = new SolidBrush(Color.FromArgb(76, 96, 122)))
				{
					graphics.FillPolygon(brush, triangle);
				}
			}
		}
	}

	/// <inheritdoc/>
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

		bool hovered  = (state & ItemState.Hovered)  == ItemState.Hovered;
		bool selected = (state & ItemState.Selected) == ItemState.Selected;
		bool focused  = (state & ItemState.Focused)  == ItemState.Focused;
		IBackgroundStyle background = null;
		if(selected)
		{
			if(paintEventArgs.IsHostControlFocused)
			{
				if(hovered)
				{
					background = BackgroundStyle.SelectedFocused;
				}
				else if(focused)
				{
					background = BackgroundStyle.SelectedFocused;
				}
				else
				{
					background = BackgroundStyle.Selected;
				}
			}
			else
			{
				if(hovered)
				{
					background = BackgroundStyle.SelectedFocused;
				}
				else
				{
					background = BackgroundStyle.SelectedNoFocus;
				}
			}
		}
		else
		{
			if(hovered)
			{
				if(focused && paintEventArgs.IsHostControlFocused)
				{
					background = BackgroundStyle.HoveredFocused;
				}
				else
				{
					background = BackgroundStyle.Hovered;
				}
			}
			else if(focused)
			{
				if(paintEventArgs.IsHostControlFocused)
				{
					background = BackgroundStyle.Focused;
				}
			}
		}
		background?.Draw(paintEventArgs.Graphics, paintEventArgs.Dpi, paintEventArgs.Bounds);
	}

	private static void PaintCheckBox(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs, int x, ref int offset, ref int w2)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(paintEventArgs);

		var iconLookup = paintEventArgs.HoveredPart == ItemHitTestResults.CheckBox
			? ImgCheckedStateHovered
			: ImgCheckedState;
		var image = iconLookup.TryGetValue(item.CheckedState, out var imageProvider)
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
					bounds.Y + (bounds.Height - imageSize.Width) / 2,
					w2 - spaceBefore,
					imageSize.Height);
				srcRect = new Rectangle(
					0, 0,
					image.Width * (w2 - spaceBefore) / imageSize.Width,
					image.Height);
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
					image.Width,
					image.Height);
			}

			paintEventArgs.Graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
		}

		var spaceAfter = ListBoxConstants.SpaceAfterCheckbox.GetValue(paintEventArgs.Dpi);
		var dx         = spaceBefore + spaceAfter + imageSize.Width;

		offset += dx;
		w2     -= dx;
	}

	private static void PaintTreeLines(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs, int x, ref int offset, ref int w2)
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

		var spaceBefore = ListBoxConstants.SpaceBeforePlusMinus.GetValue(paintEventArgs.Dpi);
		var imageSize   = ListBoxConstants.PlusMinusImageSize.GetValue(paintEventArgs.Dpi);

		if(level != 0 || listBox.ShowRootTreeLines)
		{
			if(w2 > spaceBefore && item.Items.Count != 0)
			{
				IImageProvider imageProvider;
				if(paintEventArgs.HoveredPart == ItemHitTestResults.PlusMinus)
				{
					imageProvider = item.IsExpanded ? CommonIcons.TreeLines.MinusHover : CommonIcons.TreeLines.PlusHover;
				}
				else
				{
					imageProvider = item.IsExpanded ? CommonIcons.TreeLines.Minus : CommonIcons.TreeLines.Plus;
				}
				var image = imageProvider.GetImage(16 * paintEventArgs.Dpi.X / 96);
				var bounds = paintEventArgs.Bounds;
				Rectangle destRect, srcRect;
				if(w2 < imageSize.Width + spaceBefore)
				{
					destRect = new Rectangle(
						x + offset,
						bounds.Y + (bounds.Height - imageSize.Width) / 2,
						w2 - spaceBefore,
						imageSize.Height);
					srcRect = new Rectangle(
						0,
						0,
						image.Width * (w2 - spaceBefore) / imageSize.Width,
						image.Height);
				}
				else
				{
					destRect = new Rectangle(
						x + offset,
						bounds.Y + (bounds.Height - imageSize.Width) / 2,
						imageSize.Width,
						imageSize.Height);
					srcRect = new Rectangle(
						0, 0,
						image.Width,
						image.Height);
				}
				paintEventArgs.Graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
			}

			var spaceAfter = ListBoxConstants.SpaceAfterPlusMinus.GetValue(paintEventArgs.Dpi);
			var dx = spaceBefore + spaceAfter + imageSize.Width;

			offset += dx;
			w2     -= dx;
		}
	}

	public override void OnPaintItemContent(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(paintEventArgs);

		var rect = paintEventArgs.Bounds;

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
			if(!column.IsVisible) continue;

			int columnWidth = column.CurrentWidth;

			if(first)
			{
				first = false;
				var listBox = item.ListBox;
				int offset  = item.Level * ListBoxConstants.LevelMargin.GetValue(paintEventArgs.Dpi) + ListBoxConstants.RootMargin;
				int w2      = columnWidth - offset;

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
