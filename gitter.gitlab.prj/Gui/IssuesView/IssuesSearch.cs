#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Globalization;

using gitter.Framework.Controls;

class IssuesSearch(CustomListBox listBox) : ListBoxSearch<IssuesSearchOptions>(listBox)
{
	private static bool TestIssue(Api.Issue issue, IssuesSearchOptions search)
	{
		Assert.IsNotNull(issue);
		Assert.IsNotNull(search);

		if(TestString(issue.Title, search)) return true;
		if(int.TryParse(search.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
		{
			if(issue.Iid == id) return true;
		}
		return false;
	}

	protected override bool TestItem(CustomListBoxItem item, IssuesSearchOptions search)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(search);

		return item is IssueListItem ili && TestIssue(ili.DataContext, search);
	}
}
