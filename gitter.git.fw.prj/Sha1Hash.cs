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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NETCOREAPP
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif
using System.Text;

#if NETCOREAPP
using gitter.Framework.Intrinsics;
#endif

/// <summary>SHA-1 hash value.</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Sha1Hash : IHashValue<Sha1Hash>
{
	public const int Size = 20;
	public const int HexStringLength = Size * 2;

	sealed class EqualityComparerImpl : IEqualityComparer<Sha1Hash>
	{
		public bool Equals(Sha1Hash x, Sha1Hash y) => x == y;

		public int GetHashCode(Sha1Hash hash) => hash.GetHashCode();
	}

	sealed class HashComparerImpl : IComparer<Sha1Hash>
	{
		public int Compare(Sha1Hash x, Sha1Hash y) => x.CompareTo(y);
	}

	public static IEqualityComparer<Sha1Hash> EqualityComparer { get; } = new EqualityComparerImpl();
	public static IComparer        <Sha1Hash> Comparer         { get; } = new HashComparerImpl();

#if NET7_0_OR_GREATER
	static int IHashValue<Sha1Hash>.Size => Size;
	static int IHashValue<Sha1Hash>.HexStringLength => HexStringLength;
#endif

	unsafe struct DataContainer
	{
		public fixed byte Data[Sha1Hash.Size];
	}

	private readonly DataContainer _data;

	#region Static

	#if NETCOREAPP

	private static unsafe bool IsValidStringAVX2(char* hashStringUtf16)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool ValidateMasks256(Vector256<short> m0, Vector256<short> m1, Vector256<short> m2)
			=> Avx2.MoveMask(Avx2.Xor(Avx2.Xor(m0, m1), m2).AsByte()) == unchecked((int)0xFFFFFFFF);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool ValidateMasks128(Vector128<short> m0, Vector128<short> m1, Vector128<short> m2)
			=> Sse2.MoveMask(Sse2.Xor(Sse2.Xor(m0, m1), m2).AsByte()) == 0x0000FFFF;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Vector256<short> GetRangeMask256(Vector256<short> fromExclusive, Vector256<short> value, Vector256<short> toExclusive)
			=> Avx2.And(
				Avx2.CompareGreaterThan(value, fromExclusive),
				Avx2.CompareGreaterThan(toExclusive, value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Vector128<short> GetRangeMask128(Vector128<short> fromExclusive, Vector128<short> value, Vector128<short> toExclusive)
			=> Sse2.And(
				Sse2.CompareGreaterThan(value, fromExclusive),
				Sse2.CompareGreaterThan(toExclusive, value));

		var zeros = Vector256.Create((short)('0' - 1));
		var nines = Vector256.Create((short)('9' + 1));
		var a     = Vector256.Create((short)('a' - 1));
		var f     = Vector256.Create((short)('f' + 1));
		var A     = Vector256.Create((short)('A' - 1));
		var F     = Vector256.Create((short)('F' + 1));

		var vec0      = Avx.LoadVector256((short*)(hashStringUtf16 + 16 * 0));
		var decMask0  = GetRangeMask256(zeros, vec0, nines);
		var hexMaskL0 = GetRangeMask256(a, vec0, f);
		var hexMaskU0 = GetRangeMask256(A, vec0, F);
		if(!ValidateMasks256(decMask0, hexMaskL0, hexMaskU0)) return false;

		var vec1      = Avx.LoadVector256((short*)(hashStringUtf16 + 16 * 1));
		var decMask1  = GetRangeMask256(zeros, vec1, nines);
		var hexMaskL1 = GetRangeMask256(a, vec1, f);
		var hexMaskU1 = GetRangeMask256(A, vec1, F);
		if(!ValidateMasks256(decMask1, hexMaskL1, hexMaskU1)) return false;

		var vec2      = Sse2.LoadVector128((short*)(hashStringUtf16 + 32 * 1));
		var decMask2  = GetRangeMask128(zeros.GetLower(), vec2, nines.GetLower());
		var hexMaskL2 = GetRangeMask128(a.GetLower(), vec2, f.GetLower());
		var hexMaskU2 = GetRangeMask128(A.GetLower(), vec2, F.GetLower());
		if(!ValidateMasks128(decMask2, hexMaskL2, hexMaskU2)) return false;

		return true;
	}

	#endif

	public static unsafe bool IsValidString(char* hashStringUtf16)
	{
		#if NETCOREAPP
		if(Avx2.IsSupported) return IsValidStringAVX2(hashStringUtf16);
		#endif
		for(int i = 0; i < HexStringLength; ++i)
		{
#if NET9_0_OR_GREATER
			if(!char.IsAsciiHexDigit(hashStringUtf16[i])) return false;
#else
			if(!Uri.IsHexDigit(hashStringUtf16[i])) return false;
#endif
		}
		return true;
	}

	public static unsafe bool IsValidString(
		[NotNullWhen(returnValue: true)] string? hashStringUtf16)
	{
		if(hashStringUtf16 is null) return false;
		if(hashStringUtf16.Length != HexStringLength) return false;
		fixed(char* p = hashStringUtf16)
		{
			return IsValidString(p);
		}
	}

	public static unsafe Sha1Hash Parse(byte* hashStringUtf8)
		=> TryParse(hashStringUtf8, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf8));

	public static unsafe Sha1Hash Parse(char* hashStringUtf16)
		=> TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse(byte* hashStringUtf8, out Sha1Hash hash)
	{
		var parsed = default(Sha1Hash);
		var output = (byte*)&parsed;
		#if NETCOREAPP
		if(Avx2.IsSupported)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool ValidateMasks256(Vector256<sbyte> m0, Vector256<sbyte> m1, Vector256<sbyte> m2)
				=> Avx2.MoveMask(Avx2.Xor(Avx2.Xor(m0, m1), m2).AsByte()) == unchecked((int)0xFFFFFFFF);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool ValidateMasks128(Vector128<sbyte> m0, Vector128<sbyte> m1, Vector128<sbyte> m2)
				=> Sse2.MoveMask(Sse2.Xor(Sse2.Xor(m0, m1), m2).AsByte()) == unchecked((int)0x000000FF);

			var zeros      = Vector256.Create((sbyte)('0' - 1));
			var nines      = Vector256.Create((sbyte)('9' + 1));
			var a          = Vector256.Create((sbyte)('a' - 1));
			var f          = Vector256.Create((sbyte)('f' + 1));
			var A          = Vector256.Create((sbyte)('A' - 1));
			var F          = Vector256.Create((sbyte)('F' + 1));
			var decOffset  = Vector256.Create((sbyte)('0'));
			var hexOffsetL = Vector256.Create((sbyte)('a' - 10));
			var hexOffsetU = Vector256.Create((sbyte)('A' - 10));

			var vec0 = Avx.LoadVector256((sbyte*)hashStringUtf8);

			var decMask0 = Avx2.And(
				Avx2.CompareGreaterThan(vec0, zeros),
				Avx2.CompareGreaterThan(nines, vec0));
			var hexMaskL0 = Avx2.And(
				Avx2.CompareGreaterThan(vec0, a),
				Avx2.CompareGreaterThan(f, vec0));
			var hexMaskU0 = Avx2.And(
				Avx2.CompareGreaterThan(vec0, A),
				Avx2.CompareGreaterThan(F, vec0));

			if(!ValidateMasks256(decMask0, hexMaskL0, hexMaskU0)) return false;

			var digits0 = Avx2.Or(Avx2.Or(
				Avx2.And(Avx2.Subtract(vec0, decOffset),  decMask0),
				Avx2.And(Avx2.Subtract(vec0, hexOffsetL), hexMaskL0)),
				Avx2.And(Avx2.Subtract(vec0, hexOffsetU), hexMaskU0)).AsInt16();
			var part0 = Avx2.Permute4x64(
				Avx2.PackUnsignedSaturate(
					Avx2.Add(
						Avx2.ShiftLeftLogical (Avx2.And(digits0, Vector256.Create(unchecked((short)0x000f))), 4),
						Avx2.ShiftRightLogical(Avx2.And(digits0, Vector256.Create(unchecked((short)0x0f00))), 8)),
					default).AsUInt64(),
				control: 0b11_01_10_00).AsByte();

			Sse2.Store(output, part0.GetLower());

			var vec1 = Sse2.LoadScalarVector128((double*)(hashStringUtf8 + 32)).AsSByte();

			var decMask1 = Sse2.And(
				Sse2.CompareGreaterThan(vec1, zeros.GetLower()),
				Sse2.CompareGreaterThan(nines.GetLower(), vec1));
			var hexMaskL1 = Sse2.And(
				Sse2.CompareGreaterThan(vec1, a.GetLower()),
				Sse2.CompareGreaterThan(f.GetLower(), vec1));
			var hexMaskU1 = Sse2.And(
				Sse2.CompareGreaterThan(vec1, A.GetLower()),
				Sse2.CompareGreaterThan(F.GetLower(), vec1));

			if(!ValidateMasks128(decMask1, hexMaskL1, hexMaskU1)) return false;

			var digits1 = Sse2.Or(Sse2.Or(
				Sse2.And(Sse2.Subtract(vec1, decOffset.GetLower()),  decMask1),
				Sse2.And(Sse2.Subtract(vec1, hexOffsetL.GetLower()), hexMaskL1)),
				Sse2.And(Sse2.Subtract(vec1, hexOffsetU.GetLower()), hexMaskU1)).AsInt16();
			var part1 = Sse2.PackUnsignedSaturate(
				Sse2.Add(
					Sse2.ShiftLeftLogical (Sse2.And(digits1, Vector128.Create(unchecked((short)0x000f))), 4),
					Sse2.ShiftRightLogical(Sse2.And(digits1, Vector128.Create(unchecked((short)0x0f00))), 8)),
				default);

			Sse2.StoreScalar((int*)(output + 16), part1.AsInt32());
		}
		else
		#endif // NETCOREAPP
		{
			for(int i = 0; i < Size; ++i)
			{
				var d0 = HashUtils.TryParseByteToHexDigit(hashStringUtf8[i * 2 + 0]);
				var d1 = HashUtils.TryParseByteToHexDigit(hashStringUtf8[i * 2 + 1]);
				if((d0 | d1) < 0) return false;
				output[i] = (byte)(d0 * 16 + d1);
			}
		}
		hash = parsed;
		return true;
	}

	public static unsafe bool TryParse(char* hashStringUtf16, out Sha1Hash hash)
	{
		var parsed = default(Sha1Hash);
		var output = (byte*)&parsed;
		#if NETCOREAPP
		if(Avx2.IsSupported)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool ValidateMasks256(Vector256<short> m0, Vector256<short> m1, Vector256<short> m2)
				=> Avx2.MoveMask(Avx2.Xor(Avx2.Xor(m0, m1), m2).AsByte()) == unchecked((int)0xFFFFFFFF);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool ValidateMasks128(Vector128<short> m0, Vector128<short> m1, Vector128<short> m2)
				=> Sse2.MoveMask(Sse2.Xor(Sse2.Xor(m0, m1), m2).AsByte()) == 0x0000FFFF;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static Vector256<short> GetRangeMask256(Vector256<short> fromExclusive, Vector256<short> value, Vector256<short> toExclusive)
				=> Avx2.And(
					Avx2.CompareGreaterThan(value, fromExclusive),
					Avx2.CompareGreaterThan(toExclusive, value));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static Vector128<short> GetRangeMask128(Vector128<short> fromExclusive, Vector128<short> value, Vector128<short> toExclusive)
				=> Sse2.And(
					Sse2.CompareGreaterThan(value, fromExclusive),
					Sse2.CompareGreaterThan(toExclusive, value));

			var zeros      = Vector256.Create((short)('0' - 1));
			var nines      = Vector256.Create((short)('9' + 1));
			var a          = Vector256.Create((short)('a' - 1));
			var f          = Vector256.Create((short)('f' + 1));
			var A          = Vector256.Create((short)('A' - 1));
			var F          = Vector256.Create((short)('F' + 1));
			var decOffset  = Vector256.Create((short)('0'));
			var hexOffsetL = Vector256.Create((short)('a' - 10));
			var hexOffsetU = Vector256.Create((short)('A' - 10));

			var vec0      = Avx.LoadVector256((short*)(hashStringUtf16 + 16 * 0));
			var decMask0  = GetRangeMask256(zeros, vec0, nines);
			var hexMaskL0 = GetRangeMask256(a, vec0, f);
			var hexMaskU0 = GetRangeMask256(A, vec0, F);

			if(!ValidateMasks256(decMask0, hexMaskL0, hexMaskU0)) return false;

			var x0 = Avx2.Or(Avx2.Or(
				Avx2.And(Avx2.Subtract(vec0, decOffset), decMask0),
				Avx2.And(Avx2.Subtract(vec0, hexOffsetL), hexMaskL0)),
				Avx2.And(Avx2.Subtract(vec0, hexOffsetU), hexMaskU0));

			var vec1      = Avx.LoadVector256((short*)(hashStringUtf16 + 16 * 1));
			var decMask1  = GetRangeMask256(zeros, vec1, nines);
			var hexMaskL1 = GetRangeMask256(a, vec1, f);
			var hexMaskU1 = GetRangeMask256(A, vec1, F);

			if(!ValidateMasks256(decMask1, hexMaskL1, hexMaskU1)) return false;

			var x1 = Avx2.Or(Avx2.Or(
				Avx2.And(Avx2.Subtract(vec1, decOffset), decMask1),
				Avx2.And(Avx2.Subtract(vec1, hexOffsetL), hexMaskL1)),
				Avx2.And(Avx2.Subtract(vec1, hexOffsetU), hexMaskU1));

			var fx = Avx2.PackUnsignedSaturate(x0, x1);
			fx = Avx2.Permute4x64(fx.AsUInt64(), 0b11_01_10_00).AsByte();
			var fy = Avx2.MultiplyAddAdjacent(fx, Vector256.Create((sbyte)
				16, 1, 16, 1, 16, 1, 16, 1, 16, 1, 16, 1, 16, 1, 16, 1,
				16, 1, 16, 1, 16, 1, 16, 1, 16, 1, 16, 1, 16, 1, 16, 1));
			fx = Avx2.PackUnsignedSaturate(fy, default);
			fx = Avx2.Permute4x64(fx.AsUInt64(), 0b11_01_10_00).AsByte();
			Sse2.Store(output, fx.GetLower());

			var vec2      = Sse2.LoadVector128((short*)(hashStringUtf16 + 32 * 1));
			var decMask2  = GetRangeMask128(zeros.GetLower(), vec2, nines.GetLower());
			var hexMaskL2 = GetRangeMask128(a.GetLower(), vec2, f.GetLower());
			var hexMaskU2 = GetRangeMask128(A.GetLower(), vec2, F.GetLower());

			if(!ValidateMasks128(decMask2, hexMaskL2, hexMaskU2)) return false;

			var x3 = Sse2.Or(Sse2.Or(
				Sse2.And(Sse2.Subtract(vec2, decOffset.GetLower()),  decMask2),
				Sse2.And(Sse2.Subtract(vec2, hexOffsetL.GetLower()), hexMaskL2)),
				Sse2.And(Sse2.Subtract(vec2, hexOffsetU.GetLower()), hexMaskU2));

			var x4 = Sse2.MultiplyAddAdjacent(Vector128.Create((short)
				16, 1, 16, 1, 16, 1, 16, 1), x3);
			var shuf = Ssse3.Shuffle(x4.AsByte(), Vector128.Create((byte)
				0, 4, 8, 12, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff));
			Sse2.StoreScalar((int*)(output + 16), shuf.AsInt32());
		}
		else
		#endif
		{
			for(int i = 0; i < Size; ++i)
			{
				var d0 = HashUtils.TryParseCharToHexDigit(hashStringUtf16[i * 2 + 0]);
				var d1 = HashUtils.TryParseCharToHexDigit(hashStringUtf16[i * 2 + 1]);
				if((d0 | d1) < 0) return false;
				output[i] = (byte)(d0 * 16 + d1);
			}
		}
		hash = parsed;
		return true;
	}

	public static unsafe Sha1Hash Parse(string hashStringUtf16)
	{
		Verify.Argument.IsNotNull(hashStringUtf16);

		return TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));
	}

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] string? hashStringUtf16, out Sha1Hash hash)
	{
		if(hashStringUtf16 is not { Length: >= HexStringLength })
		{
			hash = default;
			return false;
		}
		fixed(char* p = hashStringUtf16)
		{
			return TryParse(p, out hash);
		}
	}

	public static unsafe Sha1Hash Parse(string hashStringUtf16, int offset)
	{
		Verify.Argument.IsNotNull(hashStringUtf16);

		return TryParse(hashStringUtf16, offset, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));
	}

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] string? hashStringUtf16, int offset, out Sha1Hash hash)
	{
		if(hashStringUtf16 is null || offset < 0 || hashStringUtf16.Length - offset < HexStringLength)
		{
			hash = default;
			return false;
		}
		fixed(char* p = hashStringUtf16)
		{
			return TryParse(p + offset, out hash);
		}
	}

	public static unsafe Sha1Hash Parse(char[] hashStringUtf16)
	{
		Verify.Argument.IsNotNull(hashStringUtf16);

		return TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));
	}

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] char[]? hashString, out Sha1Hash hash)
	{
		if(hashString is not { Length: >= HexStringLength })
		{
			hash = default;
			return false;
		}
		fixed(char* p = hashString)
		{
			return TryParse(p, out hash);
		}
	}

	public static unsafe Sha1Hash Parse(char[] hashStringUtf16, int offset)
	{
		Verify.Argument.IsNotNull(hashStringUtf16);

		return TryParse(hashStringUtf16, offset, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));
	}

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] char[]? hashString, int offset, out Sha1Hash hash)
	{
		if(hashString is null || offset < 0 || hashString.Length - offset < HexStringLength)
		{
			hash = default;
			return false;
		}
		fixed(char* p = hashString)
		{
			return TryParse(p + offset, out hash);
		}
	}

	public static unsafe Sha1Hash Parse(byte[] hashStringUtf8)
	{
		Verify.Argument.IsNotNull(hashStringUtf8);

		return TryParse(hashStringUtf8, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf8));
	}

	public static unsafe Sha1Hash Parse(byte[] hashStringUtf8, int offset)
	{
		Verify.Argument.IsNotNull(hashStringUtf8);

		return TryParse(hashStringUtf8, offset, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf8));
	}

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] byte[]? hashStringUtf8, out Sha1Hash hash)
	{
		if(hashStringUtf8 is null || hashStringUtf8.Length < HexStringLength)
		{
			hash = default;
			return false;
		}
		fixed(byte* p = hashStringUtf8)
		{
			return TryParse(p, out hash);
		}
	}

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] byte[]? hashStringUtf8, int offset, out Sha1Hash hash)
	{
		if(hashStringUtf8 is null || offset < 0 || hashStringUtf8.Length - offset < HexStringLength)
		{
			hash = default;
			return false;
		}
		fixed(byte* p = hashStringUtf8)
		{
			return TryParse(p + offset, out hash);
		}
	}

