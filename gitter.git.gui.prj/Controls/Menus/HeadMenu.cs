namespace gitter.Git.Gui.Controls
{
	using System;
	using System.IO;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class HeadMenu : ContextMenuStrip
	{
		private readonly Head _head;

		public HeadMenu(Head head)
		{
			Verify.Argument.IsValidGitObject(head, "head");

			_head = head;

			Items.Add(GuiItemFactory.GetViewReflogItem<ToolStripMenuItem>(head));
		}

		public Head Head
		{
			get { return _head; }
		}
	}
}
