namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class RevisionData : INamedObject
	{
		#region Data

		private readonly string _sha1;

		private string _treeHash;
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

		public RevisionData(string sha1)
		{
			Verify.Argument.IsNotNull(sha1, "sha1");
			Verify.Argument.IsTrue(sha1.Length == 40, "sha1");

			_sha1 = sha1;
			_dataFlags = RevisionField.SHA1;
		}

		#endregion

		#region Properties

		public string SHA1
		{
			get { return _sha1; }
		}

		public string TreeHash
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

		public bool DataValid(RevisionField data)
		{
			return (_dataFlags & data) == data;
		}

		string INamedObject.Name
		{
			get { return _sha1; }
		}

		public override string ToString()
		{
			return _sha1;
		}
	}
}
