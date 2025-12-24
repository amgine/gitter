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
using System.Drawing;

/// <summary>Object for unique color allocation.</summary>
public sealed class GraphColorProvider : IGraphColorProvider
{
	private readonly Color[] _palette;
	private readonly int _maxColors;
	private readonly bool[] _colors;
	private int _availableCount;
	private int _pointer;

	/// <summary>Create <see cref="GraphColorProvider"/>.</summary>
	/// <param name="palette">Palette.</param>
	public GraphColorProvider(Color[] palette)
	{
		Verify.Argument.IsNotNull(palette);

		_palette   = palette;
		_maxColors = palette.Length;
		_colors    = new bool[palette.Length];
		_pointer   = 0;
		_availableCount = palette.Length - 1;
	}

	static int DiffSqr(Color color1, Color color2)
	{
		var r = (int)color1.R - (int)color2.R;
		var g = (int)color1.G - (int)color2.G;
		var b = (int)color1.B - (int)color2.B;
		return r * r + g * g + b * b;
	}

	private int DiffSqrAvg(Color color)
	{
		var n   = 0;
		var sum = 0;
		for(int i = 1; i < _maxColors; ++i)
		{
			if(!_colors[i]) continue;

			sum += DiffSqr(_palette[i], color);
			++n;
		}
		return n == 0 ? 0 : sum / n;
	}

	private int DiffSqrMin(Color color)
	{
		var min = int.MaxValue;
		for(int i = 1; i < _maxColors; ++i)
		{
			if(!_colors[i]) continue;

			var diff = DiffSqr(_palette[i], color);
			if(diff < min) min = diff;
		}
		return min;
	}

	/// <summary>Acquire a unique color.</summary>
	/// <returns>Unique color.</returns>
	public short AcquireColor()
	{
		var first = (_pointer + 1) % _maxColors;
		var last  = _pointer;
		for(int i = first; i != last; i = (i + 1) % _maxColors)
		{
			if(i != 0 && !_colors[i])
			{
				--_availableCount;
				_pointer = i;
				_colors[i] = true;
				return (short)i;
			}
		}
		_pointer = 0;
		return 0;
		/*
		var bestPick = 0;
		var maxDiff  = int.MinValue;
		var first    = (_pointer + 1) % _maxColors;
		var last     = _pointer;
		for(int i = first; i != last; i = (i + 1) % _maxColors)
		{
			if(i != 0 && !_colors[i])
			{
				var diff = DiffSqrMin(_palette[i]);
				if(diff > maxDiff)
				{
					maxDiff  = diff;
					bestPick = i;
				}
			}
		}
		if(bestPick == 0)
		{
			_pointer = 0;
			return 0;
		}
		else
		{
			--_availableCount;
			_colors[bestPick] = true;
			_pointer = bestPick;
			return bestPick;
		}*/
	}

	/// <summary>Make <paramref name="color"/> available again.</summary>
	/// <param name="color">Color that is not needed anymore.</param>
	public void ReleaseColor(short color)
	{
		if(color == 0) return;

		if(_colors[color])
		{
			_colors[color] = false;
			++_availableCount;
		}
	}
}
