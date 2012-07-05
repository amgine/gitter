namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class TagMenu : ContextMenuStrip
	{
		private readonly Tag _tag;

		public TagMenu(Tag tag)
		{
			if(tag == null) throw new ArgumentNullException("tag");
			if(tag.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Tag"), "tag");
			_tag = tag;

			Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(_tag));
			Items.Add(GuiItemFactory.GetArchiveItem<ToolStripMenuItem>(_tag));

			Items.Add(new ToolStripSeparator()); // interactive section

			Items.Add(GuiItemFactory.GetCheckoutRevisionItem<ToolStripMenuItem>(_tag, "{0} '{1}'"));
			Items.Add(GuiItemFactory.GetResetHeadHereItem<ToolStripMenuItem>(_tag));
			Items.Add(GuiItemFactory.GetRemoveTagItem<ToolStripMenuItem>(_tag, Resources.StrDelete));

			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, tag.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullName, tag.FullName));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrPosition, tag.Revision.Name));

			Items.Add(item);

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(_tag));
			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(_tag));
		}

		public new Tag Tag
		{
			get { return _tag; }
		}
	}
}
