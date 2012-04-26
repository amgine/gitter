namespace gitter.Git
{
	using System;

	public sealed class RepositoryLogSource : BaseLogSource
	{
		private readonly Repository _repository;

		public RepositoryLogSource(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");

			_repository = repository;
		}

		public Repository Repository
		{
			get { return _repository; }
		}

		protected override RevisionLog GetLogCore(LogOptions options)
		{
			if(Repository.IsEmpty)
			{
				return new RevisionLog(Repository, new Revision[0]);
			}
			else
			{
				var log = Repository.Accessor.QueryRevisions(options.GetLogParameters());
				var res = Repository.Revisions.Resolve(log);
				return new RevisionLog(Repository, res);
			}
		}
	}
}
