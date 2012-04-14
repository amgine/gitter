namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class UnstagedItemMenu : ContextMenuStrip
	{
		private readonly TreeItem _item;

		public UnstagedItemMenu(TreeItem item)
		{
			if(item == null) throw new ArgumentNullException("item");
			if(item.IsDeleted) throw new ArgumentException(Resources.ExcObjectIsDeleted.UseAsFormat("WorkingTreeItem"), "item");
			if((item.StagedStatus & StagedStatus.Unstaged) != StagedStatus.Unstaged) throw new ArgumentException("This item is not unstaged.", "item");

			_item = item;

			Items.Add(GuiItemFactory.GetStageItem<ToolStripMenuItem>(_item));

			var dir = _item as TreeDirectory;
			if(dir != null)
			{
				if(HasRevertableItems(dir))
				{
					Items.Add(GuiItemFactory.GetRevertPathItem<ToolStripMenuItem>(_item));
				}
			}
			else
			{
				if(_item.Status == FileStatus.Removed || _item.Status == FileStatus.Modified)
				{
					Items.Add(GuiItemFactory.GetRevertPathItem<ToolStripMenuItem>(_item));
				}
				if(_item.Status == FileStatus.Modified || _item.Status == FileStatus.Added)
				{
					Items.Add(GuiItemFactory.GetRemovePathItem<ToolStripMenuItem>(_item));
				}
			}
		}

		private static bool HasRevertableItems(TreeDirectory directory)
		{
			foreach(var file in directory.Files)
			{
				if(file.Status == FileStatus.Removed || file.Status == FileStatus.Modified)
				{
					return true;
				}
			}
			foreach(var dir in directory.Directories)
			{
				if(HasRevertableItems(dir))
				{
					return true;
				}
			}
			return false;
		}

		public TreeItem Item
		{
			get { return _item; }
		}
	}
}
