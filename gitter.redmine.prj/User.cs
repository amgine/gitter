#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Redmine;

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Xml;

public sealed class User : NamedRedmineObject
{
	#region Static

	public static RedmineObjectProperty<string> LoginProperty =
		new RedmineObjectProperty<string>("login", "Login");
	public static RedmineObjectProperty<string> FirstNameProperty =
		new RedmineObjectProperty<string>("firstname", "FirstName");
	public static RedmineObjectProperty<string> LastNameProperty =
		new RedmineObjectProperty<string>("lastname", "LastName");
	public static RedmineObjectProperty<string> MailProperty =
		new RedmineObjectProperty<string>("mail", "Mail");
	public static RedmineObjectProperty<DateTime> CreatedOnProperty =
		new RedmineObjectProperty<DateTime>("created_on", "CreatedOn");
	public static RedmineObjectProperty<DateTime?> LastLoginOnProperty =
		new RedmineObjectProperty<DateTime?>("last_login_on", "LastLoginOn");

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
		Verify.Argument.IsNotNull(node);

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
		private set { UpdatePropertyValue(ref _login, value, LoginProperty); }
	}

	public string FirstName
	{
		get { return _firstName; }
		private set { UpdatePropertyValue(ref _firstName, value, FirstNameProperty); }
	}

	public string LastName
	{
		get { return _lastName; }
		private set { UpdatePropertyValue(ref _lastName, value, LastNameProperty); }
	}

	public string Mail
	{
		get { return _mail; }
		private set { UpdatePropertyValue(ref _mail, value, MailProperty); }
	}

	public DateTime CreatedOn
	{
		get { return _createdOn; }
		private set { UpdatePropertyValue(ref _createdOn, value, CreatedOnProperty); }
	}

	public DateTime? LastLoginOn
	{
		get { return _lastLoginOn; }
		private set { UpdatePropertyValue(ref _lastLoginOn, value, LastLoginOnProperty); }
	}

	#endregion
}
