namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Globalization;
	using System.Xml;

	public sealed class ProjectVersionsCollection : NamedRedmineObjectsCache<ProjectVersion>
	{
		internal ProjectVersionsCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override ProjectVersion Create(int id, string name)
		{
			return new ProjectVersion(Context, id, name);
		}

		protected override ProjectVersion Create(XmlNode node)
		{
			return new ProjectVersion(Context, node);
		}

		public LinkedList<ProjectVersion> Fetch(Project project)
		{
			Verify.Argument.IsNotNull(project, "project");

			return Fetch(project.Id);
		}

		public LinkedList<ProjectVersion> Fetch(int projectId)
		{
			return Fetch(projectId.ToString(CultureInfo.InvariantCulture));
		}

		public LinkedList<ProjectVersion> Fetch(string projectId)
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				"projects/{0}/versions.xml", projectId);
			return FetchItemsFromSinglePage(url);
		}
	}
}
