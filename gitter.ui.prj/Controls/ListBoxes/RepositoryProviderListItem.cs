#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Controls;

using System;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

public sealed class RepositoryProviderListItem : CustomListBoxItem<IRepositoryProvider>
{
	public RepositoryProviderListItem(IRepositoryProvider data)
		: base(data)
	{
	}

	private Image GetIcon(Dpi dpi, int size = 16)
		=> DataContext.Icon?.GetImage(dpi.X * 16 / 96);

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		switch(paintEventArgs.SubItemId)
		{
			case 0:
				var icon = GetIcon(paintEventArgs.Dpi);
				paintEventArgs.PaintImageAndText(icon, DataContext.DisplayName);
				break;
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		switch(measureEventArgs.SubItemId)
		{
			case 0:
				var icon = GetIcon(measureEventArgs.Dpi);
				return measureEventArgs.MeasureImageAndText(icon, DataContext.DisplayName);
			default:
				return Size.Empty;
		}
	}
}
