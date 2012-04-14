namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class IssuePriority : NamedRedmineObject
	{
		#region .ctor

		internal IssuePriority(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal IssuePriority(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
		}

		#endregion
	}
}
