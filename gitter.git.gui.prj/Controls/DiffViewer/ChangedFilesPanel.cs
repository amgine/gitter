#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
#if NETCOREAPP
using System.Collections.Immutable;
#else
using System.Collections.Generic;
#endif
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;
using System.Drawing.Drawing2D;

/// <summary><see cref="FlowPanel"/> which displays changed files.</summary>
public class ChangedFilesPanel : FlowPanel
{
	#region Static

	private static readonly StringFormat HeaderFormat = new(StringFormat.GenericTypographic)
	{
		Alignment = StringAlignment.Near,
		FormatFlags =
			StringFormatFlags.LineLimit |
			StringFormatFlags.FitBlackBox,
		LineAlignment = StringAlignment.Center,
		Trimming = StringTrimming.None,
	};

	private static readonly StringFormat ContentFormat = new(StringFormat.GenericTypographic)
	{
		Alignment = StringAlignment.Near,
		FormatFlags =
			StringFormatFlags.LineLimit |
			StringFormatFlags.FitBlackBox,
		LineAlignment = StringAlignment.Near,
		Trimming = StringTrimming.EllipsisPath,
	};

	private static readonly StringFormat DiffStatFormat = new(StringFormat.GenericDefault)
	{
		Alignment = StringAlignment.Far,
		FormatFlags =
			StringFormatFlags.LineLimit |
			StringFormatFlags.NoClip |
			StringFormatFlags.NoWrap,
		LineAlignment = StringAlignment.Center,
		Trimming = StringTrimming.None,
	};

	private static readonly StringFormat DiffStatCenterFormat = new(StringFormat.GenericDefault)
	{
		Alignment = StringAlignment.Center,
		FormatFlags =
			StringFormatFlags.LineLimit |
			StringFormatFlags.NoClip |
			StringFormatFlags.NoWrap,
		LineAlignment = StringAlignment.Center,
		Trimming = StringTrimming.None,
	};

	#endregion

	private static readonly IDpiBoundValue<int> LineHeight = DpiBoundValue.ScaleY(16 + 4);
	private static readonly IDpiBoundValue<int> HeaderHeight = DpiBoundValue.ScaleY(16 + 4);
	private static readonly IDpiBoundValue<int> HeaderBottomMargin = DpiBoundValue.ScaleY(3);
	private static readonly IDpiBoundValue<int> HeaderContentPadding = DpiBoundValue.ScaleY(3);
	private static readonly IDpiBoundValue<int> HeaderSpacing = DpiBoundValue.ScaleY(7);

	private Diff? _diff;
	private FileItem[]? _items;
	#if NETCOREAPP
	private readonly ImmutableDictionary<FileStatus, ChangesCountByType> _changesByTypeLookup;
	#else
	private readonly Dictionary<FileStatus, ChangesCountByType> _changesByTypeLookup;
	#endif
	private readonly ChangesCountByType[] _changesByTypeList;

	private static IImageProvider? GetOverlay(FileStatus fileStatus)
		=> fileStatus switch
		{
			FileStatus.Removed     => Icons.Overlays.Delete,
			FileStatus.Added       => Icons.Overlays.Add,
			FileStatus.Modified    => Icons.Overlays.Edit,
			FileStatus.Unmerged    => Icons.Overlays.Conflict,
			FileStatus.Copied      => Icons.Overlays.Copy,
			FileStatus.Renamed     => Icons.Overlays.Rename,
			FileStatus.ModeChanged => Icons.Overlays.Chmod,
			_ => default,
		};

	private sealed class FileItem
	{
		private readonly DiffFile _file;
		private readonly string _fileName;
		private readonly string _text;

		public FileItem(DiffFile file)
		{
			Verify.Argument.IsNotNull(file);

			_file = file;

			if(file.Status == FileStatus.Removed)
			{
				_text = file.SourceFile;
				_fileName = _text;
			}
			else
			{
				_text = file.TargetFile;
				_fileName = _text;
			}
			switch(file.Status)
			{
				case FileStatus.Copied or FileStatus.Renamed:
					_text = GetRenamedOrCopiedText(file.SourceFile, file.TargetFile);
					break;
			}
		}

