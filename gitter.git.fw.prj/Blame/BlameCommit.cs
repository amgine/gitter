namespace gitter.Git
{
	using System;

	/// <summary>Represents a commit, referenced by a blame output line.</summary>
	public sealed class BlameCommit
	{
		#region Data

		private readonly string _hash;
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
			string hash,
			string author, string authorEmail, DateTime authorDate, string authorTimeZone,
			string committer, string committerEmail, DateTime commitDate, string committerTimeZone,
			string summary, bool isBoundary, string previous)
		{
			_hash = hash;
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

		public string Hash
		{
			get { return _hash; }
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
