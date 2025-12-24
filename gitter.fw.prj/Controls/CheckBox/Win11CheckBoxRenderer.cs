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

sealed class Win11CheckBoxRenderer : CustomCheckBoxRenderer
{
	public static Win11CheckBoxRenderer Instance { get; } = new();

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
		var style        = GitterApplication.Style;
		var conv         = DpiConverter.FromDefaultTo(checkBox);
		var checkBoxSize = conv.Convert(new Size(16, 16));
		if(checkBox.BackColor != Color.Transparent)
		{
			graphics.GdiFill(checkBox.BackColor, clipRectangle);
		}
		var rcCheckBox = new Rectangle(1, 1 + (checkBox.Height - checkBoxSize.Height) / 2, checkBoxSize.Height - 2, checkBoxSize.Height - 2);
		var table = style.Type == GitterStyleType.DarkBackground
			? CheckRenderer.DarkColorTable
			: CheckRenderer.LightColorTable;
		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			CheckRenderer.Render(
				table.Select(checkBox.Enabled, checkBox.IsPressed, checkBox.IsMouseOver),
				graphics, conv.To, rcCheckBox, checkBox.CheckState);
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
				style.Colors.WindowText,
				TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
		}
	}
}
