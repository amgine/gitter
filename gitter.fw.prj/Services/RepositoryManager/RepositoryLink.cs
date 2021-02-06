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

	using gitter.Framework.Configuration;

	public sealed class RepositoryLink
	{
		public event EventHandler Deleted;

		private readonly string _path;
		private readonly string _type;
		private string _description;

		public RepositoryLink(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			_path = section.GetValue("Path", string.Empty);
			_type = section.GetValue("Type", string.Empty);
			_description = section.GetValue("Description", string.Empty);
		}

		public RepositoryLink(string path, string type)
		{
			_path = path;
			_type = type;
		}

		internal void InvokeDeleted()
		{
			var handler = Deleted;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public string Path
		{
			get { return _path; }
		}

		public string Type
		{
			get { return _type; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			section.SetValue("Path", _path);
			section.SetValue("Type", _type);
			section.SetValue("Description", _description);
		}
	}
}
