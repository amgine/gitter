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
		const int FieldSize = 25;

		// 0123456789012345678901234
		// 2020-06-02T16:21:00+03:00

		private readonly char[] _buffer;
		private int _offset;
		private DateTimeOffset _value;
		private FieldParserState _state;

		public StrictISO8601TimestampField()
		{
			_buffer = new char[FieldSize];
			_state = FieldParserState.Initial;
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
					if(text.Length >= FieldSize)
					{
#if NETCOREAPP
						_value = ParseCore(text);
#else
						text.MoveTo(_buffer, 0, FieldSize);
						_value = ParseCore(_buffer);
#endif
						if(text.Length > FieldSize)
						{
#if NETCOREAPP
							text = text[(FieldSize + 1)..];
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
							_state  = FieldParserState.WaitingTerminator;
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
			_state  = FieldParserState.Initial;
			_offset = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Digit(char c) => c - '0';

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
				_ => throw new FormatException($"Unexpected character at TZ offset sign: {buffer[OffsetSignIndex]}"),
			};

		private static TimeSpan GetOffset(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => new(
			GetOffsetSign(buffer) *
			(Get2Digits(buffer, OffsetHoursIndex) * 60 + Get2Digits(buffer, OffsetMinutesIndex)) *
			TimeSpan.TicksPerMinute);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static DateTimeOffset ParseCore(
#if NETCOREAPP
			in ReadOnlySpan<char> buffer
#else
			char[] buffer
#endif
			) => new(
			GetYear(buffer), GetMonth(buffer), GetDay(buffer),
			GetHours(buffer), GetMinutes(buffer), GetSeconds(buffer),
			GetOffset(buffer));

		public DateTimeOffset GetValue() => _value;
	}
}
