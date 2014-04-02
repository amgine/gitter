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

	/// <summary>Show the most recent tag that is reachable from a commit.</summary>
	public sealed class DescribeCommand : Command
	{
		public static ICommandArgument All()
		{
			return new CommandFlag("--all");
		}

		public static ICommandArgument Always()
		{
			return new CommandFlag("--always");
		}

		public static ICommandArgument Tags()
		{
			return new CommandFlag("--tags");
		}

		public static ICommandArgument Contains()
		{
			return new CommandFlag("--contains");
		}

		public static ICommandArgument Abbrev(int n)
		{
			return new CommandParameterValue("--abbrev", n.ToString());
		}

		public static ICommandArgument Candidates(int n)
		{
			return new CommandParameterValue("--candidates", n.ToString());
		}

		public static ICommandArgument ExactMatch()
		{
			return new CommandFlag("--exact-match");
		}

		public static ICommandArgument Long()
		{
			return new CommandFlag("--long");
		}

		public static ICommandArgument Debug()
		{
			return new CommandFlag("--debug");
		}

		public static ICommandArgument Match(string pattern)
		{
			return new CommandParameterValue("--match", pattern, ' ');
		}

		public DescribeCommand()
			: base("describe")
		{
		}

		public DescribeCommand(params ICommandArgument[] args)
			: base("describe", args)
		{
		}

		public DescribeCommand(IList<ICommandArgument> args)
			: base("describe", args)
		{
		}
	}
}
