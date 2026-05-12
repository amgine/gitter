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
using System.IO;

/// <summary>
/// A loaded icon pack: manifest + resolver + bitmap cache + SVG content loader.
/// </summary>
public sealed class IconPack : IDisposable
{
	private readonly Func<string, Stream?> _svgLoader;
	private readonly IconBitmapCache _cache;

	public IconPack(
		string                 name,
		IconPackManifest       manifest,
		Func<string, Stream?>  svgLoader,
		IconBitmapCache?       cache = null)
	{
		Name      = name      ?? throw new ArgumentNullException(nameof(name));
		Manifest  = manifest  ?? throw new ArgumentNullException(nameof(manifest));
		_svgLoader = svgLoader ?? throw new ArgumentNullException(nameof(svgLoader));
		Resolver   = new IconPackResolver(manifest);
		_cache     = cache ?? new IconBitmapCache();
	}

	public string           Name     { get; }
	public IconPackManifest Manifest { get; }
	public IconPackResolver Resolver { get; }
	public IconBitmapCache  Cache    => _cache;

	public Bitmap? GetFileBitmap(string fileName, int pixelSize, string? languageId = null)
	{
		var def = Resolver.ResolveFile(fileName, languageId);
		return GetBitmap(def, pixelSize);
	}

	public Bitmap? GetFolderBitmap(string folderName, int pixelSize, bool expanded = false)
	{
		var def = Resolver.ResolveFolder(folderName, expanded);
		return GetBitmap(def, pixelSize);
	}

	public Bitmap? GetBitmap(string? iconDefName, int pixelSize)
	{
		if(string.IsNullOrEmpty(iconDefName)) return null;
		var iconPath = Resolver.ResolveIconPath(iconDefName);
		if(string.IsNullOrEmpty(iconPath)) return null;
		try
		{
			return _cache.GetOrAdd(iconDefName!, pixelSize, sz => Rasterize(iconPath!, sz));
		}
		catch
		{
			return null;
		}
	}

	private Bitmap Rasterize(string iconPath, int pixelSize)
	{
		using var stream = _svgLoader(iconPath)
			?? throw new FileNotFoundException($"SVG '{iconPath}' not found in pack '{Name}'.");
		return SvgRasterizer.Rasterize(stream, pixelSize);
	}

	public void Dispose() => _cache.Dispose();
}
