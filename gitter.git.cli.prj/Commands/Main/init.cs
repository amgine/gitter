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
		public static ICommandArgument Template(string template)
		{
			return new CommandParameterValue("--template", template, '=');
		}

		public static ICommandArgument Shared(string permissions)
		{
			return new CommandParameterValue("--shared", permissions, '=');
		}

		public static ICommandArgument Bare()
		{
			return new CommandFlag("--bare");
		}

		public static ICommandArgument Quiet()
		{
			return CommandFlag.Quiet();
		}

		public InitCommand()
			: base("init")
		{
		}

		public InitCommand(params ICommandArgument[] args)
			: base("init", args)
		{
		}

		public InitCommand(IList<ICommandArgument> args)
			: base("init", args)
		{
		}
	}
}
