#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using SkiaSharp;
using Svg.Skia;

public static class SvgRasterizer
{
	public static Bitmap Rasterize(string svgPath, int pixelSize)
	{
		if(svgPath is null)  throw new ArgumentNullException(nameof(svgPath));
		if(pixelSize <= 0)   throw new ArgumentOutOfRangeException(nameof(pixelSize));
		using var stream = File.OpenRead(svgPath);
		return Rasterize(stream, pixelSize);
	}

	public static Bitmap Rasterize(Stream svgStream, int pixelSize)
	{
		if(svgStream is null) throw new ArgumentNullException(nameof(svgStream));
		if(pixelSize <= 0)    throw new ArgumentOutOfRangeException(nameof(pixelSize));

		using var svg = new SKSvg();
		var picture = svg.Load(svgStream)
			?? throw new InvalidDataException("SVG load returned no picture.");
		return Render(picture, pixelSize);
	}

	private static Bitmap Render(SKPicture picture, int pixelSize)
	{
		var bounds = picture.CullRect;
		if(bounds.Width <= 0f || bounds.Height <= 0f)
			throw new InvalidDataException("SVG has zero or negative bounds.");

		var info = new SKImageInfo(pixelSize, pixelSize, SKColorType.Bgra8888, SKAlphaType.Premul);
		using var surface = SKSurface.Create(info);
		var canvas = surface.Canvas;
		canvas.Clear(SKColors.Transparent);
		var scale = Math.Min(pixelSize / bounds.Width, pixelSize / bounds.Height);
		var dx = (pixelSize - bounds.Width  * scale) / 2f - bounds.Left * scale;
		var dy = (pixelSize - bounds.Height * scale) / 2f - bounds.Top  * scale;
		canvas.Translate(dx, dy);
		canvas.Scale(scale, scale);
		canvas.DrawPicture(picture);
		canvas.Flush();

		using var image = surface.Snapshot();
		using var data  = image.Encode(SKEncodedImageFormat.Png, 100);
		using var ms    = new MemoryStream(data.ToArray(), writable: false);
		var loaded = new Bitmap(ms);
		var copy = new Bitmap(loaded);
		loaded.Dispose();
		return copy;
	}
}
