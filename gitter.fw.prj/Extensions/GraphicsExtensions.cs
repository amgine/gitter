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

namespace gitter
{
	using System.Drawing;
	using System.Drawing.Drawing2D;

	using gitter.Framework;

	/// <summary>Extension methods for <see cref="System.Drawing.Graphics"/>.</summary>
	public static class GraphicsExtensions
	{
		public ref struct SmoothingModeSwitch
		{
			private readonly Graphics _graphics;
			private readonly SmoothingMode _smoothingMode;

			public SmoothingModeSwitch(Graphics graphics, SmoothingMode smoothingMode)
			{
				_graphics      = graphics;
				_smoothingMode = graphics.SmoothingMode;

				graphics.SmoothingMode = smoothingMode;
			}

			public void Dispose()
			{
				_graphics.SmoothingMode = _smoothingMode;
			}
		}

		public static SmoothingModeSwitch SwitchSmoothingMode(this Graphics graphics, SmoothingMode smoothingMode)
			=> new SmoothingModeSwitch(graphics, smoothingMode);

		public static void GdiFill(this Graphics graphics, Color color, Rectangle bounds)
		{
			Verify.Argument.IsNotNull(graphics, nameof(graphics));

			static Native.StockObject GetStockBrush(Color color)
			{
				if(color == Color.White) return Native.StockObject.WHITE_BRUSH;
				if(color == Color.Black) return Native.StockObject.BLACK_BRUSH;
				return Native.StockObject.NULL_BRUSH;
			}

			var hdc = graphics.GetHdc();
			try
			{
				var stockBrush = GetStockBrush(color);
				if(stockBrush != Native.StockObject.NULL_BRUSH)
				{
					Native.Gdi32.SelectObject(hdc, Native.Gdi32.GetStockObject(stockBrush));
					Native.Gdi32.SelectObject(hdc, Native.Gdi32.GetStockObject(Native.StockObject.NULL_PEN));
					Native.Gdi32.Rectangle(hdc, bounds.X, bounds.Y, bounds.Right + 1, bounds.Bottom + 1);
				}
				else
				{
					var brush = Native.Gdi32.CreateSolidBrush(Native.Macro.RGB(color.R, color.G, color.B));
					try
					{
						Native.Gdi32.SelectObject(hdc, brush);
						Native.Gdi32.SelectObject(hdc, Native.Gdi32.GetStockObject(Native.StockObject.NULL_PEN));
						Native.Gdi32.Rectangle(hdc, bounds.X, bounds.Y, bounds.Right + 1, bounds.Bottom + 1);
					}
					finally
					{
						Native.Gdi32.DeleteObject(brush);
					}
				}
			}
			finally
			{
				graphics.ReleaseHdc(hdc);
			}
		}

		public static void DrawRoundedRectangle(this Graphics g, Pen pen, RectangleF rect, float cornerRadius)
		{
			using var gp = GraphicsUtility.GetRoundedRectangle(rect, cornerRadius);
			g.DrawPath(pen, gp);
		}

		public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float cornerRadius)
		{
			using var gp = GraphicsUtility.GetRoundedRectangle(rect, cornerRadius);
			g.FillPath(brush, gp);
		}

		public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float topLeftCorner, float topRightCorner, float bottomLeftCorner, float bottomRightCorner)
		{
			using var gp = GraphicsUtility.GetRoundedRectangle(rect, topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner);
			g.FillPath(brush, gp);
		}

		public static void FillRoundedRectangle(this Graphics g, Brush fillBrush, Pen borderPen, RectangleF rect, float cornerRadius)
		{
			using var gp = GraphicsUtility.GetRoundedRectangle(rect, cornerRadius);
			g.FillPath(fillBrush, gp);
			g.DrawPath(borderPen, gp);
		}
	}
}
