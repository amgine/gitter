namespace gitter.TeamCity
{
	using System;
	using System.Xml;

	public sealed class ProjectsCollection : NamedTeamCityObjectsCache<Project>
	{
		internal ProjectsCollection(TeamCityServiceContext context)
			: base(context)
		{
		}

		protected override Project Create(string id, string name)
		{
			return new Project(Context, id, name);
		}

		protected override Project Create(string id)
		{
			return new Project(Context, id);
		}

		protected override Project Create(XmlNode node)
		{
			return new Project(Context, node);
		}

		public void UpdateCache()
		{
			var xml = Context.GetXml("projects");
			foreach(XmlElement node in xml["projects"])
			{
				Lookup(node);
			}
		}
	}
}
