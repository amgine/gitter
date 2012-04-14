namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class CustomFieldsCollection : NamedRedmineObjectsCache<CustomField>
	{
		internal CustomFieldsCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override CustomField Create(int id, string name)
		{
			return new CustomField(Context, id, name);
		}

		protected override CustomField Create(XmlNode node)
		{
			return new CustomField(Context, node);
		}
	}
}
