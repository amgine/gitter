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

namespace gitter.Native
{
	using System;

	[Flags]
	internal enum AnimationFlags : int
	{
		Roll = 0x0000, // Uses a roll animation.
		HorizontalPositive = 0x00001, // Animates the window from left to right. This flag can be used with roll or slide animation.
		HorizontalNegative = 0x00002, // Animates the window from right to left. This flag can be used with roll or slide animation.
		VerticalPositive = 0x00004, // Animates the window from top to bottom. This flag can be used with roll or slide animation.
		VerticalNegative = 0x00008, // Animates the window from bottom to top. This flag can be used with roll or slide animation.
		Center = 0x00010, // Makes the window appear to collapse inward if <c>Hide</c> is used or expand outward if the <c>Hide</c> is not used.
		Hide = 0x10000, // Hides the window. By default, the window is shown.
		Activate = 0x20000, // Activates the window.
		Slide = 0x40000, // Uses a slide animation. By default, roll animation is used.
		Blend = 0x80000, // Uses a fade effect. This flag can be used only with a top-level window.
		Mask = 0xfffff,
	}
}
