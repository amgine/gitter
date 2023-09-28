namespace gitter.Git.AccessLayer.CLI;

using System;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class HashField : ITextFieldParser<Hash>
	{
		private readonly char[] _buffer;
		private FieldParserState _state;
		private Hash _value;
		private int _offset;

		public HashField()
		{
			_buffer = new char[Hash.HexStringLength];
			_state  = FieldParserState.Initial;
		}

		public bool Parse(
#if NETCOREAPP
			ref ReadOnlySpan<char> text
#else
			ITextSegment text
#endif
			)
		{
			switch(_state)
			{
				case FieldParserState.Initial:
					if(text.Length >= Hash.HexStringLength)
					{
#if NETCOREAPP
						_value = new Hash(text);
#else
						text.MoveTo(_buffer, 0, Hash.HexStringLength);
						_value = new Hash(_buffer);
#endif
						if(text.Length > Hash.HexStringLength)
						{
#if NETCOREAPP
							text = text[(Hash.HexStringLength + 1)..];
#else
							text.Skip();
#endif
							_state = FieldParserState.Completed;
							return true;
						}
						else
						{
#if NETCOREAPP
							text = default;
#endif
							_state = FieldParserState.WaitingTerminator;
							return false;
						}
					}
					else
					{
						_state = FieldParserState.Buffering;
						goto case FieldParserState.Buffering;
					}
				case FieldParserState.Buffering:
#if NETCOREAPP
					if(FillBufferExcludeLastChar(_buffer, Hash.HexStringLength, ref _offset, ref text))
#else
					if(FillBufferExcludeLastChar(_buffer, Hash.HexStringLength, ref _offset, text))
#endif
					{
						_value = new(_buffer);
						_state = FieldParserState.Completed;
						return true;
					}
					else
					{
						return false;
					}
				case FieldParserState.WaitingTerminator:
					if(text.Length > 0)
					{
#if NETCOREAPP
						text = text[1..];
#else
						text.Skip();
#endif
						return true;
					}
					else
					{
						return false;
					}
				case FieldParserState.Completed:
					throw new InvalidOperationException("Field is already completed.");
				default:
					throw new ApplicationException($"Invalid state: {_state}");
			}
		}

		public void Reset()
		{
			_offset = 0;
			_state  = FieldParserState.Initial;
		}

		public Hash GetValue() => _value;
	}
}
