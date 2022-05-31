﻿#region Copyright Notice
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
using gitter.Framework.Controls;

using Resources = gitter.GitLab.Properties.Resources;

sealed class IssueMilestoneColumn : CustomListBoxColumn
{
	public IssueMilestoneColumn()
		: base((int)ColumnId.Milestone, Resources.StrMilestone, true)
	{
		Width = 100;
	}

	public override string IdentificationString => "Milestone";

	protected override Comparison<CustomListBoxItem> SortComparison => IssueListItem.CompareByMilestone;

	private static string GetContent(CustomListBoxItem item)
	{
		if(item is IssueListItem issue)
		{
			return issue.DataContext.Milestone?.Title;
		}
		return default;
	}

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		var text = GetContent(measureEventArgs.Item);
		if(!string.IsNullOrWhiteSpace(text))
		{
			return measureEventArgs.MeasureText(text);
		}
		return base.OnMeasureSubItem(measureEventArgs);
	}

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		var text = GetContent(paintEventArgs.Item);
		if(!string.IsNullOrWhiteSpace(text))
		{
			paintEventArgs.PaintText(text);
			return;
		}
		base.OnPaintSubItem(paintEventArgs);
	}
}
