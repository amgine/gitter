namespace gitter.Framework.Services
{
	using System;

	public sealed class RepositoryLinkEventArgs : EventArgs
	{
		private readonly RepositoryLink _repository;

		public RepositoryLinkEventArgs(RepositoryLink repository)
		{
			_repository = repository;
		}

		public RepositoryLink Repository
		{
			get { return _repository; }
		}
	}
}
