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

namespace gitter.Git.Gui;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[System.ComponentModel.DesignerCategory("")]
sealed class StatusToolTip : CustomToolTip
{
	private struct TextEntry
	{
		public string Text1;
		public string Text2;
		public IImageProvider Image;
		public IImageProvider ImageOverlay;
	}

	private List<TextEntry> _textEntries;
	private Dpi _cachedDpi;
	private Size _cachedSize;
	private int _rowHeight;
	private int _colWidth;

	private static List<TextEntry> CaptureData(Status status)
	{
		Assert.IsNotNull(status);

		var textEntries = new List<TextEntry>();
		lock(status.SyncRoot)
		{
			if(status.UnmergedCount != 0)
			{
				textEntries.Add(new TextEntry
				{
					Text1        = Resources.StrConflictingFiles.AddColon(),
					Image        = CommonIcons.File,
					ImageOverlay = Icons.Overlays.Conflict,
					Text2        = status.UnmergedCount.ToString()
				});
			}
			if(status.UnstagedUntrackedCount != 0)
			{
				textEntries.Add(new TextEntry
				{
					Text1        = Resources.StrUntrackedFiles.AddColon(),
					Image        = CommonIcons.File,
					ImageOverlay = Icons.Overlays.Add,
					Text2        = status.UnstagedUntrackedCount.ToString()
				});
			}
			if(status.UnstagedModifiedCount != 0 || status.UnstagedRemovedCount != 0)
			{
				textEntries.Add(new TextEntry
				{
					Text1 = Resources.StrUnstagedChanges.AddColon(),
				});
				if(status.UnstagedRemovedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1        = Resources.StrRemovedFiles.AddColon(),
						Image        = CommonIcons.File,
						ImageOverlay = Icons.Overlays.Delete,
						Text2        = status.UnstagedRemovedCount.ToString()
					});
				}
				if(status.UnstagedModifiedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1        = Resources.StrModifiedFiles.AddColon(),
						Image        = CommonIcons.File,
						ImageOverlay = Icons.Overlays.Edit,
						Text2        = status.UnstagedModifiedCount.ToString()
					});
				}
			}
			if(status.StagedModifiedCount != 0 || status.StagedRemovedCount != 0 || status.StagedAddedCount != 0)
			{
				textEntries.Add(new TextEntry
				{
					Text1 = Resources.StrStagedChanges.AddColon(),
				});
				if(status.StagedAddedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1 = Resources.StrAddedFiles.AddColon(),
						Image = CommonIcons.File,
						ImageOverlay = Icons.Overlays.AddStaged,
						Text2 = status.StagedAddedCount.ToString()
					});
				}
				if(status.StagedRemovedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1 = Resources.StrRemovedFiles.AddColon(),
						Image = CommonIcons.File,
						ImageOverlay = Icons.Overlays.DeleteStaged,
						Text2 = status.StagedRemovedCount.ToString()
					});
				}
				if(status.StagedModifiedCount != 0)
				{
					textEntries.Add(new TextEntry
					{
						Text1 = Resources.StrModifiedFiles.AddColon(),
						Image = CommonIcons.File,
						ImageOverlay = Icons.Overlays.EditStaged,
						Text2 = status.StagedModifiedCount.ToString()
					});
				}
			}
		}
		if(textEntries.Count == 0)
		{
			textEntries.Add(new TextEntry { Text1 = Resources.StrsWorkingDirectoryClean });
		}
		return textEntries;
	}

	public override Size Measure(Control control)
	{
		var dpi = control is not null
			? Dpi.FromControl(control)
			: Dpi.Default;

		if(dpi == _cachedDpi) return _cachedSize;

		if(_textEntries is not { Count: > 0 })
		{
			_cachedDpi  = dpi;
			_cachedSize = default;
			return default;
		}

		var conv = DpiConverter.FromDefaultTo(dpi);

		_rowHeight = 0;
		_colWidth = 0;
		var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
		int maxW = 0;
		foreach(var entry in _textEntries)
		{
			var mainSize = GitterApplication.TextRenderer.MeasureText(
				GraphicsUtility.MeasurementGraphics, entry.Text1, font, int.MaxValue, StringFormat.GenericTypographic);
			int entryW1 = entry.Image is not null ? conv.ConvertX(16) + conv.ConvertX(3) : 0;
			int entryW2 = mainSize.Width;
			int entryW3 = entry.Text2 is not null ?
				GitterApplication.TextRenderer.MeasureText(GraphicsUtility.MeasurementGraphics, entry.Text2, font, int.MaxValue, StringFormat.GenericTypographic).Width + conv.ConvertX(15) :
				0;
			int entryH = mainSize.Height;
			if(entryH > _rowHeight) _rowHeight = entryH;
			int w = entryW1 + entryW2 + entryW3;
			if(entryW3 != 0)
			{
				var cw = entryW1 + entryW2 + conv.ConvertX(10);
				if(cw > _colWidth) _colWidth = cw;
			}
			if(w > maxW) maxW = w;
		}
		_cachedDpi  = dpi;
		_cachedSize = new Size(
			conv.ConvertX(HorizontalMargin) * 2 + maxW,
			conv.ConvertY(VerticalMargin)   * 2 + _textEntries.Count * (_rowHeight + conv.ConvertY(VerticalSpacing)) - 1);
		return _cachedSize;
	}

	public StatusToolTip()
	{
		UseFading = false;
	}

	public void Update(Status status)
	{
		_textEntries = CaptureData(status);
		_cachedDpi   = default;
		_cachedSize  = default;
	}

	protected override void OnPaint(DrawToolTipEventArgs e)
	{
		Assert.IsNotNull(e);

		if(_textEntries is not { Count: > 0 }) return;

		var dpi = e.AssociatedControl is not null
			? Dpi.FromControl(e.AssociatedControl)
			: Dpi.System;
		var conv = DpiConverter.FromDefaultTo(dpi);

		var gx       = e.Graphics;
		var font     = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
		var y        = conv.ConvertY(VerticalMargin);
		var iconSize = conv.ConvertX(16);
		foreach(var entry in _textEntries)
		{
			var x = HorizontalMargin;
			var image = entry.Image?.GetImage(iconSize);
			if(image is not null)
			{
				gx.DrawImage(image, x, y + (image.Height - _rowHeight) / 2);
				var overlay = entry.ImageOverlay?.GetImage(iconSize);
				if(overlay is not null)
				{
					gx.DrawImage(overlay, x, y + (image.Height - _rowHeight) / 2);
				}

				x += image.Width + conv.ConvertX(3);
			}
			GitterApplication.TextRenderer.DrawText(
				gx, entry.Text1, font, SystemBrushes.InfoText, x, y, StringFormat.GenericTypographic);
			if(entry.Text2 != null)
			{
				x = HorizontalMargin + _colWidth;
				GitterApplication.TextRenderer.DrawText(
					gx, entry.Text2, font, SystemBrushes.InfoText, x, y, StringFormat.GenericTypographic);
			}
			y += _rowHeight + conv.ConvertY(VerticalSpacing);
		}
	}
}
