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

/// <summary>Popup animation.</summary>
[Flags]
public enum PopupAnimations : int
{
	None = 0,

	LeftToRight = 0x00001,
	RightToLeft = 0x00002,
	TopToBottom = 0x00004,
	BottomToTop = 0x00008,

	Center = 0x00010,

	Slide = 0x40000,

	Blend = 0x80000,

	Roll = 0x100000,

	SystemDefault = 0x200000,
}
