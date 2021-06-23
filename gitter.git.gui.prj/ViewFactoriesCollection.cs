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
		private readonly GuiProvider _gui;
		private Repository _repository;

		#endregion

		public ViewFactoriesCollection(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_gui = guiProvider;
			_viewFactories = new IViewFactory[]
			{
				GitViewFactory            = new GitViewFactory(guiProvider),
				HistoryViewFactory        = new HistoryViewFactory(guiProvider),
				PathHistoryViewFactory    = new PathHistoryViewFactory(guiProvider),
				ReferencesViewFactory     = new ReferencesViewFactory(guiProvider),
				CommitViewFactory         = new CommitViewFactory(guiProvider),
				StashViewFactory          = new StashViewFactory(guiProvider),
				RemotesViewFactory        = new RemotesViewFactory(guiProvider),
				RemoteViewFactory         = new RemoteViewFactory(guiProvider),
				WorkingTreeViewFactory    = new TreeViewFactory(guiProvider),
				SubmodulesViewFactory     = new SubmodulesViewFactory(guiProvider),
				ConfigurationViewFactory  = new ConfigViewFactory(guiProvider),
				UsersViewFactory          = new ContributorsViewFactory(guiProvider),
				DiffViewFactory           = new DiffViewFactory(guiProvider),
				BlameViewFactory          = new BlameViewFactory(guiProvider),
				ContextualDiffViewFactory = new ContextualDiffViewFactory(guiProvider),
				ReflogViewFactory         = new ReflogViewFactory(guiProvider),
			};

			if(guiProvider.Repository is not null)
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
			get => _repository;
			set
			{
				if(_repository != value)
				{
					if(_repository is not null)
					{
						DetachFromRepository(_repository);
					}
					if(value is not null)
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

		public IViewFactory GitViewFactory { get; }

		public IViewFactory HistoryViewFactory { get; }

		public IViewFactory PathHistoryViewFactory { get; }

		public IViewFactory ReferencesViewFactory { get; }

		public IViewFactory ReflogViewFactory { get; }

		public IViewFactory CommitViewFactory { get; }

		public IViewFactory StashViewFactory { get; }

		public IViewFactory RemotesViewFactory { get; }

		public IViewFactory RemoteViewFactory { get; }

		public IViewFactory SubmodulesViewFactory { get; }

		public IViewFactory WorkingTreeViewFactory { get; }

		public IViewFactory ConfigurationViewFactory { get; }

		public IViewFactory UsersViewFactory { get; }

		public IViewFactory DiffViewFactory { get; }

		public IViewFactory ContextualDiffViewFactory { get; }

		public IViewFactory BlameViewFactory { get; }

		#endregion

		#region IEnumerable<IViewFactory> Members

		public IEnumerator<IViewFactory> GetEnumerator()
			=> ((IEnumerable<IViewFactory>)_viewFactories).GetEnumerator();

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> _viewFactories.GetEnumerator();

		#endregion
	}
}
