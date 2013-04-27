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
	using System.Text;
	using System.Xml;

	public sealed class QueriesCollection : NamedRedmineObjectsCache<Query>
	{
		internal QueriesCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override Query Create(int id, string name)
		{
			return new Query(Context, id, name);
		}

		protected override Query Create(XmlNode node)
		{
			return new Query(Context, node);
		}

		public LinkedList<Query> Fetch()
		{
			const string url = "queries.xml";
			return FetchItemsFromAllPages(url);
		}
	}
}
