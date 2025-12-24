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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;

/// <summary>Background bounds.</summary>
/// <param name="dpi">Render DPI.</param>
/// <param name="bounds">Bounds/</param>
/// <param name="clip">Clip bounds.</param>
public readonly struct BackgroundBounds(Dpi dpi, Rectangle bounds, Rectangle clip)
{
	/// <summary>Defines background bounds.</summary>
	/// <param name="dpi">Render DPI.</param>
	/// <param name="bounds">Bounds/</param>
	public BackgroundBounds(Dpi dpi, Rectangle bounds) : this(dpi, bounds, bounds) { }

	public Rectangle Bounds { get; } = bounds;

	public Rectangle Clip { get; } = Rectangle.Intersect(clip, bounds);

	public Dpi Dpi { get; } = dpi;

	public int X => Bounds.X;

	public int Y => Bounds.Y;

	public int Width => Bounds.Width;

	public int Height => Bounds.Height;

	public int Right => Bounds.Right;

	public int Bottom => Bounds.Bottom;

	public static implicit operator Rectangle(BackgroundBounds bounds) => bounds.Bounds;
}

/// <summary>Item background style.</summary>
public interface IBackgroundStyle
{
	/// <summary>Draw item background.</summary>
	/// <param name="graphics"><see cref="Graphics"/> surface to draw on.</param>
	/// <param name="background">Item bounds.</param>
	void Draw(Graphics graphics, BackgroundBounds background);
}
