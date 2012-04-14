namespace gitter.Redmine
{
	public sealed class RedmineObjectProperty
	{
		private readonly string _xmlNodeName;
		private string _name;

		public RedmineObjectProperty(string xmlNodeName, string name)
		{
			_xmlNodeName = xmlNodeName;
			_name = name;
		}

		public string XmlNodeName
		{
			get { return _xmlNodeName; }
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
