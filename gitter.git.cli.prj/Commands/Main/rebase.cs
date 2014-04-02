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

	/// <summary>Forward-port local commits to the updated upstream head.</summary>
	public sealed class RebaseCommand : Command
	{
		public static ICommandArgument Abort()
		{
			return new CommandFlag("--abort");
		}

		public static ICommandArgument Skip()
		{
			return new CommandFlag("--skip");
		}

		public static ICommandArgument Continue()
		{
			return new CommandFlag("--continue");
		}

		public static ICommandArgument Onto(string branch)
		{
			return new CommandParameterValue("--onto", branch, ' ');
		}

		public static ICommandArgument Root()
		{
			return new CommandFlag("--root");
		}

		public static ICommandArgument Interactive()
		{
			return new CommandFlag("--interactive");
		}

		public static ICommandArgument Merge()
		{
			return new CommandFlag("--merge");
		}

		public static ICommandArgument Strategy(string strategy)
		{
			return new CommandParameterValue("--strategy", strategy, '=');
		}

		public static ICommandArgument StrategyOption(string strategyOption)
		{
			return new CommandParameterValue("--strategy-option", strategyOption, '=');
		}

		public static ICommandArgument Quiet()
		{
			return new CommandFlag("--quiet");
		}

		public static ICommandArgument Verbose()
		{
			return new CommandFlag("--verbose");
		}

		public static ICommandArgument Stat()
		{
			return new CommandFlag("--stat");
		}

		public static ICommandArgument NoStat()
		{
			return new CommandFlag("--no-stat");
		}

		public static ICommandArgument Force()
		{
			return new CommandFlag("--force-rebase");
		}

		public static ICommandArgument IgnoreWhitespace()
		{
			return new CommandFlag("--ignore-whitespace");
		}

		public static ICommandArgument PreserveMerges()
		{
			return new CommandFlag("--preserve-merges");
		}

		public static ICommandArgument Autosquash()
		{
			return new CommandFlag("--autosquash");
		}

		public static ICommandArgument NoAutosquash()
		{
			return new CommandFlag("--no-autosquash");
		}

		public static ICommandArgument NoFF()
		{
			return new CommandFlag("--no-ff");
		}

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
}
