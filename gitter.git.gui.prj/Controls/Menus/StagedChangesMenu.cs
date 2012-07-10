namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Context menu for staged changes item.</summary>
	[ToolboxItem(false)]
	public sealed class StagedChangesMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public StagedChangesMenu(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			Items.Add(GuiItemFactory.GetCommitItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetStashSaveItem<ToolStripMenuItem>(repository));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetUnstageAllItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetResetItem<ToolStripMenuItem>(repository, ResetMode.Mixed | ResetMode.Hard));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
