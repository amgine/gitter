namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class News : RedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty ProjectProperty =
			new RedmineObjectProperty("project", "Project");
		public static readonly RedmineObjectProperty AuthorProperty =
			new RedmineObjectProperty("author", "Author");
		public static readonly RedmineObjectProperty TitleProperty =
			new RedmineObjectProperty("title", "Title");
		public static readonly RedmineObjectProperty SummaryProperty =
			new RedmineObjectProperty("summary", "Summary");
		public static readonly RedmineObjectProperty DescriptionProperty =
			new RedmineObjectProperty("description", "Description");
		public static readonly RedmineObjectProperty CreatedOnProperty =
			new RedmineObjectProperty("created_on", "CreatedOn");

		#endregion

		#region Data

		private Project _project;
		private User _author;
		private string _title;
		private string _description;
		private string _summary;
		private DateTime _createdOn;

		#endregion

		#region .ctor

		internal News(RedmineServiceContext context, int id)
			: base(context, id)
		{
		}

		internal News(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], context.Projects.Lookup);
			_author			= RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName], context.Users.Lookup);
			_title			= RedmineUtility.LoadString(node[TitleProperty.XmlNodeName]);
			_description	= RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
			_summary		= RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
			_createdOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			Project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], Context.Projects.Lookup);
			Author		= RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName], Context.Users.Lookup);
			Title		= RedmineUtility.LoadString(node[TitleProperty.XmlNodeName]);
			Description	= RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
			Summary		= RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
			CreatedOn	= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
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

		public string Title
		{
			get { return _title; }
			private set
			{
				if(_title != value)
				{
					_title = value;
					OnPropertyChanged(TitleProperty);
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

		public string Summary
		{
			get { return _summary; }
			private set
			{
				if(_summary != value)
				{
					_summary = value;
					OnPropertyChanged(SummaryProperty);
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

		#endregion

		public override string ToString()
		{
			return Title;
		}
	}
}
