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

/// <summary>Register file contents in the working tree to the index.</summary>
public sealed class UpdateIndexCommand : Command
{
	public static ICommandArgument Add()
		=> new CommandFlag("--add");

	public static ICommandArgument Remove()
		=> new CommandFlag("--remove");

	public static ICommandArgument Refresh()
		=> new CommandFlag("--refresh");

	public static ICommandArgument Quiet()
		=> new CommandFlag("-q");

	public static ICommandArgument IgnoreSubmodules()
		=> new CommandFlag("--ignore-submodules");

	public static ICommandArgument Unmerged()
		=> new CommandFlag("--unmerged");

	public static ICommandArgument IgnoreMissing()
		=> new CommandFlag("--ignore-missing");

	public static ICommandArgument IndexInfo()
		=> new CommandFlag("--index-info");

	public static ICommandArgument AssumeUnchanged()
		=> new CommandFlag("--assume-unchanged");

	public static ICommandArgument NoAssumeUnchanged()
		=> new CommandFlag("--no-assume-unchanged");

	public static ICommandArgument Again()
		=> new CommandFlag("--again");

	public static ICommandArgument Unresolve()
		=> new CommandFlag("--unresolve");

	public static ICommandArgument InfoOnly()
		=> new CommandFlag("--info-only");

	public static ICommandArgument ForceRemove()
		=> new CommandFlag("--force-remove");

	public static ICommandArgument Replace()
		=> new CommandFlag("--replace");

	public static ICommandArgument Stdin()
		=> new CommandFlag("--stdin");

	public static ICommandArgument NullTerminate()
		=> new CommandFlag("-z");

	public static ICommandArgument Verbose()
		=> CommandFlag.Verbose;

	public static ICommandArgument NoMoreOptions()
		=> CommandFlag.NoMoreOptions;

	public UpdateIndexCommand()
		: base("update-index")
	{
	}

	public UpdateIndexCommand(params ICommandArgument[] args)
		: base("update-index", args)
	{
	}

	public UpdateIndexCommand(IList<ICommandArgument> args)
		: base("update-index", args)
	{
	}
}
