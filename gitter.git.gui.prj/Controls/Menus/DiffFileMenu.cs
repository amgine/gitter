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
	using System.IO;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class DiffFileMenu : ContextMenuStrip
	{
		private readonly IDiffSource _diffSource;
		private readonly DiffFile _diffFile;

		public DiffFileMenu(IDiffSource diffSource, DiffFile diffFile)
		{
			Verify.Argument.IsNotNull(diffSource, "diffSource");
			Verify.Argument.IsNotNull(diffFile, "diffFile");

			_diffSource = diffSource;
			_diffFile = diffFile;

			string fileName = diffFile.Status != FileStatus.Removed ? diffFile.TargetFile : diffFile.SourceFile;

			var indexDiff = diffSource as IIndexDiffSource;
			if(indexDiff != null)
			{
				var repository = indexDiff.Repository;
				if(diffFile.Status != FileStatus.Removed)
				{
					try
					{
						var fullPath = Path.Combine(diffSource.Repository.WorkingDirectory, diffFile.TargetFile);
						if(File.Exists(fullPath))
						{
							Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(
								Resources.StrOpen, null, fullPath));
							Items.Add(GuiItemFactory.GetOpenUrlWithItem<ToolStripMenuItem>(
								Resources.StrOpenWith.AddEllipsis(), null, fullPath));
							Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(
								Resources.StrOpenContainingFolder, null, Path.GetDirectoryName(fullPath)));
							Items.Add(new ToolStripSeparator());
						}
					}
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
					}
				}
				if(indexDiff.Cached)
				{
					var item = indexDiff.Repository.Status.TryGetStaged(fileName);
					if(item != null)
					{
						Items.Add(GuiItemFactory.GetUnstageItem<ToolStripMenuItem>(item));
						Items.Add(new ToolStripSeparator());
					}
				}
				else
				{
					var item = indexDiff.Repository.Status.TryGetUnstaged(fileName);
					if(item != null)
					{
						Items.Add(GuiItemFactory.GetStageItem<ToolStripMenuItem>(item));
						Items.Add(new ToolStripSeparator());
					}
				}
				if(diffFile.Status != FileStatus.Removed)
				{
					Items.Add(GuiItemFactory.GetBlameItem<ToolStripMenuItem>(
						indexDiff.Repository.Head, fileName));
				}
				Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(
					indexDiff.Repository.Head, fileName));
			}
			else
			{
				var revisionDiff = diffSource as IRevisionDiffSource;
				if(revisionDiff != null)
				{
					if(diffFile.Status != FileStatus.Removed)
					{
						Items.Add(GuiItemFactory.GetBlameItem<ToolStripMenuItem>(
							revisionDiff.Revision, diffFile.TargetFile));
						Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(
							revisionDiff.Revision, diffFile.TargetFile));
					}
					else
					{
						Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(
							revisionDiff.Revision, diffFile.SourceFile));
					}
				}
			}
			Items.Add(new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
				new ToolStripItem[]
				{
					GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSourceFileName, diffFile.SourceFile),
					GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrDestinationFileName, diffFile.TargetFile),
				}));
		}

		public IDiffSource DiffSource
		{
			get { return _diffSource; }
		}

		public DiffFile DiffFile
		{
			get { return _diffFile; }
		}
	}
}
