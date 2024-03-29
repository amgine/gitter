﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System;
using System.Runtime.CompilerServices;

static class Macro
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint RGB(byte byRed, byte byGreen, byte byBlue)
		=> ((uint)byRed) | ((uint)byGreen << 8) | ((uint)byBlue << 16);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int HIWORD(int n)
		=> (n >> 16) & 0xffff;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int HIWORD(IntPtr n)
		=> HIWORD(unchecked((int)(long)n));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LOWORD(int n)
		=> n & 0xffff;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LOWORD(IntPtr n)
		=> LOWORD(unchecked((int)(long)n));
}
