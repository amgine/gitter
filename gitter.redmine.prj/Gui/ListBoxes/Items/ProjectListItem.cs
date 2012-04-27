namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
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
					paintEventArgs.PaintText(DataContext.CreatedOn.ToString());
					break;
				case ColumnId.UpdatedOn:
					paintEventArgs.PaintText(DataContext.UpdatedOn.ToString());
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
					return measureEventArgs.MeasureText(DataContext.CreatedOn.ToString());
				case ColumnId.UpdatedOn:
					return measureEventArgs.MeasureText(DataContext.UpdatedOn.ToString());
				default:
					return Size.Empty;
			}
		}
	}
}
