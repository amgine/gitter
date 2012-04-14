namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Globalization;
	using System.Xml;

	public class AttachmentsCollection : RedmineObjectsCache<Attachment>
	{
		internal AttachmentsCollection(RedmineServiceContext context)
			: base(context)
		{
		}

		protected override Attachment Create(int id)
		{
			return new Attachment(Context, id);
		}

		protected override Attachment Create(XmlNode node)
		{
			return new Attachment(Context, node);
		}
	}
}
