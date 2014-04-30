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

	public sealed class RevisionLog
	{
		#region Data

		private readonly Repository _repository;
		private readonly IList<Revision> _revisions;
		private readonly Dictionary<Revision, IList<Revision>> _parents;

		#endregion

		#region .ctor

		public RevisionLog(Repository repository, IList<Revision> revisions)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(revisions, "revisions");

			_repository = repository;
			_revisions  = revisions;
		}

		public RevisionLog(Repository repository, IList<Revision> revisions, Dictionary<Revision, IList<Revision>> parents)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(revisions, "revisions");

			_repository = repository;
			_revisions  = revisions;
			_parents    = parents;
		}

		#endregion

		#region Methods

		public IList<Revision> GetParents(Revision revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");

			if(_parents == null)
			{
				return revision.Parents;
			}
			else
			{
				IList<Revision> parents;
				if(_parents.TryGetValue(revision, out parents))
				{
					return parents;
				}
				else
				{
					return new Revision[0];
				}
			}
		}

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		public IList<Revision> Revisions
		{
			get { return _revisions; }
		}

		public int RevisionsCount
		{
			get { return _revisions.Count; }
		}

		#endregion
	}
}
