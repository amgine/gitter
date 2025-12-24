#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

#if NETCOREAPP

namespace gitter.Framework.Intrinsics;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

public static unsafe class Avx2HashHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Vector256<sbyte> ReadHexDigits256(byte* src)
	{
		var vec = Sse2.LoadVector128(src).ToVector256();
		var l = Avx2.UnpackLow (default, vec);
		var h = Avx2.UnpackHigh(default, vec);
		var p = Avx2.Permute2x128(l, h, 0b0010_0000).AsInt16();
		return Avx2.Or(
			Avx2.ShiftRightLogical(
				Avx2.And(p, Vector256.Create(unchecked((short)0xf000))),
				12),
			Avx2.And(p, Vector256.Create((short)0x0f00))).AsSByte();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Vector128<sbyte> ReadHexDigits128(byte* src)
	{
		var vec = Sse2.LoadScalarVector128((int*)src).AsByte();
		var unpacked = Sse2.UnpackLow(default, vec).AsInt16();
		return Sse2.Or(
			Sse2.ShiftRightLogical(
				Sse2.And(unpacked, Vector128.Create(unchecked((short)0xf000))),
				12),
			Sse2.And(unpacked, Vector128.Create((short)0x0f00))).AsSByte();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Vector256<sbyte> ToAsciiHexDigits256(Vector256<sbyte> values)
	{
		var m = Avx2.CompareGreaterThan(values, Vector256.Create((sbyte)9));
		m = Avx2.Or(
			Avx2.And   (m, Vector256.Create((sbyte)('a' - 10))),
			Avx2.AndNot(m, Vector256.Create((sbyte)'0')));
		return Avx2.Add(values, m);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Vector128<sbyte> ToAsciiHexDigits128(Vector128<sbyte> values)
	{
		var m = Sse2.CompareGreaterThan(values, Vector128.Create((sbyte)9));
		m = Sse2.Or(
			Sse2.And   (m, Vector128.Create((sbyte)('a' - 10))),
			Sse2.AndNot(m, Vector128.Create((sbyte)'0')));
		return Sse2.Add(values, m);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static void StoreAsciiVector256AsUtf16(char* hashStringUtf16, Vector256<sbyte> values)
	{
		var l = Avx2.UnpackLow (default, values.AsByte());
		var h = Avx2.UnpackHigh(default, values.AsByte());

		l = Avx2.ShiftRightLogical128BitLane(l, 1);
		h = Avx2.ShiftRightLogical128BitLane(h, 1);

		Avx.Store((byte*)(hashStringUtf16 + 16 * 0), Avx2.Permute2x128(l, h, 0b0010_0000));
		Avx.Store((byte*)(hashStringUtf16 + 16 * 1), Avx2.Permute2x128(l, h, 0b0011_0001));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static void StoreAsciiVector128AsUtf16(char* hashStringUtf16, Vector128<sbyte> values)
	{
		var unpacked = Sse2.UnpackLow(default, values.AsByte());
		unpacked = Sse2.ShiftRightLogical128BitLane(unpacked, 1);
		Sse2.Store((byte*)hashStringUtf16, unpacked);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToHexStringFrom16Bytes(byte* bytes, char* hashStringUtf16)
	{
		StoreAsciiVector256AsUtf16(hashStringUtf16,
			ToAsciiHexDigits256(ReadHexDigits256(bytes)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToHexStringFrom4Bytes(byte* bytes, char* hashStringUtf16)
	{
		StoreAsciiVector128AsUtf16(hashStringUtf16,
			ToAsciiHexDigits128(ReadHexDigits128(bytes)));
	}
}

#endif
