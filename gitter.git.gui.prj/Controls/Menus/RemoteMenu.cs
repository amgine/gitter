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

	[ToolboxItem(false)]
	public sealed class RemoteMenu : ContextMenuStrip
	{
		public RemoteMenu(Remote remote)
		{
			Verify.Argument.IsValidGitObject(remote, nameof(remote));

			Remote = remote;

			var dpiBindings = new DpiBindings(this);
			var factory     = new GuiItemFactory(dpiBindings);

			Items.Add(GuiItemFactory.GetShowRemoteItem<ToolStripMenuItem>(remote));
			Items.Add(GuiItemFactory.GetEditRemotePropertiesItem<ToolStripMenuItem>(remote));

			Items.Add(new ToolStripSeparator());

			Items.Add(factory.GetFetchFromItem<ToolStripMenuItem>(remote, "{0}"));
			Items.Add(factory.GetPullFromItem<ToolStripMenuItem>(remote, "{0}"));
			Items.Add(factory.GetPruneRemoteItem<ToolStripMenuItem>(remote, "{0}"));

			Items.Add(new ToolStripSeparator());

			Items.Add(factory.GetRemoveRemoteItem<ToolStripMenuItem>(remote, "{0}"));
			Items.Add(factory.GetRenameRemoteItem<ToolStripMenuItem>(remote, "{0}"));
			
			Items.Add(new ToolStripSeparator());

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);

			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, remote.Name));
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFetchUrl, remote.FetchUrl));
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrPushUrl, remote.PushUrl));

			Items.Add(item);
		}

		public Remote Remote { get; }
	}
}
