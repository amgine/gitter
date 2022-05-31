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
using System.Xml;

public sealed class IssueCategory : NamedRedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<Project> ProjectProperty =
		new RedmineObjectProperty<Project>("project", "Project");
	public static readonly RedmineObjectProperty<User> AssignedToProperty =
		new RedmineObjectProperty<User>("assigned_to", "AssignedTo");

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
		private set { UpdatePropertyValue(ref _project, value, ProjectProperty); }
	}

	public User AssignedTo
	{
		get { return _assignedTo; }
		private set { UpdatePropertyValue(ref _assignedTo, value, AssignedToProperty); }
	}

	#endregion
}
