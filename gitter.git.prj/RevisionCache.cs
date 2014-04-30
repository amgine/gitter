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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	/// <summary>Contains cached <see cref="Revision"/> objects.</summary>
	public sealed class RevisionCache : GitObject, IEnumerable<Revision>
	{
		#region Data

		private readonly Dictionary<Hash, Revision> _revisions;
		private readonly object _syncRoot;

		#endregion

		#region .ctor

		/// <summary>Initializes <see cref="RevisionCache"/>.</summary>
		/// <param name="repository">Repository.</param>
		public RevisionCache(Repository repository)
			: base(repository)
		{
			_revisions = new Dictionary<Hash, Revision>(Hash.EqualityComparer);
			_syncRoot  = new object();
		}

		#endregion

		/// <summary>Returns cross-thread synchronization object.</summary>
		/// <value>Cross-thread synchronization object.</value>
		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		/// <summary>Returns revision with specified SHA1.</summary>
		/// <param name="sha1">SHA-1 of required revision.</param>
		/// <returns>Revision with specified SHA-1.</returns>
		/// <exception cref="ArgumentException">Invalid SHA-1 expression.</exception>
		/// <exception cref="GitException">Revision does not exist.</exception>
		/// <remarks>If revision is not present in cache, it will be queried from git repo.</remarks>
		public Revision this[Hash sha1]
		{
			get
			{
				lock(SyncRoot)
				{
					Revision revision;
					if(_revisions.TryGetValue(sha1, out revision))
					{
						if(!revision.IsLoaded)
						{
							revision.Load();
						}
					}
					else
					{
						var revisionData = Repository.Accessor.QueryRevision.Invoke(
							new QueryRevisionParameters(sha1));
						revision = ObjectFactories.CreateRevision(Repository, revisionData);
					}
					return revision;
				}
			}
		}

		/// <summary>Returns revision with specified SHA1 or <c>null</c> if such revision does not exist.</summary>
		/// <param name="sha1">SHA-1 of required revision.</param>
		/// <returns>Revision with specified SHA-1 or <c>null</c> if such revision does not exist.</returns>
		/// <remarks>If revision is not present in cache, it will be queried from git repo.</remarks>
		public Revision TryGetRevision(Hash sha1)
		{
			lock(SyncRoot)
			{
				Revision revision;
				if(_revisions.TryGetValue(sha1, out revision))
				{
					if(!revision.IsLoaded)
					{
						revision.Load();
					}
				}
				else
				{
					RevisionData revisionData = null;
					try
					{
						revisionData = Repository.Accessor.QueryRevision.Invoke(
							new QueryRevisionParameters(sha1));
					}
					catch(GitException)
					{
						return null;
					}
					revision = ObjectFactories.CreateRevision(Repository, revisionData);
				}
				return revision;
			}
		}

		/// <summary>Returns revision with specified SHA1 or <c>null</c> if such revision is not found in cache.</summary>
		/// <param name="sha1">SHA-1 of required revision.</param>
		/// <returns>Revision with specified SHA-1 or <c>null</c> if such revision is not found in cache.</returns>
		/// <remarks>Does not query revision from git repository if it is not present in cache..</remarks>
		public Revision TryGetRevisionFromCacheOnly(Hash sha1)
		{
			lock(SyncRoot)
			{
				Revision revision;
				if(_revisions.TryGetValue(sha1, out revision))
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
			get
			{
				lock(SyncRoot)
				{
					return _revisions.Count;
				}
			}
		}

		internal Revision GetOrCreateRevision(Hash sha1)
		{
			Revision revision;
			if(!_revisions.TryGetValue(sha1, out revision))
			{
				revision = new Revision(Repository, sha1);
				_revisions.Add(sha1, revision);
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
			Verify.Argument.IsNotNull(data, "data");

			var res = new Revision[data.Count];
			lock(SyncRoot)
			{
				for(int i = 0; i < data.Count; ++i)
				{
					var revisionData = data[i];
					Revision revision;
					if(_revisions.TryGetValue(revisionData.SHA1, out revision))
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
					res[i] = revision;
				}
			}
			return res;
		}

		#region IEnumerable<Revision>

		public IEnumerator<Revision> GetEnumerator()
		{
			return _revisions.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _revisions.Values.GetEnumerator();
		}

		#endregion
	}
}
