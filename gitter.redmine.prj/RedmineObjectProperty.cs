namespace gitter.Redmine
{
	using System;

	public abstract class RedmineObjectProperty
	{
		private readonly string _xmlNodeName;
		private readonly string _name;

		internal RedmineObjectProperty(string xmlNodeName, string name)
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

		public abstract Type Type { get; }

		public override string ToString()
		{
			return _name;
		}
	}

	public sealed class RedmineObjectProperty<T> : RedmineObjectProperty
	{
		internal RedmineObjectProperty(string xmlNodeName, string name)
			: base(xmlNodeName, name)
		{
		}

		public override Type Type
		{
			get { return typeof(T); }
		}

		public T GetValue(RedmineObject obj)
		{
			if(obj == null) throw new ArgumentNullException("obj");

			return (T)obj.GetType().GetProperty(Name).GetValue(obj, null);
		}
	}
}
