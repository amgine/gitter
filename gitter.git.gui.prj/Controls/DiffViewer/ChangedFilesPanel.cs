namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
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

		private const int LineSpacing = 4;
		private const int LineHeight = 20;

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
				_icon = Utility.QueryIcon(_text);
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

			private void PaintDiffStats(Graphics graphics, Font font, Rectangle rect)
			{
				const int squares = 10;
				const int squareWidth = 4;
				const int squareHeight = 13;
				const int squareSpacing = 1;
				const float radius = 1.5f;
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
						SystemBrushes.Window,
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
							SystemBrushes.Window,
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
							SystemBrushes.Window,
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
								SystemBrushes.WindowText,
								rect,
								DiffStatFormat);
						}
					}
				}
			}

			public void Draw(Graphics graphics, Font font, Rectangle rect, int index, bool hover)
			{
				const int IconSize = 16;
				const int StatSize = (9 + 1) * 5 + 20;

				if(hover)
				{
					BackgroundStyle.Hovered.Draw(graphics, rect);
				}
				else if(index % 2 == 1)
				{
					graphics.FillRectangle(Brushes.WhiteSmoke, rect);
				}

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
					PaintDiffStats(graphics, font, statRect);
					rect.Width -= StatSize + 3;
				}
				GitterApplication.TextRenderer.DrawText(
					graphics, _text, font, SystemBrushes.WindowText, rect, ContentFormat);
			}
		}

		private sealed class ChangesCountByType
		{
			private readonly FileStatus _status;
			private readonly Bitmap _image;
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
				set { _count = value; }
			}
		}

		private readonly TrackingService<FileItem> _fileHover;

		private static readonly Font Font = GitterApplication.FontManager.UIFont.Font;
		private static int FontHeight = -1;

		/// <summary>Create <see cref="ChangedFilesPanel"/>.</summary>
		public ChangedFilesPanel()
		{
			_fileHover = new TrackingService<FileItem>();
			_fileHover.Changed += OnFileHoverChanged;

			_changesByType = new ChangesCountByType[7];
			_changesByType[0] = new ChangesCountByType(FileStatus.Added);
			_changesByType[1] = new ChangesCountByType(FileStatus.Removed);
			_changesByType[2] = new ChangesCountByType(FileStatus.Modified);
			_changesByType[3] = new ChangesCountByType(FileStatus.Renamed);
			_changesByType[4] = new ChangesCountByType(FileStatus.Copied);
			_changesByType[5] = new ChangesCountByType(FileStatus.Unmerged);
			_changesByType[6] = new ChangesCountByType(FileStatus.ModeChanged);
		}

		private void OnFileHoverChanged(object sender, TrackingEventArgs<FileItem> e)
		{
			Invalidate(new Rectangle(0, (e.Index + 1) * (LineHeight), FlowControl.ContentArea.Width, LineHeight));
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
					for(int i = 0; i < _changesByType.Length; ++i)
					{
						_changesByType[i].Count = 0;
					}
					if(_diff != null)
					{
						if(_items == null || _items.Length != _diff.FilesCount)
						{
							_items = new FileItem[_diff.FilesCount];
						}
						for(int i = 0; i < _diff.FilesCount; ++i)
						{
							_items[i] = new FileItem(_diff[i]);
							for(int j = 0; j < _changesByType.Length; ++j)
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
						_items = new FileItem[0];
					}
					_fileHover.Reset(-1, null);
				}
			}
		}

		protected override void OnMouseLeave()
		{
			_fileHover.Drop();
		}

		private int HitTest(int x, int y)
		{
			if(x < 5 || x >= FlowControl.ContentArea.Width - 5) return -1;
			y -= LineHeight;
			if(y < 0)
			{
				return -1;
			}
			else
			{
				int id = y / LineHeight;
				if(id >= _items.Length)
				{
					return -1;
				}
				else
				{
					return id;
				}
			}
		}

		protected override void OnMouseDown(int x, int y, MouseButtons button)
		{
			switch(button)
			{
				case MouseButtons.Left:
					{
						int id = HitTest(x, y);
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
					}
					break;
				case MouseButtons.Right:
					{
						int id = HitTest(x, y);
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
			int id = HitTest(x, y);
			if(id == -1)
			{
				_fileHover.Drop();
			}
			else
			{
				_fileHover.Track(id, _items[id]);
			}
			base.OnMouseMove(x, y);
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			if(_diff == null) return Size.Empty;
			if(FontHeight == -1) FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(measureEventArgs.Graphics, Font) + 0.5f);
			return new Size(0, (_diff.FilesCount + 1) * (LineHeight));
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			if(_diff == null) return;
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var clip = paintEventArgs.ClipRectangle;
			int y = rect.Y;
			if(FontHeight == -1) FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(graphics, Font) + 0.5);
			var rc = new Rectangle(rect.X + 5, rect.Y, FlowControl.ContentArea.Width - 10, LineHeight);
			var rcClip = Rectangle.Intersect(rc, clip);
			if(_diff.FilesCount == 0)
			{
				if(rcClip.Height > 0 && rcClip.Width > 0)
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, Resources.StrNoChangedFiles, Font, SystemBrushes.GrayText, rc, ContentFormat);
				}
			}
			else
			{
				if(rcClip.Height > 0 && rcClip.Width > 0)
				{
					var headerBounds = rc;

					string headerText = Resources.StrChangedFiles.AddColon();
					var headerWidth = GitterApplication.TextRenderer.MeasureText(
						graphics, headerText, Font, short.MaxValue, ContentFormat).Width;
					GitterApplication.TextRenderer.DrawText(
						graphics, headerText, Font, SystemBrushes.WindowText, headerBounds, ContentFormat);

					headerBounds.X += headerWidth + 5;
					headerBounds.Width -= headerWidth + 5;

					for(int i = 0; i < _changesByType.Length; ++i)
					{
						if(headerBounds.Width <= 0) break;

						if(_changesByType[i].Count != 0)
						{
							headerText = _changesByType[i].Count.ToString();
							var image = _changesByType[i].Image;

							graphics.DrawImage(image, headerBounds.X, headerBounds.Y);
							headerBounds.X += 16 + 3;
							headerBounds.Width -= 16 + 3;

							if(headerBounds.Width <= 0) break;

							headerWidth = GitterApplication.TextRenderer.MeasureText(
								graphics, headerText, Font, short.MaxValue, ContentFormat).Width;
							GitterApplication.TextRenderer.DrawText(
								graphics, headerText, Font, SystemBrushes.WindowText, headerBounds, ContentFormat);
							headerBounds.X += headerWidth + 5;
							headerBounds.Width -= headerWidth + 5;
						}
					}
				}
				for(int i = 0; i < _items.Length; ++i)
				{
					rc.Y += LineHeight;
					rcClip = Rectangle.Intersect(rc, clip);
					if(rcClip.Height > 0 && rcClip.Width > 0)
					{
						_items[i].Draw(graphics, Font, rc, i, i == _fileHover.Index);
					}
				}
			}
		}
	}
}
