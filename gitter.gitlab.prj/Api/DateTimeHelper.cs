#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Api;

using System;
using System.Runtime.CompilerServices;

static class DateTimeHelper
{
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void WriteISO8601Offset(char* p, TimeSpan offset)
	{
		var h = offset.Hours;
		var m = offset.Minutes;
		if(offset.Ticks < 0)
		{
			p[0] = '-';
			h = -h;
			m = -m;
		}
		else
		{
			p[0] = '+';
		}
		Write2Digits(p + 1, h);
		Write2Digits(p + 3, m);
	}

	public static unsafe string FormatISO8601(DateTimeOffset date)
	{
		var offset = date.Offset;
		var chars  = stackalloc char[24];

		Write4Digits(chars + 0, date.Year);
		chars[4] = '-';
		Write2Digits(chars + 5, date.Month);
		chars[7] = '-';
		Write2Digits(chars + 8, date.Day);
		chars[10] = 'T';
		Write2Digits(chars + 11, date.Hour);
		chars[13] = ':';
		Write2Digits(chars + 14, date.Minute);
		chars[16] = ':';
		Write2Digits(chars + 17, date.Second);
		if(offset.Ticks == 0)
		{
			chars[19] = 'Z';
			return new(chars, 0, 20);
		}
		else
		{
			WriteISO8601Offset(chars + 19, offset);
			return new(chars, 0, 24);
		}
	}
}
