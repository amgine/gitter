#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity.Gui;

using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Git.Gui;
using gitter.TeamCity.Properties;

[DesignerCategory("")]
sealed class BuildContextMenu : ContextMenuStrip
{
	public BuildContextMenu(Build build)
	{
		Verify.Argument.IsNotNull(build);

		Build = build;

		var dpiBindings = new DpiBindings(this);
		if(build.WebUrl is { Length: not 0 } url)
		{
			HyperlinkContextMenu.AddItemsTo(dpiBindings, Items, url);
			Items.Add(new ToolStripSeparator());
		}
		var factory = new GuiItemFactory(dpiBindings);
		AddCopyToClipboardItems(factory);
	}

	public Build Build { get; }

	private void AddCopyToClipboardItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
		var copyItems = item.DropDownItems;
		item.DropDown.Renderer = Renderer;
		copyItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrId, Build.Id));
		if(Build.Number is { Length: not 0 } number)
		{
			copyItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrNumber, number));
		}
		if(Build.BranchName is { Length: not 0 } branchName)
		{
			copyItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrBranchName, branchName));
		}
		Items.Add(item);
	}
}
