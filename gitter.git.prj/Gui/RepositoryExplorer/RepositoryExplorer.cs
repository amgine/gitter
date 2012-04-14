namespace gitter.Git.Gui
{
	using System;

	using gitter.Framework.Controls;

	sealed class RepositoryExplorer
	{
		#region Data

		private readonly GuiProvider _gui;
		private Repository _repository;

		private readonly RepositoryRootItem _rootItem;

		#endregion

		public RepositoryExplorer(GuiProvider gui)
		{
			if(gui == null) throw new ArgumentNullException("gui");
			_gui = gui;
			_repository = gui.Repository;

			_rootItem = new RepositoryRootItem()
				{
					Repository = _repository,
				};
			_rootItem.IsExpanded = true;
		}

		public CustomListBoxItem RootItem
		{
			get { return _rootItem; }
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					if(_repository != null)
					{
						DetachFromRepository(_repository);
					}
					if(value != null)
					{
						AttachToRepository(value);
					}
				}
			}
		}

		private void AttachToRepository(Repository repository)
		{
			_repository = repository;
			_rootItem.Repository = repository;
		}

		private void DetachFromRepository(Repository repository)
		{
			_repository = null;
			_rootItem.Repository = null;
		}
	}
}
