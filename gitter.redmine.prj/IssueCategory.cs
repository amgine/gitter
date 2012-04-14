namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class IssueCategory : NamedRedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty ProjectProperty =
			new RedmineObjectProperty("project", "Project");
		public static readonly RedmineObjectProperty AssignedToProperty =
			new RedmineObjectProperty("assigned_to", "AssignedTo");

		#endregion

		#region Data

		private Project _project;
		private User _assignedTo;

		#endregion

		#region .ctor

		internal IssueCategory(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal IssueCategory(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_project	= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName],		context.Projects.Lookup);
			_assignedTo	= RedmineUtility.LoadNamedObject(node[AssignedToProperty.XmlNodeName],	context.Users.Lookup);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			Project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName],		Context.Projects.Lookup);
			AssignedTo	= RedmineUtility.LoadNamedObject(node[AssignedToProperty.XmlNodeName],	Context.Users.Lookup);
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

		#endregion
	}
}
