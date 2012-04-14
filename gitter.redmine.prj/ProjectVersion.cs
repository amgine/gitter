namespace gitter.Redmine
{
	using System;
	using System.Globalization;
	using System.Xml;

	public sealed class ProjectVersion : NamedRedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty ProjectProperty =
			new RedmineObjectProperty("project", "Project");
		public static readonly RedmineObjectProperty DescriptionProperty =
			new RedmineObjectProperty("description", "Description");
		public static readonly RedmineObjectProperty StatusProperty =
			new RedmineObjectProperty("status", "Status");
		public static readonly RedmineObjectProperty DueDateProperty =
			new RedmineObjectProperty("due_date", "DueDate");
		public static readonly RedmineObjectProperty CreatedOnProperty =
			new RedmineObjectProperty("created_on", "CreatedOn");
		public static readonly RedmineObjectProperty UpdatedOnProperty =
			new RedmineObjectProperty("updated_on", "UpdatedOn");

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
			private set
			{
				if(_project != value)
				{
					_project = value;
					OnPropertyChanged(ProjectProperty);
				}
			}
		}

		public string Description
		{
			get { return _description; }
			private set
			{
				if(_description != value)
				{
					_description = value;
					OnPropertyChanged(DescriptionProperty);
				}
			}
		}

		public VersionStatus Status
		{
			get { return _status; }
			private set
			{
				if(_status != value)
				{
					_status = value;
					OnPropertyChanged(StatusProperty);
				}
			}
		}

		public DateTime? DueDate
		{
			get { return _dueDate; }
			private set
			{
				if(_dueDate != value)
				{
					_dueDate = value;
					OnPropertyChanged(DueDateProperty);
				}
			}
		}

		public DateTime CreatedOn
		{
			get { return _createdOn; }
			private set
			{
				if(_createdOn != value)
				{
					_createdOn = value;
					OnPropertyChanged(CreatedOnProperty);
				}
			}
		}

		public DateTime UpdatedOn
		{
			get { return _updatedOn; }
			private set
			{
				if(_updatedOn != value)
				{
					_updatedOn = value;
					OnPropertyChanged(UpdatedOnProperty);
				}
			}
		}

		#endregion
	}
}
