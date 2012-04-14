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

		public RepositoryExplorer(IWorkingEnvironment env, RedmineServiceContext service)
		{
			_service = service;
			_rootItem = new RepositoryExplorerRootListItem(env, service);
		}

		public CustomListBoxItem RootItem
		{
			get { return _rootItem; }
		}
	}
}
