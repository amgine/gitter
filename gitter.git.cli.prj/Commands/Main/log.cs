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

	/// <summary>Show commit logs.</summary>
	public sealed class LogCommand : Command
	{
		public static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		public static CommandArgument Not()
		{
			return new CommandArgument("--not");
		}

		public static CommandArgument Branches(string pattern)
		{
			return new CommandArgument("--branches", pattern, '=');
		}

		public static CommandArgument Tags(string pattern)
		{
			return new CommandArgument("--tags", pattern, '=');
		}

		public static CommandArgument Glob(string pattern)
		{
			return new CommandArgument("--glob", pattern, '=');
		}

		public static CommandArgument Remotes(string pattern)
		{
			return new CommandArgument("--remotes", pattern, '=');
		}

		public static CommandArgument Author(string pattern)
		{
			return new CommandArgument("--author", pattern, '=');
		}

		public static CommandArgument SimplifyByDecoration()
		{
			return new CommandArgument("--simplify-by-decoration");
		}

		public static CommandArgument Committer(string pattern)
		{
			return new CommandArgument("--committer", pattern, '=');
		}

		public static CommandArgument Grep(string pattern)
		{
			return new CommandArgument("--grep", pattern, '=');
		}

		public static CommandArgument RegexpIgnoreCase()
		{
			return new CommandArgument("--regexp-ignore-case");
		}

		public static CommandArgument ExtendedRegexp()
		{
			return new CommandArgument("--extended-regexp");
		}

		public static CommandArgument FixedStrings()
		{
			return new CommandArgument("--fixed-strings");
		}

		public static CommandArgument AllMatch()
		{
			return new CommandArgument("--all-match");
		}

		/// <summary>Follow only the first parent commit upon seeing a merge commit.</summary>
		public static CommandArgument FirstParent()
		{
			return new CommandArgument("--first-parent");
		}

		/// <summary>Print also the parents of the commit (in the form "commit parent..."). Also enables parent rewriting.</summary>
		public static CommandArgument Parents()
		{
			return new CommandArgument("--parents");
		}

		/// <summary>Print only merge commits.</summary>
		public static CommandArgument Merges()
		{
			return new CommandArgument("--merges");
		}

		/// <summary>Do not print commits with more than one parent.</summary>
		public static CommandArgument NoMerges()
		{
			return new CommandArgument("--no-merges");
		}

		public static CommandArgument Graph()
		{
			return new CommandArgument("--graph");
		}

		public static CommandArgument WalkReflogs()
		{
			return new CommandArgument("--walk-reflogs");
		}

		/// <summary>Continue listing the history of a file beyond renames (works only for a single file).</summary>
		public static CommandArgument Follow()
		{
			return new CommandArgument("--follow");
		}

		/// <summary>Stop when a given path disappears from the tree.</summary>
		public static CommandArgument RemoveEmpty()
		{
			return new CommandArgument("--remove-empty");
		}

		public static CommandArgument MaxCount(int limit)
		{
			return new CommandArgument("--max-count", limit.ToString());
		}

		public static CommandArgument Skip(int skip)
		{
			return new CommandArgument("--skip", skip.ToString());
		}

		public static CommandArgument Format(string format)
		{
			return new CommandArgument("--format", "format:\"" + format + "\"", '=');
		}

		public static CommandArgument FormatRaw()
		{
			return new CommandArgument("--format", "raw", '=');
		}

		public static CommandArgument TFormat(string format)
		{
			return new CommandArgument("--format", "tformat:\"" + format + "\"");
		}

		public static CommandArgument OneLine()
		{
			return new CommandArgument("--oneline");
		}

		public static CommandArgument Reverse()
		{
			return new CommandArgument("--reverse");
		}

		public static CommandArgument TopoOrder()
		{
			return new CommandArgument("--topo-order");
		}

		public static CommandArgument DateOrder()
		{
			return new CommandArgument("--date-order");
		}

		public static CommandArgument Since(DateTime dateTime)
		{
			return new CommandArgument("--since", dateTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		public static CommandArgument Until(DateTime dateTime)
		{
			return new CommandArgument("--until", dateTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		public static CommandArgument NullTerminate()
		{
			return new CommandArgument("-z");
		}

		public LogCommand()
			: base("log")
		{
		}

		public LogCommand(params CommandArgument[] args)
			: base("log", args)
		{
		}

		public LogCommand(IList<CommandArgument> args)
			: base("log", args)
		{
		}
	}
}
