#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Git.AccessLayer;

/// <summary>Collections of repository's references ("$GIT_DIR/refs/" cache).</summary>
public sealed class RefsCollection : GitObject, IReadOnlyCollection<Reference>
{
	#region .ctor

	/// <summary>Initializes a new instance of the <see cref="RefsCollection"/> class.</summary>
	/// <param name="repository">Host repository.</param>
	/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
	internal RefsCollection(Repository repository)
		: base(repository)
	{
		Heads   = new RefsHeadsCollection(repository);
		Remotes = new RefsRemotesCollection(repository);
		Tags    = new RefsTagsCollection(repository);
	}

	#endregion

	#region Properties

	/// <summary>Gets the collection of repository's heads.</summary>
	/// <value>Collection of repository's heads.</value>
	public RefsHeadsCollection Heads { get; }

	/// <summary>Gets the collection of repository's remote heads.</summary>
	/// <value>Collection of repository's remote heads.</value>
	public RefsRemotesCollection Remotes { get; }

	/// <summary>Gets the collection of repository's tags.</summary>
	/// <value>Collection of repository's tags.</value>
	public RefsTagsCollection Tags { get; }

	/// <summary>Returns total reference count.</summary>
	/// <value>Total reference count.</value>
	/// <remarks>Only heads, remotes and tags are counted.</remarks>
	public int Count
	{
		get
		{
			int count = 0;
			lock(Heads.SyncRoot)
			{
				count += Heads.Count;
			}
			lock(Remotes.SyncRoot)
			{
				count += Remotes.Count;
			}
			lock(Tags.SyncRoot)
			{
				count += Tags.Count;
			}
			return count;
		}
	}

	#endregion

	#region Refresh()

	private static QueryReferencesRequest GetQueryReferencesRequest() => new(
		ReferenceType.LocalBranch |
		ReferenceType.RemoteBranch |
		ReferenceType.Tag);

	private void Refresh(ReferencesData refs)
	{
		Assert.IsNotNull(refs);

		if(refs.Heads   is not null) Heads  .Refresh(refs.Heads);
		if(refs.Remotes is not null) Remotes.Refresh(refs.Remotes);
		if(refs.Tags    is not null) Tags   .Refresh(refs.Tags);
	}

	private void RefreshRequestedTypes(ReferencesData refs, ReferenceType referenceTypes)
	{
		Assert.IsNotNull(refs);

		static bool Requested(ReferenceType flags, ReferenceType flag)
			=> (flags & flag) == flag;

		if(Requested(referenceTypes, ReferenceType.LocalBranch) && refs.Heads is not null)
		{
			Heads.Refresh(refs.Heads);
		}
		if(Requested(referenceTypes, ReferenceType.RemoteBranch) && refs.Remotes is not null)
		{
			Remotes.Refresh(refs.Remotes);
		}
		if(Requested(referenceTypes, ReferenceType.Tag) && refs.Tags is not null)
		{
			Tags.Refresh(refs.Tags);
		}
	}

	/// <summary>Updates all cached references.</summary>
	public void Refresh()
	{
		var refs = Repository.Accessor.QueryReferences.Invoke(
			GetQueryReferencesRequest());

		Refresh(refs);
	}

