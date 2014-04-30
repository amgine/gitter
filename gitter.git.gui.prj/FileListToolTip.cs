#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class FileListToolTip : CustomToolTip
	{
		private struct TextEntry
		{
			public string Text;
			public Bitmap Image;
		}

		private List<TextEntry> _textEntries;
		private Size _size;
		private int _rowHeight;

		private const int MaximumFiles = 10;

		private static List<TextEntry> CaptureData(Status status, bool staged, FileStatus fileStatus)
		{
			int count = 0;
			var list = new List<TextEntry>();
			lock(status.SyncRoot)
			{
				switch(fileStatus)
				{
					case FileStatus.Added:
						if(staged)
						{
							count = status.StagedAddedCount;
							list.Add(new TextEntry { Text = Resources.StrAddedFiles.AddColon() });
						}
						else
						{
							count = status.UnstagedUntrackedCount;
							list.Add(new TextEntry { Text = Resources.StrUntrackedFiles.AddColon() });
						}
						break;
					case FileStatus.Removed:
						if(staged)
						{
							count = status.StagedRemovedCount;
						}
						else
						{
							count = status.UnstagedRemovedCount;
						}
						list.Add(new TextEntry { Text = Resources.StrRemovedFiles.AddColon() });
						break;
					case FileStatus.Modified:
						if(staged)
						{
							count = status.StagedModifiedCount;
						}
						else
						{
							count = status.UnstagedModifiedCount;
						}
						list.Add(new TextEntry { Text = Resources.StrModifiedFiles.AddColon() });
						break;
					case FileStatus.Unmerged:
						if(staged) throw new ArgumentException("fileStatus");
						list.Add(new TextEntry { Text = Resources.StrConflictingFiles.AddColon() });
						count = status.UnmergedCount;
						break;
					default:
						throw new ArgumentException("fileStatus");
				}
				var files = staged ? status.StagedFiles : status.UnstagedFiles;
				int i = 0;
				foreach(var file in files)
				{
					if(file.Status == fileStatus)
					{
						list.Add(new TextEntry
						{
							Text = file.RelativePath,
							Image = GraphicsUtility.QueryIcon(file.RelativePath),
						});
						++i;
						if(i >= MaximumFiles)
						{
							int remaining = count - i;
							if(remaining != 0)
							{
								list.Add(new TextEntry
								{
									Text = "{0} more files are not shown".UseAsFormat(remaining).SurroundWith('(', ')'),
								});
							}
							break;
						}
					}
				}
			}
			return list;
		}

		private void Measure(List<TextEntry> textEntries)
		{
			_rowHeight = 0;
			var font = gitter.Framework.GitterApplication.FontManager.UIFont;
			int maxW = 0;
			using(var b = new Bitmap(1, 1))
			{
				using(var gx = Graphics.FromImage(b))
				{
					foreach(var entry in textEntries)
					{
						var mainSize = GitterApplication.TextRenderer.MeasureText(
							gx, entry.Text, font, int.MaxValue, StringFormat.GenericTypographic);
						int entryW1 = entry.Image != null ? entry.Image.Width + 3 : 0;
						int entryW2 = (int)(mainSize.Width + .5f);
						int entryH = (int)(mainSize.Height + .5f);
						if(entryH > _rowHeight) _rowHeight = entryH;
						int w = entryW1 + entryW2;
						if(w > maxW) maxW = w;
					}
				}
			}
			_size = new Size(HorizontalMargin * 2 + maxW, VerticalMargin * 2 + textEntries.Count * (_rowHeight + VerticalSpacing));
		}

		public override Size Size
		{
			get { return _size; }
		}

		public FileListToolTip()
		{
			UseFading = false;
		}

		public void Update(Status status, bool staged, FileStatus fileStatus)
		{
			_textEntries = CaptureData(status, staged, fileStatus);
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
						gx, entry.Text, font, SystemBrushes.InfoText, x, y, StringFormat.GenericTypographic);
					y += _rowHeight + VerticalSpacing;
				}
			}
		}
	}
}
