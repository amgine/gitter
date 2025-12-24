#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;

using gitter.Framework;

public sealed class GraphStyle : IGraphStyle, IDisposable
{
	public event EventHandler? Changed;

	private void OnChanged(EventArgs e) => Changed?.Invoke(this, e);

	private const float HoverLightAmount = 0.4f;

	const int CornerExtension = 1;
	const int DotMargin       = 1;

#if !NET9_0_OR_GREATER
	[ThreadStatic]
	private static Point []? _polyline;

	[ThreadStatic]
	private static Point []? _tagPoints;

	[ThreadStatic]
	private static PointF[]? _currentIndicator;
#endif

	[ThreadStatic]
	private static Pen? _pen;

	#if NETCOREAPP
	[MemberNotNull(nameof(_pen))]
	#endif
	private static Pen GetCachedPen(Color color, float width = 1)
	{
		if(_pen is null)
		{
			_pen = new Pen(color, width);
		}
		else
		{
			_pen.Color = color;
			_pen.Width = width;
		}
		return _pen;
	}

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

	private void OnOptionsProviderValueChanged(object? sender, EventArgs e)
		=> OnChanged(EventArgs.Empty);

	private IValueProvider<GraphStyleOptions> OptionsProvider { get; }

	private static int GetGraphLineWidth(Dpi dpi, int baseWidth) => Math.Max(1, baseWidth + (dpi.X - 96) / (96 / 2));
	//private static int GetGraphLineWidth(Dpi dpi, int baseWidth) => Math.Max(1, baseWidth * (dpi.X + 95) / 96);

	static void SetupLeftTopConer(
#if NETCOREAPP
		Span<Point> polyline,
#else
		Point[] polyline,
#endif
		Rectangle cellBounds)
	{
		var hw = cellBounds.Width  >> 1;
		var hh = cellBounds.Height >> 1;
		polyline[0] = new(cellBounds.X - CornerExtension, cellBounds.Y + hh);
		polyline[1] = new(cellBounds.X,                   cellBounds.Y + hh);
		polyline[2] = new(cellBounds.X + hw,              cellBounds.Y);
		polyline[3] = new(cellBounds.X + hw,              cellBounds.Y -  CornerExtension);
	}

	static void SetupRightTopCorner(
#if NETCOREAPP
		Span<Point> polyline,
#else
		Point[] polyline,
#endif
		Rectangle cellBounds)
	{
		var hw = cellBounds.Width  >> 1;
		var hh = cellBounds.Height >> 1;
		polyline[0] = new(cellBounds.X + cellBounds.Width + CornerExtension, cellBounds.Y + hh);
		polyline[1] = new(cellBounds.X + cellBounds.Width - 1,               cellBounds.Y + hh);
		polyline[2] = new(cellBounds.X + hw,                                 cellBounds.Y);
		polyline[3] = new(cellBounds.X + hw,                                 cellBounds.Y -  CornerExtension);
	}

	static void SetupLeftBottomCorner(
#if NETCOREAPP
		Span<Point> polyline,
#else
		Point[] polyline,
#endif
		Rectangle cellBounds)
	{
		var hw = cellBounds.Width  >> 1;
		var hh = cellBounds.Height >> 1;
		polyline[0] = new(cellBounds.X + hw,              cellBounds.Y + cellBounds.Height + CornerExtension);
		polyline[1] = new(cellBounds.X + hw,              cellBounds.Y + cellBounds.Height - 1);
		polyline[2] = new(cellBounds.X,                   cellBounds.Y + hh);
		polyline[3] = new(cellBounds.X - CornerExtension, cellBounds.Y + hh);
	}

