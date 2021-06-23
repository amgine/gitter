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

	[ToolboxItem(false)]
	public sealed class ConflictedFileMenu : ContextMenuStrip
	{
		public ConflictedFileMenu(TreeFile file)
		{
			Verify.Argument.IsValidGitObject(file, nameof(file));
			Verify.Argument.AreEqual(StagedStatus.Unstaged, file.StagedStatus & StagedStatus.Unstaged, nameof(file),
				"This file is not unstaged.");
			Verify.Argument.AreNotEqual(ConflictType.None, file.ConflictType, nameof(file),
				"This file is not in conflicted state.");

			File = file;

			var dpiBindings = new DpiBindings(this);
			var factory     = new GuiItemFactory(dpiBindings);

			Items.Add(factory.GetMergeToolItem<ToolStripMenuItem>(File));
			if( File.ConflictType != ConflictType.DeletedByUs &&
				File.ConflictType != ConflictType.DeletedByThem &&
				File.ConflictType != ConflictType.AddedByThem &&
				File.ConflictType != ConflictType.AddedByUs)
			{
				var mergeTools = new ToolStripMenuItem("Select Merge Tool");
				foreach(var tool in MergeTool.KnownTools)
				{
					if(tool.SupportsWin)
					{
						mergeTools.DropDownItems.Add(factory.GetMergeToolItem<ToolStripMenuItem>(File, tool));
					}
				}
				Items.Add(mergeTools);
			}

			Items.Add(new ToolStripSeparator());

			switch(File.ConflictType)
			{
				case ConflictType.DeletedByThem:
				case ConflictType.DeletedByUs:
					Items.Add(factory.GetResolveConflictItem<ToolStripMenuItem>(File, ConflictResolution.KeepModifiedFile));
					Items.Add(factory.GetResolveConflictItem<ToolStripMenuItem>(File, ConflictResolution.DeleteFile));
					break;
				case ConflictType.AddedByThem:
				case ConflictType.AddedByUs:
					Items.Add(factory.GetResolveConflictItem<ToolStripMenuItem>(File, ConflictResolution.KeepModifiedFile));
					Items.Add(factory.GetResolveConflictItem<ToolStripMenuItem>(File, ConflictResolution.DeleteFile));
					break;
				default:
					Items.Add(factory.GetMarkAsResolvedItem<ToolStripMenuItem>(File));
					Items.Add(factory.GetResolveConflictItem<ToolStripMenuItem>(File, ConflictResolution.UseOurs));
					Items.Add(factory.GetResolveConflictItem<ToolStripMenuItem>(File, ConflictResolution.UseTheirs));
					break;
			}
		}

		public TreeFile File { get; }
	}
}
