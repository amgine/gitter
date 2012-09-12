namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	public sealed partial class Revision : GitObject, IRevisionPointer
	{
		#region Data

		private readonly RevisionParentsCollection _parents;
		private readonly RevisionReferencesCollection _references;
		private bool _isLoaded;

		private readonly string _hash;
		private string _treeHash;

		private string _subject;
		private string _body;

		private DateTime _commitDate;
		private User _committer;

		private DateTime _authorDate;
		private User _author;

		#endregion

		#region .ctor

		internal Revision(Repository repository, string hash)
			: base(repository)
		{
			Verify.Argument.IsNotNull(hash, "hash");
			Verify.Argument.IsTrue(hash.Length == 40, "hash");

			_parents = new RevisionParentsCollection();
			_references = new RevisionReferencesCollection();
			_hash = hash;
		}

		#endregion

		internal void Load()
		{
			var revisionData = Repository.Accessor.QueryRevision(
				new QueryRevisionParameters(Hash));
			ObjectFactories.UpdateRevision(this, revisionData);
		}

		internal bool IsLoaded
		{
			get { return _isLoaded; }
			set { _isLoaded = value; }
		}

		#region Properties

		public RevisionParentsCollection Parents
		{
			get { return _parents; }
		}

		public RevisionReferencesCollection References
		{
			get { return _references; }
		}

		public bool IsCurrent
		{
			get { return Repository.Head.Revision == this; }
		}

		#endregion

		#region Commit Attributes

		public string Hash
		{
			get { return _hash; }
		}

		public string TreeHash
		{
			get { return _treeHash; }
			internal set
			{
				if(!_isLoaded)
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
				if(!_isLoaded)
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

		#region IRevisionPointer Members

		ReferenceType IRevisionPointer.Type
		{
			get { return ReferenceType.Revision; }
		}

		string IRevisionPointer.Pointer
		{
			get { return Hash; }
		}

		string IRevisionPointer.FullName
		{
			get { return Hash; }
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
			return string.Format("{0}: {1}", Hash.Substring(0, 7), Subject);
		}
	}
}
