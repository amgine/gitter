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

public sealed class MSVS2012ProgressBarRenderer(MSVS2012ProgressBarRenderer.ColorTable colorTable) : CustomProgressBarRenderer
{
	public readonly record struct Colors(
		Color Background,
		Color Foreground,
		Color Border);

	public sealed record class ColorTable(
		Colors Normal,
		Colors Disabled)
	{
		public static ColorTable Dark { get; } = new(
			Normal: new(
				Background: Color.FromArgb( 32,  32,  32),
				Foreground: Color.FromArgb(190, 190, 190),
				Border:     Color.FromArgb( 84,  84,  92)),
			Disabled: new(
				Border:     Color.FromArgb( 67,  67,  70),
				Foreground: Color.FromArgb(109, 109, 109),
				Background: Color.FromArgb( 37,  37,  38)));
	}

	public static ColorTable DarkColors => ColorTable.Dark;

	private Colors GetColors(CustomProgressBar progressBar)
	{
		if(!progressBar.Enabled) return colorTable.Disabled;
		return colorTable.Normal;
	}

	public override void Render(Graphics graphics, Rectangle clipRectangle, CustomProgressBar progressBar)
	{
		var colors = GetColors(progressBar);
		var bounds = new Rectangle(Point.Empty, progressBar.Size);
		using var hdc = graphics.AsGdi();
		var conv = DpiConverter.FromDefaultTo(progressBar);
		var borderThickness = conv.ConvertX(1);
		hdc.DrawBorder(bounds, colors.Border, colors.Background, borderThickness);
		borderThickness += conv.ConvertX(1);
		bounds.Inflate(-borderThickness, -borderThickness);

		if(progressBar.IsIndeterminate)
		{
			var ticks = Environment.TickCount & int.MaxValue;
			ticks = ticks > progressBar.AnimationTimestamp
				? ticks - progressBar.AnimationTimestamp
				: ticks + int.MaxValue - progressBar.AnimationTimestamp;
			ticks /= 6;
			var w = bounds.Width / 3;
			var range = bounds.Width + w;
			var offset = ticks % range;
			var b = bounds;
			b.X += offset - w;
			b.Width = w;
			b.Intersect(bounds);
			b.Intersect(clipRectangle);
			if(b.Width > 0 && b.Height > 0)
			{
				hdc.Fill(colors.Foreground, b);
			}
		}
		else
		{
			var min = progressBar.Minimum;
			var max = progressBar.Maximum;
			if(max > min)
			{
				var value = progressBar.Value;
				if(value < min) value = min;
				if(value > max) value = max;

				var w = (long)bounds.Width * value / (max - min);
				var b = bounds;
				b.Width = (int)w;
				b.Intersect(clipRectangle);
				if(b.Width > 0 && b.Height > 0)
				{
					hdc.Fill(colors.Foreground, b);
				}
			}
		}
	}
}
