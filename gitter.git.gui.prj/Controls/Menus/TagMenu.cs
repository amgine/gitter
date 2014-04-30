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

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class TagMenu : ContextMenuStrip
	{
		private readonly Tag _tag;

		public TagMenu(Tag tag)
		{
			Verify.Argument.IsValidGitObject(tag, "tag");

			_tag = tag;

			Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(_tag));
			Items.Add(GuiItemFactory.GetArchiveItem<ToolStripMenuItem>(_tag));

			Items.Add(new ToolStripSeparator()); // interactive section

			Items.Add(GuiItemFactory.GetCheckoutRevisionItem<ToolStripMenuItem>(_tag, "{0} '{1}'"));
			Items.Add(GuiItemFactory.GetResetHeadHereItem<ToolStripMenuItem>(_tag));
			Items.Add(GuiItemFactory.GetRemoveTagItem<ToolStripMenuItem>(_tag, Resources.StrDelete));

			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, tag.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullName, tag.FullName));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrPosition, tag.Revision.Hash.ToString()));

			Items.Add(item);

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(_tag));
			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(_tag));
		}

		public new Tag Tag
		{
			get { return _tag; }
		}
	}
}
