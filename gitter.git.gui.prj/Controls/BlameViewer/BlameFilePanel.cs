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

namespace gitter.Git.Gui.Controls;

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
	private readonly TrackingService _lineHover;
	private Size _size;
	private Dpi _sizeDpi;
	private int _selOrigin;
	private int _selStart;
	private int _selEnd;
	private bool _selecting;
	private int _hashColumnWidth;
	private int _autorColumnWidth;
	private RevisionToolTip? _revisionToolTip;

	#endregion

	private struct HitTestResults
	{
		public static readonly HitTestResults Nowhere = new() { Area = -1, Column = -1, Line = -1 };

		public int Area;
		public int Column;
		public int Line;
	}

	public BlameFilePanel(Repository repository, BlameFile blameFile)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(blameFile);

		_repository = repository;
		BlameFile = blameFile;
		_lineHover = new TrackingService(e => Invalidate(GetLineBounds(e.Index)));
		_selStart = -1;
		_selEnd = -1;
		_selOrigin = -1;
	}

	public BlameFile BlameFile { get; }

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

	protected override bool ShowHeader => false;

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
		if(FlowControl is null) return HitTestResults.Nowhere;

		int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
		if(BlameFile == null ||
			x < Margin || x > contentWidth - Margin ||
			y < 0 || y >= _size.Height)
		{
			return HitTestResults.Nowhere;
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
		var cellSize = GetCellSize(Dpi.FromControl(FlowControl));
		x -= (GetDecimalDigits(BlameFile.LineCount) + 1) * cellSize.Width + 2;
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
			Line = y / cellSize.Height,
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
						foreach(var h in BlameFile)
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

						var menu = new ContextMenuStrip
						{
							Renderer = GitterApplication.Style.ToolStripRenderer,
						};
						var dpiBindings = new DpiBindings(menu);
						var factory     = new GuiItemFactory(dpiBindings);

						var lines = GetSelectedLines();
						menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(
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
							menu.Items.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, commit.Hash.ToString()));
							menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSummary, commit.Summary));
							menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthor, commit.Author.Name));
							if(commit.Author != commit.Committer)
							{
								menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitter, commit.Committer.Name));
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
		int yOffset  = ShowHeader ? HeaderHeight : 1;
		var cellSize = GetCellSize(Dpi.FromControlOrSystem(FlowControl));
		int line = (y - yOffset) / cellSize.Height;
		if(line < 0)
		{
			line = 0;
		}
		else if(line >= BlameFile.LineCount)
		{
			line = BlameFile.LineCount - 1;
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

		if(FlowControl is null)
		{
			HideToolTip();
			return;
		}

		var htr = HitTest(x, y);
		if(_selecting)
		{
			UpdateSelection(x, y);
		}
		_lineHover.Track(htr.Line);
		if(htr.Column is 1 or 2)
		{
			if(_lineHover.Index >= 0 && _lineHover.Index < BlameFile.LineCount)
			{
				var line = GetLinesToContainingHunk(_lineHover.Index, out var blameHunk);
				var revision = default(Revision);
				if(blameHunk is not null)
				{
					try
					{
						revision = _repository.Revisions[blameHunk.Commit.Hash];
					}
					catch(Exception exc) when(!exc.IsCritical)
					{
					}
				}
				if(revision is not null && _revisionToolTip is not null)
				{
					var cellSize = GetCellSize(Dpi.FromControl(FlowControl));
					if(_revisionToolTip.Tag is not null || _revisionToolTip.Revision != revision)
					{
						_revisionToolTip.Revision = revision;
						var point = new Point(Margin, Bounds.Y - FlowControl.VScrollPos);
						point.X += (GetDecimalDigits(BlameFile.LineCount) + 1) * cellSize.Width + _autorColumnWidth + _hashColumnWidth + 1;
						point.Y += line * cellSize.Height;
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

	private int GetLinesToContainingHunk(int lineIndex, out BlameHunk? blameHunk)
	{
		int res = 0;
		for(int i = 0; i < BlameFile.Count; ++i)
		{
			if(res + BlameFile[i].Count > lineIndex)
			{
				blameHunk = BlameFile[i];
				return res;
			}
			res += BlameFile[i].Count;
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
		if(_revisionToolTip is null) return;
		if(FlowControl is null) return;

		_revisionToolTip.Hide(FlowControl);
		_revisionToolTip.Tag = "hidden";
	}

	public BlameLine[] GetSelectedLines()
	{
		if(_selStart == -1) return Preallocated<BlameLine>.EmptyArray;
		int offset = 0;
		int i = 0;
		int num = _selStart;
		while(BlameFile[i].Count <= num)
		{
			int lc = BlameFile[i].Count;
			offset += lc;
			num -= lc;
			++i;
		}
		int count = _selEnd - _selStart + 1;
		var res = new BlameLine[count];
		int id = 0;
		while(id != res.Length)
		{
			res[id++] = BlameFile[i][num++];
			if(num >= BlameFile[i].Count)
			{
				++i;
				num = 0;
			}
		}
		return res;
	}

	public int SelectionStart => _selStart;

	public int SelectionLength => _selStart == -1 ? 0 : _selEnd - _selStart + 1;

	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		if(BlameFile is null) return Size.Empty;
		var dpi = measureEventArgs.Dpi;
		if(_sizeDpi == dpi) return _size;

		int maxLength = 0;
		var longestLine = default(BlameLine);
		string longestAuthor = string.Empty;
		foreach(var hunk in BlameFile)
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
			if(hunk.Commit.Author.Name.Length > longestAuthor.Length)
			{
				longestAuthor = hunk.Commit.Author.Name;
			}
		}
		var digits   = GetDecimalDigits(BlameFile.LineCount) + 1;
		var font     = GitterApplication.FontManager.ViewerFont.ScalableFont.GetValue(dpi);
		var cellSize = GetCellSize(dpi);
		int w = cellSize.Width * digits + 2 * Margin;
		if(longestLine is not null)
		{
			int longestLineWidth;
			try
			{
				longestLineWidth = GitterApplication.TextRenderer.MeasureText(
					measureEventArgs.Graphics, longestLine.Text, font, int.MaxValue, ContentFormat).Width + (cellSize.Width / 2);
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
				longestLineWidth = (int)(maxLength * cellSize.Width);
			}
			int longestAuthorWidth;
			try
			{
				longestAuthorWidth = GitterApplication.TextRenderer.MeasureText(
					measureEventArgs.Graphics, longestAuthor, font, int.MaxValue, ContentFormat).Width + cellSize.Width;
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
				longestAuthorWidth = (int)(longestAuthor.Length * cellSize.Width);
			}
			longestAuthorWidth += cellSize.Width;
			_autorColumnWidth = longestAuthorWidth;
			w += longestLineWidth + longestAuthorWidth;
			_hashColumnWidth = cellSize.Width * 7 + cellSize.Width;
			w += _hashColumnWidth;
		}
		var h = BlameFile.LineCount * cellSize.Height;
		if(ShowHeader)
		{
			h += HeaderHeight;
		}
		if(BlameFile.LineCount != 0)
		{
			h += 2;
		}
		_sizeDpi = dpi;
		return _size = new Size(w, h);
	}

	private void PaintLine(int lineIndex,
		BlameHunk hunk, BlameLine line,
		bool paintHeader, int digits,
		Graphics graphics, Font font,
		bool hover, bool selected, bool alternate,
		int x, int y, int width, Size cellSize,
		Rectangle clipRectangle)
	{
		var rcColNumbers = new Rectangle(x, y, (digits + 1) * cellSize.Width + 2, cellSize.Height);
		var rcColNumbersBackground = Rectangle.Intersect(clipRectangle, rcColNumbers);
		if(rcColNumbersBackground is { Width: > 0, Height: > 0 })
		{
			var backgroundColor = hover
				? Style.Colors.LineNumberBackgroundHover
				: Style.Colors.LineNumberBackground;
			if(backgroundColor != Color.Transparent)
			{
				graphics.GdiFill(backgroundColor, rcColNumbersBackground);
			}
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
		int lx = x + (digits - d) * cellSize.Width + cellSize.Width / 2;
		GitterApplication.TextRenderer.DrawText(
			graphics,
			num.ToString(CultureInfo.InvariantCulture),
			font,
			Style.Colors.LineNumberForeground,
			lx, y,
			ContentFormat);
		int lineX = x;
		graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + cellSize.Height);
		lineX = x + (digits + 1) * cellSize.Width + 1;
		graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + cellSize.Height);
		lineX = x + width - Margin * 2;
		graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + cellSize.Height);
		var rcLine = new Rectangle(
			x + rcColNumbers.Width, y,
			width - 2 * Margin - rcColNumbers.Width, cellSize.Height);
		graphics.SmoothingMode = SmoothingMode.Default;

		var rcBackground = Rectangle.Intersect(clipRectangle, rcLine);
		if(rcBackground is { Width: > 0, Height: > 0 })
		{
			var backgroundColor = hover
				? selected
					? Style.Colors.LineSelectedBackgroundHover
					: Style.Colors.LineBackgroundHover
				: selected
					? Style.Colors.LineSelectedBackground
					: alternate
						? Style.Colors.Alternate
						: Style.Colors.LineContextBackground;
			if(backgroundColor != Color.Transparent)
			{
				graphics.GdiFill(backgroundColor, rcBackground);
			}
		}

		lineX = x + digits * cellSize.Width + _hashColumnWidth + _autorColumnWidth + 1;
		graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + cellSize.Height);
		var textColor = Style.Colors.WindowText;
		if(paintHeader)
		{
			var rcHash   = new Rectangle(rcLine.X, rcLine.Y, _hashColumnWidth, rcLine.Height);
			var rcAuthor = new Rectangle(rcLine.X + _hashColumnWidth, rcLine.Y, _autorColumnWidth, rcLine.Height);
			GitterApplication.TextRenderer.DrawText(
				graphics, hunk.Commit.Hash.ToString(7), font, textColor,
				rcHash.X + cellSize.Width / 2, rcHash.Y, ContentFormat);
			GitterApplication.TextRenderer.DrawText(
				graphics, hunk.Commit.Author.Name, font, textColor,
				rcAuthor.X + cellSize.Width / 2, rcAuthor.Y, ContentFormat);
		}

		var dx = _hashColumnWidth + _autorColumnWidth;
		rcLine.X     += dx;
		rcLine.Width -= dx;
		GitterApplication.TextRenderer.DrawText(
			graphics, line.Text, font, textColor,
			rcLine.X, rcLine.Y, ContentFormat);
	}

	protected override void OnFlowControlAttached(FlowLayoutControl flowControl)
	{
		base.OnFlowControlAttached(flowControl);
		_revisionToolTip ??= new();
		_revisionToolTip.Tag = "hidden";
	}

	protected override void OnFlowControlDetached(FlowLayoutControl flowControl)
	{
		base.OnFlowControlDetached(flowControl);
		DisposableUtility.Dispose(ref _revisionToolTip);
	}

	/// <inheritdoc/>
	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(FlowControl is null) return;

		var graphics = paintEventArgs.Graphics;
		var rect = paintEventArgs.Bounds;
		var clip = paintEventArgs.ClipRectangle;
		var contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
		var x = rect.X + Margin;
		var y = 0;
		if(ShowHeader)
		{
			var rcHeader = new Rectangle(rect.X + Margin, rect.Y, contentWidth - 2 * Margin, HeaderHeight);
			if(Rectangle.Intersect(clip, rcHeader) is { Width: > 0, Height: > 0 } rcHeaderClip)
			{
				PaintHeader(graphics, paintEventArgs.Dpi, rcHeader, paintEventArgs.ClipRectangle,
					GraphicsUtility.QueryIcon(BlameFile.Name, paintEventArgs.Dpi), null, BlameFile.Name);
			}
			y += rcHeader.Bottom;
		}
		else
		{
			if(BlameFile.LineCount != 0)
			{
				graphics.DrawLine(Pens.Gray, x, rect.Y, rect.X + contentWidth - Margin, rect.Y);
			}
			y += rect.Y + 1;
		}
		int maxLineNum = BlameFile.LineCount;
		int digits = GetDecimalDigits(maxLineNum);
		var font = GitterApplication.FontManager.ViewerFont.ScalableFont.GetValue(paintEventArgs.Dpi);
		bool reachedEnd = false;
		int lineIndex = 0;
		bool alternate = false;
		bool first;
		var cellSize = GetCellSize(paintEventArgs.Dpi);
		foreach(var hunk in BlameFile)
		{
			first = true;
			foreach(var line in hunk)
			{
				if(y >= clip.Bottom)
				{
					reachedEnd = true;
					break;
				}
				if(y + cellSize.Height >= clip.Y)
				{
					PaintLine(
						lineIndex, hunk, line, first, digits,
						graphics, font, lineIndex == _lineHover.Index,
						lineIndex >= _selStart && lineIndex <= _selEnd,
						alternate,
						x, y, contentWidth, cellSize, clip);
				}
				y += cellSize.Height;
				++lineIndex;
				first = false;
			}
			alternate = !alternate;
			if(reachedEnd) break;
		}
		if(!reachedEnd && BlameFile.LineCount != 0)
		{
			graphics.DrawLine(Pens.Gray, x, y, rect.X + contentWidth - Margin - 1, y);
		}
	}
}
