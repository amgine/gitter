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

[Flags]
public enum GraphElement : short
{
	/// <summary>
	/// <para>...</para>
	/// <para>...</para>
	/// <para>...</para>
	/// </summary>
	Space = (0 << 0),

	/// <summary>
	/// <para>...</para>
	/// <para>.+.</para>
	/// <para>...</para>
	/// </summary>
	Dot = (1 << GraphElementId.Dot),

	/// <summary>
	/// <para>...</para>
	/// <para>-..</para>
	/// <para>...</para>
	/// </summary>
	HorizontalLeft = (1 << GraphElementId.HorizontalLeft),
	/// <summary>
	/// <para>...</para>
	/// <para>..-</para>
	/// <para>...</para>
	/// </summary>
	HorizontalRight = (1 << GraphElementId.HorizontalRight),
	/// <summary>
	/// <para>.|.</para>
	/// <para>...</para>
	/// <para>...</para>
	/// </summary>
	VerticalTop = (1 << GraphElementId.VerticalTop),
	/// <summary>
	/// <para>...</para>
	/// <para>...</para>
	/// <para>.|.</para>
	/// </summary>
	VerticalBottom = (1 << GraphElementId.VerticalBottom),
	/// <summary>
	/// <para>\..</para>
	/// <para>...</para>
	/// <para>...</para>
	/// </summary>
	LeftTop = (1 << GraphElementId.LeftTop),
	/// <summary>
	/// <para>...</para>
	/// <para>...</para>
	/// <para>..\</para>
	/// </summary>
	RightBottom = (1 << GraphElementId.RightBottom),
	/// <summary>
	/// <para>../</para>
	/// <para>...</para>
	/// <para>...</para>
	/// </summary>
	RightTop = (1 << GraphElementId.RightTop),
	/// <summary>
	/// <para>...</para>
	/// <para>...</para>
	/// <para>/..</para>
	/// </summary>
	LeftBottom = (1 << GraphElementId.LeftBottom),
	/// <summary>
	/// <para>/..</para>
	/// <para>...</para>
	/// <para>...</para>
	/// </summary>
	LeftTopCorner = (1 << GraphElementId.LeftTopCorner),
	/// <summary>
	/// <para>..\</para>
	/// <para>...</para>
	/// <para>...</para>
	/// </summary>
	RightTopCorner = (1 << GraphElementId.RightTopCorner),
	/// <summary>
	/// <para>...</para>
	/// <para>...</para>
	/// <para>\..</para>
	/// </summary>
	LeftBottomCorner = (1 << GraphElementId.LeftBottomCorner),
	/// <summary>
	/// <para>...</para>
	/// <para>...</para>
	/// <para>../</para>
	/// </summary>
	RightBottomCorner = (1 << GraphElementId.RightBottomCorner),

	/// <summary>
	/// <code>
	/// ...
	/// ---
	/// ...
	/// </code>
	/// </summary>
	Horizontal = HorizontalLeft | HorizontalRight,
	/// <summary>
	/// <code>
	/// .|.
	/// .|.
	/// .|.
	/// </code>
	/// </summary>
	Vertical = VerticalTop | VerticalBottom,
	/// <summary>
	/// <code>
	/// \..
	/// .\.
	/// ..\
	/// </code>
	/// </summary>
	DiagonalLTRB = LeftTop | RightBottom,
	/// <summary>
	/// <code>
	/// ../
	/// ./.
	/// /..
	/// </code>
	/// </summary>
	DiagonalRTLB = RightTop | LeftBottom,
}
