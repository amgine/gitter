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
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class UnstagedItemMenu : ContextMenuStrip
{
	public UnstagedItemMenu(TreeItem item)
	{
		Verify.Argument.IsValidGitObject(item, nameof(item));
		Verify.Argument.AreEqual(StagedStatus.Unstaged, item.StagedStatus & StagedStatus.Unstaged, nameof(item),
			"This item is not unstaged.");

		Item = item;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		var dir = Item as TreeDirectory;
		if(Item.Status != FileStatus.Removed)
		{
			var fullPath = Item.FullPath;
			if(dir is null)
			{
				Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpen, null, fullPath));
				Items.Add(GuiItemFactory.GetOpenUrlWithItem<ToolStripMenuItem>(Resources.StrOpenWith.AddEllipsis(), null, fullPath));
				Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenContainingFolder, null, Path.GetDirectoryName(fullPath)));
			}
			else
			{
				Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, fullPath));
			}
			Items.Add(new ToolStripSeparator());
		}
		Items.Add(factory.GetStageItem<ToolStripMenuItem>(Item));
		if(dir is not null)
		{
			if(HasRevertableItems(dir))
			{
				Items.Add(factory.GetRevertPathItem<ToolStripMenuItem>(Item));
			}
		}
		else
		{
			if(Item.Status is FileStatus.Removed or FileStatus.Modified)
			{
				Items.Add(factory.GetRevertPathItem<ToolStripMenuItem>(Item));
			}
			if(Item.Status is FileStatus.Modified or FileStatus.Added)
			{
				Items.Add(factory.GetRemovePathItem<ToolStripMenuItem>(Item));
			}
		}
	}

	private static bool HasRevertableItems(TreeDirectory directory)
	{
		foreach(var file in directory.Files)
		{
			if(file.Status is FileStatus.Removed or FileStatus.Modified)
			{
				return true;
			}
		}
		foreach(var dir in directory.Directories)
		{
			if(HasRevertableItems(dir))
			{
				return true;
			}
		}
		return false;
	}

	public TreeItem Item { get; }
}
