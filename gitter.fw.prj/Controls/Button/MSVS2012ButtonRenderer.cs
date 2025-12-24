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
using System.Windows.Forms;

public sealed class MSVS2012ButtonRenderer(MSVS2012ButtonRenderer.ColorTable colorTable) : CustomButtonRenderer
{
	public readonly record struct Colors(
		Color Background,
		Color Foreground,
		Color Border);

	public sealed record class ColorTable(
		Colors Normal,
		Colors Hover,
		Colors Pressed,
		Colors Disabled)
	{
		public static ColorTable Dark { get; } = new(
			Normal: new(
				Background: Color.FromArgb( 63,  63,  70),
				Foreground: MSVS2012DarkColors.WINDOW_TEXT,
				Border:     Color.FromArgb( 84,  84,  92)),
			Hover: new(
				Background: Color.FromArgb( 84,  84,  92),
				Foreground: MSVS2012DarkColors.WINDOW_TEXT,
				Border:     Color.FromArgb(106, 106, 117)),
			Pressed: new(
				Background: Color.FromArgb(  0, 122, 204),
				Foreground: MSVS2012DarkColors.WINDOW_TEXT,
				Border:     Color.FromArgb( 28, 151, 234)),
			Disabled: new(
				Border:     Color.FromArgb( 67,  67,  70),
				Foreground: Color.FromArgb(109, 109, 109),
				Background: Color.FromArgb( 37,  37,  38)));
	}

	public static ColorTable DarkColors => ColorTable.Dark;

	private Colors GetColors(CustomButton button)
	{
		if(!button.Enabled) return colorTable.Disabled;
		if(button.IsPressed && button.IsMouseOver) return colorTable.Pressed;
		if(button.Focused   || button.IsMouseOver) return colorTable.Hover;
		return colorTable.Normal;
	}

	public override void Render(Graphics graphics, Rectangle clipRectangle, CustomButton button)
	{
		var colors = GetColors(button);
		var bounds = new Rectangle(Point.Empty, button.Size);
		using(var hdc = graphics.AsGdi())
		{
			var conv = DpiConverter.FromDefaultTo(button);
			var borderThickness = conv.ConvertX(1);
			hdc.DrawBorder(bounds, colors.Border, colors.Background, borderThickness);
			bounds.Inflate(-borderThickness, -borderThickness);
		}
		TextRenderer.DrawText(graphics, button.Text, button.Font, bounds, colors.Foreground);
	}
}
