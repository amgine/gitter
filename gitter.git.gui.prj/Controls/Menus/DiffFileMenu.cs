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

namespace gitter.Git.Gui.Controls;

using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class DiffFileMenu : ContextMenuStrip
{
	public DiffFileMenu(IDiffSource diffSource, DiffFile diffFile)
	{
		Verify.Argument.IsNotNull(diffSource);
		Verify.Argument.IsNotNull(diffFile);

		Renderer = GitterApplication.Style.ToolStripRenderer;

		DiffSource = diffSource;
		DiffFile   = diffFile;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		string fileName = diffFile.Status != FileStatus.Removed ? diffFile.TargetFile : diffFile.SourceFile;

		if(diffSource is IIndexDiffSource indexDiff)
		{
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
							Resources.StrOpenContainingFolder, null, Path.GetDirectoryName(fullPath) ?? ""));
						Items.Add(new ToolStripSeparator());
					}
				}
				catch(Exception exc) when(!exc.IsCritical)
				{
				}
			}
			if(indexDiff.Cached)
			{
				var item = indexDiff.Repository.Status.TryGetStaged(fileName);
				if(item is not null)
				{
					Items.Add(factory.GetUnstageItem<ToolStripMenuItem>(item));
					Items.Add(new ToolStripSeparator());
				}
			}
			else
			{
				var item = indexDiff.Repository.Status.TryGetUnstaged(fileName);
				if(item is not null)
				{
					Items.Add(factory.GetStageItem<ToolStripMenuItem>(item));
					Items.Add(new ToolStripSeparator());
				}
			}
			if(diffFile.Status != FileStatus.Removed)
			{
				Items.Add(factory.GetBlameItem<ToolStripMenuItem>(
					indexDiff.Repository.Head, fileName));
			}
			Items.Add(factory.GetPathHistoryItem<ToolStripMenuItem>(
				indexDiff.Repository.Head, fileName));
		}
		else if(diffSource is IRevisionDiffSource revisionDiff)
		{
			if(diffFile.Status != FileStatus.Removed)
			{
				Items.Add(factory.GetBlameItem<ToolStripMenuItem>(
					revisionDiff.Revision, diffFile.TargetFile));
				Items.Add(factory.GetPathHistoryItem<ToolStripMenuItem>(
					revisionDiff.Revision, diffFile.TargetFile));
			}
			else
			{
				Items.Add(factory.GetPathHistoryItem<ToolStripMenuItem>(
					revisionDiff.Revision, diffFile.SourceFile));
			}
		}
		Items.Add(new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
			new ToolStripItem[]
			{
				factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSourceFileName, diffFile.SourceFile),
				factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrDestinationFileName, diffFile.TargetFile),
			}));
	}

	public IDiffSource DiffSource { get; }

	public DiffFile DiffFile { get; }
}
