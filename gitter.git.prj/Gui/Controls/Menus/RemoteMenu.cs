namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class RemoteMenu : ContextMenuStrip
	{
		private readonly Remote _remote;

		public RemoteMenu(Remote remote)
		{
			if(remote == null) throw new ArgumentNullException("remote");
			_remote = remote;

			Items.Add(GuiItemFactory.GetBrowseRemoteItem<ToolStripMenuItem>(remote));
			Items.Add(GuiItemFactory.GetEditRemotePropertiesItem<ToolStripMenuItem>(remote));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetFetchFromItem<ToolStripMenuItem>(remote, "{0}"));
			Items.Add(GuiItemFactory.GetPullFromItem<ToolStripMenuItem>(remote, "{0}"));
			Items.Add(GuiItemFactory.GetPruneRemoteItem<ToolStripMenuItem>(remote, "{0}"));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetRemoveRemoteItem<ToolStripMenuItem>(remote, "{0}"));
			Items.Add(GuiItemFactory.GetRenameRemoteItem<ToolStripMenuItem>(remote, "{0}"));
			
			Items.Add(new ToolStripSeparator());

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);

			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, remote.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFetchUrl, remote.FetchUrl));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrPushUrl, remote.PushUrl));

			Items.Add(item);
		}

		public Remote Remote
		{
			get { return _remote; }
		}
	}
}
