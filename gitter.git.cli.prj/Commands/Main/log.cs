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
	using System.Globalization;

	/// <summary>Show commit logs.</summary>
	public sealed class LogCommand : Command
	{
		public static ICommandArgument All()
		{
			return new CommandFlag("--all");
		}

		public static ICommandArgument Not()
		{
			return new CommandFlag("--not");
		}

		public static ICommandArgument Branches(string pattern)
		{
			return new CommandParameterValue("--branches", pattern, '=');
		}

		public static ICommandArgument Tags(string pattern)
		{
			return new CommandParameterValue("--tags", pattern, '=');
		}

		public static ICommandArgument Glob(string pattern)
		{
			return new CommandParameterValue("--glob", pattern, '=');
		}

		public static ICommandArgument Remotes(string pattern)
		{
			return new CommandParameterValue("--remotes", pattern, '=');
		}

		public static ICommandArgument Author(string pattern)
		{
			return new CommandParameterValue("--author", pattern, '=');
		}

		public static ICommandArgument SimplifyByDecoration()
		{
			return new CommandFlag("--simplify-by-decoration");
		}

		public static ICommandArgument Committer(string pattern)
		{
			return new CommandParameterValue("--committer", pattern, '=');
		}

		public static ICommandArgument Grep(string pattern)
		{
			return new CommandParameterValue("--grep", pattern, '=');
		}

		public static ICommandArgument RegexpIgnoreCase()
		{
			return new CommandFlag("--regexp-ignore-case");
		}

		public static ICommandArgument ExtendedRegexp()
		{
			return new CommandFlag("--extended-regexp");
		}

		public static ICommandArgument FixedStrings()
		{
			return new CommandFlag("--fixed-strings");
		}

		public static ICommandArgument AllMatch()
		{
			return new CommandFlag("--all-match");
		}

		/// <summary>Follow only the first parent commit upon seeing a merge commit.</summary>
		public static ICommandArgument FirstParent()
		{
			return new CommandFlag("--first-parent");
		}

		/// <summary>Print also the parents of the commit (in the form "commit parent..."). Also enables parent rewriting.</summary>
		public static ICommandArgument Parents()
		{
			return new CommandFlag("--parents");
		}

		/// <summary>Print only merge commits.</summary>
		public static ICommandArgument Merges()
		{
			return new CommandFlag("--merges");
		}

		/// <summary>Do not print commits with more than one parent.</summary>
		public static ICommandArgument NoMerges()
		{
			return new CommandFlag("--no-merges");
		}

		public static ICommandArgument Graph()
		{
			return new CommandFlag("--graph");
		}

		public static ICommandArgument WalkReflogs()
		{
			return new CommandFlag("--walk-reflogs");
		}

		/// <summary>Continue listing the history of a file beyond renames (works only for a single file).</summary>
		public static ICommandArgument Follow()
		{
			return new CommandFlag("--follow");
		}

		/// <summary>Stop when a given path disappears from the tree.</summary>
		public static ICommandArgument RemoveEmpty()
		{
			return new CommandFlag("--remove-empty");
		}

		public static ICommandArgument MaxCount(int limit)
		{
			return new CommandParameterValue("--max-count", limit.ToString(CultureInfo.InvariantCulture));
		}

		public static ICommandArgument Skip(int skip)
		{
			return new CommandParameterValue("--skip", skip.ToString(CultureInfo.InvariantCulture));
		}

		public static ICommandArgument Format(string format)
		{
			return new CommandParameterValue("--format", "format:\"" + format + "\"", '=');
		}

		public static ICommandArgument FormatRaw()
		{
			return new CommandParameterValue("--format", "raw", '=');
		}

		public static ICommandArgument TFormat(string format)
		{
			return new CommandParameterValue("--format", "tformat:\"" + format + "\"");
		}

		public static ICommandArgument OneLine()
		{
			return new CommandFlag("--oneline");
		}

		public static ICommandArgument Reverse()
		{
			return new CommandFlag("--reverse");
		}

		public static ICommandArgument TopoOrder()
		{
			return new CommandFlag("--topo-order");
		}

		public static ICommandArgument DateOrder()
		{
			return new CommandFlag("--date-order");
		}

		public static ICommandArgument Since(DateTime dateTime)
		{
			return new CommandParameterValue("--since", dateTime.ToString(CultureInfo.InvariantCulture));
		}

		public static ICommandArgument Until(DateTime dateTime)
		{
			return new CommandParameterValue("--until", dateTime.ToString(CultureInfo.InvariantCulture));
		}

		public static ICommandArgument NullTerminate()
		{
			return new CommandFlag("-z");
		}

		public LogCommand()
			: base("log")
		{
		}

		public LogCommand(params ICommandArgument[] args)
			: base("log", args)
		{
		}

		public LogCommand(IList<ICommandArgument> args)
			: base("log", args)
		{
		}
	}
}
