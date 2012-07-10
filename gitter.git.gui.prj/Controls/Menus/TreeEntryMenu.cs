namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	sealed class TreeEntryMenu : ContextMenuStrip
	{
		private readonly ITreeSource _treeSource;
		private readonly TreeItem _treeItem;

		public TreeEntryMenu(ITreeSource treeSource, TreeItem treeItem)
		{
			if(treeSource == null) throw new ArgumentNullException("treeSource");
			if(treeItem == null) throw new ArgumentNullException("treeItem");

			_treeSource = treeSource;
			_treeItem = treeItem;

			// save as
			// checkout

			Items.Add(GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(treeSource, treeItem.RelativePath));

		}
	}
}
