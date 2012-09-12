namespace gitter.Git
{
	using System;
	using System.Globalization;
	using System.Runtime.InteropServices;

	/// <summary>Represents SHA1 hash value.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SHA1
	{
		#region Data

		/// <summary>Bytes 0-3</summary>
		private readonly uint _value0;
		/// <summary>Bytes 4-7</summary>
		private readonly uint _value1;
		/// <summary>Bytes 8-11</summary>
		private readonly uint _value2;
		/// <summary>Bytes 12-15</summary>
		private readonly uint _value3;
		/// <summary>Bytes 16-19</summary>
		private readonly uint _value4;

		#endregion

		#region .ctor

		public SHA1(byte[] bytes)
			: this(bytes, 0)
		{
		}

		public SHA1(byte[] bytes, int offset)
		{
			Verify.Argument.IsNotNull(bytes);
			Verify.Argument.IsTrue(offset + bytes.Length == 20, "Must be 20 bytes long.");

			_value0 = BitConverter.ToUInt32(bytes, offset + 0);
			_value1 = BitConverter.ToUInt32(bytes, offset + 4);
			_value2 = BitConverter.ToUInt32(bytes, offset + 8);
			_value3 = BitConverter.ToUInt32(bytes, offset + 12);
			_value4 = BitConverter.ToUInt32(bytes, offset + 16);
		}

		public SHA1(string value)
			: this(value, 0)
		{
		}

		public SHA1(string value, int offset)
		{
			Verify.Argument.IsNotNull(value);
			Verify.Argument.IsTrue(value.Length == 40, "String is not a valid SHA1.");

			_value0 = UnhexStr(value, offset + 0);
			_value1 = UnhexStr(value, offset + 8);
			_value2 = UnhexStr(value, offset + 16);
			_value3 = UnhexStr(value, offset + 24);
			_value4 = UnhexStr(value, offset + 32);
		}

		#endregion

		#region Helpers

		private static uint UnhexStr(string value, int offset)
		{
			return uint.Parse(
				value.Substring(offset, 8),
				NumberStyles.AllowHexSpecifier,
				CultureInfo.InvariantCulture);
		}

		private static readonly char[] Alphabet = new[]
			{
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
				'a', 'b', 'c', 'd', 'e', 'f'
			};

		private static void ToHex(uint value, char[] str, int offset)
		{
			str[offset + 0] = Alphabet[(value >> 28) & 0x0f];
			str[offset + 1] = Alphabet[(value >> 24) & 0x0f];
			str[offset + 2] = Alphabet[(value >> 20) & 0x0f];
			str[offset + 3] = Alphabet[(value >> 16) & 0x0f];
			str[offset + 4] = Alphabet[(value >> 12) & 0x0f];
			str[offset + 5] = Alphabet[(value >>  8) & 0x0f];
			str[offset + 6] = Alphabet[(value >>  4) & 0x0f];
			str[offset + 7] = Alphabet[(value >>  0) & 0x0f];
		}

		#endregion

		#region Operators

		public static implicit operator SHA1(string value)
		{
			return new SHA1(value);
		}

		public static implicit operator string(SHA1 value)
		{
			return value.ToString();
		}

		public static bool operator ==(SHA1 a, SHA1 b)
		{
			return
				a._value0 == b._value0 &&
				a._value1 == b._value1 &&
				a._value2 == b._value2 &&
				a._value3 == b._value3 &&
				a._value4 == b._value4;
		}

		public static bool operator !=(SHA1 a, SHA1 b)
		{
			return
				a._value0 != b._value0 ||
				a._value1 != b._value1 ||
				a._value2 != b._value2 ||
				a._value3 != b._value3 ||
				a._value4 != b._value4;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			if(!(obj is SHA1)) return false;
			return this == (SHA1)obj;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return (int)(
				_value0.GetHashCode() ^
				_value1.GetHashCode() ^
				_value2.GetHashCode() ^
				_value3.GetHashCode() ^
				_value4.GetHashCode());
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			var result = new char[40];
			ToHex(_value0, result, 0);
			ToHex(_value1, result, 8);
			ToHex(_value2, result, 16);
			ToHex(_value3, result, 24);
			ToHex(_value4, result, 32);
			return new string(result);
		}

		#endregion
	}
}
