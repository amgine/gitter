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
		public static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		public static CommandArgument Always()
		{
			return new CommandArgument("--always");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument Contains()
		{
			return new CommandArgument("--contains");
		}

		public static CommandArgument Abbrev(int n)
		{
			return new CommandArgument("--abbrev", n.ToString());
		}

		public static CommandArgument Candidates(int n)
		{
			return new CommandArgument("--candidates", n.ToString());
		}

		public static CommandArgument ExactMatch()
		{
			return new CommandArgument("--exact-match");
		}

		public static CommandArgument Long()
		{
			return new CommandArgument("--long");
		}

		public static CommandArgument Debug()
		{
			return new CommandArgument("--debug");
		}

		public static CommandArgument Match(string pattern)
		{
			return new CommandArgument("--match", pattern, ' ');
		}

		public DescribeCommand()
			: base("describe")
		{
		}

		public DescribeCommand(params CommandArgument[] args)
			: base("describe", args)
		{
		}

		public DescribeCommand(IList<CommandArgument> args)
			: base("describe", args)
		{
		}
	}
}
