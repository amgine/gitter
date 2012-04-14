namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class CustomField : NamedRedmineObject
	{
		#region .ctor

		internal CustomField(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal CustomField(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
		}

		#endregion
	}
}
