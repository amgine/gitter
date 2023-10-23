namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class MultiHashFieldParser : ITextFieldParser<List<Hash>>
	{
		enum ParserState
		{
			ExpectNewLineOrHash,
			ExpectNewLineOrSpace,
			ExpectHash,
		}

		const char Separator = ' ';
		const char Terminator = '\n';

		private readonly char[] _buffer;
		private readonly List<Hash> _hashes;
		private int _offset;
		private ParserState _state;
		private bool _isCompleted;

		public MultiHashFieldParser()
		{
			_buffer = new char[Hash.HexStringLength];
			_hashes = new List<Hash>();
		}

		public void Reset()
		{
			_state = ParserState.ExpectNewLineOrHash;
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
						case ParserState.ExpectNewLineOrHash:
							if(text[0] == Terminator)
							{
								text = text[1..];
								_isCompleted = true;
								return true;
							}
							goto case ParserState.ExpectHash;
						case ParserState.ExpectNewLineOrSpace:
							switch(text[0])
							{
								case Terminator: // no more hashes available
									text = text[1..];
									_isCompleted = true;
									return true;
								case Separator: // more hashes available
									text = text[1..];
									_state = ParserState.ExpectHash;
									continue;
								default:
									throw new ApplicationException(
										$"Unexpected character: '{text[0]}'. Expected '\\n' or ' '.");
							}
						case ParserState.ExpectHash:
							if(text.Length >= Hash.HexStringLength)
							{
								_hashes.Add(new Hash(text));
								text = text[Hash.HexStringLength..];
								_state = ParserState.ExpectNewLineOrSpace;
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
					int c = Math.Min(text.Length, Hash.HexStringLength - _offset);
					text[..c].CopyTo(new(_buffer, _offset, c));
					text = text[c..];
					_offset += c;
					if(_offset == Hash.HexStringLength)
					{
						_hashes.Add(new Hash(_buffer));
						_offset = 0;
						_state = ParserState.ExpectNewLineOrSpace;
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
						case ParserState.ExpectNewLineOrHash:
							if(textSegment.PeekChar() == Terminator)
							{
								textSegment.Skip(1);
								_isCompleted = true;
								return true;
							}
							goto case ParserState.ExpectHash;
						case ParserState.ExpectNewLineOrSpace:
							switch(textSegment.PeekChar())
							{
								case Terminator: // no more hashes available
									textSegment.Skip(1);
									_isCompleted = true;
									return true;
								case Separator: // more hashes available
									textSegment.Skip(1);
									_state = ParserState.ExpectHash;
									continue;
								default:
									throw new ApplicationException(
										$"Unexpected character: '{textSegment.PeekChar()}'. Expected '\\n' or ' '.");
							}
						case ParserState.ExpectHash:
							if(textSegment.Length >= Hash.HexStringLength)
							{
								_hashes.Add(new Hash(textSegment.ReadString(Hash.HexStringLength)));
								_state = ParserState.ExpectNewLineOrSpace;
								continue;
							}
							else
							{
								_offset = textSegment.Length;
								textSegment.MoveTo(_buffer, 0, textSegment.Length);
								return false;
							}
					}
				}
				else
				{
					int c = Math.Min(textSegment.Length, Hash.HexStringLength - _offset);
					textSegment.MoveTo(_buffer, _offset, c);
					_offset += c;
					if(_offset == Hash.HexStringLength)
					{
						_hashes.Add(new Hash(_buffer));
						_offset = 0;
						_state = ParserState.ExpectNewLineOrSpace;
						continue;
					}
				}
			}
			return false;
		}

#endif

		public List<Hash> GetValue() => _hashes;
	}
}
