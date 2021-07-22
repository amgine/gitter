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

		private readonly Notifications _notifications;

		private Repository _repository;
		private IWorkingEnvironment _environment;
		private RepositoryExplorer _explorer;

		#endregion

		/// <summary>Create <see cref="GuiProvider"/>.</summary>
		/// <param name="repositoryProvider">Git repository provider.</param>
		public GuiProvider(RepositoryProvider repositoryProvider)
		{
			Verify.Argument.IsNotNull(repositoryProvider, nameof(repositoryProvider));

			RepositoryProvider = repositoryProvider;

			MainToolBar    = new GitToolbar(this);
			ViewFactories  = new ViewFactoriesCollection(this);
			Statusbar      = new Statusbar(this);
			Menus          = new MainGitMenus(this);
			_notifications = new Notifications(this);
		}

		public RepositoryProvider RepositoryProvider { get; }

		public Repository Repository
		{
			get => _repository;
			set
			{
				if(_repository != value)
				{
					_repository = value;
					MainToolBar.Repository    = _repository;
					ViewFactories.Repository  = _repository;
					if(_explorer != null)
					{
						_explorer.Repository  = _repository;
					}
					Statusbar.Repository      = _repository;
					Menus.Repository          = _repository;
					_notifications.Repository = _repository;
				}
			}
		}

		IRepository IRepositoryGuiProvider.Repository
		{
			get => Repository;
			set => Repository = value as Repository;
		}

		public IWorkingEnvironment Environment => _environment;

		public ViewFactoriesCollection ViewFactories { get; }

		public RepositoryExplorer RepositoryExplorer { get; }

		public GitToolbar MainToolBar { get; }

		public Statusbar Statusbar { get; }

		public MainGitMenus Menus { get; }

		public IRevisionPointer GetFocusedRevisionPointer()
			=> Environment.ViewDockService.ActiveView switch
			{
				HistoryView    historyView    => historyView.SelectedRevision,
				ReferencesView referencesView => referencesView.SelectedReference,
				_ => default,
			};

		public DialogResult StartCreateBranchDialog()
		{
			var revision = GetFocusedRevisionPointer();
			string startingRevision;
			string defaultBranchName;
			if(revision is not null)
			{
				startingRevision  = revision.Pointer;
				defaultBranchName = BranchHelper.TryFormatDefaultLocalBranchName(revision);
			}
			else
			{
				startingRevision  = GitConstants.HEAD;
				defaultBranchName = string.Empty;
			}
			return StartCreateBranchDialog(startingRevision, defaultBranchName);
		}

		public DialogResult StartCreateBranchDialog(string startingRevision, string defaultBranchName)
		{
			using var dlg = new CreateBranchDialog(_repository);
			dlg.StartingRevision.Value = startingRevision;
			dlg.BranchName.Value       = defaultBranchName;
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartCheckoutDialog()
		{
			var rev = GetFocusedRevisionPointer();
			if(rev is Revision revision)
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
			using var dlg = new CheckoutDialog(_repository);
			if(rev is not null)
			{
				dlg.Revision.Value = rev.Pointer;
			}
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartMergeDialog(bool multiMerge = false)
		{
			using var dlg = new MergeDialog(_repository);
			if(multiMerge)
			{
				dlg.EnableMultipleBrunchesMerge();
			}
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartPushDialog()
		{
			using var dlg = new PushDialog(_repository);
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartApplyPatchesDialog()
		{
			using var dlg = new ApplyPatchesDialog(_repository);
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartCreateTagDialog()
		{
			var rev = GetFocusedRevisionPointer();
			using var dlg = new CreateTagDialog(_repository);
			dlg.Revision.Value = rev is not null ? rev.Pointer : GitConstants.HEAD;
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartAddNoteDialog()
		{
			var rev = GetFocusedRevisionPointer();
			using var dlg = new AddNoteDialog(_repository);
			dlg.Revision.Value = rev is not null ? rev.Pointer : GitConstants.HEAD;
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartStageFilesDialog()
		{
			using var dlg = new StageDialog(_repository);
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartStashSaveDialog()
		{
			using var dlg = new StashSaveDialog(_repository);
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartCleanDialog()
		{
			using var dlg = new CleanDialog(_repository);
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartResolveConflictsDialog()
		{
			using var dlg = new ConflictsDialog(_repository);
			return dlg.Run(Environment.MainForm);
		}

		public DialogResult StartUserIdentificationDialog()
		{
			using var dlg = new UserIdentificationDialog(Environment, Repository);
			var result = dlg.Run(Environment.MainForm);
			if(result == DialogResult.OK)
			{
				Statusbar.UpdateUserIdentityLabel();
			}
			return result;
		}

		public DialogResult StartAddRemoteDialog()
		{
			using var dlg = new AddRemoteDialog(Repository);
			return dlg.Run(Environment.MainForm);
		}

		public void SaveTo(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
		}

		public void AttachToEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));
			Verify.State.IsTrue(Environment == null);

			_environment = environment;

			_explorer = new RepositoryExplorer(this);

			foreach(var factory in ViewFactories)
			{
				environment.ViewDockService.RegisterFactory(factory);
			}

			environment.ProvideRepositoryExplorerItem(_explorer.RootItem);
			environment.ProvideToolbar(MainToolBar);
			for(int i = 0; i < Statusbar.LeftAlignedItems.Length; ++i)
			{
				environment.ProvideStatusBarObject(Statusbar.LeftAlignedItems[i], true);
			}
			for(int i = 0; i < Statusbar.RightAlignedItems.Length; ++i)
			{
				environment.ProvideStatusBarObject(Statusbar.RightAlignedItems[i], false);
			}
			foreach(var menu in Menus.Menus)
			{
				environment.ProvideMainMenuItem(menu);
			}
			foreach(var item in Menus.ViewMenuItems)
			{
				item.Environment = environment;
				environment.ProvideViewMenuItem(item);
			}

			ActivateDefaultView();
		}

		public void DetachFromEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));
			Verify.Argument.AreNotEqual(Environment, environment, nameof(environment), string.Empty);

			foreach(var factory in ViewFactories)
			{
				factory.CloseAllViews();
				environment.ViewDockService.UnregisterFactory(factory);
			}

			environment.RemoveRepositoryExplorerItem(_explorer.RootItem);
			environment.RemoveToolbar(MainToolBar);
			for(int i = 0; i < Statusbar.LeftAlignedItems.Length; ++i)
			{
				environment.RemoveStatusBarObject(Statusbar.LeftAlignedItems[i]);
			}
			for(int i = 0; i < Statusbar.RightAlignedItems.Length; ++i)
			{
				environment.RemoveStatusBarObject(Statusbar.RightAlignedItems[i]);
			}
			foreach(var menu in Menus.Menus)
			{
				environment.RemoveMainMenuItem(menu);
			}
			foreach(var item in Menus.ViewMenuItems)
			{
				item.Environment = null;
				environment.RemoveViewMenuItem(item);
			}

			_explorer = null;
			_environment = null;
		}

		public void ActivateDefaultView()
		{
			Verify.State.IsTrue(Environment is not null);

			Environment.ViewDockService.ShowView(Guids.HistoryViewGuid);
		}

		public void Dispose()
		{
			MainToolBar.Dispose();
			Statusbar.Dispose();
			Menus.Dispose();
			_notifications.Dispose();
		}
	}
}
