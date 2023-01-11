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
		=> new CommandParameterValue("--author", author.SurroundWithDoubleQuotes(), '=');

	public static ICommandArgument Date(DateTime date)
		=> new CommandParameterValue("--date", ((int)(date - GitConstants.UnixEraStart).TotalSeconds).ToString(
			System.Globalization.CultureInfo.InvariantCulture), '=');

	public static ICommandArgument Date(DateTimeOffset date)
		=> new CommandParameterValue("--date", date.ToUnixTimeSeconds().ToString(
			System.Globalization.CultureInfo.InvariantCulture), '=');

	public static ICommandArgument Message(string message)
		=> new CommandParameterValue("--message", message.SurroundWithDoubleQuotes(), '=');

	/// <summary>
	///	Take an existing commit object, and reuse the log message and the authorship information (including the timestamp)
	///	when creating the commit.
	/// </summary>
	public static ICommandArgument ReuseMessage(string commit)
		=> new CommandParameterValue("--reuse-message", commit);

	public static ICommandArgument ReeditMessage(string commit)
		=> new CommandParameterValue("--reedit-message", commit);

	public static ICommandArgument File(string file)
		=> new CommandParameterValue("--file", file.AssureDoubleQuotes());

	public static ICommandArgument Template(string file)
		=> new CommandParameterValue("--template", file);

	public static ICommandArgument Only()
		=> new CommandFlag("--only");

	public static ICommandArgument Include()
		=> new CommandFlag("--include");

	public static ICommandArgument SignOff()
		=> CommandFlag.SignOff;

	public static ICommandArgument NoVerify()
		=> new CommandFlag("--no-verify");

	public static ICommandArgument AllowEmpty()
		=> new CommandFlag("--allow-empty");

	public static ICommandArgument AllowEmptyMessage()
		=> new CommandFlag("--allow-empty-message");

	public static ICommandArgument Edit()
		=> new CommandFlag("--edit");

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
