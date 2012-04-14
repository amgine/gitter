namespace gitter.Framework.Services
{
	using System;
	using System.Drawing;
	using System.Collections.Generic;

	public sealed class GdiPlusTextRenderer : ITextRenderer
	{
		private static readonly StringFormat DefaultStringFormat = Utility.DefaultStringFormat;
		private static readonly Bitmap _dummy = new Bitmap(1, 1);
		private static readonly Graphics _g = Graphics.FromImage(_dummy);

		private static readonly Dictionary<Font, float> _fontHeight = new Dictionary<Font, float>();

		private static Size TruncateSize(SizeF size)
		{
			int w = (int)(size.Width + .5f);
			int h = (int)(size.Height + .5f);
			return new Size(w, h);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringFormat format)
		{
			graphics.DrawString(text, font, brush, layoutRectangle, format);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point, StringFormat format)
		{
			graphics.DrawString(text, font, brush, point, format);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y, StringFormat format)
		{
			graphics.DrawString(text, font, brush, x, y, format);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle)
		{
			graphics.DrawString(text, font, brush, layoutRectangle, DefaultStringFormat);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point)
		{
			graphics.DrawString(text, font, brush, point, DefaultStringFormat);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y)
		{
			graphics.DrawString(text, font, brush, x, y, DefaultStringFormat);
		}

		public Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea, StringFormat format)
		{
			return TruncateSize(graphics.MeasureString(text, font, layoutArea, format));
		}

		public Size MeasureText(Graphics graphics, string text, Font font, int width, StringFormat format)
		{
			return TruncateSize(graphics.MeasureString(text, font, width, format));
		}

		public Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea)
		{
			return TruncateSize(graphics.MeasureString(text, font, layoutArea, DefaultStringFormat));
		}

		public Size MeasureText(Graphics graphics, string text, Font font, int width)
		{
			return TruncateSize(graphics.MeasureString(text, font, width, DefaultStringFormat));
		}

		public float GetFontHeight(Font font)
		{
			return GetFontHeight(null, font);
		}

		public float GetFontHeight(Graphics graphics, Font font)
		{
			if(graphics == null) graphics = _g;
			float height;
			if(!_fontHeight.TryGetValue(font, out height))
			{
				var size = graphics.MeasureString("0", font, 10000, DefaultStringFormat);
				height = size.Height;
				if(font.Name != "Consolas") ++height;
				_fontHeight.Add(font, height);
			}
			return height;
		}
	}
}
