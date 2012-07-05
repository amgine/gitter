namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Context menu for <see cref="Revision"/>.</summary>
	[ToolboxItem(false)]
	public sealed class RevisionMenu : ContextMenuStrip
	{
		private readonly Revision _revision;

		/// <summary>Create <see cref="RevisionMenu"/>.</summary>
		/// <param name="revision">Related <see cref="Revision"/>.</param>
		/// <exception cref="T:System.NullReferenceException"><paramref name="revision"/> == <c>null</c>.</exception>
		public RevisionMenu(Revision revision)
		{
			if(revision == null) throw new ArgumentNullException("revision");
			_revision = revision;

			Items.Add(GuiItemFactory.GetViewDiffItem<ToolStripMenuItem>(new RevisionChangesDiffSource(_revision)));
			Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(_revision));
			Items.Add(GuiItemFactory.GetSavePatchItem<ToolStripMenuItem>(_revision));
			Items.Add(GuiItemFactory.GetArchiveItem<ToolStripMenuItem>(_revision));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCheckoutRevisionItem<ToolStripMenuItem>(_revision, "{0}"));
			Items.Add(GuiItemFactory.GetResetHeadHereItem<ToolStripMenuItem>(_revision));
			Items.Add(GuiItemFactory.GetRebaseHeadHereItem<ToolStripMenuItem>(_revision));
			Items.Add(GuiItemFactory.GetRevertItem<ToolStripMenuItem>(_revision));
			Items.Add(GuiItemFactory.GetCherryPickItem<ToolStripMenuItem>(_revision, "{0}"));

			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, revision.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrTreeHash, revision.TreeHash));
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

			Items.Add(item);

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(_revision));

			var branches = _revision.References.GetAllBranches();
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

			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(_revision));

			var tags = _revision.References.GetTags();
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
		public Revision Revision
		{
			get { return _revision; }
		}
	}
}
