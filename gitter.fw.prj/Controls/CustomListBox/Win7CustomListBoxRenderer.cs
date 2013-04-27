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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class Win7CustomListBoxRenderer : CustomListBoxRenderer
	{
		#region Static Data

		private static readonly Brush ExtenderHoveredBrush	= new SolidBrush(Color.FromArgb(210, 229, 253));
		private static readonly Brush ExtenderBorderBrush	= new LinearGradientBrush(Point.Empty, new Point(0, 23), Color.FromArgb(223, 234, 247), Color.FromArgb(238, 242, 249));

		private static readonly Pen ExtenderBorderPenHovered = new Pen(Color.FromArgb(215, 227, 241));

		private static readonly Bitmap ImgColumnExtender	= CachedResources.Bitmaps["ImgColumnExtender"];
		private static readonly Bitmap ImgCollapse			= CachedResources.Bitmaps["ImgMinus"];
		private static readonly Bitmap ImgCollapseHovered	= CachedResources.Bitmaps["ImgMinusHovered"];
		private static readonly Bitmap ImgExpand			= CachedResources.Bitmaps["ImgPlus"];
		private static readonly Bitmap ImgExpandHovered		= CachedResources.Bitmaps["ImgPlusHovered"];

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
			using(var p = new Pen(c1))
			{
				var rc = rect;
				rc.Y -= 1;
				rc.X += 1;
				rc.Width -= 2;
				graphics.DrawRectangle(p, rc);
			}
			using(var b = new SolidBrush(c2))
			{
				var rc = rect;
				rc.Y += 3;
				rc.X += 2;
				rc.Width -= 4;
				rc.Height -= 4;
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
			if(column.Extender != null)
			{
				if(rect.Width > CustomListBoxColumn.ExtenderButtonWidth)
				{
					graphics.FillRectangle(ExtenderBorderBrush, rect.Right - CustomListBoxColumn.ExtenderButtonWidth - 0.5f, rect.Y, 1, rect.Height - 1);
					graphics.DrawImage(ImgColumnExtender, rect.Right - CustomListBoxColumn.ExtenderButtonWidth + 4, rect.Y + 9, 7, 4);
				}
			}
		}

		private static void RenderColumnHoverBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var c1 = Color.FromArgb(227, 232, 238);
			var c2 = Color.FromArgb(241, 245, 251);
			using(var p = new Pen(c1))
			{
				var rc = rect;
				rc.Y -= 1;
				rc.Width -= 1;
				graphics.DrawRectangle(p, rc);
			}
			using(var b = new SolidBrush(c2))
			{
				var rc = rect;
				rc.X += 2;
				rc.Y += 1;
				rc.Width -= 4;
				rc.Height -= 3;
				graphics.FillRectangle(b, rc);
			}
			if(column.Extender != null)
			{
				if(rect.Width > CustomListBoxColumn.ExtenderButtonWidth)
				{
					if(paintEventArgs.HoveredPart == ColumnHitTestResults.Extender)
					{
						graphics.FillRectangle(ExtenderHoveredBrush,
							rect.Right - CustomListBoxColumn.ExtenderButtonWidth + 1.5f, rect.Y + 1.5f,
							CustomListBoxColumn.ExtenderButtonWidth - 4, rect.Height - 4);
						graphics.DrawRectangle(ExtenderBorderPenHovered,
							rect.Right - CustomListBoxColumn.ExtenderButtonWidth, 0,
							CustomListBoxColumn.ExtenderButtonWidth - 1, rect.Height - 1);
					}
					else
					{
						graphics.FillRectangle(ExtenderBorderBrush,
							rect.Right - CustomListBoxColumn.ExtenderButtonWidth - 0.5f, rect.Y,
							1, rect.Height);
					}
					graphics.DrawImage(ImgColumnExtender,
						rect.Right - CustomListBoxColumn.ExtenderButtonWidth + 4, rect.Y + 9,
						7, 4);
				}
			}
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

			bool hovered = (state & ItemState.Hovered) == ItemState.Hovered;
			bool selected = (state & ItemState.Selected) == ItemState.Selected;
			bool focused = (state & ItemState.Focused) == ItemState.Focused;
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
			if(background != null)
			{
				background.Draw(paintEventArgs.Graphics, paintEventArgs.Bounds);
			}
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
									Bitmap image;
									if(hoveredPart == ItemHitTestResults.PlusMinus)
									{
										image = (item.IsExpanded) ? (ImgCollapseHovered) : (ImgExpandHovered);
									}
									else
									{
										image = (item.IsExpanded) ? (ImgCollapse) : (ImgExpand);
									}
									Rectangle destRect, srcRect;
									if(w2 < ListBoxConstants.PlusMinusImageWidth + ListBoxConstants.SpaceBeforePlusMinus)
									{
										destRect = new Rectangle(
											x + offset,
											subrect.Y + (subrect.Height - ListBoxConstants.PlusMinusImageWidth) / 2,
											w2 - ListBoxConstants.SpaceBeforePlusMinus,
											ListBoxConstants.PlusMinusImageWidth);
										srcRect = new Rectangle(
											0,
											0,
											w2 - ListBoxConstants.SpaceBeforePlusMinus,
											ListBoxConstants.PlusMinusImageWidth);
									}
									else
									{
										destRect = new Rectangle(
											x + offset,
											subrect.Y + (subrect.Height - ListBoxConstants.PlusMinusImageWidth) / 2,
											ListBoxConstants.PlusMinusImageWidth,
											ListBoxConstants.PlusMinusImageWidth);
										srcRect = new Rectangle(
											0, 0,
											ListBoxConstants.PlusMinusImageWidth,
											ListBoxConstants.PlusMinusImageWidth);
									}
									graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
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
						paintEventArgs.State, hoveredPart, paintEventArgs.IsHostControlFocused, i, column));
				}
			}
		}
	}
}
