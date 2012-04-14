namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class UserRole : NamedRedmineObject
	{
		#region .ctor

		internal UserRole(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal UserRole(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
		}

		#endregion
	}
}
