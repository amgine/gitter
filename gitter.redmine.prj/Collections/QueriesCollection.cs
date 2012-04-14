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
