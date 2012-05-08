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
