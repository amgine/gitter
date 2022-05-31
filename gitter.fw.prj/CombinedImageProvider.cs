#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework;

using System;
using System.Collections.Generic;
using System.Drawing;

public class CombinedImageProvider : IImageProvider
{
	private readonly Dictionary<int, Image> _cache = new();

	public CombinedImageProvider(IImageProvider image, IImageProvider overlay)
	{
		Verify.Argument.IsNotNull(image);
		Verify.Argument.IsNotNull(overlay);

		Image   = image;
		Overlay = overlay;
	}

	private IImageProvider Image { get; }

	private IImageProvider Overlay { get; }

	private static Image Combine(Image image, Image overlay, int size)
	{
		if(overlay is null) return image;
		var bitmap = new Bitmap(size, size);
		using var graphics = Graphics.FromImage(bitmap);
		graphics.Clear(Color.Transparent);
		var bounds = new Rectangle(0, 0, size, size);
		graphics.DrawImage(image,   bounds);
		graphics.DrawImage(overlay, bounds);
		return bitmap;
	}

	public Image GetImage(int size)
	{
		Verify.Argument.IsPositive(size);

		if(!_cache.TryGetValue(size, out var image))
		{
			_cache.Add(size, image = Combine(Image.GetImage(size), Overlay.GetImage(size), size));
		}
		return image;
	}
}
