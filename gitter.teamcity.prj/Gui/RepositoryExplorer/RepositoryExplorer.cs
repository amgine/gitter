namespace gitter.TeamCity.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.TeamCity.Gui;

	sealed class RepositoryExplorer
	{
		private readonly RepositoryExplorerRootListItem _rootItem;
		private readonly TeamCityServiceContext _service;

		public RepositoryExplorer(IWorkingEnvironment environment, TeamCityGuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(environment, "environment");
			Verify.Argument.IsNotNull(guiProvider, "guiProvider");

			_service = guiProvider.ServiceContext;
			_rootItem = new RepositoryExplorerRootListItem(environment, guiProvider);
		}

		public CustomListBoxItem RootItem
		{
			get { return _rootItem; }
		}
	}
}
