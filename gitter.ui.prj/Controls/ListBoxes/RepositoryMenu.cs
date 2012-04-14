namespace gitter
{
	using System;
	using System.Windows.Forms;

	using Resources = gitter.Properties.Resources;

	class RepositoryMenu : ContextMenuStrip
	{
		private readonly RepositoryListItem _repository;

		public RepositoryMenu(RepositoryListItem repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			Items.Add(new ToolStripMenuItem(Resources.StrOpen, null, (s, e) => _repository.Activate()));
			Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, _repository.Data.Path));
			Items.Add(GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, _repository.Data.Path));
			Items.Add(GuiItemFactory.GetRemoveRepositoryItem<ToolStripMenuItem>(_repository));
		}

		public RepositoryListItem Repository
		{
			get { return _repository; }
		}
	}
}
