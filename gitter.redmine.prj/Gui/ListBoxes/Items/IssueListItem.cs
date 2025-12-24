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

namespace gitter.Redmine.Gui;

using System;
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
		var data1 = item1.DataContext.Id;
		var data2 = item2.DataContext.Id;
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
		var data1 = item1.DataContext.DoneRatio;
		var data2 = item2.DataContext.DoneRatio;
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
		var data1 = item1.DataContext.CreatedOn;
		var data2 = item2.DataContext.CreatedOn;
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
		var data1 = item1.DataContext.UpdatedOn;
		var data2 = item2.DataContext.UpdatedOn;
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
		var data1 = item1.DataContext.StartDate;
		var data2 = item2.DataContext.StartDate;
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
		var data1 = item1.DataContext.DueDate;
		var data2 = item2.DataContext.DueDate;
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
		var data1 = item1.DataContext.Category;
		var data2 = item2.DataContext.Category;
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
		var data1 = item1.DataContext.Project;
		var data2 = item2.DataContext.Project;
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
		var data1 = item1.DataContext.Priority;
		var data2 = item2.DataContext.Priority;
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
		var data1 = item1.DataContext.Status;
		var data2 = item2.DataContext.Status;
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
		var data1 = item1.DataContext.Tracker;
		var data2 = item2.DataContext.Tracker;
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
		var data1 = item1.DataContext.AssignedTo;
		var data2 = item2.DataContext.AssignedTo;
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
		var data1 = item1.DataContext.Author;
		var data2 = item2.DataContext.Author;
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
		var data1 = item1.DataContext.Subject;
		var data2 = item2.DataContext.Subject;
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
		Verify.Argument.IsNotNull(issue);
	}

	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		DataContext.PropertyChanged += OnIssuePropertyChanged;
	}

	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DataContext.PropertyChanged -= OnIssuePropertyChanged;
		base.OnListBoxDetached(listBox);
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
			return measureEventArgs.MeasureText(text);
		}
		else
		{
			return DateColumn.OnMeasureSubItem(measureEventArgs, date.Value);
		}
	}

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Id:
				return measureEventArgs.MeasureText(DataContext.Id.ToString());
			case ColumnId.Name:
			case ColumnId.Subject:
				return measureEventArgs.MeasureText(DataContext.Subject);
			case ColumnId.Author:
				return MeasureOptionalContent(DataContext.Author, measureEventArgs);
			case ColumnId.AssignedTo:
				return MeasureOptionalContent(DataContext.AssignedTo, measureEventArgs);
			case ColumnId.Tracker:
				return MeasureOptionalContent(DataContext.Tracker, measureEventArgs);
			case ColumnId.Priority:
				return MeasureOptionalContent(DataContext.Priority, measureEventArgs);
			case ColumnId.Project:
				return MeasureOptionalContent(DataContext.Project, measureEventArgs);
			case ColumnId.Status:
				return MeasureOptionalContent(DataContext.Status, measureEventArgs);
			case ColumnId.Category:
				return MeasureOptionalContent(DataContext.Category, measureEventArgs);
			case ColumnId.CreatedOn:
				return IssueCreatedOnColumn.OnMeasureSubItem(measureEventArgs, DataContext.CreatedOn);
			case ColumnId.UpdatedOn:
				return IssueUpdatedOnColumn.OnMeasureSubItem(measureEventArgs, DataContext.UpdatedOn);
			case ColumnId.StartDate:
				return MeasureOptionalContent(DataContext.StartDate, measureEventArgs);
			case ColumnId.DueDate:
				return MeasureOptionalContent(DataContext.DueDate, measureEventArgs);
			case ColumnId.DoneRatio:
				return new Size(60, 1);
			default:
				if(measureEventArgs.SubItemId >= (int)ColumnId.CustomFieldOffset)
				{
					var cfid = measureEventArgs.SubItemId - (int)ColumnId.CustomFieldOffset;
					return MeasureOptionalContent(DataContext.CustomFields[cfid], measureEventArgs);
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
				paintEventArgs.PaintText(DataContext.Id.ToString());
				break;
			case ColumnId.Name:
			case ColumnId.Subject:
				paintEventArgs.PaintText(DataContext.Subject);
				break;
			case ColumnId.Author:
				RedmineGuiUtility.PaintOptionalContent(DataContext.Author, paintEventArgs);
				break;
			case ColumnId.AssignedTo:
				RedmineGuiUtility.PaintOptionalContent(DataContext.AssignedTo, paintEventArgs);
				break;
			case ColumnId.Tracker:
				RedmineGuiUtility.PaintOptionalContent(DataContext.Tracker, paintEventArgs);
				break;
			case ColumnId.Priority:
				RedmineGuiUtility.PaintOptionalContent(DataContext.Priority, paintEventArgs);
				break;
			case ColumnId.Project:
				RedmineGuiUtility.PaintOptionalContent(DataContext.Project, paintEventArgs);
				break;
			case ColumnId.Status:
				RedmineGuiUtility.PaintOptionalContent(DataContext.Status, paintEventArgs);
				break;
			case ColumnId.Category:
				RedmineGuiUtility.PaintOptionalContent(DataContext.Category, paintEventArgs);
				break;
			case ColumnId.CreatedOn:
				IssueCreatedOnColumn.OnPaintSubItem(paintEventArgs, DataContext.CreatedOn);
				break;
			case ColumnId.UpdatedOn:
				IssueUpdatedOnColumn.OnPaintSubItem(paintEventArgs, DataContext.UpdatedOn);
				break;
			case ColumnId.StartDate:
				RedmineGuiUtility.PaintOptionalContent(DataContext.StartDate, paintEventArgs);
				break;
			case ColumnId.DueDate:
				RedmineGuiUtility.PaintOptionalContent(DataContext.DueDate, paintEventArgs);
				break;
			case ColumnId.DoneRatio:
				var r = paintEventArgs.Bounds;
				r.Inflate(-4, -8);
				paintEventArgs.Graphics.FillRectangle(Brushes.LightBlue, r);
				if(DataContext.DoneRatio != 0)
				{
					var w = (int)(r.Width * DataContext.DoneRatio / 100);
					r.Width = w;
					paintEventArgs.Graphics.FillRectangle(Brushes.Blue, r);
				}
				break;
			default:
				if(paintEventArgs.SubItemId >= (int)ColumnId.CustomFieldOffset)
				{
					var cfid = paintEventArgs.SubItemId - (int)ColumnId.CustomFieldOffset;
					RedmineGuiUtility.PaintOptionalContent(DataContext.CustomFields[cfid], paintEventArgs);
				}
				break;
		}
	}

	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		var menu = new IssueMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
