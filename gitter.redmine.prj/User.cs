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
using System.Threading.Tasks;
using System.Threading;

public sealed class User : NamedRedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<string>    LoginProperty       = new("login",         nameof(Login));
	public static readonly RedmineObjectProperty<string>    FirstNameProperty   = new("firstname",     nameof(FirstName));
	public static readonly RedmineObjectProperty<string>    LastNameProperty    = new("lastname",      nameof(LastName));
	public static readonly RedmineObjectProperty<string>    MailProperty        = new("mail",          nameof(Mail));
	public static readonly RedmineObjectProperty<DateTime>  CreatedOnProperty   = new("created_on",    nameof(CreatedOn));
	public static readonly RedmineObjectProperty<DateTime?> LastLoginOnProperty = new("last_login_on", nameof(LastLoginOn));

	#endregion

	#region Data

	private readonly Dictionary<int, UserMembership> _memberships = [];
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
		_createdOn		= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
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
		CreatedOn	= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
		LastLoginOn	= RedmineUtility.LoadDate(node[LastLoginOnProperty.XmlNodeName]);
		LoadMemberships(node["memberships"]);
	}

	public override Task UpdateAsync(CancellationToken cancellationToken = default)
	{
		var url = string.Format(CultureInfo.InvariantCulture,
			@"users/{0}.xml?include=memberships", Id);
		return Context.Users.FetchSingleItemAsync(url, cancellationToken);
	}

	private void LoadMemberships(XmlElement element)
	{
		var res = _memberships;
		if(element is not null)
		{
			if(element.ChildNodes.Count != 0)
			{
				foreach(XmlNode childNode in element.ChildNodes)
				{
					var project = RedmineUtility.LoadNamedObject(childNode["project"], Context.Projects.Lookup);
					var id = project.Id;
					var roles = new List<UserRole>();
					var rolesNode = childNode["roles"];
					if(rolesNode is not null)
					{
						foreach(XmlNode roleNode in rolesNode.ChildNodes)
						{
							var role = RedmineUtility.LoadNamedObject(roleNode, Context.UserRoles.Lookup);
							roles.Add(role);
						}
					}
					if(res.TryGetValue(id, out var membership))
					{
						res[id] = membership = new(this, project, roles);
					}
					else
					{
						res.Add(id, membership = new(this, project, roles));
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
		get => _login;
		private set => UpdatePropertyValue(ref _login, value, LoginProperty);
	}

	public string FirstName
	{
		get => _firstName;
		private set => UpdatePropertyValue(ref _firstName, value, FirstNameProperty);
	}

	public string LastName
	{
		get => _lastName;
		private set => UpdatePropertyValue(ref _lastName, value, LastNameProperty);
	}

	public string Mail
	{
		get => _mail;
		private set => UpdatePropertyValue(ref _mail, value, MailProperty);
	}

	public DateTime CreatedOn
	{
		get => _createdOn;
		private set => UpdatePropertyValue(ref _createdOn, value, CreatedOnProperty);
	}

	public DateTime? LastLoginOn
	{
		get => _lastLoginOn;
		private set => UpdatePropertyValue(ref _lastLoginOn, value, LastLoginOnProperty);
	}

	#endregion
}
