namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Views;

	internal sealed class ViewFactoriesCollection : IEnumerable<IViewFactory>
	{
		#region Data

		private readonly IViewFactory[] _viewFactories;
		private readonly IViewFactory _viewGit;
		private readonly IViewFactory _viewHistory;
		private readonly IViewFactory _viewPathHistory;
		private readonly IViewFactory _viewReferences;
		private readonly IViewFactory _viewCommit;
		private readonly IViewFactory _viewStash;
		private readonly IViewFactory _viewRemotes;
		private readonly IViewFactory _viewTree;
		private readonly IViewFactory _viewConfig;
		private readonly IViewFactory _viewSubmodules;
		private readonly IViewFactory _viewUsers;
		private readonly IViewFactory _viewDiff;
		private readonly IViewFactory _viewBlame;
		private readonly IViewFactory _viewContextualDiff;
		private readonly IViewFactory _viewReflog;

		private readonly GuiProvider _gui;
		private Repository _repository;

		#endregion

		public ViewFactoriesCollection(GuiProvider gui)
		{
			if(gui == null) throw new ArgumentNullException("gui");
			_gui = gui;
			_repository = gui.Repository;

			_viewFactories = new IViewFactory[]
			{
				_viewGit =				new GitViewFactory(gui),
				_viewHistory =			new HistoryViewFactory(gui),
				_viewPathHistory =		new PathHistoryViewFactory(gui),
				_viewReferences =		new ReferencesViewFactory(gui),
				_viewCommit =			new CommitViewFactory(gui),
				_viewStash =			new StashViewFactory(gui),
				_viewRemotes =			new RemotesViewFactory(gui),
				_viewTree =				new TreeViewFactory(gui),
				_viewSubmodules =		new SubmodulesViewFactory(gui),
				_viewConfig =			new ConfigViewFactory(gui),
				_viewUsers =			new ContributorsViewFactory(gui),
				_viewDiff =				new DiffViewFactory(gui),
				_viewBlame =			new BlameViewFactory(gui),
				_viewContextualDiff =	new ContextualDiffViewFactory(gui),
				_viewReflog =			new ReflogViewFactory(gui),
			};
		}

		public IViewFactory this[Guid guid]
		{
			get
			{
				foreach(var viewFactory in _viewFactories)
				{
					if(viewFactory.Guid == guid) return viewFactory;
				}
				return null;
			}
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
			foreach(var factory in _viewFactories)
			{
				foreach(GitViewBase view in factory.CreatedViews)
				{
					view.Repository = repository;
				}
			}
		}

		private void DetachFromRepository(Repository repository)
		{
			_repository = null;
			foreach(var factory in _viewFactories)
			{
				if(factory.Singleton)
				{
					foreach(GitViewBase view in factory.CreatedViews)
					{
						view.Repository = null;
					}
				}
				else
				{
					factory.CloseAllViews();
				}
			}
		}

		#region View Factories

		public IViewFactory GitViewFactory
		{
			get { return _viewGit; }
		}

		public IViewFactory HistoryViewFactory
		{
			get { return _viewHistory; }
		}

		public IViewFactory PathHistoryViewFactory
		{
			get { return _viewPathHistory; }
		}

		public IViewFactory ReferencesViewFactory
		{
			get { return _viewReferences; }
		}

		public IViewFactory ReflogViewFactory
		{
			get { return _viewReflog; }
		}

		public IViewFactory CommitViewFactory
		{
			get { return _viewCommit; }
		}

		public IViewFactory StashViewFactory
		{
			get { return _viewStash; }
		}

		public IViewFactory RemotesViewFactory
		{
			get { return _viewRemotes; }
		}

		public IViewFactory SubmodulesViewFactory
		{
			get { return _viewSubmodules; }
		}

		public IViewFactory WorkingTreeViewFactory
		{
			get { return _viewTree; }
		}

		public IViewFactory ConfigurationViewFactory
		{
			get { return _viewConfig; }
		}

		public IViewFactory UsersViewFactory
		{
			get { return _viewUsers; }
		}

		#endregion

		#region IEnumerable<IViewFactory> Members

		public IEnumerator<IViewFactory> GetEnumerator()
		{
			return ((IEnumerable<IViewFactory>)_viewFactories).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _viewFactories.GetEnumerator();
		}

		#endregion
	}
}
