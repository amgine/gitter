namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class ProjectsCollection : NamedRedmineObjectsCache<Project>
	{
		internal ProjectsCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override Project Create(int id, string name)
		{
			return new Project(Context, id, name);
		}

		protected override Project Create(XmlNode node)
		{
			return new Project(Context, node);
		}

		public LinkedList<Project> Fetch()
		{
			const string url = "projects.xml";
			return FetchItemsFromAllPages(url);
		}
	}
}
