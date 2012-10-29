namespace gitter.TeamCity
{
	using System;

	public abstract class TeamCityObjectProperty
	{
		private readonly string _xmlNodeName;
		private readonly string _name;

		internal TeamCityObjectProperty(string xmlNodeName, string name)
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

	public sealed class TeamCityObjectProperty<T> : TeamCityObjectProperty
	{
		internal TeamCityObjectProperty(string xmlNodeName, string name)
			: base(xmlNodeName, name)
		{
		}

		public override Type Type
		{
			get { return typeof(T); }
		}

		public T GetValue(TeamCityObject obj)
		{
			Verify.Argument.IsNotNull(obj, "obj");

			return (T)obj.GetType().GetProperty(Name).GetValue(obj, null);
		}
	}
}