	/// <summary>Updates all cached references.</summary>
	public async Task RefreshAsync(CancellationToken cancellationToken = default)
	{
		var refs = await Repository.Accessor.QueryReferences
			.InvokeAsync(GetQueryReferencesRequest(), cancellationToken: cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		Refresh(refs);
	}

	/// <summary>Updates cached references of specified types.</summary>
	/// <param name="referenceTypes">Reference types to update.</param>
	public void Refresh(ReferenceType referenceTypes)
	{
		var refs = Repository.Accessor.QueryReferences.Invoke(
			new QueryReferencesRequest(referenceTypes));
		RefreshRequestedTypes(refs, referenceTypes);
	}

	/// <summary>Updates cached references of specified types.</summary>
	/// <param name="referenceTypes">Reference types to update.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task RefreshAsync(ReferenceType referenceTypes, CancellationToken cancellationToken = default)
	{
		var refs = await Repository.Accessor.QueryReferences
			.InvokeAsync(new QueryReferencesRequest(referenceTypes), cancellationToken: cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		RefreshRequestedTypes(refs, referenceTypes);
	}

	/// <summary>
	/// Updates all references of <see cref="ReferenceType.LocalBranch"/> or
	/// <see cref="ReferenceType.RemoteBranch"/> type.
	/// </summary>
	public void RefreshBranches()
		=> Refresh(ReferenceType.Branch);

	/// <summary>
	/// Updates all references of <see cref="ReferenceType.LocalBranch"/> or
	/// <see cref="ReferenceType.RemoteBranch"/> type.
	/// </summary>
	public Task RefreshBranchesAsync(CancellationToken cancellationToken = default)
		=> RefreshAsync(ReferenceType.Branch, cancellationToken);

	/// <summary>
	/// Updates all references of <see cref="ReferenceType.Tag"/> type.
	/// </summary>
	public void RefreshTags()
		=> Refresh(ReferenceType.Tag);

	/// <summary>
	/// Updates all references of <see cref="ReferenceType.Tag"/> type.
	/// </summary>
	public Task RefreshTagsAsync(CancellationToken cancellationToken = default)
		=> RefreshAsync(ReferenceType.Tag, cancellationToken);

	#endregion

	#region Get()

	/// <summary>
	/// Returns a reference with a given name or <c>null</c>,
	/// if such reference does not exist.
	/// </summary>
	/// <param name="name">Reference name.</param>
	/// <returns>Reference with a given name or <c>null</c>, if such reference does not exist</returns>
	public Reference? TryGetReference(string name)
		=> (Reference?)Heads.TryGetItem(name)
		?? (Reference?)Remotes.TryGetItem(name)
		?? (Reference?)Tags.TryGetItem(name);

	/// <summary>Gets the list of unmerged branches.</summary>
	/// <returns>List of unmerged branches.</returns>
	public IReadOnlyList<BranchBase> GetUnmergedBranches()
	{
		var refs = Repository.Accessor.QueryBranches.Invoke(
			new QueryBranchesRequest(QueryBranchRestriction.All, BranchQueryMode.NoMerged));
		var heads = refs.Heads;
		var remotes = refs.Remotes;
		var count = heads.Count + remotes.Count;
		if(count == 0)
		{
			return Preallocated<BranchBase>.EmptyArray;
		}
		var res = new List<BranchBase>(count);
		lock(Heads.SyncRoot)
		{
			foreach(var head in heads)
			{
				var branch = Heads.TryGetItem(head.Name);
				if(branch is not null) res.Add(branch);
			}
		}
		lock(Remotes.SyncRoot)
		{
			foreach(var remote in remotes)
			{
				var branch = Remotes.TryGetItem(remote.Name);
				if(branch is not null) res.Add(branch);
			}
		}
		return res;
	}

	/// <summary>Gets the list of merged branches.</summary>
	/// <returns>List of merged branches.</returns>
	public IReadOnlyList<BranchBase> GetMergedBranches()
	{
		var refs = Repository.Accessor.QueryBranches.Invoke(
			new QueryBranchesRequest(QueryBranchRestriction.All, BranchQueryMode.Merged));
		var heads = refs.Heads;
		var remotes = refs.Remotes;
		var count = heads.Count + remotes.Count;
		if(count == 0)
		{
			return Preallocated<BranchBase>.EmptyArray;
		}
		var res = new List<BranchBase>(count);
		lock(Heads.SyncRoot)
		{
			foreach(var head in heads)
			{
				var branch = Heads.TryGetItem(head.Name);
				if(branch != null) res.Add(branch);
			}
		}
		lock(Remotes.SyncRoot)
		{
			foreach(var remote in remotes)
			{
				var branch = Remotes.TryGetItem(remote.Name);
				if(branch != null) res.Add(branch);
			}
		}
		return res;
	}

	/// <summary>Gets the list of branches, containing specified <paramref name="revision"/>.</summary>
	/// <param name="revision">Revision which must be present in any resulting branch.</param>
	/// <returns>List of branches, containing specified <paramref name="revision"/>.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="revision"/> == <c>null</c>.</exception>
	public IReadOnlyList<BranchBase> GetBranchesContaining(IRevisionPointer revision)
	{
		Verify.Argument.IsValidRevisionPointer(revision, Repository, nameof(revision));

		var refs = Repository.Accessor.QueryBranches.Invoke(
		    new QueryBranchesRequest(QueryBranchRestriction.All, BranchQueryMode.Contains, revision.Pointer));
		var heads = refs.Heads;
		var remotes = refs.Remotes;
		var count = heads.Count + remotes.Count;
		if(count == 0)
		{
			return Preallocated<BranchBase>.EmptyArray;
		}
		var res = new List<BranchBase>(count);
		lock(Heads.SyncRoot)
		{
			foreach(var head in heads)
			{
				var branch = Heads.TryGetItem(head.Name);
				if(branch is not null)
				{
					res.Add(branch);
				}
			}
		}
		lock(Remotes.SyncRoot)
		{
			foreach(var remote in remotes)
			{
				var branch = Remotes.TryGetItem(remote.Name);
				if(branch is not null) res.Add(branch);
			}
		}
		return res;
	}

	#endregion

	#region Load()

	/// <summary>Loads the specified references.</summary>
	/// <param name="refs">References data.</param>
	internal void Load(ReferencesData refs)
	{
		Verify.Argument.IsNotNull(refs);

		if(refs.Heads   is not null) Heads  .Load(refs.Heads);
		if(refs.Remotes is not null) Remotes.Load(refs.Remotes);
		if(refs.Tags    is not null) Tags   .Load(refs.Tags);
	}

	#endregion

	#region IEnumerable<Reference>

	/// <summary>Returns an enumerator that iterates through a collection.</summary>
	/// <returns>
	/// An <see cref="T:System.Collections.IEnumerator&lt;Reference&gt;"/> object that can be used to iterate through the collection.
	/// </returns>
	public IEnumerator<Reference> GetEnumerator()
	{
		foreach(var head in Heads)     yield return head;
		foreach(var remote in Remotes) yield return remote;
		foreach(var tag in Tags)       yield return tag;
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>
	/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
	/// </returns>
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> GetEnumerator();

	#endregion
}
