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
	using System.Text;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class BlameFilePanel : FilePanel
	{
		#region Data

		private readonly Repository _repository;
		private readonly BlameFile _blameFile;
		private readonly TrackingService _lineHover;
		private Size _size;
		private int _selOrigin;
		private int _selStart;
		private int _selEnd;
		private bool _selecting;
		private int _hashColumnWidth;
		private int _autorColumnWidth;
		private RevisionToolTip _revisionToolTip;

		#endregion

		private struct HitTestResults
		{
			public int Area;
			public int Column;
			public int Line;
		}

		public BlameFilePanel(Repository repository, BlameFile blameFile)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(blameFile, "blameFile");

			_repository = repository;
			_blameFile = blameFile;
			_lineHover = new TrackingService(e => Invalidate(GetLineBounds(e.Index)));
			_selStart = -1;
			_selEnd = -1;
			_selOrigin = -1;
		}

		public BlameFile BlameFile
		{
			get { return _blameFile; }
		}

		public override void InvalidateSize()
		{
			_size = Size.Empty;
			base.InvalidateSize();
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
				Invalidate(GetLineBounds(invMin, invMax - invMin + 1));
			}
		}

		protected override bool ShowHeader
		{
			get { return false; }
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
						Invalidate(GetLineBounds(oldStart, oldEnd - oldStart + 1));
						Invalidate(GetLineBounds(start, end - start + 1));
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
						{
							Invalidate(GetLineBounds(r1Start, r1Count));
						}
						if(r2Count != 0)
						{
							Invalidate(GetLineBounds(r2Start, r2Count));
						}
					}
				}
				else
				{
					_selStart = start;
					_selEnd = end;
					Invalidate(GetLineBounds(start, end - start + 1));
				}
			}
		}

		private HitTestResults HitTest(int x, int y)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			if(_blameFile == null ||
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
			if(ShowHeader)
			{
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
			}
			x -= Margin;
			int column = 3;
			x -= (GetDecimalDigits(_blameFile.LineCount) + 1) * CellSize.Width + 2;
			if(x < 0)
			{
				column = 0;
			}
			x -= _hashColumnWidth;
			if(x < 0)
			{
				column = 1;
			}
			x -= _autorColumnWidth;
			if(x < 0)
			{
				column = 2;
			}
			return new HitTestResults()
			{
				Area = 2,
				Column = column,
				Line = y / CellSize.Height,
			};
		}

		protected override void OnMouseDoubleClick(int x, int y, MouseButtons button)
		{
			switch(button)
			{
				case MouseButtons.Left:
					{
						var htr = HitTest(x, y);
						if(htr.Line != -1)
						{
							int start = 0;
							int end = 0;
							foreach(var h in _blameFile)
							{
								end = h.Count + start;
								if(end > htr.Line)
								{
									break;
								}
								else
								{
									start = end;
								}
							}
							SetSelection(start, end - 1);
						}
					}
					break;
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
							menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(
								Resources.StrCopyToClipboard,
								() => LinesToString(lines)));
							bool sameCommit = true;
							for(int i = 1; i < lines.Length; ++i)
							{
								if(lines[i].Commit != lines[0].Commit)
								{
									sameCommit = false;
									break;
								}
							}
							if(sameCommit)
							{
								var commit = lines[0].Commit;
								menu.Items.Add(new ToolStripSeparator());
								menu.Items.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, commit.Hash.ToString()));
								menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSummary, commit.Summary));
								menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthor, commit.Author));
								if(commit.Author != commit.Committer)
								{
									menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitter, commit.Committer));
								}
							}
							Utility.MarkDropDownForAutoDispose(menu);
							ShowContextMenu(menu, x, y);
						}
						else
						{
							DropSelection();
						}
					}
					break;
			}
			base.OnMouseDown(x, y, button);
		}

		private static string LinesToString(IEnumerable<BlameLine> lines)
		{
			var sb = new StringBuilder();
			foreach(var line in lines)
			{
				sb.Append(line.Text);
				sb.Append(line.Ending);
			}
			return sb.ToString();
		}

		private void UpdateSelection(int x, int y)
		{
			int yOffset = ShowHeader ? HeaderHeight : 1;
			int line = (y - yOffset) / CellSize.Height;
			if(line < 0)
			{
				line = 0;
			}
			else if(line >= _blameFile.LineCount)
			{
				line = _blameFile.LineCount - 1;
			}
			SetSelection(_selOrigin, line);
		}

		protected override void OnMouseUp(int x, int y, MouseButtons button)
		{
			_selecting = false;
			base.OnMouseUp(x, y, button);
		}

		protected override void OnMouseMove(int x, int y)
		{
			base.OnMouseMove(x, y);
			var htr = HitTest(x, y);
			if(_selecting)
			{
				UpdateSelection(x, y);
			}
			_lineHover.Track(htr.Line);
			if(htr.Column == 1 || htr.Column == 2)
			{
				if(_lineHover.Index >= 0 && _lineHover.Index < _blameFile.LineCount)
				{
					BlameHunk blameHunk;
					var line = GetLinesToContainingHunk(_lineHover.Index, out blameHunk);
					Revision revision = null;
					try
					{
						revision = _repository.Revisions[blameHunk.Commit.Hash];
					}
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
					}
					if(revision != null)
					{
						if(_revisionToolTip.Tag != null || _revisionToolTip.Revision != revision)
						{
							_revisionToolTip.Revision = revision;
							var point = new Point(Margin, Bounds.Y - FlowControl.VScrollPos);
							point.X += (GetDecimalDigits(_blameFile.LineCount) + 1) * CellSize.Width + _autorColumnWidth + _hashColumnWidth + 1;
							point.Y += line * CellSize.Height;
							point.Y += 2;
							if(ShowHeader)
							{
								point.Y += HeaderHeight;
							}
							if(point.Y < 1)
							{
								point.Y = 1;
							}
							_revisionToolTip.Show(FlowControl, point);
							_revisionToolTip.Tag = null;
						}
					}
					else
					{
						HideToolTip();
					}
				}
				else
				{
					HideToolTip();
				}
			}
			else
			{
				HideToolTip();
			}
		}

		private int GetLinesToContainingHunk(int lineIndex, out BlameHunk blameHunk)
		{
			int res = 0;
			for(int i = 0; i < _blameFile.Count; ++i)
			{
				if(res + _blameFile[i].Count > lineIndex)
				{
					blameHunk = _blameFile[i];
					return res;
				}
				res += _blameFile[i].Count;
			}
			blameHunk = null;
			return res;
		}

		protected override void OnMouseLeave()
		{
			base.OnMouseLeave();
			_lineHover.Drop();
			HideToolTip();
		}

		private void HideToolTip()
		{
			_revisionToolTip.Hide(FlowControl);
			_revisionToolTip.Tag = "hidden";
		}

		public BlameLine[] GetSelectedLines()
		{
			if(_selStart == -1) return new BlameLine[0];
			int offset = 0;
			int i = 0;
			int num = _selStart;
			while(_blameFile[i].Count <= num)
			{
				int lc = _blameFile[i].Count;
				offset += lc;
				num -= lc;
				++i;
			}
			int count = _selEnd - _selStart + 1;
			var res = new BlameLine[count];
			int id = 0;
			while(id != res.Length)
			{
				res[id++] = _blameFile[i][num++];
				if(num >= _blameFile[i].Count)
				{
					++i;
					num = 0;
				}
			}
			return res;
		}

		public int SelectionStart
		{
			get { return _selStart; }
		}

		public int SelectionLength
		{
			get { return _selStart == -1 ? 0 : _selEnd - _selStart + 1; }
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			if(_blameFile == null) return Size.Empty;
			if(_size.IsEmpty)
			{
				int maxLength = 0;
				BlameLine longestLine = null;
				string longestAuthor = string.Empty;
				foreach(var hunk in _blameFile)
				{
					foreach(var line in hunk)
					{
						int l = line.GetCharacterPositions(TabSize);
						if(l > maxLength)
						{
							maxLength = l;
							longestLine = line;
						}
					}
					if(hunk.Commit.Author.Length > longestAuthor.Length)
					{
						longestAuthor = hunk.Commit.Author;
					}
				}
				var digits = GetDecimalDigits(_blameFile.LineCount) + 1;
				var font = GitterApplication.FontManager.ViewerFont.Font;
				int w = CellSize.Width * digits + 2 * Margin;
				if(longestLine != null)
				{
					int longestLineWidth;
					try
					{
						longestLineWidth = GitterApplication.TextRenderer.MeasureText(
							measureEventArgs.Graphics, longestLine.Text, font, int.MaxValue, ContentFormat).Width + (CellSize.Width / 2);
					}
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
						longestLineWidth = (int)(maxLength * CellSize.Width);
					}
					int longestAuthorWidth;
					try
					{
						longestAuthorWidth = GitterApplication.TextRenderer.MeasureText(
							measureEventArgs.Graphics, longestAuthor, font, int.MaxValue, ContentFormat).Width + CellSize.Width;
					}
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
						longestAuthorWidth = (int)(longestAuthor.Length * CellSize.Width);
					}
					longestAuthorWidth += CellSize.Width;
					_autorColumnWidth = longestAuthorWidth;
					w += longestLineWidth + longestAuthorWidth;
					_hashColumnWidth = CellSize.Width * 7 + CellSize.Width;
					w += _hashColumnWidth;
				}
				var h = _blameFile.LineCount * CellSize.Height;
				if(ShowHeader)
				{
					h += HeaderHeight;
				}
				if(_blameFile.LineCount != 0)
				{
					h += 2;
				}
				_size = new Size(w, h);
			}
			return _size;
		}

		private void PaintLine(int lineIndex, BlameHunk hunk, BlameLine line, bool paintHeader, int digits, Graphics graphics, Font font, bool hover, bool selected, bool alternate, int x, int y, int width)
		{
			Brush backgroundBrush;

			var rcColNumbers = new Rectangle(x, y, (digits + 1) * CellSize.Width + 2, CellSize.Height);
			graphics.SmoothingMode = SmoothingMode.Default;
			using(backgroundBrush = hover ?
				GetLineNumberHoverBackground() :
				GetLineNumberBackground())
			{
				graphics.FillRectangle(backgroundBrush, rcColNumbers);
			}
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			var num = line.Number;
			var temp = num;
			int d = 0;
			while(temp != 0)
			{
				temp /= 10;
				++d;
			}
			int lx = x + ((digits - d)) * CellSize.Width + CellSize.Width / 2;
			using(var brush = GetLineNumberText())
			{
				GitterApplication.TextRenderer.DrawText(
					graphics,
					num.ToString(CultureInfo.InvariantCulture),
					font,
					brush,
					lx, y,
					ContentFormat);
			}
			int lineX = x;
			graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + CellSize.Height);
			lineX = x + (digits + 1) * CellSize.Width + 1;
			graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + CellSize.Height);
			lineX = x + width - Margin * 2;
			graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + CellSize.Height);
			var rcLine = new Rectangle(
				x + rcColNumbers.Width, y,
				width - 2 * Margin - rcColNumbers.Width, CellSize.Height);
			graphics.SmoothingMode = SmoothingMode.Default;
			if(hover)
			{
				backgroundBrush = selected ?
					GetLineSelectedHoverBackground() :
					GetLineHoverBackground();
			}
			else
			{
				backgroundBrush = selected ?
					GetLineSelectedBackground() :
					(alternate ?
						new SolidBrush(Style.Colors.Alternate) :
						new SolidBrush(Style.Colors.LineContextBackground));
			}
			using(backgroundBrush)
			{
				graphics.FillRectangle(backgroundBrush, rcLine);
			}
			lineX = x + digits * CellSize.Width + _hashColumnWidth + _autorColumnWidth + 1;
			graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + CellSize.Height);
			using(var brush = new SolidBrush(Style.Colors.WindowText))
			{
				if(paintHeader)
				{
					var rcHash   = new Rectangle(rcLine.X, rcLine.Y, _hashColumnWidth, rcLine.Height);
					var rcAuthor = new Rectangle(rcLine.X + _hashColumnWidth, rcLine.Y, _autorColumnWidth, rcLine.Height);
					GitterApplication.TextRenderer.DrawText(
						graphics, hunk.Commit.Hash.ToString(7), font, brush,
						rcHash.X + CellSize.Width / 2, rcHash.Y, ContentFormat);
					GitterApplication.TextRenderer.DrawText(
						graphics, hunk.Commit.Author, font, brush,
						rcAuthor.X + CellSize.Width / 2, rcAuthor.Y, ContentFormat);
				}

				rcLine.X += _hashColumnWidth + _autorColumnWidth;
				rcLine.Width -= _hashColumnWidth + _autorColumnWidth;
				GitterApplication.TextRenderer.DrawText(
					graphics, line.Text, font, brush,
					rcLine.X, rcLine.Y, ContentFormat);
			}
		}

		protected override void OnFlowControlAttached()
		{
			base.OnFlowControlAttached();
			_revisionToolTip = new RevisionToolTip();
			_revisionToolTip.Tag = "hidden";
		}

		protected override void OnFlowControlDetached()
		{
			base.OnFlowControlDetached();
			_revisionToolTip.Dispose();
			_revisionToolTip = null;
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var clip = paintEventArgs.ClipRectangle;
			var contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			var x = rect.X + Margin;
			var y = 0;
			if(ShowHeader)
			{
				var rcHeader = new Rectangle(rect.X + Margin, rect.Y, contentWidth - 2 * Margin, HeaderHeight);
				var rcHeaderClip = Rectangle.Intersect(clip, rcHeader);
				if(rcHeaderClip.Width != 0 && rcHeaderClip.Height != 0)
				{
					PaintHeader(graphics, rcHeader, GraphicsUtility.QueryIcon(_blameFile.Name), null, _blameFile.Name);
				}
				y += rcHeader.Bottom;
			}
			else
			{
				if(_blameFile.LineCount != 0)
				{
					graphics.DrawLine(Pens.Gray, x, rect.Y, rect.X + contentWidth - Margin, rect.Y);
				}
				y += rect.Y + 1;
			}
			int maxLineNum = _blameFile.LineCount;
			int digits = GetDecimalDigits(maxLineNum);
			var font = GitterApplication.FontManager.ViewerFont.Font;
			bool reachedEnd = false;
			int lineIndex = 0;
			bool alternate = false;
			bool first;
			foreach(var hunk in _blameFile)
			{
				first = true;
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
							lineIndex, hunk, line, first, digits,
							graphics, font, lineIndex == _lineHover.Index,
							lineIndex >= _selStart && lineIndex <= _selEnd,
							alternate,
							x, y, contentWidth);
					}
					y += CellSize.Height;
					++lineIndex;
					first = false;
				}
				alternate = !alternate;
				if(reachedEnd) break;
			}
			if(!reachedEnd && _blameFile.LineCount != 0)
			{
				graphics.DrawLine(Pens.Gray, x, y, rect.X + contentWidth - Margin - 1, y);
			}
		}
	}
}
