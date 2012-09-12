namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class StagedItemMenu : ContextMenuStrip
	{
		private readonly TreeItem _item;

		public StagedItemMenu(TreeItem item)
		{
			Verify.Argument.IsValidGitObject(item, "item");
			Verify.Argument.AreEqual(StagedStatus.Staged, item.StagedStatus & StagedStatus.Staged, "item",
				"This item is not staged.");

			_item = item;

			Items.Add(GuiItemFactory.GetUnstageItem<ToolStripMenuItem>(_item));
		}

		public TreeItem Item
		{
			get { return _item; }
		}
	}
}
