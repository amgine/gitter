#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Native;

using System;
using System.Drawing;

static class TextRendererEx
{
	static TextPaddingOptions TextPadding { get; set; }

	/// <summary>
	/// Calculates the spacing required for drawing text w/o clipping hanging parts of a glyph.
	/// </summary>
	static float GetOverhangPadding(IntPtr font)
	{
		// Some parts of a glyphs may be clipped depending on the font & font style, GDI+ adds 1/6 of tmHeight
		// to each size of the text bounding box when drawing text to account for that; we do it here as well.

		//WindowsFont tmpfont = font;

		//if(tmpfont == IntPtr.Zero)
		//{
			//tmpfont = Gdi32.GetCurrentObject(hDC, 6);

		//}

		LOGFONT f;
		unsafe
		{
			_ = Gdi32.GetObject(font, sizeof(LOGFONT), &f);
		}



		float overhangPadding = f.lfHeight / 6f;

		return overhangPadding;
	}

	/// <summary>
	/// Get the bounding box internal text padding to be used when drawing text.
	/// </summary>
	static DRAWTEXTPARAMS GetTextMargins(IntPtr font)
	{
		const float ItalicPaddingFactor = 1/2f;

		// DrawText(Ex) adds a small space at the beginning of the text bounding box but not at the end,
		// this is more noticeable when the font has the italic style.  We compensate with this factor.

		int leftMargin  = 0;
		int rightMargin = 0;
		float overhangPadding;

		switch(TextPadding)
		{
			case TextPaddingOptions.GlyphOverhangPadding:
				// [overhang padding][Text][overhang padding][italic padding]
				overhangPadding = GetOverhangPadding(font);
#if NETCOREAPP
				leftMargin  = (int)MathF.Ceiling(overhangPadding);
				rightMargin = (int)MathF.Ceiling(overhangPadding * (1 + ItalicPaddingFactor));
#else
				leftMargin  = (int)Math.Ceiling(overhangPadding);
				rightMargin = (int)Math.Ceiling(overhangPadding * (1 + ItalicPaddingFactor));
#endif
				break;

			case TextPaddingOptions.LeftAndRightPadding:
				// [2 * overhang padding][Text][2 * overhang padding][italic padding]
				overhangPadding = GetOverhangPadding(font);
#if NETCOREAPP
				leftMargin  = (int)MathF.Ceiling(2 * overhangPadding);
				rightMargin = (int)MathF.Ceiling(overhangPadding * (2 + ItalicPaddingFactor));
#else
				leftMargin  = (int)Math.Ceiling(2 * overhangPadding);
				rightMargin = (int)Math.Ceiling(overhangPadding * (2 + ItalicPaddingFactor));
#endif
				break;

			case TextPaddingOptions.NoPadding:
			default:
				break;
		}

		return new DRAWTEXTPARAMS(leftMargin, rightMargin);
	}

	static unsafe Rectangle AdjustForVerticalAlignment(IntPtr hdc, char* ptext, int length, Rectangle bounds, DT flags, DRAWTEXTPARAMS* dtparams)
	{
		// Ok if any Top (Cannot test IntTextFormatFlags.Top because it is 0), single line text or measuring text.
		bool isTop = (flags & DT.BOTTOM) == 0 && (flags & DT.VCENTER) == 0;
		if(isTop || ((flags & DT.SINGLELINE) != 0) || ((flags & DT.CALCRECT) != 0))
		{
			return bounds;
		}

		var rect = new RECT(bounds);

		// Get the text bounds.
		flags |= DT.CALCRECT;
		var textHeight = User32.DrawTextExW(hdc, ptext, length, &rect, flags, dtparams);

		// if the text does not fit inside the bounds then return the bounds that were passed in
		if(textHeight > bounds.Height)
		{
			return bounds;
		}

		if((flags & DT.VCENTER) != 0)
		{
			// Middle
			bounds.Y = bounds.Top + bounds.Height / 2 - textHeight / 2;
		}
		else
		{
			// Bottom
			bounds.Y = bounds.Bottom - textHeight;
		}

		return bounds;
	}