		private static string GetRenamedOrCopiedText(string from, string to)
		{
			/*
			int index = 0;
			int startIndex = 0;
			while(index < to.Length && index < from.Length)
			{
				if(from[index] != to[index]) break;
				if(to[index] == '/')
				{
					startIndex = index + 1;
				}
				++index;
			}
			if(startIndex != 0)
			{
				bool shortenName = true;
				for(int i = startIndex; i < to.Length; ++i)
				{
					if(to[i] == '/')
					{
						shortenName = false;
						break;
					}
				}
				if(shortenName)
				{
					to = to.Substring(startIndex);
				}
			}
			*/
			return from + " -> " + to;
		}

		public DiffFile File => _file;

		private void PaintDiffStats(Graphics graphics, Dpi dpi, Font font, DpiConverter conv, Color textColor, Rectangle rect)
		{
			const int squares = 10;
			int squareWidth   = conv.ConvertX(4);
			int squareHeight  = conv.ConvertY(13);
			int squareSpacing = conv.ConvertX(1);
			const float radius = 1.5f;
			var oldMode = graphics.SmoothingMode;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			if(_file.IsBinary)
			{
				var rc = new Rectangle(
					rect.Right - squareWidth * squares - squareSpacing * (squares - 1), rect.Y + (rect.Height - squareHeight) / 2,
					squareWidth * squares + squareSpacing * (squares - 1), squareHeight);
				graphics.FillRoundedRectangle(Brushes.Gray, rc, radius);
				GitterApplication.TextRenderer.DrawText(
					graphics,
					Resources.StrlBinary,
					font,
					textColor,
					rc,
					DiffStatCenterFormat);
			}
			else
			{
				var added = _file.Stats.AddedLinesCount;
				var removed = _file.Stats.RemovedLinesCount;
				var changed = added + removed;
				int n1, n2;
				if(changed == 0 && _file.Status == FileStatus.Renamed)
				{
					var rc = new Rectangle(
						rect.Right - squareWidth * squares - squareSpacing * (squares - 1), rect.Y + (rect.Height - squareHeight) / 2,
						squareWidth * squares + squareSpacing * (squares - 1), squareHeight);
					graphics.FillRoundedRectangle(Brushes.DarkCyan, rc, radius);
					GitterApplication.TextRenderer.DrawText(
						graphics,
						Resources.StrlRename,
						font,
						Color.White,
						rc,
						DiffStatCenterFormat);
				}
				else if(changed == 0 && _file.Status == FileStatus.Copied)
				{
					var rc = new Rectangle(
						rect.Right - squareWidth * squares - squareSpacing * (squares - 1), rect.Y + (rect.Height - squareHeight) / 2,
						squareWidth * squares + squareSpacing * (squares - 1), squareHeight);
					graphics.FillRoundedRectangle(Brushes.DarkGreen, rc, radius);
					GitterApplication.TextRenderer.DrawText(
						graphics,
						Resources.StrlCopy,
						font,
						Color.White,
						rc,
						DiffStatCenterFormat);
				}
				else
				{
					var rc = new Rectangle(
						rect.Right - squareWidth, rect.Y + (rect.Height - squareHeight) / 2,
						squareWidth, squareHeight);
					if(changed <= squares)
					{
						n1 = added;
						n2 = removed;
					}
					else
					{
						n1 = added * squares / changed;
						n2 = removed * squares / changed;
					}
					int n3 = squares - n1 - n2;

					Brush brush;
					for(int i = 0; i < squares; ++i)
					{
						if(n3 != 0)
						{
							brush = Brushes.Gray;
							--n3;
						}
						else if(n2 != 0)
						{
							brush = Brushes.Red;
							--n2;
						}
						else
						{
							brush = Brushes.Green;
							--n1;
						}
						graphics.FillRoundedRectangle(brush, rc, radius);
						rc.X -= squareWidth + squareSpacing;
					}
					rect.Width -= (squareWidth + squareSpacing) * squares + conv.ConvertX(2);
					if(rect.Width > 2)
					{
						GitterApplication.TextRenderer.DrawText(
							graphics,
							changed.ToString(CultureInfo.InvariantCulture),
							font,
							textColor,
							rect,
							DiffStatFormat);
					}
				}
			}
			graphics.SmoothingMode = oldMode;
		}

		private bool DrawIcon(Graphics graphics, DpiConverter conv, Rectangle iconRect)
		{
			var icon = GraphicsUtility.QueryIcon(_fileName, conv.To);
			if(icon is null) return false;
			graphics.DrawImage(icon, iconRect);
			var overlay = GetOverlay(File.Status)?.GetImage(conv.ConvertX(16));
			if(overlay is null) return false;
			graphics.DrawImage(overlay, iconRect);
			return true;
		}

