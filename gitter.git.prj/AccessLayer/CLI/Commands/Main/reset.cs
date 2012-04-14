namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Reset current HEAD to the specified state.</summary>
	public sealed class ResetCommand : Command
	{
		public static CommandArgument Mode(ResetMode mode)
		{
			switch(mode)
			{
				case ResetMode.Mixed:
					return ResetCommand.Mixed();
				case ResetMode.Soft:
					return ResetCommand.Soft();
				case ResetMode.Hard:
					return ResetCommand.Hard();
				case ResetMode.Merge:
					return ResetCommand.Merge();
				case ResetMode.Keep:
					return ResetCommand.Keep();
				default:
					throw new ArgumentException("Unknown ResetMode", "mode");
			}
		}

		/// <summary>
		/// Resets the index but not the working tree (i.e., the changed files are preserved but not marked for commit)
		/// and reports what has not been updated. This is the default action.
		/// </summary>
		public static CommandArgument Mixed()
		{
			return new CommandArgument("--mixed");
		}

		/// <summary>
		/// Does not touch the index file nor the working tree at all, but requires them to be in a good order.
		/// This leaves all your changed files "Changes to be committed", as git-status would put it
		/// </summary>
		public static CommandArgument Soft()
		{
			return new CommandArgument("--soft");
		}

		/// <summary>
		/// Matches the working tree and index to that of the tree being switched to. Any changes to
		/// tracked files in the working tree since "commit" are lost.
		/// </summary>
		public static CommandArgument Hard()
		{
			return new CommandArgument("--hard");
		}

		/// <summary>
		/// Resets the index to match the tree recorded by the named commit, and updates the
		/// files that are different between the named commit and the current commit in the working tree.
		/// </summary>
		public static CommandArgument Merge()
		{
			return new CommandArgument("--merge");
		}

		public static CommandArgument Keep()
		{
			return new CommandArgument("--keep");
		}

		/// <summary>Be quiet, only report errors.</summary>
		public static CommandArgument Quiet()
		{
			return new CommandArgument("-q");
		}

		/// <summary></summary>
		public static CommandArgument NoMoreOptions()
		{
			return new CommandArgument("--");
		}

		public ResetCommand()
			: base("reset")
		{
		}

		public ResetCommand(params CommandArgument[] args)
			: base("reset", args)
		{
		}

		public ResetCommand(IList<CommandArgument> args)
			: base("reset", args)
		{
		}
	}
}
