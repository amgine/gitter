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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public abstract class FilePanel : FlowPanel
	{
		protected static readonly StringFormat HeaderFormat = new StringFormat(StringFormat.GenericDefault)
		{
			Alignment = StringAlignment.Near,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.NoClip |
				StringFormatFlags.NoWrap,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.None,
		};

		protected static readonly StringFormat ContentFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.NoClip |
				StringFormatFlags.NoWrap |
				StringFormatFlags.FitBlackBox |
				StringFormatFlags.MeasureTrailingSpaces,
			LineAlignment = StringAlignment.Near,
			Trimming = StringTrimming.None,
		};

		protected Brush GetLineHoverBackground()
		{
			return new SolidBrush(Style.Colors.LineBackgroundHover);
		}

		protected Brush GetLineSelectedBackground()
		{
			return new SolidBrush(Style.Colors.LineSelectedBackground);
		}

		protected Brush GetLineSelectedHoverBackground()
		{
			return new SolidBrush(Style.Colors.LineSelectedBackgroundHover);
		}

		protected Brush GetLineNumberText()
		{
			return new SolidBrush(Style.Colors.LineNumberForeground);
		}

		protected Brush GetLineNumberBackground()
		{
			return new SolidBrush(Style.Colors.LineNumberBackground);
		}

		protected Brush GetLineNumberHoverBackground()
		{
			return new SolidBrush(Style.Colors.LineNumberBackgroundHover);
		}

		protected static readonly Size CellSize;

		protected const int HeaderHeight = 35;
		protected const int Margin = 5;
		protected const int MinDigits = 4;

		static FilePanel()
		{
			CellSize = GitterApplication.TextRenderer.MeasureText(
				GraphicsUtility.MeasurementGraphics,
				"0",
				GitterApplication.FontManager.ViewerFont.Font,
				int.MaxValue / 2,
				StringFormat.GenericTypographic);

			float tabSize = CellSize.Width * TabSize;
			ContentFormat.SetTabStops(TabSize, new[]
				{ tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, });
		}

		protected virtual bool ShowHeader
		{
			get { return true; }
		}

		protected static int TabSize
		{
			get
			{
				return (GitterApplication.TextRenderer == GitterApplication.GdiPlusTextRenderer) ? 4 : 8;
			}
		}

		protected Rectangle GetLineBounds(int line)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			int x = Margin;
			int w = contentWidth - Margin * 2;
			int yOffset = ShowHeader ? HeaderHeight : 1;
			return new Rectangle(x, yOffset + line * CellSize.Height, w, CellSize.Height);
		}

		protected Rectangle GetLineBounds(int line, int count)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			int x = Margin;
			int w = contentWidth - Margin * 2;
			int yOffset = ShowHeader ? HeaderHeight : 1;
			return new Rectangle(x, yOffset + line * CellSize.Height, w, CellSize.Height * count);
		}

		protected void PaintHeader(Graphics graphics, Rectangle rcHeader, Image icon, Image overlay, string text)
		{
			using(var brush = new LinearGradientBrush(
				rcHeader,
				Style.Colors.FileHeaderColor1,
				Style.Colors.FileHeaderColor2,
				LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(brush, rcHeader);
			}

			var rcBorder = rcHeader;
			--rcBorder.Height;
			--rcBorder.Width;
			using(var pen = new Pen(Style.Colors.FilePanelBorder))
			{
				graphics.DrawRectangle(pen, rcBorder);
			}

			var rcImage = new Rectangle(0, 0, 16, 16);
			const int offset = (HeaderHeight - 16) / 2;
			var rcStatus = new Rectangle(rcHeader.X + offset, rcHeader.Y + offset, 16, 16);
			graphics.DrawImage(icon, rcStatus, rcImage, GraphicsUnit.Pixel);
			if(overlay != null)
			{
				graphics.DrawImage(overlay, rcStatus, rcImage, GraphicsUnit.Pixel);
			}

			var font = GitterApplication.FontManager.ViewerFont.Font;
			rcHeader.X += offset * 2 + rcImage.Width;
			rcHeader.Width -= offset * 2 + rcImage.Width;
			GitterApplication.TextRenderer.DrawText(
				graphics, text, font, Style.Colors.WindowText, rcHeader, HeaderFormat);
		}

		protected static int GetDecimalDigits(int num)
		{
			if(num < 1000)
			{
				return MinDigits;
			}
			else
			{
				var digits = 1;
				while(num != 0)
				{
					num /= 10;
					++digits;
				}
				return digits;
			}
		}
	}
}
