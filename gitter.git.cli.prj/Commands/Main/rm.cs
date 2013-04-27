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
		public static CommandArgument Recursive()
		{
			return new CommandArgument("-r");
		}

		public static CommandArgument IgnoreUnmatch()
		{
			return new CommandArgument("--ignore-unmatch");
		}

		public static CommandArgument Cached()
		{
			return new CommandArgument("--cached");
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public static CommandArgument DryRun()
		{
			return CommandArgument.DryRun();
		}

		public RmCommand()
			: base("rm")
		{
		}

		public RmCommand(params CommandArgument[] args)
			: base("rm", args)
		{
		}

		public RmCommand(IList<CommandArgument> args)
			: base("rm", args)
		{
		}
	}
}
