#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

using Label = gitter.GitLab.Api.Label;

static class LabelChip
{
	public const int BaseHeight = 14;
	public const int BasePadX   = 4;
	public const int BaseGap    = 4;

	public static readonly Color DefaultColor = Color.FromArgb(94, 110, 130);

	public static (Color Fill, Color Text) GetColors(Label? label)
	{
		var fill = ParseColor(label?.Color) ?? DefaultColor;
		var text = ParseColor(label?.TextColor) ?? AutoContrast(fill);
		return (fill, text);
	}

	public static int MeasureWidth(Graphics graphics, Font font, string text, int padX)
		=> (int)Math.Ceiling(graphics.MeasureString(text, font).Width) + 2 * padX;

	public static void Draw(Graphics graphics, Font font, int x, int y, int w, int h,
		Color fill, Color text, string label, int padX)
	{
		var radius = Math.Max(2, h / 2);
		using var path  = RoundedRect(new Rectangle(x, y, w, h), radius);
		using var brush = new SolidBrush(fill);
		graphics.FillPath(brush, path);
		TextRenderer.DrawText(graphics, label, font, new Rectangle(x + padX, y, w - 2 * padX, h),
			text, TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
	}

	public static GraphicsModeScope BeginDraw(Graphics graphics) => new(graphics);

	public readonly struct GraphicsModeScope : IDisposable
	{
		private readonly Graphics _graphics;
		private readonly System.Drawing.Text.TextRenderingHint _hint;
		private readonly SmoothingMode _mode;

		internal GraphicsModeScope(Graphics graphics)
		{
			_graphics = graphics;
			_hint     = graphics.TextRenderingHint;
			_mode     = graphics.SmoothingMode;

			graphics.SmoothingMode     = SmoothingMode.AntiAlias;
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		}

		public void Dispose()
		{
			if(_graphics is null) return;
			_graphics.TextRenderingHint = _hint;
			_graphics.SmoothingMode     = _mode;
		}
	}

	public static Color? ParseColor(string? hex)
	{
		if(string.IsNullOrEmpty(hex)) return null;

		var s = hex!.TrimStart('#');
		if(s.Length == 3)
		{
			s = string.Concat(s[0], s[0], s[1], s[1], s[2], s[2]);
		}
		if(s.Length != 6) return null;
		if(int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
		{
			return Color.FromArgb(0xFF, (rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
		}
		return null;
	}

	public static Color AutoContrast(Color background)
	{
		var yiq = (background.R * 299 + background.G * 587 + background.B * 114) / 1000;
		return yiq >= 128 ? Color.Black : Color.White;
	}

	private static GraphicsPath RoundedRect(Rectangle rect, int radius)
	{
		var path = new GraphicsPath();
		var d = radius * 2;
		if(d > rect.Width)  d = rect.Width;
		if(d > rect.Height) d = rect.Height;
		path.AddArc(rect.X,             rect.Y,              d, d, 180, 90);
		path.AddArc(rect.Right - d - 1, rect.Y,              d, d, 270, 90);
		path.AddArc(rect.Right - d - 1, rect.Bottom - d - 1, d, d,   0, 90);
		path.AddArc(rect.X,             rect.Bottom - d - 1, d, d,  90, 90);
		path.CloseFigure();
		return path;
	}
}
