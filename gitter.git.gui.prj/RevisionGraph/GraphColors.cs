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

internal static class GraphColors
{
	public static readonly Color[] ColorsForLightBackground = MakePalette(Color.FromArgb( 26,  26,  26), 16, 0.7, 0.8);
	public static readonly Color[] ColorsForDarkBackground  = MakePalette(Color.FromArgb(224, 224, 224), 16, 0.7, 1.0);
	public static readonly int TotalColors = Math.Min(ColorsForLightBackground.Length, ColorsForDarkBackground.Length);

	public static readonly Brush DotBrushForLightBackground = Brushes.Black;
	public static readonly Brush DotBrushForDarkBackground  = Brushes.White;

	public static readonly Pen TagBorderPenForLightBackground = Pens.Black;
	public static readonly Pen TagBorderPenForDarkBackground  = new Pen(Color.FromArgb(180, 180, 180));
	//public static readonly Pen TagBorderPenForDarkBackground  = new Pen(Color.FromArgb(29, 29, 29));

	public static readonly Pen CirclePenForLightBackground = new(Color.Black, 1.5f);
	public static readonly Pen CirclePenForDarkBackground  = new(Color.White, 1.5f);

	static Color HSVtoRGB(double h, double s, double v)
	{
		var hh = h >= 360.0 ? 0.0 : h / 60.0;
		var i = (long)hh;
		var ff = hh - i;
		var p = v * (1.0 - s);
		var q = v * (1.0 - (s * ff));
		var t = v * (1.0 - (s * (1.0 - ff)));

		var (r, g, b) = i switch
		{
			0 => (v, t, p),
			1 => (q, v, p),
			2 => (p, v, t),
			3 => (p, q, v),
			4 => (t, p, v),
			_ => (v, p, q),
		};

		return Color.FromArgb(
			(byte)(r * 255 + 0.5),
			(byte)(g * 255 + 0.5),
			(byte)(b * 255 + 0.5));
	}

	static Color[] MakePalette(Color defaultColor, int n, double s, double v)
	{
		var colors = new Color[n + 1];
		colors[0] = defaultColor;
		int index = 1;
		const int k = 2;
		for(int j = 0; j < k; ++j)
		{
			for(int i = j; i < n; i += k)
			{
				var h = 360 * i / (float)n;
				colors[index++] = HSVtoRGB(h, s, v);
			}
		}
		return colors;
	}
}
