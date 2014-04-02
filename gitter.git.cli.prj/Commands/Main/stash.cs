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

	/// <summary>Stash the changes in a dirty working directory away.</summary>
	public sealed class StashCommand : Command
	{
		public static ICommandArgument Save()
		{
			return new CommandParameter("save");
		}

		public static ICommandArgument List()
		{
			return new CommandParameter("list");
		}

		public static ICommandArgument Show()
		{
			return new CommandParameter("show");
		}

		public static ICommandArgument Pop()
		{
			return new CommandParameter("pop");
		}

		public static ICommandArgument Apply()
		{
			return new CommandParameter("apply");
		}

		public static ICommandArgument Branch()
		{
			return new CommandParameter("branch");
		}

		public static ICommandArgument Clear()
		{
			return new CommandParameter("clear");
		}

		public static ICommandArgument Drop()
		{
			return new CommandParameter("drop");
		}

		public static ICommandArgument Create()
		{
			return new CommandParameter("create");
		}

		public static ICommandArgument NoKeepIndex()
		{
			return new CommandFlag("--no-keep-index");
		}

		public static ICommandArgument KeepIndex()
		{
			return new CommandFlag("--keep-index");
		}

		public static ICommandArgument Index()
		{
			return new CommandFlag("--index");
		}

		public static ICommandArgument IncludeUntracked()
		{
			return new CommandFlag("--include-untracked");
		}

		public static ICommandArgument Quiet()
		{
			return CommandFlag.Quiet();
		}

		public StashCommand()
			: base("stash")
		{
		}

		public StashCommand(params ICommandArgument[] args)
			: base("stash", args)
		{
		}

		public StashCommand(IList<ICommandArgument> args)
			: base("stash", args)
		{
		}
	}
}
