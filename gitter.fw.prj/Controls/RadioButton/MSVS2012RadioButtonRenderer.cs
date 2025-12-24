#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Windows.Forms;

sealed class MSVS2012RadioButtonRenderer(MSVS2012RadioButtonRenderer.ColorTable colorTable) : CustomRadioButtonRenderer
{
	public static ColorTable DarkColors => ColorTable.Dark;

	public readonly record struct Colors(
		Color Foreground,
		Color CheckBackground,
		Color CheckBorder = default);

	public sealed record class ColorTable(
		Colors Normal,
		Colors Highlight,
		Colors Disabled)
	{
		public static ColorTable Dark { get; } = new(
			Normal: new(
				Foreground:      Color.FromArgb(190, 190, 190),
				CheckBackground: Color.FromArgb( 63,  63,  63)),
			Highlight: new(
				Foreground:      Color.FromArgb(241, 241, 241),
				CheckBackground: Color.FromArgb( 70,  70,  70),
				CheckBorder:     Color.FromArgb( 85, 170, 255)),
			Disabled: new(
				Foreground:      Color.FromArgb(101, 101, 101),
				CheckBackground: Color.FromArgb( 63,  63,  63)));
	}

	private Colors GetColors(CustomRadioButton radioButton)
	{
		if(!radioButton.Enabled) return colorTable.Disabled;
		var highlight = radioButton.IsMouseOver || radioButton.Focused;
		return highlight ? colorTable.Highlight : colorTable.Normal;
	}

	public override Rectangle Measure(CustomRadioButton radioButton)
	{
		var conv         = DpiConverter.FromDefaultTo(radioButton);
		var checkBoxSize = conv.Convert(new Size(16, 16));
		var bounds       = radioButton.ClientRectangle;
		var textOffset   = checkBoxSize.Width;
		if(radioButton.Image is not null)
		{
			var imageSize = conv.Convert(new Size(16, 16));
			textOffset += imageSize.Width + conv.ConvertX(4);
		}
		var textSize     = TextRenderer.MeasureText(radioButton.Text, radioButton.Font, bounds.Size);
		if(textSize.Width < bounds.Width - textOffset)
		{
			bounds.Width = textSize.Width + textOffset;
		}
		return bounds;
	}

	public override void Render(Graphics graphics, Dpi dpi, Rectangle clipRectangle, CustomRadioButton radioButton)
	{
		var conv         = DpiConverter.FromDefaultTo(dpi);
		var checkBoxSize = conv.Convert(new Size(16, 16));
		if(radioButton.BackColor != Color.Transparent)
		{
			graphics.GdiFill(radioButton.BackColor, clipRectangle);
		}
		var colors = GetColors(radioButton);
		var rcRadioButton = new Rectangle(1, 1 + (radioButton.Height - checkBoxSize.Height) / 2, checkBoxSize.Height - 2, checkBoxSize.Height - 2);
		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			using(var brush = SolidBrushCache.Get(colors.CheckBackground))
			{
				graphics.FillEllipse(brush, rcRadioButton);
			}
			rcRadioButton.X -= 1;
			rcRadioButton.Y -= 1;
			rcRadioButton.Width += 1;
			rcRadioButton.Height += 1;
			if(colors.CheckBorder.A != 0)
			{
				using var pen = new Pen(colors.CheckBorder);
				graphics.DrawEllipse(pen, rcRadioButton);
			}
			if(radioButton.IsChecked)
			{
				var dx = conv.ConvertX(4);
				var dy = conv.ConvertY(4);
				var rect = new RectangleF(
					rcRadioButton.X + dx,
					rcRadioButton.Y + dy,
					rcRadioButton.Width  - dx * 2,
					rcRadioButton.Height - dy * 2);
				using var brush = SolidBrushCache.Get(colors.Foreground);
				graphics.FillEllipse(brush, rect);
			}
			int textOffset = checkBoxSize.Width;
			if(radioButton.Image is not null)
			{
				var image     = radioButton.Image;
				var imageSize = conv.Convert(new Size(16, 16));
				textOffset += imageSize.Width + conv.ConvertX(4);
				graphics.DrawImage(image, new Rectangle(
					checkBoxSize.Width + conv.ConvertX(3),
					(radioButton.Height - image.Height) / 2,
					imageSize.Width, imageSize.Height));
			}
			var text = radioButton.Text;
			if(!string.IsNullOrWhiteSpace(text))
			{
				TextRenderer.DrawText(
					graphics,
					text,
					radioButton.Font,
					new Rectangle(textOffset, 0, radioButton.Width - textOffset, radioButton.Height),
					colors.Foreground,
					TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
			}
		}
	}
}
