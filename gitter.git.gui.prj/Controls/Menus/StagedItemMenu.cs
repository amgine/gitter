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
			if(item == null) throw new ArgumentNullException("item");
			if(item.IsDeleted) throw new ArgumentException(Resources.ExcObjectIsDeleted.UseAsFormat("WorkingTreeItem"), "item");
			if((item.StagedStatus & StagedStatus.Staged) != StagedStatus.Staged) throw new ArgumentException("This item is not staged.", "item");

			_item = item;

			Items.Add(GuiItemFactory.GetUnstageItem<ToolStripMenuItem>(_item));
		}

		public TreeItem Item
		{
			get { return _item; }
		}
	}
}
