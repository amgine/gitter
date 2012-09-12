namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Context menu for unstaged changes item.</summary>
	[ToolboxItem(false)]
	public sealed class UnstagedChangesMenu : ContextMenuStrip
	{
		private readonly Repository _repository;

		public UnstagedChangesMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(GuiItemFactory.GetStashSaveKeepIndexItem<ToolStripMenuItem>(repository));

			if(repository.Status.UnmergedCount != 0)
			{
				Items.Add(new ToolStripSeparator());

				Items.Add(GuiItemFactory.GetResolveConflictsItem<ToolStripMenuItem>(repository));
			}
			
			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetStageAllItem<ToolStripMenuItem>(repository, Resources.StrStageAll));
			Items.Add(GuiItemFactory.GetUpdateItem<ToolStripMenuItem>(repository, Resources.StrUpdate));
			Items.Add(GuiItemFactory.GetManualStageItem<ToolStripMenuItem>(repository, Resources.StrManualStage.AddEllipsis()));
			
			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCleanItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetResetItem<ToolStripMenuItem>(repository, ResetMode.Mixed | ResetMode.Hard));
		}

		public Repository Repository
		{
			get { return _repository; }
		}
	}
}
