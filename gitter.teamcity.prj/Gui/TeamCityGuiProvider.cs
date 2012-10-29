namespace gitter.TeamCity.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.TeamCity.Gui.Views;

	sealed class TeamCityGuiProvider : IGuiProvider
	{
		private readonly IRepository _repository;
		private readonly TeamCityServiceContext _service;
		private RepositoryExplorer _repositoryExplorer;

		public TeamCityGuiProvider(IRepository repository, TeamCityServiceContext svc)
		{
			_repository = repository;
			_service = svc;
		}

		public IRepository Repository
		{
			get { return _repository; }
		}

		public TeamCityServiceContext ServiceContext
		{
			get { return _service; }
		}

		public void AttachToEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_repositoryExplorer = new RepositoryExplorer(environment, this);
			environment.ProvideRepositoryExplorerItem(_repositoryExplorer.RootItem);
		}

		public void DetachFromEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			var views1 = environment.ViewDockService.FindViews(Guids.BuildTypeBuildsViewGuid).ToList();
			foreach(var view in views1) view.Close();
			environment.RemoveRepositoryExplorerItem(_repositoryExplorer.RootItem);
			_repositoryExplorer = null;
		}

		public void SaveTo(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
		}
	}
}
