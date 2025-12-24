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

namespace gitter.Framework;

using System;
using System.Collections.Generic;
using System.Drawing;

sealed class GdiFontCache
{
	public static GdiFontCache Shared { get; } = new();

	readonly record struct Key(string Name, float Size, FontStyle Style)
	{
		public static Key FromFont(Font font)
			=> new(font.Name, font.SizeInPoints, font.Style);
	}

	private readonly Dictionary<Key, IntPtr> _fonts = [];

	public IntPtr GetFont(Font font)
	{
		var key = Key.FromFont(font);
		if(!_fonts.TryGetValue(key, out var handle))
		{
			_fonts.Add(key, handle = font.ToHfont());
		}
		return handle;
	}

	public IntPtr GetFont(Font font, FontStyle style)
	{
		var key = Key.FromFont(font) with { Style = style };
		if(!_fonts.TryGetValue(key, out var handle))
		{
			if(font.Style != style)
			{
				using var temp = new Font(font, style);
				_fonts.Add(key, handle = temp.ToHfont());
			}
			else
			{
				_fonts.Add(key, handle = font.ToHfont());
			}
		}
		return handle;
	}
}
