namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.IO;

	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class UsersMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public UsersMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetShowContributorsViewItem<ToolStripMenuItem>());
			Items.Add(GuiItemFactory.GetRefreshContributorsItem<ToolStripMenuItem>(_repository));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
