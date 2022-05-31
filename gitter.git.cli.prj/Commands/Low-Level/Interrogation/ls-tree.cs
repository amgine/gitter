﻿#region Copyright Notice
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

/// <summary>List the contents of a tree object.</summary>
public sealed class LsTreeCommand : Command
{
	public static ICommandArgument Directories()
		=> new CommandFlag("-d");

	public static ICommandArgument Recurse()
		=> new CommandFlag("-r");

	public static ICommandArgument Tree()
		=> new CommandFlag("-t");

	public static ICommandArgument Long()
		=> new CommandFlag("--long");

	public static ICommandArgument NameOnly()
		=> new CommandFlag("--name-only");

	public static ICommandArgument FullName()
		=> new CommandFlag("--full-name");

	public static ICommandArgument FullTree()
		=> new CommandFlag("--full-tree");

	public static ICommandArgument Abbrev()
		=> new CommandFlag("--abbrev");

	public static ICommandArgument Abbrev(int n)
		=> new CommandParameterValue("--abbrev", n.ToString(System.Globalization.CultureInfo.InvariantCulture), '=');

	public static ICommandArgument NullTerminate()
		=> new CommandFlag("-z");

	public LsTreeCommand()
		: base("ls-tree")
	{
	}

	public LsTreeCommand(params ICommandArgument[] args)
		: base("ls-tree", args)
	{
	}

	public LsTreeCommand(IList<ICommandArgument> args)
		: base("ls-tree", args)
	{
	}
}
