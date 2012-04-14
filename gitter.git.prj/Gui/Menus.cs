namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	internal sealed class MainGitMenus : IDisposable
	{
		private readonly GuiProvider _gui;

		private Repository _repository;
		private ToolStripMenuItem[] _menus;
		private ToolStripMenuItem _gitMenu;

		public MainGitMenus(GuiProvider gui)
		{
			if(gui == null) throw new ArgumentNullException("gui");
			_gui = gui;

			var repository = gui.Repository;

			_menus = new ToolStripMenuItem[]
			{
				_gitMenu = new ToolStripMenuItem(
					Resources.StrGit),
			};

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
				new ToolStripMenuItem(Resources.StrlGui, CachedResources.Bitmaps["ImgGit"], OnGitGuiClick));
			_gitMenu.DropDownItems.Add(
				new ToolStripMenuItem(Resources.StrlGitk, CachedResources.Bitmaps["ImgGit"], OnGitGitkClick));
			_gitMenu.DropDownItems.Add(
				new ToolStripMenuItem(Resources.StrlBash, CachedResources.Bitmaps["ImgTerminal"], OnGitBashClick));

			if(repository != null)
				AttachToRepository(repository);
		}

		public IEnumerable<ToolStripMenuItem> Menus
		{
			get { return _menus; }
		}

		public GuiProvider Gui
		{
			get { return _gui; }
		}

		private void OnCreateBranchClick(object sender, EventArgs e)
		{
			_gui.StartCreateBranchDialog();
		}

		private void OnCreateTagClick(object sender, EventArgs e)
		{
			_gui.StartCreateTagDialog();
		}

		private void OnGitGuiClick(object sender, EventArgs e)
		{
			StandardTools.StartGitGui(_repository);
		}

		private void OnGitGitkClick(object sender, EventArgs e)
		{
			StandardTools.StartGitk(_repository);
		}

		private void OnGitBashClick(object sender, EventArgs e)
		{
			StandardTools.StartBash(_repository);
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					if(_repository != null)
						DetachFromRepository(_repository);
					if(value != null)
						AttachToRepository(value);
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
