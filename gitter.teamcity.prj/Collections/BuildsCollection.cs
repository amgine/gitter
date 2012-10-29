namespace gitter.TeamCity
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public sealed class BuildsCollection : TeamCityObjectsCache<Build>
	{
		const string QUERY = @"builds/?locator=";

		internal BuildsCollection(TeamCityServiceContext context)
			: base(context)
		{
		}

		public Build[] Query(BuildLocator locator)
		{
			Verify.Argument.IsNotNull(locator, "locator");

			var xml = Context.GetXml(QUERY + locator.ToString());
			var root = xml["builds"];
			var result = new Build[TeamCityUtility.LoadInt(root.Attributes["count"])];
			int id = 0;
			foreach(XmlElement node in root.ChildNodes)
			{
				result[id++] = Lookup(node);
			}
			return result;
		}

		public void UpdateCache(BuildLocator locator)
		{
			Verify.Argument.IsNotNull(locator, "locator");

			var xml = Context.GetXml(QUERY + locator.ToString());
			var root = xml["builds"];
			foreach(XmlElement node in root.ChildNodes)
			{
				Lookup(node);
			}
		}

		protected override Build Create(string id)
		{
			return new Build(Context, id);
		}

		protected override Build Create(XmlNode node)
		{
			return new Build(Context, node);
		}
	}
}
