#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
			Verify.Argument.IsNotNull(remoteBranch, "remoteBranch");
			Verify.Argument.IsFalse(remoteBranch.IsDeleted, "remoteBranch",
				Resources.ExcObjectIsDeleted.UseAsFormat("RemoteBranch"));

			_remoteBranch = remoteBranch;

			Items.Add(GuiItemFactory.GetRemoveRemoteBranchItem<ToolStripMenuItem>(_remoteBranch, "{0}"));

			var copyToClipboardItem = new ToolStripMenuItem(Resources.StrCopyToClipboard);

			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrName, _remoteBranch.Name));
			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrFullName, _remoteBranch.FullName));
			copyToClipboardItem.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(
				Resources.StrPosition, _remoteBranch.Hash.ToString()));

			Items.Add(copyToClipboardItem);
		}

		/// <summary>Remote branch, for which menu is generated.</summary>
		public RemoteRepositoryBranch RemoteBranch
		{
			get { return _remoteBranch; }
		}
	}
}
