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
	using System.Collections.Generic;

	sealed class FsckCommand : Command
	{
		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument Root()
		{
			return new CommandArgument("--root");
		}

		public static CommandArgument Unreachable()
		{
			return new CommandArgument("--unreachable");
		}

		public static CommandArgument Cache()
		{
			return new CommandArgument("--cache");
		}

		public static CommandArgument NoReflogs()
		{
			return new CommandArgument("--no-reflogs");
		}

		public static CommandArgument Full()
		{
			return new CommandArgument("--full");
		}

		public static CommandArgument NoFull()
		{
			return new CommandArgument("--no-full");
		}

		public static CommandArgument Strict()
		{
			return new CommandArgument("--strict");
		}

		public static CommandArgument LostFound()
		{
			return new CommandArgument("--lost-found");
		}

		public static CommandArgument Dangling()
		{
			return new CommandArgument("--dangling");
		}

		public static CommandArgument NoDangling()
		{
			return new CommandArgument("--no-dangling");
		}

		public static CommandArgument Progress()
		{
			return new CommandArgument("--progress");
		}

		public static CommandArgument NoProgress()
		{
			return new CommandArgument("--no-progress");
		}

		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public FsckCommand()
			: base("fsck")
		{
		}

		public FsckCommand(params CommandArgument[] args)
			: base("fsck", args)
		{
		}

		public FsckCommand(IEnumerable<CommandArgument> args)
			: base("fsck", args)
		{
		}
	}
}
