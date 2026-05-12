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

namespace gitter.GitLab.Gui;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.GitLab.Api;

using Resources = gitter.GitLab.Properties.Resources;

sealed class IssueLabelsColumn : CustomListBoxColumn
{
	private const int BaseChipHeight = 14;
	private const int BaseChipPadX   = 4;
	private const int BaseChipGap    = 4;

	public IssueLabelsColumn()
		: base((int)ColumnId.Labels, Resources.StrLabels, visible: true)
	{
		Width = 180;
	}

	public override string IdentificationString => "Labels";

	protected override Comparison<CustomListBoxItem> SortComparison
		=> static (a, b) =>
		{
			var sa = AsKey(a);
			var sb = AsKey(b);
			return string.CompareOrdinal(sa, sb);
		};

	private static string AsKey(CustomListBoxItem item)
		=> item is IssueListItem { DataContext.Labels: { } labels }
			? string.Join(",", labels)
			: string.Empty;

	private static bool TryGetLabels(CustomListBoxItem item, out string[] names)
	{
		if(item is IssueListItem { DataContext.Labels: { Length: not 0 } src })
		{
			names = src;
			return true;
		}
		names = Array.Empty<string>();
		return false;
	}

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		if(!TryGetLabels(measureEventArgs.Item, out var names))
			return base.OnMeasureSubItem(measureEventArgs);

		var conv = measureEventArgs.DpiConverter;
		var h    = conv.ConvertY(BaseChipHeight + 4);
		var w    = MeasureChipsTotalWidth(measureEventArgs.Gaphics, measureEventArgs.Column.ContentFont, conv, names);
		return new Size(w, h);
	}

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		if(!TryGetLabels(paintEventArgs.Item, out var names))
		{
			base.OnPaintSubItem(paintEventArgs);
			return;
		}

		var g    = paintEventArgs.Graphics;
		var conv = paintEventArgs.DpiConverter;
		var font = paintEventArgs.Column.ContentFont;

		var chipHeight = conv.ConvertY(BaseChipHeight);
		var padX       = conv.ConvertX(BaseChipPadX);
		var gap        = conv.ConvertX(BaseChipGap);

		var bounds = paintEventArgs.Bounds;
		var top    = bounds.Y + (bounds.Height - chipHeight) / 2;
		var x      = bounds.X + conv.ConvertX(2);
		var right  = bounds.Right - conv.ConvertX(2);

		var registry = LabelRegistry.Current;
		var oldHint  = g.TextRenderingHint;
		var oldMode  = g.SmoothingMode;
		g.SmoothingMode      = SmoothingMode.AntiAlias;
		g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		try
		{
			for(var i = 0; i < names.Length; i++)
			{
				var name = names[i];
				var label = registry?.Find(name);
				var color = ParseColor(label?.Color) ?? Color.FromArgb(94, 110, 130);
				var textColor = ParseColor(label?.TextColor) ?? AutoContrast(color);

				var textWidth = (int)Math.Ceiling(g.MeasureString(name, font).Width);
				var chipWidth = textWidth + 2 * padX;

				if(x + chipWidth > right)
				{
					var remaining = names.Length - i;
					var more = "+" + remaining.ToString(CultureInfo.InvariantCulture);
					var moreW = (int)Math.Ceiling(g.MeasureString(more, font).Width);
					var moreChip = moreW + 2 * padX;
					if(x + moreChip <= right + padX)
					{
						DrawChip(g, font, x, top, moreChip, chipHeight, Color.Gray, Color.White, more, padX);
					}
					break;
				}

				DrawChip(g, font, x, top, chipWidth, chipHeight, color, textColor, name, padX);
				x += chipWidth + gap;
			}
		}
		finally
		{
			g.TextRenderingHint = oldHint;
			g.SmoothingMode     = oldMode;
		}
	}

	private static void DrawChip(Graphics g, Font font, int x, int y, int w, int h, Color fill, Color text, string label, int padX)
	{
		var radius = Math.Max(2, h / 2);
		using var path = RoundedRect(new Rectangle(x, y, w, h), radius);
		using var brush = new SolidBrush(fill);
		g.FillPath(brush, path);
		TextRenderer.DrawText(g, label, font, new Rectangle(x + padX, y, w - 2 * padX, h),
			text, TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
	}

	private static int MeasureChipsTotalWidth(Graphics g, Font font, DpiConverter conv, string[] names)
	{
		var padX = conv.ConvertX(BaseChipPadX);
		var gap  = conv.ConvertX(BaseChipGap);
		var total = 0;
		for(var i = 0; i < names.Length; i++)
		{
			var w = (int)Math.Ceiling(g.MeasureString(names[i], font).Width) + 2 * padX;
			total += w;
			if(i < names.Length - 1) total += gap;
		}
		return total + conv.ConvertX(4);
	}

	private static Color? ParseColor(string? hex)
	{
		if(string.IsNullOrEmpty(hex)) return null;
		var s = hex!.TrimStart('#');
		if(s.Length == 3)
		{
			// #abc -> #aabbcc
			s = string.Concat(s[0], s[0], s[1], s[1], s[2], s[2]);
		}
		if(s.Length != 6) return null;
		if(int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
		{
			return Color.FromArgb(0xFF, (rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
		}
		return null;
	}

	// YIQ luminance contrast
	internal static Color AutoContrast(Color background)
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
		path.AddArc(rect.X,                rect.Y,                d, d, 180, 90);
		path.AddArc(rect.Right - d - 1,    rect.Y,                d, d, 270, 90);
		path.AddArc(rect.Right - d - 1,    rect.Bottom - d - 1,   d, d,   0, 90);
		path.AddArc(rect.X,                rect.Bottom - d - 1,   d, d,  90, 90);
		path.CloseFigure();
		return path;
	}
}
