#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git;

using System;
using System.Runtime.CompilerServices;

static class HashUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int TryParseCharToHexDigit(char ch)
		=> ch switch
		{
			>= '0' and <= '9' => ch - '0',
			>= 'a' and <= 'f' => ch - ('a' - 10),
			>= 'A' and <= 'F' => ch - ('A' - 10),
			_ => -1,
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ParseCharToHexDigit(char ch)
		=> ch switch
		{
			>= '0' and <= '9' => ch - '0',
			>= 'a' and <= 'f' => ch - ('a' - 10),
			>= 'A' and <= 'F' => ch - ('A' - 10),
			_ => throw new ArgumentException("Hexadecimal digit expected.", nameof(ch)),
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int TryParseByteToHexDigit(byte ch)
		=> ch switch
		{
			>= (byte)'0' and <= (byte)'9' => ch - '0',
			>= (byte)'a' and <= (byte)'f' => ch - ('a' - 10),
			>= (byte)'A' and <= (byte)'F' => ch - ('A' - 10),
			_ => -1,
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ParseByteToHexDigit(byte ch)
		=> ch switch
		{
			>= (byte)'0' and <= (byte)'9' => ch - '0',
			>= (byte)'a' and <= (byte)'f' => ch - ('a' - 10),
			>= (byte)'A' and <= (byte)'F' => ch - ('A' - 10),
			_ => throw new ArgumentException("Hexadecimal digit expected.", nameof(ch)),
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToHexDigit(int digit)
		=> digit >= 10 ? (char)('a' + (digit - 10)) : (char)('0' + digit);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToHexDigit(uint digit)
		=> digit >= 10 ? (char)('a' + (digit - 10)) : (char)('0' + digit);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToHexDigit(ulong digit)
		=> digit >= 10 ? (char)('a' + (digit - 10)) : (char)('0' + digit);
}
