namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	partial class Revision
	{
		public sealed class RevisionParentsCollection : IList<Revision>
		{
			private readonly List<Revision> _container;

			internal RevisionParentsCollection()
			{
				_container = new List<Revision>(2);
			}

			public int IndexOf(Revision item)
			{
				return _container.IndexOf(item);
			}

			void IList<Revision>.Insert(int index, Revision item)
			{
				throw new NotSupportedException();
			}

			void IList<Revision>.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public Revision this[int index]
			{
				get { return _container[index]; }
				set { throw new NotSupportedException(); }
			}

			internal void AddInternal(Revision item)
			{
				_container.Add(item);
			}

			internal void InsertInternal(int index, Revision item)
			{
				_container.Insert(index, item);
			}

			internal void RemoveInternal(Revision item)
			{
				_container.Remove(item);
			}

			void ICollection<Revision>.Add(Revision item)
			{
				throw new NotSupportedException();
			}

			void ICollection<Revision>.Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(Revision item)
			{
				return _container.Contains(item);
			}

			public void CopyTo(Revision[] array, int arrayIndex)
			{
				_container.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return _container.Count; }
			}

			bool ICollection<Revision>.IsReadOnly
			{
				get { return true; }
			}

			bool ICollection<Revision>.Remove(Revision item)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<Revision> GetEnumerator()
			{
				return _container.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _container.GetEnumerator();
			}
		}
	}
}
