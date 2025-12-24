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
using System.Collections.Generic;

using gitter.Framework;
using gitter.Git.AccessLayer;

/// <summary>Contains cached <see cref="Revision"/> objects.</summary>
/// <param name="repository">Repository.</param>
public sealed class RevisionCache(Repository repository)
	: GitObject(repository), IEnumerable<Revision>
{
	private readonly Dictionary<Sha1Hash, Revision> _revisions = new(Sha1Hash.EqualityComparer);

	/// <summary>Returns cross-thread synchronization object.</summary>
	/// <value>Cross-thread synchronization object.</value>
	public LockType SyncRoot { get; } = new();

	/// <summary>Returns revision with specified SHA1.</summary>
	/// <param name="hash">SHA-1 of required revision.</param>
	/// <returns>Revision with specified SHA-1.</returns>
	/// <exception cref="ArgumentException">Invalid SHA-1 expression.</exception>
	/// <exception cref="GitException">Revision does not exist.</exception>
	/// <remarks>If revision is not present in cache, it will be queried from git repo.</remarks>
	public Revision this[Sha1Hash hash]
	{
		get
		{
			lock(SyncRoot)
			{
				if(_revisions.TryGetValue(hash, out var revision))
				{
					if(!revision.IsLoaded)
					{
						revision.Load();
					}
					return revision;
				}
				var revisionData = Repository.Accessor.QueryRevision
					.Invoke(new QueryRevisionRequest(hash));
				return ObjectFactories.CreateRevision(Repository, revisionData);
			}
		}
	}

	/// <summary>Returns revision with specified SHA1 or <c>null</c> if such revision does not exist.</summary>
	/// <param name="hash">SHA-1 of required revision.</param>
	/// <returns>Revision with specified SHA-1 or <c>null</c> if such revision does not exist.</returns>
	/// <remarks>If revision is not present in cache, it will be queried from git repo.</remarks>
	public Revision? TryGetRevision(Sha1Hash hash)
	{
		lock(SyncRoot)
		{
			if(_revisions.TryGetValue(hash, out var revision))
			{
				if(!revision.IsLoaded)
				{
					revision.Load();
				}
				return revision;
			}
			RevisionData? revisionData;
			try
			{
				revisionData = Repository.Accessor.QueryRevision.Invoke(
					new QueryRevisionRequest(hash));
			}
			catch(GitException)
			{
				return default;
			}
			return ObjectFactories.CreateRevision(Repository, revisionData);
		}
	}

	/// <summary>Returns revision with specified SHA1 or <c>null</c> if such revision is not found in cache.</summary>
	/// <param name="hash">SHA-1 of required revision.</param>
	/// <returns>Revision with specified SHA-1 or <c>null</c> if such revision is not found in cache.</returns>
	/// <remarks>Does not query revision from git repository if it is not present in cache.</remarks>
	public Revision? TryGetRevisionFromCacheOnly(Sha1Hash hash)
	{
		lock(SyncRoot)
		{
			if(_revisions.TryGetValue(hash, out var revision))
			{
				if(!revision.IsLoaded)
				{
					revision.Load();
				}
			}
			return revision;
		}
	}

	/// <summary>Returns count of cached revisions.</summary>
	/// <value>Cached revisions count.</value>
	public int Count
	{
		get { lock(SyncRoot) return _revisions.Count; }
	}

	internal Revision GetOrCreateRevision(Sha1Hash hash)
	{
		if(!_revisions.TryGetValue(hash, out var revision))
		{
			_revisions.Add(hash, revision = new(Repository, hash));
		}
		return revision;
	}

	private Revision ResolveCore(RevisionData revisionData)
	{
		Assert.IsNotNull(revisionData);

		if(_revisions.TryGetValue(revisionData.CommitHash, out var revision))
		{
			if(!revision.IsLoaded)
			{
				ObjectFactories.UpdateRevision(revision, revisionData);
			}
		}
		else
		{
			revision = ObjectFactories.CreateRevision(Repository, revisionData);
		}
		return revision;
	}

	/// <summary>
	/// Transforms list of <see cref="RevisionData"/> into array of <see cref="Revision"/> objects
	/// creating, caching and updating revisions if necessary.
	/// </summary>
	/// <param name="data">List of <see cref="RevisionData"/> objects.</param>
	/// <returns>Array of corresponding <see cref="Revision"/> objects.</returns>
	internal Revision[] Resolve(IList<RevisionData> data)
	{
		Verify.Argument.IsNotNull(data);

		var count = data.Count;
		if(count == 0) return Preallocated<Revision>.EmptyArray;
		var res = new Revision[count];
		lock(SyncRoot)
		{
			for(int i = 0; i < count; ++i)
			{
				res[i] = ResolveCore(data[i]);
			}
		}
		return res;
	}

	/// <summary>
	/// Transforms list of <see cref="RevisionData"/> into array of <see cref="Revision"/> objects
	/// creating, caching and updating revisions if necessary.
	/// </summary>
	/// <param name="data">List of <see cref="RevisionData"/> objects.</param>
	/// <returns>Array of corresponding <see cref="Revision"/> objects.</returns>
	internal Revision[] Resolve(IReadOnlyList<RevisionData> data)
	{
		Verify.Argument.IsNotNull(data);

		var count = data.Count;
		if(count == 0) return Preallocated<Revision>.EmptyArray;
		var res = new Revision[count];
		lock(SyncRoot)
		{
			for(int i = 0; i < count; ++i)
			{
				res[i] = ResolveCore(data[i]);
			}
		}
		return res;
	}

	#region IEnumerable<Revision>

	public IEnumerator<Revision> GetEnumerator()
		=> _revisions.Values.GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> _revisions.Values.GetEnumerator();

	#endregion
}
