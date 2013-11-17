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

namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Linq;
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
		{
			return new News(Context, id);
		}

		protected override News Create(XmlNode node)
		{
			return new News(Context, node);
		}

		public LinkedList<News> Fetch(Project project)
		{
			Verify.Argument.IsNotNull(project, "project");

			return Fetch(project.Id);
		}

		public LinkedList<News> Fetch(int projectId)
		{
			return Fetch(projectId.ToString(CultureInfo.InvariantCulture));
		}

		public LinkedList<News> Fetch(string projectId)
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"projects/{0}/news.xml", projectId);
			return FetchItemsFromAllPages(url);
		}

		public Task<LinkedList<News>> FetchAsync(string projectId, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"projects/{0}/news.xml", projectId);
			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsFetchingNews.AddEllipsis()));
			}
			return FetchItemsFromAllPagesAsync(url, cancellationToken);
		}
	}
}
