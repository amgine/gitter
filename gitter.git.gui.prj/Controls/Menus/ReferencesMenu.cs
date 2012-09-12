namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class ReferencesMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public ReferencesMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetShowReferencesViewItem<ToolStripMenuItem>());
			Items.Add(GuiItemFactory.GetRefreshAllReferencesListItem<ToolStripMenuItem>(repository));
			Items.Add(new ToolStripSeparator());
			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(repository));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
