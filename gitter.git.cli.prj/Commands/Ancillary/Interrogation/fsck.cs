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
			=> new CommandFlag("--tags");

		public static ICommandArgument Root()
			=> new CommandFlag("--root");

		public static ICommandArgument Unreachable()
			=> new CommandFlag("--unreachable");

		public static ICommandArgument Cache()
			=> new CommandFlag("--cache");

		public static ICommandArgument NoReflogs()
			=> new CommandFlag("--no-reflogs");

		public static ICommandArgument Full()
			=> new CommandFlag("--full");

		public static ICommandArgument NoFull()
			=> new CommandFlag("--no-full");

		public static ICommandArgument Strict()
			=> new CommandFlag("--strict");

		public static ICommandArgument LostFound()
			=> new CommandFlag("--lost-found");

		public static ICommandArgument Dangling()
			=> new CommandFlag("--dangling");

		public static ICommandArgument NoDangling()
			=> new CommandFlag("--no-dangling");

		public static ICommandArgument Progress()
			=> new CommandFlag("--progress");

		public static ICommandArgument NoProgress()
			=> new CommandFlag("--no-progress");

		public static ICommandArgument Verbose()
			=> CommandFlag.Verbose();

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
