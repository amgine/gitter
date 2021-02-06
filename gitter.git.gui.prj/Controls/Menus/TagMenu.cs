#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

	/// <summary>Context menu for <see cref="Tag"/> object.</summary>
	[ToolboxItem(false)]
	public sealed class TagMenu : ContextMenuStrip
	{
		public TagMenu(Tag tag)
		{
			Verify.Argument.IsValidGitObject(tag, nameof(tag));

			Tag = tag;

			AddViewItems();
			Items.Add(new ToolStripSeparator());
			AddActionItems();
			Items.Add(new ToolStripSeparator());
			AddCopyToClipboardItems();
			Items.Add(new ToolStripSeparator());
			AddCreateItems();
		}

		public new Tag Tag { get; }

		private void AddViewItems()
		{
			Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(Tag));
			Items.Add(GuiItemFactory.GetArchiveItem<ToolStripMenuItem>(Tag));
		}

		private void AddActionItems()
		{
			Items.Add(GuiItemFactory.GetCheckoutRevisionItem<ToolStripMenuItem>(Tag, "{0} '{1}'"));
			Items.Add(GuiItemFactory.GetResetHeadHereItem<ToolStripMenuItem>(Tag));
			Items.Add(GuiItemFactory.GetRemoveTagItem<ToolStripMenuItem>(Tag, Resources.StrDelete));
		}

		private void AddCopyToClipboardItems()
		{
			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, Tag.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullName, Tag.FullName));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrPosition, Tag.Revision.Hash.ToString()));
			Items.Add(item);
		}

		private void AddCreateItems()
		{
			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(Tag));
			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(Tag));
		}
	}
}
