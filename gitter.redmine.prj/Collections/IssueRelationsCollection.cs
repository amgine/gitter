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

	public class IssueRelationsCollection : RedmineObjectsCache<IssueRelation>
	{
		internal IssueRelationsCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override IssueRelation Create(int id)
		{
			return new IssueRelation(Context, id);
		}

		protected override IssueRelation Create(XmlNode node)
		{
			return new IssueRelation(Context, node);
		}

		public LinkedList<IssueRelation> Fetch(Issue issue)
		{
			Verify.Argument.IsNotNull(issue, "issue");

			return Fetch(issue.Id);
		}

		public LinkedList<IssueRelation> Fetch(int issueId)
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"issues/{0}/relations.xml", issueId);
			return FetchItemsFromSinglePage(url);
		}
	}
}
