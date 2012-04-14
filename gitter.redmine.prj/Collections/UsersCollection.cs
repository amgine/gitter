namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class UsersCollection : NamedRedmineObjectsCache<User>
	{
		internal UsersCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override User Create(int id, string name)
		{
			return new User(Context, id, name);
		}

		protected override User Create(XmlNode node)
		{
			return new User(Context, node);
		}

		public LinkedList<User> Fetch()
		{
			const string url = "users.xml";
			return FetchItemsFromAllPages(url);
		}

		public User FetchCurrent()
		{
			const string url = "users/current.xml";
			return FetchSingleItem(url);
		}
	}
}
