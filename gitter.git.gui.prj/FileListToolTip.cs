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

namespace gitter.Git.Gui;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[System.ComponentModel.DesignerCategory("")]
sealed class FileListToolTip : CustomToolTip
{
	private struct TextEntry
	{
		public string Text;
		public bool IsFile;
	}

	private List<TextEntry> _textEntries;
	private Size _cachedSize;
	private Dpi  _cachedDpi;
	private int _rowHeight;

	private const int MaximumFiles = 10;

	private static List<TextEntry> CaptureData(Status status, bool staged, FileStatus fileStatus)
	{
		int count = 0;
		var list = new List<TextEntry>();
		lock(status.SyncRoot)
		{
			switch(fileStatus)
			{
				case FileStatus.Added when staged:
					count = status.StagedAddedCount;
					list.Add(new TextEntry { Text = Resources.StrAddedFiles.AddColon() });
					break;
				case FileStatus.Added when !staged:
					count = status.UnstagedUntrackedCount;
					list.Add(new TextEntry { Text = Resources.StrUntrackedFiles.AddColon() });
					break;
				case FileStatus.Removed:
					count = staged
						? status.StagedRemovedCount
						: status.UnstagedRemovedCount;
					list.Add(new TextEntry { Text = Resources.StrRemovedFiles.AddColon() });
					break;
				case FileStatus.Modified:
					count = staged
						? status.StagedModifiedCount
						: status.UnstagedModifiedCount;
					list.Add(new TextEntry { Text = Resources.StrModifiedFiles.AddColon() });
					break;
				case FileStatus.Unmerged:
					if(staged) throw new ArgumentException(nameof(fileStatus));
					list.Add(new TextEntry { Text = Resources.StrConflictingFiles.AddColon() });
					count = status.UnmergedCount;
					break;
				default:
					throw new ArgumentException($"Unknown {nameof(FileStatus)}: {fileStatus}", nameof(fileStatus));
			}
			var files = staged ? status.StagedFiles : status.UnstagedFiles;
			int i = 0;
			foreach(var file in files)
			{
				if(file.Status == fileStatus)
				{
					list.Add(new TextEntry
					{
						Text   = file.RelativePath,
						IsFile = true,
					});
					++i;
					if(i >= MaximumFiles)
					{
						int remaining = count - i;
						if(remaining != 0)
						{
							list.Add(new TextEntry
							{
								Text = "{0} more files are not shown".UseAsFormat(remaining).SurroundWith('(', ')'),
							});
						}
						break;
					}
				}
			}
		}
		return list;
	}

	public override Size Measure(Control associatedControl)
	{
		var dpi = associatedControl is not null
			? Dpi.FromControl(associatedControl)
			: Dpi.Default;

		if(_cachedDpi == dpi)
		{
			return _cachedSize;
		}

		if(_textEntries is not { Count: > 0 })
		{
			_cachedDpi  = dpi;
			_cachedSize = default;
			return default;
		}

		var conv = DpiConverter.FromDefaultTo(dpi);

		_rowHeight = 0;
		var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
		int maxW = 0;
		foreach(var entry in _textEntries)
		{
			var mainSize = GitterApplication.TextRenderer.MeasureText(
				GraphicsUtility.MeasurementGraphics, entry.Text, font, int.MaxValue, StringFormat.GenericTypographic);
			int entryW1 = entry.IsFile ? conv.ConvertX(16) + conv.ConvertX(3) : 0;
			int entryW2 = mainSize.Width;
			int entryH  = mainSize.Height;
			if(entryH > _rowHeight) _rowHeight = entryH;
			int w = entryW1 + entryW2;
			if(w > maxW) maxW = w;
		}
		_cachedDpi  = dpi;
		_cachedSize = new Size(
			conv.ConvertX(HorizontalMargin) * 2 + maxW,
			conv.ConvertY(VerticalMargin) * 2 + _textEntries.Count * (_rowHeight + conv.ConvertY(VerticalSpacing)));

		return _cachedSize;
	}

	public FileListToolTip()
	{
		UseFading = false;
	}

	public void Update(Status status, bool staged, FileStatus fileStatus)
	{
		_textEntries = CaptureData(status, staged, fileStatus);
		_cachedDpi   = default;
		_cachedSize  = default;
	}

	protected override void OnPaint(DrawToolTipEventArgs e)
	{
		Assert.IsNotNull(e);

		if(_textEntries is not { Count: > 0 }) return;

		var gx   = e.Graphics;
		var dpi  = e.AssociatedControl is not null ? Dpi.FromControl(e.AssociatedControl) : new Dpi(gx);
		var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var y    = conv.ConvertY(VerticalMargin);
		var hmargin = conv.ConvertX(HorizontalMargin);
		foreach(var entry in _textEntries)
		{
			var x = hmargin;
			if(entry.IsFile)
			{
				var image = GraphicsUtility.QueryIcon(entry.Text, dpi);
				if(image is not null)
				{
					gx.DrawImage(image, x, y + (image.Height - _rowHeight) / 2);
					x += image.Width + conv.ConvertX(3);
				}
			}
			GitterApplication.TextRenderer.DrawText(
				gx, entry.Text, font, SystemBrushes.InfoText, x, y, StringFormat.GenericTypographic);
			y += _rowHeight + conv.ConvertY(VerticalSpacing);
		}
	}
}
