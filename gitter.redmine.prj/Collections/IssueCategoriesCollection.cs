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

namespace gitter.Redmine;

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

public sealed class IssueCategoriesCollection : NamedRedmineObjectsCache<IssueCategory>
{
	internal IssueCategoriesCollection(RedmineServiceContext context)
		: base(context)
	{
	}

	protected override IssueCategory Create(int id, string name)
		=> new(Context, id, name);

	protected override IssueCategory Create(XmlNode node)
		=> new(Context, node);

	public Task<List<IssueCategory>> FetchAsync(Project project, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(project);

		return FetchAsync(project.Id, cancellationToken);
	}

	public Task<List<IssueCategory>> FetchAsync(int projectId, CancellationToken cancellationToken = default)
		=> FetchAsync(projectId.ToString(CultureInfo.InvariantCulture), cancellationToken);

	public Task<List<IssueCategory>> FetchAsync(string projectId, CancellationToken cancellationToken = default)
	{
		var url = string.Format(CultureInfo.InvariantCulture,
			"projects/{0}/issue_categories.xml", projectId);
		return FetchItemsFromSinglePageAsync(url, cancellationToken);
	}
}