#if NETCOREAPP

	public static Sha1Hash Parse(ReadOnlySpan<char> hashStringUtf16)
		=> TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse(ReadOnlySpan<char> hashStringUtf16, out Sha1Hash hash)
	{
		if(hashStringUtf16 is not { Length: >= HexStringLength })
		{
			hash = default;
			return false;
		}
		fixed(char* p = hashStringUtf16)
		{
			return TryParse(p, out hash);
		}
	}

	public static Sha1Hash Parse(ReadOnlySpan<byte> hashStringUtf8)
		=> TryParse(hashStringUtf8, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf8));

	public static unsafe bool TryParse(ReadOnlySpan<byte> hashStringUtf8, out Sha1Hash hash)
	{
		if(hashStringUtf8.Length < HexStringLength)
		{
			hash = default;
			return false;
		}
		fixed(byte* p = hashStringUtf8)
		{
			return TryParse(p, out hash);
		}
	}

#endif

	#endregion

	#region .ctor

	private Sha1Hash(DataContainer data)
	{
		_data = data;
	}

	public unsafe Sha1Hash(byte[] hash)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsTrue(hash.Length >= Size, nameof(hash), "Hash must be at least 20 bytes long.");

		fixed(byte* src = hash)
		{
			InitData(src);
		}
	}

	public unsafe Sha1Hash(byte[] hash, int offset)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsInRange(0, offset, hash.Length - Size, nameof(hash), "Hash must be at least 20 bytes long after offset.");

		fixed(byte* src = hash)
		{
			InitData(src + offset);
		}
	}

