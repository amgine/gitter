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
	using System;
	using System.Collections.Generic;

	/// <summary>Join two or more development histories together.</summary>
	public sealed class MergeCommand : Command
	{
		public static ICommandArgument Stat()
		{
			return new CommandFlag("--stat");
		}

		public static ICommandArgument NoStat()
		{
			return new CommandFlag("--no-stat");
		}

		public static ICommandArgument Log()
		{
			return new CommandFlag("--log");
		}

		public static ICommandArgument NoLog()
		{
			return new CommandFlag("--no-log");
		}

		public static ICommandArgument Commit()
		{
			return new CommandFlag("--commit");
		}

		public static ICommandArgument NoCommit()
		{
			return new CommandFlag("--no-commit");
		}

		public static ICommandArgument Squash()
		{
			return new CommandFlag("--squash");
		}

		public static ICommandArgument NoSquash()
		{
			return new CommandFlag("--no-squash");
		}

		public static ICommandArgument FastForward()
		{
			return new CommandFlag("--ff");
		}

		public static ICommandArgument NoFastForward()
		{
			return new CommandFlag("--no-ff");
		}

		public static ICommandArgument Message(string msg)
		{
			return new CommandParameterValue("-m", "\"" + msg + "\"", ' ');
		}

		public static ICommandArgument Strategy(string strategy)
		{
			return new CommandParameterValue("--strategy", strategy, '=');
		}

		public static ICommandArgument Strategy(MergeStrategy strategy)
		{
			switch(strategy)
			{
				case MergeStrategy.Octopus:
					return new CommandParameterValue("--strategy", "octopus", '=');
				case MergeStrategy.Ours:
					return new CommandParameterValue("--strategy", "ours", '=');
				case MergeStrategy.Recursive:
					return new CommandParameterValue("--strategy", "recursive", '=');
				case MergeStrategy.Resolve:
					return new CommandParameterValue("--strategy", "resolve", '=');
				case MergeStrategy.Subtree:
					return new CommandParameterValue("--strategy", "subtree", '=');
				default:
					return null;
			}
		}

		public static ICommandArgument StrategyOption(string option)
		{
			return new CommandParameterValue("--strategy-option", option, '=');
		}

		public static ICommandArgument Quiet()
		{
			return new CommandFlag("--quiet");
		}

		public static ICommandArgument Verbose()
		{
			return new CommandFlag("--verbose");
		}

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
}
