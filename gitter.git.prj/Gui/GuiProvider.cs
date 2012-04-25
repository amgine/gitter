namespace gitter.Git.Gui
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Views;
	using gitter.Git.Gui.Dialogs;

	/// <summary>Git Gui provider.</summary>
	internal sealed class GuiProvider : IRepositoryGuiProvider
	{
		#region Data

		private Repository _repository;
		private IWorkingEnvironment _environment;

		private readonly MainToolbar _mainToolbar;
		private readonly ViewFactoriesCollection _viewFactories;
		private readonly Statusbar _statusbar;
		private readonly MainGitMenus _menus;
		private RepositoryExplorer _explorer;

		#endregion

		/// <summary>Create <see cref="GuiProvider"/>.</summary>
		/// <param name="repository">Related repository.</param>
		public GuiProvider(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			_mainToolbar = new MainToolbar(this);
			_viewFactories = new ViewFactoriesCollection(this);
			_statusbar = new Statusbar(this);
			_menus = new MainGitMenus(this);
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					_repository = value;
					_mainToolbar.Repository = _repository;
					_viewFactories.Repository = _repository;
					_explorer.Repository = _repository;
					_statusbar.Repository = _repository;
					_menus.Repository = _repository;
				}
			}
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

		public MainToolbar MainToolBar
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
			var tool = _environment.ViewDockService.ActiveView;
			if(tool != null)
			{
				var historyTool = tool as HistoryView;
				if(historyTool != null)
				{
					return historyTool.SelectedRevision;
				}
				var referencesTool = tool as ReferencesView;
				if(referencesTool != null)
				{
					return referencesTool.SelectedReference;
				}
			}
			return null;
		}

		public void StartCreateBranchDialog()
		{
			var revision = GetFocusedRevisionPointer();
			string startingRevision;
			string defaultName;
			if(revision != null)
			{
				startingRevision = revision.Pointer;
				var branch = revision as Branch;
				if(branch != null && branch.IsRemote)
				{
					defaultName = branch.Name.Substring(branch.Name.LastIndexOf('/') + 1);
				}
				else
				{
					defaultName = string.Empty;
				}
			}
			else
			{
				startingRevision = GitConstants.HEAD;
				defaultName = string.Empty;
			}
			using(var dlg = new CreateBranchDialog(_repository))
			{
				dlg.StartingRevision = startingRevision;
				dlg.BranchName = defaultName;
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartCheckoutDialog()
		{
			var rev = GetFocusedRevisionPointer();
			var revision = rev as Revision;
			if(revision != null)
			{
				foreach(var branch in revision.GetBranches())
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
					dlg.Revision = rev.Pointer;
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
				dlg.Revision = rev != null ? rev.Pointer : GitConstants.HEAD;
				dlg.Run(_environment.MainForm);
			}
		}

		public void StartAddNoteDialog()
		{
			var rev = GetFocusedRevisionPointer();
			using(var dlg = new AddNoteDialog(_repository))
			{
				dlg.Revision = rev != null ? rev.Pointer : GitConstants.HEAD;
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

		public void SaveTo(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
		}

		public void AttachToEnvironment(IWorkingEnvironment env)
		{
			if(env == null) throw new ArgumentNullException("env");
			if(_environment != null) throw new InvalidOperationException();

			_environment = env;

			_explorer = new RepositoryExplorer(this);

			foreach(var factory in _viewFactories)
			{
				env.ViewDockService.RegisterFactory(factory);
			}

			env.ProvideRepositoryExplorerItem(_explorer.RootItem);
			env.ProvideToolbar(_mainToolbar);
			for(int i = 0; i < _statusbar.LeftAlignedItems.Length; ++i)
			{
				env.ProvideStatusBarObject(_statusbar.LeftAlignedItems[i], true);
			}
			for(int i = 0; i < _statusbar.RightAlignedItems.Length; ++i)
			{
				env.ProvideStatusBarObject(_statusbar.RightAlignedItems[i], false);
			}
			foreach(var menu in _menus.Menus)
			{
				env.ProvideMainMenuItem(menu);
			}

			ActivateDefaultTool();
		}

		public void DetachFromEnvironment(IWorkingEnvironment env)
		{
			if(env == null) throw new ArgumentNullException("env");
			if(_environment != env) throw new InvalidOperationException();

			foreach(var factory in _viewFactories)
			{
				factory.CloseAllViews();
				env.ViewDockService.UnregisterFactory(factory);
			}

			env.RemoveRepositoryExplorerItem(_explorer.RootItem);
			env.RemoveToolbar(_mainToolbar);
			for(int i = 0; i < _statusbar.LeftAlignedItems.Length; ++i)
			{
				env.RemoveStatusBarObject(_statusbar.LeftAlignedItems[i]);
			}
			for(int i = 0; i < _statusbar.RightAlignedItems.Length; ++i)
			{
				env.RemoveStatusBarObject(_statusbar.RightAlignedItems[i]);
			}
			foreach(var menu in _menus.Menus)
			{
				env.RemoveMainMenuItem(menu);
			}

			_explorer = null;
			_environment = null;
		}

		public void ActivateDefaultTool()
		{
			if(_environment == null) throw new InvalidOperationException();

			_environment.ViewDockService.ShowView(Guids.HistoryViewGuid);
		}

		IRepository IRepositoryGuiProvider.Repository
		{
			get { return _repository; }
			set
			{
				var repo = value as Repository;
				if(repo == null && value != null) throw new ArgumentException();
				Repository = repo;
			}
		}
	}
}
