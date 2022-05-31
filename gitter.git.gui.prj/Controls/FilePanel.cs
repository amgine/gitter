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

namespace gitter.Git.Gui.Controls;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

public abstract class FilePanel : FlowPanel
{
	protected static readonly StringFormat HeaderFormat = new(StringFormat.GenericDefault)
	{
		Alignment = StringAlignment.Near,
		FormatFlags =
			StringFormatFlags.LineLimit |
			StringFormatFlags.NoClip |
			StringFormatFlags.NoWrap,
		LineAlignment = StringAlignment.Center,
		Trimming = StringTrimming.None,
	};

	protected static readonly StringFormat ContentFormat = new(StringFormat.GenericTypographic)
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

	private static Dpi  CellSizeDpi;
	private static Size CellSize;

	protected static Size GetCellSize(Dpi dpi)
	{
		if(CellSizeDpi != dpi)
		{
			CellSize = GitterApplication.TextRenderer.MeasureText(
				GraphicsUtility.MeasurementGraphics,
				"0",
				GitterApplication.FontManager.ViewerFont.ScalableFont.GetValue(dpi),
				int.MaxValue / 2,
				StringFormat.GenericTypographic);
			CellSizeDpi = dpi;
		}
		return CellSize;
	}

	protected const int Margin    = 5;
	protected const int MinDigits = 4;

	static FilePanel()
	{
		float tabSize = CellSize.Width * TabSize;
		ContentFormat.SetTabStops(TabSize, new[]
			{ tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, });
	}

	protected virtual bool ShowHeader => true;

	protected int HeaderHeight
	{
		get
		{
			const int BaseValue = 35;

			if(FlowControl is not null)
			{
				var conv = new DpiConverter(FlowControl);
				return conv.ConvertY(BaseValue);
			}

			return BaseValue;
		}
	}

	protected static int TabSize
		=> GitterApplication.TextRenderer == GitterApplication.GdiPlusTextRenderer ? 4 : 8;

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

	private void PaintHeaderBackground(Graphics graphics, Rectangle rcHeader, Rectangle clipRectangle)
	{
		if(Style.Colors.FileHeaderColor1 == Style.Colors.FileHeaderColor2)
		{
			var rcBackground = Rectangle.Intersect(rcHeader, clipRectangle);
			if(rcBackground is { Width: > 0, Height: > 0 })
			{
				graphics.GdiFill(Style.Colors.FileHeaderColor1, rcBackground);
			}
		}
		else
		{
			using var brush = new LinearGradientBrush(
				rcHeader,
				Style.Colors.FileHeaderColor1,
				Style.Colors.FileHeaderColor2,
				LinearGradientMode.Vertical);
			graphics.FillRectangle(brush, rcHeader);
		}
	}

	private void PaintHeaderBorder(Graphics graphics, Rectangle rcHeader)
	{
		var rcBorder = rcHeader;
		--rcBorder.Height;
		--rcBorder.Width;
		using var pen = new Pen(Style.Colors.FilePanelBorder);
		graphics.DrawRectangle(pen, rcBorder);
	}

	protected void PaintHeader(Graphics graphics, Dpi dpi, Rectangle rcHeader, Rectangle clipRectangle,
		Image icon, Image overlay, string text)
	{
		PaintHeaderBackground(graphics, rcHeader, clipRectangle);
		PaintHeaderBorder(graphics, rcHeader);

		var iconSize = new Size(icon.Width, icon.Height);
		var offset   = (HeaderHeight - iconSize.Height) / 2;
		var rcStatus = new Rectangle(rcHeader.X + offset, rcHeader.Y + offset, iconSize.Width, iconSize.Height);
		graphics.DrawImage(icon, rcStatus);
		if(overlay is not null)
		{
			graphics.DrawImage(overlay, rcStatus);
		}

		var font = GitterApplication.FontManager.ViewerFont.ScalableFont.GetValue(dpi);
		rcHeader.X     += offset * 2 + iconSize.Width;
		rcHeader.Width -= offset * 2 + iconSize.Width;
		GitterApplication.TextRenderer.DrawText(
			graphics, text, font, Style.Colors.WindowText, rcHeader, HeaderFormat);
	}

	protected static int GetDecimalDigits(int num)
	{
		if(num < 1000) return MinDigits;

		var digits = 1;
		while(num != 0)
		{
			num /= 10;
			++digits;
		}
		return digits;
	}
}
