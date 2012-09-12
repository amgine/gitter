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
