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

namespace gitter.Git.Gui;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using gitter.Framework;

public sealed class GraphStyle : IGraphStyle, IDisposable
{
	public event EventHandler Changed;

	private void OnChanged(EventArgs e) => Changed?.Invoke(this, e);

	private const float HoverLightAmount = 0.4f;

	const int CornerExtensinon = 1;
	const int DotMargin        = 1;

	private static readonly Brush TagBrush = new SolidBrush(ColorScheme.TagBackColor);
	private static readonly Brush LocalBranchBrush = new SolidBrush(ColorScheme.LocalBranchBackColor);
	private static readonly Brush RemoteBranchBrush = new SolidBrush(ColorScheme.RemoteBranchBackColor);
	private static readonly Brush StashBrush = new SolidBrush(ColorScheme.StashBackColor);

	private static readonly Brush TagBrushHovered          = new SolidBrush(ColorScheme.TagBackColor.Lighter(HoverLightAmount));
	private static readonly Brush LocalBranchBrushHovered  = new SolidBrush(ColorScheme.LocalBranchBackColor.Lighter(HoverLightAmount));
	private static readonly Brush RemoteBranchBrushHovered = new SolidBrush(ColorScheme.RemoteBranchBackColor.Lighter(HoverLightAmount));
	private static readonly Brush StashBrushHovered        = new SolidBrush(ColorScheme.StashBackColor.Lighter(HoverLightAmount));

	[ThreadStatic]
	private static readonly Point [] TagPoints        = new Point [6];

	[ThreadStatic]
	private static readonly PointF[] CurrentIndicator = new PointF[3];

	[ThreadStatic]
	private static readonly Point [] Polyline         = new Point [4];

	[ThreadStatic]
	private static readonly Pen _pen = new(Color.White, 1);

	public GraphStyle(IValueProvider<GraphStyleOptions> optionsProvider)
	{
		Verify.Argument.IsNotNull(optionsProvider);

		OptionsProvider = optionsProvider;
		OptionsProvider.ValueChanged += OnOptionsProviderValueChanged;
	}

	public void Dispose()
	{
		OptionsProvider.ValueChanged -= OnOptionsProviderValueChanged;
	}

	private void OnOptionsProviderValueChanged(object sender, EventArgs e)
	{
		OnChanged(EventArgs.Empty);
	}

	private IValueProvider<GraphStyleOptions> OptionsProvider { get; }

	private static int GetGraphLineWidth(Dpi dpi, int baseWidth) => Math.Max(1, baseWidth + (dpi.X - 96) / (96 / 2));
	//private static int GetGraphLineWidth(Dpi dpi, int baseWidth) => Math.Max(1, baseWidth * (dpi.X + 95) / 96);

