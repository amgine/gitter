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

namespace gitter.GitLab.Gui;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using gitter.GitLab.Api;

using Resources = gitter.GitLab.Properties.Resources;

sealed class IssueListItem : CustomListBoxItem<Issue>
{
	#region Comparers

	private static int CompareString(string data1, string data2)
	{
		if(data1 == data2) return 0;
		if(data1 == null) return 1;
		else if(data2 == null) return -1;
		return string.Compare(data1, data2);
	}

	public static int CompareById(IssueListItem item1, IssueListItem item2)
	{
		var data1 = item1.DataContext.Id;
		var data2 = item2.DataContext.Id;
		return data1.CompareTo(data2);
	}

	public static int CompareById(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareById(i1, i2);
	}

	public static int CompareByTitle(IssueListItem item1, IssueListItem item2)
		=> CompareString(item1.DataContext.Title, item2.DataContext.Title);

	public static int CompareByTitle(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareByTitle(i1, i2);
	}

	public static int CompareByAssignee(IssueListItem item1, IssueListItem item2)
		=> CompareString(item1.DataContext.Assignee?.Name, item2.DataContext.Assignee?.Name);

	public static int CompareByAssignee(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareByAssignee(i1, i2);
	}

	public static int CompareByAuthor(IssueListItem item1, IssueListItem item2)
		=> CompareString(item1.DataContext.Author?.Name, item2.DataContext.Author?.Name);

	public static int CompareByAuthor(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareByAuthor(i1, i2);
	}

	public static int CompareByClosedBy(IssueListItem item1, IssueListItem item2)
		=> CompareString(item1.DataContext.ClosedBy?.Name, item2.DataContext.ClosedBy?.Name);

	public static int CompareByClosedBy(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareByClosedBy(i1, i2);
	}

	public static int CompareByMilestone(IssueListItem item1, IssueListItem item2)
		=> CompareString(item1.DataContext.Milestone?.Title, item2.DataContext.Milestone?.Title);

	public static int CompareByMilestone(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareByMilestone(i1, i2);
	}

	public static int CompareByCreatedAt(IssueListItem item1, IssueListItem item2)
	{
		var data1 = item1.DataContext.CreatedAt;
		var data2 = item2.DataContext.CreatedAt;
		return data1.CompareTo(data2);
	}

	public static int CompareByCreatedAt(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareByCreatedAt(i1, i2);
	}

	public static int CompareByUpdatedAt(IssueListItem item1, IssueListItem item2)
	{
		var data1 = item1.DataContext.UpdatedAt;
		var data2 = item2.DataContext.UpdatedAt;
		return data1.CompareTo(data2);
	}

	public static int CompareByUpdatedAt(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1 is not IssueListItem i1) return 0;
		if(item2 is not IssueListItem i2) return 0;
		return CompareByUpdatedAt(i1, i2);
	}

	#endregion

	public IssueListItem(Issue issue)
		: base(issue)
	{
		Verify.Argument.IsNotNull(issue);
	}

	protected override void OnListBoxAttached()
	{
		base.OnListBoxAttached();
		//DataContext.PropertyChanged += OnIssuePropertyChanged;
	}

	protected override void OnListBoxDetached()
	{
		//DataContext.PropertyChanged -= OnIssuePropertyChanged;
		base.OnListBoxDetached();
	}

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
			case ColumnId.Title:
				return measureEventArgs.MeasureText(DataContext.Title);
			default:
				return base.OnMeasureSubItem(measureEventArgs);
		}
	}

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
			case ColumnId.Title:
				paintEventArgs.PaintText(DataContext.Title);
				break;
			default:
				base.OnPaintSubItem(paintEventArgs);
				break;
		}
	}

	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		var menu = new HyperlinkContextMenu(DataContext.WebUrl);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
