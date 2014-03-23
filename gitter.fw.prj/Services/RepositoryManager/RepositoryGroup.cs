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

namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;

	public sealed class RepositoryGroup
	{
		#region Data

		private string _name;
		private readonly RepositoryCollection _repositories;
		private readonly RepositoryGroupCollection _groups;

		#endregion

		#region Events

		public event EventHandler NameChanged;

		private void OnNameChanged()
		{
			var handler = NameChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public event EventHandler Deleted;

		internal void OnDeleted()
		{
			var handler = Deleted;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		public RepositoryGroup(string name)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			_name         = name;
			_repositories = new RepositoryCollection();
			_groups       = new RepositoryGroupCollection();
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
			set
			{
				if(_name != value)
				{
					_name = value;
					OnNameChanged();
				}
			}
		}

		public RepositoryGroupCollection Groups
		{
			get { return _groups; }
		}

		public RepositoryCollection Respositories
		{
			get { return _repositories; }
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return Name;
		}

		#endregion
	}
}
