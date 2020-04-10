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

namespace gitter.Git
{	
	using System.IO;
	

	/// <summary>Repository hook.</summary>
	public sealed class Hook : GitNamedObjectWithLifetime
	{
		#region Data

		private readonly string _fullPath;
		private readonly string _relativePath;

		#endregion

		/// <summary>Create <see cref="Hook"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Hook name.</param>
		internal Hook(Repository repository, string name)
			: base(repository, name)
		{
			_relativePath = "hooks" + Path.DirectorySeparatorChar + name;
			_fullPath = Path.Combine(repository.GitDirectory, _relativePath);
			if(!File.Exists(_fullPath))
			{
				MarkAsDeleted();
			}
		}

		public string RelativePath
		{
			get { return _relativePath; }
		}

		public string FullPath
		{
			get { return _fullPath; }
		}

		public bool IsAvailable
		{
			get { return File.Exists(_fullPath); }
		}

		public void Set(string value)
		{
			File.WriteAllText(_fullPath, value);
		}

		public void Delete()
		{
			if(IsAvailable)
			{
				try
				{
					File.Delete(_fullPath);
				}
				catch
				{
				}
			}
		}
	}
}
