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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class UntrackedFilesPanel : FlowPanel
{
	private static readonly StringFormat ContentFormat = new(StringFormat.GenericTypographic)
	{
		Alignment     = StringAlignment.Near,
		FormatFlags   = StringFormatFlags.FitBlackBox,
		LineAlignment = StringAlignment.Near,
		Trimming      = StringTrimming.EllipsisPath,
	};

	private static readonly IDpiBoundValue<int>  LineHeight = DpiBoundValue.ScaleY(16 + 4);
	private static readonly IDpiBoundValue<Font> Font       = GitterApplication.FontManager.UIFont.ScalableFont;
	private static int FontHeight = -1;
	private FileItem[] _items;
	private readonly TrackingService<FileItem> _fileHover;

	private sealed class FileItem
	{
		private readonly string _text;

		public FileItem(TreeFile file)
		{
			Verify.Argument.IsNotNull(file);

			File  = file;
			_text = file.RelativePath;
		}

		public TreeFile File { get; }

		public void Draw(Graphics graphics, Font font, DpiConverter conv, Brush textBrush, Rectangle rect, int index)
		{
			var iconSize = conv.Convert(new Size(16, 16));
			int dy = (rect.Height - iconSize.Height) / 2;
			var iconRect = new Rectangle(rect.X + dy, rect.Y + dy, iconSize.Width, iconSize.Height);
			var icon = GraphicsUtility.QueryIcon(_text, conv.To);
			if(icon is not null)
			{
				graphics.DrawImage(icon, iconRect);
			}
			var overlay = CachedResources.ScaledBitmaps[@"overlays.add", conv.ConvertX(16)];
			if(overlay is not null)
			{
				graphics.DrawImage(overlay, iconRect);
			}
			var dx = dy + iconSize.Width + conv.ConvertX(3);
			rect.X     += dx;
			rect.Width -= dx;
			dy = (LineHeight.GetValue(conv.To) - FontHeight) / 2;
			rect.Y      += dy;
			rect.Height -= dy;
			GitterApplication.TextRenderer.DrawText(
				graphics, _text, font, textBrush, rect, ContentFormat);
		}
	}

	public UntrackedFilesPanel(Status status)
		: this(status, null)
	{
	}

	public int Count => _items.Length;

	public UntrackedFilesPanel(Status status, IEnumerable<string> paths)
	{
		Verify.Argument.IsNotNull(status);

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
		var h = LineHeight.GetValue(Dpi.FromControl(FlowControl));
		Invalidate(new Rectangle(0, (e.Index + 1) * h, FlowControl.ContentArea.Width, h));
		//if(e.IsTracked)
		//    FlowControl.Cursor = Cursors.Hand;
		//else
		//    FlowControl.Cursor = Cursors.Default;
	}

	private int HitTest(int x, int y)
	{
		var dpi     = Dpi.FromControl(FlowControl);
		var conv    = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.ConvertX(5);
		if(x < padding || x >= FlowControl.ContentArea.Width - padding) return -1;
		var h = LineHeight.GetValue(dpi);
		y -= h;
		if(y < 0) return -1;
		int id = y / h;
		return id < _items.Length ? id : -1;
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
			if(FlowControl is DiffViewer p)
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
		Assert.IsNotNull(measureEventArgs);

		if(_items is not { Length: > 0 }) return Size.Empty;
		var font = FlowControl.Font;
		if(FontHeight == -1) FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(measureEventArgs.Graphics, font) + 0.5);
		return new Size(0, (_items.Length + 1) * LineHeight.GetValue(measureEventArgs.Dpi));
	}

	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(_items is not { Length: > 0 }) return;
		var graphics = paintEventArgs.Graphics;
		var conv = DpiConverter.FromDefaultTo(paintEventArgs.Dpi);
		var rect = paintEventArgs.Bounds;
		var clip = paintEventArgs.ClipRectangle;
		var font = Font.GetValue(paintEventArgs.Dpi);
		if(FontHeight == -1)
		{
			FontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(graphics, font) + 0.5);
		}
		var h      = LineHeight.GetValue(paintEventArgs.Dpi);
		var rc     = new Rectangle(rect.X + conv.ConvertX(5), rect.Y, FlowControl.ContentArea.Width - conv.ConvertX(5) * 2, h);
		var rcClip = Rectangle.Intersect(rc, clip);
		using var textBrush = new SolidBrush(FlowControl.Style.Colors.WindowText);
		if(rcClip is { Width: > 0, Height: > 0 })
		{
			GitterApplication.TextRenderer.DrawText(
				graphics, Resources.StrUntrackedFiles.AddColon(), font, textBrush, rc, ContentFormat);
		}
		for(int i = 0; i < _items.Length; ++i)
		{
			rc.Y += h;
			rcClip = Rectangle.Intersect(rc, clip);
			if(rcClip is { Width: > 0, Height: > 0 })
			{
				if(i % 2 == 1)
				{
					graphics.GdiFill(FlowControl.Style.Colors.Alternate, rcClip);
				}
				if(i == _fileHover.Index)
				{
					FlowControl.Style.ItemBackgroundStyles.Hovered.Draw(graphics, conv.To, rc);
				}
				_items[i].Draw(graphics, font, conv, textBrush, rc, i);
			}
		}
	}
}
