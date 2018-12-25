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
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Context menu for <see cref="Revision"/>.</summary>
	[ToolboxItem(false)]
	public sealed class RevisionMenu : ContextMenuStrip
	{
		/// <summary>Create <see cref="RevisionMenu"/>.</summary>
		/// <param name="revision">Related <see cref="Revision"/>.</param>
		/// <exception cref="T:System.NullReferenceException"><paramref name="revision"/> == <c>null</c>.</exception>
		public RevisionMenu(Revision revision)
		{
			Verify.Argument.IsNotNull(revision, nameof(revision));

			Revision = revision;

			Items.Add(GuiItemFactory.GetViewDiffItem<ToolStripMenuItem>(Revision.GetDiffSource()));
			Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(Revision));
			Items.Add(GuiItemFactory.GetSavePatchItem<ToolStripMenuItem>(Revision));
			Items.Add(GuiItemFactory.GetArchiveItem<ToolStripMenuItem>(Revision));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCheckoutRevisionItem<ToolStripMenuItem>(Revision, "{0}"));
			Items.Add(GuiItemFactory.GetResetHeadHereItem<ToolStripMenuItem>(Revision));
			Items.Add(GuiItemFactory.GetRebaseHeadHereItem<ToolStripMenuItem>(Revision));
			Items.Add(GuiItemFactory.GetRevertItem<ToolStripMenuItem>(Revision));
			Items.Add(GuiItemFactory.GetCherryPickItem<ToolStripMenuItem>(Revision, "{0}"));

			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, revision.HashString));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrTreeHash, revision.TreeHashString));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, revision.Subject));
			if(!string.IsNullOrEmpty(revision.Body))
			{
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrBody, revision.Body));
			}
			if(revision.Committer != revision.Author)
			{
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitter, revision.Committer.Name));
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitterEmail, revision.Committer.Email));
			}
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthor, revision.Author.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthorEmail, revision.Author.Email));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrPatch, () => Encoding.UTF8.GetString(revision.FormatPatch())));

			Items.Add(item);

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(Revision));

			var branches = Revision.References.GetAllBranches();
			if(branches.Count != 0)
			{
				if(branches.Count == 1)
				{
					foreach(var branch in branches)
					{
						Items.Add(GuiItemFactory.GetRemoveBranchItem<ToolStripMenuItem>(branch, "{0} '{1}'"));
					}
				}
				else
				{
					var submenu = new ToolStripMenuItem(Resources.StrRemoveBranch);
					foreach(var branch in branches)
					{
						submenu.DropDownItems.Add(GuiItemFactory.GetRemoveBranchItem<ToolStripMenuItem>(branch, "{1}"));
					}
					Items.Add(submenu);
				}
			}

			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(Revision));

			var tags = Revision.References.GetTags();
			if(tags.Count != 0)
			{
				if(tags.Count == 1)
				{
					foreach(var tag in tags)
					{
						Items.Add(GuiItemFactory.GetRemoveTagItem<ToolStripMenuItem>(tag, "{0} '{1}'"));
					}
				}
				else
				{
					var submenu = new ToolStripMenuItem(Resources.StrRemoveTag);
					foreach(var tag in tags)
					{
						submenu.DropDownItems.Add(GuiItemFactory.GetRemoveTagItem<ToolStripMenuItem>(tag, "{1}"));
					}
					Items.Add(submenu);
				}
			}
			/*
			Items.Add(new ToolStripSeparator()); // notes section
			
			Items.Add(GuiItemFactory.GetAddNoteItem<ToolStripMenuItem>(_revision));
			*/
		}

		/// <summary>Associated revision.</summary>
		public Revision Revision { get; }
	}
}
