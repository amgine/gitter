namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Linq;
	using System.Text;

	sealed class MSVS2012OverlayRenderer : ProcessOverlayRenderer
	{
		private const byte BackgroundAlpha = 255;

		public interface IColorTable
		{
			Color Text { get; }
			Color Background { get; }
		}

		private sealed class DarkColorTable : IColorTable
		{
			public Color Text { get { return MSVS2012DarkColors.WINDOW_TEXT; } }

			public Color Background { get { return MSVS2012DarkColors.WORK_AREA; } }
		}

		private static IColorTable _darkColors;

		public static IColorTable DarkColors
		{
			get
			{
				if(_darkColors == null)
				{
					_darkColors = new DarkColorTable();
				}
				return _darkColors;
			}
		}

		private readonly IColorTable _colorTable;

		public MSVS2012OverlayRenderer(IColorTable colorTable)
		{
			Verify.Argument.IsNotNull(colorTable, "colorTable");

			_colorTable = colorTable;
		}

		private IColorTable ColorTable
		{
			get { return _colorTable; }
		}

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

		private void DrawIndeterminateProgress(Graphics graphics, int x, int y, int w, int h)
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
					color = ColorTable.Text;
				}
				else
				{
					if((current + 1) % n == i)
					{
						color = ColorTable.Background;
					}
					else
					{
						var d = i - current;
						if(d < 0) d += n;
						d = n - d;
						var k = (double)d / (double)n;
						color = ColorLERP(ColorTable.Text, ColorTable.Background, k);
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
			const int spacing = 10;
			using(var brush = new SolidBrush(Color.FromArgb(BackgroundAlpha, ColorTable.Background)))
			{
				graphics.FillRectangle(brush, bounds);
			}
			var font = processOverlay.Font;
			var tw = GitterApplication.TextRenderer.MeasureText(
				graphics, processOverlay.Title, font, bounds.Width, TitleStringFormat).Width;
			var oldMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			DrawIndeterminateProgress(graphics, bounds.X + (bounds.Width - tw) / 2 - 14 - 5, bounds.Y + (bounds.Height - 14) / 2, 14, 14);
			graphics.SmoothingMode = oldMode;
			var titleRect = new Rectangle(bounds.X + (bounds.Width - tw) / 2, bounds.Y, bounds.Width - spacing * 2 - 5 - 14, bounds.Height);
			if(!string.IsNullOrWhiteSpace(processOverlay.Title))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, processOverlay.Title, font, ColorTable.Text, titleRect, TitleStringFormat);
			}
			if(!string.IsNullOrWhiteSpace(processOverlay.Message))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, processOverlay.Message, font, ColorTable.Text, bounds, StringFormat);
			}
		}

		public override void PaintMessage(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds, string status)
		{
			using(var brush = new SolidBrush(Color.FromArgb(BackgroundAlpha, ColorTable.Background)))
			{
				graphics.FillRectangle(brush, bounds);
			}
			if(!string.IsNullOrWhiteSpace(status))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, status, processOverlay.Font, ColorTable.Text, bounds, StringFormat);
			}
		}
	}
}
