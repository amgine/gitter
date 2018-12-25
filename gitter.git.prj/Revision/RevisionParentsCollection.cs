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
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public sealed class RevisionParentsCollection : IList<Revision>
	{
		#region Data

		private readonly List<Revision> _container;

		#endregion

		#region .ctor

		internal RevisionParentsCollection()
		{
			_container = new List<Revision>(2);
		}

		#endregion

		#region Properties

		public Revision this[int index]
		{
			get { return _container[index]; }
			internal set
			{
				Assert.IsNotNull(value);

				_container[index] = value;
			}
		}

		public int Count => _container.Count;

		#endregion

		#region Internal Methods

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

		#endregion

		#region Public Methods

		public int IndexOf(Revision item)
		{
			return _container.IndexOf(item);
		}

		public bool Contains(Revision item)
		{
			return _container.Contains(item);
		}

		public void CopyTo(Revision[] array, int arrayIndex)
		{
			_container.CopyTo(array, arrayIndex);
		}

		#endregion

		#region IList<Revision>

		Revision IList<Revision>.this[int index]
		{
			get { return _container[index]; }
			set { throw new NotSupportedException(); }
		}

		void IList<Revision>.Insert(int index, Revision item)
		{
			throw new NotSupportedException();
		}

		void IList<Revision>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region ICollection<Revision>

		void ICollection<Revision>.Add(Revision item)
		{
			throw new NotSupportedException();
		}

		void ICollection<Revision>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<Revision>.IsReadOnly
		{
			get { return true; }
		}

		bool ICollection<Revision>.Remove(Revision item)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IEnumerable<Revision>

		public List<Revision>.Enumerator GetEnumerator()
		{
			return _container.GetEnumerator();
		}

		IEnumerator<Revision> IEnumerable<Revision>.GetEnumerator()
		{
			return _container.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _container.GetEnumerator();
		}

		#endregion
	}
}
