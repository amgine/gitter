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

		internal void InvokeDeleted()
			=> Deleted?.Invoke(this, EventArgs.Empty);

		public RepositoryLink(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			Path        = section.GetValue("Path", string.Empty);
			Type        = section.GetValue("Type", string.Empty);
			Description = section.GetValue("Description", string.Empty);
		}

		public RepositoryLink(string path, string type)
		{
			Path = path;
			Type = type;
		}

		public string Path { get; }

		public string Type { get; }

		public string Description { get; set; }

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			section.SetValue("Path", Path);
			section.SetValue("Type", Type);
			section.SetValue("Description", Description);
		}
	}
}
