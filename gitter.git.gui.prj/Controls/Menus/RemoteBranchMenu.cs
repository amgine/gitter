namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Menu for <see cref="RemoteBranch"/> object.</summary>
	[ToolboxItem(false)]
	public sealed class RemoteBranchMenu : ContextMenuStrip
	{
		private readonly RemoteRepositoryBranch _remoteBranch;

		/// <summary>Create <see cref="RemoteBranchMenu"/>.</summary>
		/// <param name="remoteBranch">Remote branch, for which menu is generated.</param>
		public RemoteBranchMenu(RemoteRepositoryBranch remoteBranch)
		{
			if(remoteBranch == null) throw new ArgumentNullException("remoteBranch");
			if(remoteBranch.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "RemoteBranch"), "remoteBranch");
			_remoteBranch = remoteBranch;

			Items.Add(GuiItemFactory.GetRemoveRemoteBranchItem<ToolStripMenuItem>(_remoteBranch, "{0}"));

			var copyToClipboardItem = new ToolStripMenuItem(Resources.StrCopyToClipboard);

			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrName, _remoteBranch.Name));
			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrFullName, _remoteBranch.FullName));
			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(
				Resources.StrPosition, _remoteBranch.Hash));

			Items.Add(copyToClipboardItem);
		}

		/// <summary>Remote branch, for which menu is generated.</summary>
		public RemoteRepositoryBranch RemoteBranch
		{
			get { return _remoteBranch; }
		}
	}
}
