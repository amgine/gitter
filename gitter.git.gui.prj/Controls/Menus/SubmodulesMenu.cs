namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.IO;

	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class SubmodulesMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public SubmodulesMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetShowSubmodulesViewItem<ToolStripMenuItem>());
			Items.Add(GuiItemFactory.GetRefreshSubmodulesItem<ToolStripMenuItem>(_repository));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetUpdateSubmodulesItem<ToolStripMenuItem>(_repository.Submodules));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetAddSubmoduleItem<ToolStripMenuItem>(_repository));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
