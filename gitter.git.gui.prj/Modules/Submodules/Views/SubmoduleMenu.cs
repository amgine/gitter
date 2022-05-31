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

[ToolboxItem(false)]
[DesignerCategory("")]
#if NET6_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
public sealed class SubmoduleMenu : ContextMenuStrip
{
	public SubmoduleMenu(Submodule submodule)
	{
		Verify.Argument.IsValidGitObject(submodule, nameof(submodule));

		Submodule = submodule;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Items.Add(GuiItemFactory.GetOpenAppItem<ToolStripMenuItem>(
			Resources.StrOpenWithGitter, null, Application.ExecutablePath, Submodule.FullPath.SurroundWithDoubleQuotes()));
		Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(
			Resources.StrOpenInWindowsExplorer, null, Submodule.FullPath));
		Items.Add(GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(
			Resources.StrOpenCommandLine, null, Submodule.FullPath));

		Items.Add(new ToolStripSeparator());

		Items.Add(factory.GetUpdateSubmoduleItem<ToolStripMenuItem>(submodule));
		Items.Add(factory.GetSyncSubmoduleItem<ToolStripMenuItem>(submodule));

		Items.Add(new ToolStripSeparator()); // copy to clipboard section

		var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, Submodule.Name));
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrPath, Submodule.Path));
		item.DropDownItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrUrl, Submodule.Url));

		Items.Add(item);
	}

	public Submodule Submodule { get; }
}
