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

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>Parser helper object.</summary>
public class Parser
{
	#region Data

	private Stack<int>? _positions;
	private int _position;

	#endregion

	/// <summary>Create <see cref="Parser"/>.</summary>
	/// <param name="string">String to parse.</param>
	public Parser(string @string)
	{
		Verify.Argument.IsNotNull(@string);

		String = @string;
	}

	/// <summary>Parser is at the start of string.</summary>
	public bool IsAtStartOfString
		=> _position == 0;

	/// <summary>Parser is at the start of line.</summary>
	public bool IsAtStartOfLine
		=> _position == 0 || (_position < String.Length && String[_position - 1] == '\n');

	/// <summary>Parser is at the end of line.</summary>
	public bool IsAtEndOfLine
		=> _position >= String.Length || String[_position] == '\n';

	/// <summary>Parser is at the end of string.</summary>
	public bool IsAtEndOfString
		=> _position >= String.Length;

	/// <summary>Parsed string.</summary>
	public string String { get; }

	private int PositionOrLength(int pos)
		=> pos >= 0 ? pos : Length;

	private int NextPositionOrLength(int pos)
		=> pos >= 0 ? (pos + 1) : Length;

	/// <summary>Find next <paramref name="value"/>.</summary>
	/// <param name="value">Character to look for.</param>
	/// <returns>Character position or string length if it was not found.</returns>
	public int FindPositionSafe(char value)
		=> PositionOrLength(String.IndexOf(value, _position));

	/// <summary>Find next \0.</summary>
	/// <returns>Character position or string length if it was not found.</returns>
	public int FindNullOrEndOfString()
		=> PositionOrLength(String.IndexOf('\0', _position));

	/// <summary>Find next \0.</summary>
	/// <returns>Character position or string length if it was not found.</returns>
	public void FindNullAndSkip()
		=> NextPositionOrLength(String.IndexOf('\0', _position));

	/// <summary>Find next \n.</summary>
	/// <returns>Character position or string length if it was not found.</returns>
	public int FindNewLineOrEndOfString()
		=> PositionOrLength(String.IndexOf('\n', _position));

	/// <summary>Find next line ending.</summary>
	/// <param name="ending">Detected line ending.</param>
	/// <returns>Character position or string length if it was not found.</returns>
	public int FindLineEnding(out string ending)
	{
		for(int i = _position; i < String.Length; ++i)
		{
			switch(String[i])
			{
				case '\r':
					ending = i != String.Length - 1 && String[i + 1] == '\n'
						? LineEnding.CrLf
						: LineEnding.Cr;
					return i;
				case '\n':
					ending = LineEnding.Lf;
					return i;
			}
		}
		ending = string.Empty;
		return String.Length;
	}

	/// <summary>Find next \n line ending.</summary>
	/// <returns>Character position or string length if it was not found.</returns>
	public int FindLfLineEnding()
		=> PositionOrLength(String.IndexOf('\n', _position));

	/// <summary>Find next space character.</summary>
	/// <returns>Character position or string length if it was not found.</returns>
	public int FindSpace() => String.IndexOf(' ', _position);

	/// <summary>Find next space character.</summary>
	/// <returns>Character position or string length if it was not found.</returns>
	public int FindSpace(int limit) => String.IndexOf(' ', _position, limit);
	
	/// <summary>Find next , character.</summary>
	/// <returns>Character position or -1 if it was not found.</returns>
	public int FindComma() => String.IndexOf(',', _position);

	/// <summary>Find next , character.</summary>
	/// <returns>Character position or -1 if it was not found.</returns>
	public int FindComma(int limit) => String.IndexOf(',', _position, limit);

	/// <summary>Find next space character.</summary>
	public void FindSpaceAndSkip()
		=> _position = NextPositionOrLength(String.IndexOf(' ', _position));

	/// <summary>Find next <paramref name="value"/>.</summary>
	/// <param name="value">String to look for.</param>
	/// <returns>String position or string length if it was not found.</returns>
	public int FindPositionSafe(string value)
		=> PositionOrLength(String.IndexOf(value, _position));

#if NETCOREAPP

	/// <summary>Read line from current position and advance to the next line.</summary>
	/// <returns>Read line.</returns>
	public ReadOnlySpan<char> ReadLineSpan()
	{
		ReadOnlySpan<char> res;
		int pos = String.IndexOf('\n', _position);
		if(pos < 0)
		{
			res = String.AsSpan(_position);
			_position = String.Length;
		}
		else
		{
			res = String.AsSpan(_position, pos - _position);
			_position = pos + 1;
		}
		return res;
	}

#endif

	/// <summary>Read line from current position and advance to the next line.</summary>
	/// <returns>Read line.</returns>
	public string ReadLine()
	{
		string res;
		int pos = String.IndexOf('\n', _position);
		if(pos == -1)
		{
			res = String.Substring(_position);
			_position = String.Length;
		}
		else
		{
			res = String.Substring(_position, pos - _position);
			_position = pos + 1;
		}
		return res;
	}

