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

using gitter.Framework.Controls;

public sealed class IssueCustomFieldColumn : CustomListBoxColumn
{
	private readonly CustomField _field;

	public IssueCustomFieldColumn(CustomField field)
		: base((int)ColumnId.CustomFieldOffset + field.Id, field.Name, false)
	{
		_field = field;
		Width = 100;
	}

	public override string IdentificationString => Name;

	private int Compare(IssueListItem item1, IssueListItem item2)
	{
		var data1 = item1.DataContext.CustomFields[_field];
		var data2 = item2.DataContext.CustomFields[_field];
		if(data1 == data2) return 0;
		if(data1 == null) return 1;
		else if(data2 == null) return -1;
		return string.Compare(data1, data2);
	}

	private int Compare(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		var i1 = item1 as IssueListItem;
		if(i1 == null) return 0;
		var i2 = item2 as IssueListItem;
		if(i2 == null) return 0;
		return Compare(i1, i2);
	}

	protected override Comparison<CustomListBoxItem> SortComparison => Compare;
}
