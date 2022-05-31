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

namespace gitter.Git;

using System;

/// <summary>Represents a commit, referenced by a blame output line.</summary>
public sealed class BlameCommit
{
	public BlameCommit(
		Hash hash,
		string author,    string authorEmail,    DateTimeOffset authorDate, string authorTimeZone,
		string committer, string committerEmail, DateTimeOffset commitDate, string committerTimeZone,
		string summary, bool isBoundary, string previous)
	{
		Hash              = hash;
		Author            = author;
		AuthorEmail       = authorEmail;
		AuthorDate        = authorDate;
		AuthorTimeZone    = authorTimeZone;
		Committer         = committer;
		CommitterEmail    = committerEmail;
		CommitDate        = commitDate;
		CommitterTimeZone = committerTimeZone;
		Summary           = summary;
		IsBoundary        = isBoundary;
		Previous          = previous;
	}

	public Hash Hash { get; }

	public string Summary { get; }

	public string Author { get; }

	public string AuthorTimeZone { get; }

	public DateTimeOffset AuthorDate { get; }

	public string AuthorEmail { get; }

	public string Committer { get; }

	public string CommitterTimeZone { get; }

	public string CommitterEmail { get; }

	public DateTimeOffset CommitDate { get; }

	public bool IsBoundary { get; }

	public string Previous { get; }

	public override string ToString() =>Summary;
}
