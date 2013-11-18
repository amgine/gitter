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
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class BuildType : NamedTeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<Project> ProjectProperty =
			new TeamCityObjectProperty<Project>("projectId", "Project");

		#endregion

		#region Data

		private readonly BuildTypeBuildsCollection _builds;
		private Project _project;

		#endregion

		#region .ctor

		internal BuildType(TeamCityServiceContext context, string id, string name)
			: base(context, id, name)
		{
			_builds = new BuildTypeBuildsCollection(this, Context.Builds);
		}

		internal BuildType(TeamCityServiceContext context, string id)
			: base(context, id)
		{
			_builds = new BuildTypeBuildsCollection(this, Context.Builds);
		}

		internal BuildType(TeamCityServiceContext context, XmlNode node)
			: base(context, node)
		{
			_builds = new BuildTypeBuildsCollection(this, Context.Builds);
			_project = Context.Projects.Lookup(TeamCityUtility.LoadString(node.Attributes["projectId"]), TeamCityUtility.LoadString(node.Attributes["projectName"]));
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);
			Project = Context.Projects.Lookup(
				TeamCityUtility.LoadString(node.Attributes["projectId"]),
				TeamCityUtility.LoadString(node.Attributes["projectName"]));
		}

		public BuildTypeLocator CreateLocator()
		{
			return new BuildTypeLocator() { Id = Id };
		}

		#endregion

		#region Properties

		public BuildTypeBuildsCollection Builds
		{
			get { return _builds; }
		}

		public Project Project
		{
			get { return _project; }
			private set { UpdatePropertyValue(ref _project, value, ProjectProperty); }
		}

		#endregion
	}
}
