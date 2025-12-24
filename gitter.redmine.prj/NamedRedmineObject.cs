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

public abstract class NamedRedmineObject : RedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<string> NameProperty = new("name", nameof(Name));

	#endregion

	#region Data

	private string _name;

	#endregion

	#region .ctor

	protected NamedRedmineObject(RedmineServiceContext context, int id, string name)
		: base(context, id)
	{
		_name = name;
	}

	protected NamedRedmineObject(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_name = RedmineUtility.LoadString(node[NameProperty.XmlNodeName]);
	}

	#endregion

	#region Properties

	public string Name
	{
		get => _name;
		internal set => UpdatePropertyValue(ref _name, value, NameProperty);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);
		Name = RedmineUtility.LoadString(node[NameProperty.XmlNodeName]);
	}

	public override string ToString() => Name;

	#endregion
}
