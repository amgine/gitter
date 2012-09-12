namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class BranchMenu : ContextMenuStrip
	{
		private readonly BranchBase _branch;

		public BranchMenu(BranchBase branch)
		{
			Verify.Argument.IsValidGitObject(branch, "branch");

			_branch = branch;

			Items.Add(GuiItemFactory.GetViewReflogItem<ToolStripMenuItem>(_branch));
			Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(_branch));

			Items.Add(new ToolStripSeparator()); // interactive section

			Items.Add(GuiItemFactory.GetCheckoutRevisionItem<ToolStripMenuItem>(_branch, "{0} '{1}'"));
			Items.Add(GuiItemFactory.GetResetHeadHereItem<ToolStripMenuItem>(_branch));
			Items.Add(GuiItemFactory.GetRebaseHeadHereItem<ToolStripMenuItem>(_branch));
			Items.Add(GuiItemFactory.GetMergeItem<ToolStripMenuItem>(_branch));
			if(!branch.IsRemote)
			{
				Items.Add(GuiItemFactory.GetRenameBranchItem<ToolStripMenuItem>((Branch)_branch, "{0}"));
			}
			Items.Add(GuiItemFactory.GetRemoveBranchItem<ToolStripMenuItem>(_branch));
			if(!branch.IsRemote)
			{
				lock(branch.Repository.Remotes.SyncRoot)
				{
					if(branch.Repository.Remotes.Count != 0)
					{
						Items.Add(new ToolStripSeparator());
						var pushTo = new ToolStripMenuItem(Resources.StrPushTo, CachedResources.Bitmaps["ImgPush"]);
						foreach(var remote in branch.Repository.Remotes)
						{
							pushTo.DropDownItems.Add(GuiItemFactory.GetPushBranchToRemoteItem<ToolStripMenuItem>((Branch)branch, remote));
						}
						Items.Add(pushTo);
					}
				}
			}
			else
			{

			}
			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, _branch.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullName, _branch.FullName));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrPosition, _branch.Revision.Hash));

			Items.Add(item);

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(_branch));
			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(_branch));
		}

		public BranchBase Branch
		{
			get { return _branch; }
		}
	}
}
