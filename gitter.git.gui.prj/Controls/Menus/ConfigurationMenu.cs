namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class ConfigurationMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public ConfigurationMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetShowConfigurationViewItem<ToolStripMenuItem>());
			Items.Add(GuiItemFactory.GetRefreshConfigurationItem<ToolStripMenuItem>(_repository));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
