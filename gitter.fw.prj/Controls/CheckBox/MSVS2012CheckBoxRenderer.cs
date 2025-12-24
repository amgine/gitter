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
using System.Windows.Forms;

sealed class MSVS2012CheckBoxRenderer(MSVS2012CheckBoxRenderer.ColorTable colorTable) : CustomCheckBoxRenderer
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

	private Colors GetColors(CustomCheckBox checkBox)
	{
		if(!checkBox.Enabled) return colorTable.Disabled;
		var highlight = checkBox.IsMouseOver || checkBox.Focused;
		return highlight ? colorTable.Highlight : colorTable.Normal;
	}

	public override Rectangle Measure(CustomCheckBox checkBox)
	{
		var conv         = DpiConverter.FromDefaultTo(checkBox);
		var checkBoxSize = conv.Convert(new Size(16, 16));
		var bounds       = checkBox.ClientRectangle;
		var textOffset   = checkBoxSize.Width;
		if(checkBox.Image is not null)
		{
			var imageSize = conv.Convert(new Size(16, 16));
			textOffset += imageSize.Width + conv.ConvertX(4);
		}
		var textSize     = TextRenderer.MeasureText(checkBox.Text, checkBox.Font, bounds.Size);
		if(textSize.Width < bounds.Width - textOffset)
		{
			bounds.Width = textSize.Width + textOffset;
		}
		return bounds;
	}

	public override void Render(Graphics graphics, Rectangle clipRectangle, CustomCheckBox checkBox)
	{
		var conv         = DpiConverter.FromDefaultTo(checkBox);
		var checkBoxSize = conv.Convert(new Size(16, 16));
		if(checkBox.BackColor != Color.Transparent)
		{
			graphics.GdiFill(checkBox.BackColor, clipRectangle);
		}
		var colors = GetColors(checkBox);
		var rcCheckBox = new Rectangle(1, 1 + (checkBox.Height - checkBoxSize.Height) / 2, checkBoxSize.Height - 2, checkBoxSize.Height - 2);
		using(var brush = SolidBrushCache.Get(colors.CheckBackground))
		{
			graphics.FillRectangle(brush, rcCheckBox);
		}
		rcCheckBox.X -= 1;
		rcCheckBox.Y -= 1;
		rcCheckBox.Width += 1;
		rcCheckBox.Height += 1;
		if(colors.CheckBorder.A != 0)
		{
			using var pen = new Pen(colors.CheckBorder);
			graphics.DrawRectangle(pen, rcCheckBox);
		}
		switch(checkBox.CheckState)
		{
			case CheckState.Checked:
				{
					using(var pen = new Pen(colors.Foreground, conv.ConvertX(1.7f)))
					using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
					{
						graphics.DrawLines(pen,
							[
								new(conv.ConvertX( 4), conv.ConvertY( 7) + rcCheckBox.Y),
								new(conv.ConvertX( 6), conv.ConvertY(10) + rcCheckBox.Y),
								new(conv.ConvertX(11), conv.ConvertY( 3) + rcCheckBox.Y),
							]);
					}
				}
				break;
			case CheckState.Indeterminate:
				{
					var rect = new Rectangle(
						rcCheckBox.X + conv.ConvertX(5),
						rcCheckBox.Y + conv.ConvertY(5),
						rcCheckBox.Width  - conv.ConvertX(9),
						rcCheckBox.Height - conv.ConvertY(9));
					using(var brush = SolidBrushCache.Get(colors.Foreground))
					{
						graphics.FillRectangle(brush, rect);
					}
				}
				break;
		}
		int textOffset = checkBoxSize.Width;
		if(checkBox.Image is not null)
		{
			var image     = checkBox.Image;
			var imageSize = conv.Convert(new Size(16, 16));
			textOffset += imageSize.Width + conv.ConvertX(4);
			graphics.DrawImage(image, new Rectangle(checkBoxSize.Width + conv.ConvertX(3), (checkBox.Height - image.Height) / 2, imageSize.Width, imageSize.Height));
		}
		var text = checkBox.Text;
		if(!string.IsNullOrWhiteSpace(text))
		{
			TextRenderer.DrawText(
				graphics,
				text,
				checkBox.Font,
				new Rectangle(textOffset, 0, checkBox.Width - textOffset, checkBox.Height),
				colors.Foreground,
				TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
		}
	}
}
