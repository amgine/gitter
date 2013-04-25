namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	sealed class FsckCommand : Command
	{
		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument Root()
		{
			return new CommandArgument("--root");
		}

		public static CommandArgument Unreachable()
		{
			return new CommandArgument("--unreachable");
		}

		public static CommandArgument Cache()
		{
			return new CommandArgument("--cache");
		}

		public static CommandArgument NoReflogs()
		{
			return new CommandArgument("--no-reflogs");
		}

		public static CommandArgument Full()
		{
			return new CommandArgument("--full");
		}

		public static CommandArgument NoFull()
		{
			return new CommandArgument("--no-full");
		}

		public static CommandArgument Strict()
		{
			return new CommandArgument("--strict");
		}

		public static CommandArgument LostFound()
		{
			return new CommandArgument("--lost-found");
		}

		public static CommandArgument Dangling()
		{
			return new CommandArgument("--dangling");
		}

		public static CommandArgument NoDangling()
		{
			return new CommandArgument("--no-dangling");
		}

		public static CommandArgument Progress()
		{
			return new CommandArgument("--progress");
		}

		public static CommandArgument NoProgress()
		{
			return new CommandArgument("--no-progress");
		}

		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public FsckCommand()
			: base("fsck")
		{
		}

		public FsckCommand(params CommandArgument[] args)
			: base("fsck", args)
		{
		}

		public FsckCommand(IEnumerable<CommandArgument> args)
			: base("fsck", args)
		{
		}
	}
}
