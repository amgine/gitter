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
using System.Globalization;

using gitter.Framework.Controls;

using gitter.GitLab.Api;

using Resources = gitter.GitLab.Properties.Resources;

public sealed class PipelineStatusColumn : CustomListBoxColumn
{
	public PipelineStatusColumn()
		: base((int)ColumnId.Status, Resources.StrStatus, true)
	{
		Width = 100;
	}

	/// <inheritdoc/>
	public override string IdentificationString => "Status";

	/// <inheritdoc/>
	protected override Comparison<CustomListBoxItem> SortComparison => PipelineListItem.CompareByStatus;

	private static bool TryGetContent(CustomListBoxItem item, out PipelineStatus value)
	{
		if(item is PipelineListItem pipeline)
		{
			value = pipeline.DataContext.Status;
			return true;
		}
		value = default;
		return false;
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(TryGetContent(measureEventArgs.Item, out var value))
		{
			GuiUtils.GetPipelineStatusIconAndName(value, out var image, out var name);
			return measureEventArgs.MeasureImageAndText(image, name);
		}
		return base.OnMeasureSubItem(measureEventArgs);
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(TryGetContent(paintEventArgs.Item, out var value))
		{
			GuiUtils.GetPipelineStatusIconAndName(value, out var image, out var name);
			paintEventArgs.PaintImageAndText(image, name);
			return;
		}
		base.OnPaintSubItem(paintEventArgs);
	}
}
