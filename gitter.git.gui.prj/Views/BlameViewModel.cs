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

namespace gitter.Git.Gui.Views
{
	public class BlameViewModel
	{
		#region Data

		private readonly IBlameSource _blameSource;

		#endregion

		#region .ctor

		public BlameViewModel(IBlameSource blameSource)
		{
			_blameSource = blameSource;
		}

		#endregion

		#region Properties

		public IBlameSource BlameSource
		{
			get { return _blameSource; }
		}

		#endregion

		#region Methods

		public override int GetHashCode()
		{
			return BlameSource != null ? BlameSource.GetHashCode() : 0;
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
			{
				return false;
			}
			var other = obj as BlameViewModel;
			if(other == null)
			{
				return false;
			}
			return object.Equals(BlameSource, other.BlameSource);
		}

		#endregion
	}
}
