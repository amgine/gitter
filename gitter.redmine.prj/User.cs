namespace gitter.Redmine
{
	using System;
	using System.Globalization;
	using System.Collections.Generic;
	using System.Xml;

	public sealed class User : NamedRedmineObject
	{
		#region Static

		public static RedmineObjectProperty LoginProperty =
			new RedmineObjectProperty("login", "Login");
		public static RedmineObjectProperty FirstNameProperty =
			new RedmineObjectProperty("firstname", "FirstName");
		public static RedmineObjectProperty LastNameProperty =
			new RedmineObjectProperty("lastname", "LastName");
		public static RedmineObjectProperty MailProperty =
			new RedmineObjectProperty("mail", "Mail");
		public static RedmineObjectProperty CreatedOnProperty =
			new RedmineObjectProperty("created_on", "CreatedOn");
		public static RedmineObjectProperty LastLoginOnProperty =
			new RedmineObjectProperty("last_login_on", "LastLoginOn");

		#endregion

		#region Data

		private readonly Dictionary<int, UserMembership> _memberships;
		private string _login;
		private string _firstName;
		private string _lastName;
		private string _mail;
		private DateTime _createdOn;
		private DateTime? _lastLoginOn;

		#endregion

		#region .ctor

		internal User(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
			_memberships = new Dictionary<int, UserMembership>();
		}

		internal User(RedmineServiceContext context, XmlNode node)
			: this(context, RedmineUtility.LoadInt(node[IdProperty.XmlNodeName]),
				RedmineUtility.LoadString(node[FirstNameProperty.XmlNodeName]) + " " +
				RedmineUtility.LoadString(node[LastNameProperty.XmlNodeName]))
		{
			_login			= RedmineUtility.LoadString(node[LoginProperty.XmlNodeName]);
			_firstName		= RedmineUtility.LoadString(node[FirstNameProperty.XmlNodeName]);
			_lastName		= RedmineUtility.LoadString(node[LastNameProperty.XmlNodeName]);
			_mail			= RedmineUtility.LoadString(node[MailProperty.XmlNodeName]);
			_createdOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			_lastLoginOn	= RedmineUtility.LoadDate(node[LastLoginOnProperty.XmlNodeName]);
			LoadMemberships(node["memberships"]);
		}

		#endregion

		internal override void Update(XmlNode node)
		{
			if(node == null) throw new ArgumentNullException("node");

			Login		= RedmineUtility.LoadString(node[LoginProperty.XmlNodeName]);
			FirstName	= RedmineUtility.LoadString(node[FirstNameProperty.XmlNodeName]);
			LastName	= RedmineUtility.LoadString(node[LastNameProperty.XmlNodeName]);
			Name		= FirstName + " " + LastName;
			Mail		= RedmineUtility.LoadString(node[MailProperty.XmlNodeName]);
			CreatedOn	= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
			LastLoginOn	= RedmineUtility.LoadDate(node[LastLoginOnProperty.XmlNodeName]);
			LoadMemberships(node["memberships"]);
		}

		public override void Update()
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"users/{0}.xml?include=memberships", Id);
			Context.Users.FetchSingleItem(url);
		}

		private void LoadMemberships(XmlNode node)
		{
			var res = _memberships;
			if(node != null)
			{
				if(node.ChildNodes.Count != 0)
				{
					foreach(XmlNode childNode in node.ChildNodes)
					{
						var project = RedmineUtility.LoadNamedObject(childNode["project"], Context.Projects.Lookup);
						var id = project.Id;
						var roles = new List<UserRole>();
						var rolesNode = childNode["roles"];
						if(rolesNode != null)
						{
							foreach(XmlNode roleNode in rolesNode.ChildNodes)
							{
								var role = RedmineUtility.LoadNamedObject(roleNode, Context.UserRoles.Lookup);
								roles.Add(role);
							}
						}
						UserMembership membership;
						if(res.TryGetValue(id, out membership))
						{
							membership = new UserMembership(this, project, roles);
							res[id] = membership;
						}
						else
						{
							membership = new UserMembership(this, project, roles);
							res.Add(id, membership);
						}
					}
				}
				else
				{
					_memberships.Clear();
				}
			}
			else
			{
				_memberships.Clear();
			}
		}

		#region Properties

		public string Login
		{
			get { return _login; }
			internal set
			{
				if(_login != value)
				{
					_login = value;
					OnPropertyChanged(LoginProperty);
				}
			}
		}

		public string FirstName
		{
			get { return _firstName; }
			internal set
			{
				if(_firstName != value)
				{
					_firstName = value;
					OnPropertyChanged(FirstNameProperty);
				}
			}
		}

		public string LastName
		{
			get { return _lastName; }
			internal set
			{
				if(_lastName != value)
				{
					_lastName = value;
					OnPropertyChanged(LastNameProperty);
				}
			}
		}

		public string Mail
		{
			get { return _mail; }
			internal set
			{
				if(_mail != value)
				{
					_mail = value;
					OnPropertyChanged(MailProperty);
				}
			}
		}

		public DateTime CreatedOn
		{
			get { return _createdOn; }
			internal set
			{
				if(_createdOn != value)
				{
					_createdOn = value;
					OnPropertyChanged(CreatedOnProperty);
				}
			}
		}

		public DateTime? LastLoginOn
		{
			get { return _lastLoginOn; }
			internal set
			{
				if(_lastLoginOn != value)
				{
					_lastLoginOn = value;
					OnPropertyChanged(LastLoginOnProperty);
				}
			}
		}

		#endregion
	}
}
