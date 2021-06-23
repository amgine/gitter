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

	public sealed class ProjectBuildTypesCollection : CacheSegment<BuildType>
	{
		private readonly Project _project;

		internal ProjectBuildTypesCollection(Project project, BuildTypesCollection buildTypes)
			: base(buildTypes)
		{
			Verify.Argument.IsNotNull(project, nameof(project));

			_project = project;
		}

		protected override bool IsIncluded(BuildType item)
			=> item.Project == _project;

		public override void Refresh()
			=> Context.BuildTypes.UpdateCache(_project.CreateLocator());
	}
}
