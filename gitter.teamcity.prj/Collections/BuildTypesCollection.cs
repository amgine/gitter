namespace gitter.TeamCity
{
	using System;
	using System.Xml;

	public sealed class BuildTypesCollection : NamedTeamCityObjectsCache<BuildType>
	{
		internal BuildTypesCollection(TeamCityServiceContext context)
			: base(context)
		{
		}

		protected override BuildType Create(string id, string name)
		{
			return new BuildType(Context, id, name);
		}

		protected override BuildType Create(string id)
		{
			return new BuildType(Context, id);
		}

		protected override BuildType Create(XmlNode node)
		{
			return new BuildType(Context, node);
		}

		public void UpdateCache()
		{
			var xml = Context.GetXml("buildTypes");
			foreach(XmlElement node in xml["buildTypes"])
			{
				Lookup(node);
			}
		}

		public void UpdateCache(ProjectLocator projectLocator)
		{
			Verify.Argument.IsNotNull(projectLocator, "projectLocator");
			var pl = projectLocator.ToString();
			Verify.Argument.IsNeitherNullNorWhitespace(pl, "projectLocator");

			var xml = Context.GetXml("projects/" + pl + "/buildTypes");
			foreach(XmlElement node in xml["buildTypes"])
			{
				Lookup(node);
			}
		}
	}
}