#if NETCOREAPP

	public unsafe Sha1Hash(ReadOnlySpan<byte> hash)
	{
		Verify.Argument.IsTrue(hash.Length >= Size, nameof(hash), "Hash must be at least 20 bytes long.");

		fixed(byte* src = hash)
		{
			InitData(src);
		}
	}

#endif

	private unsafe void InitData(byte* src)
	{
		fixed(byte* dst = _data.Data)
		{
#if NET7_0_OR_GREATER
			NativeMemory.Copy(src, dst, Size);
#else
			Buffer.MemoryCopy(src, dst, Size, Size);
#endif
		}
	}

	#endregion

	#region Operators

	public static explicit operator string(Sha1Hash hash) => hash.ToString();

	public static explicit operator Sha1Hash(string hash) => Parse(hash);

	public static unsafe bool operator ==(Sha1Hash a, Sha1Hash b)
	{
#if NETCOREAPP
		var l = new ReadOnlySpan<byte>(a._data.Data, Size);
		var r = new ReadOnlySpan<byte>(b._data.Data, Size);
		return l.SequenceEqual(r);
#else
		for(int i = 0; i < Size; ++i)
		{
			if(a._data.Data[i] != b._data.Data[i]) return false;
		}
		return true;
#endif
	}

	public static bool operator !=(Sha1Hash a, Sha1Hash b)
		=> !(a == b);

	public static unsafe bool operator >(Sha1Hash a, Sha1Hash b)
	{
		for(int i = 0; i < Size; ++i)
		{
			var l = a._data.Data[i];
			var r = b._data.Data[i];
			if(l > r) return true;
			if(l < r) return false;
		}
		return false;
	}

	public static unsafe bool operator >=(Sha1Hash a, Sha1Hash b)
	{
		for(int i = 0; i < Size; ++i)
		{
			var l = a._data.Data[i];
			var r = b._data.Data[i];
			if(l > r) return true;
			if(l < r) return false;
		}
		return true;
	}

	public static unsafe bool operator <(Sha1Hash a, Sha1Hash b)
	{
		for(int i = 0; i < Size; ++i)
		{
			var l = a._data.Data[i];
			var r = b._data.Data[i];
			if(l < r) return true;
			if(l > r) return false;
		}
		return false;
	}

	public static unsafe bool operator <=(Sha1Hash a, Sha1Hash b)
	{
		for(int i = 0; i < Size; ++i)
		{
			var l = a._data.Data[i];
			var r = b._data.Data[i];
			if(l < r) return true;
			if(l > r) return false;
		}
		return true;
	}

	#endregion

	#region Methods

	/// <inheritdoc/>
	public override unsafe int GetHashCode()
	{
#if NETCOREAPP
		var hc = new HashCode();
		fixed(byte* p = _data.Data)
		{
			hc.AddBytes(new ReadOnlySpan<byte>(p, Size));
		}
		return hc.ToHashCode();
#else
		fixed(byte* p = _data.Data)
		{
			var p0 = *(int*)(p + sizeof(int) * 0);
			var p1 = *(int*)(p + sizeof(int) * 1);
			var p2 = *(int*)(p + sizeof(int) * 2);
			var p3 = *(int*)(p + sizeof(int) * 3);
			var p4 = *(int*)(p + sizeof(int) * 4);
			return p0 ^ p1 ^ p2 ^ p3 ^ p4;
		}
#endif
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(returnValue: true)] object? obj)
		=> obj is Sha1Hash other && this == other;

	/// <inheritdoc/>
	public bool Equals(Sha1Hash other)
		=> this == other;

	/// <inheritdoc/>
	public int CompareTo(object? other)
	{
		if(other is null) return 1;
		if(other is not Sha1Hash hash) throw new ArgumentException("Argument must be a Hash value.", nameof(other));
		return CompareTo(hash);
	}

	/// <inheritdoc/>
	public unsafe int CompareTo(Sha1Hash other)
	{
		for(int i = 0; i < Size; ++i)
		{
			var l = _data.Data[i];
			var r = other._data.Data[i];
			if(l < r) return -1;
			if(l > r) return 1;
		}
		return 0;
	}

	public unsafe bool StartsWith(string? text)
	{
		if(text is not { Length: not 0 }) return true;
		if(text.Length > HexStringLength) return false;

		var length = text.Length;
		byte value = 0;
		var odd = (length & 1) != 0;
		length >>= 1;
		var index = 0;
		for(int i = 0; i < length; ++i)
		{
			value = _data.Data[i];
			if(HashUtils.ToHexDigit(value >>  4) != text[index++]) return false;
			if(HashUtils.ToHexDigit(value & 0xf) != text[index++]) return false;
		}
		if(odd)
		{
			if(HashUtils.ToHexDigit(_data.Data[length] >> 4) != text[index]) return false;
		}
		return true;
	}

	public unsafe byte[] ToByteArray()
	{
		var buffer = new byte[Size];
		fixed(byte* src = _data.Data)
		fixed(byte* dst = buffer)
		{
			Buffer.MemoryCopy(src, dst, Size, Size);
		}
		return buffer;
	}

	public unsafe void CopyTo(byte[] buffer, int offset)
	{
		Verify.Argument.IsNotNull(buffer);
		Verify.Argument.IsInRange(0, offset, buffer.Length - Size);

		fixed(byte* src = _data.Data)
		fixed(byte* dst = buffer)
		{
			Buffer.MemoryCopy(src, dst + offset, Size, Size);
		}
	}

