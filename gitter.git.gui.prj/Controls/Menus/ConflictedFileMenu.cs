namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class ConflictedFileMenu : ContextMenuStrip
	{
		private readonly TreeFile _file;

		public ConflictedFileMenu(TreeFile file)
		{
			Verify.Argument.IsValidGitObject(file, "file");
			Verify.Argument.AreEqual(StagedStatus.Unstaged, file.StagedStatus & StagedStatus.Unstaged, "file",
				"This file is not unstaged.");
			Verify.Argument.AreNotEqual(ConflictType.None, file.ConflictType, "file",
				"This file is not in conflicted state.");

			_file = file;

			Items.Add(GuiItemFactory.GetMergeToolItem<ToolStripMenuItem>(_file));
			if( _file.ConflictType != ConflictType.DeletedByUs &&
				_file.ConflictType != ConflictType.DeletedByThem &&
				_file.ConflictType != ConflictType.AddedByThem &&
				_file.ConflictType != ConflictType.AddedByUs)
			{
				var mergeTools = new ToolStripMenuItem("Select Merge Tool");
				foreach(var tool in MergeTool.KnownTools)
				{
					if(tool.SupportsWin)
					{
						mergeTools.DropDownItems.Add(GuiItemFactory.GetMergeToolItem<ToolStripMenuItem>(_file, tool));
					}
				}
				Items.Add(mergeTools);
			}

			Items.Add(new ToolStripSeparator());

			switch(_file.ConflictType)
			{
				case ConflictType.DeletedByThem:
				case ConflictType.DeletedByUs:
					Items.Add(GuiItemFactory.GetResolveConflictItem<ToolStripMenuItem>(_file, ConflictResolution.KeepModifiedFile));
					Items.Add(GuiItemFactory.GetResolveConflictItem<ToolStripMenuItem>(_file, ConflictResolution.DeleteFile));
					break;
				case ConflictType.AddedByThem:
				case ConflictType.AddedByUs:
					Items.Add(GuiItemFactory.GetResolveConflictItem<ToolStripMenuItem>(_file, ConflictResolution.KeepModifiedFile));
					Items.Add(GuiItemFactory.GetResolveConflictItem<ToolStripMenuItem>(_file, ConflictResolution.DeleteFile));
					break;
				default:
					Items.Add(GuiItemFactory.GetMarkAsResolvedItem<ToolStripMenuItem>(_file));
					Items.Add(GuiItemFactory.GetResolveConflictItem<ToolStripMenuItem>(_file, ConflictResolution.UseOurs));
					Items.Add(GuiItemFactory.GetResolveConflictItem<ToolStripMenuItem>(_file, ConflictResolution.UseTheirs));
					break;
			}
		}

		public TreeFile File
		{
			get { return _file; }
		}
	}
}
