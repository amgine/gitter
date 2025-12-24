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

namespace gitter.Git;

public enum CommitMessageCleanupMode
{
	/// <summary>
	/// Same as <see cref="Strip"/> if the message is to be edited. Otherwise <see cref="Whitespace"/>.
	/// </summary>
	Default,
	/// <summary>
	/// Strip leading and trailing empty lines, trailing whitespace,
	/// commentary and collapse consecutive empty lines.
	/// </summary>
	Strip,
	/// <summary>
	/// Same as strip except #commentary is not removed.
	/// </summary>
	Whitespace,
	/// <summary>
	/// Do not change the message at all.
	/// </summary>
	Verbatim,
	/// <summary>
	/// Same as whitespace except that everything from (and including) the line
	/// found below is truncated, if the message is to be edited.<br/>
	/// "#" can be customized with <c>core.commentChar</c>.
	/// <code>
	/// # ------------------------ >8 ------------------------
	/// </code>
	/// </summary>
	Scissors,
}
