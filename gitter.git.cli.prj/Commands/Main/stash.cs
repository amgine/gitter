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

/// <summary>Stash the changes in a dirty working directory away.</summary>
public sealed class StashCommand : Command
{
	public static ICommandArgument Save()
		=> new CommandParameter("save");

	public static ICommandArgument List()
		=> new CommandParameter("list");

	public static ICommandArgument Show()
		=> new CommandParameter("show");

	public static ICommandArgument Pop()
		=> new CommandParameter("pop");

	public static ICommandArgument Apply()
		=> new CommandParameter("apply");

	public static ICommandArgument Branch()
		=> new CommandParameter("branch");

	public static ICommandArgument Clear()
		=> new CommandParameter("clear");

	public static ICommandArgument Drop()
		=> new CommandParameter("drop");

	public static ICommandArgument Create()
		=> new CommandParameter("create");

	public static ICommandArgument NoKeepIndex()
		=> new CommandFlag("--no-keep-index");

	public static ICommandArgument KeepIndex()
		=> new CommandFlag("--keep-index");

	public static ICommandArgument Index()
		=> new CommandFlag("--index");

	public static ICommandArgument IncludeUntracked()
		=> new CommandFlag("--include-untracked");

	public static ICommandArgument Quiet()
		=> CommandFlag.Quiet;

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
