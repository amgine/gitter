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

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>Show commit logs.</summary>
public sealed class LogCommand : Command
{
	public static ICommandArgument All()
		=> new CommandFlag("--all");

	public static ICommandArgument Not()
		=> new CommandFlag("--not");

	public static ICommandArgument Branches(string pattern)
		=> new CommandParameterValue("--branches", pattern, '=');

	public static ICommandArgument Tags(string pattern)
		=> new CommandParameterValue("--tags", pattern, '=');

	public static ICommandArgument Glob(string pattern)
		=> new CommandParameterValue("--glob", pattern, '=');

	public static ICommandArgument Remotes(string pattern)
		=> new CommandParameterValue("--remotes", pattern, '=');

	public static ICommandArgument Author(string pattern)
		=> new CommandParameterValue("--author", pattern, '=');

	public static ICommandArgument SimplifyByDecoration()
		=> new CommandFlag("--simplify-by-decoration");

	public static ICommandArgument Committer(string pattern)
		=> new CommandParameterValue("--committer", pattern, '=');

	public static ICommandArgument Grep(string pattern)
		=> new CommandParameterValue("--grep", pattern, '=');

	public static ICommandArgument RegexpIgnoreCase()
		=> new CommandFlag("--regexp-ignore-case");

	public static ICommandArgument ExtendedRegexp()
		=> new CommandFlag("--extended-regexp");

	public static ICommandArgument FixedStrings()
		=> new CommandFlag("--fixed-strings");

	public static ICommandArgument AllMatch()
		=> new CommandFlag("--all-match");

	/// <summary>Follow only the first parent commit upon seeing a merge commit.</summary>
	public static ICommandArgument FirstParent()
		=> new CommandFlag("--first-parent");

	/// <summary>Print also the parents of the commit (in the form "commit parent..."). Also enables parent rewriting.</summary>
	public static ICommandArgument Parents()
		=> new CommandFlag("--parents");

	/// <summary>Print only merge commits.</summary>
	public static ICommandArgument Merges()
		=> new CommandFlag("--merges");

	/// <summary>Do not print commits with more than one parent.</summary>
	public static ICommandArgument NoMerges()
		=> new CommandFlag("--no-merges");

	public static ICommandArgument Graph()
		=> new CommandFlag("--graph");

	public static ICommandArgument WalkReflogs()
		=> new CommandFlag("--walk-reflogs");

	/// <summary>Continue listing the history of a file beyond renames (works only for a single file).</summary>
	public static ICommandArgument Follow()
		=> new CommandFlag("--follow");

	/// <summary>Stop when a given path disappears from the tree.</summary>
	public static ICommandArgument RemoveEmpty()
		=> new CommandFlag("--remove-empty");

	public static ICommandArgument MaxCount(int limit)
		=> new CommandParameterValue("--max-count", limit.ToString(CultureInfo.InvariantCulture));

	public static ICommandArgument Skip(int skip)
		=> new CommandParameterValue("--skip", skip.ToString(CultureInfo.InvariantCulture));

	public static ICommandArgument Format(string format)
		=> new CommandParameterValue("--format", "format:\"" + format + "\"", '=');

	public static ICommandArgument FormatRaw()
		=> new CommandParameterValue("--format", "raw", '=');

	public static ICommandArgument TFormat(string format)
		=> new CommandParameterValue("--format", "tformat:\"" + format + "\"");

	public static ICommandArgument OneLine()
		=> new CommandFlag("--oneline");

	public static ICommandArgument Reverse()
		=> new CommandFlag("--reverse");

	public static ICommandArgument TopoOrder()
		=> new CommandFlag("--topo-order");

	public static ICommandArgument DateOrder()
		=> new CommandFlag("--date-order");

	public static ICommandArgument Since(DateTime dateTime)
		=> new CommandParameterValue("--since", dateTime.ToString(CultureInfo.InvariantCulture));

	public static ICommandArgument Until(DateTime dateTime)
		=> new CommandParameterValue("--until", dateTime.ToString(CultureInfo.InvariantCulture));

	public static ICommandArgument NullTerminate()
		=> new CommandFlag("-z");

	public LogCommand()
		: base("log")
	{
	}

	public LogCommand(params ICommandArgument[] args)
		: base("log", args)
	{
	}

	public LogCommand(IEnumerable<ICommandArgument> args)
		: base("log", args)
	{
	}
}
