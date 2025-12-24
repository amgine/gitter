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

static class CheckRenderer
{
	public record class ColorTable(
		ContentColors Normal,
		ContentColors Hover,
		ContentColors Pressed,
		ContentColors Disabled)
	{
		public ContentColors Select(bool isEnabled, bool isPressed, bool isMouseOver)
		{
			if(!isEnabled)  return Disabled;
			if(isMouseOver) return isPressed ? Pressed : Hover;
			return Normal;
		}
	}

	public record class ContentColors(
		UncheckedColors Unchecked,
		CheckedColors   Checked);

	public record class UncheckedColors(
		Color Background,
		Color Border);

	public record class CheckedColors(
		Color Background,
		Color Check,
		Color Border = default);

	public static readonly ColorTable DarkColorTable = new(
		Normal: new(
			Unchecked: new(
				Background: Color.FromArgb(0x25, 0x26, 0x33),
				Border:     Color.FromArgb(0x9D, 0x9E, 0xA4)),
			Checked: new(
				Background: Color.FromArgb(0x40, 0xBD, 0xFF),
				Check:      Color.FromArgb(0x00, 0x00, 0x00))),
		Pressed: new(
			Unchecked: new(
				Background: Color.FromArgb(0x38, 0x39, 0x45),
				Border:     Color.FromArgb(0x4D, 0x4E, 0x59)),
			Checked: new(
				Background: Color.FromArgb(0x3B, 0x9F, 0xD7),
				Check:      Color.FromArgb(0x1F, 0x58, 0x77))),
		Hover: new(
			Unchecked: new(
				Background: Color.FromArgb(0x32, 0x33, 0x40),
				Border:     Color.FromArgb(0x9D, 0x9E, 0xA4)),
			Checked: new(
				Background: Color.FromArgb(0x3D, 0xAE, 0xEB),
				Check:      Color.FromArgb(0x1F, 0x58, 0x77))),
		Disabled: new(
			Unchecked: new(
				Background: Color.FromArgb(0x32, 0x32, 0x32),
				Border:     Color.FromArgb(0x43, 0x43, 0x43)),
			Checked: new(
				Background: Color.FromArgb(0x43, 0x43, 0x43),
				Check:      Color.FromArgb(0xA7, 0xA7, 0xA7))));

	public static readonly ColorTable LightColorTable = new(
		Normal: new(
			Unchecked: new(
				Background: Color.FromArgb(0xF5, 0xF5, 0xF7),
				Border:     Color.FromArgb(0x8A, 0x8A, 0x8B)),
			Checked: new(
				Background: Color.FromArgb(0x00, 0x55, 0xA1),
				Check:      Color.FromArgb(0xFF, 0xFF, 0xFF))),
		Pressed: new(
			Unchecked: new(
				Background: Color.FromArgb(0xE3, 0xE3, 0xE5),
				Border:     Color.FromArgb(0xC0, 0xC0, 0xC2)),
			Checked: new(
				Background: Color.FromArgb(0x32, 0x76, 0xB2),
				Check:      Color.FromArgb(0xC2, 0xD6, 0xE8))),
		Hover: new(
			Unchecked: new(
				Background: Color.FromArgb(0xEC, 0xEC, 0xEE),
				Border:     Color.FromArgb(0x89, 0x89, 0x8A)),
			Checked: new(
				Background: Color.FromArgb(0x1A, 0x66, 0xAA),
				Check:      Color.FromArgb(0xFF, 0xFF, 0xFF))),
		Disabled: new(
			Unchecked: new(
				Background: Color.FromArgb(0xFF, 0xFF, 0xFF),
				Border:     Color.FromArgb(0xBF, 0xBF, 0xBF)),
			Checked: new(
				Background: Color.FromArgb(0xBF, 0xBF, 0xBF),
				Check:      Color.FromArgb(0xFF, 0xFF, 0xFF))));

	public static ColorTable GetColorTable(GitterStyleType styleType)
		=> styleType == GitterStyleType.DarkBackground
			? DarkColorTable
			: LightColorTable;

	public static ColorTable GetColorTable()
		=> GetColorTable(GitterApplication.Style.Type);

	private static RectangleF PrepareBounds(Rectangle bounds, int penWidth)
	{
		var rc = bounds;
		rc.X += penWidth / 2;
		rc.Y += penWidth / 2;
		rc.Width  -= (penWidth | 1);
		rc.Height -= (penWidth | 1);
		return rc;
	}

	static SizeF GetCornerRadius(Dpi dpi)
	{
		const int MinSize = 6;

		var conv   = DpiConverter.FromDefaultTo(dpi);
		var radius = conv.Convert(new Size(4, 4));
		if(radius.Width  < MinSize) radius.Width  = MinSize;
		if(radius.Height < MinSize) radius.Height = MinSize;
		return radius;
	}

	private static void AddRoundedRectangle(GraphicsPath path, RectangleF rect, SizeF radius)
	{
#if NET9_0_OR_GREATER
		path.AddRoundedRectangle(rect, radius);
#else
		path.StartFigure();
		path.AddArc(
			rect.Right - radius.Width,
			rect.Top,
			radius.Width,
			radius.Height,
			-90.0f, 90.0f);
		path.AddArc(
			rect.Right - radius.Width,
			rect.Bottom - radius.Height,
			radius.Width,
			radius.Height,
			0.0f, 90.0f);
		path.AddArc(
			rect.Left,
			rect.Bottom - radius.Height,
			radius.Width,
			radius.Height,
			90.0f, 90.0f);
		path.AddArc(
			rect.Left,
			rect.Top,
			radius.Width,
			radius.Height,
			180.0f, 90.0f);
		path.CloseFigure();
#endif
	}

