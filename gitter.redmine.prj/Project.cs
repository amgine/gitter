namespace gitter.Redmine
{
	using System;
	using System.Globalization;
	using System.Xml;

	public sealed class Project : NamedRedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty IdentifierProperty =
			new RedmineObjectProperty("identifier", "Identifier");
		public static readonly RedmineObjectProperty DescriptionProperty =
			new RedmineObjectProperty("description", "Description");
		public static readonly RedmineObjectProperty ParentProperty =
			new RedmineObjectProperty("parent", "Parent");
		public static readonly RedmineObjectProperty CreatedOnProperty =
			new RedmineObjectProperty("created_on", "CreatedOn");
		public static readonly RedmineObjectProperty UpdatedOnProperty =
			new RedmineObjectProperty("updated_on", "UpdatedOn");

		#endregion

		#region Data

		private Project _parent;
		private string _identifier;
		private string _description;
		private DateTime _createdOn;
		private DateTime _updatedOn;

		#endregion

		#region .ctor

		internal Project(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal Project(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_identifier		= RedmineUtility.LoadString(node[IdentifierProperty.XmlNodeName]);
			_description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
			_parent			= RedmineUtility.LoadNamedObject(node[ParentProperty.XmlNodeName], context.Projects.Lookup);
			_createdOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			_updatedOn		= RedmineUtility.LoadDateForSure(node[UpdatedOnProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			Identifier		= RedmineUtility.LoadString(node[IdentifierProperty.XmlNodeName]);
			Description		= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
			Parent			= RedmineUtility.LoadNamedObject(node[ParentProperty.XmlNodeName], Context.Projects.Lookup);
			CreatedOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			UpdatedOn		= RedmineUtility.LoadDateForSure(node[UpdatedOnProperty.XmlNodeName]);
		}

		public override void Update()
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"projects/{0}.xml", Id);
			Context.Projects.FetchSingleItem(url);
		}

		#endregion

		#region Properties

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

		public string Identifier
		{
			get { return _identifier; }
			private set
			{
				if(_identifier != value)
				{
					_identifier = value;
					OnPropertyChanged(IdentifierProperty);
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
					if(_createdOn != value)
					{
						_createdOn = value;
						OnPropertyChanged(CreatedOnProperty);
					}
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

		public Project Parent
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

		#endregion
	}
}
