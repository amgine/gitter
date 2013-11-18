#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
		private readonly IViewFactory _viewRemote;
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

		public ViewFactoriesCollection(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, "guiProvider");

			_gui = guiProvider;
			_viewFactories = new IViewFactory[]
			{
				_viewGit            = new GitViewFactory(guiProvider),
				_viewHistory        = new HistoryViewFactory(guiProvider),
				_viewPathHistory    = new PathHistoryViewFactory(guiProvider),
				_viewReferences     = new ReferencesViewFactory(guiProvider),
				_viewCommit         = new CommitViewFactory(guiProvider),
				_viewStash          = new StashViewFactory(guiProvider),
				_viewRemotes        = new RemotesViewFactory(guiProvider),
				_viewRemote         = new RemoteViewFactory(guiProvider),
				_viewTree           = new TreeViewFactory(guiProvider),
				_viewSubmodules     = new SubmodulesViewFactory(guiProvider),
				_viewConfig         = new ConfigViewFactory(guiProvider),
				_viewUsers          = new ContributorsViewFactory(guiProvider),
				_viewDiff           = new DiffViewFactory(guiProvider),
				_viewBlame          = new BlameViewFactory(guiProvider),
				_viewContextualDiff = new ContextualDiffViewFactory(guiProvider),
				_viewReflog         = new ReflogViewFactory(guiProvider),
			};

			if(guiProvider.Repository != null)
			{
				AttachToRepository(guiProvider.Repository);
			}
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
				if(factory.IsSingleton)
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

		public IViewFactory RemoteViewFactory
		{
			get { return _viewRemote; }
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