	static void PaintVerticalLine(Graphics graphics, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors)
	{
		Assert.IsNotNull(graphics);

		if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.VerticalTop)];
		var x  = cellBounds.X + (cellBounds.Width >> 1);
		var y0 = cellBounds.Y;
		var y1 = cellBounds.Y + cellBounds.Height;
		graphics.DrawLine(_pen, x, y0, x, y1);
	}

	private static void PaintVerticalLines(GraphStyleOptions options, Graphics graphics, Dpi dpi, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors)
	{
		Assert.IsNotNull(options);
		Assert.IsNotNull(graphics);

		if(!cell.HasAnyOfElements(GraphElement.Vertical)) return;

		var @break = options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
		var x = cellBounds.X + (cellBounds.Width >> 1);
		var color0 = cell.ColorOf(GraphElementId.VerticalTop);
		var color1 = cell.ColorOf(GraphElementId.VerticalBottom);
		if(!@break && cell.HasElement(GraphElement.Vertical) && (!useColors || (color0 == color1)))
		{
			if(useColors) _pen.Color = palette[color0];
			var y0 = cellBounds.Y;
			var y1 = cellBounds.Y + cellBounds.Height;
			graphics.DrawLine(_pen, x, y0, x, y1);
		}
		else
		{
			if(cell.HasElement(GraphElement.VerticalTop))
			{
				if(useColors) _pen.Color = palette[color0];
				var dy = @break ? (options.NodeRadius * dpi.Y / 96 + DotMargin) : 0;
				var y0 = cellBounds.Y;
				var y1 = y0 + (cellBounds.Height >> 1) - dy;
				graphics.DrawLine(_pen, x, y0, x, y1);
			}
			if(cell.HasElement(GraphElement.VerticalBottom))
			{
				if(useColors) _pen.Color = palette[color1];
				var dy = @break ? (options.NodeRadius * dpi.Y / 96 + DotMargin + 1) : 0;
				var y0 = cellBounds.Y + (cellBounds.Height >> 1) + dy;
				var y1 = cellBounds.Y +  cellBounds.Height;
				graphics.DrawLine(_pen, x, y0, x, y1);
			}
		}
	}

	static void PaintHorizontalLine(Graphics graphics, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors)
	{
		Assert.IsNotNull(graphics);

		if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.HorizontalLeft)];

		var y  = cellBounds.Y + (cellBounds.Height >> 1);
		var x0 = cellBounds.X;
		var x1 = x0 + cellBounds.Width;
		graphics.DrawLine(_pen, x0, y, x1, y);
	}

	private static void PaintLeftLine(GraphStyleOptions options, Graphics graphics, Dpi dpi, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors)
	{
		Assert.IsNotNull(options);
		Assert.IsNotNull(graphics);

		var @break = options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
		if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.HorizontalLeft)];
		var dx = @break ? (options.NodeRadius * dpi.X / 96 + DotMargin) : 0;
		var y  = cellBounds.Y + (cellBounds.Height >> 1);
		var x0 = cellBounds.X;
		var x1 = x0 + (cellBounds.Width >> 1) - dx;
		graphics.DrawLine(_pen, x0, y, x1, y);
	}

	private static void PaintRightLine(GraphStyleOptions options, Graphics graphics, Dpi dpi, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors)
	{
		Assert.IsNotNull(options);
		Assert.IsNotNull(graphics);

		var @break = options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
		if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.HorizontalRight)];
		var y  = cellBounds.Y + (cellBounds.Height >> 1);
		var dx = @break ? (options.NodeRadius * dpi.X / 96 + DotMargin + 1) : 0;
		var x0 = cellBounds.X + (cellBounds.Width >> 1) + dx;
		var x1 = cellBounds.X +  cellBounds.Width;
		graphics.DrawLine(_pen, x0, y, x1, y);
	}

	private static void PaintHorizontalLines(GraphStyleOptions options, Graphics graphics, Dpi dpi, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors)
	{
		Assert.IsNotNull(graphics);
		Assert.IsNotNull(options);

		if(!cell.HasAnyOfElements(GraphElement.Horizontal)) return;

		var @break = options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
		var y = cellBounds.Y + (cellBounds.Height >> 1);
		var color0 = cell.ColorOf(GraphElementId.HorizontalLeft);
		var color1 = cell.ColorOf(GraphElementId.HorizontalRight);
		if(!@break && cell.HasElement(GraphElement.Horizontal) && (!useColors || (color0 == color1)))
		{
			if(useColors) _pen.Color = palette[color0];
			var x0 = cellBounds.X;
			var x1 = x0 + cellBounds.Width;
			graphics.DrawLine(_pen, x0, y, x1, y);
		}
		else
		{
			if(cell.HasElement(GraphElement.HorizontalLeft))
			{
				if(useColors) _pen.Color = palette[color0];
				var dx = @break ? (options.NodeRadius * dpi.X / 96 + DotMargin) : 0;
				var x0 = cellBounds.X;
				var x1 = x0 + (cellBounds.Width >> 1) - dx;
				graphics.DrawLine(_pen, x0, y, x1, y);
			}
			if(cell.HasElement(GraphElement.HorizontalRight))
			{
				if(useColors) _pen.Color = palette[color1];
				var dx = @break ? (options.NodeRadius * dpi.X / 96 + DotMargin + 1) : 0;
				var x0 = cellBounds.X + (cellBounds.Width >> 1) + dx;
				var x1 = cellBounds.X +  cellBounds.Width;
				graphics.DrawLine(_pen, x0, y, x1, y);
			}
		}
	}

	static void SetupLeftTopConer(Rectangle cellBounds)
	{
		Polyline[0].X = cellBounds.X -  CornerExtensinon;
		Polyline[0].Y = cellBounds.Y + (cellBounds.Height >> 1);
		Polyline[1].X = cellBounds.X;
		Polyline[1].Y = cellBounds.Y + (cellBounds.Height >> 1);
		Polyline[2].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[2].Y = cellBounds.Y;
		Polyline[3].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[3].Y = cellBounds.Y -  CornerExtensinon;
	}

	static void SetupRightTopCorner(Rectangle cellBounds)
	{
		Polyline[0].X = cellBounds.X +  cellBounds.Width + CornerExtensinon;
		Polyline[0].Y = cellBounds.Y + (cellBounds.Height >> 1);
		Polyline[1].X = cellBounds.X +  cellBounds.Width - 1;
		Polyline[1].Y = cellBounds.Y + (cellBounds.Height >> 1);
		Polyline[2].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[2].Y = cellBounds.Y;
		Polyline[3].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[3].Y = cellBounds.Y -  CornerExtensinon;
	}

	static void SetupLeftBottomCorner(Rectangle cellBounds)
	{
		Polyline[0].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[0].Y = cellBounds.Y +  cellBounds.Height + CornerExtensinon;
		Polyline[1].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[1].Y = cellBounds.Y +  cellBounds.Height - 1;
		Polyline[2].X = cellBounds.X;
		Polyline[2].Y = cellBounds.Y + (cellBounds.Height >> 1);
		Polyline[3].X = cellBounds.X -  CornerExtensinon;
		Polyline[3].Y = cellBounds.Y + (cellBounds.Height >> 1);
	}

	static void SetupRightBottomCorner(Rectangle cellBounds)
	{
		Polyline[0].X = cellBounds.X +  cellBounds.Width + CornerExtensinon;
		Polyline[0].Y = cellBounds.Y + (cellBounds.Height >> 1);
		Polyline[1].X = cellBounds.X +  cellBounds.Width - 1;
		Polyline[1].Y = cellBounds.Y + (cellBounds.Height >> 1);
		Polyline[2].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[2].Y = cellBounds.Y +  cellBounds.Height - 1;
		Polyline[3].X = cellBounds.X + (cellBounds.Width  >> 1);
		Polyline[3].Y = cellBounds.Y +  cellBounds.Height + CornerExtensinon;
	}

	private static void PaintTopCornerLines(GraphStyleOptions options, Graphics graphics, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors, int lineWidth)
	{
		Assert.IsNotNull(options);
		Assert.IsNotNull(graphics);

		if(cell.HasElement(GraphElement.LeftTopCorner))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.LeftTopCorner)];
			if(options.RoundedCorners)
			{
				RectangleF b = cellBounds;
				var delta = 0.5f + ((lineWidth + 1) & 1) * 0.5f;
				b.X -= b.Width  / 2.0f + delta;
				b.Y -= b.Height / 2.0f + delta;
				using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
				{
					graphics.DrawArc(_pen, b, -5, 100);
				}
			}
			else
			{
				SetupLeftTopConer(cellBounds);
				graphics.DrawLines(_pen, Polyline);
			}
		}
		if(cell.HasElement(GraphElement.RightTopCorner))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.RightTopCorner)];
			if(options.RoundedCorners)
			{
				RectangleF b = cellBounds;
				var delta = 0.5f + ((lineWidth + 1) & 1) * 0.5f;
				b.X += b.Width  / 2.0f - delta;
				b.Y -= b.Height / 2.0f + delta;
				using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
				{
					graphics.DrawArc(_pen, b, 80, 100);
				}
			}
			else
			{
				SetupRightTopCorner(cellBounds);
				graphics.DrawLines(_pen, Polyline);
			}
		}
	}

	private static void PaintBottomCornerLines(GraphStyleOptions options, Graphics graphics, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors, int lineWidth)
	{
		Assert.IsNotNull(options);
		Assert.IsNotNull(graphics);

		if(cell.HasElement(GraphElement.LeftBottomCorner))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.LeftBottomCorner)];
			if(options.RoundedCorners)
			{
				RectangleF b = cellBounds;
				var delta = 0.5f + ((lineWidth + 1) & 1) * 0.5f;
				b.X -= b.Width  / 2.0f + delta;
				b.Y += b.Height / 2.0f - delta;
				using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
				{
					graphics.DrawArc(_pen, b, 265, 100);
				}
			}
			else
			{
				SetupLeftBottomCorner(cellBounds);
				graphics.DrawLines(_pen, Polyline);
			}
		}
		if(cell.HasElement(GraphElement.RightBottomCorner))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.RightBottomCorner)];
			if(options.RoundedCorners)
			{
				RectangleF b = cellBounds;
				var delta = 0.5f + ((lineWidth + 1) & 1) * 0.5f;
				b.X += b.Width  / 2.0f - delta;
				b.Y += b.Height / 2.0f - delta;
				using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
				{
					graphics.DrawArc(_pen, b, 175, 100);
				}
			}
			else
			{
				SetupRightBottomCorner(cellBounds);
				graphics.DrawLines(_pen, Polyline);
			}
		}
	}

	static void PaintDiagonalLines(Graphics graphics, GraphCell cell, Rectangle cellBounds, Color[] palette, bool useColors)
	{
		Assert.IsNotNull(graphics);

		if(cell.HasElement(GraphElement.LeftTop))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.LeftTop)];
			graphics.DrawLine(_pen,
				cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1),
				cellBounds.X, cellBounds.Y);
		}
		if(cell.HasElement(GraphElement.RightTop))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.RightTop)];
			graphics.DrawLine(_pen,
				cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1),
				cellBounds.X + cellBounds.Width, cellBounds.Y);
		}
		if(cell.HasElement(GraphElement.RightBottom))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.RightBottom)];
			graphics.DrawLine(_pen,
				cellBounds.X + cellBounds.Width, cellBounds.Y + cellBounds.Height,
				cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1));
		}
		if(cell.HasElement(GraphElement.LeftBottom))
		{
			if(useColors) _pen.Color = palette[cell.ColorOf(GraphElementId.LeftBottom)];
			graphics.DrawLine(_pen,
				cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1),
				cellBounds.X, cellBounds.Y + cellBounds.Height);
		}
	}

	private static void PaintDot(GraphStyleOptions options, Graphics graphics, Dpi dpi, GraphCell cell, RevisionGraphItemType type, Rectangle cellBounds, Color[] palette, int penWidth, bool useColors)
	{
		Assert.IsNotNull(options);
		Assert.IsNotNull(graphics);

		if(!cell.HasElement(GraphElement.Dot)) return;

		var cdw = dpi.X * options.NodeRadius * 2 / 96.0f;
		var cdh = dpi.Y * options.NodeRadius * 2 / 96.0f;
		var cx = cellBounds.X + (cellBounds.Width  >> 1) - cdw / 2;
		var cy = cellBounds.Y + (cellBounds.Height >> 1) - cdh / 2;
		cx -= 0.5f * ((penWidth + 1) & 1);
		var lightBackground = GitterApplication.Style.Type == GitterStyleType.LightBackground;
		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			switch(type)
			{
				case RevisionGraphItemType.Generic:
					{
						if(useColors && options.ColorNodes)
						{
							var color = cell.ColorOf(GraphElementId.Dot);
							using var brush = SolidBrushCache.Get(palette[color]);
							graphics.FillEllipse(brush, cx, cy, cdw, cdh);
						}
						else
						{
							var brush = lightBackground ? GraphColors.DotBrushForLightBackground : GraphColors.DotBrushForDarkBackground;
							graphics.FillEllipse(brush, cx, cy, cdw, cdh);
						}
					}
					break;
				case RevisionGraphItemType.Current:
					{
						var brush = Brushes.LightGreen;
						graphics.FillEllipse(brush, cx, cy, cdw, cdh);
						var pen = lightBackground ? GraphColors.CirclePenForLightBackground : GraphColors.CirclePenForDarkBackground;
						graphics.DrawEllipse(pen, cx + .5f, cy + .5f, cdw - 1, cdh - 1);
					}
					break;
				case RevisionGraphItemType.Uncommitted:
					{
						var brush = Brushes.Cyan;
						graphics.FillEllipse(brush, cx, cy, cdw, cdh);
						var pen = lightBackground ? GraphColors.CirclePenForLightBackground : GraphColors.CirclePenForDarkBackground;
						graphics.DrawEllipse(pen, cx + .5f, cy + .5f, cdw - 1, cdh - 1);
					}
					break;
				case RevisionGraphItemType.Unstaged:
					{
						var brush = Brushes.Red;
						graphics.FillEllipse(brush, cx, cy, cdw, cdh);
						var pen = lightBackground ? GraphColors.CirclePenForLightBackground : GraphColors.CirclePenForDarkBackground;
						graphics.DrawEllipse(pen, cx + .5f, cy + .5f, cdw - 1, cdh - 1);
					}
					break;
			}
		}
	}

	public void DrawBackground(Graphics graphics, Dpi dpi, GraphCell[] graphLine, Rectangle bounds, Rectangle clip, int cellWidth, bool useColors)
	{
		Verify.Argument.IsNotNull(graphics);

		if(graphLine is not { Length: > 0 }) return;

		var palette = GitterApplication.Style.Type == GitterStyleType.LightBackground
			? GraphColors.ColorsForLightBackground
			: GraphColors.ColorsForDarkBackground;

		var dot = FindElementIndex(graphLine, GraphElement.Dot);
		if(dot < 0) return;

		const int PaddingBase = 2;
		const int AccentWidth = 2;
		var padding = PaddingBase * dpi.Y / 96;
		var accent  = AccentWidth * dpi.X / 96;

		var color = useColors
			? palette[graphLine[dot].ColorOf(GraphElementId.Dot)]
			: palette[0];

		var x0 = bounds.X + cellWidth * dot + (cellWidth >> 1);
		var x1 = bounds.Right;
		var bcolor = Color.FromArgb(30, color);
		using var backgroundBrush = SolidBrushCache.Get(bcolor);
		var back = new Rectangle(x0, bounds.Y + padding, x1 - x0 - accent, bounds.Height - padding * 2);
		back.Intersect(clip);
		if(back.Width > 0 && back.Height > 0)
		{
			graphics.FillRectangle(backgroundBrush, back);
		}

		var accentBounds = new Rectangle(bounds.Right - accent, bounds.Y + padding, accent, bounds.Height - padding * 2);
		accentBounds.Intersect(clip);
		if(accentBounds.Width > 0 && accentBounds.Height > 0)
		{
			graphics.GdiFill(color, accentBounds);
		}
	}

	public void DrawGraph(Graphics graphics, Dpi dpi, GraphCell[] graphLine, Rectangle bounds, Rectangle clip, int cellWidth, RevisionGraphItemType type, bool useColors)
	{
		Verify.Argument.IsNotNull(graphics);

		if(graphLine is not { Length: > 0 }) return;

		var options = OptionsProvider.Value;

		var penWidth = GetGraphLineWidth(dpi, options.BaseLineWidth);
		_pen.Width = penWidth;

		var palette = GitterApplication.Style.Type == GitterStyleType.LightBackground
			? GraphColors.ColorsForLightBackground
			: GraphColors.ColorsForDarkBackground;
		var start = clip.X > bounds.X
			? (clip.X - bounds.X) / cellWidth
			: 0;

		for(int i = start, x = bounds.X + start * cellWidth, maxX = Math.Min(clip.Right, bounds.Right); (i < graphLine.Length) && (x < maxX); ++i, x += cellWidth)
		{
			var cell = graphLine[i];
			if(cell.IsEmpty) continue;

			var cellBounds = new Rectangle(x, bounds.Y, cellWidth, bounds.Height);
			if(Rectangle.Intersect(clip, cellBounds) is not { Width: > 0, Height: > 0 } cellClip)
			{
				continue;
			}
			graphics.SetClip(cellClip);

			if(cell.Elements != GraphElement.Dot)
			{
				if(!useColors) _pen.Color = palette[0];

				switch(cell.Elements)
				{
					case GraphElement.Vertical:
						PaintVerticalLine  (graphics, cell, cellBounds, palette, useColors);
						continue;
					case GraphElement.Horizontal:
						PaintHorizontalLine(graphics, cell, cellBounds, palette, useColors);
						continue;
				}

				PaintVerticalLines(options, graphics, dpi, cell, cellBounds, palette, useColors);
				var left  = cell.HasElement(GraphElement.HorizontalLeft);
				var right = cell.HasElement(GraphElement.HorizontalRight);
				if(left && i > 0)
				{
					var prev = graphLine[i - 1];
					if(!prev.HasElement(GraphElement.HorizontalRight) || prev.ColorOf(GraphElementId.HorizontalRight) != cell.ColorOf(GraphElementId.HorizontalLeft))
					{
						PaintLeftLine(options, graphics, dpi, cell, cellBounds, palette, useColors);
						left = false;
					}
				}
				if(right && i < graphLine.Length - 1)
				{
					var next = graphLine[i + 1];
					if(!next.HasElement(GraphElement.HorizontalLeft) || next.ColorOf(GraphElementId.HorizontalLeft) != cell.ColorOf(GraphElementId.HorizontalRight))
					{
						PaintRightLine(options, graphics, dpi, cell, cellBounds, palette, useColors);
						right = false;
					}
				}
				PaintBottomCornerLines(options, graphics, cell, cellBounds, palette, useColors, penWidth);
				PaintTopCornerLines   (options, graphics, cell, cellBounds, palette, useColors, penWidth);
				PaintDiagonalLines    (graphics, cell, cellBounds, palette, useColors);

				if(left && right)
				{
					PaintHorizontalLines(options, graphics, dpi, cell, cellBounds, palette, useColors);
				}
				else
				{
					if(left)  PaintLeftLine (options, graphics, dpi, cell, cellBounds, palette, useColors);
					if(right) PaintRightLine(options, graphics, dpi, cell, cellBounds, palette, useColors);
				}
			}
			PaintDot(options, graphics, dpi, cell, type, cellBounds, palette, penWidth, useColors);
		}

		graphics.SetClip(clip);
	}

	private static int FindElementIndex(GraphCell[] graphLine, GraphElement element)
	{
		for(int i = 0; i < graphLine.Length; ++i)
		{
			if(graphLine[i].HasElement(element))
			{
				return i;
			}
		}
		return -1;
	}

	private void DrawReferenceConnectorCore(Graphics graphics, Dpi dpi, int revisionPos, int graphX, int cellWidth, int refX, int y, int h)
	{
		var lightBackground = GitterApplication.Style.Type == GitterStyleType.LightBackground;
		float cx = graphX + revisionPos * cellWidth + cellWidth / 2;
		float cy = y + h / 2;
		var conv = DpiConverter.FromDefaultTo(dpi);
		var cw = conv.ConvertX(5.0f);
		var ch = conv.ConvertY(5.0f);
		var penWidth = GetGraphLineWidth(dpi, 1);
		cx -= 0.5f * ((GetGraphLineWidth(dpi, OptionsProvider.Value.BaseLineWidth) + 1) & 1);
		_pen.Width = penWidth;
		_pen.Color = lightBackground ? Color.Black : Color.White;
		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			graphics.DrawEllipse(_pen, cx - cw, cy - ch, cw * 2, ch * 2);
		}
		if(refX >= 0)
		{
			_pen.Width = 1;
			_pen.Color = lightBackground ? Color.Black : Color.White;
			graphics.DrawLine(_pen, cx + (int)(cw + 1), cy, refX, cy);
		}
	}

	public void DrawReferenceConnector(Graphics graphics, Dpi dpi, GraphCell[] graphLine, int graphX, int cellWidth, int refX, int y, int h)
	{
		var revisionPos = FindElementIndex(graphLine, GraphElement.Dot);
		if(revisionPos < 0) return;
		DrawReferenceConnectorCore(graphics, dpi, revisionPos, graphX, cellWidth, refX, y, h);
	}

	public void DrawReferencePresenceIndicator(Graphics graphics, Dpi dpi, GraphCell[] graphLine, int graphX, int cellWidth, int y, int h)
	{
		var revisionPos = FindElementIndex(graphLine, GraphElement.Dot);
		if(revisionPos < 0) return;
		DrawReferenceConnectorCore(graphics, dpi, revisionPos, graphX, cellWidth, -1, y, h);
	}

	private static void SetTagPoints(int x, int y, int w, int h)
	{
		int d = (h + 1) / 2;
		int vc = y + d;
		TagPoints[0] = new Point(x, vc + 1);
		TagPoints[1] = new Point(x, vc - 1);
		TagPoints[2] = new Point(x + d - 1, y);
		TagPoints[3] = new Point(x + w, y);
		TagPoints[4] = new Point(x + w, y + h);
		TagPoints[5] = new Point(x + d - 1, y + h);
	}

	private static Rectangle DrawInlineTag(Graphics graphics, Dpi dpi, Font font, Brush backBrush, StringFormat format, string text, int x, int y, int right, int h)
	{
		Assert.IsNotNull(graphics);
		Assert.IsNotNull(font);

		var conv = DpiConverter.FromDefaultTo(dpi);

		int spacing = conv.ConvertX(4);

		var size = GitterApplication.TextRenderer.MeasureText(
			graphics, text, font, right - x, format);
		int w = size.Width + 1 + conv.ConvertX(6);
		int texth = size.Height;
		right -= spacing;
		if(w > right - x)
		{
			w = right - x;
		}
		if(w <= conv.ConvertX(5)) return Rectangle.Empty;

		const int Padding = 2;
		var padding = conv.ConvertY(Padding);
		var tagY = y + padding;
		var tagH = h - (2 * padding + 1);

		SetTagPoints(x, tagY, w + conv.ConvertX(4), tagH);
		graphics.FillPolygon(backBrush, TagPoints);
		var lightBackground = GitterApplication.Style.Type == GitterStyleType.LightBackground;
		var pen = lightBackground ? GraphColors.TagBorderPenForLightBackground : GraphColors.TagBorderPenForDarkBackground;
		graphics.DrawPolygon(pen, TagPoints);
		if(w >= conv.ConvertX(7))
		{
			int d = (h - texth) / 2;
			var rc = new Rectangle(x + conv.ConvertX(4 + 3), y + d, w - conv.ConvertX(6), h - d);
			GitterApplication.TextRenderer.DrawText(
				graphics, text, font, SystemBrushes.WindowText, rc, format);
		}
		return new(x, tagY, w + spacing, tagH + 1);
	}

	public Rectangle DrawTag(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, Tag tag)
	{
		Verify.Argument.IsNotNull(graphics);
		Verify.Argument.IsNotNull(font);
		Verify.Argument.IsNotNull(format);
		Verify.Argument.IsNotNull(tag);

		var bounds = DrawInlineTag(graphics, dpi, font, hovered?TagBrushHovered:TagBrush, format, tag.Name, x, y, right, h);
		if(tag.TagType == TagType.Annotated)
		{
			var lightBackground = GitterApplication.Style.Type == GitterStyleType.LightBackground;
			var pen = lightBackground ? GraphColors.TagBorderPenForLightBackground : Pens.Black;
			const int d = 4;
			var conv = DpiConverter.FromDefaultTo(dpi);
			graphics.DrawLine(pen, x + bounds.Width - conv.ConvertX(d), bounds.Y, x + bounds.Width, bounds.Y + conv.ConvertY(d));
		}
		return bounds;
	}

	public Rectangle DrawBranch(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, BranchBase branch)
	{
		Verify.Argument.IsNotNull(graphics);
		Verify.Argument.IsNotNull(font);
		Verify.Argument.IsNotNull(format);
		Verify.Argument.IsNotNull(branch);

		if(branch.IsRemote)
		{
			var brush = hovered ? RemoteBranchBrushHovered : RemoteBranchBrush;
			return DrawInlineTag(graphics, dpi, font, brush, format, branch.Name, x, y, right, h);
		}
		else
		{
			var brush = hovered ? LocalBranchBrushHovered : LocalBranchBrush;
			var size = DrawInlineTag(graphics, dpi, font, brush, format, branch.Name, x, y, right, h);
			if(branch.IsCurrent)
			{
				const int ArrowSize = 4;

				var conv = DpiConverter.FromDefaultTo(dpi);
				var dy = conv.ConvertY(ArrowSize);
				var y0 =  y + (h >> 1);
				var y1 = y0 - dy - .5f;
				var y2 = y0 + dy + .5f;
				var x0 =  x + conv.ConvertX(1) + .5f;
				var x1 = x0 + conv.ConvertX(ArrowSize);
				CurrentIndicator[0].X = x0;
				CurrentIndicator[0].Y = y0;
				CurrentIndicator[1].X = x1;
				CurrentIndicator[1].Y = y1;
				CurrentIndicator[2].X = x1;
				CurrentIndicator[2].Y = y2;
				graphics.FillPolygon(SystemBrushes.InfoText, CurrentIndicator);
			}
			return size;
		}
	}

	public Rectangle DrawStash(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, StashedState stash)
	{
		Verify.Argument.IsNotNull(graphics);
		Verify.Argument.IsNotNull(font);
		Verify.Argument.IsNotNull(format);
		Verify.Argument.IsNotNull(stash);

		return DrawInlineTag(graphics, dpi, font, hovered?StashBrushHovered:StashBrush, format, GitConstants.StashName, x, y, right, h);
	}

	public bool HitTestReference(Rectangle bounds, int x, int y)
	{
		if(!bounds.Contains(x, y)) return false;
		x -= bounds.X;
		y -= bounds.Y;
		var hh = (bounds.Height - 3) / 2;
		if(y < hh)
		{
			return x >= Math.Abs(y - hh);
		}
		else if(y > hh + 3)
		{
			return x >= Math.Abs((y - 3) - hh);
		}
		else
		{
			return true;
		}
	}

	private int MeasureInlineTag(Graphics graphics, Dpi dpi, Font font, StringFormat format, string text)
	{
		var size = GitterApplication.TextRenderer.MeasureText(
			graphics, text, font, int.MaxValue, format);
		var conv = DpiConverter.FromDefaultTo(dpi);
		return size.Width + 1 + conv.ConvertX(6 + 5);
	}

	public int MeasureTag(Graphics graphics, Dpi dpi, Font font, StringFormat format, Tag tag)
	{
		return MeasureInlineTag(graphics, dpi, font, format, tag.Name);
	}

	public int MeasureBranch(Graphics graphics, Dpi dpi, Font font, StringFormat format, Branch branch)
	{
		return MeasureInlineTag(graphics, dpi, font, format, branch.Name);
	}

	public int MeasureStash(Graphics graphics, Dpi dpi, Font font, StringFormat format, StashedState stash)
	{
		return MeasureInlineTag(graphics, dpi, font, format, GitConstants.StashName);
	}
}
