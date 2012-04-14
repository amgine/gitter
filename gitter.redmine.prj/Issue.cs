namespace gitter.Redmine
{
	using System;
	using System.Globalization;
	using System.Collections.Generic;
	using System.Xml;

	/// <summary>Represents redmine issue object.</summary>
	public sealed class Issue : RedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty ProjectProperty =
			new RedmineObjectProperty("project", "Project");
		public static readonly RedmineObjectProperty ParentProperty =
			new RedmineObjectProperty("parent", "Parent");
		public static readonly RedmineObjectProperty TrackerProperty =
			new RedmineObjectProperty("tracker", "Tracker");
		public static readonly RedmineObjectProperty StatusProperty =
			new RedmineObjectProperty("status", "Status");
		public static readonly RedmineObjectProperty PriorityProperty =
			new RedmineObjectProperty("priority", "Priority");
		public static readonly RedmineObjectProperty AuthorProperty =
			new RedmineObjectProperty("author", "Author");
		public static readonly RedmineObjectProperty AssignedToProperty =
			new RedmineObjectProperty("assigned_to", "AssignedTo");
		public static readonly RedmineObjectProperty CategoryProperty =
			new RedmineObjectProperty("category", "Category");
		public static readonly RedmineObjectProperty FixedVersionProperty =
			new RedmineObjectProperty("fixed_version", "FixedVersion");
		public static readonly RedmineObjectProperty SubjectProperty =
			new RedmineObjectProperty("subject", "Subject");
		public static readonly RedmineObjectProperty DescriptionProperty =
			new RedmineObjectProperty("description", "Description");
		public static readonly RedmineObjectProperty StartDateProperty =
			new RedmineObjectProperty("start_date", "StartDate");
		public static readonly RedmineObjectProperty DueDateProperty =
			new RedmineObjectProperty("due_date", "DueDate");
		public static readonly RedmineObjectProperty DoneRatioProperty =
			new RedmineObjectProperty("done_ratio", "DoneRatio");
		public static readonly RedmineObjectProperty EstimatedHoursProperty =
			new RedmineObjectProperty("estimated_hours", "EstimatedHours");
		public static readonly RedmineObjectProperty CustomFieldsProperty =
			new RedmineObjectProperty("custom_fields", "CustomFields");
		public static readonly RedmineObjectProperty CreatedOnProperty =
			new RedmineObjectProperty("created_on", "CreatedOn");
		public static readonly RedmineObjectProperty UpdatedOnProperty =
			new RedmineObjectProperty("updated_on", "UpdatedOn");

		#endregion

		#region Data

		private Project _project;

		private Issue _parent;

		private IssueTracker _tracker;
		private IssueStatus _status;
		private IssuePriority _priority;

		private User _author;
		private User _assignedTo;
		private IssueCategory _category;
		private ProjectVersion _fixedVersion;

		private string _subject;
		private string _description;

		private DateTime? _startDate;
		private DateTime? _dueDate;

		private double _doneRatio;
		private double _estimatedHours;

		private CustomFields _customFields;

		private DateTime _createdOn;
		private DateTime _updatedOn;

		#endregion

		#region .ctor

		internal Issue(RedmineServiceContext context, int id)
			: base(context, id)
		{
			_customFields	= new CustomFields();
		}

		internal Issue(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_parent			= RedmineUtility.LoadObject(node[ParentProperty.XmlNodeName],				context.Issues.Lookup);

			_project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName],			context.Projects.Lookup);
			_tracker		= RedmineUtility.LoadNamedObject(node[TrackerProperty.XmlNodeName],			context.Trackers.Lookup);
			_status			= RedmineUtility.LoadNamedObject(node[StatusProperty.XmlNodeName],			context.IssueStatuses.Lookup);
			_priority		= RedmineUtility.LoadNamedObject(node[PriorityProperty.XmlNodeName],		context.IssuePriorities.Lookup);

			_author			= RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName],			context.Users.Lookup);
			_assignedTo		= RedmineUtility.LoadNamedObject(node[AssignedToProperty.XmlNodeName],		context.Users.Lookup);

			_category		= RedmineUtility.LoadNamedObject(node[CategoryProperty.XmlNodeName],		context.IssueCategories.Lookup);
			_fixedVersion	= RedmineUtility.LoadNamedObject(node[FixedVersionProperty.XmlNodeName],	context.ProjectVersions.Lookup);

			_subject		= RedmineUtility.LoadString(node[SubjectProperty.XmlNodeName]);
			_description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);

			_startDate		= RedmineUtility.LoadDate(node[StartDateProperty.XmlNodeName]);
			_dueDate		= RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);

			_doneRatio		= RedmineUtility.LoadDouble(node[DoneRatioProperty.XmlNodeName]);
			_estimatedHours	= RedmineUtility.LoadDouble(node[EstimatedHoursProperty.XmlNodeName]);

			_customFields	= RedmineUtility.LoadCustomFields(node[CustomFieldsProperty.XmlNodeName],	context.CustomFields.Lookup);

			_createdOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			_updatedOn		= RedmineUtility.LoadDateForSure(node[UpdatedOnProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			Parent			= RedmineUtility.LoadObject(node[ParentProperty.XmlNodeName],				Context.Issues.Lookup);

			Project			= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName],			Context.Projects.Lookup);
			Tracker			= RedmineUtility.LoadNamedObject(node[TrackerProperty.XmlNodeName],			Context.Trackers.Lookup);
			Status			= RedmineUtility.LoadNamedObject(node[StatusProperty.XmlNodeName],			Context.IssueStatuses.Lookup);
			Priority		= RedmineUtility.LoadNamedObject(node[PriorityProperty.XmlNodeName],		Context.IssuePriorities.Lookup);

			Author			= RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName],			Context.Users.Lookup);
			AssignedTo		= RedmineUtility.LoadNamedObject(node[AssignedToProperty.XmlNodeName],		Context.Users.Lookup);

			Category		= RedmineUtility.LoadNamedObject(node[CategoryProperty.XmlNodeName],		Context.IssueCategories.Lookup);
			FixedVersion	= RedmineUtility.LoadNamedObject(node[FixedVersionProperty.XmlNodeName],	Context.ProjectVersions.Lookup);

			Subject			= RedmineUtility.LoadString(node[SubjectProperty.XmlNodeName]);
			Description		= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);

			StartDate		= RedmineUtility.LoadDate(node[StartDateProperty.XmlNodeName]);
			DueDate			= RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);

			DoneRatio		= RedmineUtility.LoadDouble(node[DoneRatioProperty.XmlNodeName]);
			EstimatedHours	= RedmineUtility.LoadDouble(node[EstimatedHoursProperty.XmlNodeName]);

			_customFields.Update(node[CustomFieldsProperty.XmlNodeName], Context.CustomFields.Lookup);

			CreatedOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			UpdatedOn		= RedmineUtility.LoadDateForSure(node[UpdatedOnProperty.XmlNodeName]);
		}

		public override void Update()
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"issues/{0}.xml", Id);
			Context.Issues.FetchSingleItem(url);
		}

		public IssueModification Modify()
		{
			return new IssueModification(this);
		}

		#endregion

		#region Prpoerties

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

		public Issue Parent
		{
			get { return _parent; }
			private set
			{
				if(_parent != value)
				{
					_parent = value;
					OnPropertyChanged(ParentProperty);
				}
			}
		}

		public IssueTracker Tracker
		{
			get { return _tracker; }
			private set
			{
				if(_tracker != value)
				{
					_tracker = value;
					OnPropertyChanged(TrackerProperty);
				}
			}
		}

		public IssueStatus Status
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

		public IssuePriority Priority
		{
			get { return _priority; }
			private set
			{
				if(_priority != value)
				{
					_priority = value;
					OnPropertyChanged(PriorityProperty);
				}
			}
		}

		public User Author
		{
			get { return _author; }
			private set
			{
				if(_author != value)
				{
					_author = value;
					OnPropertyChanged(AuthorProperty);
				}
			}
		}

		public User AssignedTo
		{
			get { return _assignedTo; }
			private set
			{
				if(_assignedTo != value)
				{
					_assignedTo = value;
					OnPropertyChanged(AssignedToProperty);
				}
			}
		}

		public bool IsAssigned
		{
			get { return _assignedTo != null; }
		}

		public IssueCategory Category
		{
			get { return _category; }
			private set
			{
				if(_category != value)
				{
					_category = value;
					OnPropertyChanged(CategoryProperty);
				}
			}
		}

		public ProjectVersion FixedVersion
		{
			get { return _fixedVersion; }
			private set
			{
				if(_fixedVersion != value)
				{
					_fixedVersion = value;
					OnPropertyChanged(FixedVersionProperty);
				}
			}
		}

		public string Subject
		{
			get { return _subject; }
			private set
			{
				if(_subject != value)
				{
					_subject = value;
					OnPropertyChanged(SubjectProperty);
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

		public DateTime? StartDate
		{
			get { return _startDate; }
			private set
			{
				if(_startDate != value)
				{
					_startDate = value;
					OnPropertyChanged(StartDateProperty);
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

		public double DoneRatio
		{
			get { return _doneRatio; }
			private set
			{
				if(_doneRatio != value)
				{
					_doneRatio = value;
					OnPropertyChanged(DoneRatioProperty);
				}
			}
		}

		public double EstimatedHours
		{
			get { return _estimatedHours; }
			private set
			{
				if(_estimatedHours != value)
				{
					_estimatedHours = value;
					OnPropertyChanged(EstimatedHoursProperty);
				}
			}
		}

		public CustomFields CustomFields
		{
			get { return _customFields; }
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

		public override string ToString()
		{
			return _subject;
		}
	}
}
