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

namespace gitter.Framework.Controls;

using System.Collections.Generic;
using System.Drawing;

sealed class ScalablePoints : IDpiBoundValue<Point[]>
{
	private readonly Dictionary<Dpi, Point[]> _cache = new();
	private readonly Point[] _original;

	public ScalablePoints(Point[] points)
	{
		_original = points;
		_cache    = new() { [Dpi.Default] = points };
	}

	public Point[] GetValue(Dpi dpi)
	{
		if(!_cache.TryGetValue(dpi, out var points))
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			points = new Point[_original.Length];
			for(int i = 0; i < points.Length; ++ i)
			{
				points[i] = conv.Convert(_original[i]);
			}
			_cache.Add(dpi, points);
		}
		return points;
	}
}
