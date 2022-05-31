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

namespace gitter.Native;

enum PenStyle : int
{
	PS_SOLID = 0, //The pen is solid.
	PS_DASH = 1, //The pen is dashed.
	PS_DOT = 2, //The pen is dotted.
	PS_DASHDOT = 3, //The pen has alternating dashes and dots.
	PS_DASHDOTDOT = 4, //The pen has alternating dashes and double dots.
	PS_NULL = 5, //The pen is invisible.
	PS_INSIDEFRAME = 6,// Normally when the edge is drawn, it’s centred on the outer edge meaning that half the width of the pen is drawn
					   // outside the shape’s edge, half is inside the shape’s edge. When PS_INSIDEFRAME is specified the edge is drawn
					   //completely inside the outer edge of the shape.
	PS_USERSTYLE = 7,
	PS_ALTERNATE = 8,
	PS_STYLE_MASK = 0x0000000F,

	PS_ENDCAP_ROUND = 0x00000000,
	PS_ENDCAP_SQUARE = 0x00000100,
	PS_ENDCAP_FLAT = 0x00000200,
	PS_ENDCAP_MASK = 0x00000F00,

	PS_JOIN_ROUND = 0x00000000,
	PS_JOIN_BEVEL = 0x00001000,
	PS_JOIN_MITER = 0x00002000,
	PS_JOIN_MASK = 0x0000F000,

	PS_COSMETIC = 0x00000000,
	PS_GEOMETRIC = 0x00010000,
	PS_TYPE_MASK = 0x000F0000
}
