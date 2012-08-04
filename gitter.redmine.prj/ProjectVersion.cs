namespace gitter.Redmine
{
	using System;
	using System.Globalization;
	using System.Xml;

	public sealed class ProjectVersion : NamedRedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty<Project> ProjectProperty =
			new RedmineObjectProperty<Project>("project", "Project");
		public static readonly RedmineObjectProperty<string> DescriptionProperty =
			new RedmineObjectProperty<string>("description", "Description");
		public static readonly RedmineObjectProperty<VersionStatus> StatusProperty =
			new RedmineObjectProperty<VersionStatus>("status", "Status");
		public static readonly RedmineObjectProperty<DateTime?> DueDateProperty =
			new RedmineObjectProperty<DateTime?>("due_date", "DueDate");
		public static readonly RedmineObjectProperty<DateTime> CreatedOnProperty =
			new RedmineObjectProperty<DateTime>("created_on", "CreatedOn");
		public static readonly RedmineObjectProperty<DateTime> UpdatedOnProperty =
			new RedmineObjectProperty<DateTime>("updated_on", "UpdatedOn");

		#endregion

		#region Data

		private Project _project;
		private string _description;
		private VersionStatus _status;
		private DateTime? _dueDate;
		private DateTime _createdOn;
		private DateTime _updatedOn;

		#endregion

		#region .ctor

		internal ProjectVersion(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal ProjectVersion(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], context.Projects.Lookup);
			_description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
			_status			= RedmineUtility.LoadVersionStatus(node[StatusProperty.XmlNodeName]);
			_dueDate		= RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);
			_createdOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			_updatedOn		= RedmineUtility.LoadDateForSure(node[UpdatedOnProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			Project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], Context.Projects.Lookup);
			Description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
			Status		= RedmineUtility.LoadVersionStatus(node[StatusProperty.XmlNodeName]);
			DueDate		= RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);
			CreatedOn	= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			UpdatedOn	= RedmineUtility.LoadDateForSure(node[UpdatedOnProperty.XmlNodeName]);
		}

		public override void Update()
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"versions/{0}.xml", Id);
			Context.ProjectVersions.FetchSingleItem(url);
		}

		#endregion

		#region Properties

		public Project Project
		{
			get { return _project; }
			private set { UpdatePropertyValue(ref _project, value, ProjectProperty); }
		}

		public string Description
		{
			get { return _description; }
			private set { UpdatePropertyValue(ref _description, value, DescriptionProperty); }
		}

		public VersionStatus Status
		{
			get { return _status; }
			private set { UpdatePropertyValue(ref _status, value, StatusProperty); }
		}

		public DateTime? DueDate
		{
			get { return _dueDate; }
			private set { UpdatePropertyValue(ref _dueDate, value, DueDateProperty); }
		}

		public DateTime CreatedOn
		{
			get { return _createdOn; }
			private set { UpdatePropertyValue(ref _createdOn, value, CreatedOnProperty); }
		}

		public DateTime UpdatedOn
		{
			get { return _updatedOn; }
			private set { UpdatePropertyValue(ref _updatedOn, value, UpdatedOnProperty); }
		}

		#endregion
	}
}
