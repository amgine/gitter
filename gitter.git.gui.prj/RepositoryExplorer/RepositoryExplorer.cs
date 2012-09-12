namespace gitter.Git.Gui
{
	using System;

	using gitter.Framework.Controls;

	sealed class RepositoryExplorer
	{
		#region Data

		private readonly GuiProvider _guiProvider;
		private readonly RepositoryRootItem _rootItem;
		private Repository _repository;

		#endregion

		public RepositoryExplorer(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, "guiProvider");

			_guiProvider = guiProvider;
			_rootItem = new RepositoryRootItem(_guiProvider.Environment)
				{
					Repository = guiProvider.Repository,
				};
			_repository = guiProvider.Repository;

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
