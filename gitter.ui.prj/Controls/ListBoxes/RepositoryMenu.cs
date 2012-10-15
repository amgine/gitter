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
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(new ToolStripMenuItem(Resources.StrOpen, null, (s, e) => _repository.Activate()));
			Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, _repository.DataContext.Path));
			Items.Add(GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, _repository.DataContext.Path));

			var actions = GuiItemFactory.GetRepositoryActions<ToolStripMenuItem>(repository.DataContext.Path);
			if(actions.Count != 0)
			{
				Items.Add(new ToolStripSeparator());
				foreach(var item in actions)
				{
					Items.Add(item);
				}
			}

			Items.Add(new ToolStripSeparator());
			Items.Add(GuiItemFactory.GetRemoveRepositoryItem<ToolStripMenuItem>(_repository));
		}

		public RepositoryListItem Repository
		{
			get { return _repository; }
		}
	}
}
