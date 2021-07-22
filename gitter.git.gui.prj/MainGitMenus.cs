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
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Gui.Properties.Resources;

	internal sealed class MainGitMenus : IDisposable
	{
		private readonly GuiProvider _guiProvider;

		private Repository _repository;
		private ToolStripMenuItem[] _menus;
		private ToolStripMenuItem _gitMenu;
		private readonly List<ViewMenuItem> _viewMenuItems = new();

		public MainGitMenus(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;

			var repository = guiProvider.Repository;

			_menus = new ToolStripMenuItem[]
			{
				_gitMenu = new ToolStripMenuItem(
					Resources.StrGit),
			};

			//_gitMenu.DropDownItems.Add(new ToolStripMenuItem(
			//    Resources.StrCheckout.AddEllipsis(), CachedResources.Bitmaps["ImgCheckout"], OnCheckoutClick));
			//_gitMenu.DropDownItems.Add(new ToolStripMenuItem(
			//    Resources.StrAddRemote.AddEllipsis(), CachedResources.Bitmaps["ImgRemoteAdd"], OnAddRemoteClick));
			_gitMenu.DropDownItems.Add(new ToolStripMenuItem(
				Resources.StrCreateBranch.AddEllipsis(), CachedResources.Bitmaps["ImgBranchAdd"], OnCreateBranchClick)
				{
					ShortcutKeys = Keys.Control | Keys.B,
				});
			_gitMenu.DropDownItems.Add(new ToolStripMenuItem(
				Resources.StrCreateTag.AddEllipsis(), CachedResources.Bitmaps["ImgTagAdd"], OnCreateTagClick)
				{
					ShortcutKeys = Keys.Control | Keys.T,
				});

			_gitMenu.DropDownItems.Add(new ToolStripSeparator());

			_gitMenu.DropDownItems.Add(
				new ToolStripMenuItem(Resources.StrlGui, CachedResources.Bitmaps["ImgGit"], OnGitGuiClick)
				{
					ShortcutKeys = Keys.F5,
				});
			_gitMenu.DropDownItems.Add(
				new ToolStripMenuItem(Resources.StrlGitk, CachedResources.Bitmaps["ImgGit"], OnGitGitkClick)
				{
					Enabled = StandardTools.CanStartGitk,
					ShortcutKeys = Keys.F6,
				});
			_gitMenu.DropDownItems.Add(
				new ToolStripMenuItem(Resources.StrlBash, CachedResources.Bitmaps["ImgTerminal"], OnGitBashClick)
				{
					Enabled = StandardTools.CanStartBash,
					ShortcutKeys = Keys.F7,
				});
			_gitMenu.DropDownItems.Add(
				new ToolStripMenuItem(Resources.StrlCmd, CachedResources.Bitmaps["ImgTerminal"], OnCmdClick)
				{
					ShortcutKeys = Keys.F8,
				});

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

		public IEnumerable<ToolStripMenuItem> Menus => _menus;

		public IEnumerable<ViewMenuItem> ViewMenuItems => _viewMenuItems;

		public GuiProvider Gui => _guiProvider;

		//private void OnCheckoutClick(object sender, EventArgs e)
		//{
		//    _gui.StartCheckoutDialog();
		//}

		private void OnCreateBranchClick(object sender, EventArgs e)
		{
			_guiProvider.StartCreateBranchDialog();
		}

		private void OnCreateTagClick(object sender, EventArgs e)
		{
			_guiProvider.StartCreateTagDialog();
		}

		//private void OnAddRemoteClick(object sender, EventArgs e)
		//{
		//    _gui.StartAddRemoteDialog();
		//}

		private void OnGitGuiClick(object sender, EventArgs e)
		{
			StandardTools.StartGitGui(_repository.WorkingDirectory);
		}

		private void OnGitGitkClick(object sender, EventArgs e)
		{
			StandardTools.StartGitk(_repository.WorkingDirectory);
		}

		private void OnGitBashClick(object sender, EventArgs e)
		{
			StandardTools.StartBash(_repository.WorkingDirectory);
		}

		private void OnCmdClick(object sender, EventArgs e)
		{
			var psi = new System.Diagnostics.ProcessStartInfo(@"cmd")
			{
				WorkingDirectory = Repository.WorkingDirectory,
			};
			System.Diagnostics.Process.Start(psi)?.Dispose();
		}

		private void OnShowViewItemClick(object sender, EventArgs e)
		{
			var guid = (Guid)((ToolStripMenuItem)sender).Tag;
			Gui.Environment.ViewDockService.ShowView(guid);
		}

		public Repository Repository
		{
			get => _repository;
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
			_gitMenu.Enabled = true;
			_repository = repository;
		}

		private void DetachFromRepository(Repository repository)
		{
			_gitMenu.Enabled = false;
			_repository = null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if(_gitMenu != null)
			{
				_gitMenu.Dispose();
				_gitMenu = null;
			}
			_menus = null;
			_repository = null;
		}

		#endregion
	}
}
