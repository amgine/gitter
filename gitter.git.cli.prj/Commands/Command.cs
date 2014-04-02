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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>Represents git command line command.</summary>
	public class Command
	{
		#region Data

		private readonly IEnumerable<ICommandArgument> _arguments;
		private readonly string _name;

		#endregion

		#region .ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class.
		/// </summary>
		/// <param name="name">Command name.</param>
		/// <param name="arguments">Command arguments.</param>
		public Command(string name, IEnumerable<ICommandArgument> arguments)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			_name = name;
			_arguments = arguments ?? Enumerable.Empty<ICommandArgument>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class.
		/// </summary>
		/// <param name="name">Command name.</param>
		public Command(string name)
			: this(name, null)
		{
		}

		#endregion

		#region Properties

		/// <summary>Returns command name.</summary>
		/// <value>Command name.</value>
		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>Returns collection of command arguments.</summary>
		/// <value>Collection of command arguments.</value>
		public IEnumerable<ICommandArgument> Arguments
		{
			get { return _arguments; }
		}

		#endregion

		#region Methods

		/// <summary>Appends command with all arguments to a specified <paramref name="stringBuilder"/>.</summary>
		/// <param name="stringBuilder"><see cref="StringBuilder"/> which will receive command string representation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> == <c>null</c>.</exception>
		public void ToString(StringBuilder stringBuilder)
		{
			Verify.Argument.IsNotNull(stringBuilder, "stringBuilder");

			const char ArgumentSeparator = ' ';
			stringBuilder.Append(Name);
			foreach(var arg in Arguments)
			{
				stringBuilder.Append(ArgumentSeparator);
				arg.ToString(stringBuilder);
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

		#endregion
	}
}
