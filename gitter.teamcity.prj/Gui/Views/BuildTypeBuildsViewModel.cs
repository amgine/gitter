#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity.Gui.Views
{
	public class BuildTypeBuildsViewModel
	{
		#region Data

		private readonly BuildType _buildType;

		#endregion

		#region .ctor

		public BuildTypeBuildsViewModel(BuildType buildType)
		{
			_buildType = buildType;
		}

		#endregion

		#region Properties

		public BuildType BuildType
		{
			get { return _buildType; }
		}

		#endregion

		#region Methods

		public override int GetHashCode()
		{
			return BuildType != null ? BuildType.GetHashCode() : 0;
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
			{
				return false;
			}
			var other = obj as BuildTypeBuildsViewModel;
			if(other == null)
			{
				return false;
			}
			return object.Equals(BuildType, other.BuildType);
		}

		#endregion
	}
}