	/// <summary>Read line from current position and advance to the next line.</summary>
	/// <param name="ending">Line ending.</param>
	/// <returns>Read line.</returns>
	public string ReadLine(out string ending)
	{
		string res;
		int pos = String.IndexOf('\n', _position);
		if(pos == -1)
		{
			res = String.Substring(_position);
			ending = "";
			_position = String.Length;
		}
		else
		{
			if(pos > 0 && String[pos - 1] == '\r')
			{
				--pos;
				ending = LineEnding.CrLf;
			}
			else
			{
				ending = LineEnding.Lf;
			}
			res = String.Substring(_position, pos - _position);
			_position = pos + ending.Length;
		}
		return res;
	}

	public string ReadLineNoAdvance()
	{
		int pos = String.IndexOf('\n', _position);
		return pos >= 0
			? String.Substring(_position, pos - _position)
			: String.Substring(_position);
	}

	public char ReadChar() => String[_position++];

	public string ReadString(int length)
	{
		var res = String.Substring(_position, length);
		_position += length;
		return res;
	}

	public string ReadStringNoAdvance(int length)
	{
		return String.Substring(_position, length);
	}

	public string ReadStringUpTo(int position)
	{
		var res = String.Substring(_position, position - _position);
		_position = position;
		return res;
	}

	public string ReadStringUpToNoAdvance(int position)
	{
		return String.Substring(_position, position - _position);
	}

	public string ReadStringUpTo(int position, int skip)
	{
		var res = String.Substring(_position, position - _position);
		_position = position + skip;
		return res;
	}

#if NETCOREAPP

	public ReadOnlySpan<char> ReadSpanUpTo(int position, int skip)
	{
		var res = String.AsSpan(_position, position - _position);
		_position = position + skip;
		return res;
	}

#endif

	public string ReadString(int length, int skip)
	{
		var res = String.Substring(_position, length);
		_position += length + skip;
		return res;
	}

	/// <summary>Skip current line.</summary>
	public void SkipLine()
		=> _position = NextPositionOrLength(String.IndexOf('\n', _position));

	/// <summary>Check if current character is <paramref name="value"/>.</summary>
	/// <param name="value">Character to check for.</param>
	/// <returns>True if current character is <paramref name="value"/>.</returns>
	public bool CheckValue(char value)
		=> _position < String.Length && String[_position] == value;

	/// <summary>Check if <paramref name="value"/> can be found at current position.</summary>
	/// <param name="value">String to check for.</param>
	/// <returns>True if current string is <paramref name="value"/>.</returns>
	public bool CheckValue(string value)
	{
		Verify.Argument.IsNotNull(value);

		var vl = value.Length;
		var sl = String.Length;
		if(_position + vl > sl) return false;
#if NETCOREAPP
		return String.AsSpan(_position, vl).SequenceEqual(value);
#else
		return String.IndexOf(value, _position, vl) != -1;
#endif
	}

