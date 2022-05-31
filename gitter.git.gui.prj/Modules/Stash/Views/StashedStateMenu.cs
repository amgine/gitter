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
public sealed class StashedStateMenu : ContextMenuStrip
{
	public StashedStateMenu(StashedState stashedState)
	{
		Verify.Argument.IsValidGitObject(stashedState);

		StashedState = stashedState;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Items.AddRange(
			new ToolStripItem[]
			{
				factory.GetViewDiffItem<ToolStripMenuItem>(StashedState.GetDiffSource()),
				factory.GetViewTreeItem<ToolStripMenuItem>(StashedState),
				factory.GetSavePatchItem<ToolStripMenuItem>(StashedState),
				new ToolStripSeparator(),
				factory.GetStashPopItem<ToolStripMenuItem>(StashedState),
				factory.GetStashApplyItem<ToolStripMenuItem>(StashedState),
				factory.GetStashDropItem<ToolStripMenuItem>(StashedState),
				new ToolStripSeparator(),
				factory.GetStashToBranchItem<ToolStripMenuItem>(StashedState),
				new ToolStripSeparator(),
				new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
					new ToolStripItem[]
					{
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, ((IRevisionPointer)StashedState).Pointer),
						factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, StashedState.Revision.Hash.ToString()),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, StashedState.Revision.Subject),
					}),
			});
	}

	public StashedState StashedState { get; }
}
