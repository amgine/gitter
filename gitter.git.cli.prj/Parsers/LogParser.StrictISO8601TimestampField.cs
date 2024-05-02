namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Runtime.CompilerServices;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class StrictISO8601TimestampField : ITextFieldParser<DateTimeOffset>
	{
		const int FieldSizeZ = 20;
		const int FieldSize  = 25;

		// 0123456789012345678901234
		// 2020-06-02T16:21:00+03:00
		// 2020-06-02T16:21:00Z

		private readonly char[] _buffer;
		private int _offset;
		private DateTimeOffset _value;
		private FieldParserState _state;

		public StrictISO8601TimestampField()
		{
			_buffer = new char[FieldSize];
			_state = FieldParserState.Initial;
		}

#if NETCOREAPP

		private bool InitialParseFull(ref ReadOnlySpan<char> text, int fieldSize)
		{
			_value = ParseCore(text);
			if(text.Length > fieldSize)
			{
				text = text[(fieldSize + 1)..];
				_state = FieldParserState.Completed;
				return true;
			}
			else
			{
				text = default;
				_state = FieldParserState.WaitingTerminator;
				return false;
			}
		}

		private void InitialBuffer(ref ReadOnlySpan<char> text)
		{
			_offset = text.Length;
			text.CopyTo(_buffer);
			text = default;
			_state = FieldParserState.Buffering;
		}

		private bool ParseTerminator(ref ReadOnlySpan<char> text)
		{
			if(text.Length == 0) return false;

			text = text[1..];
			_state = FieldParserState.Completed;
			return true;
		}

#else

		private bool InitialParseFull(ITextSegment text, int fieldSize)
		{
			text.MoveTo(_buffer, 0, fieldSize);
			_value = ParseCore(_buffer);
			if(text.Length > 0)
			{
				text.Skip();
				_state = FieldParserState.Completed;
				return true;
			}
			else
			{
				_state = FieldParserState.WaitingTerminator;
				return false;
			}
		}

		private void InitialBuffer(ITextSegment text)
		{
			_offset = text.Length;
			text.MoveTo(_buffer, 0, _offset);
			_state = FieldParserState.Buffering;
		}

		private bool ParseTerminator(ITextSegment text)
		{
			if(text.Length == 0) return false;

			text.Skip();
			_state = FieldParserState.Completed;
			return true;
		}

#endif

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
					if(text.Length >= FieldSizeZ && text[OffsetSignIndex] == 'Z')
					{
#if NETCOREAPP
						return InitialParseFull(ref text, FieldSizeZ);
#else
						return InitialParseFull(text, FieldSizeZ);
#endif
					}
					else if(text.Length >= FieldSize)
					{
#if NETCOREAPP
						return InitialParseFull(ref text, FieldSize);
#else
						return InitialParseFull(text, FieldSize);
#endif
					}
					else
					{
#if NETCOREAPP
						InitialBuffer(ref text);
#else
						InitialBuffer(text);
#endif
						return false;
					}
				case FieldParserState.Buffering:
					if(_offset >= FieldSizeZ)
					{
#if NETCOREAPP
						if(FillBufferExcludeLastChar(_buffer, FieldSize, ref _offset, ref text))
#else
						if(FillBufferExcludeLastChar(_buffer, FieldSize, ref _offset, text))
#endif
						{
							_value = ParseCore(_buffer);
							_state = FieldParserState.Completed;
							return true;
						}
						else
						{
							return false;
						}
					}
					else
					{
#if NETCOREAPP
						if(FillBuffer(_buffer, FieldSizeZ, ref _offset, ref text))
#else
						if(FillBuffer(_buffer, FieldSizeZ, ref _offset, text))
#endif
						{
							if(_buffer[OffsetSignIndex] == 'Z')
							{
								_value = ParseCore(_buffer);
								if(text.Length > 0)
								{
#if NETCOREAPP
									text = text[1..];
#else
									text.Skip();
#endif
									_state = FieldParserState.Completed;
									return true;
								}
								else
								{
									_state = FieldParserState.WaitingTerminator;
									return false;
								}
							}
							else
							{
								if(text.Length <= 0) return false;
								goto case FieldParserState.Buffering;
							}
						}
						else
						{
							return false;
						}
					}
				case FieldParserState.WaitingTerminator:
#if NETCOREAPP
					return ParseTerminator(ref text);
#else
					return ParseTerminator(text);
#endif
				case FieldParserState.Completed:
					throw new InvalidOperationException("Field is already completed.");
				default:
					throw new ApplicationException($"Invalid state: {_state}");
			}
		}

		public void Reset()
		{
			_state  = FieldParserState.Initial;
			_offset = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Digit(char c)
		{
#if DEBUG
			var d = c - '0';
			if(d is < 0 or > 9) throw new FormatException();
			return d;
#else
			return c - '0';
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetYear(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) =>
			Digit(buffer[0]) * 1000 +
			Digit(buffer[1]) *  100 +
			Digit(buffer[2]) *   10 +
			Digit(buffer[3]) *    1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Get2Digits(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer,
#else
			char[] buffer,
#endif
			int offset) =>
			Digit(buffer[offset + 0]) * 10 +
			Digit(buffer[offset + 1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetMonth(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => Get2Digits(buffer, 5);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDay(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => Get2Digits(buffer, 8);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetHours(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => Get2Digits(buffer, 11);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetMinutes(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => Get2Digits(buffer, 14);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetSeconds(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => Get2Digits(buffer, 17);

		const int OffsetSignIndex    = 19;
		const int OffsetHoursIndex   = 20;
		const int OffsetMinutesIndex = 23;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetOffsetSign(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			)
			=> buffer[OffsetSignIndex] switch
			{
				'+' =>  1,
				'-' => -1,
				'Z' =>  0,
				 _  => throw new FormatException($"Unexpected character at TZ offset sign: {buffer[OffsetSignIndex]}"),
			};

		private static TimeSpan GetOffset(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			)
		{
			var sign = GetOffsetSign(buffer);
			if(sign == 0) return TimeSpan.Zero;
			var hours   = Get2Digits(buffer, OffsetHoursIndex);
			var minutes = Get2Digits(buffer, OffsetMinutesIndex);
			return new(sign * (hours * 60 + minutes) * TimeSpan.TicksPerMinute);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static DateTimeOffset ParseCore(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => new(
			year:   GetYear   (buffer),
			month:  GetMonth  (buffer),
			day:    GetDay    (buffer),
			hour:   GetHours  (buffer),
			minute: GetMinutes(buffer),
			second: GetSeconds(buffer),
			offset: GetOffset (buffer));

		public DateTimeOffset GetValue() => _value;
	}
}
