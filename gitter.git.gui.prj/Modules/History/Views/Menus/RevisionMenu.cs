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

namespace gitter.Git.Gui.Controls;

using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Context menu for <see cref="Revision"/>.</summary>
[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class RevisionMenu : ContextMenuStrip
{
	/// <summary>Create <see cref="RevisionMenu"/>.</summary>
	/// <param name="revision">Related <see cref="Revision"/>.</param>
	/// <exception cref="T:System.NullReferenceException"><paramref name="revision"/> == <c>null</c>.</exception>
	public RevisionMenu(Revision revision)
	{
		Verify.Argument.IsNotNull(revision);

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Renderer = GitterApplication.Style.ToolStripRenderer;
		Revision = revision;

		AddViewItems(factory);
		Items.Add(new ToolStripSeparator());
		AddActionItems(factory);
		Items.Add(new ToolStripSeparator());
		AddCopyToClipboardItems(factory);
		Items.Add(new ToolStripSeparator());
		AddBranchItems(factory);
		AddTagItems(factory);
		/*
		Items.Add(new ToolStripSeparator());
		AddNoteItems();
		*/
	}

	/// <summary>Associated revision.</summary>
	public Revision Revision { get; }

	private void AddViewItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetViewDiffItem<ToolStripMenuItem>(Revision.GetDiffSource()));
		Items.Add(factory.GetViewTreeItem<ToolStripMenuItem>(Revision));
		Items.Add(factory.GetSavePatchItem<ToolStripMenuItem>(Revision));
		Items.Add(factory.GetArchiveItem<ToolStripMenuItem>(Revision));
	}

	private void AddActionItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetCheckoutRevisionItem<ToolStripMenuItem>(Revision, "{0}"));
		Items.Add(factory.GetResetHeadHereItem<ToolStripMenuItem>(Revision));
		Items.Add(factory.GetRebaseHeadHereItem<ToolStripMenuItem>(Revision));
		Items.Add(factory.GetRevertItem<ToolStripMenuItem>(Revision));
		Items.Add(factory.GetCherryPickItem<ToolStripMenuItem>(Revision, "{0}"));
	}

	private void AddCopyToClipboardItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
		item.DropDown.Renderer = GitterApplication.Style.ToolStripRenderer;
		item.DropDownItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, Revision.HashString));
		item.DropDownItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrTreeHash, Revision.TreeHashString));
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, Revision.Subject));
		if(!string.IsNullOrEmpty(Revision.Body))
		{
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrBody, Revision.Body));
		}
		if(Revision.Committer != Revision.Author)
		{
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitter, Revision.Committer.Name));
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitterEmail, Revision.Committer.Email));
		}
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthor, Revision.Author.Name));
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthorEmail, Revision.Author.Email));
		item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrPatch, () => Encoding.UTF8.GetString(Revision.FormatPatch())));
		Items.Add(item);
	}

	private void AddBranchItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetCreateBranchItem<ToolStripMenuItem>(Revision));

		var branches = Revision.References.GetAllBranches();
		switch(branches.Count)
		{
			case 1:
				Items.Add(factory.GetRemoveBranchItem<ToolStripMenuItem>(branches[0], "{0} '{1}'"));
				break;
			case > 1:
				var submenu = new ToolStripMenuItem(Resources.StrRemoveBranch);
				foreach(var branch in branches)
				{
					submenu.DropDownItems.Add(factory.GetRemoveBranchItem<ToolStripMenuItem>(branch, "{1}"));
				}
				Items.Add(submenu);
				break;
		}
	}

	private void AddTagItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetCreateTagItem<ToolStripMenuItem>(Revision));

		var tags = Revision.References.GetTags();
		switch(tags.Count)
		{
			case 1:
				Items.Add(factory.GetRemoveTagItem<ToolStripMenuItem>(tags[0], "{0} '{1}'"));
				break;
			case > 1:
				var submenu = new ToolStripMenuItem(Resources.StrRemoveTag);
				foreach(var tag in tags)
				{
					submenu.DropDownItems.Add(factory.GetRemoveTagItem<ToolStripMenuItem>(tag, "{1}"));
				}
				Items.Add(submenu);
				break;
		}
	}

	private void AddNoteItems(GuiItemFactory factory)
	{
		Assert.IsNotNull(factory);

		Items.Add(factory.GetAddNoteItem<ToolStripMenuItem>(Revision));
	}
}
