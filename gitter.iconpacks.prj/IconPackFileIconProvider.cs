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

namespace gitter.IconPacks;

using System;
using System.Drawing;

using gitter.Framework;

public sealed class IconPackFileIconProvider : IFileIconProvider
{
	private readonly IFileIconProvider _fallback;
	private IconPack? _pack;

	public IconPackFileIconProvider(IconPack? pack, IFileIconProvider fallback)
	{
		_pack     = pack;
		_fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
	}

	public IconPack? Pack
	{
		get => _pack;
		set => _pack = value;
	}

	public Bitmap? GetFileIcon(string fileName, Dpi dpi, string? languageId = null)
	{
		var pack = _pack;
		if(pack is not null)
		{
			try
			{
				var bmp = pack.GetFileBitmap(fileName, DpiConverter.FromDefaultTo(dpi).ConvertX(16), languageId);
				if(bmp is not null) return bmp;
			}
			catch(Exception ex)
			{
				IconPackLog.Warn($"GetFileIcon('{fileName}') failed", ex);
			}
		}
		return _fallback.GetFileIcon(fileName, dpi, languageId);
	}

	public Bitmap? GetFolderIcon(string folderName, Dpi dpi, bool expanded = false)
	{
		var pack = _pack;
		if(pack is not null)
		{
			try
			{
				var bmp = pack.GetFolderBitmap(folderName, DpiConverter.FromDefaultTo(dpi).ConvertX(16), expanded);
				if(bmp is not null) return bmp;
			}
			catch(Exception ex)
			{
				IconPackLog.Warn($"GetFolderIcon('{folderName}') failed", ex);
			}
		}
		return _fallback.GetFolderIcon(folderName, dpi, expanded);
	}
}
