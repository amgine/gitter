namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class UserRolesCollection : NamedRedmineObjectsCache<UserRole>
	{
		internal UserRolesCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override UserRole Create(int id, string name)
		{
			return new UserRole(Context, id, name);
		}

		protected override UserRole Create(XmlNode node)
		{
			return new UserRole(Context, node);
		}

		public LinkedList<UserRole> Fetch()
		{
			const string url = "roles.xml";
			return FetchItemsFromSinglePage(url);
		}
	}
}
