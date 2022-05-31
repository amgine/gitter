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

namespace gitter.Framework.Options;

using System;

using gitter.Framework.Controls;

using Resources = gitter.Framework.Properties.Resources;

internal sealed class FontsListBox : CustomListBox
{
	public FontsListBox()
	{
		Columns.Add(new CustomListBoxColumn(0, Resources.StrName, true) { SizeMode = ColumnSizeMode.Fill });
		Columns.Add(new CustomListBoxColumn(1, Resources.StrFont, true) { Width = new(200, Dpi.Default) });

		if(GitterApplication.FontManager is not null)
		{
			foreach(var f in GitterApplication.FontManager)
			{
				Items.Add(new FontListItem(f));
			}
		}

		ItemActivation = ItemActivation.SingleClick;
	}
}
