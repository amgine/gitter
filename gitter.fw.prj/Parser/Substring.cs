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

namespace gitter.Framework;

#nullable enable

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a substring, allowing to effectively manipulate its position
/// without creating new <see cref="T:System.String"/> objects.
/// </summary>
public sealed class Substring : IEnumerable<char>, IEquatable<Substring>
{
	#region Static

	/// <summary>An empty substring.</summary>
	public static readonly Substring Empty = new(string.Empty);

	#endregion

	#region .ctor

	private string? _cachedString;

	/// <summary>Create <see cref="Substring"/>.</summary>
	/// <param name="string">Base string.</param>
	/// <param name="start">Substring start.</param>
	/// <param name="end">Substring last character index.</param>
	/// <returns>Created <see cref="Substring"/>.</returns>
	public static Substring FromStartEnd(string @string, int start, int end)
		=> new(@string, start, end - start + 1);

	/// <summary>Create <see cref="Substring"/>.</summary>
	/// <param name="string">Base string.</param>
	public Substring(string @string)
	{
		Verify.Argument.IsNotNull(@string, nameof(@string));

		String = @string;
		Start  = 0;
		Length = @string.Length;
	}

	/// <summary>Create <see cref="Substring"/>.</summary>
	/// <param name="string">Base string.</param>
	/// <param name="start">Substring start.</param>
	public Substring(string @string, int start)
	{
		Verify.Argument.IsNotNull(@string);
		Verify.Argument.IsValidIndex(start, @string.Length);

		String = @string;
		Start  = start;
		Length = @string.Length - start;
	}

	/// <summary>Create <see cref="Substring"/>.</summary>
	/// <param name="string">Base string.</param>
	/// <param name="start">Substring start.</param>
	/// <param name="length">Substring length.</param>
	public Substring(string @string, int start, int length)
	{
		Verify.Argument.IsNotNull(@string);
		Verify.Argument.IsValidIndex(start, @string.Length);
		Verify.Argument.IsValidIndex(length, @string.Length - start + 1);

		String = @string;
		Start  = start;
		Length = length;
	}

	#endregion

	#region Properties

	/// <summary>Base string.</summary>
	public string String { get; }

	/// <summary>Substring start.</summary>
	public int Start { get; }

	/// <summary>Substring length.</summary>
	public int Length { get; }

	/// <summary>First character.</summary>
	public char FirstCharacter => String[Start];

	/// <summary>Last character index.</summary>
	public int End => Start + Length - 1;

	/// <summary>Last character.</summary>
	public char LastCharacter => String[Start + Length - 1];

	/// <summary>Substring is empty.</summary>
	public bool IsEmpty => Length == 0;

	/// <summary>Get a character from this substring.</summary>
	/// <param name="index">Character index.</param>
	/// <returns>Character at position <paramref name="index"/>.</returns>
	public char this[int index] => String[Start + index];

	#endregion

	#region Public Methods

	/// <summary>Get substring of this <see cref="Substring"/>.</summary>
	/// <param name="start">Start index.</param>
	/// <returns>Created <see cref="Substring"/>.</returns>
	public Substring GetSubstring(int start)
	{
		Verify.Argument.IsValidIndex(Start, start, Start + Length, nameof(start));

		if(start == 0) return this;
		int length = Length - start;
		if(length == 0) return Empty;
		return new Substring(String, Start + start, length);
	}

	/// <summary>Get substring of this <see cref="Substring"/>.</summary>
	/// <param name="start">Start index.</param>
	/// <param name="length">Substring length.</param>
	/// <returns>Created <see cref="Substring"/>.</returns>
	public Substring GetSubstring(int start, int length)
	{
		Verify.Argument.IsValidIndex(Start, start, Length, nameof(start));
		Verify.Argument.IsValidIndex(length, Length - start + 1, nameof(length));

		if(length == 0) return Empty;
		if(start == 0 && length == Length) return this;
		return new Substring(String, Start + start, length);
	}

	/// <summary>Find character <paramref name="value"/>.</summary>
	/// <param name="value">Character to find.</param>
	/// <returns>Character index or -1 if it was not found.</returns>
	public int IndexOf(char value)
	{
		if(Length == 0) return -1;
		return String.IndexOf(value, Start, Length);
	}

	/// <summary>Find string <paramref name="value"/>.</summary>
	/// <param name="value">String to find.</param>
	/// <returns>String starting index or -1 if it was not found.</returns>
	public int IndexOf(string value)
	{
		Verify.Argument.IsNeitherNullNorEmpty(value);

		if(Length < value.Length) return -1;
		return String.IndexOf(value, Start, Length);
	}

