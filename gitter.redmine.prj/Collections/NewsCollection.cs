namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Globalization;
	using System.Xml;

	public sealed class NewsCollection : RedmineObjectsCache<News>
	{
		internal NewsCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override News Create(int id)
		{
			return new News(Context, id);
		}

		protected override News Create(XmlNode node)
		{
			return new News(Context, node);
		}

		public LinkedList<News> Fetch(Project project)
		{
			Verify.Argument.IsNotNull(project, "project");

			return Fetch(project.Id);
		}

		public LinkedList<News> Fetch(int projectId)
		{
			return Fetch(projectId.ToString(CultureInfo.InvariantCulture));
		}

		public LinkedList<News> Fetch(string projectId)
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"projects/{0}/news.xml", projectId);
			return FetchItemsFromAllPages(url);
		}
	}
}
