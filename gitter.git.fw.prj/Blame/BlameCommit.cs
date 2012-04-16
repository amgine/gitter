namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public sealed class BlameCommit
	{
		#region Data

		private readonly string _sha1;
		private readonly string _author;
		private readonly string _authorEmail;
		private readonly DateTime _authorDate;
		private readonly string _authorTimeZone;
		private readonly string _committer;
		private readonly string _committerEmail;
		private readonly DateTime _commitDate;
		private readonly string _committerTimeZone;
		private readonly string _summary;
		private readonly bool _isBoundary;
		private readonly string _previous;

		#endregion

		#region .ctor

		public BlameCommit(
			string sha1,
			string author, string authorEmail, DateTime authorDate, string authorTimeZone,
			string committer, string committerEmail, DateTime commitDate, string committerTimeZone,
			string summary, bool isBoundary, string previous)
		{
			_sha1 = sha1;
			_author = author;
			_authorEmail = authorEmail;
			_authorDate = authorDate;
			_authorTimeZone = authorTimeZone;
			_committer = committer;
			_committerEmail = committerEmail;
			_commitDate = commitDate;
			_committerTimeZone = committerTimeZone;
			_summary = summary;
			_isBoundary = isBoundary;
			_previous = previous;
		}

		#endregion

		#region Properties

		public string SHA1
		{
			get { return _sha1; }
		}

		public string Summary
		{
			get { return _summary; }
		}

		public string Author
		{
			get { return _author; }
		}

		public DateTime AuthorDate
		{
			get { return _authorDate; }
		}

		public string AuthorEmail
		{
			get { return _authorEmail; }
		}

		public string Committer
		{
			get { return _committer; }
		}

		public string CommitterEmail
		{
			get { return _committerEmail; }
		}

		public DateTime CommitDate
		{
			get { return _commitDate; }
		}

		public bool IsBoundary
		{
			get { return _isBoundary; }
		}

		public string Previous
		{
			get { return _previous; }
		}

		#endregion

		public override string ToString()
		{
			return _summary;
		}
	}
}
