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

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.GitLab.Api;

using Resources = gitter.GitLab.Properties.Resources;

sealed class IssueAssigneeColumn : CustomListBoxColumn
{
	public IssueAssigneeColumn()
		: base((int)ColumnId.Assignee, Resources.StrAssignee, visible: false)
	{
		Width = 140;
	}

	public override string IdentificationString => "Assignee";

	protected override Comparison<CustomListBoxItem> SortComparison => IssueListItem.CompareByAssignee;

	private static Assignee? GetUser(CustomListBoxItem item)
		=> item is IssueListItem { DataContext.Assignees: { Length: not 0 } assignees }
			? assignees[0]
			: null;

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		var user = GetUser(measureEventArgs.Item);
		if(user?.Name is not { } name) return base.OnMeasureSubItem(measureEventArgs);
		return GitterApplication.IntegrationFeatures.Gravatar.IsEnabled && user.Avatar is { } avatar
			? measureEventArgs.MeasureImageAndText(avatar.Image, name)
			: measureEventArgs.MeasureText(name);
	}

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		var user = GetUser(paintEventArgs.Item);
		if(user?.Name is not { } name) { base.OnPaintSubItem(paintEventArgs); return; }

		if(GitterApplication.IntegrationFeatures.Gravatar.IsEnabled && user.Avatar is { } avatar)
		{
			if(avatar.Image is null) UpdateAvatarAsync(avatar, paintEventArgs.Item, paintEventArgs.Column);
			paintEventArgs.PaintImageAndText(avatar.Image, ImagePainter.Circle, name);
			return;
		}
		paintEventArgs.PaintText(name);
	}

	private static async void UpdateAvatarAsync(IAvatar avatar, CustomListBoxItem item, CustomListBoxColumn column)
	{
		await avatar.UpdateAsync();
		item.InvalidateSubItem(column.Id);
	}
}
