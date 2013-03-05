namespace gitter.Redmine.Gui
{
	using System;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class VersionListItem : CustomListBoxItem<ProjectVersion>
	{
		#region Comparers

		public static int CompareById(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.DataContext.Id;
			var data2 = item2.DataContext.Id;
			return data1.CompareTo(data2);
		}

		public static int CompareById(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as VersionListItem;
			if(i1 == null) return 0;
			var i2 = item2 as VersionListItem;
			if(i2 == null) return 0;
			return CompareById(i1, i2);
		}

		public static int CompareByStatus(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.DataContext.Status;
			var data2 = item2.DataContext.Status;
			return data1.CompareTo(data2);
		}

		public static int CompareByStatus(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as VersionListItem;
			if(i1 == null) return 0;
			var i2 = item2 as VersionListItem;
			if(i2 == null) return 0;
			return CompareByStatus(i1, i2);
		}

		public static int CompareByCreatedOn(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.DataContext.CreatedOn;
			var data2 = item2.DataContext.CreatedOn;
			return data1.CompareTo(data2);
		}

		public static int CompareByCreatedOn(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as VersionListItem;
			if(i1 == null) return 0;
			var i2 = item2 as VersionListItem;
			if(i2 == null) return 0;
			return CompareByCreatedOn(i1, i2);
		}

		public static int CompareByUpdatedOn(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.DataContext.UpdatedOn;
			var data2 = item2.DataContext.UpdatedOn;
			return data1.CompareTo(data2);
		}

		public static int CompareByUpdatedOn(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as VersionListItem;
			if(i1 == null) return 0;
			var i2 = item2 as VersionListItem;
			if(i2 == null) return 0;
			return CompareByUpdatedOn(i1, i2);
		}

		public static int CompareByDueDate(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.DataContext.DueDate;
			var data2 = item2.DataContext.DueDate;
			if(data1 == data2) return 0;
			if(!data1.HasValue) return 1;
			else if(!data2.HasValue) return -1;
			return data1.Value.CompareTo(data2.Value);
		}

		public static int CompareByDueDate(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as VersionListItem;
			if(i1 == null) return 0;
			var i2 = item2 as VersionListItem;
			if(i2 == null) return 0;
			return CompareByDueDate(i1, i2);
		}

		public static int CompareByName(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.DataContext.Name;
			var data2 = item2.DataContext.Name;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1, data2);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as VersionListItem;
			if(i1 == null) return 0;
			var i2 = item2 as VersionListItem;
			if(i2 == null) return 0;
			return CompareByName(i1, i2);
		}

		public static int CompareByDescription(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.DataContext.Description;
			var data2 = item2.DataContext.Description;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1, data2);
		}

		public static int CompareByDescription(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as VersionListItem;
			if(i1 == null) return 0;
			var i2 = item2 as VersionListItem;
			if(i2 == null) return 0;
			return CompareByDescription(i1, i2);
		}

		#endregion

		public VersionListItem(ProjectVersion version)
			: base(version)
		{
			Verify.Argument.IsNotNull(version, "version");
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.PropertyChanged += OnVersionPropertyChanged;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.PropertyChanged -= OnVersionPropertyChanged;
			base.OnListBoxDetached();
		}

		private void OnVersionPropertyChanged(object sender, RedmineObjectPropertyChangedEventArgs e)
		{
			if(e.Property == ProjectVersion.IdProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Id);
			}
			else if(e.Property == ProjectVersion.NameProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Name);
			}
			else if(e.Property == ProjectVersion.CreatedOnProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.CreatedOn);
			}
			else if(e.Property == ProjectVersion.DueDateProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.DueDate);
			}
			else if(e.Property == ProjectVersion.DescriptionProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Description);
			}
			else if(e.Property == ProjectVersion.StatusProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Status);
			}
			else if(e.Property == ProjectVersion.ProjectProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Project);
			}
		}

		private static Size MeasureOptionalContent(NamedRedmineObject data, SubItemMeasureEventArgs measureEventArgs)
		{
			string text;
			if(data == null)
			{
				text = Resources.StrsUnassigned.SurroundWith('<', '>');
			}
			else
			{
				text = data.Name;
			}
			return measureEventArgs.MeasureText(text);
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Id:
					return measureEventArgs.MeasureText(DataContext.Id.ToString(CultureInfo.InvariantCulture));
				case ColumnId.Name:
					return measureEventArgs.MeasureText(DataContext.Name);
				case ColumnId.Description:
					return measureEventArgs.MeasureText(DataContext.Description);
				case ColumnId.Project:
					return MeasureOptionalContent(DataContext.Project, measureEventArgs);
				case ColumnId.CreatedOn:
					return VersionCreatedOnColumn.OnMeasureSubItem(measureEventArgs, DataContext.CreatedOn);
				case ColumnId.UpdatedOn:
					return VersionUpdatedOnColumn.OnMeasureSubItem(measureEventArgs, DataContext.UpdatedOn);
				case ColumnId.Status:
					return measureEventArgs.MeasureText(DataContext.Status.ToString());
				case ColumnId.DueDate:
					if(DataContext.DueDate.HasValue)
					{
						return VersionDueDateColumn.OnMeasureSubItem(measureEventArgs, DataContext.DueDate.Value);
					}
					else
					{
						return measureEventArgs.MeasureText(Resources.StrsUnassigned.SurroundWith('<', '>'));
					}
			}
			return base.MeasureSubItem(measureEventArgs);
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Id:
					paintEventArgs.PaintText(DataContext.Id.ToString(CultureInfo.InvariantCulture));
					break;
				case ColumnId.Name:
					paintEventArgs.PaintText(DataContext.Name);
					break;
				case ColumnId.Description:
					paintEventArgs.PaintText(DataContext.Description);
					break;
				case ColumnId.Project:
					RedmineGuiUtility.PaintOptionalContent(DataContext.Project, paintEventArgs);
					break;
				case ColumnId.Status:
					paintEventArgs.PaintText(DataContext.Status.ToString());
					break;
				case ColumnId.CreatedOn:
					VersionCreatedOnColumn.OnPaintSubItem(paintEventArgs, DataContext.CreatedOn);
					break;
				case ColumnId.UpdatedOn:
					VersionUpdatedOnColumn.OnPaintSubItem(paintEventArgs, DataContext.UpdatedOn);
					break;
				case ColumnId.DueDate:
					RedmineGuiUtility.PaintOptionalContent(DataContext.DueDate, paintEventArgs);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new VersionMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
