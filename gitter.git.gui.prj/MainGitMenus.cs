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
using System.Collections.Generic;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Gui.Properties.Resources;

internal sealed class MainGitMenus : IDisposable
{
	private readonly GuiProvider _guiProvider;

	private Repository? _repository;
	private ToolStripMenuItem[]? _menus;
	private ToolStripMenuItem? _gitMenu;
	private readonly List<ViewMenuItem> _viewMenuItems = [];

	public MainGitMenus(GuiProvider guiProvider)
	{
		Verify.Argument.IsNotNull(guiProvider);

		_guiProvider = guiProvider;

		var repository = guiProvider.Repository;

		_menus =
		[
			_gitMenu = new ToolStripMenuItem(
				Resources.StrGit),
		];

		var dpiBindings = guiProvider.MainFormDpiBindings;

		//_gitMenu.DropDownItems.Add(new ToolStripMenuItem(
		//    Resources.StrCheckout.AddEllipsis(), CachedResources.Bitmaps["ImgCheckout"], OnCheckoutClick));
		//_gitMenu.DropDownItems.Add(new ToolStripMenuItem(
		//    Resources.StrAddRemote.AddEllipsis(), CachedResources.Bitmaps["ImgRemoteAdd"], OnAddRemoteClick));
		var branchAdd = new ToolStripMenuItem(Resources.StrCreateBranch.AddEllipsis(), null, OnCreateBranchClick)
		{
			ShortcutKeys = Keys.Control | Keys.B,
		};
		dpiBindings.BindImage(branchAdd, Icons.BranchAdd);
		_gitMenu.DropDownItems.Add(branchAdd);

		var tagAdd = new ToolStripMenuItem(Resources.StrCreateTag.AddEllipsis(), null, OnCreateTagClick)
		{
			ShortcutKeys = Keys.Control | Keys.T,
		};
		dpiBindings.BindImage(tagAdd, Icons.TagAdd);
		_gitMenu.DropDownItems.Add(tagAdd);

		_gitMenu.DropDownItems.Add(new ToolStripSeparator());

		var gitGui = new ToolStripMenuItem(Resources.StrlGui, null, OnGitGuiClick)
		{
			ShortcutKeys = Keys.F5,
		};
		dpiBindings.BindImage(gitGui, Icons.Git);
		_gitMenu.DropDownItems.Add(gitGui);
		var gitk = new ToolStripMenuItem(Resources.StrlGitk, null, OnGitGitkClick)
		{
			Enabled = StandardTools.CanStartGitk,
			ShortcutKeys = Keys.F6,
		};
		dpiBindings.BindImage(gitk, Icons.Git);
		_gitMenu.DropDownItems.Add(gitk);
		var bash = new ToolStripMenuItem(Resources.StrlBash, null, OnGitBashClick)
		{
			Enabled = StandardTools.CanStartBash,
			ShortcutKeys = Keys.F7,
		};
		dpiBindings.BindImage(bash, Icons.Terminal);
		_gitMenu.DropDownItems.Add(bash);
		var term = new ToolStripMenuItem(Resources.StrlCmd, null, OnCmdClick)
		{
			ShortcutKeys = Keys.F8,
		};
		dpiBindings.BindImage(term, Icons.Terminal);
		_gitMenu.DropDownItems.Add(term);

		foreach(var factory in Gui.ViewFactories)
		{
			if(factory.IsSingleton)
			{
				var item = new ViewMenuItem(factory);
				_viewMenuItems.Add(item);
			}
		}

		if(repository is not null)
		{
			AttachToRepository(repository);
		}
	}

	public IReadOnlyList<ToolStripMenuItem> Menus => _menus ?? [];

	public IReadOnlyList<ViewMenuItem> ViewMenuItems => _viewMenuItems;

	public GuiProvider Gui => _guiProvider;

	//private void OnCheckoutClick(object? sender, EventArgs e)
	//{
	//    _gui.StartCheckoutDialog();
	//}

	private void OnCreateBranchClick(object? sender, EventArgs e)
	{
		_guiProvider.StartCreateBranchDialog();
	}

	private void OnCreateTagClick(object? sender, EventArgs e)
	{
		_guiProvider.StartCreateTagDialog();
	}

	//private void OnAddRemoteClick(object? sender, EventArgs e)
	//{
	//    _gui.StartAddRemoteDialog();
	//}

	private void OnGitGuiClick(object? sender, EventArgs e)
	{
		if(_repository is null) return;
		StandardTools.StartGitGui(_repository.WorkingDirectory);
	}

	private void OnGitGitkClick(object? sender, EventArgs e)
	{
		if(_repository is null) return;
		StandardTools.StartGitk(_repository.WorkingDirectory);
	}

	private void OnGitBashClick(object? sender, EventArgs e)
	{
		if(_repository is null) return;
		StandardTools.StartBash(_repository.WorkingDirectory);
	}

	private void OnCmdClick(object? sender, EventArgs e)
	{
		if(_repository is null) return;
		var psi = new System.Diagnostics.ProcessStartInfo(@"cmd")
		{
			WorkingDirectory = _repository.WorkingDirectory,
		};
		System.Diagnostics.Process.Start(psi)?.Dispose();
	}

	private void OnShowViewItemClick(object? sender, EventArgs e)
	{
		var guid = (Guid)((ToolStripMenuItem)sender!).Tag!;
		Gui.Environment?.ViewDockService.ShowView(guid);
	}

	public Repository? Repository
	{
		get => _repository;
		set
		{
			if(_repository == value) return;

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

	private void AttachToRepository(Repository repository)
	{
		if(_gitMenu is not null)
		{
			_gitMenu.Enabled = true;
		}
		_repository = repository;
	}

	private void DetachFromRepository(Repository repository)
	{
		if(_gitMenu is not null)
		{
			_gitMenu.Enabled = false;
		}
		_repository = null;
	}

	#region IDisposable Members

	public void Dispose()
	{
		DisposableUtility.Dispose(ref _gitMenu);
		_menus = null;
		_repository = null;
	}

	#endregion
}
