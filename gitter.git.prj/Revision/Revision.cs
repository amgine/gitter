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
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Git.AccessLayer;

public sealed partial class Revision : GitObject, IRevisionPointer
{
	private Sha1Hash _treeHash;

	internal Revision(Repository repository, Sha1Hash hash)
		: base(repository)
	{
		References = [];
		Hash       = hash;
		HashString = hash.ToString();
	}

	public void Load()
	{
		var revisionData = Repository.Accessor.QueryRevision
			.Invoke(new QueryRevisionRequest(Hash));
		ObjectFactories.UpdateRevision(this, revisionData);
	}

	public async Task LoadAsync(CancellationToken cancellationToken = default)
	{
		var revisionData = await Repository.Accessor.QueryRevision
			.InvokeAsync(new QueryRevisionRequest(Hash), cancellationToken: cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		ObjectFactories.UpdateRevision(this, revisionData);
	}

	public Many<Revision> Parents { get; internal set; }

	public RevisionReferencesCollection References { get; }

	public bool IsCurrent => Repository.Head.Revision == this;

	public bool IsLoaded { get; internal set; }

	public Sha1Hash Hash { get; }

	public string HashString { get; } = default!;

	public Sha1Hash TreeHash
	{
		get => _treeHash;
		internal set
		{
			if(!IsLoaded)
			{
				_treeHash = value;
				TreeHashString = value.ToString();
			}
			else
			{
				if(_treeHash != value)
				{
					_treeHash = value;
					TreeHashString = value.ToString();
				}
			}
		}
	}

	public string TreeHashString { get; private set; } = default!;

	public User Author { get; internal set; } = default!;

	public DateTimeOffset AuthorDate { get; internal set; }

	public User Committer { get; internal set; } = default!;

	public DateTimeOffset CommitDate { get; internal set; }

	public string Subject { get; internal set; } = default!;

	public string Body { get; internal set; } = default!;

	ReferenceType IRevisionPointer.Type => ReferenceType.Revision;

	string IRevisionPointer.Pointer => HashString;

	string IRevisionPointer.FullName => HashString;

	bool IRevisionPointer.IsDeleted => false;

	Revision IRevisionPointer.Dereference() => this;

	ValueTask<Revision?> IRevisionPointer.DereferenceAsync(CancellationToken cancellationToken) => new(this);

	/// <inheritdoc/>
	public override string ToString()
		=> string.Format(CultureInfo.InvariantCulture, "{0}: {1}", Hash.ToString(7), Subject);
}
