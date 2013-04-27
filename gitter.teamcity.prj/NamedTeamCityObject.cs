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

namespace gitter.TeamCity
{
	using System;
	using System.Xml;

	public abstract class NamedTeamCityObject : TeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<string> NameProperty =
			new TeamCityObjectProperty<string>("name", "Name");

		#endregion

		#region Data

		private string _name;

		#endregion

		#region .ctor

		protected NamedTeamCityObject(TeamCityServiceContext context, string id)
			: base(context, id)
		{
		}

		protected NamedTeamCityObject(TeamCityServiceContext context, string id, string name)
			: base(context, id)
		{
			_name = name;
		}

		protected NamedTeamCityObject(TeamCityServiceContext context, XmlNode node)
			: base(context, node)
		{
			_name = TeamCityUtility.LoadString(node.Attributes[NameProperty.XmlNodeName]);
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
			internal set { UpdatePropertyValue(ref _name, value, NameProperty); }
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);
			Name = TeamCityUtility.LoadString(node.Attributes[NameProperty.XmlNodeName]);
		}

		public override string ToString()
		{
			return _name;
		}

		#endregion
	}
}
