namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public sealed class RepositoryManagerService
	{
		private const int MaxRecentRepositories = 10;

		private readonly RepositoryCollection _recent;
		private readonly RepositoryCollection _local;

		public RepositoryManagerService()
		{
			_recent = new RepositoryCollection();
			_local = new RepositoryCollection();
		}

		public RepositoryCollection RecentRepositories
		{
			get { return _recent; }
		}

		public RepositoryCollection LocalRepositories
		{
			get { return _local; }
		}
	}
}
