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

	using gitter.Framework;

	public sealed class RevisionReferencesCollection : IEnumerable<Reference>
	{
		#region Events

		public event EventHandler Changed;

		private void OnChanged()
			=> Changed?.Invoke(this, EventArgs.Empty);

		#endregion

		#region Data

		private readonly SortedDictionary<string, Reference> _container = new();

		#endregion

		#region .ctor

		internal RevisionReferencesCollection()
		{
		}

		#endregion

		#region Properties

		public object SyncRoot => _container;

		public int Count
		{
			get { lock(SyncRoot) return _container.Count; }
		}

		public Reference this[string name]
		{
			get { lock(SyncRoot) return _container[name]; }
		}

		#endregion

		#region Public Methods

		public bool Contains(string referenceName)
		{
			lock(SyncRoot)
			{
				return _container.ContainsKey(referenceName);
			}
		}

		public bool Contains(Reference reference)
		{
			if(reference == null) return false;

			lock(SyncRoot)
			{
				return _container.ContainsKey(reference.Name);
			}
		}

		private IReadOnlyList<T> GetRefs<T>()
			where T : Reference
		{
			lock(SyncRoot)
			{
				if(_container.Count == 0) return Preallocated<T>.EmptyArray;
				var list = default(List<T>);
				foreach(var reference in _container.Values)
				{
					if(reference is T typedRef)
					{
						list ??= new List<T>(_container.Count);
						list.Add(typedRef);
					}
				}
				return list ?? (IReadOnlyList<T>)Preallocated<T>.EmptyArray;
			}
		}

		public IReadOnlyList<Branch> GetBranches() => GetRefs<Branch>();

		public IReadOnlyList<RemoteBranch> GetRemoteBranches() => GetRefs<RemoteBranch>();

		public IReadOnlyList<BranchBase> GetAllBranches() => GetRefs<BranchBase>();

		public IReadOnlyList<Tag> GetTags() => GetRefs<Tag>();

		#endregion

		#region Internal Methods

		internal void Remove(string reference)
		{
			Assert.IsNotNull(reference);

			bool removed;
			lock(SyncRoot)
			{
				removed = _container.Remove(reference);
			}
			if(removed) OnChanged();
		}

		internal void Remove(Reference reference)
		{
			Assert.IsNotNull(reference);

			bool removed;
			lock(SyncRoot)
			{
				removed = _container.Remove(reference.FullName);
			}
			if(removed) OnChanged();
		}

		internal void Rename(string oldName, Reference reference)
		{
			Assert.IsNeitherNullNorWhitespace(oldName);
			Verify.Argument.IsNotNull(reference, nameof(reference));

			lock(SyncRoot)
			{
				_container.Remove(oldName);
				_container.Add(reference.FullName, reference);
			}
			OnChanged();
		}

		internal void Add(Reference reference)
		{
			Verify.Argument.IsNotNull(reference, nameof(reference));

			lock(SyncRoot)
			{
				_container.Add(reference.FullName, reference);
			}
			OnChanged();
		}

		#endregion

		#region IEnumerable<Reference>

		public SortedDictionary<string, Reference>.ValueCollection.Enumerator GetEnumerator()
			=> _container.Values.GetEnumerator();

		IEnumerator<Reference> IEnumerable<Reference>.GetEnumerator()
			=> _container.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> _container.Values.GetEnumerator();

		#endregion
	}
}