	/// <summary>Check if <paramref name="value"/> can be found at current position and skips value if it is found.</summary>
	/// <param name="value">String to check for.</param>
	/// <returns>True if current string is <paramref name="value"/>.</returns>
	public bool CheckValueAndSkip(string value)
	{
		Verify.Argument.IsNotNull(value);

		var vl = value.Length;
		var sl = String.Length;
		if(_position + vl > sl) return false;
#if NETCOREAPP
		if(String.AsSpan(_position, vl).SequenceEqual(value))
#else
		if(String.IndexOf(value, _position, vl) != -1)
#endif
		{
			Skip(vl);
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>Set position to the start of <paramref name="value"/>.</summary>
	/// <param name="value">String to look for.</param>
	public void Find(string value)
	{
		Verify.Argument.IsNotNull(value);

		var vl = value.Length;
		var sl = String.Length;
		if(_position + vl > sl)
		{
			_position = sl;
		}
		else
		{
			int pos = String.IndexOf(value, _position);
			_position = pos == -1 ? sl : pos;
		}
	}

	/// <summary>Looks for a line which starts with <paramref name="start"/>.</summary>
	/// <param name="start">Desired line start.</param>
	public void FindStartOfLine(string start)
	{
		if(!CheckValue(start))
		{
			Find("\n" + start);
			Skip();
		}
	}

	/// <summary>Looks for a line which starts with <paramref name="start"/>.</summary>
	/// <param name="start">Desired line start.</param>
	public int FindStartOfLineNoAdvance(string start)
	{
		if(!CheckValue(start))
		{
			int pos = FindNoAdvance("\n" + start);
			if(pos == -1) return -1;
			return pos;
		}
		return _position;
	}

	/// <summary>Looks for a line which starts with <paramref name="start"/>.</summary>
	/// <param name="start">Desired line start.</param>
	/// <param name="limit">Number of characters to examine.</param>
	public int FindStartOfLineNoAdvance(string start, int limit)
	{
		if(!CheckValue(start))
		{
			int pos = FindNoAdvance("\n" + start, limit);
			if(pos == -1) return -1;
			return pos;
		}
		return _position;
	}

	/// <summary>Find next <paramref name="value"/> but do not change position.</summary>
	/// <param name="value">String to look for.</param>
	/// <returns>Substring.</returns>
	public Substring? FindSubstring(string value)
	{
		Verify.Argument.IsNotNull(value);

		var vl = value.Length;
		var sl = String.Length;
		if(_position + vl > sl)
		{
			return null;
		}
		else
		{
			int pos = String.IndexOf(value, _position);
			if(pos == -1)
			{
				return null;
			}
			else
			{
				return new Substring(String, pos, value.Length);
			}
		}
	}

	/// <summary>Find next substring <paramref name="value"/> and skip it.</summary>
	/// <param name="value">String to look for.</param>
	public void FindAndSkip(string value)
	{
		Verify.Argument.IsNotNull(value);

		var vl = value.Length;
		var sl = String.Length;
		if(_position + vl > sl)
		{
			_position = sl;
		}
		else
		{
			int pos = String.IndexOf(value, _position);
			if(pos == -1)
			{
				_position = sl;
			}
			else
			{
				_position = pos + vl;
			}
		}
	}

	public int FindNoAdvance(string value)
		=> String.IndexOf(value, _position);

	public int FindNoAdvance(string value, int limit)
		=> String.IndexOf(value, _position, limit);

	public int FindNoAdvance(char value)
		=> String.IndexOf(value, _position);

	public int FindNoAdvance(char value, int limit)
		=> String.IndexOf(value, _position, limit);

	public int FindSeparatingEmptyLine(int limit, out int part2Start)
	{
		bool prevN = false;
		bool prevR = false;
		int lines = 0;
		for(int i = _position; i < limit; ++i)
		{
			var c = String[i];
			switch(c)
			{
				case '\r':
					if(prevN)
					{
						prevN = false;
					}
					else
					{
						lines = 0;
					}
					prevR = true;
					break;
				case '\n':
					if(prevN)
					{
						part2Start = i + 1;
						return i - 1;
					}
					else
					{
						if(prevR)
						{
							++lines;
							if(lines == 2)
							{
								part2Start = i + 1;
								return i - 3;
							}
							prevR = false;
						}
						prevN = true;
					}
					break;
				default:
					lines = 0;
					prevN = false;
					prevR = false;
					break;
			}
		}
		part2Start = -1;
		return -1;
	}

	/// <summary>Current parser position.</summary>
	public int Position
	{
		get => _position;
		set
		{
			Verify.Argument.IsNotNegative(value);

			if(value > String.Length)
			{
				_position = String.Length;
			}
			else
			{
				_position = value;
			}
		}
	}

	public int RemainingSymbols => String.Length - _position;

	public int Length => String.Length;

	public char CurrentChar => String[_position];

	public char this[int index] => String[_position + index];

	public void Skip() => ++_position;

	public void Skip(int amount) => _position += amount;

	public int Skip(char value)
	{
		var position = _position;
		while(_position < String.Length && String[_position] == value)
		{
			++_position;
		}
		return _position - position;
	}

	public void GoToStart()
	{
		_position = 0;
	}

	public void GoToEnd()
	{
		_position = String.Length;
	}

	public void PushPosition()
	{
		_positions ??= new Stack<int>();
		_positions.Push(_position);
	}

	public void PushPosition(int newPosition)
	{
		PushPosition();
		_position = newPosition;
	}

	public void PopPosition()
	{
		Verify.State.IsTrue(_positions is { Count: not 0 });

		_position = _positions!.Pop();
	}

	public unsafe Version ReadVersion()
	{
		while(!IsAtEndOfLine && !char.IsDigit(CurrentChar))
		{
			Skip();
		}
		PushPosition();
		Skip();
		int parts = 0;
		var values = stackalloc int[4];
		while(!IsAtEndOfLine && parts < 4)
		{
			if(CheckValue('.'))
			{
				var pos = Position;
				PopPosition();
				if(!int.TryParse(ReadStringUpTo(pos, 1), NumberStyles.None, CultureInfo.InvariantCulture, out values[parts])) break;
				++parts;
				PushPosition();
			}
			else
			{
				if(!char.IsNumber(CurrentChar))
				{
					var pos = Position;
					PopPosition();
					if(pos != Position)
					{
						if(!int.TryParse(ReadStringUpTo(pos, 0), NumberStyles.None, CultureInfo.InvariantCulture, out values[parts])) break;
						++parts;
					}
					break;
				}
				else
				{
					Skip();
				}
			}
		}
		return parts switch
		{
			2 => new Version(values[0], values[1]),
			3 => new Version(values[0], values[1], values[2]),
			4 => new Version(values[0], values[1], values[2], values[3]),
			_ => throw new Exception("Unable to read version."),
		};
	}

	/// <inheritdoc/>
	public override string ToString() => String;
}
