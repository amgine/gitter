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
