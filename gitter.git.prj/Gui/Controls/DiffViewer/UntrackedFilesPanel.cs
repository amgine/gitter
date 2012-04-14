namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Properties.Resources;

	sealed class UntrackedFilesPanel : FlowPanel
	{
		private static readonly StringFormat ContentFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			FormatFlags =
				StringFormatFlags.FitBlackBox,
			LineAlignment = StringAlignment.Near,
			Trimming = StringTrimming.EllipsisPath,
		};

		private const int LineSpacing = 4;
		private const int LineHeight = 20;
		private static readonly Font Font = GitterApplication.FontManager.UIFont.Font;
		private static int FontHeight = -1;
		private FileItem[] _items;
		private readonly TrackingService<FileItem> _fileHover;

		private sealed class FileItem
		{
			private readonly TreeFile _file;
			private readonly Bitmap _icon;
			private readonly Bitmap _overlay;
			private readonly string _text;

			public FileItem(TreeFile file)
			{
				if(file == null) throw new ArgumentNullException("file");
				_file = file;
				_text = file.RelativePath;
				_icon = Utility.QueryIcon(file.RelativePath);
				_overlay = CachedResources.Bitmaps["ImgOverlayAdd"];
			}

			public void Draw(Graphics graphics, Font font, Rectangle rect, int index, bool hover)
			{
				const int IconSize = 16;

				if(hover)
					BackgroundStyle.Hovered.Draw(graphics, rect);
				else if(index % 2 == 1)
					graphics.FillRectangle(Brushes.WhiteSmoke, rect);

				int d = (rect.Height - 16) / 2;
				var iconRect = new Rectangle(rect.X + d, rect.Y + d, IconSize, IconSize);
				graphics.DrawImage(_icon, iconRect, 0, 0, IconSize, IconSize, GraphicsUnit.Pixel);
				if(_overlay != null)
					graphics.DrawImage(_overlay, iconRect, 0, 0, IconSize, IconSize, GraphicsUnit.Pixel);
				rect.X += d + IconSize + 3;
				rect.Width -= d + IconSize + 3;
				d = (LineHeight - FontHeight) / 2;
				rect.Y += d;
				rect.Height -= d;
				GitterApplication.TextRenderer.DrawText(
					graphics, _text, font, SystemBrushes.WindowText, rect, ContentFormat);
			}
		}

		public UntrackedFilesPanel(Status status)
			: this(status, null)
		{
		}

		public int Count
		{
			get { return _items.Length; }
		}

		public UntrackedFilesPanel(Status status, IEnumerable<string> paths)
		{
			if(status == null) throw new ArgumentNullException("status");

			lock(status.SyncRoot)
			{
				var items = new List<FileItem>(status.UnstagedUntrackedCount);
				foreach(var file in status.UnstagedFiles)
				{
					if(file.Status == FileStatus.Added)
					{
						bool found;
						if(paths != null)
						{
							found = false;
							foreach(var path in paths)
							{
								if(file.RelativePath.StartsWith(path))
								{
									found = true;
									break;
								}
							}
						}
						else
						{
							found = true;
						}
						if(found)
							items.Add(new FileItem(file));
					}
				}
				_items = items.ToArray();
			}
			_fileHover = new TrackingService<FileItem>();
			_fileHover.Changed += OnFileHoverChanged;
		}

		private void OnFileHoverChanged(object sender, TrackingEventArgs<FileItem> e)
		{
			Invalidate(new Rectangle(0, (e.Index + 1) * (LineHeight), FlowControl.ContentArea.Width, LineHeight));
			//if(e.IsTracked)
			//    FlowControl.Cursor = Cursors.Hand;
			//else
			//    FlowControl.Cursor = Cursors.Default;
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
					return -1;
				else
					return id;
			}
		}

		protected override void OnMouseLeave()
		{
			_fileHover.Drop();
		}

		protected override void OnMouseMove(int x, int y)
		{
			if(FontHeight == -1) return;
			int id = HitTest(x, y);
			if(id == -1)
				_fileHover.Drop();
			else
				_fileHover.Track(id, _items[id]);
			base.OnMouseMove(x, y);
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			if(_items == null || _items.Length == 0) return Size.Empty;
			var font = FlowControl.Font;
			if(FontHeight == -1) FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(measureEventArgs.Graphics, font) + 0.5);
			return new Size(0, (_items.Length + 1) * (LineHeight));
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			if(_items == null || _items.Length == 0) return;
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var clip = paintEventArgs.ClipRectangle;
			int y = rect.Y;
			if(FontHeight == -1)
				FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(graphics, Font) + 0.5);
			var rc = new Rectangle(rect.X + 5, rect.Y, FlowControl.ContentArea.Width - 10, LineHeight);
			if(!Rectangle.Intersect(rc, clip).IsEmpty)
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, Resources.StrUntrackedFiles.AddColon(), Font, SystemBrushes.WindowText, rc, ContentFormat);
			}
			for(int i = 0; i < _items.Length; ++i)
			{
				rc.Y += LineHeight;
				if(!Rectangle.Intersect(rc, clip).IsEmpty)
				{
					_items[i].Draw(graphics, Font, rc, i, i == _fileHover.Index);
				}
			}
		}
	}
}
