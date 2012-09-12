namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Menu for <see cref="RemoteTag"/> object.</summary>
	[ToolboxItem(false)]
	public sealed class RemoteTagMenu : ContextMenuStrip
	{
		private readonly RemoteRepositoryTag _remoteTag;

		/// <summary>Create <see cref="RemoteBranchMenu"/>.</summary>
		/// <param name="remoteTag">Remote branch, for which menu is generated.</param>
		public RemoteTagMenu(RemoteRepositoryTag remoteTag)
		{
			Verify.Argument.IsNotNull(remoteTag, "remoteTag");
			Verify.Argument.IsFalse(remoteTag.IsDeleted, "remote",
				Resources.ExcObjectIsDeleted.UseAsFormat("RemoteTag"));

			_remoteTag = remoteTag;

			Items.Add(GuiItemFactory.GetRemoveRemoteTagItem<ToolStripMenuItem>(_remoteTag, "{0}"));

			var copyToClipboardItem = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrName, _remoteTag.Name));
			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrFullName, _remoteTag.FullName));
			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(
				Resources.StrPosition, _remoteTag.Hash));
			Items.Add(copyToClipboardItem);
		}

		/// <summary>Remote tag, for which menu is generated.</summary>
		public RemoteRepositoryTag RemoteTag
		{
			get { return _remoteTag; }
		}
	}
}
