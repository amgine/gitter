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

using System;
using System.Collections.Generic;

/// <summary>Join two or more development histories together.</summary>
public sealed class MergeCommand : Command
{
	public static ICommandArgument Stat()
		=> new CommandFlag("--stat");

	public static ICommandArgument NoStat()
		=> new CommandFlag("--no-stat");

	public static ICommandArgument Log()
		=> new CommandFlag("--log");

	public static ICommandArgument NoLog()
		=> new CommandFlag("--no-log");

	public static ICommandArgument Commit()
		=> new CommandFlag("--commit");

	public static ICommandArgument NoCommit()
		=> new CommandFlag("--no-commit");

	public static ICommandArgument Squash()
		=> new CommandFlag("--squash");

	public static ICommandArgument NoSquash()
		=> new CommandFlag("--no-squash");

	public static ICommandArgument FastForward()
		=> new CommandFlag("--ff");

	public static ICommandArgument NoFastForward()
		=> new CommandFlag("--no-ff");

	public static ICommandArgument Message(string msg)
		=> new CommandParameterValue("-m", "\"" + msg + "\"", ' ');

	public static ICommandArgument Strategy(string strategy)
		=> new CommandParameterValue("--strategy", strategy, '=');

	public static ICommandArgument Strategy(MergeStrategy strategy)
		=> strategy switch
		{
			MergeStrategy.Octopus   => new CommandParameterValue("--strategy", "octopus", '='),
			MergeStrategy.Ours      => new CommandParameterValue("--strategy", "ours", '='),
			MergeStrategy.Recursive => new CommandParameterValue("--strategy", "recursive", '='),
			MergeStrategy.Resolve   => new CommandParameterValue("--strategy", "resolve", '='),
			MergeStrategy.Subtree   => new CommandParameterValue("--strategy", "subtree", '='),
			_ => null,
		};

	public static ICommandArgument StrategyOption(string option)
		=> new CommandParameterValue("--strategy-option", option, '=');

	public static ICommandArgument Quiet()
		=> new CommandFlag("--quiet");

	public static ICommandArgument Verbose()
		=> new CommandFlag("--verbose");

	public MergeCommand()
		: base("merge")
	{
	}

	public MergeCommand(params ICommandArgument[] args)
		: base("merge", args)
	{
	}

	public MergeCommand(IList<ICommandArgument> args)
		: base("merge", args)
	{
	}
}
