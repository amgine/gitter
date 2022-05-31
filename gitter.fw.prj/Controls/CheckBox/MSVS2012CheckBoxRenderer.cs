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

sealed class MSVS2012CheckBoxRenderer : CustomCheckBoxRenderer
{
	#region Color Tables

	private static IColorTable _darkColors;

	public static IColorTable DarkColors => _darkColors ??= new DarkColorTable();

	public interface IColorTable
	{
		Color Foreground { get; }

		Color ForegroundHighlight { get; }

		Color ForegroundDisabled { get; }

		Color CheckBackground { get; }

		Color CheckBackgroundHighlight { get; }

		Color CheckHighlight { get; }
	}

	private sealed class DarkColorTable : IColorTable
	{
		private static readonly Color FOREGROUND = Color.FromArgb(153, 153, 153);
		private static readonly Color FOREGROUND_HIGHLIGHT = Color.FromArgb(241, 241, 241);
		private static readonly Color FOREGROUND_DISABLED = Color.FromArgb(101, 101, 101);
		private static readonly Color CHECK_BACKGROUND = Color.FromArgb(63, 63, 63);
		private static readonly Color CHECK_BACKGROUND_HIGHLIGHT = Color.FromArgb(70, 70, 70);
		private static readonly Color CHECK_HIGHLIGHT = Color.FromArgb(85, 170, 255);

		public Color Foreground => FOREGROUND;

		public Color ForegroundHighlight => FOREGROUND_HIGHLIGHT;

		public Color ForegroundDisabled => FOREGROUND_DISABLED;

		public Color CheckBackground => CHECK_BACKGROUND;

		public Color CheckBackgroundHighlight => CHECK_BACKGROUND_HIGHLIGHT;

		public Color CheckHighlight => CHECK_HIGHLIGHT;
	}

	#endregion

	public MSVS2012CheckBoxRenderer(IColorTable colorTable)
	{
		Verify.Argument.IsNotNull(colorTable);

		ColorTable = colorTable;
	}

	private IColorTable ColorTable { get; }

	public override void Render(Graphics graphics, Dpi dpi, Rectangle clipRectangle, CustomCheckBox checkBox)
	{
		var conv         = DpiConverter.FromDefaultTo(dpi);
		var checkBoxSize = conv.Convert(new Size(16, 16));
		if(checkBox.BackColor != Color.Transparent)
		{
			graphics.GdiFill(checkBox.BackColor, clipRectangle);
		}
		bool highlight;
		Color foregroundColor;
		if(checkBox.Enabled)
		{
			highlight = checkBox.IsMouseOver || checkBox.Focused;
			foregroundColor = highlight ? ColorTable.ForegroundHighlight : ColorTable.Foreground;
		}
		else
		{
			highlight = false;
			foregroundColor = ColorTable.ForegroundDisabled;
		}
		var rcCheckBox = new Rectangle(1, 1 + (checkBox.Height - checkBoxSize.Height) / 2, checkBoxSize.Height - 2, checkBoxSize.Height - 2);
		using(var brush = new SolidBrush(highlight ? ColorTable.CheckBackgroundHighlight : ColorTable.CheckBackground))
		{
			graphics.FillRectangle(brush, rcCheckBox);
		}
		rcCheckBox.X -= 1;
		rcCheckBox.Y -= 1;
		rcCheckBox.Width += 1;
		rcCheckBox.Height += 1;
		if(highlight)
		{
			using var pen = new Pen(ColorTable.CheckHighlight);
			graphics.DrawRectangle(pen, rcCheckBox);
		}
		switch(checkBox.CheckState)
		{
			case CheckState.Checked:
				{
					var path = new Point[]
					{
						new Point(conv.ConvertX( 4), conv.ConvertY( 7) + rcCheckBox.Y),
						new Point(conv.ConvertX( 6), conv.ConvertY(10) + rcCheckBox.Y),
						new Point(conv.ConvertX(11), conv.ConvertY( 3) + rcCheckBox.Y),
					};
					using(var pen = new Pen(foregroundColor, conv.ConvertX(1.7f)))
					using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
					{
						graphics.DrawLines(pen, path);
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
					using(var brush = new SolidBrush(foregroundColor))
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
				foregroundColor,
				TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
		}
	}
}
