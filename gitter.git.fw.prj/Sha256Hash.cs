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

[StructLayout(LayoutKind.Sequential)]
public readonly struct Sha256Hash : IHashValue<Sha256Hash>
{
	public const int Size = 32;
	public const int HexStringLength = Size * 2;

	sealed class EqualityComparerImpl : IEqualityComparer<Sha256Hash>
	{
		public bool Equals(Sha256Hash x, Sha256Hash y) => x == y;

		public int GetHashCode(Sha256Hash hash) => hash.GetHashCode();
	}

	sealed class HashComparerImpl : IComparer<Sha256Hash>
	{
		public int Compare(Sha256Hash x, Sha256Hash y) => x.CompareTo(y);
	}

	public static IEqualityComparer<Sha256Hash> EqualityComparer { get; } = new EqualityComparerImpl();
	public static IComparer        <Sha256Hash> Comparer         { get; } = new HashComparerImpl();

#if NET7_0_OR_GREATER
	static int IHashValue<Sha256Hash>.Size => Size;
	static int IHashValue<Sha256Hash>.HexStringLength => HexStringLength;
#endif

	unsafe struct DataContainer
	{
		public fixed byte Data[Sha256Hash.Size];
	}

	private readonly DataContainer _data;

	public static unsafe Sha256Hash Parse(byte* hashStringUtf8)
		=> TryParse(hashStringUtf8, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf8));

	public static unsafe Sha256Hash Parse(char* hashStringUtf16)
		=> TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse(byte* hashStringUtf8, out Sha256Hash hash)
	{
		var parsed = default(Sha256Hash);
		var output = (byte*)&parsed;
		#if NETCOREAPP
		if(Avx2.IsSupported)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool ValidateMasks(Vector256<sbyte> m0, Vector256<sbyte> m1, Vector256<sbyte> m2)
				=> Avx2.MoveMask(Avx2.Xor(Avx2.Xor(m0, m1), m2)) == unchecked((int)0xFFFFFFFF);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool GetDigits(byte* input, out Vector256<sbyte> digits)
			{
				var zeros      = Vector256.Create((sbyte)('0' - 1));
				var nines      = Vector256.Create((sbyte)('9' + 1));
				var a          = Vector256.Create((sbyte)('a' - 1));
				var f          = Vector256.Create((sbyte)('f' + 1));
				var A          = Vector256.Create((sbyte)('A' - 1));
				var F          = Vector256.Create((sbyte)('F' + 1));
				var decOffset  = Vector256.Create((sbyte)('0'));
				var hexOffsetL = Vector256.Create((sbyte)('a' - 10));
				var hexOffsetU = Vector256.Create((sbyte)('A' - 10));

				var vec = Avx.LoadVector256((sbyte*)(input + 32 * 0));

				var decMask = Avx2.And(
					Avx2.CompareGreaterThan(vec, zeros),
					Avx2.CompareGreaterThan(nines, vec));
				var hexMaskL = Avx2.And(
					Avx2.CompareGreaterThan(vec, a),
					Avx2.CompareGreaterThan(f, vec));
				var hexMaskU = Avx2.And(
					Avx2.CompareGreaterThan(vec, A),
					Avx2.CompareGreaterThan(F, vec));

				if(!ValidateMasks(decMask, hexMaskL, hexMaskU))
				{
					digits = default;
					return false;
				}

				digits = Avx2.Or(Avx2.Or(
					Avx2.And(Avx2.Subtract(vec, decOffset),  decMask),
					Avx2.And(Avx2.Subtract(vec, hexOffsetL), hexMaskL)),
					Avx2.And(Avx2.Subtract(vec, hexOffsetU), hexMaskU));
				return true;
			}

			if(!GetDigits(hashStringUtf8 + 32 * 0, out var digits0)) return false;
			var part0 = Avx2.Permute4x64(
				Avx2.PackUnsignedSaturate(
					Avx2.Add(
						Avx2.ShiftLeftLogical (Avx2.And(digits0.AsInt16(), Vector256.Create(unchecked((short)0x000f))), 4),
						Avx2.ShiftRightLogical(Avx2.And(digits0.AsInt16(), Vector256.Create(unchecked((short)0x0f00))), 8)),
					Vector256<short>.Zero).AsUInt64(),
				control: 0b11_01_10_00).AsByte();

			if(!GetDigits(hashStringUtf8 + 32 * 1, out var digits1)) return false;
			var part1 = Avx2.Permute4x64(
				Avx2.PackUnsignedSaturate(
					Avx2.Add(
						Avx2.ShiftLeftLogical (Avx2.And(digits1.AsInt16(), Vector256.Create(unchecked((short)0x000f))), 4),
						Avx2.ShiftRightLogical(Avx2.And(digits1.AsInt16(), Vector256.Create(unchecked((short)0x0f00))), 8)),
					Vector256<short>.Zero).AsUInt64(),
				control: 0b10_00_11_01).AsByte();

			Avx.Store(output, Avx2.Or(part0, part1));
		}
		else
		#endif // NETCOREAPP
		{
			for(int i = 0; i < Size; ++i)
			{
				output[i] = (byte)(
					HashUtils.ParseByteToHexDigit(hashStringUtf8[i * 2 + 0]) * 16 +
					HashUtils.ParseByteToHexDigit(hashStringUtf8[i * 2 + 1]));
			}
		}
		hash = parsed;
		return true;
	}

	public static unsafe bool TryParse(char* hashStringUtf16, out Sha256Hash hash)
	{
		var parsed = default(Sha256Hash);
		var output = (byte*)&parsed;
		#if NETCOREAPP
		if(Avx2.IsSupported)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool ValidateMasks(Vector256<short> m0, Vector256<short> m1, Vector256<short> m2)
				=> Avx2.MoveMask(Avx2.Xor(Avx2.Xor(m0, m1), m2).AsByte()) == 0x0000FFFF;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool GetValues(char* input, out Vector256<short> values)
			{
				var zeros      = Vector256.Create((short)('0' - 1));
				var nines      = Vector256.Create((short)('9' + 1));
				var a          = Vector256.Create((short)('a' - 1));
				var f          = Vector256.Create((short)('f' + 1));
				var A          = Vector256.Create((short)('A' - 1));
				var F          = Vector256.Create((short)('F' + 1));
				var decOffset  = Vector256.Create((short)('0'));
				var hexOffsetL = Vector256.Create((short)('a' - 10));
				var hexOffsetU = Vector256.Create((short)('A' - 10));
				var mul = Vector256.Create((short)
					16, 1, 16, 1, 16, 1, 16, 1,
					16, 1, 16, 1, 16, 1, 16, 1);

				var vec0 = Avx.LoadVector256((short*)input + 16 * 0);
				var decMask0 = Avx2.And(
					Avx2.CompareGreaterThan(vec0, zeros),
					Avx2.CompareGreaterThan(nines, vec0));
				var hexMaskL0 = Avx2.And(
					Avx2.CompareGreaterThan(vec0, a),
					Avx2.CompareGreaterThan(f, vec0));
				var hexMaskU0 = Avx2.And(
					Avx2.CompareGreaterThan(vec0, A),
					Avx2.CompareGreaterThan(F, vec0));

				if(!ValidateMasks(decMask0, hexMaskL0, hexMaskU0))
				{
					values = default;
					return false;
				}

				var x0 = Avx2.Or(Avx2.Or(
					Avx2.And(Avx2.Subtract(vec0, decOffset), decMask0),
					Avx2.And(Avx2.Subtract(vec0, hexOffsetL), hexMaskL0)),
					Avx2.And(Avx2.Subtract(vec0, hexOffsetU), hexMaskU0));

				x0 = Avx2.MultiplyLow(x0, mul);

				var vec1 = Avx.LoadVector256((short*)input + 16 * 1);
				var decMask1 = Avx2.And(
					Avx2.CompareGreaterThan(vec1, zeros),
					Avx2.CompareGreaterThan(nines, vec1));
				var hexMaskL1 = Avx2.And(
					Avx2.CompareGreaterThan(vec1, a),
					Avx2.CompareGreaterThan(f, vec1));
				var hexMaskU1 = Avx2.And(
					Avx2.CompareGreaterThan(vec1, A),
					Avx2.CompareGreaterThan(F, vec1));

				if(!ValidateMasks(decMask1, hexMaskL1, hexMaskU1))
				{
					values = default;
					return false;
				}

				var x1 = Avx2.Or(Avx2.Or(
					Avx2.And(Avx2.Subtract(vec1, decOffset), decMask1),
					Avx2.And(Avx2.Subtract(vec1, hexOffsetL), hexMaskL1)),
					Avx2.And(Avx2.Subtract(vec1, hexOffsetU), hexMaskU1));

				x1 = Avx2.MultiplyLow(x1, mul);

				var x2 = Avx2.HorizontalAdd(x0, x1);

				values = Avx2.Permute4x64(x2.AsUInt64(), 0b11_01_10_00).AsInt16();
				return true;
			}

			if(!GetValues(hashStringUtf16 + 32 * 0, out var values1)) return false;
			if(!GetValues(hashStringUtf16 + 32 * 1, out var values2)) return false;

			var combined = Avx2.PackUnsignedSaturate(values1, values2);
			combined = Avx2.Permute4x64(combined.AsUInt64(), 0b11_01_10_00).AsByte();

			Avx.Store(output, combined);
		}
		else
		#endif
		{
			for(int i = 0; i < Size; ++i)
			{
				output[i] = (byte)(
					HashUtils.ParseCharToHexDigit(hashStringUtf16[i * 2 + 0]) * 16 +
					HashUtils.ParseCharToHexDigit(hashStringUtf16[i * 2 + 1]));
			}
		}
		hash = parsed;
		return true;
	}

	public static unsafe Sha256Hash Parse(string hashStringUtf16)
		=> TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] string? hashStringUtf16, out Sha256Hash hash)
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

	public static unsafe Sha256Hash Parse(string hashStringUtf16, int offset)
		=> TryParse(hashStringUtf16, offset, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] string? hashStringUtf16, int offset, out Sha256Hash hash)
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

	public static unsafe Sha256Hash Parse(char[] hashStringUtf16)
		=> TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] char[]? hashString, out Sha256Hash hash)
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

	public static unsafe Sha256Hash Parse(char[] hashStringUtf16, int offset)
		=> TryParse(hashStringUtf16, offset, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] char[]? hashString, int offset, out Sha256Hash hash)
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

	public static unsafe Sha256Hash Parse(byte[] hashStringUtf8, int offset)
		=> TryParse(hashStringUtf8, offset, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf8));

	public static unsafe bool TryParse([NotNullWhen(returnValue: true)] byte[]? hashStringUtf8, int offset, out Sha256Hash hash)
	{
		if(hashStringUtf8 is null || offset < 0 || hashStringUtf8.Length - offset < HexStringLength)
		{
			hash = default;
			return false;
		}
		fixed(byte* p = hashStringUtf8)
		{
			return TryParse(p, out hash);
		}
	}

