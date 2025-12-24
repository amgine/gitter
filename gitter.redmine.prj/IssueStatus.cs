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

public sealed class IssueStatus : NamedRedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<bool> IsDefaultProperty = new ("is_default", nameof(IsDefault));
	public static readonly RedmineObjectProperty<bool> IsClosedProperty  = new ("is_closed",  nameof(IsClosed));

	#endregion

	#region Data

	private bool _isDefault;
	private bool _isClosed;

	#endregion

	#region .ctor

	internal IssueStatus(RedmineServiceContext context, int id, string name)
		: base(context, id, name)
	{
	}

	internal IssueStatus(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_isDefault = RedmineUtility.LoadBoolean(node[IsDefaultProperty.XmlNodeName]);
		_isClosed  = RedmineUtility.LoadBoolean(node[IsClosedProperty.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);

		Name      = RedmineUtility.LoadString(node[NameProperty.XmlNodeName]);
		IsDefault = RedmineUtility.LoadBoolean(node[IsDefaultProperty.XmlNodeName]);
		IsClosed  = RedmineUtility.LoadBoolean(node[IsClosedProperty.XmlNodeName]);
	}

	#endregion

	#region Properties

	public bool IsDefault
	{
		get => _isDefault;
		private set => UpdatePropertyValue(ref _isDefault, value, IsDefaultProperty);
	}

	public bool IsClosed
	{
		get => _isClosed;
		private set => UpdatePropertyValue(ref _isDefault, value, IsClosedProperty);
	}

	#endregion
}
