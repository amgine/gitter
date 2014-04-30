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
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	public sealed partial class Revision : GitObject, IRevisionPointer
	{
		#region Data

		private readonly RevisionParentsCollection _parents;
		private readonly RevisionReferencesCollection _references;
		private bool _isLoaded;

		private readonly Hash _hash;
		private readonly string _hashString;
		private Hash _treeHash;
		private string _treeHashString;

		private string _subject;
		private string _body;

		private DateTime _commitDate;
		private User _committer;

		private DateTime _authorDate;
		private User _author;

		#endregion

		#region .ctor

		internal Revision(Repository repository, Hash hash)
			: base(repository)
		{
			_parents    = new RevisionParentsCollection();
			_references = new RevisionReferencesCollection();
			_hash       = hash;
			_hashString = hash.ToString();
		}

		#endregion

		public void Load()
		{
			var revisionData = Repository.Accessor.QueryRevision.Invoke(
				new QueryRevisionParameters(Hash));
			ObjectFactories.UpdateRevision(this, revisionData);
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

		public bool IsLoaded
		{
			get { return _isLoaded; }
			set { _isLoaded = value; }
		}

		#endregion

		#region Commit Attributes

		public Hash Hash
		{
			get { return _hash; }
		}

		public string HashString
		{
			get { return _hashString; }
		}

		public Hash TreeHash
		{
			get { return _treeHash; }
			internal set
			{
				if(!_isLoaded)
				{
					_treeHash = value;
					_treeHashString = value.ToString();
				}
				else
				{
					if(_treeHash != value)
					{
						_treeHash = value;
						_treeHashString = value.ToString();
					}
				}
			}
		}

		public string TreeHashString
		{
			get { return _treeHashString; }
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
			get { return _hashString; }
		}

		string IRevisionPointer.FullName
		{
			get { return _hashString; }
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
			return string.Format("{0}: {1}", _hash.ToString(7), Subject);
		}
	}
}
