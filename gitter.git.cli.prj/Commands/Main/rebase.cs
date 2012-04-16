namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Forward-port local commits to the updated upstream head.</summary>
	public sealed class RebaseCommand : Command
	{
		public static CommandArgument Abort()
		{
			return new CommandArgument("--abort");
		}

		public static CommandArgument Skip()
		{
			return new CommandArgument("--skip");
		}

		public static CommandArgument Continue()
		{
			return new CommandArgument("--continue");
		}

		public static CommandArgument Onto(string branch)
		{
			return new CommandArgument("--onto", branch, ' ');
		}

		public static CommandArgument Root()
		{
			return new CommandArgument("--root");
		}

		public static CommandArgument Interactive()
		{
			return new CommandArgument("--interactive");
		}

		public static CommandArgument Merge()
		{
			return new CommandArgument("--merge");
		}

		public static CommandArgument Strategy(string strategy)
		{
			return new CommandArgument("--strategy", strategy, '=');
		}

		public static CommandArgument StrategyOption(string strategyOption)
		{
			return new CommandArgument("--strategy-option", strategyOption, '=');
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument Verbose()
		{
			return new CommandArgument("--verbose");
		}

		public static CommandArgument Stat()
		{
			return new CommandArgument("--stat");
		}

		public static CommandArgument NoStat()
		{
			return new CommandArgument("--no-stat");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force-rebase");
		}

		public static CommandArgument IgnoreWhitespace()
		{
			return new CommandArgument("--ignore-whitespace");
		}

		public static CommandArgument PreserveMerges()
		{
			return new CommandArgument("--preserve-merges");
		}

		public static CommandArgument Autosquash()
		{
			return new CommandArgument("--autosquash");
		}

		public static CommandArgument NoAutosquash()
		{
			return new CommandArgument("--no-autosquash");
		}

		public static CommandArgument NoFF()
		{
			return new CommandArgument("--no-ff");
		}

		public RebaseCommand()
			: base("rebase")
		{
		}

		public RebaseCommand(params CommandArgument[] args)
			: base("rebase", args)
		{
		}

		public RebaseCommand(IList<CommandArgument> args)
			: base("rebase", args)
		{
		}
	}
}
