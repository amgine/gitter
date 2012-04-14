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
			if(@string == null) throw new ArgumentNullException("string");

			_string = @string;
			_start = 0;
			_length = @string.Length;
		}

		/// <summary>Create <see cref="Substring"/>.</summary>
		/// <param name="string">Base string.</param>
		/// <param name="start">Substring start.</param>
		public Substring(string @string, int start)
		{
			if(@string == null) throw new ArgumentNullException("string");
			if(start < 0 || start >= @string.Length) throw new ArgumentOutOfRangeException("start");

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
			if(@string == null) throw new ArgumentNullException("string");
			if(start < 0 || start >= @string.Length) throw new ArgumentOutOfRangeException("start");
			if(length < 0 || start + length > @string.Length) throw new ArgumentOutOfRangeException("length");

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
				if(value != _start)
				{
					if(value < 0 || value >= _string.Length) throw new ArgumentOutOfRangeException("start");
					if(value + _length > _string.Length) throw new ArgumentOutOfRangeException("start");
					_start = value;
				}
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

		/// <summary>Substring length.</summary>
		public int Length
		{
			get { return _length; }
			set
			{
				if(value != _length)
				{
					if(value < 0 || _start + value > _string.Length) throw new ArgumentOutOfRangeException("length");
					_length = value;
				}
			}
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
				if(index < 0 || index >= _length) throw new ArgumentOutOfRangeException("index");
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
			if(start < 0 || start >= _string.Length) throw new ArgumentOutOfRangeException("start");
			if(length < 0 || start + length > _string.Length) throw new ArgumentOutOfRangeException("length");
			_start = start;
			_length = length;
		}

		/// <summary>Get substring of this <see cref="Substring"/>.</summary>
		/// <param name="start">Start index.</param>
		/// <returns>Created <see cref="Substring"/>.</returns>
		public Substring GetSubstring(int start)
		{
			if(start < 0 || start >= _start + _length) throw new IndexOutOfRangeException("start");
			int length = _length - start;
			if(length == 0) return Empty;
			if(start == 0) return this;
			return new Substring(_string, _start + start, length);
		}

		/// <summary>Get substring of this <see cref="Substring"/>.</summary>
		/// <param name="start">Start index.</param>
		/// <param name="length">Substring length.</param>
		/// <returns>Created <see cref="Substring"/>.</returns>
		public Substring GetSubstring(int start, int length)
		{
			if(start < 0 || start >= _start + _length) throw new IndexOutOfRangeException("start");
			if(length < 0 || start + length > length) throw new IndexOutOfRangeException("length");
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
			if(value == null) throw new ArgumentNullException("value");
			if(value.Length == 0) throw new ArgumentException("value");
			if(_length < value.Length) return -1;
			return _string.IndexOf(value, _start, _length);
		}

		/// <summary>Find character <paramref name="value"/>.</summary>
		/// <param name="value">Character to find.</param>
		/// <param name="startIndex">Index to start from.</param>
		/// <returns>Character index or -1 if it was not found.</returns>
		public int IndexOf(char value, int startIndex)
		{
			if(startIndex < 0 || startIndex >= _length) throw new ArgumentOutOfRangeException("startIndex");
			if(_length == 0) return -1;
			return _string.IndexOf(value, _start + startIndex, _length - startIndex);
		}

		/// <summary>Find string <paramref name="value"/>.</summary>
		/// <param name="value">String to find.</param>
		/// <param name="startIndex">Index to start from.</param>
		/// <returns>String starting index or -1 if it was not found.</returns>
		public int IndexOf(string value, int startIndex)
		{
			if(value == null) throw new ArgumentNullException("value");
			if(value.Length == 0) throw new ArgumentException("value");
			if(startIndex < 0 || startIndex + value.Length > _length) throw new ArgumentOutOfRangeException("startIndex");
			return _string.IndexOf(value, _start + startIndex, _length - startIndex);
		}

		/// <summary>Find character <paramref name="value"/>.</summary>
		/// <param name="value">Character to find.</param>
		/// <param name="startIndex">Index to start from.</param>
		/// <param name="count">Number of characters to examine.</param>
		/// <returns>Character index or -1 if it was not found.</returns>
		public int IndexOf(char value, int startIndex, int count)
		{
			if(startIndex < 0 || startIndex >= _length) throw new ArgumentOutOfRangeException("startIndex");
			if(count < 0 || startIndex + count > _length) throw new ArgumentOutOfRangeException("count");
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
			if(startIndex < 0 || startIndex >= _length) throw new ArgumentOutOfRangeException("startIndex");
			if(count < 0 || startIndex + count > _length) throw new ArgumentOutOfRangeException("count");
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
			if(value == null) throw new ArgumentNullException();
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
			if(value == null) throw new ArgumentNullException();
			if(value.Length == 0) return true;
			if(_length < value.Length) return false;
			return _string.IndexOf(value, _start + _length - value.Length, value.Length) != -1;
		}

		#endregion

		#region Overrides

		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var s = obj as Substring;
			if(s == null) return false;
			return _equals_non_null(this, s);
		}

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
				if(substring2 == null)
					return false;
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
				if(substring2 == null)
					return false;
				return _equals_non_null(substring1, substring2);
			}
		}

		private static bool _equals_non_null(Substring substring1, Substring substring2)
		{
			if(object.ReferenceEquals(substring1, substring2))
				return true;
			if(substring1._length == substring2._length)
			{
				if(object.ReferenceEquals(substring1._string, substring2._string) &&
					substring1._start == substring2._start)
					return true;
				var end = substring1._start + substring1._length;
				for(int i = substring1._start, j = substring2._start; i < end; ++i, ++j)
				{
					if(substring1._string[i] != substring2._string[j])
						return false;
				}
				return true;
			}
			return false;
		}

		private static bool _equals_non_null(Substring substring1, string substring2)
		{
			if(object.ReferenceEquals(substring1, substring2))
				return true;
			if(substring1._length == substring2.Length)
			{
				if(object.ReferenceEquals(substring1._string, substring2) &&
					substring1._start == 0)
					return true;
				var end = substring1._start + substring1._length;
				for(int i = substring1._start, j = 0; i < end; ++i, ++j)
				{
					if(substring1._string[i] != substring2[j])
						return false;
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
