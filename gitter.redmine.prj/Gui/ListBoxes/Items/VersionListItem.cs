namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Globalization;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class VersionListItem : CustomListBoxItem<ProjectVersion>
	{
		#region Comparers

		public static int CompareById(VersionListItem item1, VersionListItem item2)
		{
			var data1 = item1.Data.Id;
			var data2 = item2.Data.Id;
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
			var data1 = item1.Data.Status;
			var data2 = item2.Data.Status;
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
			var data1 = item1.Data.CreatedOn;
			var data2 = item2.Data.CreatedOn;
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
			var data1 = item1.Data.UpdatedOn;
			var data2 = item2.Data.UpdatedOn;
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
			var data1 = item1.Data.DueDate;
			var data2 = item2.Data.DueDate;
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
			var data1 = item1.Data.Name;
			var data2 = item2.Data.Name;
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
			var data1 = item1.Data.Description;
			var data2 = item2.Data.Description;
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
			if(version == null) throw new ArgumentNullException("version");
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Data.PropertyChanged += OnVersionPropertyChanged;
		}

		protected override void OnListBoxDetached()
		{
			Data.PropertyChanged -= OnVersionPropertyChanged;
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

		private static void PaintOptionalContent(NamedRedmineObject data, SubItemPaintEventArgs paintEventArgs)
		{
			string text;
			Brush brush;
			if(data == null)
			{
				text = Resources.StrsUnassigned.SurroundWith('<', '>');
				brush = SystemBrushes.GrayText;
			}
			else
			{
				text = data.Name;
				brush = paintEventArgs.Brush;
			}
			paintEventArgs.PaintText(text, brush);
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
				case ColumnId.Project:
					return MeasureOptionalContent(Data.Project, measureEventArgs);
				case ColumnId.CreatedOn:
					return measureEventArgs.MeasureText(Data.CreatedOn.ToString());
				case ColumnId.UpdatedOn:
					return measureEventArgs.MeasureText(Data.UpdatedOn.ToString());
				case ColumnId.Status:
					return measureEventArgs.MeasureText(Data.Status.ToString());
				case ColumnId.DueDate:
					if(Data.DueDate.HasValue)
					{
						return measureEventArgs.MeasureText(Data.DueDate.Value.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern));
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
					paintEventArgs.PaintText(Data.Id.ToString());
					break;
				case ColumnId.Name:
					paintEventArgs.PaintText(Data.Name);
					break;
				case ColumnId.Description:
					paintEventArgs.PaintText(Data.Description);
					break;
				case ColumnId.Project:
					PaintOptionalContent(Data.Project, paintEventArgs);
					break;
				case ColumnId.Status:
					paintEventArgs.PaintText(Data.Status.ToString());
					break;
				case ColumnId.CreatedOn:
					paintEventArgs.PaintText(Data.CreatedOn.ToString());
					break;
				case ColumnId.UpdatedOn:
					paintEventArgs.PaintText(Data.UpdatedOn.ToString());
					break;
				case ColumnId.DueDate:
					if(Data.DueDate.HasValue)
					{
						paintEventArgs.PaintText(Data.DueDate.Value.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern));
					}
					else
					{
						paintEventArgs.PaintText(Resources.StrsUnassigned.SurroundWith('<', '>'), SystemBrushes.GrayText);
					}
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new VersionMenu(Data);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
