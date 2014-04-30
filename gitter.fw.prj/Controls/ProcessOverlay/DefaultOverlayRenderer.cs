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
	using System.Drawing;
	using System.Drawing.Drawing2D;

	sealed class DefaultOverlayRenderer : ProcessOverlayRenderer
	{
		private static readonly Color BackgroundColor = Color.FromArgb(225, 240, 240, 255);
		private static readonly Color BorderColor = Color.FromArgb(75, 75, 100);

		private static readonly Brush FontBrush = new SolidBrush(BorderColor);

		private static Color ColorLERP(Color c1, Color c2, double position)
		{
			byte r1 = c1.R;
			byte r2 = c2.R;
			byte r = (byte)(r1 + (r2 - r1) * position);

			byte g1 = c1.G;
			byte g2 = c2.G;
			byte g = (byte)(g1 + (g2 - g1) * position);

			byte b1 = c1.B;
			byte b2 = c2.B;
			byte b = (byte)(b1 + (b2 - b1) * position);

			return Color.FromArgb(r, g, b);
		}

		private static void DrawIndeterminateProgress(Graphics graphics, int x, int y, int w, int h)
		{
			const int n = 12;

			int cx = x + w / 2;
			int cy = y + h / 2;

			int r = (w < h ? w : h) / 2;

			long current = (DateTime.Now.Ticks / 1000000) % n;

			for(int i = 0; i < n; ++i)
			{
				var a = i * (Math.PI * 2) / n;
				var cos = Math.Cos(a);
				var sin = Math.Sin(a);
				float x1 = (float)(cx + cos * r / 3.0);
				float y1 = (float)(cy + sin * r / 3.0);
				float x2 = (float)(cx + cos * r);
				float y2 = (float)(cy + sin * r);

				Color color;
				if(i == current)
				{
					color = BorderColor;
				}
				else
				{
					if((current + 1) % n == i)
					{
						color = BackgroundColor;
					}
					else
					{
						var d = i - current;
						if(d < 0) d += n;
						d = n - d;
						var k = (double)d / (double)n;
						color = ColorLERP(BorderColor, BackgroundColor, k);
					}
				}

				using(var pen = new Pen(color, 2.0f))
				{
					graphics.DrawLine(pen, x1, y1, x2, y2);
				}
			}
		}

		public override void Paint(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds)
		{
			var font = processOverlay.Font;
			var oldMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			const int spacing = 10;
			using(var path = GraphicsUtility.GetRoundedRectangle(bounds, processOverlay.Rounding))
			{
				using(var brush = new SolidBrush(BackgroundColor))
				{
					graphics.FillPath(brush, path);
				}
				using(var pen = new Pen(BorderColor, 2.0f))
				{
					graphics.DrawPath(pen, path);
				}
			}
			var tw = GitterApplication.TextRenderer.MeasureText(
				graphics, processOverlay.Title, font, bounds.Width, TitleStringFormat).Width;
			DrawIndeterminateProgress(graphics, bounds.X + (bounds.Width - tw) / 2 - 14 - 5, bounds.Y + (bounds.Height - 14) / 2, 14, 14);
			var titleRect = new Rectangle(bounds.X + (bounds.Width - tw) / 2, bounds.Y, bounds.Width - spacing * 2 - 5 - 14, bounds.Height);
			graphics.SmoothingMode = oldMode;
			if(!string.IsNullOrWhiteSpace(processOverlay.Title))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, processOverlay.Title, font, FontBrush, titleRect, TitleStringFormat);
			}
			if(!string.IsNullOrWhiteSpace(processOverlay.Message))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, processOverlay.Message, font, FontBrush, bounds, StringFormat);
			}
		}

		public override void PaintMessage(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds, string status)
		{
			using(var path = GraphicsUtility.GetRoundedRectangle(bounds, processOverlay.Rounding))
			{
				var oldMode = graphics.SmoothingMode;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				using(var brush = new SolidBrush(BackgroundColor))
				{
					graphics.FillPath(brush, path);
				}
				using(var pen = new Pen(BorderColor, 2.0f))
				{
					graphics.DrawPath(pen, path);
				}
				graphics.SmoothingMode = oldMode;
			}
			if(!string.IsNullOrWhiteSpace(status))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, status, processOverlay.Font, FontBrush, bounds, StringFormat);
			}
		}
	}
}