#if NETCOREAPP

	public static Sha256Hash Parse(ReadOnlySpan<char> hashStringUtf16)
		=> TryParse(hashStringUtf16, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf16));

	public static unsafe bool TryParse(ReadOnlySpan<char> hashStringUtf16, out Sha256Hash hash)
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

	public static Sha256Hash Parse(ReadOnlySpan<byte> hashStringUtf8)
		=> TryParse(hashStringUtf8, out var hash)
			? hash
			: throw new ArgumentException("Invalid hexadecimal sequence.", nameof(hashStringUtf8));

	public static unsafe bool TryParse(ReadOnlySpan<byte> hashStringUtf8, out Sha256Hash hash)
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

	private Sha256Hash(DataContainer data)
	{
		_data = data;
	}

	public unsafe Sha256Hash(byte[] hash)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsTrue(hash.Length >= Size, nameof(hash), "Hash must be at least 20 bytes long.");

		fixed(byte* src = hash)
		{
			InitData(src);
		}
	}

	public unsafe Sha256Hash(byte[] hash, int offset)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsInRange(0, offset, hash.Length - Size, nameof(hash), "Hash must be at least 20 bytes long after offset.");

		fixed(byte* src = hash)
		{
			InitData(src + offset);
		}
	}

#if NETCOREAPP

	public unsafe Sha256Hash(ReadOnlySpan<byte> hash)
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
			var p5 = *(int*)(p + sizeof(int) * 5);
			var p6 = *(int*)(p + sizeof(int) * 6);
			var p7 = *(int*)(p + sizeof(int) * 7);
			return p0 ^ p1 ^ p2 ^ p3 ^ p4 ^ p5 ^ p6 ^ p7;
		}
