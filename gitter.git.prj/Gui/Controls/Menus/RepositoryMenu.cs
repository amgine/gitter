namespace gitter.Git.Gui
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	sealed class RepositoryMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public RepositoryMenu(Repository repository)
		{
			if(repository == null)
				throw new ArgumentNullException("repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetCompressRepositoryItem<ToolStripMenuItem>(_repository));
		}
	}
}