		public void Draw(Graphics graphics, Dpi dpi, Font font, DpiConverter conv, Color textColor, Rectangle rect)
		{
			var iconSize = conv.Convert(new Size(16, 16));
			var statSize = conv.ConvertX((9 + 1) * 5 + 20);

			int dy = (rect.Height - iconSize.Height) / 2;
			var iconRect = new Rectangle(rect.X + dy, rect.Y + dy, iconSize.Width, iconSize.Height);
			DrawIcon(graphics, conv, iconRect);
			var dx = dy + iconSize.Width + conv.ConvertX(3);
			rect.X += dx;
			rect.Width -= dx;
			var statRect = new Rectangle(rect.Right - statSize - conv.ConvertX(3), rect.Y, statSize, rect.Height);
			if(rect.Width > statSize * 2)
			{
				PaintDiffStats(graphics, dpi, font, conv, textColor, statRect);
				rect.Width -= statSize + conv.ConvertX(3);
			}
			const TextFormatFlags flags =
				TextFormatFlags.Left |
				TextFormatFlags.VerticalCenter |
				TextFormatFlags.SingleLine |
				TextFormatFlags.PathEllipsis |
				TextFormatFlags.NoPadding |
				TextFormatFlags.PreserveGraphicsClipping;
			TextRenderer.DrawText(graphics, _text, font, rect, textColor, flags);
		}
	}

	private sealed class ChangesCountByType(FileStatus status)
	{
		private int _count;
		private string? _displayText;

		private string GetDisplayText()
		{
			var suffix = Status switch
			{
				StatusFilterAll when Count == 1 => Resources.StrlChange,
				StatusFilterAll                 => Resources.StrlChanges,
				FileStatus.Added       => Resources.StrlAdded,
				FileStatus.Modified    => Resources.StrlModified,
				FileStatus.Removed     => Resources.StrlRemoved,
				FileStatus.Unmerged    => Resources.StrlUnmerged,
				FileStatus.Copied      => Resources.StrlCopied,
				FileStatus.Renamed     => Resources.StrlRenamed,
				FileStatus.ModeChanged => Resources.StrlChmod,
				_ => default,
			};
			return Count.ToString(CultureInfo.InvariantCulture) + " " + suffix;
		}

		public IImageProvider? ImageOverlay { get; } = GetOverlay(status);

		public FileStatus Status { get; } = status;

		public int Count
		{
			get => _count;
			set
			{
				if(_count == value) return;

				_count = value;
				_displayText = default;
			}
		}

		public string DisplayText => _displayText ??= GetDisplayText();

		public Rectangle DisplayBounds { get; set; }
	}

	private readonly TrackingService<FileItem> _fileHover;
	private readonly TrackingService<ChangesCountByType> _filterHover;

	private static IDpiBoundValue<Font> Font => GitterApplication.FontManager.UIFont.ScalableFont;

	private const FileStatus StatusFilterAll =
		FileStatus.Added | FileStatus.Removed | FileStatus.Modified |
		FileStatus.Renamed | FileStatus.Copied | FileStatus.ModeChanged | FileStatus.Unmerged;

	private FileStatus _statusFilter;

	#region Events

	public event EventHandler? StatusFilterChanged;

	private void OnStatusFilterChanged()
		=> StatusFilterChanged?.Invoke(this, EventArgs.Empty);

	public event EventHandler<DiffFileEventArgs>? FileNavigationRequested;

	private void OnFileNavigationRequested(DiffFile diffFile)
		=> FileNavigationRequested?.Invoke(this, new DiffFileEventArgs(diffFile));

	#endregion

	/// <summary>Create <see cref="ChangedFilesPanel"/>.</summary>
	public ChangedFilesPanel()
	{
		_fileHover    = new TrackingService<FileItem>(OnFileHoverChanged);
		_filterHover  = new TrackingService<ChangesCountByType>(OnStatusFilterHoverChanged);
		_statusFilter = StatusFilterAll;

		_changesByTypeList =
		[
			new(StatusFilterAll),
			new(FileStatus.Added),
			new(FileStatus.Removed),
			new(FileStatus.Modified),
			new(FileStatus.Renamed),
			new(FileStatus.Copied),
			new(FileStatus.Unmerged),
			new(FileStatus.ModeChanged),
		];
#if NETCOREAPP
		var builder = ImmutableDictionary.CreateBuilder<FileStatus, ChangesCountByType>();
		foreach(var item in _changesByTypeList)
		{
			builder.Add(item.Status, item);
		}
		_changesByTypeLookup = builder.ToImmutable();
#else
		_changesByTypeLookup = new(capacity: _changesByTypeList.Length);
		foreach(var item in _changesByTypeList)
		{
			_changesByTypeLookup.Add(item.Status, item);
		}
#endif
	}

