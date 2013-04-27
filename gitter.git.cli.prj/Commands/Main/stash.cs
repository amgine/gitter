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
		public static CommandArgument Save()
		{
			return new CommandArgument("save");
		}

		public static CommandArgument List()
		{
			return new CommandArgument("list");
		}

		public static CommandArgument Show()
		{
			return new CommandArgument("show");
		}

		public static CommandArgument Pop()
		{
			return new CommandArgument("pop");
		}

		public static CommandArgument Apply()
		{
			return new CommandArgument("apply");
		}

		public static CommandArgument Branch()
		{
			return new CommandArgument("branch");
		}

		public static CommandArgument Clear()
		{
			return new CommandArgument("clear");
		}

		public static CommandArgument Drop()
		{
			return new CommandArgument("drop");
		}

		public static CommandArgument Create()
		{
			return new CommandArgument("create");
		}

		public static CommandArgument NoKeepIndex()
		{
			return new CommandArgument("--no-keep-index");
		}

		public static CommandArgument KeepIndex()
		{
			return new CommandArgument("--keep-index");
		}

		public static CommandArgument Index()
		{
			return new CommandArgument("--index");
		}

		public static CommandArgument IncludeUntracked()
		{
			return new CommandArgument("--include-untracked");
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public StashCommand()
			: base("stash")
		{
		}

		public StashCommand(params CommandArgument[] args)
			: base("stash", args)
		{
		}

		public StashCommand(IList<CommandArgument> args)
			: base("stash", args)
		{
		}
	}
}
