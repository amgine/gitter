namespace gitter.TeamCity
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public abstract class TeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<string> IdProperty =
			new TeamCityObjectProperty<string>("id", "Id");
		public static readonly TeamCityObjectProperty<string> WebUrlProperty =
			new TeamCityObjectProperty<string>("webUrl", "WebUrl");

		#endregion

		#region Data

		private readonly TeamCityServiceContext _context;
		private readonly string _id;
		private string _webUrl;

		#endregion

		#region Events

		public event EventHandler<TeamCityObjectPropertyChangedEventArgs> PropertyChanged;

		protected void OnPropertyChanged(TeamCityObjectProperty property)
		{
			var handler = PropertyChanged;
			if(handler != null)
			{
				handler(this, new TeamCityObjectPropertyChangedEventArgs(property));
			}
		}

		#endregion

		#region .ctor

		protected TeamCityObject(TeamCityServiceContext context, string id)
		{
			Verify.Argument.IsNotNull(context, "context");

			_context = context;
			_id = id;
		}

		protected TeamCityObject(TeamCityServiceContext context, XmlNode node)
		{
			Verify.Argument.IsNotNull(context, "context");
			Verify.Argument.IsNotNull(node, "node");

			_context	= context;
			_id			= TeamCityUtility.LoadString(node.Attributes[IdProperty.XmlNodeName]);
			_webUrl		= TeamCityUtility.LoadString(node.Attributes[WebUrlProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal virtual void Update(XmlNode node)
		{
			Verify.Argument.IsNotNull(node, "node");

			WebUrl = TeamCityUtility.LoadString(node.Attributes[WebUrlProperty.XmlNodeName]);
		}

		public virtual void Update()
		{
			throw new NotSupportedException();
		}

		public object GetValue(TeamCityObjectProperty property)
		{
			Verify.Argument.IsNotNull(property, "property");

			return GetType().GetProperty(property.Name).GetValue(this, null);
		}

		protected void UpdatePropertyValue<T>(ref T field, T value, TeamCityObjectProperty<T> property)
		{
			if(!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				OnPropertyChanged(property);
			}
		}

		#endregion

		#region Properties

		public string Id
		{
			get { return _id; }
		}

		public TeamCityServiceContext Context
		{
			get { return _context; }
		}

		public string WebUrl
		{
			get { return _webUrl; }
			private set { UpdatePropertyValue(ref _webUrl, value, WebUrlProperty); }
		}

		#endregion
	}
}
