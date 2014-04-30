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
	public class TreeViewModel
	{
		#region Data

		private readonly ITreeSource _treeSource;

		#endregion

		#region .ctor

		public TreeViewModel(ITreeSource treeSource)
		{
			_treeSource = treeSource;
		}

		#endregion

		#region Properties

		public ITreeSource TreeSource
		{
			get { return _treeSource; }
		}

		#endregion

		#region Methods

		public override int GetHashCode()
		{
			return TreeSource != null ? TreeSource.GetHashCode() : 0;
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
			{
				return false;
			}
			var other = obj as TreeViewModel;
			if(other == null)
			{
				return false;
			}
			return object.Equals(TreeSource, other.TreeSource);
		}

		#endregion
	}
}
