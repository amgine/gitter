namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class RevisionLog
	{
		private readonly Repository _repository;
		private readonly IList<Revision> _revisions;

		public RevisionLog(Repository repository, IList<Revision> revisions)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			if(revisions == null) throw new ArgumentNullException("revisions");

			_repository = repository;
			_revisions = revisions;
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
