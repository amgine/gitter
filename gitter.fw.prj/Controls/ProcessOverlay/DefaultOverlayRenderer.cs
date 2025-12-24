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

	private static void DrawIndeterminateProgress(Graphics graphics, Dpi dpi, int x, int y, int w, int h)
	{
		const int n = 12;

		int cx = x + w / 2;
		int cy = y + h / 2;

		int r = (w < h ? w : h) / 2;

#if NETCOREAPP
		long current = (Environment.TickCount64 / 100) % n;
#else
		long current = (Environment.TickCount / 100) % n;
#endif

		using var pen = new Pen(Color.Transparent, dpi.X * 2.0f / 96);
		for(int i = 0; i < n; ++i)
		{
			var a = i * (Math.PI * 2) / n;
			var cos = Math.Cos(a);
			var sin = Math.Sin(a);
			var x1 = (float)(cx + cos * r / 3.0);
			var y1 = (float)(cy + sin * r / 3.0);
			var x2 = (float)(cx + cos * r);
			var y2 = (float)(cy + sin * r);

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
			pen.Color = color;
			graphics.DrawLine(pen, x1, y1, x2, y2);
		}
	}

	public override void Paint(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds)
	{
		const int spacing = 10;

		var dpi  = processOverlay.HostControl is not null ? Dpi.FromControl(processOverlay.HostControl) : Dpi.Default;
		var conv = DpiConverter.FromDefaultTo(dpi);
		var size = conv.Convert(new Size(14, 14));

		var font = processOverlay.Font;

		var tw = GitterApplication.TextRenderer.MeasureText(
			graphics, processOverlay.Title, font, bounds.Width, TitleStringFormat).Width;

		var s = conv.ConvertX(5);

		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			using var path = GraphicsUtility.GetRoundedRectangle(bounds, processOverlay.Rounding);
			using(var brush = new SolidBrush(BackgroundColor))
			{
				graphics.FillPath(brush, path);
			}
			using(var pen = new Pen(BorderColor, conv.ConvertX(2.0f)))
			{
				graphics.DrawPath(pen, path);
			}

			DrawIndeterminateProgress(graphics, dpi,
				bounds.X + (bounds.Width - tw) / 2 - size.Width - s,
				bounds.Y + (bounds.Height - size.Height) / 2,
				size.Width, size.Height);
		}

		var titleRect = new Rectangle(
			bounds.X + (bounds.Width - tw) / 2,
			bounds.Y,
			bounds.Width - conv.ConvertX(spacing) * 2 - s - size.Width,
			bounds.Height);

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
		var conv = processOverlay.HostControl is not null
			? DpiConverter.FromDefaultTo(processOverlay.HostControl)
			: DpiConverter.Identity;
		//var size = conv.Convert(new Size(14, 14));

		using(var path = GraphicsUtility.GetRoundedRectangle(bounds, processOverlay.Rounding))
		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			using(var brush = new SolidBrush(BackgroundColor))
			{
				graphics.FillPath(brush, path);
			}
			using(var pen = new Pen(BorderColor, conv.ConvertX(2.0f)))
			{
				graphics.DrawPath(pen, path);
			}
		}
		if(!string.IsNullOrWhiteSpace(status))
		{
			GitterApplication.TextRenderer.DrawText(
				graphics, status, processOverlay.Font, FontBrush, bounds, StringFormat);
		}
	}
}
