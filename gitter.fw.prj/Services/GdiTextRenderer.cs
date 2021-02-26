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

namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Text;
	using System.Windows.Forms;

	public sealed class GdiTextRenderer : ITextRenderer
	{
		private static readonly Dictionary<Font, float> _fontHeight = new();
		private const TextFormatFlags DefaultFormatFlags =
			TextFormatFlags.NoPadding |
			TextFormatFlags.NoPrefix |
			TextFormatFlags.PreserveGraphicsTranslateTransform |
			TextFormatFlags.PreserveGraphicsClipping |
			TextFormatFlags.Top |
			TextFormatFlags.Left |
			TextFormatFlags.EndEllipsis |
			TextFormatFlags.ExpandTabs;

		private static TextFormatFlags ExtractFormatFlags(StringFormat format)
		{
			var flags = TextFormatFlags.Default;
			if(format.HotkeyPrefix != HotkeyPrefix.Show)
			{
				flags |= TextFormatFlags.NoPrefix;
			}
			flags |= TextFormatFlags.NoPadding;
			flags |= TextFormatFlags.PreserveGraphicsClipping;
			flags |= TextFormatFlags.PreserveGraphicsTranslateTransform;
			flags |= TextFormatFlags.ExpandTabs;
			switch(format.Alignment)
			{
				case StringAlignment.Near:
					flags |= TextFormatFlags.Left;
					break;
				case StringAlignment.Far:
					flags |= TextFormatFlags.Right;
					break;
				case StringAlignment.Center:
					flags |= TextFormatFlags.HorizontalCenter;
					break;
			}
			switch(format.LineAlignment)
			{
				case StringAlignment.Near:
					flags |= TextFormatFlags.Top;
					break;
				case StringAlignment.Center:
					flags |= TextFormatFlags.VerticalCenter;
					break;
				case StringAlignment.Far:
					flags |= TextFormatFlags.Bottom;
					break;
			}
			switch(format.Trimming)
			{
				case StringTrimming.Character:
				case StringTrimming.EllipsisCharacter:
					flags |= TextFormatFlags.EndEllipsis;
					break;
				case StringTrimming.Word:
				case StringTrimming.EllipsisWord:
					flags |= TextFormatFlags.WordEllipsis;
					break;
				case StringTrimming.EllipsisPath:
					flags |= TextFormatFlags.PathEllipsis;
					break;
			}
			switch(format.HotkeyPrefix)
			{
				case HotkeyPrefix.None:
					flags |= TextFormatFlags.NoPrefix;
					break;
			}
			if((format.FormatFlags & StringFormatFlags.NoClip) == StringFormatFlags.NoClip)
			{
				flags |= TextFormatFlags.NoClipping;
			}
			if((format.FormatFlags & StringFormatFlags.LineLimit) == StringFormatFlags.LineLimit)
			{
				flags |= TextFormatFlags.SingleLine;
			}
			if((format.FormatFlags & StringFormatFlags.NoWrap) != StringFormatFlags.NoWrap)
			{
				flags |= TextFormatFlags.WordBreak;
			}
			if((format.FormatFlags & StringFormatFlags.DirectionRightToLeft) == StringFormatFlags.DirectionRightToLeft)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			if((format.FormatFlags & StringFormatFlags.FitBlackBox) == StringFormatFlags.FitBlackBox)
			{
				flags |= TextFormatFlags.GlyphOverhangPadding;
			}
			return flags;
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
				Trimming = StringTrimming.EllipsisCharacter,
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

		private static Color ExtractColor(Brush brush)
		{
			var scb = brush as SolidBrush;
			if(scb != null) return scb.Color;
			return SystemColors.WindowText;
		}

		public StringFormat LeftAlign =>  DefaultStringFormatLeftAlign;

		public StringFormat RightAlign => DefaultStringFormatRightAlign;

		public StringFormat CenterAlign => DefaultStringFormatCenterAlign;

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringFormat format)
		{
			TextRenderer.DrawText(graphics, text, font, layoutRectangle, ExtractColor(brush), ExtractFormatFlags(format));
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point, StringFormat format)
		{
			TextRenderer.DrawText(graphics, text, font, point, ExtractColor(brush), ExtractFormatFlags(format));
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y, StringFormat format)
		{
			TextRenderer.DrawText(graphics, text, font, new Point(x, y), ExtractColor(brush), ExtractFormatFlags(format));
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle)
		{
			TextRenderer.DrawText(graphics, text, font, layoutRectangle, ExtractColor(brush), DefaultFormatFlags);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point)
		{
			TextRenderer.DrawText(graphics, text, font, point, ExtractColor(brush), DefaultFormatFlags);
		}

		public void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y)
		{
			TextRenderer.DrawText(graphics, text, font, new Point(x, y), ExtractColor(brush), DefaultFormatFlags);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Rectangle layoutRectangle, StringFormat format)
		{
			TextRenderer.DrawText(graphics, text, font, layoutRectangle, color, ExtractFormatFlags(format));
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Point point, StringFormat format)
		{
			TextRenderer.DrawText(graphics, text, font, point, color, ExtractFormatFlags(format));
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, int x, int y, StringFormat format)
		{
			TextRenderer.DrawText(graphics, text, font, new Point(x, y), color, ExtractFormatFlags(format));
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Rectangle layoutRectangle)
		{
			TextRenderer.DrawText(graphics, text, font, layoutRectangle, color, DefaultFormatFlags);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, Point point)
		{
			TextRenderer.DrawText(graphics, text, font, point, color, DefaultFormatFlags);
		}

		public void DrawText(Graphics graphics, string text, Font font, Color color, int x, int y)
		{
			TextRenderer.DrawText(graphics, text, font, new Point(x, y), color, DefaultFormatFlags);
		}

		public Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea, StringFormat format)
		{
			return TextRenderer.MeasureText(graphics, text, font, layoutArea, ExtractFormatFlags(format));
		}

		public Size MeasureText(Graphics graphics, string text, Font font, int width, StringFormat format)
		{
			return TextRenderer.MeasureText(graphics, text, font, new Size(width, short.MaxValue), ExtractFormatFlags(format));
		}

		public Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea)
		{
			return TextRenderer.MeasureText(graphics, text, font, layoutArea, DefaultFormatFlags);
		}

		public Size MeasureText(Graphics graphics, string text, Font font, int width)
		{
			return TextRenderer.MeasureText(graphics, text, font, new Size(width, short.MaxValue), DefaultFormatFlags);
		}

		public float GetFontHeight(Font font) => GetFontHeight(null, font);

		public float GetFontHeight(Graphics graphics, Font font)
		{
			if(!_fontHeight.TryGetValue(font, out var height))
			{
				var size = TextRenderer.MeasureText("0", font, new Size(1000, 1000), DefaultFormatFlags);
				height = size.Height;
				if(font.Size >= 9) --height;
				_fontHeight.Add(font, height);
			}
			return height;
		}
	}
}
