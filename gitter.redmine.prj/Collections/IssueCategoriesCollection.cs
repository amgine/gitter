namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Globalization;
	using System.Xml;

	public sealed class IssueCategoriesCollection : NamedRedmineObjectsCache<IssueCategory>
	{
		internal IssueCategoriesCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override IssueCategory Create(int id, string name)
		{
			return new IssueCategory(Context, id, name);
		}

		protected override IssueCategory Create(XmlNode node)
		{
			return new IssueCategory(Context, node);
		}

		public LinkedList<IssueCategory> Fetch(Project project)
		{
			if(project == null) throw new ArgumentNullException("project");

			return Fetch(project.Id);
		}

		public LinkedList<IssueCategory> Fetch(int projectId)
		{
			return Fetch(projectId.ToString(CultureInfo.InvariantCulture));
		}

		public LinkedList<IssueCategory> Fetch(string projectId)
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				"projects/{0}/issue_categories.xml", projectId);
			return FetchItemsFromSinglePage(url);
		}
	}
}