	static void SetupRightBottomCorner(
#if NETCOREAPP
		Span<Point> polyline,
#else
		Point[] polyline,
#endif
		Rectangle cellBounds)
	{
		var hw = cellBounds.Width  >> 1;
		var hh = cellBounds.Height >> 1;
		polyline[0] = new(cellBounds.X + cellBounds.Width + CornerExtension, cellBounds.Y + hh);
		polyline[1] = new(cellBounds.X + cellBounds.Width - 1,               cellBounds.Y + hh);
		polyline[2] = new(cellBounds.X + hw,                                 cellBounds.Y + cellBounds.Height - 1);
		polyline[3] = new(cellBounds.X + hw,                                 cellBounds.Y + cellBounds.Height + CornerExtension);
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

	readonly ref struct PaintContext(GraphStyleOptions options, Graphics graphics, Dpi dpi, Rectangle clip, int cellWidth, bool useColors, int penWidth, Color[] palette)
	{
		public GraphStyleOptions Options   { get; } = options;
		public Graphics          Graphics  { get; } = graphics;
		public Dpi               Dpi       { get; } = dpi;
		public Rectangle         Clip      { get; } = clip;
		public int               CellWidth { get; } = cellWidth;
		public bool              UseColors { get; } = useColors;
		public int               PenWidth  { get; } = penWidth;
		public Color[]           Palette   { get; } = palette;

		public Pen GetPenByElementId(in GraphCell cell, int elementId)
			=> UseColors
				? GetCachedPen(Palette[cell.ColorOf(elementId)], PenWidth)
				: GetCachedPen(Palette[0], PenWidth);

		public Pen GetPenByColorIndex(int colorIndex)
			=> UseColors
				? GetCachedPen(Palette[colorIndex], PenWidth)
				: GetCachedPen(Palette[0], PenWidth);

		public void DrawArc(Pen pen, RectangleF bounds, int startAngle, int sweepAngle)
		{
			using(Graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
			{
				Graphics.DrawArc(pen, bounds, startAngle, sweepAngle);
			}
		}

		public void PaintVerticalLine(in GraphCell cell, Rectangle cellBounds)
		{
			var x   = cellBounds.X + (cellBounds.Width >> 1);
			var y0  = cellBounds.Y;
			var y1  = cellBounds.Y + cellBounds.Height;
			var pen = GetPenByElementId(in cell, GraphElementId.VerticalTop);
			Graphics.DrawLine(pen, x, y0, x, y1);
		}

		public void PaintHorizontalLine(in GraphCell cell, Rectangle cellBounds)
		{
			var y   = cellBounds.Y + (cellBounds.Height >> 1);
			var x0  = cellBounds.X;
			var x1  = x0 + cellBounds.Width;
			var pen = GetPenByElementId(cell, GraphElementId.HorizontalLeft);
			Graphics.DrawLine(pen, x0, y, x1, y);
		}

		public void PaintVerticalLines(in GraphCell cell, Rectangle cellBounds)
		{
			if(!cell.HasAnyOfElements(GraphElement.Vertical)) return;

			var @break = Options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
			var x      = cellBounds.X + (cellBounds.Width >> 1);
			var color0 = cell.ColorOf(GraphElementId.VerticalTop);
			var color1 = cell.ColorOf(GraphElementId.VerticalBottom);
			if(!@break && cell.HasElement(GraphElement.Vertical) && (!UseColors || (color0 == color1)))
			{
				var y0  = cellBounds.Y;
				var y1  = cellBounds.Y + cellBounds.Height;
				var pen = GetPenByColorIndex(color0);
				Graphics.DrawLine(pen, x, y0, x, y1);
				return;
			}
			if(cell.HasElement(GraphElement.VerticalTop))
			{
				var dy  = @break ? (Options.NodeRadius * Dpi.Y / 96 + DotMargin) : 0;
				var y0  = cellBounds.Y;
				var y1  = y0 + (cellBounds.Height >> 1) - dy;
				var pen = GetPenByColorIndex(color0);
				Graphics.DrawLine(pen, x, y0, x, y1);
			}
			if(cell.HasElement(GraphElement.VerticalBottom))
			{
				var dy  = @break ? (Options.NodeRadius * Dpi.Y / 96 + DotMargin + 1) : 0;
				var y0  = cellBounds.Y + (cellBounds.Height >> 1) + dy;
				var y1  = cellBounds.Y +  cellBounds.Height;
				var pen = GetPenByColorIndex(color1);
				Graphics.DrawLine(pen, x, y0, x, y1);
			}
		}

		public void PaintLeftLine(in GraphCell cell, Rectangle cellBounds)
		{
			var @break = Options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
			var dx  = @break ? (Options.NodeRadius * Dpi.X / 96 + DotMargin) : 0;
			var y   = cellBounds.Y + (cellBounds.Height >> 1);
			var x0  = cellBounds.X;
			var x1  = x0 + (cellBounds.Width >> 1) - dx;
			var pen = GetPenByElementId(cell, GraphElementId.HorizontalLeft);
			Graphics.DrawLine(pen, x0, y, x1, y);
		}

		public void PaintRightLine(in GraphCell cell, Rectangle cellBounds)
		{
			var @break = Options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
			var y   = cellBounds.Y + (cellBounds.Height >> 1);
			var dx  = @break ? (Options.NodeRadius * Dpi.X / 96 + DotMargin + 1) : 0;
			var x0  = cellBounds.X + (cellBounds.Width >> 1) + dx;
			var x1  = cellBounds.X +  cellBounds.Width;
			var pen = GetPenByElementId(cell, GraphElementId.HorizontalRight);
			Graphics.DrawLine(pen, x0, y, x1, y);
		}

		public void PaintHorizontalLines(in GraphCell cell, Rectangle cellBounds)
		{
			if(!cell.HasAnyOfElements(GraphElement.Horizontal)) return;

			var @break = Options.BreakLinesWithDot && cell.HasElement(GraphElement.Dot);
			var y      = cellBounds.Y + (cellBounds.Height >> 1);
			var color0 = cell.ColorOf(GraphElementId.HorizontalLeft);
			var color1 = cell.ColorOf(GraphElementId.HorizontalRight);
			if(!@break && cell.HasElement(GraphElement.Horizontal) && (!UseColors || (color0 == color1)))
			{
				var x0 = cellBounds.X;
				var x1 = x0 + cellBounds.Width;
				var pen = GetPenByColorIndex(color0);
				Graphics.DrawLine(pen, x0, y, x1, y);
				return;
			}
			if(cell.HasElement(GraphElement.HorizontalLeft))
			{
				var dx = @break ? (Options.NodeRadius * Dpi.X / 96 + DotMargin) : 0;
				var x0 = cellBounds.X;
				var x1 = x0 + (cellBounds.Width >> 1) - dx;
				var pen = GetPenByColorIndex(color0);
				Graphics.DrawLine(pen, x0, y, x1, y);
			}
			if(cell.HasElement(GraphElement.HorizontalRight))
			{
				var dx  = @break ? (Options.NodeRadius * Dpi.X / 96 + DotMargin + 1) : 0;
				var x0  = cellBounds.X + (cellBounds.Width >> 1) + dx;
				var x1  = cellBounds.X +  cellBounds.Width;
				var pen = GetPenByColorIndex(color1);
				Graphics.DrawLine(pen, x0, y, x1, y);
			}
		}

		public void PaintBottomCornerLines(in GraphCell cell, Rectangle cellBounds)
		{
			if(cell.HasElement(GraphElement.LeftBottomCorner))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.LeftBottomCorner);
				if(Options.RoundedCorners)
				{
					RectangleF b = cellBounds;
					var delta = 0.5f + ((PenWidth + 1) & 1) * 0.5f;
					b.X -= b.Width  / 2.0f + delta;
					b.Y += b.Height / 2.0f - delta;
					DrawArc(pen, b, 265, 100);
				}
				else
				{
#if NET9_0_OR_GREATER
					Span<Point> polyline = stackalloc Point[4];
#else
					var polyline = _polyline ??= new Point[4];
#endif
					SetupLeftBottomCorner(polyline, cellBounds);
					Graphics.DrawLines(pen, polyline);
				}
			}
			if(cell.HasElement(GraphElement.RightBottomCorner))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.RightBottomCorner);
				if(Options.RoundedCorners)
				{
					RectangleF b = cellBounds;
					var delta = 0.5f + ((PenWidth + 1) & 1) * 0.5f;
					b.X += b.Width  / 2.0f - delta;
					b.Y += b.Height / 2.0f - delta;
					DrawArc(pen, b, 175, 100);
				}
				else
				{
#if NET9_0_OR_GREATER
					Span<Point> polyline = stackalloc Point[4];
#else
					var polyline = _polyline ??= new Point[4];
#endif
					SetupRightBottomCorner(polyline, cellBounds);
					Graphics.DrawLines(pen, polyline);
				}
			}
		}

		public void PaintTopCornerLines(in GraphCell cell, Rectangle cellBounds)
		{
			if(cell.HasElement(GraphElement.LeftTopCorner))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.LeftTopCorner);
				if(Options.RoundedCorners)
				{
					RectangleF b = cellBounds;
					var delta = 0.5f + ((PenWidth + 1) & 1) * 0.5f;
					b.X -= b.Width  / 2.0f + delta;
					b.Y -= b.Height / 2.0f + delta;
					DrawArc(pen, b, -5, 100);
				}
				else
				{
#if NET9_0_OR_GREATER
					Span<Point> polyline = stackalloc Point[4];
#else
					var polyline = _polyline ??= new Point[4];
#endif
					SetupLeftTopConer(polyline, cellBounds);
					Graphics.DrawLines(pen, polyline);
				}
			}
			if(cell.HasElement(GraphElement.RightTopCorner))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.RightTopCorner);
				if(Options.RoundedCorners)
				{
					RectangleF b = cellBounds;
					var delta = 0.5f + ((PenWidth + 1) & 1) * 0.5f;
					b.X += b.Width  / 2.0f - delta;
					b.Y -= b.Height / 2.0f + delta;
					DrawArc(pen, b, 80, 100);
				}
				else
				{
#if NET9_0_OR_GREATER
					Span<Point> polyline = stackalloc Point[4];
#else
					var polyline = _polyline ??= new Point[4];
#endif
					SetupRightTopCorner(polyline, cellBounds);
					Graphics.DrawLines(pen, polyline);
				}
			}
		}

		public void PaintDiagonalLines(in GraphCell cell, Rectangle cellBounds)
		{
			if(cell.HasElement(GraphElement.LeftTop))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.LeftTop);
				Graphics.DrawLine(pen,
					cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1),
					cellBounds.X, cellBounds.Y);
			}
			if(cell.HasElement(GraphElement.RightTop))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.RightTop);
				Graphics.DrawLine(pen,
					cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1),
					cellBounds.X + cellBounds.Width, cellBounds.Y);
			}
			if(cell.HasElement(GraphElement.RightBottom))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.RightBottom);
				Graphics.DrawLine(pen,
					cellBounds.X + cellBounds.Width, cellBounds.Y + cellBounds.Height,
					cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1));
			}
			if(cell.HasElement(GraphElement.LeftBottom))
			{
				var pen = GetPenByElementId(in cell, GraphElementId.LeftBottom);
				Graphics.DrawLine(pen,
					cellBounds.X + (cellBounds.Width >> 1), cellBounds.Y + (cellBounds.Height >> 1),
					cellBounds.X, cellBounds.Y + cellBounds.Height);
			}
		}

		public void PaintDot(in GraphCell cell, RevisionGraphItemType type, Rectangle cellBounds)
		{
			if(!cell.HasElement(GraphElement.Dot)) return;

			var cdw = Dpi.X * Options.NodeRadius * 2 / 96.0f;
			var cdh = Dpi.Y * Options.NodeRadius * 2 / 96.0f;
			var cx  = cellBounds.X + (cellBounds.Width  >> 1) - cdw / 2;
			var cy  = cellBounds.Y + (cellBounds.Height >> 1) - cdh / 2;
			cx -= 0.5f * ((PenWidth + 1) & 1);
			var lightBackground = GitterApplication.Style.Type == GitterStyleType.LightBackground;
			using(Graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
			{
				switch(type)
				{
					case RevisionGraphItemType.Generic:
						{
							if(UseColors && Options.ColorNodes)
							{
								var color = cell.ColorOf(GraphElementId.Dot);
								using var brush = SolidBrushCache.Get(Palette[color]);
								Graphics.FillEllipse(brush, cx, cy, cdw, cdh);
							}
							else
							{
								var brush = lightBackground ? GraphColors.DotBrushForLightBackground : GraphColors.DotBrushForDarkBackground;
								Graphics.FillEllipse(brush, cx, cy, cdw, cdh);
							}
						}
						break;
					case RevisionGraphItemType.Current:
						{
							var brush = Brushes.LightGreen;
							Graphics.FillEllipse(brush, cx, cy, cdw, cdh);
							var pen = lightBackground ? GraphColors.CirclePenForLightBackground : GraphColors.CirclePenForDarkBackground;
							Graphics.DrawEllipse(pen, cx + .5f, cy + .5f, cdw - 1, cdh - 1);
						}
						break;
					case RevisionGraphItemType.Uncommitted:
						{
							var brush = Brushes.Cyan;
							Graphics.FillEllipse(brush, cx, cy, cdw, cdh);
							var pen = lightBackground ? GraphColors.CirclePenForLightBackground : GraphColors.CirclePenForDarkBackground;
							Graphics.DrawEllipse(pen, cx + .5f, cy + .5f, cdw - 1, cdh - 1);
						}
						break;
					case RevisionGraphItemType.Unstaged:
						{
							var brush = Brushes.Red;
							Graphics.FillEllipse(brush, cx, cy, cdw, cdh);
							var pen = lightBackground ? GraphColors.CirclePenForLightBackground : GraphColors.CirclePenForDarkBackground;
							Graphics.DrawEllipse(pen, cx + .5f, cy + .5f, cdw - 1, cdh - 1);
						}
						break;
				}
			}
		}

	}

	public void DrawGraph(Graphics graphics, Dpi dpi, GraphCell[]? graphLine, Rectangle bounds, Rectangle clip, int cellWidth, RevisionGraphItemType type, bool useColors)
	{
		Verify.Argument.IsNotNull(graphics);

		if(graphLine is not { Length: > 0 }) return;

		var options = OptionsProvider.Value;

		var penWidth = GetGraphLineWidth(dpi, options.BaseLineWidth);

		var palette = GitterApplication.Style.Type == GitterStyleType.LightBackground
			? GraphColors.ColorsForLightBackground
			: GraphColors.ColorsForDarkBackground;
		var start = clip.X > bounds.X
			? (clip.X - bounds.X) / cellWidth
			: 0;

		var context    = new PaintContext(options, graphics, dpi, clip, cellWidth, useColors, penWidth, palette);
		var cellBounds = new Rectangle(0, bounds.Y, cellWidth, bounds.Height);

		for(int i = start, x = bounds.X + start * cellWidth, maxX = Math.Min(clip.Right, bounds.Right); (i < graphLine.Length) && (x < maxX); ++i, x += cellWidth)
		{
			ref GraphCell cell = ref graphLine[i];
			if(cell.IsEmpty) continue;

			cellBounds.X = x;
			if(Rectangle.Intersect(clip, cellBounds) is not { Width: > 0, Height: > 0 } cellClip)
			{
				continue;
			}
			graphics.SetClip(cellClip);

			if(cell.Elements != GraphElement.Dot)
			{
				switch(cell.Elements)
				{
					case GraphElement.Vertical:
						context.PaintVerticalLine  (in cell, cellBounds);
						continue;
					case GraphElement.Horizontal:
						context.PaintHorizontalLine(in cell, cellBounds);
						continue;
				}

				context.PaintVerticalLines(in cell, cellBounds);
				var left  = cell.HasElement(GraphElement.HorizontalLeft);
				var right = cell.HasElement(GraphElement.HorizontalRight);
				if(left && i > 0)
				{
					ref GraphCell prev = ref graphLine[i - 1];
					if(!prev.HasElement(GraphElement.HorizontalRight) || prev.ColorOf(GraphElementId.HorizontalRight) != cell.ColorOf(GraphElementId.HorizontalLeft))
					{
						context.PaintLeftLine(in cell, cellBounds);
						left = false;
					}
				}
				if(right && i < graphLine.Length - 1)
				{
					ref GraphCell next = ref graphLine[i + 1];
					if(!next.HasElement(GraphElement.HorizontalLeft) || next.ColorOf(GraphElementId.HorizontalLeft) != cell.ColorOf(GraphElementId.HorizontalRight))
					{
						context.PaintRightLine(in cell, cellBounds);
						right = false;
					}
				}
				context.PaintBottomCornerLines(in cell, cellBounds);
				context.PaintTopCornerLines   (in cell, cellBounds);
				context.PaintDiagonalLines    (in cell, cellBounds);

				if(left && right)
				{
					context.PaintHorizontalLines(in cell, cellBounds);
				}
				else if(left)  context.PaintLeftLine (in cell, cellBounds);
				else if(right) context.PaintRightLine(in cell, cellBounds);
			}
			context.PaintDot(in cell, type, cellBounds);
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
		if(_pen is null)
		{
			_pen = new(lightBackground ? Color.Black : Color.White, penWidth);
		}
		else
		{
			_pen.Width = penWidth;
			_pen.Color = lightBackground ? Color.Black : Color.White;
		}
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

	private static void SetTagPoints(
#if NETCOREAPP
		Span<Point> points,
#else
		Point[] points,
#endif
		int x, int y, int w, int h)
	{
		int d = (h + 1) / 2;
		int vc = y + d;
		points[0] = new Point(x, vc + 1);
		points[1] = new Point(x, vc - 1);
		points[2] = new Point(x + d - 1, y);
		points[3] = new Point(x + w, y);
		points[4] = new Point(x + w, y + h);
		points[5] = new Point(x + d - 1, y + h);
	}

	private static Rectangle DrawInlineTag(Graphics graphics, Dpi dpi, Font font, Brush backBrush, Color textColor, StringFormat format, string text, int x, int y, int right, int h)
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

		const int Padding = 1;
		var padding = conv.ConvertY(Padding);
		var tagY = y + padding;
		var tagH = h - (2 * padding + 1);

#if NET9_0_OR_GREATER
		Span<Point> tagPoints = stackalloc Point[6];
#else
		var tagPoints = _tagPoints ??= new Point[6];
#endif
		SetTagPoints(tagPoints, x, tagY, w + conv.ConvertX(4), tagH);
		graphics.FillPolygon(backBrush, tagPoints);
		var lightBackground = GitterApplication.Style.Type == GitterStyleType.LightBackground;
		var pen = lightBackground ? GraphColors.TagBorderPenForLightBackground : GraphColors.TagBorderPenForDarkBackground;
		graphics.DrawPolygon(pen, tagPoints);
		if(w >= conv.ConvertX(7))
		{
			int d = (h - texth) / 2;
			var rc = new Rectangle(x + conv.ConvertX(4 + 3), y + d, w - conv.ConvertX(6), h - d);
			GitterApplication.TextRenderer.DrawText(
				graphics, text, font, textColor, rc, format);
		}
		return new(x, tagY, w + spacing, tagH + 1);
	}

	public Rectangle DrawTag(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, Tag tag)
	{
		Verify.Argument.IsNotNull(graphics);
		Verify.Argument.IsNotNull(font);
		Verify.Argument.IsNotNull(format);
		Verify.Argument.IsNotNull(tag);

		Rectangle bounds;
		var colors = GraphColorScheme.Current;
		var color = colors.TagBackColor;
		if(hovered) color = color.Lighter(HoverLightAmount);
		using(var brush = SolidBrushCache.Get(color))
		{
			bounds = DrawInlineTag(graphics, dpi, font, brush, colors.TextColor, format, tag.Name, x, y, right, h);
		}
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

		Rectangle bounds;
		var colors = GraphColorScheme.Current;
		if(branch.IsRemote)
		{
			var color = colors.RemoteBranchBackColor;
			if(hovered) color = color.Lighter(HoverLightAmount);
			using(var brush = SolidBrushCache.Get(color))
			{
				bounds = DrawInlineTag(graphics, dpi, font, brush, colors.TextColor, format, branch.Name, x, y, right, h);
			}
		}
		else
		{
			var color = colors.LocalBranchBackColor;
			if(hovered) color = color.Lighter(HoverLightAmount);
			using(var brush = SolidBrushCache.Get(color))
			{
				bounds = DrawInlineTag(graphics, dpi, font, brush, colors.TextColor, format, branch.Name, x, y, right, h);
			}
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
#if NET9_0_OR_GREATER
				Span<PointF> points = stackalloc PointF[3];
#else
				var points = _currentIndicator ??= new PointF[3];
#endif
				points[0] = new(x0, y0);
				points[1] = new(x1, y1);
				points[2] = new(x1, y2);
				using(var brush = SolidBrushCache.Get(colors.TextColor))
				{
					graphics.FillPolygon(brush, points);
				}
			}
		}
		return bounds;
	}

	public Rectangle DrawStash(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, StashedState stash)
	{
		Verify.Argument.IsNotNull(graphics);
		Verify.Argument.IsNotNull(font);
		Verify.Argument.IsNotNull(format);
		Verify.Argument.IsNotNull(stash);

		var colors = GraphColorScheme.Current;
		var color = colors.StashBackColor;
		if(hovered) color = color.Lighter(HoverLightAmount);
		using(var brush = SolidBrushCache.Get(color))
		{
			return DrawInlineTag(graphics, dpi, font, brush, colors.TextColor, format, GitConstants.StashName, x, y, right, h);
		}
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