	private static void RenderBox(Graphics graphics, Dpi dpi, Rectangle bounds, Color background, Color border)
	{
		Assert.IsNotNull(graphics);

		var conv     = DpiConverter.FromDefaultTo(dpi);
		var radius   = GetCornerRadius(dpi);
		var penWidth = conv.ConvertX(1);
		var rc       = PrepareBounds(bounds, penWidth);
		using var path = new GraphicsPath();
		AddRoundedRectangle(path, rc, radius);
		using(var brush = SolidBrushCache.Get(background))
		{
			graphics.FillPath(brush, path);
		}
		using(var pen = new Pen(border, penWidth))
		{
			graphics.DrawPath(pen, path);
		}
	}

	private static void RenderCheck(Graphics graphics, Dpi dpi, Rectangle bounds, Color color)
	{
		Assert.IsNotNull(graphics);

		var conv           = DpiConverter.FromDefaultTo(dpi);
		var checkThickness = conv.ConvertX(1.5f);
		if(checkThickness < 1.6f) checkThickness = 1.6f;
		var d = dpi.X == 96 ? bounds.Width / 5 : dpi.X * 4 / 96;
		var a = (bounds.Width - d * 2) / 3;
		var x0 = bounds.X + d;
		var y0 = bounds.Y + bounds.Height / 2;
		using var pen = new Pen(color, checkThickness);
		graphics.DrawLines(pen,
		[
			new PointF(x0,         y0    ),
			new PointF(x0 + a,     y0 + a),
			new PointF(x0 + a * 3, y0 - a),
		]);
	}

	private static void RenderBar(Graphics graphics, Dpi dpi, Rectangle bounds, Color color)
	{
		Assert.IsNotNull(graphics);

		var conv           = DpiConverter.FromDefaultTo(dpi);
		var checkThickness = conv.ConvertX(1.5f);
		if(checkThickness < 1.6f) checkThickness = 1.6f;
		var d = dpi.X == 96 ? bounds.Width / 5 : dpi.X * 4 / 96;
		var a = (bounds.Width - d * 2) / 3;
		using var pen = new Pen(color, checkThickness);
		graphics.DrawLine(pen, bounds.X + a, bounds.Y + bounds.Height / 2, bounds.Right - a, bounds.Y + bounds.Height / 2);
	}

	public static void Render(ContentColors colors, Graphics graphics, Dpi dpi, Rectangle bounds, bool @checked)
	{
		if(@checked)
		{
			RenderChecked(colors.Checked, graphics, dpi, bounds);
		}
		else
		{
			RenderUnchecked(colors.Unchecked, graphics, dpi, bounds);
		}
	}

	public static void Render(ContentColors colors, Graphics graphics, Dpi dpi, Rectangle bounds, CheckState state)
	{
		switch(state)
		{
			case CheckState.Unchecked:
				RenderUnchecked(colors.Unchecked, graphics, dpi, bounds);
				break;
			case CheckState.Checked:
				RenderChecked(colors.Checked, graphics, dpi, bounds);
				break;
			case CheckState.Indeterminate:
				RenderIndeterminate(colors.Checked, graphics, dpi, bounds);
				break;
		}
	}

	public static void Render(ContentColors colors, Graphics graphics, Dpi dpi, Rectangle bounds, CheckedState state)
	{
		switch(state)
		{
			case CheckedState.Unchecked:
				RenderUnchecked(colors.Unchecked, graphics, dpi, bounds);
				break;
			case CheckedState.Checked:
				RenderChecked(colors.Checked, graphics, dpi, bounds);
				break;
			case CheckedState.Indeterminate:
				RenderIndeterminate(colors.Checked, graphics, dpi, bounds);
				break;
		}
	}

	public static void RenderChecked(CheckedColors colors, Graphics graphics, Dpi dpi, Rectangle bounds)
	{
		Assert.IsNotNull(colors);
		Assert.IsNotNull(graphics);

		RenderBox  (graphics, dpi, bounds, colors.Background, colors.Border == default ? colors.Background : colors.Border);
		RenderCheck(graphics, dpi, bounds, colors.Check);
	}

	public static void RenderUnchecked(UncheckedColors colors, Graphics graphics, Dpi dpi, Rectangle bounds)
	{
		Assert.IsNotNull(colors);
		Assert.IsNotNull(graphics);

		RenderBox(graphics, dpi, bounds, colors.Background, colors.Border);
	}

	public static void RenderIndeterminate(CheckedColors colors, Graphics graphics, Dpi dpi, Rectangle bounds)
	{
		Assert.IsNotNull(colors);
		Assert.IsNotNull(graphics);

		RenderBox(graphics, dpi, bounds, colors.Background, colors.Border == default ? colors.Background : colors.Border);
		RenderBar(graphics, dpi, bounds, colors.Check);
	}

	public static Rectangle GetCheckBounds(Rectangle bounds)
	{
		var size = Math.Min(bounds.Width, bounds.Height);
		return new(bounds.X + (bounds.Width - size) / 2, bounds.Y + (bounds.Height - size) / 2, size, size);
	}
}
