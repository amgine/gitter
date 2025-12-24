#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

public static class SolidBrushCache
{
	[ThreadStatic]
	private static Stack<SolidBrush>? _brushes;

	public ref struct CachedSolidBrush
	{
		private SolidBrush _brush;

		internal CachedSolidBrush(Color color)
		{
			if(_brushes is { Count: > 0 })
			{
				_brush = _brushes.Pop();
				_brush.Color = color;
			}
			else
			{
				_brush = new(color);
			}
		}

		public readonly Color Color
		{
			get => _brush.Color;
			set => _brush.Color = value;
		}

		public static implicit operator Brush(CachedSolidBrush cached) => cached._brush;

		public static implicit operator SolidBrush(CachedSolidBrush cached) => cached._brush;

		public void Dispose()
		{
			if(_brush is not null)
			{
				(_brushes ??= new()).Push(_brush);
				_brush = null!;
			}
		}
	}

	public static CachedSolidBrush Get(Color color)
		=> new(color);
}
