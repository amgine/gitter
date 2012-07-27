namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	partial class Revision
	{
		public sealed class RevisionReferencesCollection : IEnumerable<Reference>
		{
			public event EventHandler Changed;

			private void InvokeChanged()
			{
				var handler = Changed;
				if(handler != null) handler(this, EventArgs.Empty);
			}

			private readonly SortedDictionary<string, Reference> _container;

			internal RevisionReferencesCollection()
			{
				_container = new SortedDictionary<string, Reference>();
			}

			public object SyncRoot
			{
				get { return _container; }
			}

			public int Count
			{
				get
				{
					lock(SyncRoot)
					{
						return _container.Count;
					}
				}
			}

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

			public Reference this[string name]
			{
				get
				{
					lock(SyncRoot)
					{
						return _container[name];
					}
				}
			}

			public IList<Branch> GetBranches()
			{
				lock(SyncRoot)
				{
					if(_container.Count == 0) return new Branch[0];
					var list = new List<Branch>(_container.Count);
					foreach(var reference in _container.Values)
					{
						var branch = reference as Branch;
						if(branch != null) list.Add(branch);
					}
					return list;
				}
			}

			public IList<RemoteBranch> GetRemoteBranches()
			{
				lock(SyncRoot)
				{
					if(_container.Count == 0) return new RemoteBranch[0];
					var list = new List<RemoteBranch>(_container.Count);
					foreach(var reference in _container.Values)
					{
						var branch = reference as RemoteBranch;
						if(branch != null) list.Add(branch);
					}
					return list;
				}
			}

			public IList<BranchBase> GetAllBranches()
			{
				lock(SyncRoot)
				{
					if(_container.Count == 0) return new BranchBase[0];
					var list = new List<BranchBase>(_container.Count);
					foreach(var reference in _container.Values)
					{
						var branch = reference as BranchBase;
						if(branch != null) list.Add(branch);
					}
					return list;
				}
			}

			public IList<Tag> GetTags()
			{
				lock(SyncRoot)
				{
					if(_container.Count == 0) return new Tag[0];
					var list = new List<Tag>(_container.Count);
					foreach(var reference in _container.Values)
					{
						var tag = reference as Tag;
						if(tag != null) list.Add(tag);
					}
					return list;
				}
			}

			internal void Remove(string reference)
			{
				if(reference == null) throw new ArgumentNullException("reference");

				bool removed;
				lock(SyncRoot)
				{
					removed = _container.Remove(reference);
				}
				if(removed) InvokeChanged();
			}

			internal void Remove(Reference reference)
			{
				if(reference == null) throw new ArgumentNullException("reference");

				bool removed;
				lock(SyncRoot)
				{
					removed = _container.Remove(reference.FullName);
				}
				if(removed) InvokeChanged();
			}

			internal void Rename(string oldName, Reference reference)
			{
				if(reference == null) throw new ArgumentNullException("reference");
				if(oldName == null) throw new ArgumentNullException("oldName");

				lock(SyncRoot)
				{
					_container.Remove(oldName);
					_container.Add(reference.FullName, reference);
				}
				InvokeChanged();
			}

			internal void Add(Reference reference)
			{
				if(reference == null) throw new ArgumentNullException("reference");

				lock(SyncRoot)
				{
					_container.Add(reference.FullName, reference);
				}
				InvokeChanged();
			}

			#region IEnumerable<Reference>

			public IEnumerator<Reference> GetEnumerator()
			{
				return _container.Values.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _container.Values.GetEnumerator();
			}

			#endregion
		}
	}
}
