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

namespace gitter;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;

internal sealed class RecentRepositoryListItem : CustomListBoxItem<RepositoryLink>
{
	private static readonly StringFormat PathStringFormat;

	static RecentRepositoryListItem()
	{
		PathStringFormat = new StringFormat(GitterApplication.TextRenderer.LeftAlign);
		PathStringFormat.Trimming = StringTrimming.EllipsisPath;
		PathStringFormat.FormatFlags |= StringFormatFlags.NoClip;
	}

	public RecentRepositoryListItem(RepositoryLink repository)
		: base(repository)
	{
		Verify.Argument.IsNotNull(repository);
	}

	private static Image? GetIcon(Dpi dpi)
		=> Icons.Repository.GetImage(DpiConverter.FromDefaultTo(dpi).ConvertX(16));

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch(measureEventArgs.SubItemId)
		{
			case 0:
				return measureEventArgs.MeasureImageAndText(GetIcon(measureEventArgs.Dpi), DataContext.Path);
			default:
				return Size.Empty;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch(paintEventArgs.SubItemId)
		{
			case 0:
				paintEventArgs.PaintImageAndText(GetIcon(paintEventArgs.Dpi), DataContext.Path, paintEventArgs.Brush, PathStringFormat);
				break;
		}
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		var menu = new RecentRepositoryMenu(this);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
