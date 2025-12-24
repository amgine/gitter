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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>Extension methods for <see cref="System.DateTime"/>.</summary>
public static class DateTimeExtensions
{
	private static readonly string[] RFC2822Months =
	{
		"Jan", "Feb", "Mar", "Apr",
		"May", "Jun", "Jul", "Aug",
		"Sep", "Oct", "Nov", "Dec"
	};

	private static readonly string[] RFC2822WeekDays =
	{
		"Sun", "Mon", "Tue", "Wed", "Thu",
		"Fri", "Sat"
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void Write4Digits(char* p, int value)
	{
		var d1 = Math.DivRem(value, 1000, out value);
		var d2 = Math.DivRem(value,  100, out value);
		var d3 = Math.DivRem(value,   10, out var d4);

		p[0] = (char)(d1 + '0');
		p[1] = (char)(d2 + '0');
		p[2] = (char)(d3 + '0');
		p[3] = (char)(d4 + '0');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void Write2Digits(char* p, int value)
	{
		var d1 = Math.DivRem(value, 10, out var d2);
		p[0] = (char)(d1 + '0');
		p[1] = (char)(d2 + '0');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void WriteISO8601Offset(char* p, TimeSpan offset)
	{
		var ticks = offset.Ticks;
		if(ticks < 0)
		{
			p[0] = '-';
			ticks = -ticks;
		}
		else
		{
			p[0] = '+';
		}
		var hours   = (int)(ticks / TimeSpan.TicksPerHour);
		var minutes = (int)(ticks / TimeSpan.TicksPerMinute % 60);
		Write2Digits(p + 1, hours);
		Write2Digits(p + 3, minutes);
	}

	/// <summary>Formats date in ISO8601 format (1989-10-24 15:22:03 +0200).</summary>
	/// <param name="date">Date.</param>
	/// <param name="includeUTCOffset">Include UTC offset.</param>
	/// <returns>ISO8601-formatted date.</returns>
	public static unsafe string FormatISO8601(this DateTime date, bool includeUTCOffset = true)
	{
		var len   = includeUTCOffset ? 25 : 19;
		var chars = stackalloc char[len];

		Write4Digits(chars +  0, date.Year);
		chars[4] = '-';
		Write2Digits(chars +  5, date.Month);
		chars[7] = '-';
		Write2Digits(chars +  8, date.Day);
		chars[10] = ' ';
		Write2Digits(chars + 11, date.Hour);
		chars[13] = ':';
		Write2Digits(chars + 14, date.Minute);
		chars[16] = ':';
		Write2Digits(chars + 17, date.Second);
		if(includeUTCOffset)
		{
			chars[19] = ' ';
			WriteISO8601Offset(chars + 20, TimeZoneInfo.Local.GetUtcOffset(date));
		}

		return new(chars, 0, len);
	}

	private static unsafe void FormatISO8601(DateTimeOffset date, char* chars, bool includeUTCOffset)
	{
		Write4Digits(chars +  0, date.Year);
		chars[4] = '-';
		Write2Digits(chars +  5, date.Month);
		chars[7] = '-';
		Write2Digits(chars +  8, date.Day);
		chars[10] = ' ';
		Write2Digits(chars + 11, date.Hour);
		chars[13] = ':';
		Write2Digits(chars + 14, date.Minute);
		chars[16] = ':';
		Write2Digits(chars + 17, date.Second);
		if(includeUTCOffset)
		{
			chars[19] = ' ';
			var offset = date.Offset;
			WriteISO8601Offset(chars + 20, date.Offset);
		}
	}

	/// <summary>Formats date in <c>ISO8601</c> format.</summary>
	/// <param name="date">Date.</param>
	/// <param name="includeUTCOffset">Include UTC offset.</param>
	/// <returns>ISO8601-formatted date.</returns>
	/// <remarks><c>1989-10-24 15:22:03 +0200</c></remarks>
	public static unsafe string FormatISO8601(this DateTimeOffset date, bool includeUTCOffset = true)
	{
		var len   = includeUTCOffset ? 25 : 19;
		var chars = stackalloc char[len];

		FormatISO8601(date, chars, includeUTCOffset);

		return new(chars, 0, len);
	}

	/// <summary>Formats date in <c>ISO8601</c> format.</summary>
	/// <param name="date">Date.</param>
	/// <param name="text">Buffer that will receive the formatted date.</param>
	/// <param name="charsWritten">Number of written characters.</param>
	/// <param name="includeUTCOffset">Include UTC offset.</param>
	/// <returns><c>true</c>, if date was formatted, <c>false</c> otherwise.</returns>
	/// <remarks><c>1989-10-24 15:22:03 +0200</c></remarks>
	public static unsafe bool TryFormatISO8601(this DateTimeOffset date, Span<char> text, out int charsWritten, bool includeUTCOffset = true)
	{
		var len = includeUTCOffset ? 25 : 19;
		if(text.Length < len)
		{
			charsWritten = 0;
			return false;
		}
		fixed(char* chars = text)
		{
			FormatISO8601(date, chars, includeUTCOffset);
		}
		charsWritten = len;
		return true;
	}

	/// <summary>Formats date in RFC2822 format (Tue, 7 Dec 2010 21:30:44 +0300).</summary>
	/// <param name="date">Date.</param>
	/// <param name="includeUTCOffset">Include UTC offset.</param>
	/// <returns>RFC2822-formatted date.</returns>
	public static string FormatRFC2822(this DateTime date, bool includeUTCOffset = true)
	{
		var ci = CultureInfo.InvariantCulture;
		var sb = new StringBuilder(3 + 2 + 2 + 1 + 3 + 1 + 4 + 1 + 2 + 1 + 2 + 1 + 2 + 2 + 2 + 2);
		sb.Append(RFC2822WeekDays[(int)date.DayOfWeek]);
		sb.Append(", ");
		sb.Append(date.Day.ToString(ci));
		sb.Append(' ');
		sb.Append(RFC2822Months[date.Month - 1]);
		sb.Append(' ');
		sb.Append(date.Year.ToString(ci));
		sb.Append(' ');
		if(date.Hour <= 9) sb.Append('0');
		sb.Append(date.Hour.ToString(ci));
		sb.Append(':');
		if(date.Minute <= 9) sb.Append('0');
		sb.Append(date.Minute.ToString(ci));
		sb.Append(':');
		if(date.Second <= 9) sb.Append('0');
		sb.Append(date.Second.ToString(ci));
		if(includeUTCOffset)
		{
			var offset = TimeZoneInfo.Local.GetUtcOffset(date);
			if(offset.Ticks < 0)
			{
				offset = offset.Negate();
				sb.Append(" -");
			}
			else
			{
				sb.Append(" +");
			}
			sb.Append(offset.Hours > 10 ? offset.Hours.ToString() : "0" + offset.Hours.ToString());
			sb.Append(offset.Minutes > 10 ? offset.Minutes.ToString() : "0" + offset.Minutes.ToString());
		}
		return sb.ToString();
	}

	private static unsafe int FormatRFC2822(DateTimeOffset date, char* chars, bool includeUTCOffset)
	{
		var start = chars;

		var dow = RFC2822WeekDays[(int)date.DayOfWeek];
		for(int i = 0; i < dow.Length; ++i)
		{
			*chars++ = dow[i];
		}
		chars[0] = ',';
		chars[1] = ' ';
		chars += 2;
		var day = date.Day;
		if(day < 10)
		{
			*chars++ = (char)('0' + day);
		}
		else
		{
			Write2Digits(chars, day);
			chars += 2;
		}
		*chars++ = ' ';
		var mon = RFC2822Months[date.Month - 1];
		for(int i = 0; i < mon.Length; ++i)
		{
			*chars++ = mon[i];
		}
		*chars++ = ' ';
		var year = date.Year;
		if(year is >= 1000 and <= 9999)
		{
			Write4Digits(chars, year);
			chars += 4;
		}
		else
		{
			chars += WriteDigits(chars, year);
		}
		*chars++ = ' ';
		Write2Digits(chars + 0, date.Hour);
		chars[2] = ':';
		Write2Digits(chars + 3, date.Minute);
		chars[5] = ':';
		Write2Digits(chars + 6, date.Second);
		chars += 8;

		if(includeUTCOffset)
		{
			chars[0] = ' ';
			WriteISO8601Offset(chars + 1, date.Offset);
			chars += 7;
		}

		return (int)(chars - start);
	}

	private static int GetDecimalDigitsCount(int value)
	{
		int n = 0;
		do
		{
			value /= 10;
			++n;
		}
		while(value != 0) ;
		return n;
	}

	private static unsafe int WriteDigits(char* ptr, int value)
	{
		if(value == 0)
		{
			ptr[0] = '0';
			return 1;
		}

		var d = 1;
		while((value / d) != 0) d *= 10;
		d /= 10;

		var n = 0;
		while(d != 0)
		{
			ptr[n++] = (char)('0' + (value / d % 10));
			d /= 10;
		}
		return n;
	}

	/// <summary>Formats date in <c>RFC2822</c> format.</summary>
	/// <param name="date">Date.</param>
	/// <param name="includeUTCOffset">Include UTC offset.</param>
	/// <returns>RFC2822-formatted date.</returns>
	/// <remarks><c>Tue, 7 Dec 2010 21:30:44 +0300</c></remarks>
	public static unsafe string FormatRFC2822(this DateTimeOffset date, bool includeUTCOffset = true)
	{
		var chars = stackalloc char[64];
		var n = FormatRFC2822(date, chars, includeUTCOffset);
		return new string(chars, 0, n);
	}

	public static unsafe bool TryFormatRFC2822(this DateTimeOffset date, Span<char> text, out int charsWritten, bool includeUTCOffset = true)
	{
		if(text.Length < 32)
		{
			charsWritten = 0;
			return false;
		}
		fixed(char* chars = text)
		{
			charsWritten = FormatRFC2822(date, chars, includeUTCOffset);
		}
		return true;
	}

}
