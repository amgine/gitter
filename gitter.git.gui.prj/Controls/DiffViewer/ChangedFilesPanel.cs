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
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary><see cref="FlowPanel"/> which displayes changed files.</summary>
	public class ChangedFilesPanel : FlowPanel
	{
		#region Static

		private static readonly StringFormat HeaderFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.FitBlackBox,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.None,
		};

		private static readonly StringFormat ContentFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.FitBlackBox,
			LineAlignment = StringAlignment.Near,
			Trimming = StringTrimming.EllipsisPath,
		};

		private static readonly StringFormat DiffStatFormat = new StringFormat(StringFormat.GenericDefault)
		{
			Alignment = StringAlignment.Far,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.NoClip |
				StringFormatFlags.NoWrap,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.None,
		};

		private static readonly StringFormat DiffStatCenterFormat = new StringFormat(StringFormat.GenericDefault)
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

		private sealed class FileItem
		{
			private readonly DiffFile _file;
			private readonly Bitmap _icon;
			private readonly Bitmap _overlay;
			private readonly string _text;

			public FileItem(DiffFile file)
			{
				Verify.Argument.IsNotNull(file, "file");

				_file = file;

				if(file.Status == FileStatus.Removed)
				{
					_text = file.SourceFile;
				}
				else
				{
					_text = file.TargetFile;
				}
				_icon = GraphicsUtility.QueryIcon(_text);
				switch(file.Status)
				{
					case FileStatus.Removed:
						_overlay = CachedResources.Bitmaps["ImgOverlayDel"];
						break;
					case FileStatus.Added:
						_overlay = CachedResources.Bitmaps["ImgOverlayAdd"];
						break;
					case FileStatus.Modified:
						_overlay = CachedResources.Bitmaps["ImgOverlayEdit"];
						break;
					case FileStatus.Unmerged:
						_overlay = CachedResources.Bitmaps["ImgOverlayConflict"];
						break;
					case FileStatus.Copied:
						_overlay = CachedResources.Bitmaps["ImgOverlayCopy"];
						_text = GetRenamedOrCopiedText(file.SourceFile, file.TargetFile);
						break;
					case FileStatus.Renamed:
						_overlay = CachedResources.Bitmaps["ImgOverlayRename"];
						_text = GetRenamedOrCopiedText(file.SourceFile, file.TargetFile);
						break;
					case FileStatus.ModeChanged:
						_overlay = CachedResources.Bitmaps["ImgOverlayChmod"];
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

			public DiffFile File
			{
				get { return _file; }
			}

			private void PaintDiffStats(Graphics graphics, Font font, Brush textBrush, Rectangle rect)
			{
				const int squares = 10;
				const int squareWidth = 4;
				const int squareHeight = 13;
				const int squareSpacing = 1;
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
						rect.Width -= (squareWidth + squareSpacing) * squares + 2;
						if(rect.Width > 2)
						{
							GitterApplication.TextRenderer.DrawText(
								graphics,
								changed.ToString(System.Globalization.CultureInfo.InvariantCulture),
								font,
								textBrush,
								rect,
								DiffStatFormat);
						}
					}
				}
				graphics.SmoothingMode = oldMode;
			}

			public void Draw(Graphics graphics, Font font, Brush textBrush, Rectangle rect)
			{
				const int IconSize = 16;
				const int StatSize = (9 + 1) * 5 + 20;

				int d = (rect.Height - 16) / 2;
				var iconRect = new Rectangle(rect.X + d, rect.Y + d, IconSize, IconSize);
				graphics.DrawImage(_icon, iconRect, 0, 0, IconSize, IconSize, GraphicsUnit.Pixel);
				if(_overlay != null)
				{
					graphics.DrawImage(_overlay, iconRect, 0, 0, IconSize, IconSize, GraphicsUnit.Pixel);
				}
				rect.X += d + IconSize + 3;
				rect.Width -= d + IconSize + 3;
				d = (LineHeight - FontHeight) / 2;
				rect.Y += d;
				rect.Height -= d;
				var statRect = new Rectangle(rect.Right - StatSize - 3, rect.Y, StatSize, rect.Height);
				if(rect.Width > StatSize * 2)
				{
					PaintDiffStats(graphics, font, textBrush, statRect);
					rect.Width -= StatSize + 3;
				}
				GitterApplication.TextRenderer.DrawText(
					graphics, _text, font, textBrush, rect, ContentFormat);
			}
		}

		private sealed class ChangesCountByType
		{
			private readonly FileStatus _status;
			private readonly Bitmap _image;
			private string _displayText;
			private Rectangle _displayBounds;
			private int _count;

			public ChangesCountByType(FileStatus status)
			{
				_status = status;
				switch(status)
				{
					case FileStatus.Added:
						_image = FileStatusIcons.ImgUnstagedUntracked;
						break;
					case FileStatus.Modified:
						_image = FileStatusIcons.ImgUnstagedModified;
						break;
					case FileStatus.Removed:
						_image = FileStatusIcons.ImgUnstagedRemoved;
						break;
					case FileStatus.Unmerged:
						_image = FileStatusIcons.ImgUnmerged;
						break;
					case FileStatus.Copied:
						_image = FileStatusIcons.ImgCopied;
						break;
					case FileStatus.Renamed:
						_image = FileStatusIcons.ImgRenamed;
						break;
					case FileStatus.ModeChanged:
						_image = FileStatusIcons.ImgModeChanged;
						break;
				}
				UpdateDisplayText();
			}

			private void UpdateDisplayText()
			{
				switch(Status)
				{
					case StatusFilterAll:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}",
							Count == 1 ? Resources.StrlChange : Resources.StrlChanges, Count);
						break;
					case FileStatus.Added:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}", Resources.StrlAdded, Count);
						break;
					case FileStatus.Modified:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}", Resources.StrlModified, Count);
						break;
					case FileStatus.Removed:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}", Resources.StrlRemoved, Count);
						break;
					case FileStatus.Unmerged:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}", Resources.StrlUnmerged, Count);
						break;
					case FileStatus.Copied:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}", Resources.StrlCopied, Count);
						break;
					case FileStatus.Renamed:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}", Resources.StrlRenamed, Count);
						break;
					case FileStatus.ModeChanged:
						_displayText = string.Format(CultureInfo.InvariantCulture, "{1} {0}", Resources.StrlChmod, Count);
						break;
				}
			}

			public Bitmap Image
			{
				get { return _image; }
			}

			public FileStatus Status
			{
				get { return _status; }
			}

			public int Count
			{
				get { return _count; }
				set
				{
					if(_count != value)
					{
						_count = value;
						UpdateDisplayText();
					}
				}
			}

			public string DisplayText
			{
				get { return _displayText; }
			}

			public Rectangle DisplayBounds
			{
				get { return _displayBounds; }
				set { _displayBounds = value; }
			}
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
		{
			var handler = StatusFilterChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		/// <summary>Create <see cref="ChangedFilesPanel"/>.</summary>
		public ChangedFilesPanel()
		{
			_fileHover		= new TrackingService<FileItem>(OnFileHoverChanged);
			_filterHover	= new TrackingService<ChangesCountByType>(OnStatusFilterHoverChanged);
			_statusFilter	= StatusFilterAll;

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
			get { return _diff; }
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
						_items = new FileItem[0];
					}
					_fileHover.Reset(-1, null);
				}
			}
		}

		public FileStatus StatusFilter
		{
			get { return _statusFilter; }
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
							foreach(var panel in FlowControl.Panels)
							{
								var diffpanel = panel as FileDiffPanel;
								if(diffpanel != null && diffpanel.DiffFile == file)
								{
									diffpanel.ScrollIntoView();
									break;
								}
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
							var viewer = (FlowControl as DiffViewer);
							if(viewer != null)
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

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			if(_diff == null) return Size.Empty;
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

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			if(_diff == null) return;
			var graphics	= paintEventArgs.Graphics;
			var rect		= paintEventArgs.Bounds;
			var clip		= paintEventArgs.ClipRectangle;

			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			if(FontHeight == -1)
			{
				FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(graphics, Font) + 0.5);
			}
			var rcHeader = new Rectangle(rect.X + 5, rect.Y, FlowControl.ContentArea.Width - 10, HeaderHeight);
			var rcClip = Rectangle.Intersect(rcHeader, clip);
			if(_diff.FilesCount == 0)
			{
				if(rcClip.Width <= 0 || rcClip.Height <= 0)
				{
					return;
				}
				using(var brush = new SolidBrush(Style.Colors.GrayText))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, Resources.StrNoChangedFiles, Font, brush, rcHeader, ContentFormat);
				}
			}
			else
			{
				using(var textBrush = new SolidBrush(Style.Colors.WindowText))
				{
					if(rcClip.Width > 0 || rcClip.Height > 0)
					{
						var headerBounds = rcHeader;

						headerBounds.X += 5;
						headerBounds.Width -= 5;

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
								var image = _changesByType[i].Image;
								var displayBounds = new Rectangle(
									headerBounds.X - HeaderContentPadding,
									headerBounds.Y,
									headerWidth + (image != null ? image.Width + 3 : 0) + HeaderContentPadding * 2,
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
								if(image != null)
								{
									graphics.DrawImage(image, headerBounds.X, headerBounds.Y + (headerBounds.Height - image.Height) / 2);
									headerBounds.X += image.Width + 3;
									headerBounds.Width -= image.Width + 3;
								}

								if(headerBounds.Width <= 0) break;
								// header text
								GitterApplication.TextRenderer.DrawText(
									graphics, headerText, Font, textBrush, headerBounds, HeaderFormat);

								headerBounds.X += headerWidth + HeaderSpacing;
								headerBounds.Width -= headerWidth + HeaderSpacing;
								if(i == 0)
								{
									headerBounds.X += HeaderSpacing;
									headerBounds.Width -= HeaderSpacing;
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
					using(var alternateBackgroundBrush = new SolidBrush(Style.Colors.Alternate))
					{
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
										graphics.FillRectangle(alternateBackgroundBrush, rcClip);
									}
									if(i == _fileHover.Index)
									{
										Style.ItemBackgroundStyles.Hovered.Draw(graphics, rcLine);
									}
									_items[i].Draw(graphics, Font, textBrush, rcLine);
								}
								alternate = !alternate;
								rcLine.Y += LineHeight;
							}
						}
					}
				}
			}
		}
	}
}
