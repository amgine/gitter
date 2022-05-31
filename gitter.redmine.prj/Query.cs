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

public sealed class Query : NamedRedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<bool> IsPublicProperty  = new("is_public",  "IsPublic");
	public static readonly RedmineObjectProperty<int>  ProjectIdProperty = new("project_id", "ProjectId");

	#endregion

	#region Data

	private int _projectId;
	private bool _isPublic;

	#endregion

	#region .ctor

	internal Query(RedmineServiceContext context, int id, string name)
		: base(context, id, name)
	{
	}

	internal Query(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_projectId	= RedmineUtility.LoadInt(node[ProjectIdProperty.XmlNodeName]);
		_isPublic	= RedmineUtility.LoadBoolean(node[IsPublicProperty.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);

		ProjectId	= RedmineUtility.LoadInt(node[ProjectIdProperty.XmlNodeName]);
		IsPublic	= RedmineUtility.LoadBoolean(node[IsPublicProperty.XmlNodeName]);
	}

	#endregion

	#region Properties

	public int ProjectId
	{
		get { return _projectId; }
		private set { UpdatePropertyValue(ref _projectId, value, ProjectIdProperty); }
	}

	public bool IsPublic
	{
		get { return _isPublic; }
		private set { UpdatePropertyValue(ref _isPublic, value, IsPublicProperty); }
	}

	#endregion
}