	/// <summary>Find character <paramref name="value"/>.</summary>
	/// <param name="value">Character to find.</param>
	/// <param name="startIndex">Index to start from.</param>
	/// <returns>Character index or -1 if it was not found.</returns>
	public int IndexOf(char value, int startIndex)
	{
		Verify.Argument.IsValidIndex(startIndex, Length, nameof(startIndex));

		if(Length == 0) return -1;
		return String.IndexOf(value, Start + startIndex, Length - startIndex);
	}

	/// <summary>Find string <paramref name="value"/>.</summary>
	/// <param name="value">String to find.</param>
	/// <param name="startIndex">Index to start from.</param>
	/// <returns>String starting index or -1 if it was not found.</returns>
	public int IndexOf(string value, int startIndex)
	{
		Verify.Argument.IsNeitherNullNorEmpty(value);
		Verify.Argument.IsValidIndex(startIndex, Length - value.Length + 1, nameof(startIndex));

		return String.IndexOf(value, Start + startIndex, Length - startIndex);
	}

	/// <summary>Find character <paramref name="value"/>.</summary>
	/// <param name="value">Character to find.</param>
	/// <param name="startIndex">Index to start from.</param>
	/// <param name="count">Number of characters to examine.</param>
	/// <returns>Character index or -1 if it was not found.</returns>
	public int IndexOf(char value, int startIndex, int count)
	{
		Verify.Argument.IsValidIndex(startIndex, Length, nameof(startIndex));
		Verify.Argument.IsValidIndex(count, Length - startIndex + 1, nameof(count));

		if(Length == 0) return -1;
		return String.IndexOf(value, Start + startIndex, count);
	}

	/// <summary>Find string <paramref name="value"/>.</summary>
	/// <param name="value">Character to find.</param>
	/// <param name="startIndex">Index to start from.</param>
	/// <param name="count">Number of characters to examine.</param>
	/// <returns>String index or -1 if it was not found.</returns>
	public int IndexOf(string value, int startIndex, int count)
	{
		Verify.Argument.IsNeitherNullNorEmpty(value);
		Verify.Argument.IsValidIndex(startIndex, Length - value.Length + 1, nameof(startIndex));
		Verify.Argument.IsValidIndex(count, Length - startIndex + 1, nameof(count));

		if(count < value.Length) return -1;
		return String.IndexOf(value, Start + startIndex, count);
	}

	/// <summary>Checks if this <see cref="Substring"/> starts with <paramref name="value"/>.</summary>
	/// <param name="value">Character to look for at start.</param>
	/// <returns>True if this <see cref="Substring"/> starts with <paramref name="value"/>.</returns>
	public bool StartsWith(char value)
	{
		if(Length == 0) return false;
		return String[Start] == value;
	}

	/// <summary>Checks if this <see cref="Substring"/> starts with <paramref name="value"/>.</summary>
	/// <param name="value">String to look for at start.</param>
	/// <returns>True if this <see cref="Substring"/> starts with <paramref name="value"/>.</returns>
	public bool StartsWith(string value)
	{
		Verify.Argument.IsNotNull(value);

		if(value.Length == 0) return true;
		if(Length < value.Length) return false;
		return String.IndexOf(value, Start, value.Length) != -1;
	}

	/// <summary>Checks if this <see cref="Substring"/> ends with <paramref name="value"/>.</summary>
	/// <param name="value">Character to look for at the end.</param>
	/// <returns>True if this <see cref="Substring"/> ends with <paramref name="value"/>.</returns>
	public bool EndsWith(char value)
	{
		if(Length == 0) return false;
		return String[Start + Length -1] == value;
	}

	/// <summary>Checks if this <see cref="Substring"/> starts with <paramref name="value"/>.</summary>
	/// <param name="value">String to look for at start.</param>
	/// <returns>True if this <see cref="Substring"/> starts with <paramref name="value"/>.</returns>
	public bool EndsWith(string value)
	{
		Verify.Argument.IsNotNull(value);

		if(value.Length == 0) return true;
		if(Length < value.Length) return false;
		return String.IndexOf(value, Start + Length - value.Length, value.Length) != -1;
	}

	/// <inheritdoc/>
	public bool Equals(Substring? other)
		=> _equals(this, other);

	public string AsString()
		=> _cachedString ??= String.Substring(Start, Length);

	public string AsString(int offset)
		=> String.Substring(Start + offset, Length - offset);

	public string AsString(int offset, int length)
		=> String.Substring(Start + offset, length);

	#if NETCOREAPP

	public ReadOnlySpan<char> AsSpan()
		=> String.AsSpan(Start, Length);

	public ReadOnlySpan<char> AsSpan(int offset)
		=> String.AsSpan(Start + offset, Length - offset);

