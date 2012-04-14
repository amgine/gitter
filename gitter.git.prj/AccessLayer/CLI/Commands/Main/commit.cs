namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Record changes to the repository.</summary>
	public sealed class CommitCommand : Command
	{
		/// <summary>
		///	Tell the command to automatically stage files that have been modified and deleted, but new files you have not
		///	told git about are not affected.
		/// </summary>
		public static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		/// <summary>
		///	Used to amend the tip of the current branch. Prepare the tree object you would want to replace the latest
		///	commit as usual (this includes the usual -i/-o and explicit paths), and the commit log editor is seeded with
		///	the commit message from the tip of the current branch. The commit you create replaces the current tip —
		///	if it was a merge, it will have the parents of the current tip as parents — so the current top commit is discarded. 
		/// </summary>
		public static CommandArgument Amend()
		{
			return new CommandArgument("--amend");
		}

		public static CommandArgument ResetAuthor()
		{
			return new CommandArgument("--reset-author");
		}

		public static CommandArgument Author(string author)
		{
			return new CommandArgument("--author", author.SurroundWithDoubleQuotes(), '=');
		}

		public static CommandArgument Date(DateTime date)
		{
			return new CommandArgument("--date", ((int)(date - GitConstants.UnixEraStart).TotalSeconds).ToString(
				System.Globalization.CultureInfo.InvariantCulture), '=');
		}

		public static CommandArgument Message(string message)
		{
			return new CommandArgument("--message", message.SurroundWithDoubleQuotes(), '=');
		}

		/// <summary>
		///	Take an existing commit object, and reuse the log message and the authorship information (including the timestamp)
		///	when creating the commit.
		/// </summary>
		public static CommandArgument ReuseMessage(string commit)
		{
			return new CommandArgument("--reuse-message", commit);
		}

		public static CommandArgument ReeditMessage(string commit)
		{
			return new CommandArgument("--reedit-message", commit);
		}

		public static CommandArgument File(string file)
		{
			return new CommandArgument("--file", file);
		}

		public static CommandArgument Template(string file)
		{
			return new CommandArgument("--template", file);
		}

		public static CommandArgument Only()
		{
			return new CommandArgument("--only");
		}

		public static CommandArgument Include()
		{
			return new CommandArgument("--include");
		}

		public static CommandArgument SignOff()
		{
			return CommandArgument.SignOff();
		}

		public static CommandArgument NoVerify()
		{
			return new CommandArgument("--no-verify");
		}

		public static CommandArgument AllowEmpty()
		{
			return new CommandArgument("--allow-empty");
		}

		public static CommandArgument AllowEmptyMessage()
		{
			return new CommandArgument("--allow-empty-message");
		}

		public static CommandArgument Edit()
		{
			return new CommandArgument("--edit");
		}

		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public static CommandArgument NoMoreOptions()
		{
			return new CommandArgument("--");
		}

		public CommitCommand()
			: base("commit")
		{
		}

		public CommitCommand(params CommandArgument[] args)
			: base("commit", args)
		{
		}

		public CommitCommand(IList<CommandArgument> args)
			: base("commit", args)
		{
		}
	}
}
