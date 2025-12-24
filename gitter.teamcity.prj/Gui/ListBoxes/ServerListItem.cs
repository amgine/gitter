#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity.Gui;

using System.Drawing;

using gitter.Framework.Controls;

class ServerListItem(ServerInfo server) : CustomListBoxItem<ServerInfo>(server)
{
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		switch((ColumnId)measureEventArgs.Column.Id)
		{
			case ColumnId.Name:
				var w = measureEventArgs.DpiConverter.ConvertX(16);
				return measureEventArgs.MeasureText(DataContext.Name);
			default:
				return base.OnMeasureSubItem(measureEventArgs);
		}
	}

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		switch((ColumnId)paintEventArgs.Column.Id)
		{
			case ColumnId.Name:
				var w = paintEventArgs.DpiConverter.ConvertX(16);
				paintEventArgs.PaintText(DataContext.Name);
				break;
			default:
				base.OnPaintSubItem(paintEventArgs);
				break;
		}
	}
}