#endif
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(returnValue: true)] object? obj)
		=> obj is Sha256Hash other && this == other;

	/// <inheritdoc/>
	public bool Equals(Sha256Hash other)
		=> this == other;

	/// <inheritdoc/>
	public int CompareTo(object? other)
	{
		if(other is null) return 1;
		return other is Sha256Hash hash
			? CompareTo(hash)
			: throw new ArgumentException($"Argument must be a {nameof(Sha256Hash)} value.", nameof(other));
	}

	/// <inheritdoc/>
	public unsafe int CompareTo(Sha256Hash other)
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

	public static unsafe bool operator ==(Sha256Hash a, Sha256Hash b)
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

	public static bool operator !=(Sha256Hash a, Sha256Hash b)
		=> !(a == b);

	public static unsafe bool operator >(Sha256Hash a, Sha256Hash b)
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

	public static unsafe bool operator >=(Sha256Hash a, Sha256Hash b)
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

	public static unsafe bool operator <(Sha256Hash a, Sha256Hash b)
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

	public static unsafe bool operator <=(Sha256Hash a, Sha256Hash b)
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

#if NETCOREAPP

	public unsafe bool TryFormat(Span<char> buffer)
	{
		var length = buffer.Length;

		fixed(char* ptr = buffer)
		{
			ToString(ptr, Math.Min(length, HexStringLength));
		}

		return true;
	}

#endif

	public unsafe void ToString(char* hashStringUtf16)
	{
#if NETCOREAPP
		if(Avx2.IsSupported)
		{
			fixed(byte* src = _data.Data)
			{
				Avx2HashHelper.ToHexStringFrom16Bytes(src +  0, hashStringUtf16 +  0);
				Avx2HashHelper.ToHexStringFrom16Bytes(src + 16, hashStringUtf16 + 32);
			}
		}
		else
#endif
		{
			for(int i = 0; i < HexStringLength; ++i)
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

	/// <inheritdoc/>
	public unsafe override string ToString()
	{
		var buffer = stackalloc char[HexStringLength];
		ToString(buffer);
		return new(buffer, 0, HexStringLength);
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
			throw new FormatException("Length must be in [0; 64] range.");
		}
		return length >= HexStringLength
			? ToString()
			: ToString(length);
	}
}
