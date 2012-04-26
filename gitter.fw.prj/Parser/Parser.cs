namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Globalization;

	/// <summary>Parser helper object.</summary>
	public class Parser
	{
		#region Data

		private readonly string _string;
		private Stack<int> _positions;
		private int _position;

		#endregion

		/// <summary>Create <see cref="Parser"/>.</summary>
		/// <param name="string">String to parse.</param>
		public Parser(string @string)
		{
			if(@string == null) throw new ArgumentNullException("string");

			_string = @string;
		}

		/// <summary>Parser is at the start of string.</summary>
		public bool IsAtStartOfString
		{
			get { return _position == 0; }
		}

		/// <summary>Parser is at the start of line.</summary>
		public bool IsAtStartOfLine
		{
			get { return _position == 0 || (_position < _string.Length && _string[_position - 1] == '\n'); }
		}

		/// <summary>Parser is at the end of line.</summary>
		public bool IsAtEndOfLine
		{
			get { return _position >= _string.Length || _string[_position] == '\n'; }
		}

		/// <summary>Parser is at the end of string.</summary>
		public bool IsAtEndOfString
		{
			get { return _position >= _string.Length; }
		}

		/// <summary>Parsed string.</summary>
		public string String
		{
			get { return _string; }
		}

		/// <summary>Find next <see cref="value"/>.</summary>
		/// <param name="value">Character to look for.</param>
		/// <returns>Character position or string length if it was not found.</returns>
		public int FindPositionSafe(char value)
		{
			int pos = _string.IndexOf(value, _position);
			if(pos == -1) return Length;
			return pos;
		}

		/// <summary>Find next \0.</summary>
		/// <returns>Character position or string length if it was not found.</returns>
		public int FindNullOrEndOfString()
		{
			int pos = _string.IndexOf('\0', _position);
			if(pos == -1) return Length;
			return pos;
		}

		/// <summary>Find next \0.</summary>
		/// <returns>Character position or string length if it was not found.</returns>
		public void FindNullAndSkip()
		{
			int pos = _string.IndexOf('\0', _position);
			if(pos == -1)
				_position = _string.Length;
			else
				_position = pos + 1;
		}

		/// <summary>Find next \n.</summary>
		/// <returns>Character position or string length if it was not found.</returns>
		public int FindNewLineOrEndOfString()
		{
			int pos = _string.IndexOf('\n', _position);
			if(pos == -1) return _string.Length;
			return pos;
		}

		/// <summary>Find next line ending.</summary>
		/// <param name="ending">Detected line ending.</param>
		/// <returns>Character position or string length if it was not found.</returns>
		public int FindLineEnding(out string ending)
		{
			for(int i = _position; i < _string.Length; ++i)
			{
				switch(_string[i])
				{
					case '\r':
						if(i != _string.Length - 1 && _string[i + 1] == '\n')
						{
							ending = LineEnding.CrLf;
						}
						else
						{
							ending = LineEnding.Cr;
						}
						return i;
					case '\n':
						ending = LineEnding.Lf;
						return i;
				}
			}
			ending = string.Empty;
			return _string.Length;
		}

		/// <summary>Find next \n line ending.</summary>
		/// <returns>Character position or string length if it was not found.</returns>
		public int FindLfLineEnding()
		{
			for(int i = _position; i < _string.Length; ++i)
			{
				if(_string[i] == '\n') return i;
			}
			return _string.Length;
		}

		/// <summary>Find next space character.</summary>
		/// <returns>Character position or string length if it was not found.</returns>
		public int FindSpace()
		{
			return _string.IndexOf(' ', _position);
		}

		/// <summary>Find next space character.</summary>
		/// <returns>Character position or string length if it was not found.</returns>
		public int FindSpace(int limit)
		{
			return _string.IndexOf(' ', _position, limit);
		}

		/// <summary>Find next , character.</summary>
		/// <returns>Character position or -1 if it was not found.</returns>
		public int FindComma()
		{
			return _string.IndexOf(',', _position);
		}

		/// <summary>Find next , character.</summary>
		/// <returns>Character position or -1 if it was not found.</returns>
		public int FindComma(int limit)
		{
			return _string.IndexOf(',', _position, limit);
		}

		/// <summary>Find next space character.</summary>
		public void FindSpaceAndSkip()
		{
			int pos = _string.IndexOf(' ', _position);
			if(pos == -1)
				_position = _string.Length;
			else
				_position = pos + 1;
		}

		/// <summary>Find next <see cref="value"/>.</summary>
		/// <param name="value">String to look for.</param>
		/// <returns>String position or string length if it was not found.</returns>
		public int FindPositionSafe(string value)
		{
			int pos = _string.IndexOf(value, _position);
			if(pos == -1) return _string.Length;
			return pos;
		}

		/// <summary>Read line from current position and advance to the next line.</summary>
		/// <returns>Read line.</returns>
		public string ReadLine()
		{
			string res;
			int pos = _string.IndexOf('\n', _position);
			if(pos == -1)
			{
				res = _string.Substring(_position);
				_position = _string.Length;
			}
			else
			{
				res = _string.Substring(_position, pos - _position);
				_position = pos + 1;
			}
			return res;
		}

		public string ReadLineNoAdvance()
		{
			int pos = _string.IndexOf('\n', _position);
			if(pos == -1)
			{
				return _string.Substring(_position);
			}
			else
			{
				return _string.Substring(_position, pos - _position);
			}
		}

		public char ReadChar()
		{
			return _string[_position++];
		}

		public string ReadString(int length)
		{
			var res = _string.Substring(_position, length);
			_position += length;
			return res;
		}

		public string ReadStringNoAdvance(int length)
		{
			return _string.Substring(_position, length);
		}

		public string ReadStringUpTo(int position)
		{
			var res = _string.Substring(_position, position - _position);
			_position = position;
			return res;
		}

		public string ReadStringUpToNoAdvance(int position)
		{
			return _string.Substring(_position, position - _position);
		}

		public string ReadStringUpTo(int position, int skip)
		{
			var res = _string.Substring(_position, position - _position);
			_position = position + skip;
			return res;
		}

		public string ReadString(int length, int skip)
		{
			var res = _string.Substring(_position, length);
			_position += length + skip;
			return res;
		}

		/// <summary>Skip current line.</summary>
		public void SkipLine()
		{
			int pos = _string.IndexOf('\n', _position);
			if(pos == -1)
				_position = _string.Length;
			else
				_position = pos + 1;
		}

		/// <summary>Check if current character is <paramref name="value"/>.</summary>
		/// <param name="value">Character to check for.</param>
		/// <returns>True if current character is <see cref="value"/>.</returns>
		public bool CheckValue(char value)
		{
			return _position < _string.Length && _string[_position] == value;
		}

		/// <summary>Check if <paramref name="value"/> can be found at currect position.</summary>
		/// <param name="value">String to check for.</param>
		/// <returns>True if current string is <see cref="value"/>.</returns>
		public bool CheckValue(string value)
		{
			if(value == null) throw new ArgumentNullException("value");
			var vl = value.Length;
			var sl = _string.Length;
			if(_position + vl > sl) return false;
			return _string.IndexOf(value, _position, vl) != -1;
		}

		/// <summary>Set position to the start of <paramref name="value"/>.</summary>
		/// <param name="value">String to look for.</param>
		public void Find(string value)
		{
			if(value == null) throw new ArgumentNullException("value");
			var vl = value.Length;
			var sl = _string.Length;
			if(_position + vl > sl)
				_position = sl;
			else
			{
				int pos = _string.IndexOf(value, _position);
				if(pos == -1)
					_position = sl;
				else
					_position = pos;
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
		public Substring FindSubstring(string value)
		{
			if(value == null) throw new ArgumentNullException("value");
			var vl = value.Length;
			var sl = _string.Length;
			if(_position + vl > sl)
				return null;
			else
			{
				int pos = _string.IndexOf(value, _position);
				if(pos == -1)
					return null;
				else
					return new Substring(_string, pos, value.Length);
			}
		}

		/// <summary>Find next substring <paramref name="value"/> and skip it.</summary>
		/// <param name="value">String to look for.</param>
		public void FindAndSkip(string value)
		{
			var vl = value.Length;
			var sl = _string.Length;
			if(_position + vl > sl)
				_position = sl;
			else
			{
				int pos = _string.IndexOf(value, _position);
				if(pos == -1)
					_position = sl;
				else
					_position = pos + vl;
			}
		}

		public int FindNoAdvance(string value)
		{
			return _string.IndexOf(value, _position);
		}

		public int FindNoAdvance(string value, int limit)
		{
			return _string.IndexOf(value, _position, limit);
		}

		public int FindNoAdvance(char value)
		{
			return _string.IndexOf(value, _position);
		}

		public int FindNoAdvance(char value, int limit)
		{
			return _string.IndexOf(value, _position, limit);
		}

		public int FindSeparatingEmptyLine(int limit, out int part2Start)
		{
			bool prevN = false;
			bool prevR = false;
			int lines = 0;
			for(int i = _position; i < limit; ++i)
			{
				var c = _string[i];
				switch(c)
				{
					case '\r':
						if(prevN)
							prevN = false;
						else
							lines = 0;
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
			get { return _position; }
			set
			{
				if(value < 0) throw new ArgumentOutOfRangeException();
				if(value > _string.Length)
					_position = _string.Length;
				else
					_position = value;
			}
		}

		public int RemainingSymbols
		{
			get { return _string.Length - _position; }
		}

		public int Length
		{
			get { return _string.Length; }
		}

		public char CurrentChar
		{
			get { return _string[_position]; }
		}

		public char this[int index]
		{
			get { return _string[_position + index]; }
		}

		public void Skip()
		{
			++_position;
		}

		public void Skip(int amount)
		{
			_position += amount;
		}

		public int Skip(char value)
		{
			int skipped = 0;
			while(_position < _string.Length && _string[_position] == value)
			{
				++_position;
				++skipped;
			}
			return skipped;
		}

		public void GoToStart()
		{
			_position = 0;
		}

		public void GoToEnd()
		{
			_position = _string.Length;
		}

		public void PushPosition()
		{
			if(_positions == null)
				_positions = new Stack<int>();
			_positions.Push(_position);
		}

		public void PushPosition(int newPosition)
		{
			if(_positions == null)
				_positions = new Stack<int>();
			_positions.Push(_position);
			_position = newPosition;
		}

		public void PopPosition()
		{
			if(_positions == null || _positions.Count == 0) throw new InvalidOperationException();
			_position = _positions.Pop();
		}

		public Version ReadVersion()
		{
			while(!IsAtEndOfLine && !char.IsDigit(CurrentChar))
			{
				Skip();
			}
			PushPosition();
			Skip();
			int parts = 0;
			int[] values = new int[4];
			while(!IsAtEndOfLine && parts < 4)
			{
				if(CheckValue('.'))
				{
					var pos = Position;
					PopPosition();
					values[parts++] = int.Parse(
						ReadStringUpTo(pos, 1),
						NumberStyles.None,
						CultureInfo.InvariantCulture);
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
							values[parts++] = int.Parse(
								ReadStringUpTo(pos, 0),
								NumberStyles.None,
								CultureInfo.InvariantCulture);
						}
						break;
					}
					else
					{
						Skip();
					}
				}
			}
			if(parts > 2)
			{
				return new Version(values[0], values[1], values[2], values[3]);
			}
			throw new Exception("Unable to read version.");
		}

		public override string ToString()
		{
			return _string;
		}
	}
}
