namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Redmine.Gui;

	sealed class RepositoryExplorer
	{
		private readonly RepositoryExplorerRootListItem _rootItem;
		private readonly RedmineServiceContext _service;

		public RepositoryExplorer(IWorkingEnvironment environment, RedmineGuiProvider guiProvider)
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
