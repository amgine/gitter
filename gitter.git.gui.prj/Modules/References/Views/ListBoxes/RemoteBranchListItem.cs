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

namespace gitter.Git.Gui.Controls;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="RemoteBranch"/> object.</summary>
public sealed class RemoteBranchListItem : ReferenceListItemBase<RemoteBranch>
{
	/// <summary>Create <see cref="RemoteBranchListItem"/>.</summary>
	/// <param name="branch">Related <see cref="RemoteBranch"/>.</param>
	/// <exception cref="ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
	public RemoteBranchListItem(RemoteBranch branch)
		: base(branch)
	{
		Verify.Argument.IsNotNull(branch);
	}

	protected override Image GetImage(Dpi dpi)
		=> Icons.RemoteBranch.GetImage(DpiConverter.FromDefaultTo(dpi).ConvertX(16));

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				return measureEventArgs.MeasureImageAndText(GetImage(measureEventArgs.Dpi),
					Parent is RemoteListItem rli ? DataContext.Name.Substring(rli.DataContext.Name.Length + 1) : DataContext.Name);
			default:
				return base.OnMeasureSubItem(measureEventArgs);
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
				paintEventArgs.PaintImageAndText(GetImage(paintEventArgs.Dpi),
					Parent is RemoteListItem rli ? DataContext.Name.Substring(rli.DataContext.Name.Length + 1) : DataContext.Name);
				break;
			default:
				base.OnPaintSubItem(paintEventArgs);
				break;
		}
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		var menu = new BranchMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
