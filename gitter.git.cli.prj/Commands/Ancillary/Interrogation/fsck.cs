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
		public static ICommandArgument Tags()
		{
			return new CommandFlag("--tags");
		}

		public static ICommandArgument Root()
		{
			return new CommandFlag("--root");
		}

		public static ICommandArgument Unreachable()
		{
			return new CommandFlag("--unreachable");
		}

		public static ICommandArgument Cache()
		{
			return new CommandFlag("--cache");
		}

		public static ICommandArgument NoReflogs()
		{
			return new CommandFlag("--no-reflogs");
		}

		public static ICommandArgument Full()
		{
			return new CommandFlag("--full");
		}

		public static ICommandArgument NoFull()
		{
			return new CommandFlag("--no-full");
		}

		public static ICommandArgument Strict()
		{
			return new CommandFlag("--strict");
		}

		public static ICommandArgument LostFound()
		{
			return new CommandFlag("--lost-found");
		}

		public static ICommandArgument Dangling()
		{
			return new CommandFlag("--dangling");
		}

		public static ICommandArgument NoDangling()
		{
			return new CommandFlag("--no-dangling");
		}

		public static ICommandArgument Progress()
		{
			return new CommandFlag("--progress");
		}

		public static ICommandArgument NoProgress()
		{
			return new CommandFlag("--no-progress");
		}

		public static ICommandArgument Verbose()
		{
			return CommandFlag.Verbose();
		}

		public FsckCommand()
			: base("fsck")
		{
		}

		public FsckCommand(params ICommandArgument[] args)
			: base("fsck", args)
		{
		}

		public FsckCommand(IEnumerable<ICommandArgument> args)
			: base("fsck", args)
		{
		}
	}
}
