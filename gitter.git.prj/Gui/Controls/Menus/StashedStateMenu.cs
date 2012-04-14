namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class StashedStateMenu : ContextMenuStrip
	{
		private readonly StashedState _stashedState;

		public StashedStateMenu(StashedState stashedState)
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");
			_stashedState = stashedState;

			Items.Add(GuiItemFactory.GetStashPopItem<ToolStripMenuItem>(_stashedState));
			Items.Add(GuiItemFactory.GetStashApplyItem<ToolStripMenuItem>(_stashedState));
			Items.Add(GuiItemFactory.GetStashDropItem<ToolStripMenuItem>(_stashedState));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetStashToBranchItem<ToolStripMenuItem>(_stashedState));

			Items.Add(new ToolStripSeparator());
			var copyItem = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			copyItem.DropDownItems.Add(
				GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, ((IRevisionPointer)_stashedState).Pointer));
			copyItem.DropDownItems.Add(
				GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, _stashedState.Revision.Name));
			copyItem.DropDownItems.Add(
				GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, _stashedState.Revision.Subject));
			Items.Add(copyItem);
		}

		public StashedState StashedState
		{
			get { return _stashedState; }
		}
	}
}
