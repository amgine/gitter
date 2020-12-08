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

namespace gitter.Framework
{
	using System;
	using System.Globalization;
	using System.Runtime.CompilerServices;
	using System.Text;

	/// <summary>Extension methods for <see cref="System.DateTime"/>.</summary>
	public static class DateTimeExtensions
	{
		private static readonly string[] RFC2822Months = new string[]
		{
			"Jan", "Feb", "Mar", "Apr",
			"May", "Jun", "Jul", "Aug",
			"Sep", "Oct", "Nov", "Dec"
		};

		private static readonly string[] RFC2822WeekDays = new string[]
		{
			"Sun", "Mon", "Tue", "Wed", "Thu",
			"Fri", "Sat"
		};

		/// <summary>Formats date in ISO8601 format (1989-10-24 15:22:03 +0200).</summary>
		/// <param name="date">Date.</param>
		/// <param name="includeUTCOffset">Include UTC offset.</param>
		/// <returns>ISO8601-formatted date.</returns>
		public static string FormatISO8601(this DateTime date, bool includeUTCOffset = true)
		{
			var ci = System.Globalization.CultureInfo.InvariantCulture;
			var offset = TimeZone.CurrentTimeZone.GetUtcOffset(date);
			bool neg = offset.Ticks < 0;
			if(neg) offset = offset.Negate();
			var sb = new StringBuilder(25);
			sb.Append(date.Year.ToString(ci));
			sb.Append('-');
			if(date.Month <= 9) sb.Append('0');
			sb.Append(date.Month.ToString(ci));
			sb.Append('-');
			if(date.Day <= 9) sb.Append('0');
			sb.Append(date.Day.ToString(ci));
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
				sb.Append(' ');
				sb.Append(neg ? '-' : '+');
				if(offset.Hours <= 9) sb.Append('0');
				sb.Append(offset.Hours.ToString(ci));
				if(offset.Minutes <= 9) sb.Append('0');
				sb.Append(offset.Minutes.ToString(ci));
			}
			return sb.ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe void Write4Digits(char* p, int value)
		{
			var d1 = value / 1000;
			value %= 1000;
			var d2 = value / 100;
			value %= 100;
			var d3 = value / 10;
			value %= 10;

			p[0] = (char)(d1 + '0');
			p[1] = (char)(d2 + '0');
			p[2] = (char)(d3 + '0');
			p[3] = (char)(value + '0');
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe void Write2Digits(char* p, int value)
		{
			p[0] = (char)((value / 10) + '0');
			p[1] = (char)((value % 10) + '0');
		}

		/// <summary>Formats date in ISO8601 format (1989-10-24 15:22:03 +0200).</summary>
		/// <param name="date">Date.</param>
		/// <param name="includeUTCOffset">Include UTC offset.</param>
		/// <returns>ISO8601-formatted date.</returns>
		public static string FormatISO8601(this DateTimeOffset date, bool includeUTCOffset = true)
		{
			var str = Utility.FastAllocateString(includeUTCOffset ? 25 : 19);
			unsafe
			{
				fixed(char* p = str)
				{
					Write4Digits(p + 0, date.Year);
					p[4] = '-';
					Write2Digits(p + 5, date.Month);
					p[7] = '-';
					Write2Digits(p + 8, date.Day);
					p[10] = ' ';
					Write2Digits(p + 11, date.Hour);
					p[13] = ':';
					Write2Digits(p + 14, date.Minute);
					p[16] = ':';
					Write2Digits(p + 17, date.Second);
					if(includeUTCOffset)
					{
						p[19] = ' ';
						var offset = date.Offset;
						if(offset.Ticks < 0)
						{
							p[20] = '-';
							Write2Digits(p + 21, -offset.Hours);
							Write2Digits(p + 23, -offset.Minutes);
						}
						else
						{
							p[20] = '+';
							Write2Digits(p + 21, offset.Hours);
							Write2Digits(p + 23, offset.Minutes);
						}
					}
				}
			}

			return str;
		}

		/// <summary>Formats date in RFC2822 format (Tue, 7 Dec 2010 21:30:44 +0300).</summary>
		/// <param name="date">Date.</param>
		/// <param name="includeUTCOffset">Include UTC offset.</param>
		/// <returns>RFC2822-formatted date.</returns>
		public static string FormatRFC2822(this DateTime date, bool includeUTCOffset = true)
		{
			var ci = System.Globalization.CultureInfo.InvariantCulture;
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
				var offset = TimeZone.CurrentTimeZone.GetUtcOffset(date);
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

		/// <summary>Formats date in RFC2822 format (Tue, 7 Dec 2010 21:30:44 +0300).</summary>
		/// <param name="date">Date.</param>
		/// <param name="includeUTCOffset">Include UTC offset.</param>
		/// <returns>RFC2822-formatted date.</returns>
		public static string FormatRFC2822(this DateTimeOffset date, bool includeUTCOffset = true)
		{
			var ci = System.Globalization.CultureInfo.InvariantCulture;
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
				var offset = date.Offset;
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
	}
}
