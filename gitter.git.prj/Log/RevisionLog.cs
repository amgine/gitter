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
			if(repository == null) throw new ArgumentNullException("repository");
			if(revisions == null) throw new ArgumentNullException("revisions");

			_repository = repository;
			_revisions = revisions;
		}

		public RevisionLog(Repository repository, IList<Revision> revisions, Dictionary<Revision, IList<Revision>> parents)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(revisions == null) throw new ArgumentNullException("revisions");

			_repository = repository;
			_revisions = revisions;
			_parents = parents;
		}

		#endregion

		public IList<Revision> GetParents(Revision revision)
		{
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
	}
}
