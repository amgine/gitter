namespace gitter.TeamCity
{
	using System;
	using System.Xml;

	public sealed class Build : TeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<BuildStatus> StatusProperty =
			new TeamCityObjectProperty<BuildStatus>("status", "Status");
		public static readonly TeamCityObjectProperty<BuildType> BuildTypeProperty =
			new TeamCityObjectProperty<BuildType>("buildTypeId", "BuildType");
		public static readonly TeamCityObjectProperty<string> NumberProperty =
			new TeamCityObjectProperty<string>("number", "Number");
		public static readonly TeamCityObjectProperty<DateTime> StartDateProperty =
			new TeamCityObjectProperty<DateTime>("startDate", "StartDate");

		#endregion

		#region Data

		private BuildType _buildtype;
		private BuildStatus _status;
		private string _number;
		private DateTime _startDate;

		#endregion

		#region .ctor

		internal Build(TeamCityServiceContext context, string id)
			: base(context, id)
		{
		}

		internal Build(TeamCityServiceContext context, XmlNode node)
			: base(context, node)
		{
			_status		= TeamCityUtility.LoadBuildStatus(node.Attributes[StatusProperty.XmlNodeName]);
			_number		= TeamCityUtility.LoadString(node.Attributes[NumberProperty.XmlNodeName]);
			_startDate	= TeamCityUtility.LoadDateForSure(node.Attributes[StartDateProperty.XmlNodeName]);
			_buildtype	= Context.BuildTypes.Lookup(node.Attributes[BuildTypeProperty.XmlNodeName].InnerText);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			Status		= TeamCityUtility.LoadBuildStatus(node.Attributes[StatusProperty.XmlNodeName]);
			Number		= TeamCityUtility.LoadString(node.Attributes[NumberProperty.XmlNodeName]);
			StartDate	= TeamCityUtility.LoadDateForSure(node.Attributes[StartDateProperty.XmlNodeName]);
			BuildType	= Context.BuildTypes.Lookup(node.Attributes[BuildTypeProperty.XmlNodeName].InnerText);
		}

		public BuildLocator CreateLocator()
		{
			return new BuildLocator() { Id = Id };
		}

		private string ReadSingleField(string fieldName)
		{
			return Context.GetPlainText("builds/" + "id:" + Id + "/" + fieldName);
		}

		#endregion

		#region Properties

		public BuildStatus Status
		{
			get { return _status; }
			private set { UpdatePropertyValue(ref _status, value, StatusProperty); }
		}

		public string Number
		{
			get { return _number; }
			private set { UpdatePropertyValue(ref _number, value, NumberProperty); }
		}

		public DateTime StartDate
		{
			get { return _startDate; }
			private set { UpdatePropertyValue(ref _startDate, value, StartDateProperty); }
		}

		public BuildType BuildType
		{
			get { return _buildtype; }
			private set { UpdatePropertyValue(ref _buildtype, value, BuildTypeProperty); }
		}

		#endregion
	}
}
