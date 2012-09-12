namespace gitter.Git.Gui
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	sealed class RepositoryMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public RepositoryMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetCompressRepositoryItem<ToolStripMenuItem>(_repository));
		}
	}
}
