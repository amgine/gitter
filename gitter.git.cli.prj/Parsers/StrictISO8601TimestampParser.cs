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
using System.Runtime.CompilerServices;

static class StrictISO8601TimestampParser
{
	// 0123456789012345678901234
	// 2020-06-02T16:21:00+03:00
	// 2020-06-02T16:21:00Z

	public static class OffsetOf
	{
		public const int Year             = 0;
		public const int Month            = Year    + 4 + 1;
		public const int Day              = Month   + 2 + 1;
		public const int Hours            = Day     + 2 + 1;
		public const int Minutes          = Hours   + 2 + 1;
		public const int Seconds          = Minutes + 2 + 1;
		public const int UtcOffsetSign    = Seconds + 2;
		public const int UtcOffsetHours   = UtcOffsetSign  + 1;
		public const int UtcOffsetMinutes = UtcOffsetHours + 2 + 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Digit(char c)
	{
		var digit = c - '0';
		return digit is >=0 and <= 9
			? digit
			: throw new FormatException($"Expected digit: {c}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Digit(byte c)
	{
		var digit = c - '0';
		return digit is >=0 and <= 9
			? digit
			: throw new FormatException($"Expected digit: {(char)c}");
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

#if NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetYear(in ReadOnlySpan<byte> buffer) =>
		Digit(buffer[0]) * 1000 +
		Digit(buffer[1]) *  100 +
		Digit(buffer[2]) *   10 +
		Digit(buffer[3]) *    1;

#else

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetYear(ArraySegment<byte> buffer) =>
		Digit(buffer.Array[buffer.Offset + 0]) * 1000 +
		Digit(buffer.Array[buffer.Offset + 1]) *  100 +
		Digit(buffer.Array[buffer.Offset + 2]) *   10 +
		Digit(buffer.Array[buffer.Offset + 3]) *    1;

#endif

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetYear(string buffer) =>
		Digit(buffer[0]) * 1000 +
		Digit(buffer[1]) *  100 +
		Digit(buffer[2]) *   10 +
		Digit(buffer[3]) *    1;

#endif

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

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Get2Digits(string buffer, int offset) =>
		Digit(buffer[offset + 0]) * 10 +
		Digit(buffer[offset + 1]);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Get2Digits(ArraySegment<byte> buffer, int offset) =>
		Digit(buffer.Array[buffer.Offset + offset + 0]) * 10 +
		Digit(buffer.Array[buffer.Offset + offset + 1]);

#else

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Get2Digits(in ReadOnlySpan<byte> buffer, int offset) =>
		Digit(buffer[offset + 0]) * 10 +
		Digit(buffer[offset + 1]);

#endif


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMonth(
#if NETCOREAPP
		in ReadOnlySpan<char> buffer
#else
		char[] buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Month);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMonth(
#if NETCOREAPP
		in ReadOnlySpan<byte> buffer
#else
		ArraySegment<byte> buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Month);

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMonth(string buffer)
		=> Get2Digits(buffer, OffsetOf.Month);

#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetDay(
#if NETCOREAPP
		in ReadOnlySpan<char> buffer
#else
		char[] buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Day);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetDay(
#if NETCOREAPP
		in ReadOnlySpan<byte> buffer
#else
		ArraySegment<byte> buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Day);

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetDay(string buffer)
		=> Get2Digits(buffer, OffsetOf.Day);

#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetHours(
#if NETCOREAPP
		in ReadOnlySpan<char> buffer
#else
		char[] buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Hours);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetHours(
#if NETCOREAPP
		in ReadOnlySpan<byte> buffer
#else
		ArraySegment<byte> buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Hours);

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetHours(string buffer)
		=> Get2Digits(buffer, OffsetOf.Hours);

#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMinutes(
#if NETCOREAPP
		in ReadOnlySpan<char> buffer
#else
		char[] buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Minutes);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMinutes(
#if NETCOREAPP
		in ReadOnlySpan<byte> buffer
#else
		ArraySegment<byte> buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Minutes);

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMinutes(string buffer)
		=> Get2Digits(buffer, OffsetOf.Minutes);

#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetSeconds(
#if NETCOREAPP
		in ReadOnlySpan<char> buffer
#else
		char[] buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Seconds);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetSeconds(
#if NETCOREAPP
		in ReadOnlySpan<byte> buffer
#else
		ArraySegment<byte> buffer
#endif
		) => Get2Digits(buffer, OffsetOf.Seconds);

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetSeconds(string buffer)
		=> Get2Digits(buffer, OffsetOf.Seconds);

#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetOffsetSign(
#if NETCOREAPP
		in ReadOnlySpan<char> buffer
#else
		char[] buffer
#endif
		)
		=> buffer[OffsetOf.UtcOffsetSign] switch
		{
			'+' =>  1,
			'-' => -1,
			'Z' =>  0,
			_   => throw new FormatException($"Unexpected character at TZ offset sign: {buffer[OffsetOf.UtcOffsetSign]}"),
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetOffsetSign(
#if NETCOREAPP
		in ReadOnlySpan<byte> buffer
#else
		ArraySegment<byte> buffer
#endif
		)
	{
#if NETCOREAPP
		var value = buffer[OffsetOf.UtcOffsetSign];
#else
		var value = buffer.Array[buffer.Offset + OffsetOf.UtcOffsetSign];
#endif
		return value switch
		   {
			   (byte)'+' =>  1,
			   (byte)'-' => -1,
			   (byte)'Z' =>  0,
			   _         => throw new FormatException($"Unexpected character at TZ offset sign: {value}"),
		   };
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		var hours   = Get2Digits(buffer, OffsetOf.UtcOffsetHours);
		var minutes = Get2Digits(buffer, OffsetOf.UtcOffsetMinutes);
		return new(sign * (hours * 60 + minutes) * TimeSpan.TicksPerMinute);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TimeSpan GetOffset(
#if NETCOREAPP
		in ReadOnlySpan<byte> buffer
#else
		ArraySegment<byte> buffer
#endif
		)
	{
		var sign = GetOffsetSign(buffer);
		if(sign == 0) return TimeSpan.Zero;
		var hours   = Get2Digits(buffer, OffsetOf.UtcOffsetHours);
		var minutes = Get2Digits(buffer, OffsetOf.UtcOffsetMinutes);
		return new(sign * (hours * 60 + minutes) * TimeSpan.TicksPerMinute);
	}

#if !NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetOffsetSign(string buffer)
		=> buffer[OffsetOf.UtcOffsetSign] switch
		{
			'+' =>  1,
			'-' => -1,
			'Z' =>  0,
			_   => throw new FormatException($"Unexpected character at TZ offset sign: {buffer[OffsetOf.UtcOffsetSign]}"),
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TimeSpan GetOffset(string buffer)
	{
		var sign = GetOffsetSign(buffer);
		if(sign == 0) return TimeSpan.Zero;
		var hours   = Get2Digits(buffer, OffsetOf.UtcOffsetHours);
		var minutes = Get2Digits(buffer, OffsetOf.UtcOffsetMinutes);
		return new(sign * (hours * 60 + minutes) * TimeSpan.TicksPerMinute);
	}

#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DateTimeOffset Parse(
#if NETCOREAPP
		in ReadOnlySpan<char> input
#else
		char[] input
#endif
		) => new(
		year:   GetYear   (input),
		month:  GetMonth  (input),
		day:    GetDay    (input),
		hour:   GetHours  (input),
		minute: GetMinutes(input),
		second: GetSeconds(input),
		offset: GetOffset (input));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DateTimeOffset Parse(
#if NETCOREAPP
		in ReadOnlySpan<byte> input
#else
		ArraySegment<byte> input
#endif
		) => new(
		year:   GetYear   (input),
		month:  GetMonth  (input),
		day:    GetDay    (input),
		hour:   GetHours  (input),
		minute: GetMinutes(input),
		second: GetSeconds(input),
		offset: GetOffset (input));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DateTimeOffset Parse(string input)
	{
#if NETCOREAPP
		var span = input.AsSpan();
		return new(
			year:   GetYear   (in span),
			month:  GetMonth  (in span),
			day:    GetDay    (in span),
			hour:   GetHours  (in span),
			minute: GetMinutes(in span),
			second: GetSeconds(in span),
			offset: GetOffset (in span));
#else
		return new(
			year:   GetYear   (input),
			month:  GetMonth  (input),
			day:    GetDay    (input),
			hour:   GetHours  (input),
			minute: GetMinutes(input),
			second: GetSeconds(input),
			offset: GetOffset (input));
#endif
	}
}
