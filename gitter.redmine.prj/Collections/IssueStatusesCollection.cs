namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class IssueStatusesCollection : NamedRedmineObjectsCache<IssueStatus>
	{
		internal IssueStatusesCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override IssueStatus Create(int id, string name)
		{
			return new IssueStatus(Context, id, name);
		}

		protected override IssueStatus Create(XmlNode node)
		{
			return new IssueStatus(Context, node);
		}

		public LinkedList<IssueStatus> Fetch()
		{
			const string url = "issue_statuses.xml";
			return FetchItemsFromSinglePage(url);
		}
	}
}
