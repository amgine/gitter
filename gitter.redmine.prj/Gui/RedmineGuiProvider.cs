namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	sealed class RedmineGuiProvider : IGuiProvider
	{
		private readonly IRepository _repository;
		private readonly RedmineServiceContext _service;
		private RepositoryExplorer _repositoryExplorer;

		public RedmineGuiProvider(IRepository repository, RedmineServiceContext svc)
		{
			_repository = repository;
			_service = svc;
		}

		public void AttachToEnvironment(IWorkingEnvironment environment)
		{
			_repositoryExplorer = new RepositoryExplorer(environment, _service);
			environment.ProvideRepositoryExplorerItem(_repositoryExplorer.RootItem);
		}

		public void DetachFromEnvironment(IWorkingEnvironment environment)
		{
			var views1 = environment.ViewDockService.FindViews(Guids.IssuesViewGuid).ToList();
			foreach(var view in views1) view.Close();
			var views2 = environment.ViewDockService.FindViews(Guids.NewsViewGuid).ToList();
			foreach(var view in views2) view.Close();
			var views3 = environment.ViewDockService.FindViews(Guids.VersionsViewGuid).ToList();
			foreach(var view in views3) view.Close();
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
