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
