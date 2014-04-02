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

	/// <summary>Remove files from the working tree and from the index.</summary>
	public sealed class RmCommand : Command
	{
		public static ICommandArgument Recursive()
		{
			return new CommandFlag("-r");
		}

		public static ICommandArgument IgnoreUnmatch()
		{
			return new CommandFlag("--ignore-unmatch");
		}

		public static ICommandArgument Cached()
		{
			return new CommandFlag("--cached");
		}

		public static ICommandArgument NoMoreOptions()
		{
			return CommandFlag.NoMoreOptions();
		}

		public static ICommandArgument Force()
		{
			return new CommandFlag("--force");
		}

		public static ICommandArgument Quiet()
		{
			return CommandFlag.Quiet();
		}

		public static ICommandArgument DryRun()
		{
			return CommandFlag.DryRun();
		}

		public RmCommand()
			: base("rm")
		{
		}

		public RmCommand(params ICommandArgument[] args)
			: base("rm", args)
		{
		}

		public RmCommand(IList<ICommandArgument> args)
			: base("rm", args)
		{
		}
	}
}
