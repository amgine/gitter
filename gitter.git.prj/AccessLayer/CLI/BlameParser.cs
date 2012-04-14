namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Globalization;

	internal sealed class BlameParser : GitParser
	{
		#region Constants

		private const string AuthorHeader = "author";
		private const string AuthorMailHeader = "author-mail";
		private const string AuthorTimeHeader = "author-time";
		private const string AuthorTimeZoneHeader = "author-tz";

		private const string CommitterHeader = "committer";
		private const string CommitterMailHeader = "committer-mail";
		private const string CommitterTimeHeader = "committer-time";
		private const string CommitterTimeZoneHeader = "committer-tz";

		private const string SummaryHeader = "summary";
		private const string BoundaryHeader = "boundary";
		private const string PreviousHeader = "previous";
		private const string FileNameHeader = "filename";

		#endregion

		#region Headers

		private enum Header
		{
			Unknown = -1,

			Author = 0,
			AuthorMail = 1,
			AuthorTime = 2,
			AuthorTimeZone = 3,
			Committer = 4,
			CommitterMail = 5,
			CommitterTime = 6,
			CommitterTimeZone = 7,
			Summary = 8,
			Boundary = 9,
			Previous = 10,
			FileName = 11,
		}

		private static readonly string[] Headers = new[]
		{
			AuthorHeader,
			AuthorMailHeader,
			AuthorTimeHeader,
			AuthorTimeZoneHeader,

			CommitterHeader,
			CommitterMailHeader,
			CommitterTimeHeader,
			CommitterTimeZoneHeader,

			SummaryHeader,
			BoundaryHeader,
			PreviousHeader,
			FileNameHeader,
		};

		#endregion

		public BlameParser(string @string)
			: base(@string)
		{
		}

		public BlameCommit ParseCommitInfo(IDictionary<string, BlameCommit> cache)
		{
			var sha = ReadString(40);
			SkipLine();

			BlameCommit commit;
			if(cache.TryGetValue(sha, out commit))
			{
				return commit;
			}

			Skip(AuthorHeader.Length + 1);
			var author = ReadLine();
			Skip(AuthorMailHeader.Length + 1);
			var authorMail = ReadLine();
			Skip(AuthorTimeHeader.Length + 1);
			var authorTime = ReadUnixTimestampLine();
			Skip(AuthorTimeZoneHeader.Length + 1);
			var authorTZ = ReadLine();
			Skip(CommitterHeader.Length + 1);
			var committer = ReadLine();
			Skip(CommitterMailHeader.Length + 1);
			var committerMail = ReadLine();
			Skip(CommitterTimeHeader.Length + 1);
			var committerTime = ReadUnixTimestampLine();
			Skip(CommitterTimeZoneHeader.Length + 1);
			var committerTZ = ReadLine();
			Skip(SummaryHeader.Length + 1);
			var summary = ReadLine();

			bool isBoundary = false;
			string previous = null;
			if(CheckValue(BoundaryHeader))
			{
				isBoundary = true;
				Skip(BoundaryHeader.Length + 1);
			}
			if(CheckValue(PreviousHeader))
			{
				Skip(PreviousHeader.Length + 1);
				previous = ReadString(40);
				SkipLine();
			}
			Skip(FileNameHeader.Length + 1);
			var fileName = ReadLine();

			commit = new BlameCommit(sha,
				author, authorMail, authorTime, authorTZ,
				committer, committerMail, committerTime, committerTZ,
				summary, isBoundary, previous);
			cache.Add(sha, commit);
			return commit;
		}

		public BlameFile ParseBlameFile(string fileName)
		{
			var cache = new Dictionary<string, BlameCommit>();
			int lineN = 1;
			BlameCommit prev = null;
			List<BlameHunk> hunks = new List<BlameHunk>();
			List<BlameLine> lines = new List<BlameLine>();

			while(!IsAtEndOfString)
			{
				var commit = ParseCommitInfo(cache);
				Skip();
				if(IsAtEndOfString) break;

				string ending;
				int eol = FindLineEnding(out ending);
				var line = new BlameLine(commit, lineN++, ReadStringUpTo(eol, ending.Length), ending);

				if(commit != prev)
				{
					if(lines.Count != 0)
					{
						hunks.Add(new BlameHunk(prev, lines));
						lines.Clear();
					}
				}
				lines.Add(line);
				prev = commit;
			}
			if(lines.Count != 0)
			{
				hunks.Add(new BlameHunk(prev, lines));
				lines.Clear();
			}

			return new BlameFile(fileName, hunks);
		}
	}
}
