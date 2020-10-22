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

	public sealed class RevisionLog
	{
		private readonly Dictionary<Revision, IReadOnlyList<Revision>> _parents;

		public RevisionLog(Repository repository, IReadOnlyList<Revision> revisions)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));
			Verify.Argument.IsNotNull(revisions, nameof(revisions));

			Repository = repository;
			Revisions  = revisions;
		}

		public RevisionLog(Repository repository, IReadOnlyList<Revision> revisions, Dictionary<Revision, IReadOnlyList<Revision>> parents)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));
			Verify.Argument.IsNotNull(revisions, nameof(revisions));

			Repository = repository;
			Revisions  = revisions;
			_parents   = parents;
		}

		public IReadOnlyList<Revision> GetParents(Revision revision)
		{
			Verify.Argument.IsNotNull(revision, nameof(revision));

			if(_parents == null)
			{
				return revision.Parents;
			}
			else
			{
				return _parents.TryGetValue(revision, out var parents)
					? parents
					: Preallocated<Revision>.EmptyArray;
			}
		}

		public Repository Repository { get; }

		public IReadOnlyList<Revision> Revisions { get; }

		public int RevisionsCount => Revisions.Count;
	}
}
