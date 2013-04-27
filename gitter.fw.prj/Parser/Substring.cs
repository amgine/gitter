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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a substring, allowing to effectively manipulate its position
	/// without creating new <see cref="T:System.String"/> objects.
	/// </summary>
	public sealed class Substring : IEnumerable<char>
	{
		#region Static

		/// <summary>An empty substring.</summary>
		public static readonly Substring Empty = new Substring(string.Empty);

		#endregion

		#region Data

		private readonly string _string;
		private int _start;
		private int _length;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="Substring"/>.</summary>
		/// <param name="string">Base string.</param>
		/// <param name="start">Substring start.</param>
		/// <param name="end">Substring last character index.</param>
		/// <returns>Created <see cref="Substring"/>.</returns>
		public static Substring FromStartEnd(string @string, int start, int end)
		{
			return new Substring(@string, start, end - start + 1);
		}

		/// <summary>Create <see cref="Substring"/>.</summary>
		/// <param name="string">Base string.</param>
		public Substring(string @string)
		{
			Verify.Argument.IsNotNull(@string, "@string");

			_string = @string;
			_start = 0;
			_length = @string.Length;
		}

		/// <summary>Create <see cref="Substring"/>.</summary>
		/// <param name="string">Base string.</param>
		/// <param name="start">Substring start.</param>
		public Substring(string @string, int start)
		{
			Verify.Argument.IsNotNull(@string, "@string");
			Verify.Argument.IsValidIndex(start, @string.Length, "start");

			_string = @string;
			_start = start;
			_length = @string.Length - start;
		}

		/// <summary>Create <see cref="Substring"/>.</summary>
		/// <param name="string">Base string.</param>
		/// <param name="start">Substring start.</param>
		/// <param name="length">Substring length.</param>
		public Substring(string @string, int start, int length)
		{
			Verify.Argument.IsNotNull(@string, "@string");
			Verify.Argument.IsValidIndex(start, @string.Length, "start");
			Verify.Argument.IsValidIndex(length, @string.Length - start + 1, "length");

			_string = @string;
			_start = start;
			_length = length;
		}

		#endregion

		#region Properties

		/// <summary>Base string.</summary>
		public string String
		{
			get { return _string; }
		}

		/// <summary>Substring start.</summary>
		public int Start
		{
			get { return _start; }
			set
			{
				Verify.Argument.IsValidIndex(0, value, _string.Length - _length, "value");

				_start = value;
			}
		}

		/// <summary>Substring length.</summary>
		public int Length
		{
			get { return _length; }
			set
			{
				Verify.Argument.IsValidIndex(0, value, _string.Length - _start + 1, "value");

				_length = value;
			}
		}

		/// <summary>Start character.</summary>
		public char StartCharacter
		{
			get { return _string[_start]; }
		}

		/// <summary>Last character index.</summary>
		public int End
		{
			get { return _start + _length - 1; }
		}

		/// <summary>Last character.</summary>
		public char EndCharacter
		{
			get { return _string[_start + _length - 1]; }
		}

		/// <summary>Substring is empty.</summary>
		public bool IsEmpty
		{
			get { return _length == 0; }
		}

		/// <summary>Get a character from this substring.</summary>
		/// <param name="index">Character index.</param>
		/// <returns>Character at position <paramref name="index"/>.</returns>
		public char this[int index]
		{
			get
			{
				Verify.Argument.IsValidIndex(index, _length, "index");

				return _string[_start + index];
			}
		}

		#endregion

		#region Public Methods

		/// <summary>Reset substring to a new position.</summary>
		/// <param name="start">Substring start.</param>
		/// <param name="length">Substring length.</param>
		public void SetInterval(int start, int length)
		{
			Verify.Argument.IsValidIndex(start, _string.Length, "start");
			Verify.Argument.IsValidIndex(length, _string.Length - start + 1, "length");

			_start = start;
			_length = length;
		}

		/// <summary>Get substring of this <see cref="Substring"/>.</summary>
		/// <param name="start">Start index.</param>
		/// <returns>Created <see cref="Substring"/>.</returns>
		public Substring GetSubstring(int start)
		{
			Verify.Argument.IsValidIndex(_start, start, _start + _length, "start");

			if(start == 0) return this;
			int length = _length - start;
			if(length == 0) return Empty;
			return new Substring(_string, _start + start, length);
		}

		/// <summary>Get substring of this <see cref="Substring"/>.</summary>
		/// <param name="start">Start index.</param>
		/// <param name="length">Substring length.</param>
		/// <returns>Created <see cref="Substring"/>.</returns>
		public Substring GetSubstring(int start, int length)
		{
			Verify.Argument.IsValidIndex(_start, start, _length, "start");
			Verify.Argument.IsValidIndex(length, _length - start + 1, "length");

			if(length == 0) return Empty;
			if(start == 0 && length == _length) return this;
			return new Substring(_string, _start + start, length);
		}

		/// <summary>Find character <paramref name="value"/>.</summary>
		/// <param name="value">Character to find.</param>
		/// <returns>Character index or -1 if it was not found.</returns>
		public int IndexOf(char value)
		{
			if(_length == 0) return -1;
			return _string.IndexOf(value, _start, _length);
		}

		/// <summary>Find string <paramref name="value"/>.</summary>
		/// <param name="value">String to find.</param>
		/// <returns>String starting index or -1 if it was not found.</returns>
		public int IndexOf(string value)
		{
			Verify.Argument.IsNeitherNullNorEmpty(value, "value");

			if(_length < value.Length) return -1;
			return _string.IndexOf(value, _start, _length);
		}

		/// <summary>Find character <paramref name="value"/>.</summary>
		/// <param name="value">Character to find.</param>
		/// <param name="startIndex">Index to start from.</param>
		/// <returns>Character index or -1 if it was not found.</returns>
		public int IndexOf(char value, int startIndex)
		{
			Verify.Argument.IsValidIndex(startIndex, _length, "startIndex");

			if(_length == 0) return -1;
			return _string.IndexOf(value, _start + startIndex, _length - startIndex);
		}

		/// <summary>Find string <paramref name="value"/>.</summary>
		/// <param name="value">String to find.</param>
		/// <param name="startIndex">Index to start from.</param>
		/// <returns>String starting index or -1 if it was not found.</returns>
		public int IndexOf(string value, int startIndex)
		{
			Verify.Argument.IsNeitherNullNorEmpty(value, "value");
			Verify.Argument.IsValidIndex(startIndex, _length - value.Length + 1, "startIndex");

			return _string.IndexOf(value, _start + startIndex, _length - startIndex);
		}

		/// <summary>Find character <paramref name="value"/>.</summary>
		/// <param name="value">Character to find.</param>
		/// <param name="startIndex">Index to start from.</param>
		/// <param name="count">Number of characters to examine.</param>
		/// <returns>Character index or -1 if it was not found.</returns>
		public int IndexOf(char value, int startIndex, int count)
		{
			Verify.Argument.IsValidIndex(startIndex, _length, "startIndex");
			Verify.Argument.IsValidIndex(count, _length - startIndex + 1, "count");

			if(_length == 0) return -1;
			return _string.IndexOf(value, _start + startIndex, count);
		}

		/// <summary>Find string <paramref name="value"/>.</summary>
		/// <param name="value">Character to find.</param>
		/// <param name="startIndex">Index to start from.</param>
		/// <param name="count">Number of characters to examine.</param>
		/// <returns>String index or -1 if it was not found.</returns>
		public int IndexOf(string value, int startIndex, int count)
		{
			Verify.Argument.IsNeitherNullNorEmpty(value, "value");
			Verify.Argument.IsValidIndex(startIndex, _length - value.Length + 1, "startIndex");
			Verify.Argument.IsValidIndex(count, _length - startIndex + 1, "count");

			if(count < value.Length) return -1;
			return _string.IndexOf(value, _start + startIndex, count);
		}

		/// <summary>Checks if this <see cref="Substring"/> starts with <paramref name="value"/>.</summary>
		/// <param name="value">Character to look for at start.</param>
		/// <returns>True if this <see cref="Substring"/> starts with <paramref name="value"/>.</returns>
		public bool StartsWith(char value)
		{
			if(_length == 0) return false;
			return _string[_start] == value;
		}

		/// <summary>Checks if this <see cref="Substring"/> starts with <paramref name="value"/>.</summary>
		/// <param name="value">String to look for at start.</param>
		/// <returns>True if this <see cref="Substring"/> starts with <paramref name="value"/>.</returns>
		public bool StartsWith(string value)
		{
			Verify.Argument.IsNotNull(value, "value");

			if(value.Length == 0) return true;
			if(_length < value.Length) return false;
			return _string.IndexOf(value, _start, value.Length) != -1;
		}

		/// <summary>Checks if this <see cref="Substring"/> ends with <paramref name="value"/>.</summary>
		/// <param name="value">Character to look for at the end.</param>
		/// <returns>True if this <see cref="Substring"/> ends with <paramref name="value"/>.</returns>
		public bool EndsWith(char value)
		{
			if(_length == 0) return false;
			return _string[_start + _length -1] == value;
		}

		/// <summary>Checks if this <see cref="Substring"/> starts with <paramref name="value"/>.</summary>
		/// <param name="value">String to look for at start.</param>
		/// <returns>True if this <see cref="Substring"/> starts with <paramref name="value"/>.</returns>
		public bool EndsWith(string value)
		{
			Verify.Argument.IsNotNull(value, "value");

			if(value.Length == 0) return true;
			if(_length < value.Length) return false;
			return _string.IndexOf(value, _start + _length - value.Length, value.Length) != -1;
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
			var s = obj as Substring;
			if(s == null) return false;
			return _equals_non_null(this, s);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="Substring"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="Substring"/>.</returns>
		public override string ToString()
		{
			return _string.Substring(_start, _length);
		}

		#endregion

		#region Private Methods

		private static bool _equals(Substring substring1, Substring substring2)
		{
			if(substring1 == null)
			{
				if(substring2 == null) return true;
				return false;
			}
			else
			{
				if(substring2 == null) return false;
				return _equals_non_null(substring1, substring2);
			}
		}

		private static bool _equals(Substring substring1, string substring2)
		{
			if(substring1 == null)
			{
				if(substring2 == null) return true;
				return false;
			}
			else
			{
				if(substring2 == null) return false;
				return _equals_non_null(substring1, substring2);
			}
		}

		private static bool _equals_non_null(Substring substring1, Substring substring2)
		{
			if(object.ReferenceEquals(substring1, substring2))
			{
				return true;
			}
			if(substring1._length == substring2._length)
			{
				if(object.ReferenceEquals(substring1._string, substring2._string) &&
					substring1._start == substring2._start)
				{
					return true;
				}
				var end = substring1._start + substring1._length;
				for(int i = substring1._start, j = substring2._start; i < end; ++i, ++j)
				{
					if(substring1._string[i] != substring2._string[j])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static bool _equals_non_null(Substring substring1, string substring2)
		{
			if(object.ReferenceEquals(substring1, substring2))
			{
				return true;
			}
			if(substring1._length == substring2.Length)
			{
				if(object.ReferenceEquals(substring1._string, substring2) &&
					substring1._start == 0)
				{
					return true;
				}
				var end = substring1._start + substring1._length;
				for(int i = substring1._start, j = 0; i < end; ++i, ++j)
				{
					if(substring1._string[i] != substring2[j])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		#endregion

		#region Operators

		/// <summary>Convert <see cref="Substring"/> to a <see cref="T:System.String"/>.</summary>
		/// <param name="substring"><see cref="Substring"/> to convert.</param>
		/// <returns>Corresponding <see cref="T:System.String"/>.</returns>
		public static implicit operator string(Substring substring)
		{
			return substring._string.Substring(substring._start, substring._length);
		}

		public static bool operator ==(Substring substring1, Substring substring2)
		{
			return _equals(substring1, substring2);
		}

		public static bool operator !=(Substring substring1, Substring substring2)
		{
			return !_equals(substring1, substring2);
		}

		public static bool operator ==(Substring substring1, string substring2)
		{
			return _equals(substring1, substring2);
		}

		public static bool operator !=(Substring substring1, string substring2)
		{
			return !_equals(substring1, substring2);
		}

		public static bool operator ==(string substring1, Substring substring2)
		{
			return _equals(substring2, substring1);
		}

		public static bool operator !=(string substring1, Substring substring2)
		{
			return !_equals(substring2, substring1);
		}

		#endregion

		#region IEnumerable<char> Members

		private struct Enumerator : IEnumerator<char>
		{
			#region Data

			private readonly Substring _substring;
			private readonly string _string;
			private int _maxPosition;
			private int _position;
			private char _current;

			#endregion

			public Enumerator(Substring substring)
			{
				Assert.IsNotNull(substring);

				_substring = substring;
				_string = substring._string;
				_position = substring._start;
				_maxPosition = _position + substring._length;
				_current = _string[_position];
			}

			#region IEnumerator<char> Members

			char IEnumerator<char>.Current
			{
				get { return _current; }
			}

			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get { return _current; }
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				++_position;
				return _position < _maxPosition;
			}

			void System.Collections.IEnumerator.Reset()
			{
				_position = _substring._start;
				_current = _string[_position];
			}

			#endregion
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator&lt;char&gt;"/> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<char> GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion
	}
}
