#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

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
				Verify.Argument.IsNotNull(file, "file");

				_file = file;
				_text = file.RelativePath;
				_icon = GraphicsUtility.QueryIcon(file.RelativePath);
				_overlay = CachedResources.Bitmaps["ImgOverlayAdd"];
			}

			public TreeFile File
			{
				get { return _file; }
			}

			public void Draw(Graphics graphics, Font font, Brush textBrush, Rectangle rect, int index)
			{
				const int IconSize = 16;

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
				GitterApplication.TextRenderer.DrawText(
					graphics, _text, font, textBrush, rect, ContentFormat);
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
			Verify.Argument.IsNotNull(status, "status");

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
						{
							items.Add(new FileItem(file));
						}
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
				{
					return -1;
				}
				else
				{
					return id;
				}
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
			{
				_fileHover.Drop();
			}
			else
			{
				_fileHover.Track(id, _items[id]);
			}
			base.OnMouseMove(x, y);
		}

		protected override void OnMouseDown(int x, int y, MouseButtons button)
		{
			base.OnMouseDown(x, y, button);
			if(button == MouseButtons.Right)
			{
				var p = FlowControl as DiffViewer;
				if(p != null)
				{
					var htr = HitTest(x, y);
					if(htr >= 0 && htr < _items.Length)
					{
						p.OnFileContextMenuRequested(_items[htr].File);
					}
				}
			}
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
			{
				FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(graphics, Font) + 0.5);
			}
			var rc = new Rectangle(rect.X + 5, rect.Y, FlowControl.ContentArea.Width - 10, LineHeight);
			var rcClip = Rectangle.Intersect(rc, clip);
			using(var textBrush = new SolidBrush(FlowControl.Style.Colors.WindowText))
			using(var alternateBackgroundBrush = new SolidBrush(FlowControl.Style.Colors.Alternate))
			{
				if(rcClip.Width > 0 && rcClip.Height > 0)
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, Resources.StrUntrackedFiles.AddColon(), Font, textBrush, rc, ContentFormat);
				}
				for(int i = 0; i < _items.Length; ++i)
				{
					rc.Y += LineHeight;
					rcClip = Rectangle.Intersect(rc, clip);
					if(rcClip.Width > 0 && rcClip.Height > 0)
					{
						if(i % 2 == 1)
						{
							graphics.FillRectangle(alternateBackgroundBrush, rcClip);
						}
						if(i == _fileHover.Index)
						{
							FlowControl.Style.ItemBackgroundStyles.Hovered.Draw(graphics, rc);
						}
						_items[i].Draw(graphics, Font, textBrush, rc, i);
					}
				}
			}
		}
	}
}