	static unsafe bool Equals(LOGFONT* f1, LOGFONT* f2)
	{
		if(f1->lfHeight         != f2->lfHeight)         return false;
		if(f1->lfWidth          != f2->lfWidth)          return false;
		if(f1->lfEscapement     != f2->lfEscapement)     return false;
		if(f1->lfOrientation    != f2->lfOrientation)    return false;
		if(f1->lfWeight         != f2->lfWeight)         return false;
		if(f1->lfItalic         != f2->lfItalic)         return false;
		if(f1->lfUnderline      != f2->lfUnderline)      return false;
		if(f1->lfStrikeOut      != f2->lfStrikeOut)      return false;
		if(f1->lfCharSet        != f2->lfCharSet)        return false;
		if(f1->lfOutPrecision   != f2->lfOutPrecision)   return false;
		if(f1->lfClipPrecision  != f2->lfClipPrecision)  return false;
		if(f1->lfQuality        != f2->lfQuality)        return false;
		if(f1->lfPitchAndFamily != f2->lfPitchAndFamily) return false;

		for(int i = 0; i < 32; ++i)
		{
			var c1 = f1->lfFaceName[i];
			var c2 = f2->lfFaceName[i];
			if(c1 != c2) return false;
			if(c1 == '\0') break;
		}

		/*
		return this.Name == winFont.Name &&
		this.LogFontHeight == winFont.LogFontHeight && // Equivalent to comparing Size but always at hand.
		this.Style == winFont.Style &&
		this.CharSet == winFont.CharSet &&
		this.Quality == winFont.Quality;
		*/

		return true;
	}

	static IntPtr SelectFont(IntPtr hdc, IntPtr font)
	{
		const int OBJ_FONT = 6;

		var current = Gdi32.GetCurrentObject(hdc, OBJ_FONT);
		if(current == font) return IntPtr.Zero;

		LOGFONT f1;
		LOGFONT f2;
		unsafe
		{
			_ = Gdi32.GetObject(font, sizeof(LOGFONT), &f1);
			_ = Gdi32.GetObject(font, sizeof(LOGFONT), &f2);

			if(Equals(&f1, &f2)) return IntPtr.Zero;
		}

		return Gdi32.SelectObject(hdc, font);
	}

	public static unsafe void DrawText(IntPtr hdc, string text, IntPtr font, Rectangle bounds, Color foreColor, Color backColor, DT flags)
	{
		const int TRANSPARENT = 1;
		const int OPAQUE      = 2;

		if(string.IsNullOrEmpty(text) || foreColor == Color.Transparent)
		{
			return;
		}

		// DrawText requires default text alignment.
		if(Gdi32.GetTextAlign(hdc) != 0)
		{
			_ = Gdi32.SetTextAlign(hdc, 0);
		}

		// color empty means use the one currently selected in the dc.
		if(!foreColor.IsEmpty)
		{
			var textColor = Macro.RGB(foreColor.R, foreColor.G, foreColor.B);
			if(textColor != Gdi32.GetTextColor(hdc))
			{
				_ = Gdi32.SetTextColor(hdc, textColor);
			}
		}

		SelectFont(hdc, font);

		var newBackGndMode = (backColor.IsEmpty || backColor == Color.Transparent)
			? TRANSPARENT
			: OPAQUE;

		if(Gdi32.GetBkMode(hdc) != newBackGndMode)
		{
			_ = Gdi32.SetBkMode(hdc, newBackGndMode);
		}

		if(newBackGndMode != TRANSPARENT)
		{
			var bkColor = Macro.RGB(backColor.R, backColor.G, backColor.B);
			if(bkColor != Gdi32.GetBkColor(hdc))
			{
				_ = Gdi32.SetBkColor(hdc, bkColor);
			}
		}

		var dtparams = GetTextMargins(font);

		fixed(char* ptext = text)
		{
			bounds = AdjustForVerticalAlignment(hdc, ptext, text.Length, bounds, flags, &dtparams);

			// Adjust unbounded rect to avoid overflow since Rectangle ctr does not do param validation.
			if(bounds.Width  == int.MaxValue) bounds.Width  -= bounds.X;
			if(bounds.Height == int.MaxValue) bounds.Height -= bounds.Y;

			var rect = new RECT(bounds);
			_ = User32.DrawTextExW(hdc, ptext, text.Length, &rect, flags, &dtparams);
		}


		/* No need to restore previous objects into the dc (see comments on top of the class).
		 *             
		if (hOldFont != IntPtr.Zero) 
		{
			IntUnsafeNativeMethods.SelectObject(hdc, new HandleRef( null, hOldFont));
		}

		if( foreColor != textColor ) 
		{
			this.dc.SetTextColor(textColor);
		}

		if( backColor != bkColor )
		{
			this.dc.SetBackgroundColor(bkColor);
		}

		if( bckMode != newMode ) 
		{
			this.dc.SetBackgroundMode(bckMode);
		}

		if( align != DeviceContextTextAlignment.Default )
		{
			// Default text alignment required by DrewText.
			this.dc.SetTextAlignment(align);
		}
		*/
	}
}
