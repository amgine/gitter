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

/// <summary>Record changes to the repository.</summary>
/// <remarks>
/// <code>
/// <![CDATA[
/// git commit
///   [-a | --interactive | --patch]
///   [-s]
///   [-v]
///   [-u[<mode>]]
///   [--amend]
///   [--dry-run] <commit>]
///   [-F <file> | -m <msg>]
///   [--reset-author]
///   [--allow-empty]
///   [--allow-empty-message]
///   [--no-verify]
///   [-e]
///   [--author=<author>]
///   [--date=<date>]
///   [--cleanup=<mode>]
///   [--[no-]status]
///   [-i | -o]
///   [--pathspec-from-file=<file> [--pathspec-file-nul]]
///   [(--trailer <token>[(=|:)<value>])...​] [-S[<keyid>]]
///   [--]
///   [<pathspec>...]
/// ]]>
/// </code>
/// </remarks>
public sealed class CommitCommand : Command
{
	/// <summary>
	///	Tell the command to automatically stage files that have been modified and deleted, but new files you have not
	///	told git about are not affected.
	/// </summary>
	public static ICommandArgument All()
		=> new CommandFlag("--all");

	/// <summary>
	///	Used to amend the tip of the current branch. Prepare the tree object you would want to replace the latest
	///	commit as usual (this includes the usual -i/-o and explicit paths), and the commit log editor is seeded with
	///	the commit message from the tip of the current branch. The commit you create replaces the current tip —
	///	if it was a merge, it will have the parents of the current tip as parents — so the current top commit is discarded. 
	/// </summary>
	public static ICommandArgument Amend()
		=> new CommandFlag("--amend");

	public static ICommandArgument ResetAuthor()
		=> new CommandFlag("--reset-author");

	public static ICommandArgument Author(string author)
		=> new CommandParameterQuotedValue("--author", author, '=');

	public static ICommandArgument Date(DateTime date)
		=> new CommandParameterLongValue("--date", (long)(date - GitConstants.UnixEraStart).TotalSeconds, '=');

	public static ICommandArgument Date(DateTimeOffset date)
		=> new CommandParameterLongValue("--date", date.ToUnixTimeSeconds(), '=');

	public static ICommandArgument Message(string message)
		=> new CommandParameterQuotedValue("--message", message, '=');

	/// <summary>
	///	Take an existing commit object, and reuse the log message and the authorship information (including the timestamp)
	///	when creating the commit.
	/// </summary>
	public static ICommandArgument ReuseMessage(string commit)
		=> new CommandParameterValue("--reuse-message", commit);

	public static ICommandArgument ReeditMessage(string commit)
		=> new CommandParameterValue("--reedit-message", commit);

	public static ICommandArgument File(string file)
		=> new CommandParameterPathValue("--file", file);

	public static ICommandArgument Template(string file)
		=> new CommandParameterPathValue("--template", file);

	public static ICommandArgument Only()
		=> new CommandFlag("--only");

	public static ICommandArgument Include()
		=> new CommandFlag("--include");

	public static ICommandArgument SignOff()
		=> CommandFlag.SignOff;

	public static ICommandArgument NoVerify()
		=> new CommandFlag("-n", "--no-verify");

	public static ICommandArgument AllowEmpty()
		=> new CommandFlag("--allow-empty");

	public static ICommandArgument AllowEmptyMessage()
		=> new CommandFlag("--allow-empty-message");

	public static ICommandArgument Edit()
		=> new CommandFlag("-e", "--edit");

	public static ICommandArgument Status()
		=> new CommandFlag("--status");

	public static ICommandArgument NoStatus()
		=> new CommandFlag("--no-status");

	public static ICommandArgument NoPostRewrite { get; } = new CommandFlag("--no-post-rewrite");

	private static ICommandArgument CleanupDefault    { get; } = new CommandParameterValue("--cleanup", "default");
	private static ICommandArgument CleanupScissors   { get; } = new CommandParameterValue("--cleanup", "scissors");
	private static ICommandArgument CleanupVerbatim   { get; } = new CommandParameterValue("--cleanup", "verbatim");
	private static ICommandArgument CleanupWhitespace { get; } = new CommandParameterValue("--cleanup", "whitespace");
	private static ICommandArgument CleanupStrip      { get; } = new CommandParameterValue("--cleanup", "strip");

	public static ICommandArgument Cleanup(CommitMessageCleanupMode mode)
		=> mode switch
		{
			CommitMessageCleanupMode.Default    => CleanupDefault,
			CommitMessageCleanupMode.Scissors   => CleanupScissors,
			CommitMessageCleanupMode.Verbatim   => CleanupVerbatim,
			CommitMessageCleanupMode.Whitespace => CleanupWhitespace,
			CommitMessageCleanupMode.Strip      => CleanupStrip,
			_ => throw new ArgumentException($"Unknown mode: {mode}", nameof(mode)),
		};

	public static ICommandArgument Verbose()
		=> CommandFlag.Verbose;

	public static ICommandArgument Quiet()
		=> CommandFlag.Quiet;

	public static ICommandArgument NoMoreOptions()
		=> CommandFlag.NoMoreOptions;

	public CommitCommand()
		: base("commit")
	{
	}

	public CommitCommand(params ICommandArgument[] args)
		: base("commit", args)
	{
	}

	public CommitCommand(IList<ICommandArgument> args)
		: base("commit", args)
	{
	}
}
