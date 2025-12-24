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
public sealed class ConfigParameterMenu : ContextMenuStrip
{
	public ConfigParameterMenu(ConfigParameterListItem listItem)
	{
		Verify.Argument.IsNotNull(listItem);
		Verify.Argument.IsValidGitObject(listItem.DataContext);

		Renderer = GitterApplication.Style.ToolStripRenderer;

		ConfigParameter = listItem.DataContext;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		var edit = new ToolStripMenuItem(Resources.StrEditValue, null, (_, _) => listItem.StartValueEditor());
		dpiBindings.BindImage(edit, Icons.ConfigEdit);
		Items.Add(edit);

		Items.Add(factory.GetUnsetParameterItem<ToolStripMenuItem>(ConfigParameter));
		Items.Add(new ToolStripSeparator());
		Items.Add(new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
			factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, ConfigParameter.Name),
			factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrValue, ConfigParameter.Value)));
	}

	public ConfigParameterMenu(ConfigParameter parameter)
	{
		Verify.Argument.IsValidGitObject(parameter, nameof(parameter));

		Renderer = GitterApplication.Style.ToolStripRenderer;

		ConfigParameter = parameter;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Items.Add(factory.GetUnsetParameterItem<ToolStripMenuItem>(parameter));
		Items.Add(new ToolStripSeparator());
		Items.Add(new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
			factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, ConfigParameter.Name),
			factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrValue, ConfigParameter.Value)));
	}

	public ConfigParameter ConfigParameter { get; }
}
