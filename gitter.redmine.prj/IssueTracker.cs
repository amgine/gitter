namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class IssueTracker : NamedRedmineObject
	{
		#region .ctor

		internal IssueTracker(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal IssueTracker(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
		}

		#endregion
	}
}
