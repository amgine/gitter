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

namespace gitter.Git.Gui;

using System;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;

using gitter.Git.Gui.Views;
using gitter.Git.Gui.Dialogs;

using Autofac;
using gitter.Framework.Controls;
using System.Collections.Generic;

/// <summary>Git Gui provider.</summary>
internal sealed class GuiProvider : IRepositoryGuiProvider, IDisposable
{
	#region Data

	private readonly Notifications _notifications;

	private Repository? _repository;
	private IWorkingEnvironment? _environment;
	private RepositoryExplorer? _explorer;
	private readonly ILifetimeScope _parentScope;
	private ILifetimeScope? _lifetimeScope;

	#endregion

	/// <summary>Create <see cref="GuiProvider"/>.</summary>
	/// <param name="repositoryProvider">Git repository provider.</param>
	/// <param name="lifetimeScope">Parent lifetime scope.</param>
	public GuiProvider(RepositoryProvider repositoryProvider, ILifetimeScope lifetimeScope)
	{
		Verify.Argument.IsNotNull(repositoryProvider);
		Verify.Argument.IsNotNull(lifetimeScope);

		RepositoryProvider = repositoryProvider;

		_parentScope   = lifetimeScope;
		ViewFactories  = lifetimeScope.ResolveNamed<IViewFactory[]>(@"git");
		MainToolBar    = new GitToolbar(this);
		Statusbar      = new Statusbar(this);
		Menus          = new MainGitMenus(this);
		_notifications = new Notifications(this);
	}

	public RepositoryProvider RepositoryProvider { get; }

	public Repository? Repository
	{
		get => _repository;
		set
		{
			if(_repository == value) return;

			_repository = value;
			MainToolBar.Repository = _repository;
			foreach(var viewFactory in ViewFactories)
			{
				if(!viewFactory.IsSingleton)
				{
					viewFactory.CloseAllViews();
					continue;
				}
				foreach(var view in viewFactory.CreatedViews)
				{
					if(view is GitViewBase gitView)
					{
						gitView.Repository = value;
					}
				}
			}
			if(_explorer is not null)
			{
				_explorer.Repository  = _repository;
			}
			Statusbar.Repository      = _repository;
			Menus.Repository          = _repository;
			_notifications.Repository = _repository;
		}
	}

	IRepository? IRepositoryGuiProvider.Repository
	{
		get => Repository;
		set => Repository = value as Repository;
	}

	public IWorkingEnvironment? Environment => _environment;

	public DpiBindings MainFormDpiBindings { get; } = new();

	public RepositoryExplorer? RepositoryExplorer => _explorer;

	public GitToolbar MainToolBar { get; }

	public Statusbar Statusbar { get; }

	public MainGitMenus Menus { get; }

	public IReadOnlyList<IViewFactory> ViewFactories { get; }

	public IRevisionPointer? GetFocusedRevisionPointer()
	{
		if(Environment is null) return default;

		return Environment.ViewDockService.ActiveView switch
		{
			HistoryView    historyView    => historyView.SelectedRevision,
			ReferencesView referencesView => referencesView.SelectedReference,
			_ => default,
		};
	}

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
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new CreateBranchDialog(repository);
		dialog.StartingRevision.Value = startingRevision;
		dialog.BranchName.Value       = defaultBranchName;
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartCheckoutDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

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
		using var dialog = new CheckoutDialog(repository);
		if(rev is not null)
		{
			dialog.Revision.Value = rev.Pointer;
		}
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartMergeDialog(bool multiMerge = false)
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new MergeDialog(repository);
		if(multiMerge)
		{
			dialog.EnableMultipleBrunchesMerge();
		}
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartPushDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new PushDialog(repository);
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartApplyPatchesDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new ApplyPatchesDialog(repository);
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartCreateTagDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		var rev = GetFocusedRevisionPointer();
		using var dialog = new CreateTagDialog(repository);
		dialog.Revision.Value = rev is not null ? rev.Pointer : GitConstants.HEAD;
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartAddNoteDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		var rev = GetFocusedRevisionPointer();
		using var dialog = new AddNoteDialog(repository);
		dialog.Revision.Value = rev is not null ? rev.Pointer : GitConstants.HEAD;
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartStageFilesDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new StageDialog(repository);
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartStashSaveDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new StashSaveDialog(repository);
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartCleanDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new CleanDialog(repository);
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartResolveConflictsDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new ConflictsDialog(repository);
		return dialog.Run(environment.MainForm);
	}

	public DialogResult StartUserIdentificationDialog()
	{
		var environment = RequireEnvironment();

		using var dialog = new UserIdentificationDialog(RepositoryProvider, Repository);
		var result = dialog.Run(environment.MainForm);
		if(result == DialogResult.OK)
		{
			Statusbar.UpdateUserIdentityLabel();
		}
		return result;
	}

	public DialogResult StartAddRemoteDialog()
	{
		var environment = RequireEnvironment();
		var repository  = RequireRepository();

		using var dialog = new AddRemoteDialog(repository);
		return dialog.Run(environment.MainForm);
	}

	public void SaveTo(Section section)
	{
	}

	public void LoadFrom(Section section)
	{
	}

	public void AttachToEnvironment(IWorkingEnvironment environment)
	{
		Verify.Argument.IsNotNull(environment);
		Verify.State.IsTrue(Environment is null);

		_lifetimeScope = _parentScope.BeginLifetimeScope(@"git", builder =>
		{
			builder
				.RegisterInstance(this)
				.As<GuiProvider>()
				.SingleInstance()
				.ExternallyOwned();
		});

		try
		{
			_environment = environment;

			_explorer = new RepositoryExplorer(this);

			MainFormDpiBindings.Control = environment.MainForm;

			foreach(var factory in ViewFactories)
			{
				if(factory is GitViewFactoryBase gitViewFactory)
				{
					gitViewFactory.Scope = _lifetimeScope;
				}
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
		catch
		{
			DisposableUtility.Dispose(ref _lifetimeScope);
		}
	}

	public void DetachFromEnvironment(IWorkingEnvironment environment)
	{
		Verify.Argument.IsNotNull(environment);
		Verify.Argument.AreNotEqual(Environment, environment, nameof(environment), string.Empty);

		MainFormDpiBindings.Control = null;

		foreach(var factory in ViewFactories)
		{
			if(factory is GitViewFactoryBase gitViewFactory)
			{
				gitViewFactory.Scope = default;
			}
			factory.CloseAllViews();
			environment.ViewDockService.UnregisterFactory(factory);
		}

		if(_explorer is not null)
		{
			environment.RemoveRepositoryExplorerItem(_explorer.RootItem);
		}
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

		DisposableUtility.Dispose(ref _lifetimeScope);
	}

	public IWorkingEnvironment RequireEnvironment()
		=> Environment
		?? throw new InvalidOperationException("Provider is not attached to the environment.");

	public Repository RequireRepository()
		=> Repository
		?? throw new InvalidOperationException("Repository is not opened.");

	public void ActivateDefaultView()
		=> RequireEnvironment().ViewDockService.ShowView(Guids.HistoryViewGuid);

	public void Dispose()
	{
		MainToolBar.Dispose();
		Statusbar.Dispose();
		Menus.Dispose();
		_notifications.Dispose();
	}
}
