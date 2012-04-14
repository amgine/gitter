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
			if(_repository.IsEmpty) return RevisionLog.Empty;

			var log = Repository.Accessor.QueryRevisions(options.GetLogParameters());
			var res = _repository.Revisions.Resolve(log);
			return new RevisionLog(res);
		}
	}
}
