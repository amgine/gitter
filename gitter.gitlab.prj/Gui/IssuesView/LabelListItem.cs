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

using System.Drawing;

using gitter.Framework.Controls;

using gitter.GitLab.Api;

sealed class LabelListItem : CustomListBoxItem<Label>
{
	public LabelListItem(Label label)
		: base(label)
	{
		Verify.Argument.IsNotNull(label);

		CheckedState = CheckedState.Unchecked;
	}

	protected override string GetToolTipText()
		=> string.IsNullOrWhiteSpace(DataContext.Description)
			? DataContext.Name
			: DataContext.Name + "\n" + DataContext.Description;

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		if((ColumnId)measureEventArgs.SubItemId is not ColumnId.Name)
		{
			return base.OnMeasureSubItem(measureEventArgs);
		}

		var conv = measureEventArgs.DpiConverter;
		var padX = conv.ConvertX(LabelChip.BasePadX);
		var w = LabelChip.MeasureWidth(measureEventArgs.Gaphics,
			measureEventArgs.Column.ContentFont, DataContext.Name, padX);

		return new Size(w + conv.ConvertX(4), conv.ConvertY(LabelChip.BaseHeight + 4));
	}

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		if((ColumnId)paintEventArgs.SubItemId is not ColumnId.Name)
		{
			base.OnPaintSubItem(paintEventArgs);
			return;
		}

		var g    = paintEventArgs.Graphics;
		var conv = paintEventArgs.DpiConverter;
		var font = paintEventArgs.Column.ContentFont;

		var chipHeight = conv.ConvertY(LabelChip.BaseHeight);
		var padX       = conv.ConvertX(LabelChip.BasePadX);

		var bounds = paintEventArgs.Bounds;
		var top    = bounds.Y + (bounds.Height - chipHeight) / 2;
		var x      = bounds.X + conv.ConvertX(2);
		var right  = bounds.Right - conv.ConvertX(2);

		var (fill, text) = LabelChip.GetColors(DataContext);
		var width = LabelChip.MeasureWidth(g, font, DataContext.Name, padX);
		if(x + width > right) width = right - x;
		if(width <= 0) return;

		using var scope = LabelChip.BeginDraw(g);
		LabelChip.Draw(g, font, x, top, width, chipHeight, fill, text, DataContext.Name, padX);
	}
}
