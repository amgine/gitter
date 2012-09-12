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
