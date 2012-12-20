namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

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

		private string ReadStringValue(string header)
		{
			Skip(header.Length + 1);
			return ReadLine();
		}

		private DateTime ReadDateValue(string header)
		{
			Skip(header.Length + 1);
			return ReadUnixTimestampLine();
		}

		private bool ReadBoundaryValue()
		{
			bool isBoundary;
			if(CheckValue(BoundaryHeader))
			{
				isBoundary = true;
				Skip(BoundaryHeader.Length + 1);
			}
			else
			{
				isBoundary = false;
			}
			return isBoundary;
		}

		private string ReadPreviousValue()
		{
			if(CheckValue(PreviousHeader))
			{
				Skip(PreviousHeader.Length + 1);
				var previous = ReadString(40);
				SkipLine();
				return previous;
			}
			else
			{
				return null;
			}
		}

		private BlameCommit ParseCommitInfo(IDictionary<string, BlameCommit> cache)
		{
			var hash = ReadString(40);
			SkipLine();

			BlameCommit commit;
			if(cache.TryGetValue(hash, out commit))
			{
				return commit;
			}

			var author			= ReadStringValue(AuthorHeader);
			var authorMail		= ReadStringValue(AuthorMailHeader);
			var authorTime		= ReadDateValue(AuthorTimeHeader);
			var authorTZ		= ReadStringValue(AuthorTimeZoneHeader);
			var committer		= ReadStringValue(CommitterHeader);
			var committerMail	= ReadStringValue(CommitterMailHeader);
			var committerTime	= ReadDateValue(CommitterTimeHeader);
			var committerTZ		= ReadStringValue(CommitterTimeZoneHeader);
			var summary			= ReadStringValue(SummaryHeader);
			var isBoundary		= ReadBoundaryValue(); /* optional */
			var previous		= ReadPreviousValue(); /* optional */
			var fileName		= ReadStringValue(FileNameHeader);

			commit = new BlameCommit(hash,
				author, authorMail, authorTime, authorTZ,
				committer, committerMail, committerTime, committerTZ,
				summary, isBoundary, previous);
			cache.Add(hash, commit);
			return commit;
		}

		public BlameFile ParseBlameFile(string fileName)
		{
			var cache = new Dictionary<string, BlameCommit>();
			int lineN = 1;
			BlameCommit prev = null;
			var hunks = new List<BlameHunk>();
			var lines = new List<BlameLine>();

			while(!IsAtEndOfString)
			{
				var commit = ParseCommitInfo(cache);
				Skip();
				if(IsAtEndOfString) break;

				int eol = FindLfLineEnding();
				string ending;
				if(String[eol - 1] == '\r')
				{
					--eol;
					ending = LineEnding.CrLf;
				}
				else
				{
					ending = LineEnding.Lf;
				}
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
