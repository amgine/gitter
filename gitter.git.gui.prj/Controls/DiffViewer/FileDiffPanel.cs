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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary><see cref="FlowPanel"/> which displays diff of a single file.</summary>
	public class FileDiffPanel : FilePanel
	{
		#region Static

		private static readonly Bitmap ImgPlus = CachedResources.Bitmaps["ImgPlus"];
		private static readonly Bitmap ImgMinus = CachedResources.Bitmaps["ImgMinus"];

		#endregion

		#region Data

		private readonly Repository _repository;
		private readonly DiffType _diffType;
		private readonly DiffFile _diffFile;
		private Size _size;
		private int _digits;
		private int _columnWidth;
		private int _lineHeaderWidth;
		private TrackingService _lineHover;
		private int _selOrigin;
		private int _selStart;
		private int _selEnd;
		private bool _selecting;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="FileDiffPanel"/>.</summary>
		public FileDiffPanel(Repository repository, DiffFile diffFile, DiffType diffType)
		{
			Verify.Argument.IsNotNull(diffFile, "diffFile");

			_repository = repository;
			_diffType = diffType;
			_diffFile = diffFile;
			if(_diffFile.HunkCount != 0)
			{
				_digits = GetDecimalDigits(_diffFile.MaxLineNum);
				_columnWidth = _lineHeaderWidth = _digits * CellSize.Width;
				_lineHeaderWidth *= _diffFile[0].ColumnCount;
			}
			else
			{
				_digits = 0;
				_columnWidth = 0;
				_lineHeaderWidth = 0;
			}
			_lineHover = new TrackingService(OnLineHoverChanged);
			_selStart = -1;
			_selEnd = -1;
			_selOrigin = -1;
		}

		#endregion

		public DiffFile DiffFile
		{
			get { return _diffFile; }
		}

		private Brush GetLineBackgroundBrush(DiffLineState state)
		{
			switch(state)
			{
				case DiffLineState.Added:
					return new SolidBrush(Style.Colors.LineAddedBackground);
				case DiffLineState.Removed:
					return new SolidBrush(Style.Colors.LineRemovedBackground);
				case DiffLineState.Header:
					return new SolidBrush(Style.Colors.LineHeaderBackground);
				default:
					return new SolidBrush(Style.Colors.LineContextBackground);
			}
		}

		private Brush GetLineForegroundBrush(DiffLineState state)
		{
			switch(state)
			{
				case DiffLineState.Added:
					return new SolidBrush(Style.Colors.LineAddedForeground);
				case DiffLineState.Removed:
					return new SolidBrush(Style.Colors.LineRemovedForeground);
				case DiffLineState.Header:
					return new SolidBrush(Style.Colors.LineHeaderForeground);
				default:
					return new SolidBrush(Style.Colors.LineContextForeground);
			}
		}

		public override void InvalidateSize()
		{
			_size = Size.Empty;
			base.InvalidateSize();
		}

		private void OnLineHoverChanged(object sender, TrackingEventArgs e)
		{
			Invalidate(GetLineBounds(e.Index, false));
		}

		protected override void OnMouseMove(int x, int y)
		{
			var htr = HitTest(x, y);
			if(_selecting)
			{
				UpdateSelection(x, y);
			}
			_lineHover.Track(htr.Line);
			base.OnMouseMove(x, y);
		}

		public void DropSelection()
		{
			_selecting = false;
			if(_selStart != -1)
			{
				int invMin = _selStart;
				int invMax = _selEnd;
				_selStart = -1;
				_selEnd = -1;
				_selOrigin = -1;
				Invalidate(GetLineBounds(invMin, invMax - invMin + 1, true));
			}
		}

		private void SetSelection(int line)
		{
			SetSelection(line, line);
		}

		private void SetSelection(int line1, int line2)
		{
			int start, end;
			if(line1 <= line2)
			{
				start = line1;
				end = line2;
			}
			else
			{
				start = line2;
				end = line1;
			}
			if(_selStart != start || _selEnd != end)
			{
				if(_selStart != -1)
				{
					if(_selEnd < start || _selStart > end)
					{
						var oldStart = _selStart;
						var oldEnd = _selEnd;
						_selStart = start;
						_selEnd = end;
						Invalidate(GetLineBounds(oldStart, oldEnd - oldStart + 1, true));
						Invalidate(GetLineBounds(start, end - start + 1, true));
					}
					else
					{
						int r1Start, r1Count;
						int r2Start, r2Count;
						if(start < _selStart)
						{
							r1Start = start;
							r1Count = _selStart - r1Start + 1;
						}
						else if(start > _selStart)
						{
							r1Start = _selStart;
							r1Count = start - r1Start + 1;
						}
						else
						{
							r1Start = 0;
							r1Count = 0;
						}
						if(end > _selEnd)
						{
							r2Start = _selEnd;
							r2Count = end - r2Start + 1;
						}
						else if(end < _selEnd)
						{
							r2Start = end;
							r2Count = _selEnd - r2Start + 1;
						}
						else
						{
							r2Start = 0;
							r2Count = 0;
						}
						_selStart = start;
						_selEnd = end;
						if(r1Count != 0)
							Invalidate(GetLineBounds(r1Start, r1Count, true));
						if(r2Count != 0)
							Invalidate(GetLineBounds(r2Start, r2Count, true));
					}
				}
				else
				{
					_selStart = start;
					_selEnd = end;
					Invalidate(GetLineBounds(start, end - start + 1, true));
				}
			}
		}

		protected override void OnMouseDown(int x, int y, MouseButtons button)
		{
			switch(button)
			{
				case MouseButtons.Left:
					{
						var htr = HitTest(x, y);
						if(htr.Line != -1)
						{
							if(_selOrigin == -1 || Control.ModifierKeys != Keys.Shift)
							{
								_selOrigin = htr.Line;
								SetSelection(htr.Line);
							}
							else
							{
								SetSelection(_selOrigin, htr.Line);
							}
							_selecting = true;
						}
						else
						{
							DropSelection();
						}
					}
					break;
				case MouseButtons.Right:
					{
						var htr = HitTest(x, y);
						if(htr.Line != -1)
						{
							if(htr.Line < _selStart || htr.Line > _selEnd)
							{
								_selOrigin = htr.Line;
								SetSelection(htr.Line);
							}
							var menu = new ContextMenuStrip();
							var lines = GetSelectedLines();

							if(_repository != null && _diffFile.Status == FileStatus.Modified && !_diffFile.IsBinary)
							{
								bool hasModifiedLines = false;
								foreach(var line in lines)
								{
									if(line.State == DiffLineState.Added || line.State == DiffLineState.Removed)
									{
										hasModifiedLines = true;
										break;
									}
								}
								if(hasModifiedLines)
								{
									switch(_diffType)
									{
										case DiffType.StagedChanges:
											menu.Items.Add(new ToolStripMenuItem(
												Resources.StrUnstageSelection,
												CachedResources.Bitmaps["ImgUnstage"],
												OnUnstageSelectionClick));
											menu.Items.Add(new ToolStripSeparator());
											break;
										case DiffType.UnstagedChanges:
											menu.Items.Add(new ToolStripMenuItem(
												Resources.StrStageSelection,
												CachedResources.Bitmaps["ImgStage"],
												OnStageSelectionClick));
											menu.Items.Add(new ToolStripSeparator());
											break;
									}
								}
							}

							menu.Items.Add(GuiItemFactory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines, false));
							menu.Items.Add(GuiItemFactory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines, true));
							if(lines.Length > 1)
							{
								menu.Items.Add(new ToolStripSeparator());
								menu.Items.Add(GuiItemFactory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines,
									Resources.StrCopyNewVersion, false, DiffLineState.Added | DiffLineState.Context));
								menu.Items.Add(GuiItemFactory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines,
									Resources.StrCopyOldVersion, false, DiffLineState.Removed | DiffLineState.Context));
							}
							Utility.MarkDropDownForAutoDispose(menu);
							ShowContextMenu(menu, x, y);
						}
						else
						{
							if(htr.Area == 0)
							{
								var viewer = FlowControl as DiffViewer;
								if(viewer != null)
								{
									viewer.OnFileContextMenuRequested(_diffFile);
								}
							}
						}
					}
					break;
			}
			base.OnMouseDown(x, y, button);
		}

		private void ApplyPatchFromSelection(bool reverse)
		{
			var file = _diffFile.Cut(_selStart, _selEnd - _selStart + 1);
			var diff = new Diff(DiffType.Patch, new[] { file });
			try
			{
				_repository.Status.ApplyPatch(
					new PatchFromString(diff),
					AccessLayer.ApplyPatchTo.Index,
					reverse);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					FlowControl,
					exc.Message,
					Resources.ErrFailedToApplyPatch,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private void OnStageSelectionClick(object sender, EventArgs e)
		{
			ApplyPatchFromSelection(false);
		}

		private void OnUnstageSelectionClick(object sender, EventArgs e)
		{
			ApplyPatchFromSelection(true);
		}

		private void UpdateSelection(int x, int y)
		{
			int line = (y - HeaderHeight) / CellSize.Height;
			if(line < 0)
			{
				line = 0;
			}
			else if(line >= _diffFile.LineCount)
			{
				line = _diffFile.LineCount - 1;
			}
			SetSelection(_selOrigin, line);
		}

		protected override void OnMouseUp(int x, int y, MouseButtons button)
		{
			_selecting = false;
			base.OnMouseUp(x, y, button);
		}

		protected override void OnMouseDoubleClick(int x, int y, MouseButtons button)
		{
			if(button == MouseButtons.Left)
			{
				var htr = HitTest(x, y);
				if(htr.Line != -1)
				{
					DiffHunk hunk;
					var line = GetLine(htr.Line, out hunk);
					if(line.State == DiffLineState.Header)
					{
						SetSelection(htr.Line, htr.Line + hunk.LineCount - 1);
					}
				}
				_selecting = false;
			}
			base.OnMouseDoubleClick(x, y, button);
		}

		protected override void OnMouseLeave()
		{
			_lineHover.Drop();
			base.OnMouseLeave();
		}

		private struct HitTestResults
		{
			public int Area;
			public int Column;
			public int Line;
		}

		private HitTestResults HitTest(int x, int y)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			if(_diffFile == null ||
				x < Margin || x > contentWidth - Margin ||
				y < 0 || y >= _size.Height)
			{
				return new HitTestResults()
				{
					Area = -1,
					Column = -1,
					Line = -1,
				};
			}
			if(y < HeaderHeight)
			{
				return new HitTestResults()
				{
					Area = 0,
					Column = -1,
					Line = -1,
				};
			}
			y -= HeaderHeight;
			var line = y / CellSize.Height;
			if(line < 0 || line >= _diffFile.LineCount) line = -1;
			if(x < Margin + _lineHeaderWidth)
			{
				return new HitTestResults()
				{
					Area = 1,
					Column = (x - Margin) / _columnWidth,
					Line = line,
				};
			}
			else
			{
				return new HitTestResults()
				{
					Area = 2,
					Column = -1,
					Line = line,
				};
			}
		}

		private Rectangle GetLineBounds(int line, bool includeLineHeader)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			int x = Margin;
			int w = contentWidth - Margin * 2;
			if(includeLineHeader)
			{
				x += _lineHeaderWidth;
				w -= _lineHeaderWidth;
			}
			return new Rectangle(x, HeaderHeight + line * CellSize.Height, w, CellSize.Height);
		}

		private Rectangle GetLineBounds(int line, int count, bool includeLineHeader)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			int x = Margin;
			int w = contentWidth - Margin * 2;
			if(includeLineHeader)
			{
				x += _lineHeaderWidth;
				w -= _lineHeaderWidth;
			}
			return new Rectangle(x, HeaderHeight + line * CellSize.Height, w, CellSize.Height * count);
		}

		public int SelectionStart
		{
			get { return _selStart; }
		}

		public int SelectionLength
		{
			get { return _selStart == -1 ? 0 : _selEnd - _selStart + 1; }
		}

		public DiffLine[] GetSelectedLines()
		{
			if(_selStart == -1) return new DiffLine[0];
			int offset = 0;
			int i = 0;
			int num = _selStart;
			while(_diffFile[i].LineCount <= num)
			{
				int lc = _diffFile[i].LineCount;
				offset += lc;
				num -= lc;
				++i;
			}
			int count = _selEnd - _selStart + 1;
			var res = new DiffLine[count];
			int id = 0;
			while(id != res.Length)
			{
				res[id++] = _diffFile[i][num++];
				if(num >= _diffFile[i].LineCount)
				{
					++i;
					num = 0;
				}
			}
			return res;
		}

		private DiffLine GetLine(int num)
		{
			int offset = 0;
			int i = 0;
			while(_diffFile[i].LineCount <= num)
			{
				int lc = _diffFile[i].LineCount;
				offset += lc;
				num -= lc;
				++i;
			}
			return _diffFile[i][num];
		}

		private DiffLine GetLine(int num, out DiffHunk hunk)
		{
			int offset = 0;
			int i = 0;
			if(_diffFile.HunkCount == 0)
			{
				hunk = null;
				return null;
			}
			while(_diffFile[i].LineCount <= num)
			{
				int lc = _diffFile[i].LineCount;
				offset += lc;
				num -= lc;
				++i;
			}
			hunk = _diffFile[i];
			return hunk[num];
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			if(_diffFile == null) return Size.Empty;
			if(_size.IsEmpty)
			{
				int maxLength = 0;
				int lines = 0;
				DiffLine longestLine = null;
				int largestNumber = 0;
				int maxCols = 0;
				foreach(var hunk in _diffFile)
				{
					foreach(var line in hunk)
					{
						int l = line.GetCharacterPositions(TabSize);
						if(l > maxLength)
						{
							maxLength = l;
							longestLine = line;
						}
						++lines;
					}
					if(hunk.ColumnCount != 0)
					{
						var num = hunk.MaxLineNum;
						if(num > largestNumber)
							largestNumber = num;
						if(hunk.ColumnCount > maxCols)
						{
							maxCols = hunk.ColumnCount;
						}
					}
				}
				int digits;
				if(maxCols != 0)
				{
					digits = GetDecimalDigits(largestNumber);
				}
				else
				{
					digits = 0;
				}
				var font = GitterApplication.FontManager.ViewerFont.Font;
				int w = CellSize.Width * maxCols * digits + 2 * Margin;
				if(longestLine != null)
				{
					int longestLineWidth;
					try
					{
						longestLineWidth = GitterApplication.TextRenderer.MeasureText(
							measureEventArgs.Graphics, longestLine.Text, font, int.MaxValue, ContentFormat).Width + CellSize.Width;
					}
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
						longestLineWidth = (int)(maxLength * CellSize.Width);
					}
					w += longestLineWidth;
				}
				var h = HeaderHeight + lines * CellSize.Height +
					(_diffFile.LineCount != 0 ? 1 : 0);
				_size = new Size(w, h);
			}
			return _size;
		}

		private static void PaintLineColumnImage(Graphics graphics, int column, int digits, Image image, int x, int y)
		{
			int imageX = x + ((column + 1) * digits) * CellSize.Width - ImgPlus.Width;
			int imageY = y + (CellSize.Height - ImgPlus.Height) / 2;
			graphics.DrawImage(image, imageX, imageY);
		}

		private static void PaintLineColumnText(Graphics graphics, Font font, Brush brush, int column, int digits, int x, int y, string text)
		{
			int lx = x + ((column * digits) + (digits - text.Length)) * CellSize.Width;
			GitterApplication.TextRenderer.DrawText(
				graphics,
				text,
				font,
				brush,
				lx, y,
				ContentFormat);
		}

		private void PaintLine(
			int lineIndex, DiffLine line, int digits, Graphics graphics, Font font,
			bool isHovered, bool isSelected, int x, int y, int width)
		{
			int cols = line.Nums.Length;
			var rcColNumbers = new Rectangle(x, y, digits * CellSize.Width * cols + 2, CellSize.Height);
			Brush backgroundBrush;
			if(cols != 0)
			{
				using(backgroundBrush = isHovered ? GetLineNumberHoverBackground() : GetLineNumberBackground())
				{
					graphics.FillRectangle(backgroundBrush, rcColNumbers);
				}
				using(var brush = GetLineNumberText())
				{
					for(int i = 0; i < cols; ++i)
					{
						switch(line.States[i])
						{
							case DiffLineState.Added:
								PaintLineColumnImage(graphics, i, digits, ImgPlus, x, y);
								break;
							case DiffLineState.Removed:
								PaintLineColumnImage(graphics, i, digits, ImgMinus, x, y);
								break;
							case DiffLineState.Header:
								PaintLineColumnText(graphics, font, brush, i, digits, x, y, "...");
								break;
							default:
								PaintLineColumnText(graphics, font, brush, i, digits, x, y,
									line.Nums[i].ToString(CultureInfo.InvariantCulture));
								break;
						}
						int lineX = x + i * digits * CellSize.Width + ((i == 0) ? 0 : 2);
						graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + CellSize.Height);
					}
				}
			}
			{
				int lineX = x + cols * digits * CellSize.Width + (cols != 0?1:0);
				graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + CellSize.Height);
				lineX = x + width - Margin * 2 - 1;
				graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + CellSize.Height);
			}
			var rcLine = new Rectangle(
				x + rcColNumbers.Width, y,
				width - 2*Margin - rcColNumbers.Width- 1, CellSize.Height);
			if(isHovered)
			{
				backgroundBrush = isSelected ?
					GetLineSelectedHoverBackground() : GetLineHoverBackground();
			}
			else
			{
				backgroundBrush = isSelected ?
					GetLineSelectedBackground() : GetLineBackgroundBrush(line.State);
			}
			using(backgroundBrush)
			{
				graphics.FillRectangle(backgroundBrush, rcLine);
				using(var foregroundBrush = GetLineForegroundBrush(line.State))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, line.Text, font, foregroundBrush, rcLine.X + CellSize.Width / 2, rcLine.Y, ContentFormat);
				}
			}
		}

		private string GetHeaderText()
		{
			switch(_diffFile.Status)
			{
				case FileStatus.Removed:
					return _diffFile.SourceFile;
				case FileStatus.Renamed:
					return _diffFile.SourceFile + " -> " + _diffFile.TargetFile;
				case FileStatus.Copied:
					return _diffFile.SourceFile + " -> " + _diffFile.TargetFile;
				default:
					return _diffFile.TargetFile;
			}
		}

		private Bitmap GetHeaderIcon()
		{
			return GraphicsUtility.QueryIcon(
				_diffFile.Status == FileStatus.Removed
					? _diffFile.SourceFile
					: _diffFile.TargetFile);
		}

		private Bitmap GetHeaderIconOverlay()
		{
			switch(_diffFile.Status)
			{
				case FileStatus.Removed:
					return CachedResources.Bitmaps["ImgOverlayDel"];
				case FileStatus.Added:
					return CachedResources.Bitmaps["ImgOverlayAdd"];
				case FileStatus.Modified:
					return CachedResources.Bitmaps["ImgOverlayEdit"];
				case FileStatus.Unmerged:
					return CachedResources.Bitmaps["ImgOverlayConflict"];
				case FileStatus.Renamed:
					return CachedResources.Bitmaps["ImgOverlayRename"];
				case FileStatus.Copied:
					return CachedResources.Bitmaps["ImgOverlayCopy"];
				case FileStatus.ModeChanged:
					return CachedResources.Bitmaps["ImgOverlayChmod"];
				default:
					return null;
			}
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var clip = paintEventArgs.ClipRectangle;
			var contentWidth = Math.Max(_size.Width, FlowControl.ContentArea.Width);
			var rcHeader = new Rectangle(rect.X + Margin, rect.Y, contentWidth - 2 * Margin, HeaderHeight);
			var rcHeaderClip = Rectangle.Intersect(clip, rcHeader);
			if(rcHeaderClip.Width > 0 && rcHeaderClip.Height > 0)
			{
				graphics.SetClip(rcHeaderClip);
				PaintHeader(graphics, rcHeader, GetHeaderIcon(), GetHeaderIconOverlay(), GetHeaderText());
			}
			var x = rect.X + Margin;
			var y = rcHeader.Bottom;
			int maxLineNum = _diffFile.MaxLineNum;
			int digits = GetDecimalDigits(maxLineNum);
			var font = GitterApplication.FontManager.ViewerFont.Font;
			bool reachedEnd = false;
			int lineIndex = 0;
			graphics.SetClip(clip);
			graphics.SmoothingMode = SmoothingMode.Default;
			foreach(var hunk in _diffFile)
			{
				foreach(var line in hunk)
				{
					if(y >= clip.Bottom)
					{
						reachedEnd = true;
						break;
					}
					if(y + CellSize.Height >= clip.Y)
					{
						PaintLine(
							lineIndex, line, digits,
							graphics, font, lineIndex == _lineHover.Index,
							lineIndex >= _selStart && lineIndex <= _selEnd,
							rect.X + Margin, y, contentWidth);
					}
					y += CellSize.Height;
					++lineIndex;
				}
				if(reachedEnd) break;
			}
			if(!reachedEnd && _diffFile.LineCount != 0)
			{
				graphics.DrawLine(Pens.Gray, rect.X + Margin, y, rect.X + contentWidth - Margin - 1, y);
			}
			graphics.ResetClip();
		}
	}
}
