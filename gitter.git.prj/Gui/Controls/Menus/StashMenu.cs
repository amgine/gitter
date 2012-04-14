namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class StashMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public StashMenu(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			Items.Add(GuiItemFactory.GetShowStashViewItem<ToolStripMenuItem>());
			Items.Add(GuiItemFactory.GetRefreshStashItem<ToolStripMenuItem>(_repository));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
