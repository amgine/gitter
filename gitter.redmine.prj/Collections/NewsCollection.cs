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
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml;

using gitter.Framework;

using Resources = gitter.Redmine.Properties.Resources;

public sealed class NewsCollection : RedmineObjectsCache<News>
{
	internal NewsCollection(RedmineServiceContext context)
		: base(context)
	{
	}

	protected override News Create(int id)
		=> new(Context, id);

	protected override News Create(XmlNode node)
		=> new(Context, node);

	public Task<List<News>> FetchAsync(Project project, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(project);

		return FetchAsync(project.Id, cancellationToken);
	}

	public Task<List<News>> FetchAsync(int projectId, CancellationToken cancellationToken = default)
		=> FetchAsync(projectId.ToString(CultureInfo.InvariantCulture), cancellationToken);

	public Task<List<News>> FetchAsync(string projectId, CancellationToken cancellationToken = default)
	{
		var url = string.Format(CultureInfo.InvariantCulture,
			@"projects/{0}/news.xml", projectId);
		return FetchItemsFromAllPagesAsync(url, cancellationToken);
	}

	public Task<List<News>> FetchAsync(string projectId,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		progress?.Report(OperationProgress.Indeterminate(Resources.StrsFetchingNews.AddEllipsis()));
		var url = string.Format(CultureInfo.InvariantCulture,
			@"projects/{0}/news.xml", projectId);
		return FetchItemsFromAllPagesAsync(url, cancellationToken);
	}
}
