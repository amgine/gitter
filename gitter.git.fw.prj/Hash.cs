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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#nullable enable

[StructLayout(LayoutKind.Sequential)]
public readonly struct Hash : IEquatable<Hash>, IComparable<Hash>, IComparable, IFormattable
{
	public const int Size = 20;
	public const int HexStringLength = Size * 2;

	#region Helpers

	sealed class HashEqualityComparer : IEqualityComparer<Hash>
	{
		public bool Equals(Hash x, Hash y) => x == y;

		public int GetHashCode(Hash hash) => hash.GetHashCode();
	}

	sealed class HashComparer : IComparer<Hash>
	{
		public int Compare(Hash x, Hash y) => x.CompareTo(y);
	}

	#endregion

	#region Static

	public static IEqualityComparer<Hash> EqualityComparer { get; } = new HashEqualityComparer();

	public static IComparer<Hash> Comparer { get; } = new HashComparer();

	#endregion

	#region Data

	private readonly uint _part0;
	private readonly uint _part1;
	private readonly uint _part2;
	private readonly uint _part3;
	private readonly uint _part4;

	#endregion

	#region Static

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int TryParseCharToHexDigit(char ch)
		=> ch switch
		{
			>= '0' and <= '9' => ch - '0',
			>= 'a' and <= 'f' => 10 + (ch - 'a'),
			>= 'A' and <= 'F' => 10 + (ch - 'A'),
			_ => -1,
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ParseCharToHexDigit(char ch)
		=> ch switch
		{
			>= '0' and <= '9' => ch - '0',
			>= 'a' and <= 'f' => 10 + (ch - 'a'),
			>= 'A' and <= 'F' => 10 + (ch - 'A'),
			_ => throw new ArgumentException("Hexadecimal digit expected.", nameof(ch)),
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int TryParseByteToHexDigit(byte ch)
		=> ch switch
		{
			>= (byte)'0' and <= (byte)'9' => ch - '0',
			>= (byte)'a' and <= (byte)'f' => 10 + (ch - 'a'),
			>= (byte)'A' and <= (byte)'F' => 10 + (ch - 'A'),
			_ => -1,
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ParseByteToHexDigit(byte ch)
		=> ch switch
		{
			>= (byte)'0' and <= (byte)'9' => ch - '0',
			>= (byte)'a' and <= (byte)'f' => 10 + (ch - 'a'),
			>= (byte)'A' and <= (byte)'F' => 10 + (ch - 'A'),
			_ => throw new ArgumentException("Hexadecimal digit expected.", nameof(ch)),
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static char ToHexDigit(uint digit)
		=> digit >= 10 ? (char)('a' + (digit - 10)) : (char)('0' + digit);

	private static bool TryParsePart(string hash, int offset, out uint part)
	{
		part = 0;
		for(int i = 0; i < 8; ++i)
		{
			int digit = TryParseCharToHexDigit(hash[offset + i]);
			if(digit < 0)
			{
				return false;
			}
			part = (part << 4) + (uint)digit;
		}
		return true;
	}

	private static bool TryParsePart(char[] hash, int offset, out uint part)
	{
		part = 0;
		for(int i = 0; i < 8; ++i)
		{
			int digit = TryParseCharToHexDigit(hash[offset + i]);
			if(digit < 0) return false;
			part = (part << 4) + (uint)digit;
		}
		return true;
	}

	private static bool TryParsePart(byte[] hash, int offset, out uint part)
	{
		part = 0;
		for(int i = 0; i < 8; ++i)
		{
			int digit = TryParseByteToHexDigit(hash[offset + i]);
			if(digit < 0) return false;
			part = (part << 4) + (uint)digit;
		}
		return true;
	}

#if NETCOREAPP

	private static bool TryParsePart(ReadOnlySpan<byte> hash, out uint part)
	{
		part = 0;
		for(int i = 0; i < 8; ++i)
		{
			int digit = TryParseByteToHexDigit(hash[i]);
			if(digit < 0) return false;
			part = (part << 4) + (uint)digit;
		}
		return true;
	}

#endif

	private static uint ParsePart(string hash, int offset)
	{
		uint result = 0;
		for(int i = 0; i < 8; ++i)
		{
			int digit = ParseCharToHexDigit(hash[offset + i]);
			result = (result << 4) + (uint)digit;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ParsePart(char[] hash, int offset) =>
		(uint)ParseCharToHexDigit(hash[offset + 0]) << 28 |
		(uint)ParseCharToHexDigit(hash[offset + 1]) << 24 |
		(uint)ParseCharToHexDigit(hash[offset + 2]) << 20 |
		(uint)ParseCharToHexDigit(hash[offset + 3]) << 16 |
		(uint)ParseCharToHexDigit(hash[offset + 4]) << 12 |
		(uint)ParseCharToHexDigit(hash[offset + 5]) <<  8 |
		(uint)ParseCharToHexDigit(hash[offset + 6]) <<  4 |
		(uint)ParseCharToHexDigit(hash[offset + 7]) <<  0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ParsePart(byte[] hash, int offset) =>
		(uint)ParseByteToHexDigit(hash[offset + 0]) << 28 |
		(uint)ParseByteToHexDigit(hash[offset + 1]) << 24 |
		(uint)ParseByteToHexDigit(hash[offset + 2]) << 20 |
		(uint)ParseByteToHexDigit(hash[offset + 3]) << 16 |
		(uint)ParseByteToHexDigit(hash[offset + 4]) << 12 |
		(uint)ParseByteToHexDigit(hash[offset + 5]) <<  8 |
		(uint)ParseByteToHexDigit(hash[offset + 6]) <<  4 |
		(uint)ParseByteToHexDigit(hash[offset + 7]) <<  0;

#if NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ParsePart(ReadOnlySpan<char> hash) =>
		(uint)ParseCharToHexDigit(hash[0]) << 28 |
		(uint)ParseCharToHexDigit(hash[1]) << 24 |
		(uint)ParseCharToHexDigit(hash[2]) << 20 |
		(uint)ParseCharToHexDigit(hash[3]) << 16 |
		(uint)ParseCharToHexDigit(hash[4]) << 12 |
		(uint)ParseCharToHexDigit(hash[5]) <<  8 |
		(uint)ParseCharToHexDigit(hash[6]) <<  4 |
		(uint)ParseCharToHexDigit(hash[7]) <<  0;

#endif

	private static uint EvalPart(byte[] hash, int offset)
	{
		uint result = 0;
		for(int i = 0; i < 4; ++i)
		{
			result = (result << 8) + (uint)hash[offset++];
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void DumpPart(uint part, char* buffer)
	{
		buffer[0] = ToHexDigit((part >> 28) & 0x0f);
		buffer[1] = ToHexDigit((part >> 24) & 0x0f);
		buffer[2] = ToHexDigit((part >> 20) & 0x0f);
		buffer[3] = ToHexDigit((part >> 16) & 0x0f);
		buffer[4] = ToHexDigit((part >> 12) & 0x0f);
		buffer[5] = ToHexDigit((part >>  8) & 0x0f);
		buffer[6] = ToHexDigit((part >>  4) & 0x0f);
		buffer[7] = ToHexDigit((part >>  0) & 0x0f);
	}

	private static unsafe void DumpPart(uint part, char* buffer, int length)
	{
		if(length >= 8)
		{
			DumpPart(part, buffer);
			return;
		}
		if(length < 1) return; buffer[0] = ToHexDigit((part >> 28) & 0x0f);
		if(length < 2) return; buffer[1] = ToHexDigit((part >> 24) & 0x0f);
		if(length < 3) return; buffer[2] = ToHexDigit((part >> 20) & 0x0f);
		if(length < 4) return; buffer[3] = ToHexDigit((part >> 16) & 0x0f);
		if(length < 5) return; buffer[4] = ToHexDigit((part >> 12) & 0x0f);
		if(length < 6) return; buffer[5] = ToHexDigit((part >>  8) & 0x0f);
		if(length < 7) return; buffer[6] = ToHexDigit((part >>  4) & 0x0f);
		if(length < 8) return; buffer[7] = ToHexDigit((part >>  0) & 0x0f);
	}

	private static void DumpPart(uint part, StringBuilder stringBuilder)
	{
		Assert.IsNotNull(stringBuilder);

		for(int i = 28; i >= 0; i -= 4)
		{
			stringBuilder.Append(ToHexDigit((part >> i) & 0x0f));
		}
	}

	private static void DumpPart(uint part, StringBuilder stringBuilder, int length)
	{
		Assert.IsNotNull(stringBuilder);

		for(int i = 28; i >= 0 && length > 0; i -= 4, --length)
		{
			stringBuilder.Append(ToHexDigit((part >> i) & 0x0f));
		}
	}

	public static bool TryParse(string str, out Hash hash)
	{
		if(str is null || str.Length < HexStringLength)
		{
			hash = default;
			return false;
		}
		if (TryParsePart(str,  0, out var part0) &&
			TryParsePart(str,  8, out var part1) &&
			TryParsePart(str, 16, out var part2) &&
			TryParsePart(str, 24, out var part3) &&
			TryParsePart(str, 32, out var part4))
		{
			hash = new Hash(part0, part1, part2, part3, part4);
			return true;
		}
		hash = default;
		return false;
	}

	public static bool TryParse(string str, int offset, out Hash hash)
	{
		if(str is null || offset < 0 || str.Length - offset < HexStringLength)
		{
			hash = default;
			return false;
		}
		if (TryParsePart(str,  0 + offset, out var part0) &&
			TryParsePart(str,  8 + offset, out var part1) &&
			TryParsePart(str, 16 + offset, out var part2) &&
			TryParsePart(str, 24 + offset, out var part3) &&
			TryParsePart(str, 32 + offset, out var part4))
		{
			hash = new Hash(part0, part1, part2, part3, part4);
			return true;
		}
		hash = default;
		return false;
	}

	public static bool TryParse(char[] str, out Hash hash)
	{
		if(str is not { Length: >= HexStringLength })
		{
			hash = default;
			return false;
		}
		if (TryParsePart(str,  0, out var part0) &&
			TryParsePart(str,  8, out var part1) &&
			TryParsePart(str, 16, out var part2) &&
			TryParsePart(str, 24, out var part3) &&
			TryParsePart(str, 32, out var part4))
		{
			hash = new Hash(part0, part1, part2, part3, part4);
			return true;
		}
		hash = default;
		return false;
	}

	public static bool TryParse(char[] str, int offset, out Hash hash)
	{
		if(str is null || offset < 0 || str.Length - offset < HexStringLength)
		{
			hash = default;
			return false;
		}
		if (TryParsePart(str,  0 + offset, out var part0) &&
			TryParsePart(str,  8 + offset, out var part1) &&
			TryParsePart(str, 16 + offset, out var part2) &&
			TryParsePart(str, 24 + offset, out var part3) &&
			TryParsePart(str, 32 + offset, out var part4))
		{
			hash = new Hash(part0, part1, part2, part3, part4);
			return true;
		}
		hash = default;
		return false;
	}

	public static bool TryParse(byte[] str, int offset, out Hash hash)
	{
		if(str is null || offset < 0 || str.Length - offset < HexStringLength)
		{
			hash = default;
			return false;
		}
		if (TryParsePart(str,  0 + offset, out var part0) &&
			TryParsePart(str,  8 + offset, out var part1) &&
			TryParsePart(str, 16 + offset, out var part2) &&
			TryParsePart(str, 24 + offset, out var part3) &&
			TryParsePart(str, 32 + offset, out var part4))
		{
			hash = new Hash(part0, part1, part2, part3, part4);
			return true;
		}
		hash = default;
		return false;
	}

#if NETCOREAPP

	public static bool TryParse(ReadOnlySpan<byte> utf8str, out Hash hash)
	{
		if(utf8str.Length < HexStringLength)
		{
			hash = default;
			return false;
		}
		if (TryParsePart(utf8str.Slice( 0, 8), out var part0) &&
			TryParsePart(utf8str.Slice( 8, 8), out var part1) &&
			TryParsePart(utf8str.Slice(16, 8), out var part2) &&
			TryParsePart(utf8str.Slice(24, 8), out var part3) &&
			TryParsePart(utf8str.Slice(32, 8), out var part4))
		{
			hash = new Hash(part0, part1, part2, part3, part4);
			return true;
		}
		hash = default;
		return false;
	}

#endif

	#endregion

	#region .ctor

	private Hash(uint part0, uint part1, uint part2, uint part3, uint part4)
	{
		_part0 = part0;
		_part1 = part1;
		_part2 = part2;
		_part3 = part3;
		_part4 = part4;
	}

	public Hash(byte[] hash)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsTrue(hash.Length >= Size, nameof(hash), "Hash must be at least 20 bytes long.");

		_part0 = EvalPart(hash,  0);
		_part1 = EvalPart(hash,  4);
		_part2 = EvalPart(hash,  8);
		_part3 = EvalPart(hash, 12);
		_part4 = EvalPart(hash, 16);
	}

	public Hash(byte[] hash, int offset)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsInRange(0, offset, hash.Length - Size, nameof(hash), "Hash must be at least 20 bytes long after offset.");

		_part0 = EvalPart(hash,  0 + offset);
		_part1 = EvalPart(hash,  4 + offset);
		_part2 = EvalPart(hash,  8 + offset);
		_part3 = EvalPart(hash, 12 + offset);
		_part4 = EvalPart(hash, 16 + offset);
	}

	public Hash(string hash)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsTrue(hash.Length >= HexStringLength, nameof(hash), "Hash must be at least 40 characters long.");

		_part0 = ParsePart(hash,  0);
		_part1 = ParsePart(hash,  8);
		_part2 = ParsePart(hash, 16);
		_part3 = ParsePart(hash, 24);
		_part4 = ParsePart(hash, 32);
	}

	public Hash(string hash, int offset)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsInRange(0, offset, hash.Length - HexStringLength, nameof(hash), "Hash must be at least 40 characters long after offset.");

		_part0 = ParsePart(hash,  0 + offset);
		_part1 = ParsePart(hash,  8 + offset);
		_part2 = ParsePart(hash, 16 + offset);
		_part3 = ParsePart(hash, 24 + offset);
		_part4 = ParsePart(hash, 32 + offset);
	}

	public Hash(char[] hash)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsTrue(hash.Length >= HexStringLength, nameof(hash), "Hash must be at least 40 characters long.");

		_part0 = ParsePart(hash,  0);
		_part1 = ParsePart(hash,  8);
		_part2 = ParsePart(hash, 16);
		_part3 = ParsePart(hash, 24);
		_part4 = ParsePart(hash, 32);
	}

	public Hash(char[] hash, int offset)
	{
		Verify.Argument.IsNotNull(hash);
		Verify.Argument.IsInRange(0, offset, hash.Length - HexStringLength, nameof(hash), "Hash must be at least 40 characters long after offset.");

		_part0 = ParsePart(hash,  0 + offset);
		_part1 = ParsePart(hash,  8 + offset);
		_part2 = ParsePart(hash, 16 + offset);
		_part3 = ParsePart(hash, 24 + offset);
		_part4 = ParsePart(hash, 32 + offset);
	}

#if NETCOREAPP

	public Hash(ReadOnlySpan<char> hash)
	{
		Verify.Argument.IsTrue(hash.Length >= HexStringLength, nameof(hash), "Hash must be at least 40 characters long.");

		_part0 = ParsePart(hash.Slice( 0, 8));
		_part1 = ParsePart(hash.Slice( 8, 8));
		_part2 = ParsePart(hash.Slice(16, 8));
		_part3 = ParsePart(hash.Slice(24, 8));
		_part4 = ParsePart(hash.Slice(32, 8));
	}

#endif

	#endregion

	#region Operators

	public static explicit operator string(Hash hash) => hash.ToString();

	public static explicit operator Hash(string hash) => new(hash);

	public static bool operator ==(Hash a, Hash b) =>
		a._part0 == b._part0 &&
		a._part1 == b._part1 &&
		a._part2 == b._part2 &&
		a._part3 == b._part3 &&
		a._part4 == b._part4;

	public static bool operator !=(Hash a, Hash b) =>
		a._part0 != b._part0 ||
		a._part1 != b._part1 ||
		a._part2 != b._part2 ||
		a._part3 != b._part3 ||
		a._part4 != b._part4;

	public static bool operator >(Hash a, Hash b)
	{
		if(a._part1 < b._part1) return false;
		if(a._part1 > b._part1) return true;
		if(a._part2 < b._part2) return false;
		if(a._part2 > b._part2) return true;
		if(a._part3 < b._part3) return false;
		if(a._part3 > b._part3) return true;
		if(a._part4 < b._part4) return false;
		if(a._part4 > b._part4) return true;
		return false;
	}

	public static bool operator >=(Hash a, Hash b)
	{
		if(a._part1 < b._part1) return false;
		if(a._part1 > b._part1) return true;
		if(a._part2 < b._part2) return false;
		if(a._part2 > b._part2) return true;
		if(a._part3 < b._part3) return false;
		if(a._part3 > b._part3) return true;
		if(a._part4 < b._part4) return false;
		if(a._part4 > b._part4) return true;
		return true;
	}

	public static bool operator <(Hash a, Hash b)
	{
		if(a._part1 > b._part1) return false;
		if(a._part1 < b._part1) return true;
		if(a._part2 > b._part2) return false;
		if(a._part2 < b._part2) return true;
		if(a._part3 > b._part3) return false;
		if(a._part3 < b._part3) return true;
		if(a._part4 > b._part4) return false;
		if(a._part4 < b._part4) return true;
		return false;
	}

	public static bool operator <=(Hash a, Hash b)
	{
		if(a._part1 > b._part1) return false;
		if(a._part1 < b._part1) return true;
		if(a._part2 > b._part2) return false;
		if(a._part2 < b._part2) return true;
		if(a._part3 > b._part3) return false;
		if(a._part3 < b._part3) return true;
		if(a._part4 > b._part4) return false;
		if(a._part4 < b._part4) return true;
		return true;
	}

	#endregion

	#region Methods

	/// <inheritdoc/>
	public override int GetHashCode()
		=> (int)(_part0 ^ _part1 ^ _part2 ^ _part3 ^ _part4);

	/// <inheritdoc/>
	public override bool Equals(object? obj)
		=> obj is Hash other && this == other;

	/// <inheritdoc/>
	public bool Equals(Hash other) => this == other;

	/// <inheritdoc/>
	public int CompareTo(object? other)
	{
		if(other is null) return 1;
		if(other is not Hash hash) throw new ArgumentException("Argument must be a Hash value.", nameof(other));
		return CompareTo(hash);
	}

	/// <inheritdoc/>
	public int CompareTo(Hash other)
	{
		if(_part1 < other._part1) return -1;
		if(_part1 > other._part1) return  1;
		if(_part2 < other._part2) return -1;
		if(_part2 > other._part2) return  1;
		if(_part3 < other._part3) return -1;
		if(_part3 > other._part3) return  1;
		if(_part4 < other._part4) return -1;
		if(_part4 > other._part4) return  1;
		return 0;
	}

	public bool StartsWith(string text)
	{
		if(text is not { Length: not 0 }) return true;
		if(text.Length > 40) return false;

		for(int i = 0; i < text.Length; ++i)
		{
			var sd = TryParseCharToHexDigit(text[i]);
			if(sd < 0) return false;
			var p = (i / 8) switch
			{
				0 => _part0,
				1 => _part1,
				2 => _part2,
				3 => _part3,
				4 => _part4,
				_ => (uint)0,
			};
			var hd = (p >> 4 * (7 - (i % 8))) & 0xf;
			if(sd != hd) return false;
		}
		return true;
	}

	public byte[] ToByteArray()
	{
#if NET5_0_OR_GREATER
		var buffer = GC.AllocateUninitializedArray<byte>(Size);
#else
		var buffer = new byte[Size];
#endif
		unsafe
		{
			fixed(byte* ptr = buffer)
			{
				*(uint*)(ptr +  0) = _part0;
				*(uint*)(ptr +  4) = _part1;
				*(uint*)(ptr +  8) = _part2;
				*(uint*)(ptr + 12) = _part3;
				*(uint*)(ptr + 16) = _part4;
			}
		}
		return buffer;
	}

	public void ToByteArray(byte[] buffer, int offset)
	{
		Verify.Argument.IsNotNull(buffer);
		Verify.Argument.IsInRange(0, offset, buffer.Length - Size);

		unsafe
		{
			fixed(byte* ptr = &buffer[offset])
			{
				*(uint*)(ptr +  0) = _part0;
				*(uint*)(ptr +  4) = _part1;
				*(uint*)(ptr +  8) = _part2;
				*(uint*)(ptr + 12) = _part3;
				*(uint*)(ptr + 16) = _part4;
			}
		}
	}

#if NETCOREAPP

	public bool TryFormat(Span<char> buffer)
	{
		var length = buffer.Length;
		unsafe
		{
			fixed(char* ptr = buffer)
			{
				if(length >= HexStringLength)
				{
					DumpPart(_part0, ptr +  0);
					DumpPart(_part1, ptr +  8);
					DumpPart(_part2, ptr + 16);
					DumpPart(_part3, ptr + 24);
					DumpPart(_part4, ptr + 32);
				}
				else
				{
					DumpPart(_part0, ptr +  0, length -  0);
					DumpPart(_part1, ptr +  8, length -  8);
					DumpPart(_part2, ptr + 16, length - 16);
					DumpPart(_part3, ptr + 24, length - 24);
					DumpPart(_part4, ptr + 32, length - 32);
				}
			}
		}
		return true;
	}

#endif

	/// <inheritdoc/>
	public override string ToString()
	{
		unsafe
		{
			var buffer = stackalloc char[HexStringLength];
			DumpPart(_part0, buffer +  0);
			DumpPart(_part1, buffer +  8);
			DumpPart(_part2, buffer + 16);
			DumpPart(_part3, buffer + 24);
			DumpPart(_part4, buffer + 32);
			return new(buffer, 0, HexStringLength);
		}
	}

	public string ToString(int length)
	{
		Verify.Argument.IsInRange(0, length, HexStringLength);

		if(length == 0) return string.Empty;
		if(length == HexStringLength) return ToString();
		unsafe
		{
			var buffer = stackalloc char[length];
			DumpPart(_part0, buffer +  0, length -  0);
			DumpPart(_part1, buffer +  8, length -  8);
			DumpPart(_part2, buffer + 16, length - 16);
			DumpPart(_part3, buffer + 24, length - 24);
			DumpPart(_part4, buffer + 32, length - 32);
			return new(buffer, 0, length);
		}
	}

	public void ToString(StringBuilder stringBuilder)
	{
		Verify.Argument.IsNotNull(stringBuilder);

		DumpPart(_part0, stringBuilder);
		DumpPart(_part1, stringBuilder);
		DumpPart(_part2, stringBuilder);
		DumpPart(_part3, stringBuilder);
		DumpPart(_part4, stringBuilder);
	}

	public void ToString(StringBuilder stringBuilder, int length)
	{
		Verify.Argument.IsNotNull(stringBuilder);
		Verify.Argument.IsInRange(0, length, HexStringLength);

		DumpPart(_part0, stringBuilder, length);
		DumpPart(_part1, stringBuilder, length -= 8);
		DumpPart(_part2, stringBuilder, length -= 8);
		DumpPart(_part3, stringBuilder, length -= 8);
		DumpPart(_part4, stringBuilder, length -= 8);
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
		return length == HexStringLength
			? ToString()
			: ToString(length);
	}

	#endregion
}
