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
	private readonly FileItem[] _items;
	private readonly TrackingService<FileItem> _fileHover;

	private sealed class FileItem
	{
		private readonly string _text;

		public FileItem(TreeFile file)
		{
			Verify.Argument.IsNotNull(file);

			File = file;
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
			var overlay = Icons.Overlays.Add.GetImage(conv.ConvertX(16));
			if(overlay is not null)
			{
				graphics.DrawImage(overlay, iconRect);
			}
			var dx = dy + iconSize.Width + conv.ConvertX(3);
			rect.X     += dx;
			rect.Width -= dx;
			dy = (LineHeight.GetValue(conv.To) - GetFontHeight(conv.To)) / 2;
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

	public UntrackedFilesPanel(Status status, IEnumerable<string>? paths)
	{
		Verify.Argument.IsNotNull(status);

		lock(status.SyncRoot)
		{
			var items = new List<FileItem>(status.UnstagedUntrackedCount);
			foreach(var file in status.UnstagedFiles)
			{
				if(file.Status != FileStatus.Added) continue;
				if(!Filter(file, paths)) continue;
				items.Add(new FileItem(file));
			}
			_items = [.. items];
		}
		_fileHover = new TrackingService<FileItem>();
		_fileHover.Changed += OnFileHoverChanged;
	}

	private static bool Filter(TreeFile file, IEnumerable<string>? paths)
	{
		if(paths is null) return true;
		foreach(var path in paths)
		{
			if(file.RelativePath.StartsWith(path))
			{
				return true;
			}
		}
		return false;
	}

	private void OnFileHoverChanged(object? sender, TrackingEventArgs<FileItem> e)
	{
		if(FlowControl is null) return;

		var h = LineHeight.GetValue(Dpi.FromControl(FlowControl));
		Invalidate(new Rectangle(0, (e.Index + 1) * h, FlowControl.ContentArea.Width, h));
		//if(e.IsTracked)
		//    FlowControl.Cursor = Cursors.Hand;
		//else
		//    FlowControl.Cursor = Cursors.Default;
	}

	private int HitTest(int x, int y)
	{
		if(FlowControl is null) return -1;

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

	private static int _fontHeight;
	private static Dpi _fontHeightDpi;

	private static int GetFontHeight(Dpi dpi)
	{
		if(_fontHeightDpi != dpi)
		{
			_fontHeight = (int)(GitterApplication.TextRenderer.GetFontHeight(Font.GetValue(dpi)) + 0.5f);
			_fontHeightDpi = dpi;
		}
		return _fontHeight;
	}

	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(FlowControl is null) return Size.Empty;

		if(_items is not { Length: > 0 }) return Size.Empty;
		return new Size(0, (_items.Length + 1) * LineHeight.GetValue(measureEventArgs.Dpi));
	}

	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(FlowControl is null) return;

		if(_items is not { Length: > 0 }) return;
		var graphics = paintEventArgs.Graphics;
		var conv   = DpiConverter.FromDefaultTo(paintEventArgs.Dpi);
		var rect   = paintEventArgs.Bounds;
		var clip   = paintEventArgs.ClipRectangle;
		var font   = Font.GetValue(paintEventArgs.Dpi);
		var h      = LineHeight.GetValue(paintEventArgs.Dpi);
		var rc     = new Rectangle(rect.X + conv.ConvertX(5), rect.Y, FlowControl.ContentArea.Width - conv.ConvertX(5) * 2, h);
		var rcClip = Rectangle.Intersect(rc, clip);
		using var textBrush = SolidBrushCache.Get(FlowControl.Style.Colors.WindowText);
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
					FlowControl.Style.ItemBackgroundStyles.Hovered.Draw(graphics, new(conv.To, rc, clip));
				}
				_items[i].Draw(graphics, font, conv, textBrush, rc, i);
			}
		}
	}
}
