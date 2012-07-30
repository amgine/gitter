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

		public RepositoryExplorer(IWorkingEnvironment env, RedmineGuiProvider guiProvider)
		{
			if(env == null) throw new ArgumentNullException("env");
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");

			_service = guiProvider.ServiceContext;
			_rootItem = new RepositoryExplorerRootListItem(env, guiProvider);
		}

		public CustomListBoxItem RootItem
		{
			get { return _rootItem; }
		}
	}
}
