namespace gitter.TeamCity
{
	using System;
	using System.Xml;

	public abstract class NamedTeamCityObject : TeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<string> NameProperty =
			new TeamCityObjectProperty<string>("name", "Name");

		#endregion

		#region Data

		private string _name;

		#endregion

		#region .ctor

		protected NamedTeamCityObject(TeamCityServiceContext context, string id)
			: base(context, id)
		{
		}

		protected NamedTeamCityObject(TeamCityServiceContext context, string id, string name)
			: base(context, id)
		{
			_name = name;
		}

		protected NamedTeamCityObject(TeamCityServiceContext context, XmlNode node)
			: base(context, node)
		{
			_name = TeamCityUtility.LoadString(node.Attributes[NameProperty.XmlNodeName]);
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
			Name = TeamCityUtility.LoadString(node.Attributes[NameProperty.XmlNodeName]);
		}

		public override string ToString()
		{
			return _name;
		}

		#endregion
	}
}
