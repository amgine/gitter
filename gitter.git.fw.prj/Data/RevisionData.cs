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

namespace gitter.Git.AccessLayer;

using System;

using gitter.Framework;

public sealed class RevisionData : INamedObject
{
	#region Data

	private Hash _treeHash;
	private RevisionData[] _parents;
	private RevisionData[] _children;
	private string _subject;
	private string _body;
	private DateTimeOffset _commitDate;
	private string _committerName;
	private string _committerEmail;
	private DateTimeOffset _authorDate;
	private string _authorName;
	private string _authorEmail;

	#endregion

	#region .ctor

	public RevisionData(Hash commitHash)
	{
		CommitHash = commitHash;
		Fields     = RevisionField.CommitHash;
	}

	#endregion

	#region Properties

	public Hash CommitHash { get; }

	public Hash TreeHash
	{
		get => _treeHash;
		set
		{
			_treeHash = value;
			Fields |= RevisionField.TreeHash;
		}
	}

	public RevisionData[] Parents
	{
		get => _parents;
		set
		{
			_parents = value;
			Fields |= RevisionField.Parents;
		}
	}

	public RevisionData[] Children
	{
		get => _children;
		set
		{
			_children = value;
			Fields |= RevisionField.Children;
		}
	}

	public string Subject
	{
		get => _subject;
		set
		{
			_subject = value;
			Fields |= RevisionField.Subject;
		}
	}

	public string Body
	{
		get => _body;
		set
		{
			_body = value;
			Fields |= RevisionField.Body;
		}
	}

	public DateTimeOffset CommitDate
	{
		get => _commitDate;
		set
		{
			_commitDate = value;
			Fields |= RevisionField.CommitDate;
		}
	}

	public string CommitterName
	{
		get => _committerName;
		set
		{
			_committerName = value;
			Fields |= RevisionField.CommitterName;
		}
	}

	public string CommitterEmail
	{
		get => _committerEmail;
		set
		{
			_committerEmail = value;
			Fields |= RevisionField.CommitterEmail;
		}
	}


	public DateTimeOffset AuthorDate
	{
		get => _authorDate;
		set
		{
			_authorDate = value;
			Fields |= RevisionField.AuthorDate;
		}
	}

	public string AuthorName
	{
		get => _authorName;
		set
		{
			_authorName = value;
			Fields |= RevisionField.AuthorName;
		}
	}

	public string AuthorEmail
	{
		get => _authorEmail;
		set
		{
			_authorEmail = value;
			Fields |= RevisionField.AuthorEmail;
		}
	}

	public RevisionField Fields { get; private set; }

	#endregion

	#region Methods

	public bool DataValid(RevisionField data)
		=> (Fields & data) == data;

	string INamedObject.Name => CommitHash.ToString();

	public override string ToString() => CommitHash.ToString();

	#endregion
}
