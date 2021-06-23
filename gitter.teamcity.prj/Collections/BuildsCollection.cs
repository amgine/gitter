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
	using System.Collections.Generic;
	using System.Xml;

	public sealed class BuildsCollection : TeamCityObjectsCache<Build>
	{
		const string QUERY = @"builds/?locator=";

		internal BuildsCollection(TeamCityServiceContext context)
			: base(context)
		{
		}

		public Build[] Query(BuildLocator locator)
		{
			Verify.Argument.IsNotNull(locator, nameof(locator));

			var xml = Context.GetXml(QUERY + locator.ToString());
			var root = xml["builds"];
			var result = new Build[TeamCityUtility.LoadInt(root.Attributes["count"])];
			int id = 0;
			foreach(XmlElement node in root.ChildNodes)
			{
				result[id++] = Lookup(node);
			}
			return result;
		}

		public void UpdateCache(BuildLocator locator)
		{
			Verify.Argument.IsNotNull(locator, nameof(locator));

			var xml = Context.GetXml(QUERY + locator.ToString());
			var root = xml["builds"];
			foreach(XmlElement node in root.ChildNodes)
			{
				Lookup(node);
			}
		}

		protected override Build Create(string id)
			=> new Build(Context, id);

		protected override Build Create(XmlNode node)
			=> new Build(Context, node);
	}
}
