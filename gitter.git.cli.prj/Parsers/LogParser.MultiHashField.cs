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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class MultiHashField : IRevisionFieldParser<List<Sha1Hash>>
	{
		enum ParserState
		{
			ExpectTerminatorOrValue,
			ExpectTerminatorOrSeparator,
			ExpectValue,
		}

		const char Separator = ' ';
		const char Terminator = '\n';

		private readonly char[] _buffer;
		private readonly List<Sha1Hash> _hashes;
		private int _offset;
		private ParserState _state;
		private bool _isCompleted;

		public MultiHashField()
		{
			_buffer = new char[Sha1Hash.HexStringLength];
			_hashes = [];
		}

		public void Reset()
		{
			_state = ParserState.ExpectTerminatorOrValue;
			_isCompleted = false;
			_offset = 0;
			_hashes.Clear();
		}

#if NETCOREAPP

		public bool Parse(ref ReadOnlySpan<char> text)
		{
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			while(text.Length > 0)
			{
				if(_offset == 0)
				{
					switch(_state)
					{
						case ParserState.ExpectTerminatorOrValue:
							if(text[0] == Terminator)
							{
								text = text[1..];
								_isCompleted = true;
								return true;
							}
							goto case ParserState.ExpectValue;
						case ParserState.ExpectTerminatorOrSeparator:
							switch(text[0])
							{
								case Terminator: // no more hashes available
									text = text[1..];
									_isCompleted = true;
									return true;
								case Separator: // more hashes available
									text = text[1..];
									_state = ParserState.ExpectValue;
									continue;
								default:
									throw new ApplicationException(
										$"Unexpected character: '{text[0]}'. Expected '\\n' or ' '.");
							}
						case ParserState.ExpectValue:
							if(text.Length >= Sha1Hash.HexStringLength)
							{
								_hashes.Add(Sha1Hash.Parse(text));
								text = text[Sha1Hash.HexStringLength..];
								_state = ParserState.ExpectTerminatorOrSeparator;
								continue;
							}
							else
							{
								text.CopyTo(_buffer);
								_offset = text.Length;
								text = default;
								return false;
							}
					}
				}
				else
				{
					int c = Math.Min(text.Length, Sha1Hash.HexStringLength - _offset);
					text[..c].CopyTo(new(_buffer, _offset, c));
					text = text[c..];
					_offset += c;
					if(_offset == Sha1Hash.HexStringLength)
					{
						_hashes.Add(Sha1Hash.Parse(_buffer));
						_offset = 0;
						_state = ParserState.ExpectTerminatorOrSeparator;
						continue;
					}
				}
			}
			return false;
		}

#else

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			while(textSegment.Length > 0)
			{
				if(_offset == 0)
				{
					switch(_state)
					{
						case ParserState.ExpectTerminatorOrValue:
							if(textSegment.PeekChar() == Terminator)
							{
								textSegment.Skip(1);
								_isCompleted = true;
								return true;
							}
							goto case ParserState.ExpectValue;
						case ParserState.ExpectTerminatorOrSeparator:
							switch(textSegment.PeekChar())
							{
								case Terminator: // no more hashes available
									textSegment.Skip(1);
									_isCompleted = true;
									return true;
								case Separator: // more hashes available
									textSegment.Skip(1);
									_state = ParserState.ExpectValue;
									continue;
								default:
									throw new ApplicationException(
										$"Unexpected character: '{textSegment.PeekChar()}'. Expected '\\n' or ' '.");
							}
						case ParserState.ExpectValue:
							if(textSegment.Length >= Sha1Hash.HexStringLength)
							{
								textSegment.MoveTo(_buffer, 0, Sha1Hash.HexStringLength);
								_hashes.Add(Sha1Hash.Parse(_buffer));
								_state = ParserState.ExpectTerminatorOrSeparator;
								continue;
							}
							else
							{
								_offset = textSegment.Length;
								textSegment.MoveTo(_buffer, 0, _offset);
								return false;
							}
					}
				}
				else
				{
					int c = Math.Min(textSegment.Length, Sha1Hash.HexStringLength - _offset);
					textSegment.MoveTo(_buffer, _offset, c);
					_offset += c;
					if(_offset == Sha1Hash.HexStringLength)
					{
						_hashes.Add(Sha1Hash.Parse(_buffer));
						_offset = 0;
						_state = ParserState.ExpectTerminatorOrSeparator;
						continue;
					}
				}
			}
			return false;
		}

#endif

		public List<Sha1Hash> GetValue() => _hashes;
	}
}
