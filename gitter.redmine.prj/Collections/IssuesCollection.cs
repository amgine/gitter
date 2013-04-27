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
	using System.Linq;
	using System.Globalization;
	using System.Xml;

	public class IssuesCollection : RedmineObjectsCache<Issue>
	{
		internal IssuesCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		public IssueCreation CreateNew()
		{
			return new IssueCreation(Context);
		}

		protected override Issue Create(int id)
		{
			return new Issue(Context, id);
		}

		protected override Issue Create(XmlNode node)
		{
			return new Issue(Context, node);
		}

		public LinkedList<Issue> FetchOpen(Project project)
		{
			Verify.Argument.IsNotNull(project, "project");

			return FetchOpen(project.Id);
		}

		public LinkedList<Issue> FetchOpen(int projectId)
		{
			return FetchOpen(projectId.ToString(CultureInfo.InvariantCulture));
		}

		public LinkedList<Issue> FetchOpen(string projectId)
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"projects/{0}/issues.xml", projectId);
			return FetchItemsFromAllPages(url);
		}
	}
}
