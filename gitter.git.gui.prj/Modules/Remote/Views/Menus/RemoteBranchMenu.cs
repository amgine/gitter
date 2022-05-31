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

namespace gitter.Git.Gui.Controls;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Menu for <see cref="RemoteBranch"/> object.</summary>
[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class RemoteBranchMenu : ContextMenuStrip
{
	/// <summary>Create <see cref="RemoteBranchMenu"/>.</summary>
	/// <param name="remoteBranch">Remote branch, for which menu is generated.</param>
	public RemoteBranchMenu(RemoteRepositoryBranch remoteBranch)
	{
		Verify.Argument.IsNotNull(remoteBranch);
		Verify.Argument.IsFalse(remoteBranch.IsDeleted, nameof(remoteBranch),
			Resources.ExcObjectIsDeleted.UseAsFormat(nameof(RemoteBranch)));

		RemoteBranch = remoteBranch;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Items.Add(factory.GetRemoveRemoteBranchItem<ToolStripMenuItem>(RemoteBranch, "{0}"));

		var copyToClipboardItem = new ToolStripMenuItem(Resources.StrCopyToClipboard);

		copyToClipboardItem.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(
			Resources.StrName, RemoteBranch.Name));
		copyToClipboardItem.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(
			Resources.StrFullName, RemoteBranch.FullName));
		copyToClipboardItem.DropDownItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(
			Resources.StrPosition, RemoteBranch.Hash.ToString()));

		Items.Add(copyToClipboardItem);
	}

	/// <summary>Remote branch, for which menu is generated.</summary>
	public RemoteRepositoryBranch RemoteBranch { get; }
}
