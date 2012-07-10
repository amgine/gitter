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
			if(diffSource == null) throw new ArgumentNullException("branch");
			if(diffFile == null) throw new ArgumentNullException("branch");

			_diffSource = diffSource;
			_diffFile = diffFile;

			var indexDiff = diffSource as IIndexDiffSource;
			if(indexDiff != null)
			{
				var repository = indexDiff.Repository;
				if(indexDiff.Cached)
				{
				}
				else
				{
				}
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
							Items.Add(new ToolStripSeparator());
						}
					}
					catch { }
					Items.Add(GuiItemFactory.GetBlameItem<ToolStripMenuItem>(
						indexDiff.Repository.Head, diffFile.TargetFile));
					Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(
						indexDiff.Repository.Head, diffFile.TargetFile));
				}
				else
				{
					Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(
						indexDiff.Repository.Head, diffFile.SourceFile));
				}
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
