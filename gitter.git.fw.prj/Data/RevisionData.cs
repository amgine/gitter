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

public sealed class RevisionData(Sha1Hash commitHash) : INamedObject
{
	#region Data

	private Sha1Hash _treeHash;
	private Many<RevisionData> _parents;
	private Many<RevisionData> _children;
	private string _subject = default!;
	private string _body = default!;
	private DateTimeOffset _commitDate;
	private string _committerName = default!;
	private string _committerEmail = default!;
	private DateTimeOffset _authorDate;
	private string _authorName = default!;
	private string _authorEmail = default!;

	#endregion

	#region Properties

	public Sha1Hash CommitHash { get; } = commitHash;

	public Sha1Hash TreeHash
	{
		get => _treeHash;
		set => SetValue(ref _treeHash, value, RevisionField.TreeHash);
	}

	public Many<RevisionData> Parents
	{
		get => _parents;
		set => SetValue(ref _parents, value, RevisionField.Parents);
	}

	public Many<RevisionData> Children
	{
		get => _children;
		set => SetValue(ref _children, value, RevisionField.Children);
	}

	public string Subject
	{
		get => _subject;
		set => SetValue(ref _subject, value, RevisionField.Subject);
	}

	public string Body
	{
		get => _body;
		set => SetValue(ref _body, value, RevisionField.Body);
	}

	public DateTimeOffset CommitDate
	{
		get => _commitDate;
		set => SetValue(ref _commitDate, value, RevisionField.CommitDate);
	}

	public string CommitterName
	{
		get => _committerName;
		set => SetValue(ref _committerName, value, RevisionField.CommitterName);
	}

	public string CommitterEmail
	{
		get => _committerEmail;
		set => SetValue(ref _committerEmail, value, RevisionField.CommitterEmail);
	}

	public DateTimeOffset AuthorDate
	{
		get => _authorDate;
		set => SetValue(ref _authorDate, value, RevisionField.AuthorDate);
	}

	public string AuthorName
	{
		get => _authorName;
		set => SetValue(ref _authorName, value, RevisionField.AuthorName);
	}

	public string AuthorEmail
	{
		get => _authorEmail;
		set => SetValue(ref _authorEmail, value, RevisionField.AuthorEmail);
	}

	private void SetValue<T>(ref T fieldRef, T value, RevisionField field)
	{
		fieldRef = value;
		Fields |= field;
	}

	public RevisionField Fields { get; private set; } = RevisionField.CommitHash;

	#endregion

	#region Methods

	public bool HasData(RevisionField data)
		=> (Fields & data) == data;

	string INamedObject.Name => CommitHash.ToString();

	public override string ToString() => CommitHash.ToString();

	#endregion
}