#if NETCOREAPP

	public unsafe bool TryFormat(Span<char> buffer)
	{
		var length = buffer.Length;
		if(length != 0)
		{
			fixed(char* ptr = buffer)
			{
				ToString(ptr, Math.Min(length, HexStringLength));
			}
		}
		return true;
	}

#endif

	/// <inheritdoc/>
	public unsafe override string ToString()
	{
		var str = stackalloc char[HexStringLength];
		ToString(str);
		return new string(str, 0, HexStringLength);
	}

	public unsafe string ToString(int length)
	{
		Verify.Argument.IsInRange(0, length, HexStringLength);

		if(length == 0) return string.Empty;
		if(length == HexStringLength) return ToString();

		var str = stackalloc char[length];
		ToString(str, length);
		return new string(str, 0, length);
	}

	public unsafe void ToString(char* hashStringUtf16)
	{
#if NETCOREAPP
		if(Avx2.IsSupported)
		{
			fixed(byte* src = _data.Data)
			{
				Avx2HashHelper.ToHexStringFrom16Bytes(src +  0, hashStringUtf16 +  0);
				Avx2HashHelper.ToHexStringFrom4Bytes (src + 16, hashStringUtf16 + 32);
			}
		}
		else
#endif
		{
			for(int i = 0; i < Size; ++i)
			{
				var value = _data.Data[i];
				hashStringUtf16[i * 2 + 0] = HashUtils.ToHexDigit(value >> 4);
				hashStringUtf16[i * 2 + 1] = HashUtils.ToHexDigit(value & 0xf);
			}
		}
	}

	public unsafe void ToString(char* hashStringUtf16, int length)
	{
		if(length >= HexStringLength)
		{
			ToString(hashStringUtf16);
			return;
		}
		byte value = 0;
		var odd = (length & 1) != 0;
		length >>= 1;
		for(int i = 0; i < length; ++i)
		{
			value = _data.Data[i];
			hashStringUtf16[i * 2 + 0] = HashUtils.ToHexDigit(value >> 4);
			hashStringUtf16[i * 2 + 1] = HashUtils.ToHexDigit(value & 0xf);
		}
		if(odd)
		{
			hashStringUtf16[length * 2] = HashUtils.ToHexDigit(_data.Data[length] >> 4);
		}
	}

	public unsafe void ToString(StringBuilder stringBuilder)
	{
		Verify.Argument.IsNotNull(stringBuilder);

		var str = stackalloc char[HexStringLength];
		ToString(str);
		stringBuilder.Append(str, HexStringLength);
	}

	public unsafe void ToString(StringBuilder stringBuilder, int length)
	{
		Verify.Argument.IsNotNull(stringBuilder);
		Verify.Argument.IsInRange(0, length, HexStringLength);

		var str = stackalloc char[length];
		ToString(str, length);
		stringBuilder.Append(str, length);
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		if(string.IsNullOrWhiteSpace(format)) return ToString();
		if(!int.TryParse(format, NumberStyles.Integer, CultureInfo.InvariantCulture, out int length))
		{
			throw new FormatException("Unable to parse hash length.");
		}
		if(length is < 0 or > HexStringLength)
		{
			throw new FormatException("Length must be in [0; 40] range.");
		}
		return length >= HexStringLength
			? ToString()
			: ToString(length);
	}

#endregion
}
