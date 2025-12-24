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

namespace gitter.Redmine.Gui;

using System;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Redmine.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class VersionMenu : ContextMenuStrip
{
	public VersionMenu(ProjectVersion version)
	{
		Verify.Argument.IsNotNull(version);

		Renderer = GitterApplication.Style.ToolStripRenderer;

		Version = version;

		var dpiBindings = new DpiBindings(this);
		var factory = new GuiItemFactory(dpiBindings);

		Items.Add(factory.GetUpdateRedmineObjectItem<ToolStripMenuItem>(Version));

		var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrId, Version.Id.ToString(CultureInfo.InvariantCulture)));
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, Version.Name));
		if(!string.IsNullOrWhiteSpace(Version.Description))
		{
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrDescription, Version.Description));
		}

		Items.Add(item);
	}

	public ProjectVersion Version { get; }
}
