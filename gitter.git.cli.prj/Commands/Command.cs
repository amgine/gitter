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

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>Represents git command line command.</summary>
public class Command
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Command"/> class.
	/// </summary>
	/// <param name="options">Arguments passed to git before command.</param>
	/// <param name="name">Command name.</param>
	/// <param name="arguments">Command arguments.</param>
	public Command(IEnumerable<ICommandArgument> options, string name, IEnumerable<ICommandArgument> arguments = null)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);

		Name      = name;
		Options   = options;
		Arguments = arguments;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Command"/> class.
	/// </summary>
	/// <param name="name">Command name.</param>
	/// <param name="arguments">Command arguments.</param>
	public Command(string name, IEnumerable<ICommandArgument> arguments = null)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);

		Name      = name;
		Options   = null;
		Arguments = arguments;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Command"/> class.
	/// </summary>
	/// <param name="name">Command name.</param>
	public Command(string name)
		: this(name, null)
	{
	}

	/// <summary>Returns command name.</summary>
	/// <value>Command name.</value>
	public string Name { get; }

	/// <summary>Returns collection of arguments passed to git before command.</summary>
	/// <value>Collection of arguments passed to git before command.</value>
	public IEnumerable<ICommandArgument> Options { get; }
		
	/// <summary>Returns collection of command arguments.</summary>
	/// <value>Collection of command arguments.</value>
	public IEnumerable<ICommandArgument> Arguments { get; }

	/// <summary>Appends command with all arguments to a specified <paramref name="stringBuilder"/>.</summary>
	/// <param name="stringBuilder"><see cref="StringBuilder"/> which will receive command string representation.</param>
	/// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> == <c>null</c>.</exception>
	public void ToString(StringBuilder stringBuilder)
	{
		Verify.Argument.IsNotNull(stringBuilder);

		const char ArgumentSeparator = ' ';
		if(Options is not null)
		{
			foreach(var arg in Options)
			{
				arg.ToString(stringBuilder);
				stringBuilder.Append(ArgumentSeparator);
			}
		}
		stringBuilder.Append(Name);
		if(Arguments is not null)
		{
			foreach(var arg in Arguments)
			{
				stringBuilder.Append(ArgumentSeparator);
				arg.ToString(stringBuilder);
			}
		}
	}

	/// <summary>Returns a <see cref="System.String"/> that represents this <see cref="Command"/>.</summary>
	/// <returns>A <see cref="System.String"/> that represents this <see cref="Command"/>.</returns>
	public override string ToString()
	{
		var sb = new StringBuilder();
		ToString(sb);
		return sb.ToString();
	}
}
