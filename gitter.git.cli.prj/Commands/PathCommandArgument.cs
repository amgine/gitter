#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System.Text;

public sealed record class PathCommandArgument(string Path) : ICommandArgument
{
	const char DQuote = '"';

	static bool IsQuoted(string value)
		=> value.Length > 1
		&& value[0] == DQuote
		&& value[value.Length - 1] == DQuote;

	/// <inheritdoc/>
	public void ToString(StringBuilder stringBuilder)
	{
		Verify.Argument.IsNotNull(stringBuilder);

		if(string.IsNullOrEmpty(Path))
		{
			stringBuilder.Append(DQuote);
			stringBuilder.Append(DQuote);
		}
		else if(IsQuoted(Path))
		{
			stringBuilder.Append(Path);
		}
		else
		{
			stringBuilder.Append(DQuote);
			stringBuilder.Append(Path);
			stringBuilder.Append(DQuote);
		}
	}

	/// <inheritdoc/>
	public override string ToString() => Path.AssureDoubleQuotes();
}
