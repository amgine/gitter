namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class StashedStateMenu : ContextMenuStrip
	{
		private readonly StashedState _stashedState;

		public StashedStateMenu(StashedState stashedState)
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");
			_stashedState = stashedState;

			Items.AddRange(
				new ToolStripItem[]
				{
					GuiItemFactory.GetViewDiffItem<ToolStripMenuItem>(StashedState.GetDiffSource()),
					GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(StashedState),
					GuiItemFactory.GetSavePatchItem<ToolStripMenuItem>(StashedState),
					new ToolStripSeparator(),
					GuiItemFactory.GetStashPopItem<ToolStripMenuItem>(StashedState),
					GuiItemFactory.GetStashApplyItem<ToolStripMenuItem>(StashedState),
					GuiItemFactory.GetStashDropItem<ToolStripMenuItem>(StashedState),
					new ToolStripSeparator(),
					GuiItemFactory.GetStashToBranchItem<ToolStripMenuItem>(StashedState),
					new ToolStripSeparator(),
					new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
						new ToolStripItem[]
						{
							GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, ((IRevisionPointer)StashedState).Pointer),
							GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, StashedState.Revision.Name),
							GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, StashedState.Revision.Subject),
						}),
				});
		}

		public StashedState StashedState
		{
			get { return _stashedState; }
		}
	}
}
