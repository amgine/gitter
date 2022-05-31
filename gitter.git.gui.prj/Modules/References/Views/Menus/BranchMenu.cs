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
public sealed class BranchMenu : ContextMenuStrip
{
	public BranchMenu(BranchBase branch)
	{
		Verify.Argument.IsValidGitObject(branch, nameof(branch));

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Branch = branch;

		AddViewItems(factory);
		Items.Add(new ToolStripSeparator()); // interactive section
		AddActionItems(factory, dpiBindings);
		Items.Add(new ToolStripSeparator());
		AddCopyToClipboardItems(factory);
		Items.Add(new ToolStripSeparator());
		AddCreateItems(factory);
	}

	public BranchBase Branch { get; }

	private void AddViewItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetViewReflogItem<ToolStripMenuItem>(Branch));
		Items.Add(factory.GetViewTreeItem<ToolStripMenuItem>(Branch));
	}

	private void AddActionItems(GuiItemFactory factory, DpiBindings dpiBindings)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetCheckoutRevisionItem<ToolStripMenuItem>(Branch, "{0} '{1}'"));
		Items.Add(factory.GetResetHeadHereItem<ToolStripMenuItem>(Branch));
		Items.Add(factory.GetRebaseHeadHereItem<ToolStripMenuItem>(Branch));
		Items.Add(factory.GetMergeItem<ToolStripMenuItem>(Branch));
		if(!Branch.IsRemote)
		{
			Items.Add(factory.GetRenameBranchItem<ToolStripMenuItem>((Branch)Branch, "{0}"));
		}
		Items.Add(factory.GetRemoveBranchItem<ToolStripMenuItem>(Branch));
		if(!Branch.IsRemote)
		{
			var remotes = Branch.Repository.Remotes;
			lock(remotes.SyncRoot)
			{
				if(remotes.Count != 0)
				{
					var pushTo = new ToolStripMenuItem(Resources.StrPushTo);
					foreach(var remote in remotes)
					{
						pushTo.DropDownItems.Add(factory.GetPushBranchToRemoteItem<ToolStripMenuItem>((Branch)Branch, remote));
					}
					dpiBindings.BindImage(pushTo, Icons.Push);
					Items.Add(new ToolStripSeparator());
					Items.Add(pushTo);
				}
			}
		}
		else
		{

		}
	}

	private void AddCopyToClipboardItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
		var copyItems = item.DropDownItems;
		copyItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, Branch.Name));
		copyItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullName, Branch.FullName));
		copyItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrPosition, Branch.Revision.HashString));
		Items.Add(item);
	}

	private void AddCreateItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetCreateBranchItem<ToolStripMenuItem>(Branch));
		Items.Add(factory.GetCreateTagItem<ToolStripMenuItem>(Branch));
	}
}
