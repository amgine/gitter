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
public sealed class IssueMenu : ContextMenuStrip
{
	public IssueMenu(Issue issue)
	{
		Verify.Argument.IsNotNull(issue);

		Issue = issue;

		var dpiBindings = new DpiBindings(this);
		var factory = new GuiItemFactory(dpiBindings);

		Items.Add(factory.GetUpdateRedmineObjectItem<ToolStripMenuItem>(Issue));

		var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrId, Issue.Id.ToString(CultureInfo.InvariantCulture)));
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, Issue.Subject));
		if(Issue.Category is not null)
		{
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCategory, Issue.Category.Name));
		}

		Items.Add(item);
	}

	public Issue Issue { get; }
}
