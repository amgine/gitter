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

namespace gitter.Git;

using System;
using System.Collections;
using System.Collections.Generic;

public sealed class RevisionParentsCollection : IReadOnlyList<Revision>
{
	private readonly List<Revision> _container = new(capacity: 2);

	internal RevisionParentsCollection()
	{
	}

	public Revision this[int index]
	{
		get => _container[index];
		internal set
		{
			Assert.IsNotNull(value);

			_container[index] = value;
		}
	}

	public int Count => _container.Count;

	internal void AddInternal(Revision item)
	{
		Assert.IsNotNull(item);

		_container.Add(item);
	}

	internal void InsertInternal(int index, Revision item)
	{
		Assert.IsNotNull(item);

		_container.Insert(index, item);
	}

	internal void RemoveInternal(Revision item)
	{
		Assert.IsNotNull(item);

		_container.Remove(item);
	}

	internal void ClearInternal()
	{
		_container.Clear();
	}

	public int IndexOf(Revision item)
		=> _container.IndexOf(item);

	public bool Contains(Revision item)
		=>  _container.Contains(item);

	public void CopyTo(Revision[] array, int arrayIndex)
		=> _container.CopyTo(array, arrayIndex);

	Revision IReadOnlyList<Revision>.this[int index] => _container[index];

	public List<Revision>.Enumerator GetEnumerator()
		=> _container.GetEnumerator();

	IEnumerator<Revision> IEnumerable<Revision>.GetEnumerator()
		=> _container.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> _container.GetEnumerator();
}
