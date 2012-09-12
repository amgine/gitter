namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class RemotesMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public RemotesMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetShowRemotesViewItem<ToolStripMenuItem>());
			Items.Add(GuiItemFactory.GetRefreshRemotesItem<ToolStripMenuItem>(repository));
			Items.Add(new ToolStripSeparator());
			Items.Add(GuiItemFactory.GetFetchItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetPullItem<ToolStripMenuItem>(repository));
			Items.Add(new ToolStripSeparator());
			Items.Add(GuiItemFactory.GetAddRemoteItem<ToolStripMenuItem>(repository));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
