namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class IssuePrioritiesCollection : NamedRedmineObjectsCache<IssuePriority>
	{
		internal IssuePrioritiesCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override IssuePriority Create(int id, string name)
		{
			return new IssuePriority(Context, id, name);
		}

		protected override IssuePriority Create(XmlNode node)
		{
			return new IssuePriority(Context, node);
		}
	}
}
