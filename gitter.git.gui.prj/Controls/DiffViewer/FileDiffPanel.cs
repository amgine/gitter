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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary><see cref="FlowPanel"/> which displays diff of a single file.</summary>
public class FileDiffPanel : FilePanel
{
	#region Data

	private readonly Repository _repository;
	private readonly DiffType _diffType;
	private Size _size;
	private Dpi _sizeDpi;
	private int _digits;
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
		Verify.Argument.IsNotNull(diffFile);

		_repository = repository;
		_diffType = diffType;
		DiffFile = diffFile;
		_digits = DiffFile.HunkCount != 0
			? GetDecimalDigits(DiffFile.MaxLineNum)
			: 0;
		_lineHover = new TrackingService(OnLineHoverChanged);
		_selStart = -1;
		_selEnd = -1;
		_selOrigin = -1;
	}

	#endregion

	public DiffFile DiffFile { get; }

	private Color GetLineBackgroundColor(DiffLineState state)
		=> state switch
		{
			DiffLineState.Added   => Style.Colors.LineAddedBackground,
			DiffLineState.Removed => Style.Colors.LineRemovedBackground,
			DiffLineState.Header  => Style.Colors.LineHeaderBackground,
			_ => Style.Colors.LineContextBackground,
		};

	private Color GetLineForegroundColor(DiffLineState state)
		=> state switch
		{
			DiffLineState.Added   => Style.Colors.LineAddedForeground,
			DiffLineState.Removed => Style.Colors.LineRemovedForeground,
			DiffLineState.Header  => Style.Colors.LineHeaderForeground,
			_ => Style.Colors.LineContextForeground,
		};

	/// <inheritdoc/>
	public override void InvalidateSize()
	{
		_size = Size.Empty;
		base.InvalidateSize();
	}

	private void OnLineHoverChanged(object sender, TrackingEventArgs e)
	{
		Invalidate(GetLineBounds(e.Index, false));
	}

	/// <inheritdoc/>
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
						var menu        = new ContextMenuStrip();
						var dpiBindings = new DpiBindings(menu);
						var factory     = new GuiItemFactory(dpiBindings);
						var lines       = GetSelectedLines();

						if(_repository is not null && DiffFile.Status == FileStatus.Modified && !DiffFile.IsBinary)
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
										{
											var item = new ToolStripMenuItem(
												Resources.StrUnstageSelection, null,
												OnUnstageSelectionClick);
											dpiBindings.BindImage(item, Icons.Unstage);
											menu.Items.Add(item);
											menu.Items.Add(new ToolStripSeparator());
										}
										break;
									case DiffType.UnstagedChanges:
										{
											var item = new ToolStripMenuItem(
												Resources.StrStageSelection, null,
												OnStageSelectionClick);
											dpiBindings.BindImage(item, Icons.Stage);
											menu.Items.Add(item);
											menu.Items.Add(new ToolStripSeparator());
										}
										break;
								}
							}
						}

						menu.Items.Add(factory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines, false));
						menu.Items.Add(factory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines, true));
						if(lines.Length > 1)
						{
							menu.Items.Add(new ToolStripSeparator());
							menu.Items.Add(factory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines,
								Resources.StrCopyNewVersion, false, DiffLineState.Added | DiffLineState.Context));
							menu.Items.Add(factory.GetCopyDiffLinesItem<ToolStripMenuItem>(lines,
								Resources.StrCopyOldVersion, false, DiffLineState.Removed | DiffLineState.Context));
						}
						Utility.MarkDropDownForAutoDispose(menu);
						ShowContextMenu(menu, x, y);
					}
					else
					{
						if(htr.Area == 0)
						{
							(FlowControl as DiffViewer)?.OnFileContextMenuRequested(DiffFile);
						}
					}
				}
				break;
		}
		base.OnMouseDown(x, y, button);
	}

	private void ApplyPatchFromSelection(bool reverse)
	{
		var file = DiffFile.Cut(_selStart, _selEnd - _selStart + 1);
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
		var cellSize = GetCellSize(Dpi.FromControl(FlowControl));
		int line = (y - HeaderHeight) / cellSize.Height;
		if(line < 0)
		{
			line = 0;
		}
		else if(line >= DiffFile.LineCount)
		{
			line = DiffFile.LineCount - 1;
		}
		SetSelection(_selOrigin, line);
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(int x, int y, MouseButtons button)
	{
		_selecting = false;
		base.OnMouseUp(x, y, button);
	}

	/// <inheritdoc/>
	protected override void OnMouseDoubleClick(int x, int y, MouseButtons button)
	{
		if(button == MouseButtons.Left)
		{
			var htr = HitTest(x, y);
			if(htr.Line != -1)
			{
				var line = GetLine(htr.Line, out var hunk);
				if(line.State == DiffLineState.Header)
				{
					SetSelection(htr.Line, htr.Line + hunk.LineCount - 1);
				}
			}
			_selecting = false;
		}
		base.OnMouseDoubleClick(x, y, button);
	}

	/// <inheritdoc/>
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

	private void GetSizes(Size cellSize, out int columnWidth, out int lineHeaderWidth)
	{
		if(DiffFile.HunkCount == 0)
		{
			columnWidth     = 0;
			lineHeaderWidth = 0;
			return;
		}
		columnWidth     = _digits * cellSize.Width;
		lineHeaderWidth = columnWidth * DiffFile[0].ColumnCount;
	}

	private HitTestResults HitTest(int x, int y)
	{
		int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
		if(DiffFile is null ||
			x < Margin || x > contentWidth - Margin ||
			y < 0      || y >= _size.Height)
		{
			return new HitTestResults
			{
				Area = -1,
				Column = -1,
				Line = -1,
			};
		}
		if(y < HeaderHeight)
		{
			return new HitTestResults
			{
				Area = 0,
				Column = -1,
				Line = -1,
			};
		}
		y -= HeaderHeight;
		var dpi      = Dpi.FromControl(FlowControl);
		var cellSize = GetCellSize(dpi);
		GetSizes(cellSize, out var columnWidth, out var lineHeaderWidth);
		var line = y / cellSize.Height;
		if(line < 0 || line >= DiffFile.LineCount) line = -1;
		if(x < Margin + lineHeaderWidth)
		{
			return new HitTestResults
			{
				Area = 1,
				Column = (x - Margin) / columnWidth,
				Line = line,
			};
		}
		else
		{
			return new HitTestResults
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
		var cellSize = GetCellSize(Dpi.FromControl(FlowControl));
		if(includeLineHeader)
		{
			GetSizes(cellSize, out _, out var lineHeaderWidth);
			x += lineHeaderWidth;
			w -= lineHeaderWidth;
		}
		return new Rectangle(x, HeaderHeight + line * cellSize.Height, w, cellSize.Height);
	}

	private Rectangle GetLineBounds(int line, int count, bool includeLineHeader)
	{
		int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
		int x = Margin;
		int w = contentWidth - Margin * 2;
		var cellSize = GetCellSize(Dpi.FromControl(FlowControl));
		if(includeLineHeader)
		{
			GetSizes(cellSize, out _, out var lineHeaderWidth);
			x += lineHeaderWidth;
			w -= lineHeaderWidth;
		}
		return new Rectangle(x, HeaderHeight + line * cellSize.Height, w, cellSize.Height * count);
	}

	public int SelectionStart => _selStart;

	public int SelectionLength => _selStart == -1 ? 0 : _selEnd - _selStart + 1;

	public DiffLine[] GetSelectedLines()
	{
		if(_selStart == -1) return Preallocated<DiffLine>.EmptyArray;
		int offset = 0;
		int i = 0;
		int num = _selStart;
		while(DiffFile[i].LineCount <= num)
		{
			int lc = DiffFile[i].LineCount;
			offset += lc;
			num -= lc;
			++i;
		}
		int count = _selEnd - _selStart + 1;
		var res = new DiffLine[count];
		int id = 0;
		while(id != res.Length)
		{
			res[id++] = DiffFile[i][num++];
			if(num >= DiffFile[i].LineCount)
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
		while(DiffFile[i].LineCount <= num)
		{
			int lc = DiffFile[i].LineCount;
			offset += lc;
			num -= lc;
			++i;
		}
		return DiffFile[i][num];
	}

	private DiffLine GetLine(int num, out DiffHunk hunk)
	{
		int offset = 0;
		int i = 0;
		if(DiffFile.HunkCount == 0)
		{
			hunk = null;
			return null;
		}
		while(DiffFile[i].LineCount <= num)
		{
			int lc = DiffFile[i].LineCount;
			offset += lc;
			num -= lc;
			++i;
		}
		hunk = DiffFile[i];
		return hunk[num];
	}

	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		if(DiffFile is null) return Size.Empty;
		if(_sizeDpi == measureEventArgs.Dpi) return _size;

		int maxLength     = 0;
		int lines         = 0;
		var longestLine   = default(DiffLine);
		int largestNumber = 0;
		int maxCols       = 0;
		foreach(var hunk in DiffFile)
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
		var digits = maxCols != 0
			? GetDecimalDigits(largestNumber)
			: 0;
		var font = GitterApplication.FontManager.ViewerFont.ScalableFont.GetValue(measureEventArgs.Dpi);
		var cellSize = GetCellSize(measureEventArgs.Dpi);
		int w = cellSize.Width * maxCols * digits + 2 * Margin;
		if(longestLine is not null)
		{
			int longestLineWidth;
			try
			{
				longestLineWidth = GitterApplication.TextRenderer.MeasureText(
					measureEventArgs.Graphics, longestLine.Text, font, int.MaxValue, ContentFormat).Width + cellSize.Width;
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				longestLineWidth = (int)(maxLength * cellSize.Width);
			}
			w += longestLineWidth;
		}
		var h = HeaderHeight + lines * cellSize.Height +
			(DiffFile.LineCount != 0 ? 1 : 0);
		_sizeDpi = measureEventArgs.Dpi;
		return _size = new Size(w, h);
	}

	private static void PaintLineColumnImage(Graphics graphics, Size cellSize, int column, int digits, Image image, int x, int y)
	{
		var bounds = new Rectangle(x, y, image.Width, image.Height);
		bounds.X += (column + 1) * digits * cellSize.Width - image.Width;
		bounds.Y += (cellSize.Height - image.Height) / 2;
		graphics.DrawImage(image, bounds);
	}

	private static void PaintLineColumnText(Graphics graphics, Size cellSize, Font font, Color color, int column, int digits, int x, int y, string text)
	{
		int lx = x + ((column * digits) + (digits - text.Length)) * cellSize.Width;
		GitterApplication.TextRenderer.DrawText(
			graphics,
			text,
			font,
			color,
			lx, y,
			ContentFormat);
	}

	private void PaintLine(
		int lineIndex, DiffLine line, int digits, Graphics graphics, Size cellSize, Font font,
		bool isHovered, bool isSelected, int x, int y, int width, Rectangle clipRectangle)
	{
		int cols = line.Nums.Length;
		var rcColNumbers = new Rectangle(x, y, digits * cellSize.Width * cols + 2, cellSize.Height);
		if(cols > 0)
		{
			var rcColNumbersBackground = Rectangle.Intersect(rcColNumbers, clipRectangle);
			if(rcColNumbersBackground is { Width: > 0, Height: > 0 })
			{
				var backgroundColor = isHovered
					? Style.Colors.LineNumberBackgroundHover
					: Style.Colors.LineNumberBackground;
				graphics.GdiFill(backgroundColor, rcColNumbersBackground);
				var iconSize = new DpiConverter(FlowControl).ConvertX(16);
				for(int i = 0; i < cols; ++i)
				{
					switch(line.States[i])
					{
						case DiffLineState.Added:
							PaintLineColumnImage(graphics, cellSize, i, digits, Icons.Plus.GetImage(iconSize), x, y);
							break;
						case DiffLineState.Removed:
							PaintLineColumnImage(graphics, cellSize, i, digits, Icons.Minus.GetImage(iconSize), x, y);
							break;
						case DiffLineState.Header:
							PaintLineColumnText(graphics, cellSize, font, Style.Colors.LineNumberForeground, i, digits, x, y, "...");
							break;
						default:
							PaintLineColumnText(graphics, cellSize, font, Style.Colors.LineNumberForeground, i, digits, x, y,
								line.Nums[i].ToString(CultureInfo.InvariantCulture));
							break;
					}
					int lineX = x + i * digits * cellSize.Width + ((i == 0) ? 0 : 2);
					graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + cellSize.Height);
				}
			}
		}
		{
			int lineX = x + cols * digits * cellSize.Width + (cols != 0?1:0);
			graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + cellSize.Height);
			lineX = x + width - Margin * 2 - 1;
			graphics.DrawLine(Pens.Gray, lineX, y, lineX, y + cellSize.Height);
		}
		var rcLine = new Rectangle(
			x + rcColNumbers.Width, y,
			width - 2*Margin - rcColNumbers.Width - 1, cellSize.Height);
		var rcBackground = Rectangle.Intersect(rcLine, clipRectangle);
		if(rcBackground is { Width: > 0, Height: > 0 })
		{
			var backgroundColor = isHovered
				? isSelected
					? Style.Colors.LineSelectedBackgroundHover
					: Style.Colors.LineBackgroundHover
				: isSelected
					? Style.Colors.LineSelectedBackground
					: GetLineBackgroundColor(line.State);
			if(backgroundColor != Color.Transparent)
			{
				graphics.GdiFill(backgroundColor, rcBackground);
			}
		}
		GitterApplication.TextRenderer.DrawText(
			graphics, line.Text, font, GetLineForegroundColor(line.State), rcLine.X + cellSize.Width / 2, rcLine.Y, ContentFormat);
	}

	private string GetHeaderText()
		=> DiffFile.Status switch
		{
			FileStatus.Removed => DiffFile.SourceFile,
			FileStatus.Renamed => DiffFile.SourceFile + " -> " + DiffFile.TargetFile,
			FileStatus.Copied  => DiffFile.SourceFile + " -> " + DiffFile.TargetFile,
			_ => DiffFile.TargetFile,
		};

	private Bitmap GetHeaderIcon()
		=> GraphicsUtility.QueryIcon(
			DiffFile.Status == FileStatus.Removed
				? DiffFile.SourceFile
				: DiffFile.TargetFile, Dpi.FromControl(FlowControl));

	private IImageProvider GetHeaderIconOverlay()
		=> DiffFile.Status switch
		{
			FileStatus.Removed     => Icons.Overlays.Delete,
			FileStatus.Added       => Icons.Overlays.Add,
			FileStatus.Modified    => Icons.Overlays.Edit,
			FileStatus.Unmerged    => Icons.Overlays.Conflict,
			FileStatus.Renamed     => Icons.Overlays.Rename,
			FileStatus.Copied      => Icons.Overlays.Copy,
			FileStatus.ModeChanged => Icons.Overlays.Chmod,
			_ => null,
		};

	/// <inheritdoc/>
	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		var graphics = paintEventArgs.Graphics;
		var rect = paintEventArgs.Bounds;
		var clip = paintEventArgs.ClipRectangle;
		var contentWidth = Math.Max(_size.Width, FlowControl.ContentArea.Width);
		var rcHeader = new Rectangle(rect.X + Margin, rect.Y, contentWidth - 2 * Margin, HeaderHeight);
		var rcHeaderClip = Rectangle.Intersect(clip, rcHeader);
		if(rcHeaderClip is { Width: > 0, Height: > 0 })
		{
			graphics.SetClip(rcHeaderClip);
			var overlay = GetHeaderIconOverlay()?.GetImage(new DpiConverter(FlowControl).ConvertX(16));
			PaintHeader(graphics, paintEventArgs.Dpi, rcHeader, rcHeaderClip, GetHeaderIcon(), overlay, GetHeaderText());
		}
		var x = rect.X + Margin;
		var y = rcHeader.Bottom;
		int maxLineNum = DiffFile.MaxLineNum;
		int digits = GetDecimalDigits(maxLineNum);
		var font = GitterApplication.FontManager.ViewerFont.ScalableFont.GetValue(paintEventArgs.Dpi);
		bool reachedEnd = false;
		int lineIndex = 0;
		graphics.SetClip(clip);
		graphics.SmoothingMode = SmoothingMode.Default;
		var cellSize = GetCellSize(paintEventArgs.Dpi);
		foreach(var hunk in DiffFile)
		{
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
						lineIndex, line, digits,
						graphics, cellSize, font, lineIndex == _lineHover.Index,
						lineIndex >= _selStart && lineIndex <= _selEnd,
						x, y, contentWidth, paintEventArgs.ClipRectangle);
				}
				y += cellSize.Height;
				++lineIndex;
			}
			if(reachedEnd) break;
		}
		if(!reachedEnd && DiffFile.LineCount != 0)
		{
			graphics.DrawLine(Pens.Gray, x, y, rect.X + contentWidth - Margin - 1, y);
		}
		graphics.ResetClip();
	}
}
