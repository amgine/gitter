namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	sealed class StatusToolTip : CustomToolTip
	{
		private struct TextEntry
		{
			public string Text1;
			public string Text2;
			public Bitmap Image;
		}

		private List<TextEntry> _textEntries;
		private Size _size;
		private int _rowHeight;
		private int _colWidth;

		private static List<TextEntry> CaptureData(Status status)
		{
			var textEntries = new List<TextEntry>();
			lock(status.SyncRoot)
			{
				if(status.UnmergedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1 = Resources.StrConflictingFiles.AddColon(),
						Image = FileStatusIcons.ImgUnmerged,
						Text2 = status.UnmergedCount.ToString()
					});
				}
				if(status.UnstagedUntrackedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1 = Resources.StrUntrackedFiles.AddColon(),
						Image = FileStatusIcons.ImgUnstagedUntracked,
						Text2 = status.UnstagedUntrackedCount.ToString()
					});
				}
				if(status.UnstagedModifiedCount != 0 || status.UnstagedRemovedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1 = Resources.StrUnstagedChanges.AddColon(),
					});
					if(status.UnstagedRemovedCount != 0)
					{
						textEntries.Add(new TextEntry
						{
							Text1 = Resources.StrRemovedFiles.AddColon(),
							Image = FileStatusIcons.ImgUnstagedRemoved,
							Text2 = status.UnstagedRemovedCount.ToString()
						});
					}
					if(status.UnstagedModifiedCount != 0)
					{
						textEntries.Add(new TextEntry
						{
							Text1 = Resources.StrModifiedFiles.AddColon(),
							Image = FileStatusIcons.ImgUnstagedModified,
							Text2 = status.UnstagedModifiedCount.ToString()
						});
					}
				}
				if(status.StagedModifiedCount != 0 || status.StagedRemovedCount != 0 || status.StagedAddedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1 = Resources.StrStagedChanges.AddColon(),
					});
					if(status.StagedAddedCount != 0)
					{
						textEntries.Add(new TextEntry
						{
							Text1 = Resources.StrAddedFiles.AddColon(),
							Image = FileStatusIcons.ImgStagedAdded,
							Text2 = status.StagedAddedCount.ToString()
						});
					}
					if(status.StagedRemovedCount != 0)
					{
						textEntries.Add(new TextEntry
						{
							Text1 = Resources.StrRemovedFiles.AddColon(),
							Image = FileStatusIcons.ImgStagedRemoved,
							Text2 = status.StagedRemovedCount.ToString()
						});
					}
					if(status.StagedModifiedCount != 0)
					{
						textEntries.Add(new TextEntry
						{
							Text1 = Resources.StrModifiedFiles.AddColon(),
							Image = FileStatusIcons.ImgStagedModified,
							Text2 = status.StagedModifiedCount.ToString()
						});
					}
				}
			}
			if(textEntries.Count == 0)
			{
				textEntries.Add(new TextEntry { Text1 = Resources.StrsWorkingDirectoryClean });
			}
			return textEntries;
		}

		private void Measure(List<TextEntry> textEntries)
		{
			_rowHeight = 0;
			_colWidth = 0;
			var font = gitter.Framework.GitterApplication.FontManager.UIFont;
			int maxW = 0;
			using(var b = new Bitmap(1, 1))
			{
				using(var gx = Graphics.FromImage(b))
				{
					foreach(var entry in textEntries)
					{
						var mainSize = GitterApplication.TextRenderer.MeasureText(
							gx, entry.Text1, font, int.MaxValue, StringFormat.GenericTypographic);
						int entryW1 = entry.Image != null ? entry.Image.Width + 3 : 0;
						int entryW2 = mainSize.Width;
						int entryW3 = entry.Text2 != null ?
							GitterApplication.TextRenderer.MeasureText(gx, entry.Text2, font, int.MaxValue, StringFormat.GenericTypographic).Width + 15 :
							0;
						int entryH = mainSize.Height;
						if(entryH > _rowHeight) _rowHeight = entryH;
						int w = entryW1 + entryW2 + entryW3;
						if(entryW3 != 0)
						{
							var cw = entryW1 + entryW2 + 10;
							if(cw > _colWidth) _colWidth = cw;
						}
						if(w > maxW)
						{
							maxW = w;
						}
					}
				}
			}
			_size = new Size(HorizontalMargin * 2 + maxW, VerticalMargin * 2 + textEntries.Count * (_rowHeight + VerticalSpacing));
		}

		public override Size Size
		{
			get { return _size; }
		}

		public StatusToolTip()
		{
			UseFading = false;
		}

		public void Update(Status status)
		{
			_textEntries = CaptureData(status);
			Measure(_textEntries);
		}

		protected override void OnPaint(DrawToolTipEventArgs e)
		{
			if(_textEntries != null)
			{
				var gx = e.Graphics;
				var font = e.Font;
				var y = VerticalMargin;
				foreach(var entry in _textEntries)
				{
					var x = HorizontalMargin;
					if(entry.Image != null)
					{
						gx.DrawImage(entry.Image, x, y + (entry.Image.Height - _rowHeight) / 2);
						x += entry.Image.Width + 3;
					}
					GitterApplication.TextRenderer.DrawText(
						gx, entry.Text1, font, SystemBrushes.InfoText, x, y, StringFormat.GenericTypographic);
					if(entry.Text2 != null)
					{
						x = HorizontalMargin + _colWidth;
						GitterApplication.TextRenderer.DrawText(
							gx, entry.Text2, font, SystemBrushes.InfoText, x, y, StringFormat.GenericTypographic);
					}
					y += _rowHeight + VerticalSpacing;
				}
			}
		}
	}
}
