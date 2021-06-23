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

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class ReflogRecordMenu : ContextMenuStrip
	{
		public ReflogRecordMenu(ReflogRecord reflogRecord)
		{
			Verify.Argument.IsNotNull(reflogRecord, nameof(reflogRecord));

			var dpiBindings = new DpiBindings(this);
			var factory     = new GuiItemFactory(dpiBindings);

			var revision = reflogRecord.Revision;

			Items.Add(factory.GetViewDiffItem<ToolStripMenuItem>(revision.GetDiffSource()));
			Items.Add(factory.GetViewTreeItem<ToolStripMenuItem>(revision));
			Items.Add(GuiItemFactory.GetSavePatchItem<ToolStripMenuItem>(revision));

			Items.Add(new ToolStripSeparator());

			Items.Add(factory.GetCheckoutRevisionItem<ToolStripMenuItem>(revision, "{0}"));
			Items.Add(factory.GetResetHeadHereItem<ToolStripMenuItem>(revision));
			Items.Add(factory.GetCherryPickItem<ToolStripMenuItem>(revision, "{0}"));

			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, revision.Hash.ToString()));
			item.DropDownItems.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrTreeHash, revision.TreeHash.ToString()));
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrMessage, reflogRecord.Message));
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, revision.Subject));
			if(!string.IsNullOrEmpty(revision.Body))
				item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrBody, revision.Body));
			if(revision.Committer != revision.Author)
			{
				item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitter, revision.Committer.Name));
				item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitterEmail, revision.Committer.Email));
			}
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthor, revision.Author.Name));
			item.DropDownItems.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthorEmail, revision.Author.Email));
			Items.Add(item);

			Items.Add(new ToolStripSeparator());

			Items.Add(factory.GetCreateBranchItem<ToolStripMenuItem>(reflogRecord.Revision));
			Items.Add(factory.GetCreateTagItem<ToolStripMenuItem>(reflogRecord.Revision));
		}
	}
}
