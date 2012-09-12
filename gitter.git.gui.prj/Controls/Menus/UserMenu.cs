namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Context menu for <see cref="User"/> object.</summary>
	[ToolboxItem(false)]
	public sealed class UserMenu : ContextMenuStrip
	{
		private readonly User _user;

		public UserMenu(User user)
		{
			Verify.Argument.IsValidGitObject(user, "user");

			_user = user;

			Items.Add(GuiItemFactory.GetSendEmailItem<ToolStripMenuItem>(user.Email));

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, user.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrEmail, user.Email));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommits, user.Commits.ToString()));
			Items.Add(item);
		}

		public User User
		{
			get { return _user; }
		}
	}
}
