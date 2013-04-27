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

	/// <summary>Initialize, update or inspect submodules.</summary>
	public sealed class SubmoduleCommand : Command
	{
		public static CommandArgument Add()
		{
			return new CommandArgument("add");
		}

		public static CommandArgument Init()
		{
			return new CommandArgument("init");
		}

		public static CommandArgument Update()
		{
			return new CommandArgument("update");
		}

		public static CommandArgument Sync()
		{
			return new CommandArgument("sync");
		}

		public static CommandArgument Foreach()
		{
			return new CommandArgument("foreach");
		}

		public static CommandArgument Status()
		{
			return new CommandArgument("status");
		}

		public static CommandArgument Summary()
		{
			return new CommandArgument("summary");
		}

		public static CommandArgument InitFlag()
		{
			return new CommandArgument("--init");
		}

		public static CommandArgument NoFetch()
		{
			return new CommandArgument("--no-fetch");
		}

		public static CommandArgument Rebase()
		{
			return new CommandArgument("--rebase");
		}

		public static CommandArgument Recursive()
		{
			return new CommandArgument("--recursive");
		}

		public static CommandArgument Merge()
		{
			return new CommandArgument("--merge");
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		public static CommandArgument Reference(string repository)
		{
			return new CommandArgument("--reference", repository, ' ');
		}

		public static CommandArgument Branch(string name)
		{
			return new CommandArgument("-b" , name, ' ');
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public SubmoduleCommand()
			: base("submodule")
		{
		}

		public SubmoduleCommand(params CommandArgument[] args)
			: base("submodule", args)
		{
		}

		public SubmoduleCommand(IList<CommandArgument> args)
			: base("submodule", args)
		{
		}
	}
}
