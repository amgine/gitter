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

namespace gitter.Redmine.Gui
{
	using System;
	using System.Drawing;

	using gitter.Framework.Controls;

	public class ProjectListItem : CustomListBoxItem<Project>
	{
		public ProjectListItem(Project project)
			: base(project)
		{
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Id:
					paintEventArgs.PaintText(DataContext.Id.ToString());
					break;
				case ColumnId.Name:
					paintEventArgs.PaintText(DataContext.Name);
					break;
				case ColumnId.Description:
					paintEventArgs.PaintText(DataContext.Description);
					break;
				case ColumnId.Identifier:
					paintEventArgs.PaintText(DataContext.Identifier);
					break;
				case ColumnId.CreatedOn:
					DateColumn.OnPaintSubItem(paintEventArgs, DataContext.CreatedOn);
					break;
				case ColumnId.UpdatedOn:
					DateColumn.OnPaintSubItem(paintEventArgs, DataContext.UpdatedOn);
					break;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Id:
					return measureEventArgs.MeasureText(DataContext.Id.ToString());
				case ColumnId.Name:
					return measureEventArgs.MeasureText(DataContext.Name);
				case ColumnId.Description:
					return measureEventArgs.MeasureText(DataContext.Description);
				case ColumnId.Identifier:
					return measureEventArgs.MeasureText(DataContext.Identifier);
				case ColumnId.CreatedOn:
					return DateColumn.OnMeasureSubItem(measureEventArgs, DataContext.CreatedOn);
				case ColumnId.UpdatedOn:
					return DateColumn.OnMeasureSubItem(measureEventArgs, DataContext.UpdatedOn);
				default:
					return Size.Empty;
			}
		}
	}
}
