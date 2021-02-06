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

namespace gitter.Framework.Services
{
	using System;
	using System.Drawing;
	using System.Drawing.Text;
	using System.Collections.Generic;

	public sealed class GdiPlusTextRenderer : ITextRenderer
	{
		private static readonly Bitmap _dummy = new Bitmap(1, 1);
		private static readonly Graphics _g = Graphics.FromImage(_dummy);

		private static readonly Dictionary<Font, float> _fontHeight = new Dictionary<Font, float>();

		private static Size TruncateSize(SizeF size)
		{
			int w = (int)(size.Width + .5f);
			int h = (int)(size.Height + .5f);
			return new Size(w, h);
		}

		private static readonly StringFormat DefaultStringFormatLeftAlign =
			new StringFormat(StringFormat.GenericTypographic)
			{
				FormatFlags =
					StringFormatFlags.LineLimit |
					StringFormatFlags.DisplayFormatControl |
					StringFormatFlags.MeasureTrailingSpaces |
					StringFormatFlags.FitBlackBox |
					StringFormatFlags.NoWrap,
				HotkeyPrefix = HotkeyPrefix.None,
				LineAlignment = StringAlignment.Near,
				Trimming = StringTrimming.None,
			};

		private static readonly StringFormat DefaultStringFormatRightAlign =
			new StringFormat(DefaultStringFormatLeftAlign)
			{
				Alignment = StringAlignment.Far,
			};

		private static readonly StringFormat DefaultStringFormatCenterAlign =
			new StringFormat(DefaultStringFormatLeftAlign)
			{
				Alignment = StringAlignment.Center,
			};

		public StringFormat LeftAlign => DefaultStringFormatLeftAlign;

		public StringFormat RightAlign => DefaultStringFormatRightAlign;

		public StringFormat CenterAlign => DefaultStringFormatCenterAlign;

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
			graphics.DrawString(text, font, brush, layoutRectangle, DefaultStringFormatLeftAlign);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point)
		{
			graphics.DrawString(text, font, brush, point, DefaultStringFormatLeftAlign);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y)
		{
			graphics.DrawString(text, font, brush, x, y, DefaultStringFormatLeftAlign);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Rectangle layoutRectangle, StringFormat format)
		{
			using var brush = new SolidBrush(color);
			graphics.DrawString(text, font, brush, layoutRectangle, format);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Point point, StringFormat format)
		{
			using var brush = new SolidBrush(color);
			graphics.DrawString(text, font, brush, point, format);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, int x, int y, StringFormat format)
		{
			using var brush = new SolidBrush(color);
			graphics.DrawString(text, font, brush, x, y, format);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Rectangle layoutRectangle)
		{
			using var brush = new SolidBrush(color);
			graphics.DrawString(text, font, brush, layoutRectangle, DefaultStringFormatLeftAlign);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Point point)
		{
			using var brush = new SolidBrush(color);
			graphics.DrawString(text, font, brush, point, DefaultStringFormatLeftAlign);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, int x, int y)
		{
			using var brush = new SolidBrush(color);
			graphics.DrawString(text, font, brush, x, y, DefaultStringFormatLeftAlign);
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
			return TruncateSize(graphics.MeasureString(text, font, layoutArea, DefaultStringFormatLeftAlign));
		}

		public Size MeasureText(Graphics graphics, string text, Font font, int width)
		{
			return TruncateSize(graphics.MeasureString(text, font, width, DefaultStringFormatLeftAlign));
		}

		public float GetFontHeight(Font font) => GetFontHeight(null, font);

		public float GetFontHeight(Graphics graphics, Font font)
		{
			if(graphics == null) graphics = _g;
			if(!_fontHeight.TryGetValue(font, out var height))
			{
				var size = graphics.MeasureString("0", font, 10000, DefaultStringFormatLeftAlign);
				height = size.Height;
				if(font.Name != "Consolas") ++height;
				_fontHeight.Add(font, height);
			}
			return height;
		}
	}
}
