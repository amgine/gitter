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

namespace gitter.Git.AccessLayer.CLI;

using System.Collections.Generic;

/// <summary>Initialize, update or inspect submodules.</summary>
public sealed class SubmoduleCommand : Command
{
	public static ICommandArgument Add()
		=> new CommandParameter("add");

	public static ICommandArgument Init()
		=> new CommandParameter("init");

	public static ICommandArgument Update()
		=> new CommandParameter("update");

	public static ICommandArgument Sync()
		=> new CommandParameter("sync");

	public static ICommandArgument Foreach()
		=> new CommandParameter("foreach");

	public static ICommandArgument Status()
		=> new CommandParameter("status");

	public static ICommandArgument Summary()
		=> new CommandParameter("summary");

	public static ICommandArgument InitFlag()
		=> new CommandFlag("--init");

	public static ICommandArgument NoFetch()
		=> new CommandFlag("--no-fetch");

	public static ICommandArgument Rebase()
		=> new CommandFlag("--rebase");

	public static ICommandArgument Recursive()
		=> new CommandFlag("--recursive");

	public static ICommandArgument Merge()
		=> new CommandFlag("--merge");

	public static ICommandArgument Quiet()
		=> new CommandFlag("--quiet");

	public static ICommandArgument Force()
		=> new CommandFlag("--force");

	public static ICommandArgument Reference(string repository)
		=> new CommandParameterValue("--reference", repository, ' ');

	public static ICommandArgument Branch(string name)
		=> new CommandParameterValue("-b" , name, ' ');

	public static ICommandArgument NoMoreOptions()
		=> CommandFlag.NoMoreOptions();

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
