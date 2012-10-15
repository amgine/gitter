namespace gitter.Git.Gui.Controls
{
	using System;
	using System.IO;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class UnstagedItemMenu : ContextMenuStrip
	{
		private readonly TreeItem _item;

		public UnstagedItemMenu(TreeItem item)
		{
			Verify.Argument.IsValidGitObject(item, "item");
			Verify.Argument.AreEqual(StagedStatus.Unstaged, item.StagedStatus & StagedStatus.Unstaged, "item",
				"This item is not unstaged.");

			_item = item;

			var dir = _item as TreeDirectory;
			if(_item.Status != FileStatus.Removed)
			{
				var fullPath = _item.FullPath;
				if(dir == null)
				{
					Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpen, null, fullPath));
					Items.Add(GuiItemFactory.GetOpenUrlWithItem<ToolStripMenuItem>(Resources.StrOpenWith.AddEllipsis(), null, fullPath));
					Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenContainingFolder, null, Path.GetDirectoryName(fullPath)));
				}
				else
				{
					Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, fullPath));
				}
				Items.Add(new ToolStripSeparator());
			}
			Items.Add(GuiItemFactory.GetStageItem<ToolStripMenuItem>(_item));
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
