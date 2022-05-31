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

namespace gitter;

/// <summary>Extension methods for <see cref="System.Char"/>.</summary>
public static class CharExtensions
{
	/// <summary>Determines whether this char is oct digit.</summary>
	/// <param name="c">Character.</param>
	/// <returns><c>true</c> if specified char is oct digit; otherwise, <c>false</c>.</returns>
	public static bool IsOctDigit(this char c)
		=> (c - '0') is >= 0 and < 8;
}
