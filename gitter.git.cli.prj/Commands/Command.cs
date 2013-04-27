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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;
	using System.Collections.Generic;

	/// <summary>Represents git command line command.</summary>
	public class Command
	{
		protected static readonly IEnumerable<CommandArgument> NoArguments =
			new CommandArgument[0];

		private readonly IEnumerable<CommandArgument> _arguments;
		private readonly string _name;

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class.
		/// </summary>
		/// <param name="name">Command name.</param>
		/// <param name="arguments">Command arguments.</param>
		public Command(string name, IEnumerable<CommandArgument> arguments)
		{
			if(string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Invalid command name.", "name");
			}

			_name = name;
			if(arguments == null)
			{
				_arguments = NoArguments;
			}
			else
			{
				_arguments = arguments;
			}
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
		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>Returns list of command arguments.</summary>
		/// <value>List of command arguments.</value>
		public IEnumerable<CommandArgument> Arguments
		{
			get { return _arguments; }
		}

		/// <summary>Returns string representation of command with arguments.</summary>
		/// <returns>String representation of command with arguments.</returns>
		public string GetCommand()
		{
			var sb = new StringBuilder();
			GetCommand(sb);
			return sb.ToString();
		}

		/// <summary>Appends command with all arguments to a specified <paramref name="stringBuilder"/>.</summary>
		/// <param name="stringBuilder"><see cref="StringBuilder"/> which will receive command string representation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> == <c>null</c>.</exception>
		public void GetCommand(StringBuilder stringBuilder)
		{
			Verify.Argument.IsNotNull(stringBuilder, "stringBuilder");

			const char ArgumentSeparator = ' ';

			stringBuilder.Append(Name);
			foreach(var arg in Arguments)
			{
				stringBuilder.Append(ArgumentSeparator);
				arg.GetArgument(stringBuilder);
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this <see cref="Command"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this <see cref="Command"/>.
		/// </returns>
		public override string ToString()
		{
			return GetCommand();
		}
	}
}