	private void OnFileHoverChanged(object? sender, TrackingEventArgs<FileItem> e)
	{
		if(FlowControl is null) return;

		var dpi = Dpi.FromControl(FlowControl);
		var lineHeight = LineHeight.GetValue(dpi);
		var rc = new Rectangle(
			0,
			HeaderHeight.GetValue(dpi) + HeaderBottomMargin.GetValue(dpi) + GetVisualIndex(e.Index) * lineHeight,
			FlowControl.ContentArea.Width, lineHeight);
		Invalidate(rc);
		FlowControl.Cursor = e.IsTracked ? Cursors.Hand : Cursors.Default;
	}

	private void OnStatusFilterHoverChanged(object? sender, TrackingEventArgs<ChangesCountByType> e)
	{
		if(FlowControl is null) return;

		var dpi = Dpi.FromControl(FlowControl);
		Invalidate(new Rectangle(0, 0, FlowControl.ContentArea.Width, HeaderHeight.GetValue(dpi)));
		FlowControl.Cursor = e.IsTracked ? Cursors.Hand : Cursors.Default;
	}

	/// <summary>Displayed diff.</summary>
	public Diff? Diff
	{
		get => _diff;
		set
		{
			if(_diff == value) return;

			_diff = value;
			for(int i = 0; i < _changesByTypeList.Length; ++i)
			{
				_changesByTypeList[i].Count = 0;
			}
			if(_diff is not null)
			{
				_changesByTypeList[0].Count = _diff.FilesCount;
				if(_items is null || _items.Length != _diff.FilesCount)
				{
					_items = new FileItem[_diff.FilesCount];
				}
				for(int i = 0; i < _diff.FilesCount; ++i)
				{
					var file = _diff[i];
					_items[i] = new FileItem(file);
					if(_changesByTypeLookup.TryGetValue(file.Status, out var changes))
					{
						++changes.Count;
					}
				}
			}
			else
			{
				_items = Preallocated<FileItem>.EmptyArray;
			}
			_fileHover.Reset(-1, null);
		}
	}

