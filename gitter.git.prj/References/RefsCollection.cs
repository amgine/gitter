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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	/// <summary>Collections of repository's references ("$GIT_DIR/refs/" cache).</summary>
	public sealed class RefsCollection : GitObject, IEnumerable<Reference>
	{
		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="RefsCollection"/> class.</summary>
		/// <param name="repository">Host repository.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		internal RefsCollection(Repository repository)
			: base(repository, false)
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

		/// <summary>Updates all cached references.</summary>
		public void Refresh()
		{
			var refs = Repository.Accessor.QueryReferences.Invoke(
				new QueryReferencesParameters(
					ReferenceType.LocalBranch |
					ReferenceType.RemoteBranch |
					ReferenceType.Tag));
			Heads.Refresh(refs.Heads);
			Remotes.Refresh(refs.Remotes);
			Tags.Refresh(refs.Tags);
		}

		/// <summary>Updates cached references of specified types.</summary>
		/// <param name="referenceTypes">Reference types to update.</param>
		public void Refresh(ReferenceType referenceTypes)
		{
			var refs = Repository.Accessor.QueryReferences.Invoke(
				new QueryReferencesParameters(referenceTypes));
			if((referenceTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
			{
				Heads.Refresh(refs.Heads);
			}
			if((referenceTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
			{
				Remotes.Refresh(refs.Remotes);
			}
			if((referenceTypes & ReferenceType.Tag) == ReferenceType.Tag)
			{
				Tags.Refresh(refs.Tags);
			}
		}

		/// <summary>
		/// Updates all references of <see cref="ReferenceType.LocalBranch"/> or
		/// <see cref="ReferenceType.RemoteBranch"/> type.
		/// </summary>
		public void RefreshBranches()
		{
			var refs = Repository.Accessor.QueryReferences.Invoke(
				new QueryReferencesParameters(
					ReferenceType.LocalBranch |
					ReferenceType.RemoteBranch));
			Heads.Refresh(refs.Heads);
			Remotes.Refresh(refs.Remotes);
		}

		/// <summary>
		/// Updates all references of <see cref="ReferenceType.Tag"/> type.
		/// </summary>
		public void RefreshTags()
		{
			var refs = Repository.Accessor.QueryReferences.Invoke(
				new QueryReferencesParameters(ReferenceType.Tag));
			Tags.Refresh(refs.Tags);
		}

		#endregion

		#region Get()

		/// <summary>
		/// Returns a reference with a given name or <c>null</c>,
		/// if such reference does not exist.
		/// </summary>
		/// <param name="name">Reference name.</param>
		/// <returns>Reference with a given name or <c>null</c>, if such reference does not exist</returns>
		public Reference TryGetReference(string name)
		{
			var head = Heads.TryGetItem(name);
			if(head != null) return head;
			var remote = Remotes.TryGetItem(name);
			if(remote != null) return remote;
			var tag = Tags.TryGetItem(name);
			if(tag != null) return tag;
			return default;
		}

		/// <summary>Gets the list of unmerged branches.</summary>
		/// <returns>List of unmerged branches.</returns>
		public IReadOnlyList<BranchBase> GetUnmergedBranches()
		{
			var refs = Repository.Accessor.QueryBranches.Invoke(
				new QueryBranchesParameters(QueryBranchRestriction.All, BranchQueryMode.NoMerged));
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

		/// <summary>Gets the list of merged branches.</summary>
		/// <returns>List of merged branches.</returns>
		public IReadOnlyList<BranchBase> GetMergedBranches()
		{
			var refs = Repository.Accessor.QueryBranches.Invoke(
				new QueryBranchesParameters(QueryBranchRestriction.All, BranchQueryMode.Merged));
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
		        new QueryBranchesParameters(QueryBranchRestriction.All, BranchQueryMode.Contains, revision.Pointer));
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
					if(branch != null)
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
					if(branch != null) res.Add(branch);
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
			Verify.Argument.IsNotNull(refs, nameof(refs));

			if(refs.Heads != null)
			{
				Heads.Load(refs.Heads);
			}
			if(refs.Remotes != null)
			{
				Remotes.Load(refs.Remotes);
			}
			if(refs.Tags != null)
			{
				Tags.Load(refs.Tags);
			}
		}

		#endregion

		#region IEnumerable<Reference>

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator&lt;Reference&gt;"/> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Reference> GetEnumerator()
		{
			foreach(var head in Heads)
			{
				yield return head;
			}
			foreach(var remote in Remotes)
			{
				yield return remote;
			}
			foreach(var tag in Tags)
			{
				yield return tag;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
