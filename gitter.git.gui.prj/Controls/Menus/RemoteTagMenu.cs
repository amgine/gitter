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

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Menu for <see cref="RemoteTag"/> object.</summary>
	[ToolboxItem(false)]
	public sealed class RemoteTagMenu : ContextMenuStrip
	{
		/// <summary>Create <see cref="RemoteBranchMenu"/>.</summary>
		/// <param name="remoteTag">Remote branch, for which menu is generated.</param>
		public RemoteTagMenu(RemoteRepositoryTag remoteTag)
		{
			Verify.Argument.IsNotNull(remoteTag, nameof(remoteTag));
			Verify.Argument.IsFalse(remoteTag.IsDeleted, nameof(remoteTag),
				Resources.ExcObjectIsDeleted.UseAsFormat("RemoteTag"));

			RemoteTag = remoteTag;

			var dpiBindings = new DpiBindings(this);
			var factory     = new GuiItemFactory(dpiBindings);

			Items.Add(GuiItemFactory.GetRemoveRemoteTagItem<ToolStripMenuItem>(RemoteTag, "{0}"));

			var copyToClipboardItem = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			copyToClipboardItem.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrName, RemoteTag.Name));
			copyToClipboardItem.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(
				Resources.StrFullName, RemoteTag.FullName));
			copyToClipboardItem.DropDownItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(
				Resources.StrPosition, RemoteTag.Hash.ToString()));
			Items.Add(copyToClipboardItem);
		}

		/// <summary>Remote tag, for which menu is generated.</summary>
		public RemoteRepositoryTag RemoteTag { get; }
	}
}
