namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	public sealed class Revision : GitNamedObject, IRevisionPointer
	{
		#region Data

		private bool _loaded;

		private readonly List<Revision> _parents;

		private readonly SortedDictionary<string, IRevisionPointer> _refs;

		private string _subject;
		private string _body;

		private string _treeHash;

		private DateTime _commitDate;
		private User _committer;

		private DateTime _authorDate;
		private User _author;

		#endregion

		#region Events

		public event EventHandler ReferenceListChanged;

		private void InvokeReferenceListChanged()
		{
			var handler = ReferenceListChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public event EventHandler NoteChanged;

		private void InvokeNoteChanged()
		{
			var handler = NoteChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		internal Revision(Repository repository, string hash)
			: base(repository, hash)
		{
			if(hash == null) throw new ArgumentNullException("hash");
			if(hash.Length != 40) throw new ArgumentException("hash");

			_parents = new List<Revision>(2);
			_refs = new SortedDictionary<string, IRevisionPointer>();
		}

		#endregion

		internal void Load()
		{
			var revisionData = Repository.Accessor.QueryRevision(
				new QueryRevisionParameters(Name));
			ObjectFactories.UpdateRevision(this, revisionData);
		}

		internal bool IsLoaded
		{
			get { return _loaded; }
			set { _loaded = value; }
		}

		#region Properties

		public IList<Revision> Parents
		{
			get { return _parents; }
		}

		public bool IsCurrent
		{
			get { return Repository.Head.Revision == this; }
		}

		public IDictionary<string, IRevisionPointer> Refs
		{
			get { return _refs; }
		}

		public object RefsSyncRoot
		{
			get { return _refs; }
		}

		public IList<Branch> GetBranches()
		{
			lock(RefsSyncRoot)
			{
				if(_refs.Count == 0) return new Branch[0];
				var list = new List<Branch>(_refs.Count);
				foreach(var reference in _refs.Values)
				{
					var branch = reference as Branch;
					if(branch != null) list.Add(branch);
				}
				return list;
			}
		}

		public IList<RemoteBranch> GetRemoteBranches()
		{
			lock(RefsSyncRoot)
			{
				if(_refs.Count == 0) return new RemoteBranch[0];
				var list = new List<RemoteBranch>(_refs.Count);
				foreach(var reference in _refs.Values)
				{
					var branch = reference as RemoteBranch;
					if(branch != null) list.Add(branch);
				}
				return list;
			}
		}

		public IList<BranchBase> GetAllBranches()
		{
			lock(RefsSyncRoot)
			{
				if(_refs.Count == 0) return new BranchBase[0];
				var list = new List<BranchBase>(_refs.Count);
				foreach(var reference in _refs.Values)
				{
					var branch = reference as BranchBase;
					if(branch != null) list.Add(branch);
				}
				return list;
			}
		}

		public IList<Tag> GetTags()
		{
			lock(RefsSyncRoot)
			{
				if(_refs.Count == 0) return new Tag[0];
				var list = new List<Tag>(_refs.Count);
				foreach(var reference in _refs.Values)
				{
					var tag = reference as Tag;
					if(tag != null) list.Add(tag);
				}
				return list;
			}
		}

		#endregion

		#region Commit Attributes

		public string SHA1
		{
			get { return Name; }
		}

		public string TreeHash
		{
			get { return _treeHash; }
			internal set
			{
				if(!_loaded)
				{
					_treeHash = value;
				}
				else
				{
					if(_treeHash != value)
					{
						_treeHash = value;
					}
				}
			}
		}

		public User Author
		{
			get { return _author; }
			internal set
			{
				if(!_loaded)
				{
					_author = value;
				}
				else
				{
					if(_author != value)
					{
						_author = value;
					}
				}
			}
		}

		public DateTime AuthorDate
		{
			get { return _authorDate; }
			internal set
			{
				_authorDate = value;
			}
		}

		public User Committer
		{
			get { return _committer; }
			internal set
			{
				_committer = value;
			}
		}

		public DateTime CommitDate
		{
			get { return _commitDate; }
			internal set
			{
				_commitDate = value; 
			}
		}

		public string Subject
		{
			get { return _subject; }
			internal set
			{
				_subject = value;
			}
		}

		public string Body
		{
			get { return _body; }
			internal set
			{
				_body = value;
			}
		}

		#endregion

		#region Reference List Management

		internal void RemoveRef(string reference)
		{
			if(reference == null) throw new ArgumentNullException("reference");

			bool removed;
			lock(RefsSyncRoot)
			{
				removed = _refs.Remove(reference);
			}
			if(removed) InvokeReferenceListChanged();
		}

		internal void RemoveRef(IRevisionPointer reference)
		{
			if(reference == null) throw new ArgumentNullException("reference");

			bool removed;
			lock(RefsSyncRoot)
			{
				removed = _refs.Remove(reference.FullName);
			}
			if(removed) InvokeReferenceListChanged();
		}

		internal void RenameRef(string oldName, IRevisionPointer reference)
		{
			if(reference == null) throw new ArgumentNullException("reference");
			if(oldName == null) throw new ArgumentNullException("oldName");

			lock(RefsSyncRoot)
			{
				_refs.Remove(oldName);
				_refs.Add(reference.FullName, reference);
			}
			InvokeReferenceListChanged();
		}

		internal void AddRef(IRevisionPointer reference)
		{
			if(reference == null) throw new ArgumentNullException("reference");

			lock(RefsSyncRoot)
			{
				_refs.Add(reference.FullName, reference);
			}
			InvokeReferenceListChanged();
		}

		#endregion

		#region IRevisionPointer Members

		ReferenceType IRevisionPointer.Type
		{
			get { return ReferenceType.Revision; }
		}

		string IRevisionPointer.Pointer
		{
			get { return Name; }
		}

		string IRevisionPointer.FullName
		{
			get { return Name; }
		}

		bool IRevisionPointer.IsDeleted
		{
			get { return false; }
		}

		Revision IRevisionPointer.Dereference()
		{
			return this;
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0}: {1}", Name.Substring(0, 7), Subject);
		}
	}
}
