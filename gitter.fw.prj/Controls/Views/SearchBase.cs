#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System;

public abstract class SearchBase
{
	/// <summary>Tests if <paramref name="value"/> satisfies the specified <paramref name="search"/> options.</summary>
	/// <param name="value">String to test.</param>
	/// <param name="search">Search options.</param>
	/// <returns>
	/// <c>true</c>, if <paramref name="value"/> satisfies <paramref name="search"/>;<br/>
	/// <c>false</c> otherwise.
	/// </returns>
	protected static bool TestString(string? value, SearchOptions search)
	{
		Assert.IsNotNull(search);

		if(search.Text is not { Length: not 0 }) return true;
		if(value       is not { Length: not 0 }) return false;

		const StringComparison CaseSensitive   = StringComparison.Ordinal;
		const StringComparison CaseInsensitive = StringComparison.OrdinalIgnoreCase;

#if NETCOREAPP
		return value.Contains(search.Text, search.MatchCase
			? CaseSensitive
			: CaseInsensitive);
#else
		return value.IndexOf(search.Text, 0, search.MatchCase
			? CaseSensitive
			: CaseInsensitive) != -1;
#endif
	}
}