	public ReadOnlySpan<char> AsSpan(int offset, int length)
		=> String.AsSpan(Start + offset, length);

	#endif

	#endregion

	#region Overrides

	/// <inheritdoc/>
	public override bool Equals(object? obj)
		=> obj is Substring other && _equals_non_null(this, other);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = 0;
		hash ^= Start;
		hash ^= Length * 37;
		for(int i = Start; i < Length; ++i)
		{
			hash ^= (int)String[i];
		}
		return hash;
	}

	/// <inheritdoc/>
	public override string ToString()
		=> AsString();

	#endregion

	#region Private Methods

	private static bool _equals(Substring? substring1, Substring? substring2)
	{
		if(substring1 is null) return substring2 is null;
		if(substring2 is null) return false;
		return _equals_non_null(substring1, substring2);
	}

	private static bool _equals(Substring? substring1, string? substring2)
	{
		if(substring1 is null) return substring2 is null;
		if(substring2 is null) return false;
		return _equals_non_null(substring1, substring2);
	}

	private static bool _equals_non_null(Substring substring1, Substring substring2)
	{
		if(ReferenceEquals(substring1, substring2))
		{
			return true;
		}
		if(substring1.Length == substring2.Length)
		{
			if(ReferenceEquals(substring1.String, substring2.String) &&
				substring1.Start == substring2.Start)
			{
				return true;
			}
			#if NETCOREAPP
			return substring1.AsSpan().SequenceEqual(substring2.AsSpan());
			#else
			var end = substring1.Start + substring1.Length;
			for(int i = substring1.Start, j = substring2.Start; i < end; ++i, ++j)
			{
				if(substring1.String[i] != substring2.String[j])
				{
					return false;
				}
			}
			return true;
			#endif
		}
		return false;
	}

	private static bool _equals_non_null(Substring substring1, string substring2)
	{
		if(ReferenceEquals(substring1, substring2))
		{
			return true;
		}
		if(substring1.Length == substring2.Length)
		{
			if(ReferenceEquals(substring1.String, substring2) &&
				substring1.Start == 0)
			{
				return true;
			}
			#if NETCOREAPP
			return substring1.AsSpan().SequenceEqual(substring2.AsSpan());
			#else
			var end = substring1.Start + substring1.Length;
			for(int i = substring1.Start, j = 0; i < end; ++i, ++j)
			{
				if(substring1.String[i] != substring2[j])
				{
					return false;
				}
			}
			return true;
			#endif
		}
		return false;
	}

	#endregion

	#region Operators

	/// <summary>Convert <see cref="Substring"/> to a <see cref="T:System.String"/>.</summary>
	/// <param name="substring"><see cref="Substring"/> to convert.</param>
	/// <returns>Corresponding <see cref="T:System.String"/>.</returns>
	public static implicit operator string(Substring substring)
		=> substring.AsString();

#if NETCOREAPP

	public static implicit operator ReadOnlySpan<char>(Substring substring)
		=> substring.AsSpan();

#endif

	public static bool operator ==(Substring? substring1, Substring? substring2)
		=> _equals(substring1, substring2);

	public static bool operator !=(Substring? substring1, Substring? substring2)
		=> !_equals(substring1, substring2);

	public static bool operator ==(Substring? substring1, string? substring2)
		=> _equals(substring1, substring2);

	public static bool operator !=(Substring? substring1, string? substring2)
		=> !_equals(substring1, substring2);

	public static bool operator ==(string? substring1, Substring? substring2)
		=> _equals(substring2, substring1);

	public static bool operator !=(string? substring1, Substring? substring2)
		=> !_equals(substring2, substring1);

	#endregion

	#region IEnumerable<char> Members

	public struct Enumerator : IEnumerator<char>
	{
		private readonly Substring _substring;
		private readonly string _string;
		private readonly int _maxPosition;
		private int _position;

		internal Enumerator(Substring substring)
		{
			Assert.IsNotNull(substring);

			_substring   = substring;
			_string      = substring.String;
			_position    = substring.Start;
			_maxPosition = _position + substring.Length;
		}

		public readonly char Current => _string[_position];

		readonly object System.Collections.IEnumerator.Current => Current;

		public bool MoveNext()
			=> ++_position < _maxPosition;

		public void Reset()
			=> _position = _substring.Start;

		public readonly void Dispose() { }
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>
	/// An <see cref="T:System.Collections.IEnumerator{char}"/> object that can be used to iterate through the collection.
	/// </returns>
	public Enumerator GetEnumerator() => new(this);

	/// <inheritdoc/>
	IEnumerator<char> IEnumerable<char>.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

	#endregion
}
