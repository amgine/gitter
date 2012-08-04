namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public abstract class NamedRedmineObject : RedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty<string> NameProperty =
			new RedmineObjectProperty<string>("name", "Name");

		#endregion

		#region Data

		private string _name;

		#endregion

		#region .ctor

		protected NamedRedmineObject(RedmineServiceContext context, int id, string name)
			: base(context, id)
		{
			_name = name;
		}

		protected NamedRedmineObject(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_name = RedmineUtility.LoadString(node[NameProperty.XmlNodeName]);
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
			internal set { UpdatePropertyValue(ref _name, value, NameProperty); }
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);
			Name = RedmineUtility.LoadString(node[NameProperty.XmlNodeName]);
		}

		public override string ToString()
		{
			return _name;
		}

		#endregion
	}
}
