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
					paintEventArgs.PaintText(Data.Id.ToString());
					break;
				case ColumnId.Name:
					paintEventArgs.PaintText(Data.Name);
					break;
				case ColumnId.Description:
					paintEventArgs.PaintText(Data.Description);
					break;
				case ColumnId.Identifier:
					paintEventArgs.PaintText(Data.Identifier);
					break;
				case ColumnId.CreatedOn:
					paintEventArgs.PaintText(Data.CreatedOn.ToString());
					break;
				case ColumnId.UpdatedOn:
					paintEventArgs.PaintText(Data.UpdatedOn.ToString());
					break;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Id:
					return measureEventArgs.MeasureText(Data.Id.ToString());
				case ColumnId.Name:
					return measureEventArgs.MeasureText(Data.Name);
				case ColumnId.Description:
					return measureEventArgs.MeasureText(Data.Description);
				case ColumnId.Identifier:
					return measureEventArgs.MeasureText(Data.Identifier);
				case ColumnId.CreatedOn:
					return measureEventArgs.MeasureText(Data.CreatedOn.ToString());
				case ColumnId.UpdatedOn:
					return measureEventArgs.MeasureText(Data.UpdatedOn.ToString());
				default:
					return Size.Empty;
			}
		}
	}
}
