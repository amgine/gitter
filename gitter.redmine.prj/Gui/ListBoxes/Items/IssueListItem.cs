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

	public sealed class IssueListItem : CustomListBoxItem<Issue>
	{
		#region Comparers

		public static int CompareById(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Id;
			var data2 = item2.Data.Id;
			return data1.CompareTo(data2);
		}

		public static int CompareById(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareById(i1, i2);
		}

		public static int CompareByDoneRatio(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.DoneRatio;
			var data2 = item2.Data.DoneRatio;
			return data1.CompareTo(data2);
		}

		public static int CompareByDoneRatio(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByDoneRatio(i1, i2);
		}

		public static int CompareByCreatedOn(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.CreatedOn;
			var data2 = item2.Data.CreatedOn;
			return data1.CompareTo(data2);
		}

		public static int CompareByCreatedOn(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByCreatedOn(i1, i2);
		}

		public static int CompareByUpdatedOn(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.UpdatedOn;
			var data2 = item2.Data.UpdatedOn;
			return data1.CompareTo(data2);
		}

		public static int CompareByUpdatedOn(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByUpdatedOn(i1, i2);
		}

		public static int CompareByStartDate(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.StartDate;
			var data2 = item2.Data.StartDate;
			if(!data1.HasValue && !data2.HasValue) return 0;
			if(!data1.HasValue) return 1;
			else if(!data2.HasValue) return -1;
			return data1.Value.CompareTo(data2.Value);
		}

		public static int CompareByStartDate(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByStartDate(i1, i2);
		}

		public static int CompareByDueDate(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.DueDate;
			var data2 = item2.Data.DueDate;
			if(!data1.HasValue && !data2.HasValue) return 0;
			if(!data1.HasValue) return 1;
			else if(!data2.HasValue) return -1;
			return data1.Value.CompareTo(data2.Value);
		}

		public static int CompareByDueDate(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByDueDate(i1, i2);
		}

		public static int CompareByCategory(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Category;
			var data2 = item2.Data.Category;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByCategory(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByCategory(i1, i2);
		}

		public static int CompareByProject(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Project;
			var data2 = item2.Data.Project;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByProject(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByProject(i1, i2);
		}

		public static int CompareByPriority(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Priority;
			var data2 = item2.Data.Priority;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return data1.Id.CompareTo(data2.Id);
		}

		public static int CompareByPriority(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByPriority(i1, i2);
		}

		public static int CompareByStatus(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Status;
			var data2 = item2.Data.Status;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByStatus(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByStatus(i1, i2);
		}

		public static int CompareByTracker(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Tracker;
			var data2 = item2.Data.Tracker;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByTracker(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByTracker(i1, i2);
		}

		public static int CompareByAssignedTo(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.AssignedTo;
			var data2 = item2.Data.AssignedTo;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByAssignedTo(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByAssignedTo(i1, i2);
		}

		public static int CompareByAuthor(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Author;
			var data2 = item2.Data.Author;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByAuthor(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareByAuthor(i1, i2);
		}

		public static int CompareBySubject(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.Data.Subject;
			var data2 = item2.Data.Subject;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1, data2);
		}

		public static int CompareBySubject(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return CompareBySubject(i1, i2);
		}

		#endregion

		public IssueListItem(Issue issue)
			: base(issue)
		{
			if(issue == null) throw new ArgumentNullException("issue");
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Data.PropertyChanged += OnIssuePropertyChanged;
		}

		protected override void OnListBoxDetached()
		{
			Data.PropertyChanged -= OnIssuePropertyChanged;
			base.OnListBoxDetached();
		}

		private void OnIssuePropertyChanged(object sender, RedmineObjectPropertyChangedEventArgs e)
		{
			if(e.Property == Issue.IdProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Id);
			}
			else if(e.Property == Issue.SubjectProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Subject);
			}
			else if(e.Property == Issue.DescriptionProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Description);
			}
			else if(e.Property == Issue.PriorityProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Priority);
			}
			else if(e.Property == Issue.CategoryProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Category);
			}
			else if(e.Property == Issue.TrackerProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Tracker);
			}
			else if(e.Property == Issue.StatusProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Status);
			}
			else if(e.Property == Issue.DoneRatioProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.DoneRatio);
			}
			else if(e.Property == Issue.CreatedOnProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.CreatedOn);
			}
			else if(e.Property == Issue.UpdatedOnProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.UpdatedOn);
			}
			else if(e.Property == Issue.StartDateProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.StartDate);
			}
			else if(e.Property == Issue.DueDateProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.DueDate);
			}
			else if(e.Property == Issue.AuthorProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Author);
			}
			else if(e.Property == Issue.AssignedToProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.AssignedTo);
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

		private static Size MeasureOptionalContent(string data, SubItemMeasureEventArgs measureEventArgs)
		{
			string text;
			if(data == null)
			{
				text = Resources.StrsUnassigned.SurroundWith('<', '>');
			}
			else
			{
				text = data;
			}
			return measureEventArgs.MeasureText(text);
		}

		private static Size MeasureOptionalContent(DateTime? date, SubItemMeasureEventArgs measureEventArgs)
		{
			string text;
			if(!date.HasValue)
			{
				text = Resources.StrsUnassigned.SurroundWith('<', '>');
			}
			else
			{
				text = date.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
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

		private static void PaintOptionalContent(DateTime? date, SubItemPaintEventArgs paintEventArgs)
		{
			string text;
			Brush brush;
			if(!date.HasValue)
			{
				text = Resources.StrsUnassigned.SurroundWith('<', '>');
				brush = SystemBrushes.GrayText;
			}
			else
			{
				text = date.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
				brush = paintEventArgs.Brush;
			}
			paintEventArgs.PaintText(text, brush);
		}

		private static void PaintOptionalContent(string data, SubItemPaintEventArgs paintEventArgs)
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
				text = data;
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
				case ColumnId.Subject:
					return measureEventArgs.MeasureText(Data.Subject);
				case ColumnId.Author:
					return MeasureOptionalContent(Data.Author, measureEventArgs);
				case ColumnId.AssignedTo:
					return MeasureOptionalContent(Data.AssignedTo, measureEventArgs);
				case ColumnId.Tracker:
					return MeasureOptionalContent(Data.Tracker, measureEventArgs);
				case ColumnId.Priority:
					return MeasureOptionalContent(Data.Priority, measureEventArgs);
				case ColumnId.Project:
					return MeasureOptionalContent(Data.Project, measureEventArgs);
				case ColumnId.Status:
					return MeasureOptionalContent(Data.Status, measureEventArgs);
				case ColumnId.Category:
					return MeasureOptionalContent(Data.Category, measureEventArgs);
				case ColumnId.CreatedOn:
					return measureEventArgs.MeasureText(Data.CreatedOn.ToString());
				case ColumnId.UpdatedOn:
					return measureEventArgs.MeasureText(Data.UpdatedOn.ToString());
				case ColumnId.StartDate:
					return MeasureOptionalContent(Data.StartDate, measureEventArgs);
				case ColumnId.DueDate:
					return MeasureOptionalContent(Data.DueDate, measureEventArgs);
				case ColumnId.DoneRatio:
					return new Size(60, 1);
				default:
					if(measureEventArgs.SubItemId >= (int)ColumnId.CustomFieldOffset)
					{
						var cfid = measureEventArgs.SubItemId - (int)ColumnId.CustomFieldOffset;
						return MeasureOptionalContent(Data.CustomFields[cfid], measureEventArgs);
					}
					else
					{
						return base.MeasureSubItem(measureEventArgs);
					}

			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Id:
					paintEventArgs.PaintText(Data.Id.ToString());
					break;
				case ColumnId.Name:
				case ColumnId.Subject:
					paintEventArgs.PaintText(Data.Subject);
					break;
				case ColumnId.Author:
					PaintOptionalContent(Data.Author, paintEventArgs);
					break;
				case ColumnId.AssignedTo:
					PaintOptionalContent(Data.AssignedTo, paintEventArgs);
					break;
				case ColumnId.Tracker:
					PaintOptionalContent(Data.Tracker, paintEventArgs);
					break;
				case ColumnId.Priority:
					PaintOptionalContent(Data.Priority, paintEventArgs);
					break;
				case ColumnId.Project:
					PaintOptionalContent(Data.Project, paintEventArgs);
					break;
				case ColumnId.Status:
					PaintOptionalContent(Data.Status, paintEventArgs);
					break;
				case ColumnId.Category:
					PaintOptionalContent(Data.Category, paintEventArgs);
					break;
				case ColumnId.CreatedOn:
					paintEventArgs.PaintText(Data.CreatedOn.ToString());
					break;
				case ColumnId.UpdatedOn:
					paintEventArgs.PaintText(Data.UpdatedOn.ToString());
					break;
				case ColumnId.StartDate:
					PaintOptionalContent(Data.StartDate, paintEventArgs);
					break;
				case ColumnId.DueDate:
					PaintOptionalContent(Data.DueDate, paintEventArgs);
					break;
				case ColumnId.DoneRatio:
					var r = paintEventArgs.Bounds;
					r.Inflate(-4, -8);
					paintEventArgs.Graphics.FillRectangle(Brushes.LightBlue, r);
					if(Data.DoneRatio != 0)
					{
						var w = (int)(r.Width * Data.DoneRatio / 100);
						r.Width = w;
						paintEventArgs.Graphics.FillRectangle(Brushes.Blue, r);
					}
					break;
				default:
					if(paintEventArgs.SubItemId >= (int)ColumnId.CustomFieldOffset)
					{
						var cfid = paintEventArgs.SubItemId - (int)ColumnId.CustomFieldOffset;
						PaintOptionalContent(Data.CustomFields[cfid], paintEventArgs);
					}
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new IssueMenu(Data);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
