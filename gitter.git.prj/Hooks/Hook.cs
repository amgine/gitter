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
		/// <summary>Create <see cref="Hook"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Hook name.</param>
		internal Hook(Repository repository, string name)
			: base(repository, name)
		{
			RelativePath = "hooks" + Path.DirectorySeparatorChar + name;
			FullPath = Path.Combine(repository.GitDirectory, RelativePath);
			if(!File.Exists(FullPath))
			{
				MarkAsDeleted();
			}
		}

		public string RelativePath { get; }

		public string FullPath { get; }

		public bool IsAvailable => File.Exists(FullPath);

		public void Set(string value)
			=> File.WriteAllText(FullPath, value);

		public void Delete()
		{
			if(IsAvailable)
			{
				try
				{
					File.Delete(FullPath);
				}
				catch
				{
				}
			}
		}
	}
}
