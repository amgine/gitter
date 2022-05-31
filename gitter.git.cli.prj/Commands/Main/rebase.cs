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

/// <summary>Forward-port local commits to the updated upstream head.</summary>
public sealed class RebaseCommand : Command
{
	public static ICommandArgument Abort()
		=> new CommandFlag("--abort");

	public static ICommandArgument Skip()
		=> new CommandFlag("--skip");

	public static ICommandArgument Continue()
		=> new CommandFlag("--continue");

	public static ICommandArgument Onto(string branch)
		=> new CommandParameterValue("--onto", branch, ' ');

	public static ICommandArgument Root()
		=> new CommandFlag("--root");

	public static ICommandArgument Interactive()
		=> new CommandFlag("--interactive");

	public static ICommandArgument Merge()
		=> new CommandFlag("--merge");

	public static ICommandArgument Strategy(string strategy)
		=> new CommandParameterValue("--strategy", strategy, '=');

	public static ICommandArgument StrategyOption(string strategyOption)
		=> new CommandParameterValue("--strategy-option", strategyOption, '=');

	public static ICommandArgument Quiet()
		=> new CommandFlag("--quiet");

	public static ICommandArgument Verbose()
		=> new CommandFlag("--verbose");

	public static ICommandArgument Stat()
		=> new CommandFlag("--stat");

	public static ICommandArgument NoStat()
		=> new CommandFlag("--no-stat");

	public static ICommandArgument Force()
		=> new CommandFlag("--force-rebase");

	public static ICommandArgument IgnoreWhitespace()
		=> new CommandFlag("--ignore-whitespace");

	public static ICommandArgument PreserveMerges()
		=> new CommandFlag("--preserve-merges");

	public static ICommandArgument Autosquash()
		=> new CommandFlag("--autosquash");

	public static ICommandArgument NoAutosquash()
		=> new CommandFlag("--no-autosquash");

	public static ICommandArgument NoFF()
		=> new CommandFlag("--no-ff");

	public RebaseCommand()
		: base("rebase")
	{
	}

	public RebaseCommand(params ICommandArgument[] args)
		: base("rebase", args)
	{
	}

	public RebaseCommand(IList<ICommandArgument> args)
		: base("rebase", args)
	{
	}
}
