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
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Views;
	using gitter.Git.Gui.Dialogs;

	/// <summary>Git Gui provider.</summary>
	internal sealed class GuiProvider : IRepositoryGuiProvider, IDisposable
	{
		#region Data

		private readonly RepositoryProvider _repositoryProvider;
		private Repository _repository;
		private IWorkingEnvironment _environment;

		private readonly GitToolbar _mainToolbar;
		private readonly ViewFactoriesCollection _viewFactories;
		private readonly Statusbar _statusbar;
		private readonly MainGitMenus _menus;
		private readonly Notifications _notifications;
		private RepositoryExplorer _explorer;

		#endregion

		/// <summary>Create <see cref="GuiProvider"/>.</summary>
		/// <param name="repositoryProvider">Git repository provider.</param>
		public GuiProvider(RepositoryProvider repositoryProvider)
		{
			Verify.Argument.IsNotNull(repositoryProvider, "repositoryProvider");

			_repositoryProvider = repositoryProvider;

			_mainToolbar	= new GitToolbar(this);
			_viewFactories	= new ViewFactoriesCollection(this);
			_statusbar		= new Statusbar(this);
			_menus			= new MainGitMenus(this);
			_notifications	= new Notifications(this);
		}

		public RepositoryProvider RepositoryProvider
		{
			get { return _repositoryProvider; }
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					_repository = value;
					_mainToolbar.Repository		= _repository;
					_viewFactories.Repository	= _repository;
					if(_explorer != null)
					{
						_explorer.Repository = _repository;
					}
					_statusbar.Repository		= _repository;
					_menus.Repository			= _repository;
					_notifications.Repository	= _repository;
				}
			}
		}

		IRepository IRepositoryGuiProvider.Repository
		{
			get { return Repository; }
			set { Repository = value as Repository; }
		}

		public IWorkingEnvironment Environment
		{
			get { return _environment; }
		}

		public ViewFactoriesCollection ViewFactories
		{
			get { return _viewFactories; }
		}

		public RepositoryExplorer RepositoryExplorer
		{
			get { return _explorer; }
		}

		public GitToolbar MainToolBar
		{
			get { return _mainToolbar; }
		}

		public Statusbar Statusbar
		{
			get { return _statusbar; }
		}

		public MainGitMenus Menus
		{
			get { return _menus; }
		}

		public IRevisionPointer GetFocusedRevisionPointer()
		{
			var view = _environment.ViewDockService.ActiveView;
			if(view != null)
			{
				var historyView = view as HistoryView;
				if(historyView != null)
				{
					return historyView.SelectedRevision;
				}
				var referencesView = view as ReferencesView;
				if(referencesView != null)
				{
					return referencesView.SelectedReference;
				}
			}
			return null;
		}

		public void StartCreateBranchDialog()
		{
			var revision = GetFocusedRevisionPointer();
			string startingRevision;
			string defaultBranchName;
			if(revision != null)
			{
				startingRevision = revision.Pointer;
				var branch = revision as Branch;
				if(branch != null && branch.IsRemote)
				{
					defaultBranchName = branch.Name.Substring(branch.Name.LastIndexOf('/') + 1);
				}
				else
				{
					defaultBranchName = string.Empty;
				}
			}
			else
			{
				startingRevision = GitConstants.HEAD;
				defaultBranchName = string.Empty;
			}
			StartCreateBranchDialog(startingRevision, defaultBranchName);
		}

		public void StartCreateBranchDialog(string startingRevision, string defaultBranchName)
		{
			using(var dlg = new CreateBranchDialog(_repository))
			{
				dlg.StartingRevision.Value = startingRevision;
				dlg.BranchName.Value = defaultBranchName;
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartCheckoutDialog()
		{
			var rev = GetFocusedRevisionPointer();
			var revision = rev as Revision;
			if(revision != null)
			{
				foreach(var branch in revision.References.GetBranches())
				{
					if(!branch.IsRemote && !branch.IsCurrent)
					{
						rev = branch;
						break;
					}
				}
			}
			using(var dlg = new CheckoutDialog(_repository))
			{
				if(rev != null)
				{
					dlg.Revision.Value = rev.Pointer;
				}
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartMergeDialog(bool multiMerge)
		{
			using(var dlg = new MergeDialog(_repository))
			{
				if(multiMerge)
				{
					dlg.EnableMultipleBrunchesMerge();
				}
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartPushDialog()
		{
			using(var dlg = new PushDialog(_repository))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartApplyPatchesDialog()
		{
			using(var dlg = new ApplyPatchesDialog(_repository))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartCreateTagDialog()
		{
			var rev = GetFocusedRevisionPointer();
			using(var dlg = new CreateTagDialog(_repository))
			{
				dlg.Revision.Value = rev != null ? rev.Pointer : GitConstants.HEAD;
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartAddNoteDialog()
		{
			var rev = GetFocusedRevisionPointer();
			using(var dlg = new AddNoteDialog(_repository))
			{
				dlg.Revision.Value = rev != null ? rev.Pointer : GitConstants.HEAD;
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartStageFilesDialog()
		{
			using(var dlg = new StageDialog(_repository))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartStashSaveDialog()
		{
			using(var dlg = new StashSaveDialog(_repository))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartCleanDialog()
		{
			using(var dlg = new CleanDialog(_repository))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartResolveConflictsDialog()
		{
			using(var dlg = new ConflictsDialog(_repository))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartUserIdentificationDialog()
		{
			using(var dlg = new UserIdentificationDialog(Environment, Repository))
			{
				if(dlg.Run(Environment.MainForm) == DialogResult.OK)
				{
					_statusbar.UpdateUserIdentityLabel();
				}
			}
		}

		public void StartAddRemoteDialog()
		{
			using(var dlg = new AddRemoteDialog(Repository))
			{
				dlg.Run(Environment.MainForm);
			}
		}

		public void SaveTo(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
		}

		public void AttachToEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");
			Verify.State.IsTrue(_environment == null);

			_environment = environment;

			_explorer = new RepositoryExplorer(this);

			foreach(var factory in _viewFactories)
			{
				environment.ViewDockService.RegisterFactory(factory);
			}

			environment.ProvideRepositoryExplorerItem(_explorer.RootItem);
			environment.ProvideToolbar(_mainToolbar);
			for(int i = 0; i < _statusbar.LeftAlignedItems.Length; ++i)
			{
				environment.ProvideStatusBarObject(_statusbar.LeftAlignedItems[i], true);
			}
			for(int i = 0; i < _statusbar.RightAlignedItems.Length; ++i)
			{
				environment.ProvideStatusBarObject(_statusbar.RightAlignedItems[i], false);
			}
			foreach(var menu in _menus.Menus)
			{
				environment.ProvideMainMenuItem(menu);
			}
			foreach(var item in _menus.ViewMenuItems)
			{
				environment.ProvideViewMenuItem(item);
			}

			ActivateDefaultView();
		}

		public void DetachFromEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");
			Verify.Argument.AreNotEqual(_environment, environment, "environment", string.Empty);

			foreach(var factory in _viewFactories)
			{
				factory.CloseAllViews();
				environment.ViewDockService.UnregisterFactory(factory);
			}

			environment.RemoveRepositoryExplorerItem(_explorer.RootItem);
			environment.RemoveToolbar(_mainToolbar);
			for(int i = 0; i < _statusbar.LeftAlignedItems.Length; ++i)
			{
				environment.RemoveStatusBarObject(_statusbar.LeftAlignedItems[i]);
			}
			for(int i = 0; i < _statusbar.RightAlignedItems.Length; ++i)
			{
				environment.RemoveStatusBarObject(_statusbar.RightAlignedItems[i]);
			}
			foreach(var menu in _menus.Menus)
			{
				environment.RemoveMainMenuItem(menu);
			}
			foreach(var item in _menus.ViewMenuItems)
			{
				environment.RemoveViewMenuItem(item);
			}

			_explorer = null;
			_environment = null;
		}

		public void ActivateDefaultView()
		{
			Verify.State.IsTrue(_environment != null);

			_environment.ViewDockService.ShowView(Guids.HistoryViewGuid);
		}

		public void Dispose()
		{
			_mainToolbar.Dispose();
			_statusbar.Dispose();
			_menus.Dispose();
			_notifications.Dispose();
		}
	}
}
