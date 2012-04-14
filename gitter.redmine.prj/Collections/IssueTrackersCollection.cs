namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class IssueTrackersCollection : NamedRedmineObjectsCache<IssueTracker>
	{
		internal IssueTrackersCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override IssueTracker Create(int id, string name)
		{
			return new IssueTracker(Context, id, name);
		}

		protected override IssueTracker Create(XmlNode node)
		{
			return new IssueTracker(Context, node);
		}

		public LinkedList<IssueTracker> Fetch()
		{
			const string url = "trackers.xml";
			return FetchItemsFromSinglePage(url);
		}
	}
}
