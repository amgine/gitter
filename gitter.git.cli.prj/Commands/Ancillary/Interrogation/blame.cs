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

/// <summary>
/// Show what revision and author last modified each line of a file.
/// </summary>
/// <remarks>
/// <code>
/// <![CDATA[
/// git blame
///   [-c]
///   [-b]
///   [-l]
///   [--root]
///   [-t]
///   [-f]
///   [-n]
///   [-s]
///   [-e]
///   [-p]
///   [-w]
///   [--incremental]
///   [-L <range>]
///   [-S <revs-file>]
///   [-M]
///   [-C]
///   [-C]
///   [-C]
///   [--since=<date>]
///   [--ignore-rev <rev>]
///   [--ignore-revs-file <file>]
///   [--color-lines]
///   [--color-by-age]
///   [--progress]
///   [--abbrev=<n>]
///   [ --contents <file>]
///   [<rev> | --reverse <rev>..<rev>]
///   [--]
///   <file>
/// ]]>
/// </code>
/// </remarks>
sealed class BlameCommand : Command
{
	const string BlameCommandName = "blame";

	public static ICommandArgument Blank { get; } = new CommandFlag("-b");

	public static ICommandArgument Root { get; } = new CommandFlag("--root");

	public static ICommandArgument LongRev { get; } = new CommandFlag("-l");

	public static ICommandArgument RawTimestamp { get; } = new CommandFlag("-t");

	public static ICommandArgument ShowStats { get; } = new CommandFlag("--show-stats");

	public static ICommandArgument Porcelain { get; } = new CommandFlag("-p", "--porcelain");

	public static ICommandArgument LinePorcelain { get; } = new CommandFlag("--line-porcelain");

	public static ICommandArgument Incremental { get; } = new CommandFlag("--incremental");

	public static ICommandArgument IgnoreWhitespace { get; } = new CommandFlag("-w");

	public static ICommandArgument Progress { get; } = new CommandFlag("--progress");

	public static ICommandArgument NoProgress { get; } = new CommandFlag("--no-progress");

	public BlameCommand()
		: base(BlameCommandName)
	{
	}

	public BlameCommand(params ICommandArgument[] args)
		: base(BlameCommandName, args)
	{
	}

	public BlameCommand(IList<ICommandArgument> args)
		: base(BlameCommandName, args)
	{
	}
}
