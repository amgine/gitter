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
	private const int BaseChipHeight = LabelChip.BaseHeight;
	private const int BaseChipPadX   = LabelChip.BasePadX;
	private const int BaseChipGap    = LabelChip.BaseGap;

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
		using var scope = LabelChip.BeginDraw(g);

		for(var i = 0; i < names.Length; i++)
		{
			var name = names[i];
			var (color, textColor) = LabelChip.GetColors(registry?.Find(name));

			var chipWidth = LabelChip.MeasureWidth(g, font, name, padX);

			if(x + chipWidth > right)
			{
				var remaining = names.Length - i;
				var more = "+" + remaining.ToString(CultureInfo.InvariantCulture);
				var moreChip = LabelChip.MeasureWidth(g, font, more, padX);
				if(x + moreChip <= right + padX)
				{
					LabelChip.Draw(g, font, x, top, moreChip, chipHeight, Color.Gray, Color.White, more, padX);
				}
				break;
			}

			LabelChip.Draw(g, font, x, top, chipWidth, chipHeight, color, textColor, name, padX);
			x += chipWidth + gap;
		}
	}

	private static int MeasureChipsTotalWidth(Graphics g, Font font, DpiConverter conv, string[] names)
	{
		var padX = conv.ConvertX(BaseChipPadX);
		var gap  = conv.ConvertX(BaseChipGap);
		var total = 0;
		for(var i = 0; i < names.Length; i++)
		{
			total += LabelChip.MeasureWidth(g, font, names[i], padX);
			if(i < names.Length - 1) total += gap;
		}
		return total + conv.ConvertX(4);
	}
}
