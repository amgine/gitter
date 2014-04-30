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

namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class RevisionData : INamedObject
	{
		#region Data

		private readonly Hash _sha1;
		private Hash _treeHash;
		private RevisionData[] _parents;
		private RevisionData[] _children;
		private string _subject;
		private string _body;
		private DateTime _commitDate;
		private string _committerName;
		private string _committerEmail;
		private DateTime _authorDate;
		private string _authorName;
		private string _authorEmail;

		private RevisionField _dataFlags;

		#endregion

		#region .ctor

		public RevisionData(Hash sha1)
		{
			_sha1 = sha1;
			_dataFlags = RevisionField.SHA1;
		}

		#endregion

		#region Properties

		public Hash SHA1
		{
			get { return _sha1; }
		}

		public Hash TreeHash
		{
			get { return _treeHash; }
			set
			{
				_treeHash = value;
				_dataFlags |= RevisionField.TreeHash;
			}
		}

		public RevisionData[] Parents
		{
			get { return _parents; }
			set
			{
				_parents = value;
				_dataFlags |= RevisionField.Parents;
			}
		}

		public RevisionData[] Children
		{
			get { return _children; }
			set
			{
				_children = value;
				_dataFlags |= RevisionField.Children;
			}
		}

		public string Subject
		{
			get { return _subject; }
			set
			{
				_subject = value;
				_dataFlags |= RevisionField.Subject;
			}
		}

		public string Body
		{
			get { return _body; }
			set
			{
				_body = value;
				_dataFlags |= RevisionField.Body;
			}
		}

		public DateTime CommitDate
		{
			get { return _commitDate; }
			set
			{
				_commitDate = value;
				_dataFlags |= RevisionField.CommitDate;
			}
		}

		public string CommitterName
		{
			get { return _committerName; }
			set
			{
				_committerName = value;
				_dataFlags |= RevisionField.CommitterName;
			}
		}

		public string CommitterEmail
		{
			get { return _committerEmail; }
			set
			{
				_committerEmail = value;
				_dataFlags |= RevisionField.CommitterEmail;
			}
		}


		public DateTime AuthorDate
		{
			get { return _authorDate; }
			set
			{
				_authorDate = value;
				_dataFlags |= RevisionField.AuthorDate;
			}
		}

		public string AuthorName
		{
			get { return _authorName; }
			set
			{
				_authorName = value;
				_dataFlags |= RevisionField.AuthorName;
			}
		}

		public string AuthorEmail
		{
			get { return _authorEmail; }
			set
			{
				_authorEmail = value;
				_dataFlags |= RevisionField.AuthorEmail;
			}
		}

		public RevisionField Fields
		{
			get { return _dataFlags; }
		}

		#endregion

		#region Methods

		public bool DataValid(RevisionField data)
		{
			return (_dataFlags & data) == data;
		}

		string INamedObject.Name
		{
			get { return _sha1.ToString(); }
		}

		public override string ToString()
		{
			return _sha1.ToString();
		}

		#endregion
	}
}
