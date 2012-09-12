namespace gitter.Git
{
	using System;

	public sealed class RepositoryLogSource : LogSourceBase
	{
		private readonly Repository _repository;

		public RepositoryLogSource(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		public override Repository Repository
		{
			get { return _repository; }
		}

		protected override RevisionLog GetLogCore(LogOptions options)
		{
			Assert.IsNotNull(options);

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
