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
	using System.Collections.Generic;

	/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
	public sealed class InitCommand : Command
	{
		public static CommandArgument Template(string template)
		{
			return new CommandArgument("--template", template, '=');
		}

		public static CommandArgument Shared(string permissions)
		{
			return new CommandArgument("--shared", permissions, '=');
		}

		public static CommandArgument Bare()
		{
			return new CommandArgument("--bare");
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public InitCommand()
			: base("init")
		{
		}

		public InitCommand(params CommandArgument[] args)
			: base("init", args)
		{
		}

		public InitCommand(IList<CommandArgument> args)
			: base("init", args)
		{
		}
	}
}
