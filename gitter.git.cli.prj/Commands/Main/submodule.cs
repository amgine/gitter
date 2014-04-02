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
		public static ICommandArgument Add()
		{
			return new CommandParameter("add");
		}

		public static ICommandArgument Init()
		{
			return new CommandParameter("init");
		}

		public static ICommandArgument Update()
		{
			return new CommandParameter("update");
		}

		public static ICommandArgument Sync()
		{
			return new CommandParameter("sync");
		}

		public static ICommandArgument Foreach()
		{
			return new CommandParameter("foreach");
		}

		public static ICommandArgument Status()
		{
			return new CommandParameter("status");
		}

		public static ICommandArgument Summary()
		{
			return new CommandParameter("summary");
		}

		public static ICommandArgument InitFlag()
		{
			return new CommandFlag("--init");
		}

		public static ICommandArgument NoFetch()
		{
			return new CommandFlag("--no-fetch");
		}

		public static ICommandArgument Rebase()
		{
			return new CommandFlag("--rebase");
		}

		public static ICommandArgument Recursive()
		{
			return new CommandFlag("--recursive");
		}

		public static ICommandArgument Merge()
		{
			return new CommandFlag("--merge");
		}

		public static ICommandArgument Quiet()
		{
			return new CommandFlag("--quiet");
		}

		public static ICommandArgument Force()
		{
			return new CommandFlag("--force");
		}

		public static ICommandArgument Reference(string repository)
		{
			return new CommandParameterValue("--reference", repository, ' ');
		}

		public static ICommandArgument Branch(string name)
		{
			return new CommandParameterValue("-b" , name, ' ');
		}

		public static ICommandArgument NoMoreOptions()
		{
			return CommandFlag.NoMoreOptions();
		}

		public SubmoduleCommand()
			: base("submodule")
		{
		}

		public SubmoduleCommand(params ICommandArgument[] args)
			: base("submodule", args)
		{
		}

		public SubmoduleCommand(IList<ICommandArgument> args)
			: base("submodule", args)
		{
		}
	}
}
