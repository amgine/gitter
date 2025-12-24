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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;

internal sealed class BlameParser(string @string) : GitParser(@string)
{
	#region Constants

	private const string AuthorHeader            = "author";
	private const string AuthorMailHeader        = "author-mail";
	private const string AuthorTimeHeader        = "author-time";
	private const string AuthorTimeZoneHeader    = "author-tz";

	private const string CommitterHeader         = "committer";
	private const string CommitterMailHeader     = "committer-mail";
	private const string CommitterTimeHeader     = "committer-time";
	private const string CommitterTimeZoneHeader = "committer-tz";

	private const string SummaryHeader           = "summary";
	private const string BoundaryHeader          = "boundary";
	private const string PreviousHeader          = "previous";
	private const string FileNameHeader          = "filename";

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

	private static readonly string[] Headers =
	[
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
	];

	#endregion

	private string ReadStringValue(string header)
	{
		Skip(header.Length + 1);
		return ReadLine();
	}

	private long ReadDateValue(string header)
	{
		Skip(header.Length + 1);
		return ReadLineAsInt64();
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

	private string? ReadPreviousValue()
	{
		if(!CheckValue(PreviousHeader)) return default;

		Skip(PreviousHeader.Length + 1);
		var previous = ReadString(40);
		SkipLine();
		return previous;
	}

	private static TimeSpan ParseTimeZoneOffset(string value)
	{
		static int Digit(char value)
		{
			var digit = value - '0';
			return digit is >= 0 and <= 9
				? digit
				: throw new FormatException($"Expected digit: {value}");
		}

		static int Sign(char value)
			=> value switch
			{
				'+' =>  1,
				'-' => -1,
				_ => throw new FormatException($"Unexpected time zone offset sign: {value}")
			};

		switch(value.Length)
		{
			case 1 when value[0] == 'Z':
				return TimeSpan.Zero;
			case 5:
				break;
			case 6:
				if(value[3] != ':')
				{
					throw new FormatException($"Unexpected time zone offset separator: {value[3]}");
				}
				break;
			default:
				throw new FormatException($"Unexpected time zone offset string lenth: {value.Length}");
		}
		var sign    = Sign(value[0]);
		var hours   = Digit(value[1]) * 10 + Digit(value[2]);
		var minutes = value.Length == 5
			? Digit(value[3]) * 10 + Digit(value[4])
			: Digit(value[4]) * 10 + Digit(value[5]);

		return new TimeSpan(sign * TimeSpan.TicksPerHour * hours + TimeSpan.TicksPerMinute * minutes);
	}

	private BlameUserInfo ReadAuthor()
	{
		var name  = ReadStringValue(AuthorHeader);
		var email = ReadStringValue(AuthorMailHeader);
		var time  = ReadDateValue  (AuthorTimeHeader);
		var tz    = ReadStringValue(AuthorTimeZoneHeader);

		var timestamp = new DateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(time).DateTime, ParseTimeZoneOffset(tz));
		return new(name, email, timestamp);
	}

	private BlameUserInfo ReadCommitter()
	{
		var name  = ReadStringValue(CommitterHeader);
		var email = ReadStringValue(CommitterMailHeader);
		var time  = ReadDateValue  (CommitterTimeHeader);
		var tz    = ReadStringValue(CommitterTimeZoneHeader);

		var timestamp = new DateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(time).DateTime, ParseTimeZoneOffset(tz));
		return new(name, email, timestamp);
	}

	private BlameCommit ParseCommitInfoCached(Dictionary<Sha1Hash, BlameCommit> cache)
	{
		Assert.IsNotNull(cache);

		var hash = ReadHash();
		SkipLine();

		if(cache.TryGetValue(hash, out var commit))
		{
			return commit;
		}

		var author     = ReadAuthor();
		var committer  = ReadCommitter();
		var summary    = ReadStringValue(SummaryHeader);
		var isBoundary = ReadBoundaryValue(); /* optional */
		var previous   = ReadPreviousValue(); /* optional */
		var fileName   = ReadStringValue(FileNameHeader);

		commit = new BlameCommit(
			hash, author, committer,
			summary, isBoundary, previous);
		cache.Add(hash, commit);
		return commit;
	}

	public BlameFile ParseBlameFile(string fileName)
	{
		var cache = new Dictionary<Sha1Hash, BlameCommit>(Sha1Hash.EqualityComparer);
		int lineN = 1;
		var prev  = default(BlameCommit);
		var hunks = new List<BlameHunk>();
		var lines = new List<BlameLine>();

		while(!IsAtEndOfString)
		{
			var commit = ParseCommitInfoCached(cache);
			Skip();
			if(IsAtEndOfString) break;

			var text = ReadLine(out var ending);
			var line = new BlameLine(commit, lineN++, text, ending);

			if(commit != prev)
			{
				if(prev is not null && lines.Count != 0)
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
			hunks.Add(new BlameHunk(prev!, lines));
			lines.Clear();
		}

		return new BlameFile(fileName, hunks);
	}
}