	public FileStatus StatusFilter
	{
		get => _statusFilter;
		private set
		{
			if(_statusFilter == value) return;

			_statusFilter = value;
			OnStatusFilterChanged();
			InvalidateSize();
		}
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave()
	{
		_fileHover.Drop();
		_filterHover.Drop();
	}

	private int GetVisualIndex(int index)
	{
		if(_items is null) return -1;

		int visualIndex = -1;
		for(int i = 0; i <= index; ++i)
		{
			if((_items[i].File.Status & StatusFilter) != FileStatus.Unknown)
			{
				++visualIndex;
			}
		}
		return visualIndex;
	}

	private int HitTestFile(int x, int y)
	{
		if(FlowControl is null) return -1;
		if(_items is null) return -1;

		var dpi  = Dpi.FromControl(FlowControl);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var marginX = conv.ConvertX(5);
		if(x < marginX || x >= FlowControl.ContentArea.Width - marginX) return -1;
		y -= HeaderHeight.GetValue(dpi) + HeaderBottomMargin.GetValue(dpi);
		if(y < 0) return -1;

		int id = y / LineHeight.GetValue(dpi);
		if(id < 0 || id >= _items.Length)
		{
			return -1;
		}
		if(StatusFilter == StatusFilterAll)
		{
			return id;
		}
		int visualIndex = -1;
		for(int i = 0; i < _items.Length; ++i)
		{
			if(!Filter(_items[i])) continue;
			++visualIndex;
			if(visualIndex == id)
			{
				return i;
			}
		}
		return -1;
	}

	private int HitTestFilter(int x, int y)
	{
		if(FlowControl is null) return -1;

		var dpi = Dpi.FromControl(FlowControl);
		if(y < 0 || y > HeaderHeight.GetValue(dpi)) return -1;
		for(int i = 0; i < _changesByTypeList.Length; ++i)
		{
			if(_changesByTypeList[i].DisplayBounds.Contains(x, y))
			{
				return i;
			}
		}
		return -1;
	}

	protected override void OnMouseDown(int x, int y, MouseButtons button)
	{
		if(FlowControl is null) return;
		if(_items is null) return;

		switch(button)
		{
			case MouseButtons.Left:
				{
					int id = HitTestFile(x, y);
					if(id != -1)
					{
						var file = _items[id].File;
						bool found = false;
						foreach(var panel in FlowControl.Panels)
						{
							if(panel is FileDiffPanel diffpanel && diffpanel.DiffFile == file)
							{
								diffpanel.ScrollIntoView();
								found = true;
								break;
							}
						}
						if(!found)
						{
							OnFileNavigationRequested(file);
						}
					}
					else
					{
						id = HitTestFilter(x, y);
						if(id != -1)
						{
							StatusFilter = _changesByTypeList[id].Status;
						}
					}
				}
				break;
			case MouseButtons.Right:
				{
					int id = HitTestFile(x, y);
					if(id != -1)
					{
						var file = _items[id].File;
						if(FlowControl is DiffViewer viewer)
						{
							viewer.OnFileContextMenuRequested(file);
						}
					}
				}
				break;
		}
		base.OnMouseDown(x, y, button);
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(int x, int y)
	{
		int id = HitTestFile(x, y);
		if(id == -1)
		{
			_fileHover.Drop();
			id = HitTestFilter(x, y);
			if(id == -1)
			{
				_filterHover.Drop();
			}
			else
			{
				_filterHover.Track(id, _changesByTypeList[id]);
			}
		}
		else
		{
			_filterHover.Drop();
			if(_items is not null)
			{
				_fileHover.Track(id, _items[id]);
			}
		}
		base.OnMouseMove(x, y);
	}

	private int GetLineCount()
	{
		if(_items is null) return default;
		if(StatusFilter == StatusFilterAll)
		{
			return _items.Length;
		}
		var count = 0;
		foreach(var item in _items)
		{
			if(Filter(item)) ++count;
		}
		return count;
	}

	private bool Filter(FileItem item)
		=> (item.File.Status & StatusFilter) != FileStatus.Unknown;

	/// <inheritdoc/>
	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(_diff is null) return Size.Empty;

		var lineCount = GetLineCount();
		return new Size(0,
			HeaderHeight.GetValue(measureEventArgs.Dpi) +
			(lineCount == 0 ? 0 : HeaderBottomMargin.GetValue(measureEventArgs.Dpi)) +
			lineCount * LineHeight.GetValue(measureEventArgs.Dpi));
	}

	private void PaintHeader(Graphics graphics, Font font, int x, int y, Rectangle rcHeader, DpiConverter conv, Rectangle clip)
	{
		var headerBounds = rcHeader;

		var dx = conv.ConvertX(5);
		headerBounds.X     += dx;
		headerBounds.Width -= dx;

		var iconSize  = conv.Convert(new Size(16, 16));
		var textColor = Style.Colors.WindowText;
		var spacing   = HeaderSpacing.GetValue(conv.To);

		for(int i = 0; i < _changesByTypeList.Length; ++i)
		{
			if(headerBounds.Width <= 0) break;

			if(i != 0 && _changesByTypeList[i].Count == 0)
			{
				_changesByTypeList[i].DisplayBounds = Rectangle.Empty;
				continue;
			}
			// prepare
			var headerText = _changesByTypeList[i].DisplayText;
			var headerTextSize = GitterApplication.TextRenderer.MeasureText(
				graphics, headerText, font, short.MaxValue, ContentFormat);
			var headerWidth = headerTextSize.Width;
			var overlay = _changesByTypeList[i].ImageOverlay?.GetImage(iconSize.Width);
			var displayBounds = new Rectangle(
				headerBounds.X - HeaderContentPadding.GetValue(conv.To),
				headerBounds.Y,
				headerWidth + (overlay is not null ? iconSize.Width + conv.ConvertX(3) : 0) + HeaderContentPadding.GetValue(conv.To) * 2,
				headerBounds.Height);
			_changesByTypeList[i].DisplayBounds = new Rectangle(
				displayBounds.X - x, displayBounds.Y - y, displayBounds.Width, displayBounds.Height);
			// background
			if(StatusFilter == _changesByTypeList[i].Status)
			{
				Style.ItemBackgroundStyles.Selected.Draw(graphics, new(conv.To, displayBounds, clip));
			}
			else if(_filterHover.Index == i)
			{
				Style.ItemBackgroundStyles.Hovered.Draw(graphics, new(conv.To, displayBounds));
			}
			// header icon
			if(overlay is not null)
			{
				var image = CommonIcons.File.GetImage(iconSize.Width);
				if(image is not null)
				{
					var imageBounds = new Rectangle(
						headerBounds.X,
						headerBounds.Y + (headerBounds.Height - iconSize.Height) / 2,
						iconSize.Width, iconSize.Height);
					graphics.DrawImage(image,   imageBounds);
					graphics.DrawImage(overlay, imageBounds);
					dx = iconSize.Width + conv.ConvertX(3);
					headerBounds.X     += dx;
					headerBounds.Width -= dx;
				}
			}

			if(headerBounds.Width <= 0) break;
			// header text
			const TextFormatFlags flags =
				TextFormatFlags.Left |
				TextFormatFlags.VerticalCenter |
				TextFormatFlags.SingleLine |
				TextFormatFlags.PathEllipsis |
				TextFormatFlags.NoPadding |
				TextFormatFlags.PreserveGraphicsClipping;
			TextRenderer.DrawText(graphics, headerText, font, headerBounds, textColor, flags);

			dx = headerWidth + spacing;
			if(i == 0) dx += spacing;
			headerBounds.X     += dx;
			headerBounds.Width -= dx;
		}
	}

	private void PaintNoItemsMessage(Graphics graphics, Font font, Rectangle rcHeader)
	{
		GitterApplication.TextRenderer.DrawText(
			graphics, Resources.StrNoChangedFiles, font, Style.Colors.GrayText, rcHeader, ContentFormat);
	}

	private void PaintItems(Graphics graphics, Font font, Rectangle rcLine, Rectangle clip, DpiConverter conv)
	{
		if(_items is not { Length: not 0 }) return;

		rcLine.Y += HeaderBottomMargin.GetValue(conv.To) + HeaderHeight.GetValue(conv.To);
		rcLine.Height = LineHeight.GetValue(conv.To);
		var textColor = Style.Colors.WindowText;

		for(int i = 0; i < _items.Length; ++i)
		{
			if(!Filter(_items[i])) continue;

			if(Rectangle.Intersect(rcLine, clip) is { Width: > 0, Height: > 0 } rcLineClip)
			{
				if(i == _fileHover.Index)
				{
					Style.ItemBackgroundStyles.Hovered.Draw(graphics, new(conv.To, rcLine, rcLineClip));
				}
				else if((i & 1) == 1)
				{
					graphics.GdiFill(Style.Colors.Alternate, rcLineClip);
				}
				_items[i].Draw(graphics, conv.To, font, conv, textColor, rcLine);
			}
			rcLine.Y += rcLine.Height;
		}
	}

	private Rectangle GetHeaderBounds(Rectangle rect, DpiConverter conv)
	{
		if(FlowControl is null) return Rectangle.Empty;

		var marginX = conv.ConvertX(5);
		return new(rect.X + marginX, rect.Y, FlowControl.ContentArea.Width - marginX * 2, HeaderHeight.GetValue(conv.To));
	}

	/// <inheritdoc/>
	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(_diff is null) return;
		if(FlowControl is null) return;

		var graphics = paintEventArgs.Graphics;
		var bounds   = paintEventArgs.Bounds;
		var clip     = paintEventArgs.ClipRectangle;
		var conv     = DpiConverter.FromDefaultTo(paintEventArgs.Dpi);

		using(graphics.SwitchSmoothingMode(SmoothingMode.None))
		{
			var font     = Font.GetValue(paintEventArgs.Dpi);
			var rcHeader = GetHeaderBounds(bounds, conv);
			if(_items is { Length: not 0 })
			{
				if(rcHeader.IntersectsWith(clip))
				{
					PaintHeader(graphics, font, bounds.X, bounds.Y, rcHeader, conv, clip);
				}
				PaintItems(graphics, font, rcHeader, clip, conv);
			}
			else
			{
				if(rcHeader.IntersectsWith(clip))
				{
					PaintNoItemsMessage(graphics, font, rcHeader);
				}
			}
		}
	}
}
