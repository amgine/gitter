﻿#region Copyright Notice
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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;

	using gitter.Framework.Controls;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class MSVS2012CustomListBoxRenderer : CustomListBoxRenderer
	{
		#region Static Data

		private static readonly Dictionary<CheckedState, Bitmap> ImgCheckedState =
			new Dictionary<CheckedState, Bitmap>()
			{
				{ CheckedState.Checked,			CachedResources.Bitmaps["ImgChecked"]					},
				{ CheckedState.Unchecked,		CachedResources.Bitmaps["ImgUnchecked"]					},
				{ CheckedState.Indeterminate,	CachedResources.Bitmaps["ImgCheckIndeterminate"]		},
			};

		private static readonly Dictionary<CheckedState, Bitmap> ImgCheckedStateHovered =
			new Dictionary<CheckedState, Bitmap>()
			{
				{ CheckedState.Checked,			CachedResources.Bitmaps["ImgCheckedHover"]				},
				{ CheckedState.Unchecked,		CachedResources.Bitmaps["ImgUncheckedHover"]			},
				{ CheckedState.Indeterminate,	CachedResources.Bitmaps["ImgCheckIndeterminateHover"]	},
			};

		private static readonly PointF[] _triangle = new PointF[3];

		#endregion

		#region Color Tables

		public static readonly IColorTable DarkColors = new DarkColorTable();
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

			#region IColorTable

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

			#endregion
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

			#region IColorTable

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

			#endregion
		}

		#endregion

		#region .ctor

		public MSVS2012CustomListBoxRenderer(IColorTable colorTable)
		{
			Verify.Argument.IsNotNull(colorTable, nameof(colorTable));

			ColorTable = colorTable;
		}

		#endregion

		private IColorTable ColorTable { get; }

		public override Color BackColor
		{
			get { return ColorTable.Background; }
		}

		public override Color ForeColor
		{
			get { return ColorTable.Text; }
		}

		public override Color ColumnHeaderForeColor
		{
			get { return ColorTable.Text; }
		}

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
			if(column.Extender != null && bounds.Width > CustomListBoxColumn.ExtenderButtonWidth)
			{
				var graphics = paintEventArgs.Graphics;
				using(var brush = new SolidBrush(ColorTable.Background))
				{
					graphics.FillRectangle(brush,
						bounds.Right - CustomListBoxColumn.ExtenderButtonWidth - 0.5f, bounds.Y,
						1, bounds.Height);
				}
				var foregroundColor = paintEventArgs.HoveredPart == ColumnHitTestResults.Extender ?
					ColorTable.HoverExtenderForeground : ColorTable.ExtenderForeground;
				using(var brush = new SolidBrush(foregroundColor))
				{
					const int ArrowSize = 4;
					var p1 = new Point(
						bounds.Right - CustomListBoxColumn.ExtenderButtonWidth + CustomListBoxColumn.ExtenderButtonWidth / 2,
						bounds.Y + bounds.Height - (bounds.Height - ArrowSize) / 2);
					var p2 = new Point(p1.X + ArrowSize, p1.Y - ArrowSize);
					var p3 = new Point(p1.X - ArrowSize + 1, p1.Y - ArrowSize);
					_triangle[0] = p1;
					_triangle[1] = p2;
					_triangle[2] = p3;
					graphics.FillPolygon(brush, _triangle);
				}
			}
		}

		private static void RenderColumnNormalBackground(ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var bounds = paintEventArgs.Bounds;
			var c1 = Color.FromArgb(63, 63, 70);
			using(var pen = new Pen(c1))
			{
				graphics.DrawLine(pen, bounds.Right - 1, 0, bounds.Right - 1, bounds.Bottom - 1);
				graphics.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
			}
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
			using(var brush = new SolidBrush(c2))
			{
				var rc = bounds;
				rc.Width -= 1;
				rc.Height -= 1;
				graphics.FillRectangle(brush, rc);
			}
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
			using(var brush = new SolidBrush(c2))
			{
				var rc = bounds;
				rc.Width -= 1;
				rc.Height -= 1;
				graphics.FillRectangle(brush, rc);
			}
			PaintColumnExtender(column, paintEventArgs);
		}

		public override void OnPaintColumnContent(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var font = column.HeaderFont;

			ItemPaintEventArgs.PrepareContentRectangle(ref rect);
			paintEventArgs.PrepareTextRectangle(font, font, ref rect);
			if(column.Extender != null && ((paintEventArgs.State & (ItemState.Hovered | ItemState.Pressed)) != ItemState.None))
			{
				rect.Width -= CustomListBoxColumn.ExtenderButtonWidth;
				if(rect.Width <= 0) return;
			}
			StringFormat format;
			switch(column.HeaderAlignment)
			{
				case StringAlignment.Near:
					format = GitterApplication.TextRenderer.LeftAlign;
					break;
				case StringAlignment.Far:
					format = GitterApplication.TextRenderer.RightAlign;
					break;
				case StringAlignment.Center:
					format = GitterApplication.TextRenderer.CenterAlign;
					break;
				default:
					format = GitterApplication.TextRenderer.LeftAlign;
					break;
			}
			GitterApplication.TextRenderer.DrawText(graphics, column.Name, font, column.HeaderBrush, rect, format);
		}

		public override void OnPaintItemBackground(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs)
		{
			var state = paintEventArgs.State;

			if(state == ItemState.None) return;

			bool isHovered	= (state & ItemState.Hovered) == ItemState.Hovered;
			bool isSelected	= (state & ItemState.Selected) == ItemState.Selected;
			bool isFocused	= (state & ItemState.Focused) == ItemState.Focused;
			Brush brush = null;
			Pen pen = null;
			if(isSelected)
			{
				if(paintEventArgs.IsHostControlFocused)
				{
					if(isHovered)
					{
						brush = new SolidBrush(ColorTable.SelectionBackground);
					}
					else if(isFocused)
					{
						brush = new SolidBrush(ColorTable.SelectionBackground);
					}
					else
					{
						brush = new SolidBrush(ColorTable.SelectionBackground);
					}
				}
				else
				{
					if(isHovered)
					{
						brush = new SolidBrush(ColorTable.SelectionBackgroundNoFocus);
					}
					else
					{
						brush = new SolidBrush(ColorTable.SelectionBackgroundNoFocus);
					}
				}
			}
			else
			{
				if(isHovered)
				{
					if(isFocused && paintEventArgs.IsHostControlFocused)
					{
						brush = new SolidBrush(ColorTable.HoverBackground);
						pen = new Pen(ColorTable.FocusBorder);
					}
					else
					{
						brush = new SolidBrush(ColorTable.HoverBackground);
					}
				}
				else if(isFocused)
				{
					if(paintEventArgs.IsHostControlFocused)
					{
						pen = new Pen(ColorTable.FocusBorder);
					}
				}
			}
			if(brush != null)
			{
				var itemBounds = Rectangle.Intersect(paintEventArgs.ClipRectangle, paintEventArgs.Bounds);
				if(itemBounds.Width > 0 && itemBounds.Height > 0)
				{
					paintEventArgs.Graphics.FillRectangle(brush, itemBounds);
				}
				brush.Dispose();
			}
			if(pen != null)
			{
				var rect = paintEventArgs.Bounds;
				rect.Width -= 1;
				rect.Height -= 1;
				paintEventArgs.Graphics.DrawRectangle(pen, rect);
				pen.Dispose();
			}
		}

		private static void CacheMinusTrianglePolygon1(int x, int y)
		{
			var p0 = new PointF(x + 10.5f, y + 3.5f);
			_triangle[0] = p0;
			_triangle[1] = new PointF(p0.X + 0, p0.Y + 7f);
			_triangle[2] = new PointF(p0.X - 7f, p0.Y + 7f);
		}

		private static void CacheMinusTrianglePolygon2(int x, int y)
		{
			var p0 = new PointF(x + 9.5f, y + 6f);
			_triangle[0] = p0;
			_triangle[1] = new PointF(p0.X + 0, p0.Y + 3.5f);
			_triangle[2] = new PointF(p0.X - 3.5f, p0.Y + 3.5f);
		}

		private static void CachePlusTrianglePolygon1(int x, int y)
		{
			var p0 = new PointF(x + 5.5f, y + 2.5f);
			_triangle[0] = p0;
			_triangle[1] = new PointF(p0.X + 5, p0.Y + 5.5f);
			_triangle[2] = new PointF(p0.X + 0, p0.Y + 10.5f);
		}

		private static void CachePlusTrianglePolygon2(int x, int y)
		{
			var p0 = new PointF(x + 6.5f, y + 4.5f);
			_triangle[0] = p0;
			_triangle[1] = new PointF(p0.X + 3, p0.Y + 3.5f);
			_triangle[2] = new PointF(p0.X + 0, p0.Y + 6.5f);
		}

		private void RenderPlusMinus(Graphics graphics, int x, int y, bool isExpanded, bool isSelected, bool isPlusMinusHovered, bool isHostControlFocused)
		{
			var oldMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			if(isExpanded)
			{
				if(isPlusMinusHovered)
				{
					if(isHostControlFocused && isSelected)
					{
						CacheMinusTrianglePolygon1(x, y);
						using(var brush = new SolidBrush(ColorTable.PlusMinusForeground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
						CacheMinusTrianglePolygon2(x, y);
						using(var brush = new SolidBrush(ColorTable.SelectionBackground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
					}
					else
					{
						CacheMinusTrianglePolygon1(x, y);
						using(var brush = new SolidBrush(ColorTable.AccentPlusMinusForeground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
					}
				}
				else
				{
					CacheMinusTrianglePolygon1(x, y);
					using(var brush = new SolidBrush(ColorTable.PlusMinusForeground))
					{
						graphics.FillPolygon(brush, _triangle);
					}
				}
			}
			else
			{
				if(isPlusMinusHovered)
				{
					if(isHostControlFocused && isSelected)
					{
						CachePlusTrianglePolygon1(x, y);
						using(var brush = new SolidBrush(ColorTable.PlusMinusForeground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
					}
					else
					{
						CachePlusTrianglePolygon1(x, y);
						using(var brush = new SolidBrush(ColorTable.AccentPlusMinusForeground))
						{
							graphics.FillPolygon(brush, _triangle);
						}
						CachePlusTrianglePolygon2(x, y);
						if(isSelected)
						{
							if(isHostControlFocused)
							{
								using(var brush = new SolidBrush(ColorTable.SelectionBackground))
								{
									graphics.FillPolygon(brush, _triangle);
								}
							}
							else
							{
								using(var brush = new SolidBrush(ColorTable.SelectionBackgroundNoFocus))
								{
									graphics.FillPolygon(brush, _triangle);
								}
							}
						}
						else
						{
							using(var brush = new SolidBrush(ColorTable.Background))
							{
								graphics.FillPolygon(brush, _triangle);
							}
						}
					}
				}
				else
				{
					CachePlusTrianglePolygon1(x, y);
					using(var brush = new SolidBrush(ColorTable.PlusMinusForeground))
					{
						graphics.FillPolygon(brush, _triangle);
					}
					CachePlusTrianglePolygon2(x, y);
					if(isSelected)
					{
						if(isHostControlFocused)
						{
							using(var brush = new SolidBrush(ColorTable.SelectionBackground))
							{
								graphics.FillPolygon(brush, _triangle);
							}
						}
						else
						{
							using(var brush = new SolidBrush(ColorTable.SelectionBackgroundNoFocus))
							{
								graphics.FillPolygon(brush, _triangle);
							}
						}
					}
					else
					{
						using(var brush = new SolidBrush(ColorTable.Background))
						{
							graphics.FillPolygon(brush, _triangle);
						}
					}
				}
			}
			graphics.SmoothingMode = oldMode;
		}

		public override void OnPaintItemContent(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
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

						int x2 = x + column.Width;

						if(startColumnId == -1 && x2 > clipX1)
						{
							if(prev != -1 && columns[prev].ExtendsToRight)
							{
								startColumnId = prev;
								startX = x - columns[prev].Width;
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
					int columnWidth = column.Width;

					if(first)
					{
						first = false;
						var level = item.Level;
						var listBox = item.ListBox;
						int offset = level * ListBoxConstants.LevelMargin + ListBoxConstants.RootMargin;
						int w2 = columnWidth - offset;

						#region paint plus/minus

						if(listBox.ShowTreeLines)
						{
							if(!listBox.ShowRootTreeLines)
							{
								if(level != 0)
								{
									offset -= ListBoxConstants.LevelMargin;
									w2 += ListBoxConstants.LevelMargin;
								}
							}
							if(level != 0 || listBox.ShowRootTreeLines)
							{
								if(w2 > ListBoxConstants.SpaceBeforePlusMinus && item.Items.Count != 0)
								{
									RenderPlusMinus(
										graphics,
										x + offset, subrect.Y + (subrect.Height - ListBoxConstants.PlusMinusImageWidth) / 2,
										item.IsExpanded,
										(paintEventArgs.State & ItemState.Selected) == ItemState.Selected,
										hoveredPart == ItemHitTestResults.PlusMinus,
										paintEventArgs.IsHostControlFocused);
								}
								offset += ListBoxConstants.PlusMinusAreaWidth;
								w2 -= ListBoxConstants.PlusMinusAreaWidth;
							}
						}

						#endregion

						#region paint checkbox

						if(listBox.ShowCheckBoxes && item.CheckedState != CheckedState.Unavailable)
						{
							Bitmap checkedStateImage = null;
							if(hoveredPart == ItemHitTestResults.CheckBox)
							{
								ImgCheckedStateHovered.TryGetValue(item.CheckedState, out checkedStateImage);
							}
							else
							{
								ImgCheckedState.TryGetValue(item.CheckedState, out checkedStateImage);
							}
							if(checkedStateImage != null && w2 > ListBoxConstants.SpaceBeforeCheckbox)
							{
								Rectangle destRect, srcRect;
								if(w2 < ListBoxConstants.CheckboxImageWidth + ListBoxConstants.SpaceBeforeCheckbox)
								{
									destRect = new Rectangle(
										x + offset + ListBoxConstants.SpaceBeforeCheckbox,
										rect.Y + (rect.Height - ListBoxConstants.CheckboxImageWidth) / 2,
										w2 - ListBoxConstants.SpaceBeforeCheckbox,
										ListBoxConstants.CheckboxImageWidth);
									srcRect = new Rectangle(
										0, 0,
										w2 - ListBoxConstants.SpaceBeforeCheckbox,
										ListBoxConstants.CheckboxImageWidth);
								}
								else
								{
									destRect = new Rectangle(
										x + offset + ListBoxConstants.SpaceBeforeCheckbox,
										rect.Y + (rect.Height - ListBoxConstants.CheckboxImageWidth) / 2,
										ListBoxConstants.CheckboxImageWidth,
										ListBoxConstants.CheckboxImageWidth);
									srcRect = new Rectangle(
										0, 0,
										ListBoxConstants.CheckboxImageWidth,
										ListBoxConstants.CheckboxImageWidth);
								}
								graphics.DrawImage(checkedStateImage, destRect, srcRect, GraphicsUnit.Pixel);
							}
							offset += ListBoxConstants.CheckBoxAreaWidth;
							w2 -= ListBoxConstants.CheckBoxAreaWidth;
						}

						#endregion

						subrect.X = x + offset;
						subrect.Width = w2;
						x += columnWidth;
						if(w2 <= 0) continue;
					}
					else
					{
						subrect.X = x;
						subrect.Width = columnWidth;
						x += columnWidth;
					}

					item.PaintSubItem(new SubItemPaintEventArgs(paintEventArgs.Graphics, clip, subrect, paintEventArgs.Index,
						paintEventArgs.State, hoveredPart, paintEventArgs.IsHostControlFocused, item, i, column));
				}
			}
		}
	}
}
