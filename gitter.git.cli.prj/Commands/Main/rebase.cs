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
		public static CommandArgument Abort()
		{
			return new CommandArgument("--abort");
		}

		public static CommandArgument Skip()
		{
			return new CommandArgument("--skip");
		}

		public static CommandArgument Continue()
		{
			return new CommandArgument("--continue");
		}

		public static CommandArgument Onto(string branch)
		{
			return new CommandArgument("--onto", branch, ' ');
		}

		public static CommandArgument Root()
		{
			return new CommandArgument("--root");
		}

		public static CommandArgument Interactive()
		{
			return new CommandArgument("--interactive");
		}

		public static CommandArgument Merge()
		{
			return new CommandArgument("--merge");
		}

		public static CommandArgument Strategy(string strategy)
		{
			return new CommandArgument("--strategy", strategy, '=');
		}

		public static CommandArgument StrategyOption(string strategyOption)
		{
			return new CommandArgument("--strategy-option", strategyOption, '=');
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument Verbose()
		{
			return new CommandArgument("--verbose");
		}

		public static CommandArgument Stat()
		{
			return new CommandArgument("--stat");
		}

		public static CommandArgument NoStat()
		{
			return new CommandArgument("--no-stat");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force-rebase");
		}

		public static CommandArgument IgnoreWhitespace()
		{
			return new CommandArgument("--ignore-whitespace");
		}

		public static CommandArgument PreserveMerges()
		{
			return new CommandArgument("--preserve-merges");
		}

		public static CommandArgument Autosquash()
		{
			return new CommandArgument("--autosquash");
		}

		public static CommandArgument NoAutosquash()
		{
			return new CommandArgument("--no-autosquash");
		}

		public static CommandArgument NoFF()
		{
			return new CommandArgument("--no-ff");
		}

		public RebaseCommand()
			: base("rebase")
		{
		}

		public RebaseCommand(params CommandArgument[] args)
			: base("rebase", args)
		{
		}

		public RebaseCommand(IList<CommandArgument> args)
			: base("rebase", args)
		{
		}
	}
}
