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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

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

		private static readonly int LineHeight   = SystemInformation.SmallIconSize.Height + 4;
		private static readonly int HeaderHeight = SystemInformation.SmallIconSize.Height + 4;
		private const int HeaderBottomMargin = 3;
		private const int HeaderContentPadding = 3;
		private const int HeaderSpacing = 7;

		private Diff _diff;
		private FileItem[] _items;
		private ChangesCountByType[] _changesByType;

		private static string GetOverlayName(FileStatus fileStatus)
			=> fileStatus switch
			{
				FileStatus.Removed     => @"overlays.delete",
				FileStatus.Added       => @"overlays.add",
				FileStatus.Modified    => @"overlays.edit",
				FileStatus.Unmerged    => @"overlays.conflict",
				FileStatus.Copied      => @"overlays.copy",
				FileStatus.Renamed     => @"overlays.rename",
				FileStatus.ModeChanged => @"overlays.chmod",
				_ => default,
			};

		private sealed class FileItem
		{
			private readonly DiffFile _file;
			private readonly string _fileName;
			private readonly string _text;

			public FileItem(DiffFile file)
			{
				Verify.Argument.IsNotNull(file, nameof(file));

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

			private void PaintDiffStats(Graphics graphics, Font font, DpiConverter conv, Brush textBrush, Rectangle rect)
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
						textBrush,
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
							n1 = added   * squares / changed;
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
								textBrush,
								rect,
								DiffStatFormat);
						}
					}
				}
				graphics.SmoothingMode = oldMode;
			}

			public void Draw(Graphics graphics, Font font, DpiConverter conv, Brush textBrush, Rectangle rect)
			{
				var iconSize = conv.Convert(new Size(16, 16));
				var statSize = conv.ConvertX((9 + 1) * 5 + 20);

				int dy = (rect.Height - iconSize.Height) / 2;
				var iconRect = new Rectangle(rect.X + dy, rect.Y + dy, iconSize.Width, iconSize.Height);
				var icon = GraphicsUtility.QueryIcon(_fileName, conv.To);
				if(icon is not null)
				{
					graphics.DrawImage(icon, iconRect);
					var overlayName = GetOverlayName(File.Status);
					if(overlayName is not null)
					{
						var overlay = CachedResources.ScaledBitmaps[overlayName, conv.ConvertX(16)];
						if(overlay is not null)
						{
							graphics.DrawImage(overlay, iconRect);
						}
					}
				}
				var dx = dy + iconSize.Width + conv.ConvertX(3);
				rect.X     += dx;
				rect.Width -= dx;
				dy = (LineHeight - FontHeight) / 2;
				rect.Y      += dy;
				rect.Height -= dy;
				var statRect = new Rectangle(rect.Right - statSize - conv.ConvertX(3), rect.Y, statSize, rect.Height);
				if(rect.Width > statSize * 2)
				{
					PaintDiffStats(graphics, font, conv, textBrush, statRect);
					rect.Width -= statSize + conv.ConvertX(3);
				}
				GitterApplication.TextRenderer.DrawText(
					graphics, _text, font, textBrush, rect, ContentFormat);
			}
		}

		private sealed class ChangesCountByType
		{
			private int _count;

			public ChangesCountByType(FileStatus status)
			{
				Status = status;
				ImageOverlayName = GetOverlayName(status);
				UpdateDisplayText();
			}

			private void UpdateDisplayText()
			{
				var suffix = Status switch
				{
					StatusFilterAll when Count > 1 => Resources.StrlChanges,
					StatusFilterAll        => Resources.StrlChange,
					FileStatus.Added       => Resources.StrlAdded,
					FileStatus.Modified    => Resources.StrlModified,
					FileStatus.Removed     => Resources.StrlRemoved,
					FileStatus.Unmerged    => Resources.StrlUnmerged,
					FileStatus.Copied      => Resources.StrlCopied,
					FileStatus.Renamed     => Resources.StrlRenamed,
					FileStatus.ModeChanged => Resources.StrlChmod,
					_ => default,
				};
				DisplayText = Count.ToString(CultureInfo.InvariantCulture) + " " + suffix;
			}

			public string ImageOverlayName { get; }

			public FileStatus Status { get; }

			public int Count
			{
				get => _count;
				set
				{
					if(_count != value)
					{
						_count = value;
						UpdateDisplayText();
					}
				}
			}

			public string DisplayText { get; private set; }

			public Rectangle DisplayBounds { get; set; }
		}

		private readonly TrackingService<FileItem> _fileHover;
		private readonly TrackingService<ChangesCountByType> _filterHover;

		private static readonly Font Font = GitterApplication.FontManager.UIFont.Font;
		private static int FontHeight = -1;

		private const FileStatus StatusFilterAll =
			FileStatus.Added | FileStatus.Removed | FileStatus.Modified |
			FileStatus.Renamed | FileStatus.Copied | FileStatus.ModeChanged | FileStatus.Unmerged;

		private FileStatus _statusFilter;

		#region Events

		public event EventHandler StatusFilterChanged;

		private void OnStatusFilterChanged()
			=> StatusFilterChanged?.Invoke(this, EventArgs.Empty);

		public event EventHandler<DiffFileEventArgs> FileNavigationRequested;

		private void OnFileNavigationRequested(DiffFile diffFile)
			=> FileNavigationRequested?.Invoke(this, new DiffFileEventArgs(diffFile));

		#endregion

		/// <summary>Create <see cref="ChangedFilesPanel"/>.</summary>
		public ChangedFilesPanel()
		{
			_fileHover    = new TrackingService<FileItem>(OnFileHoverChanged);
			_filterHover  = new TrackingService<ChangesCountByType>(OnStatusFilterHoverChanged);
			_statusFilter = StatusFilterAll;

			_changesByType = new[]
			{
				new ChangesCountByType(StatusFilterAll),
				new ChangesCountByType(FileStatus.Added),
				new ChangesCountByType(FileStatus.Removed),
				new ChangesCountByType(FileStatus.Modified),
				new ChangesCountByType(FileStatus.Renamed),
				new ChangesCountByType(FileStatus.Copied),
				new ChangesCountByType(FileStatus.Unmerged),
				new ChangesCountByType(FileStatus.ModeChanged),
			};
		}

		private void OnFileHoverChanged(object sender, TrackingEventArgs<FileItem> e)
		{
			Invalidate(new Rectangle(0, HeaderHeight + HeaderBottomMargin + GetVisualIndex(e.Index) * LineHeight, FlowControl.ContentArea.Width, LineHeight));
			FlowControl.Cursor = e.IsTracked ? Cursors.Hand : Cursors.Default;
		}


		private void OnStatusFilterHoverChanged(object sender, TrackingEventArgs<ChangesCountByType> e)
		{
			Invalidate(new Rectangle(0, 0, FlowControl.ContentArea.Width, HeaderHeight));
			FlowControl.Cursor = e.IsTracked ? Cursors.Hand : Cursors.Default;
		}

		/// <summary>Displayed diff.</summary>
		public Diff Diff
		{
			get => _diff;
			set
			{
				if(_diff != value)
				{
					_diff = value;
					for(int i = 1; i < _changesByType.Length; ++i)
					{
						_changesByType[i].Count = 0;
					}
					if(_diff != null)
					{
						_changesByType[0].Count = _diff.FilesCount;
						if(_items == null || _items.Length != _diff.FilesCount)
						{
							_items = new FileItem[_diff.FilesCount];
						}
						for(int i = 0; i < _diff.FilesCount; ++i)
						{
							_items[i] = new FileItem(_diff[i]);
							for(int j = 1; j < _changesByType.Length; ++j)
							{
								if(_changesByType[j].Status == _diff[i].Status)
								{
									++_changesByType[j].Count;
									break;
								}
							}
						}
					}
					else
					{
						_changesByType[0].Count = 0;
						_items = Preallocated<FileItem>.EmptyArray;
					}
					_fileHover.Reset(-1, null);
				}
			}
		}

		public FileStatus StatusFilter
		{
			get => _statusFilter;
			private set
			{
				if(_statusFilter != value)
				{
					_statusFilter = value;
					OnStatusFilterChanged();
					InvalidateSize();
				}
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
			if(x < 5 || x >= FlowControl.ContentArea.Width - 5) return -1;
			y -= HeaderHeight + HeaderBottomMargin;
			if(y < 0)
			{
				return -1;
			}
			else
			{
				int id = y / LineHeight;
				if(id < 0 || id >= _items.Length)
				{
					return -1;
				}
				if(StatusFilter == StatusFilterAll)
				{
					return id;
				}
				else
				{
					int visualIndex = -1;
					for(int i = 0; i < _items.Length; ++i)
					{
						if((_items[i].File.Status & StatusFilter) != FileStatus.Unknown)
						{
							++visualIndex;
							if(visualIndex == id)
							{
								return i;
							}
						}
					}
					return -1;
				}
			}
		}

		private int HitTestFilter(int x, int y)
		{
			if(y < 0 || y > HeaderHeight) return -1;
			for(int i = 0; i < _changesByType.Length; ++i)
			{
				if(_changesByType[i].DisplayBounds.Contains(x, y))
				{
					return i;
				}
			}
			return -1;
		}

		protected override void OnMouseDown(int x, int y, MouseButtons button)
		{
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
								StatusFilter = _changesByType[id].Status;
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

		protected override void OnMouseMove(int x, int y)
		{
			if(FontHeight == -1) return;
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
					_filterHover.Track(id, _changesByType[id]);
				}
			}
			else
			{
				_filterHover.Drop();
				_fileHover.Track(id, _items[id]);
			}
			base.OnMouseMove(x, y);
		}

		/// <inheritdoc/>
		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			if(_diff is null) return Size.Empty;
			if(FontHeight == -1)
			{
				FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(measureEventArgs.Graphics, Font) + 0.5f);
			}
			int lineCount;
			if(StatusFilter == StatusFilterAll)
			{
				lineCount = _diff.FilesCount;
			}
			else
			{
				lineCount = 0;
				for(int i = 0; i < _diff.FilesCount; ++i)
				{
					if((_diff[i].Status & StatusFilter) != FileStatus.Unknown)
					{
						++lineCount;
					}
				}
			}
			return new Size(0, HeaderHeight + (lineCount == 0 ? 0 : HeaderBottomMargin) + lineCount * LineHeight);
		}

		/// <inheritdoc/>
		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			if(_diff is null) return;
			var graphics = paintEventArgs.Graphics;
			var rect     = paintEventArgs.Bounds;
			var clip     = paintEventArgs.ClipRectangle;
			var conv     = new DpiConverter(FlowControl);

			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			if(FontHeight == -1)
			{
				FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(graphics, Font) + 0.5);
			}
			var rcHeader = new Rectangle(rect.X + 5, rect.Y, FlowControl.ContentArea.Width - 10, HeaderHeight);
			var rcClip = Rectangle.Intersect(rcHeader, clip);
			if(_diff.FilesCount == 0)
			{
				if(rcClip is not { Width: > 0, Height: > 0 })
				{
					return;
				}
				GitterApplication.TextRenderer.DrawText(
					graphics, Resources.StrNoChangedFiles, Font, Style.Colors.GrayText, rcHeader, ContentFormat);
			}
			else
			{
				using var textBrush = new SolidBrush(Style.Colors.WindowText);

				if(rcClip is { Width: > 0, Height: > 0 })
				{
					var headerBounds = rcHeader;

					var dx = conv.ConvertX(5);
					headerBounds.X     += dx;
					headerBounds.Width -= dx;

					var iconSize = conv.Convert(new Size(16, 16));

					for(int i = 0; i < _changesByType.Length; ++i)
					{
						if(headerBounds.Width <= 0) break;

						if(_changesByType[i].Count != 0)
						{
							// prepare
							var headerText = _changesByType[i].DisplayText;
							var headerTextSize = GitterApplication.TextRenderer.MeasureText(
								graphics, headerText, Font, short.MaxValue, ContentFormat);
							var headerWidth = headerTextSize.Width;
							var overlayName = _changesByType[i].ImageOverlayName;
							var displayBounds = new Rectangle(
								headerBounds.X - HeaderContentPadding,
								headerBounds.Y,
								headerWidth + (overlayName is not null ? iconSize.Width + conv.ConvertX(3) : 0) + HeaderContentPadding * 2,
								headerBounds.Height);
							_changesByType[i].DisplayBounds = new Rectangle(
								displayBounds.X - rect.X, displayBounds.Y - rect.Y, displayBounds.Width, displayBounds.Height);
							// background
							if(StatusFilter == _changesByType[i].Status)
							{
								Style.ItemBackgroundStyles.Selected.Draw(graphics, displayBounds);
							}
							else if(_filterHover.Index == i)
							{
								Style.ItemBackgroundStyles.Hovered.Draw(graphics, displayBounds);
							}
							// header icon
							if(overlayName is not null)
							{
								var image = CachedResources.ScaledBitmaps[@"file", iconSize.Width];
								if(image is not null)
								{
									var imageBounds = new Rectangle(
										headerBounds.X,
										headerBounds.Y + (headerBounds.Height - iconSize.Height) / 2,
										iconSize.Width, iconSize.Height);
									graphics.DrawImage(image, imageBounds);
									var overlay = CachedResources.ScaledBitmaps[overlayName, iconSize.Width];
									if(overlay is not null)
									{
										graphics.DrawImage(overlay, imageBounds);
									}
									dx = iconSize.Width + conv.ConvertX(3);
									headerBounds.X     += dx;
									headerBounds.Width -= dx;
								}
							}

							if(headerBounds.Width <= 0) break;
							// header text
							GitterApplication.TextRenderer.DrawText(
								graphics, headerText, Font, textBrush, headerBounds, HeaderFormat);

							dx = headerWidth + conv.ConvertX(HeaderSpacing);
							headerBounds.X     += dx;
							headerBounds.Width -= dx;
							if(i == 0)
							{
								dx = conv.ConvertX(HeaderSpacing);
								headerBounds.X     += dx;
								headerBounds.Width -= dx;
							}
						}
						else
						{
							_changesByType[i].DisplayBounds = Rectangle.Empty;
						}
					}
				}
				var rcLine = rcHeader;
				rcLine.Y += HeaderBottomMargin + HeaderHeight;
				rcLine.Height = LineHeight;
				bool alternate = false;
				for(int i = 0; i < _items.Length; ++i)
				{
					if((_items[i].File.Status & StatusFilter) != FileStatus.Unknown)
					{
						rcClip = Rectangle.Intersect(rcLine, clip);
						if(rcClip.Height > 0 && rcClip.Width > 0)
						{
							if(alternate)
							{
								graphics.GdiFill(Style.Colors.Alternate, rcClip);
							}
							if(i == _fileHover.Index)
							{
								Style.ItemBackgroundStyles.Hovered.Draw(graphics, rcLine);
							}
							_items[i].Draw(graphics, Font, conv, textBrush, rcLine);
						}
						alternate = !alternate;
						rcLine.Y += LineHeight;
					}
				}
			}
		}
	}
}
