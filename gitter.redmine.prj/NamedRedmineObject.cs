namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public abstract class NamedRedmineObject : RedmineObject
	{
		public static readonly RedmineObjectProperty NameProperty =
			new RedmineObjectProperty("name", "Name");

		private string _name;

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

		internal override void Update(XmlNode node)
		{
			base.Update(node);
			Name = RedmineUtility.LoadString(node[NameProperty.XmlNodeName]);
		}

		public string Name
		{
			get { return _name; }
			internal set
			{
				if(_name != value)
				{
					_name = value;
					OnPropertyChanged(NameProperty);
				}
			}
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
