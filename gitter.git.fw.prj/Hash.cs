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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.Text;

	using gitter.Framework;

	[StructLayout(LayoutKind.Sequential)]
	public struct Hash : IEquatable<Hash>, IComparable<Hash>, IComparable, IFormattable
	{
		#region Helpers

		sealed class HashEqualityComparer : IEqualityComparer<Hash>
		{
			public bool Equals(Hash x, Hash y)
			{
				return x == y;
			}

			public int GetHashCode(Hash hash)
			{
				return hash.GetHashCode();
			}
		}

		sealed class HashComparer : IComparer<Hash>
		{
			public int Compare(Hash x, Hash y)
			{
				return x.CompareTo(y);
			}
		}

		#endregion

		#region Static

		private static readonly HashEqualityComparer _equalityComparer = new HashEqualityComparer();
		private static readonly HashComparer _comparer = new HashComparer();

		public static IEqualityComparer<Hash> EqualityComparer
		{
			get { return _equalityComparer; }
		}

		public static IComparer<Hash> Comparer
		{
			get { return _comparer; }
		}

		#endregion

		#region Data

		private readonly uint _part0;
		private readonly uint _part1;
		private readonly uint _part2;
		private readonly uint _part3;
		private readonly uint _part4;

		#endregion

		#region Static

		private static int TryParseCharToHexDigit(char ch)
		{
			int digit = ch - '0';
			if(digit < 0 || digit > 9)
			{
				digit = ch - 'a';
				if(digit >= 0 && digit <= 5)
				{
					digit += 10;
				}
				else
				{
					digit = ch - 'A';
					if(digit >= 0 && digit <= 5)
					{
						digit += 10;
					}
					else
					{
						return -1;
					}
				}
			}
			return digit;
		}

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
				if(digit < 0)
				{
					return false;
				}
				part = (part << 4) + (uint)digit;
			}
			return true;
		}

		private static uint ParsePart(string hash, int offset)
		{
			uint result = 0;
			for(int i = 0; i < 8; ++i)
			{
				int digit = TryParseCharToHexDigit(hash[offset + i]);
				if(digit < 0)
				{
					throw new ArgumentException("Argument must contain hexadecimal string representation of the hash.", "hash");
				}
				result = (result << 4) + (uint)digit;
			}
			return result;
		}

		private static uint ParsePart(char[] hash, int offset)
		{
			uint result = 0;
			for(int i = 0; i < 8; ++i)
			{
				int digit = TryParseCharToHexDigit(hash[offset + i]);
				if(digit < 0)
				{
					throw new ArgumentException("Argument must contain hexadecimal string representation of the hash.", "hash");
				}
				result = (result << 4) + (uint)digit;
			}
			return result;
		}

		private static uint ParsePart(byte[] hash, int offset)
		{
			uint result = 0;
			for(int i = 0; i < 4; ++i)
			{
				result = (result << 8) + (uint)hash[offset++];
			}
			return result;
		}

		private static unsafe void DumpPart(uint part, char* buffer)
		{
			uint digit;
			digit = (part >> 28) & 0x0f;
			buffer[0] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			digit = (part >> 24) & 0x0f;
			buffer[1] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			digit = (part >> 20) & 0x0f;
			buffer[2] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			digit = (part >> 16) & 0x0f;
			buffer[3] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			digit = (part >> 12) & 0x0f;
			buffer[4] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			digit = (part >>  8) & 0x0f;
			buffer[5] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			digit = (part >>  4) & 0x0f;
			buffer[6] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			digit = (part >>  0) & 0x0f;
			buffer[7] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
		}

		private static unsafe void DumpPart(uint part, char* buffer, int length)
		{
			uint digit;
			if(length-- < 0) return;
			digit = (part >> 28) & 0x0f;
			buffer[0] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			if(length-- < 0) return;
			digit = (part >> 24) & 0x0f;
			buffer[1] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			if(length-- < 0) return;
			digit = (part >> 20) & 0x0f;
			buffer[2] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			if(length-- < 0) return;
			digit = (part >> 16) & 0x0f;
			buffer[3] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			if(length-- < 0) return;
			digit = (part >> 12) & 0x0f;
			buffer[4] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			if(length-- < 0) return;
			digit = (part >>  8) & 0x0f;
			buffer[5] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			if(length-- < 0) return;
			digit = (part >>  4) & 0x0f;
			buffer[6] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
			if(length-- < 0) return;
			digit = (part >>  0) & 0x0f;
			buffer[7] = digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit);
		}

		private static void DumpPart(uint part, StringBuilder stringBuilder)
		{
			Assert.IsNotNull(stringBuilder);

			for(int i = 28; i >= 0; i -= 4)
			{
				uint digit = (part >> i) & 0x0f;
				stringBuilder.Append(digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit));
			}
		}

		private static void DumpPart(uint part, StringBuilder stringBuilder, int length)
		{
			Assert.IsNotNull(stringBuilder);

			for(int i = 28; i >= 0 && length > 0; i -= 4, --length)
			{
				uint digit = (part >> i) & 0x0f;
				stringBuilder.Append(digit > 9 ? (char)('a' + digit - 10) : (char)('0' + digit));
			}
		}

		public static bool TryParse(string str, out Hash hash)
		{
			if(str == null || str.Length < 40)
			{
				hash = default(Hash);
				return false;
			}
			uint part0, part1, part2, part3, part4;
			if (TryParsePart(str,  0, out part0) &&
				TryParsePart(str,  8, out part1) &&
				TryParsePart(str, 16, out part2) &&
				TryParsePart(str, 24, out part3) &&
				TryParsePart(str, 32, out part4))
			{
				hash = new Hash(part0, part1, part2, part3, part4);
				return true;
			}
			hash = default(Hash);
			return false;
		}

		public static bool TryParse(string str, int offset, out Hash hash)
		{
			if(str == null || offset < 0 || str.Length - offset < 40)
			{
				hash = default(Hash);
				return false;
			}
			uint part0, part1, part2, part3, part4;
			if (TryParsePart(str,  0 + offset, out part0) &&
				TryParsePart(str,  8 + offset, out part1) &&
				TryParsePart(str, 16 + offset, out part2) &&
				TryParsePart(str, 24 + offset, out part3) &&
				TryParsePart(str, 32 + offset, out part4))
			{
				hash = new Hash(part0, part1, part2, part3, part4);
				return true;
			}
			hash = default(Hash);
			return false;
		}

		public static bool TryParse(char[] str, out Hash hash)
		{
			if(str == null || str.Length < 40)
			{
				hash = default(Hash);
				return false;
			}
			uint part0, part1, part2, part3, part4;
			if (TryParsePart(str,  0, out part0) &&
				TryParsePart(str,  8, out part1) &&
				TryParsePart(str, 16, out part2) &&
				TryParsePart(str, 24, out part3) &&
				TryParsePart(str, 32, out part4))
			{
				hash = new Hash(part0, part1, part2, part3, part4);
				return true;
			}
			hash = default(Hash);
			return false;
		}

		public static bool TryParse(char[] str, int offset, out Hash hash)
		{
			if(str == null || offset < 0 || str.Length - offset < 40)
			{
				hash = default(Hash);
				return false;
			}
			uint part0, part1, part2, part3, part4;
			if (TryParsePart(str,  0 + offset, out part0) &&
				TryParsePart(str,  8 + offset, out part1) &&
				TryParsePart(str, 16 + offset, out part2) &&
				TryParsePart(str, 24 + offset, out part3) &&
				TryParsePart(str, 32 + offset, out part4))
			{
				hash = new Hash(part0, part1, part2, part3, part4);
				return true;
			}
			hash = default(Hash);
			return false;
		}

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
			Verify.Argument.IsNotNull(hash, "hash");
			Verify.Argument.IsTrue(hash.Length >= 20, "hash", "Hash must be at least 20 bytes long.");

			_part0 = ParsePart(hash,  0);
			_part1 = ParsePart(hash,  4);
			_part2 = ParsePart(hash,  8);
			_part3 = ParsePart(hash, 12);
			_part4 = ParsePart(hash, 16);
		}

		public Hash(byte[] hash, int offset)
		{
			Verify.Argument.IsNotNull(hash, "hash");
			Verify.Argument.IsInRange(0, offset, hash.Length - 20, "hash", "Hash must be at least 20 bytes long after offset.");

			_part0 = ParsePart(hash,  0 + offset);
			_part1 = ParsePart(hash,  4 + offset);
			_part2 = ParsePart(hash,  8 + offset);
			_part3 = ParsePart(hash, 12 + offset);
			_part4 = ParsePart(hash, 16 + offset);
		}

		public Hash(string hash)
		{
			Verify.Argument.IsNotNull(hash, "hash");
			Verify.Argument.IsTrue(hash.Length >= 40, "hash", "Hash must be at least 40 characters long.");

			_part0 = ParsePart(hash,  0);
			_part1 = ParsePart(hash,  8);
			_part2 = ParsePart(hash, 16);
			_part3 = ParsePart(hash, 24);
			_part4 = ParsePart(hash, 32);
		}

		public Hash(string hash, int offset)
		{
			Verify.Argument.IsNotNull(hash, "hash");
			Verify.Argument.IsInRange(0, offset, hash.Length - 40, "hash", "Hash must be at least 40 characters long after offset.");

			_part0 = ParsePart(hash,  0 + offset);
			_part1 = ParsePart(hash,  8 + offset);
			_part2 = ParsePart(hash, 16 + offset);
			_part3 = ParsePart(hash, 24 + offset);
			_part4 = ParsePart(hash, 32 + offset);
		}

		public Hash(char[] hash)
		{
			Verify.Argument.IsNotNull(hash, "hash");
			Verify.Argument.IsTrue(hash.Length >= 40, "hash", "Hash must be at least 40 characters long.");

			_part0 = ParsePart(hash,  0);
			_part1 = ParsePart(hash,  8);
			_part2 = ParsePart(hash, 16);
			_part3 = ParsePart(hash, 24);
			_part4 = ParsePart(hash, 32);
		}

		public Hash(char[] hash, int offset)
		{
			Verify.Argument.IsNotNull(hash, "hash");
			Verify.Argument.IsInRange(0, offset, hash.Length - 40, "hash", "Hash must be at least 40 characters long after offset.");

			_part0 = ParsePart(hash,  0 + offset);
			_part1 = ParsePart(hash,  8 + offset);
			_part2 = ParsePart(hash, 16 + offset);
			_part3 = ParsePart(hash, 24 + offset);
			_part4 = ParsePart(hash, 32 + offset);
		}

		#endregion

		#region Operators

		public static explicit operator string(Hash hash)
		{
			return hash.ToString();
		}

		public static explicit operator Hash(string hash)
		{
			return new Hash(hash);
		}

		public static bool operator ==(Hash a, Hash b)
		{
			return
				a._part0 == b._part0 &&
				a._part1 == b._part1 &&
				a._part2 == b._part2 &&
				a._part3 == b._part3 &&
				a._part4 == b._part4;
		}

		public static bool operator !=(Hash a, Hash b)
		{
			return
				a._part0 != b._part0 ||
				a._part1 != b._part1 ||
				a._part2 != b._part2 ||
				a._part3 != b._part3 ||
				a._part4 != b._part4;
		}

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

		public override int GetHashCode()
		{
			return (int)(_part0 ^ _part1 ^ _part2 ^ _part3 ^ _part4);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
			{
				return false;
			}
			if(!(obj is Hash))
			{
				return false;
			}
			var other = (Hash)obj;
			return this == other;
		}

		public bool Equals(Hash other)
		{
			return this == other;
		}

		public int CompareTo(object other)
		{
			if(other == null)
			{
				return 1;
			}
			if(!(other is Hash))
			{
				throw new ArgumentException("Argument must be a Hash value.", "other");
			}
			return CompareTo((Hash)other);
		}

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

		public byte[] ToByteArray()
		{
			var buffer = new byte[20];
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
			Verify.Argument.IsNotNull(buffer, "buffer");
			Verify.Argument.IsInRange(0, offset, buffer.Length - 20, "offset");

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

		public override string ToString()
		{
			var str = Utility.FastAllocateString(40);
			unsafe
			{
				fixed(char* buffer = str)
				{
					DumpPart(_part0, buffer +  0);
					DumpPart(_part1, buffer +  8);
					DumpPart(_part2, buffer + 16);
					DumpPart(_part3, buffer + 24);
					DumpPart(_part4, buffer + 32);
				}
			}
			return str;
		}

		public string ToString(int length)
		{
			Verify.Argument.IsInRange(0, length, 40, "length");

			if(length == 0)
			{
				return string.Empty;
			}
			if(length == 40)
			{
				return ToString();
			}
			var str = Utility.FastAllocateString(length);
			unsafe
			{
				fixed(char* buffer = str)
				{
					DumpPart(_part0, buffer +  0, length -  0);
					DumpPart(_part1, buffer +  8, length -  8);
					DumpPart(_part2, buffer + 16, length - 16);
					DumpPart(_part3, buffer + 24, length - 24);
					DumpPart(_part4, buffer + 32, length - 32);
				}
			}
			return str;
		}

		public void ToString(StringBuilder stringBuilder)
		{
			Verify.Argument.IsNotNull(stringBuilder, "stringBuilder");

			DumpPart(_part0, stringBuilder);
			DumpPart(_part1, stringBuilder);
			DumpPart(_part2, stringBuilder);
			DumpPart(_part3, stringBuilder);
			DumpPart(_part4, stringBuilder);
		}

		public void ToString(StringBuilder stringBuilder, int length)
		{
			Verify.Argument.IsNotNull(stringBuilder, "stringBuilder");
			Verify.Argument.IsInRange(0, length, 40, "length");

			DumpPart(_part0, stringBuilder, length);
			DumpPart(_part1, stringBuilder, length -= 8);
			DumpPart(_part2, stringBuilder, length -= 8);
			DumpPart(_part3, stringBuilder, length -= 8);
			DumpPart(_part4, stringBuilder, length -= 8);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if(string.IsNullOrWhiteSpace(format))
			{
				return ToString();
			}
			else
			{
				int length;
				if(!int.TryParse(format, NumberStyles.Integer, CultureInfo.InvariantCulture, out length))
				{
					throw new FormatException("Unable to parse hash length.");
				}
				if(length < 0 || length > 40)
				{
					throw new FormatException("Length must be in [0; 40] range.");
				}
				if(length == 40)
				{
					return ToString();
				}
				else
				{
					return ToString(length);
				}
			}
		}

		#endregion
	}
}
