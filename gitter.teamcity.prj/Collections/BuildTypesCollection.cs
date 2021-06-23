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

	public sealed class BuildTypesCollection : NamedTeamCityObjectsCache<BuildType>
	{
		internal BuildTypesCollection(TeamCityServiceContext context)
			: base(context)
		{
		}

		protected override BuildType Create(string id, string name)
			=> new BuildType(Context, id, name);

		protected override BuildType Create(string id)
			=> new BuildType(Context, id);

		protected override BuildType Create(XmlNode node)
			=> new BuildType(Context, node);

		public void UpdateCache()
		{
			var xml = Context.GetXml("buildTypes");
			foreach(XmlElement node in xml["buildTypes"])
			{
				Lookup(node);
			}
		}

		public void UpdateCache(ProjectLocator projectLocator)
		{
			Verify.Argument.IsNotNull(projectLocator, nameof(projectLocator));
			var pl = projectLocator.ToString();
			Verify.Argument.IsNeitherNullNorWhitespace(pl, "projectLocator");

			var xml = Context.GetXml("projects/" + pl + "/buildTypes");
			foreach(XmlElement node in xml["buildTypes"])
			{
				Lookup(node);
			}
		}
	}
}
