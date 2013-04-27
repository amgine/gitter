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

	public sealed class RepositoryGroup : IList<RepositoryLink>
	{
		private readonly string _name;
		private readonly List<RepositoryLink> _repositories;

		public RepositoryGroup(string name)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			_name = name;
			_repositories = new List<RepositoryLink>();
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return _name;
		}

		public int IndexOf(RepositoryLink item)
		{
			return _repositories.IndexOf(item);
		}

		public void Insert(int index, RepositoryLink item)
		{
			_repositories.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_repositories.RemoveAt(index);
		}

		public RepositoryLink this[int index]
		{
			get { return _repositories[index]; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				_repositories[index] = value;
			}
		}

		public void Add(RepositoryLink item)
		{
			Verify.Argument.IsNotNull(item, "item");

			_repositories.Add(item);
		}

		public void Clear()
		{
			_repositories.Clear();
		}

		public bool Contains(RepositoryLink item)
		{
			return _repositories.Contains(item);
		}

		public void CopyTo(RepositoryLink[] array, int arrayIndex)
		{
			_repositories.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _repositories.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(RepositoryLink item)
		{
			return _repositories.Remove(item);
		}

		public IEnumerator<RepositoryLink> GetEnumerator()
		{
			return _repositories.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _repositories.GetEnumerator();
		}
	}
}
