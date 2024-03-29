﻿#region Copyright Notice
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

namespace gitter.Framework.Layout;

using System;
using System.Drawing;
using System.Windows.Forms;

public static class RectangleExtensions
{
	public static Rectangle WithPadding(this Rectangle rectangle, Padding padding) => new(
		x: rectangle.X + padding.Left,
		y: rectangle.Y + padding.Top,
		width:  rectangle.Width  - padding.Horizontal,
		height: rectangle.Height - padding.Vertical);

	public static Rectangle WithMargin(this Rectangle rectangle, Padding margin) => new(
		x: rectangle.X - margin.Left,
		y: rectangle.Y - margin.Top,
		width:  rectangle.Width  + margin.Horizontal,
		height: rectangle.Height + margin.Vertical);
}
