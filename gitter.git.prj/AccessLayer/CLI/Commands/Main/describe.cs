namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Show the most recent tag that is reachable from a commit.</summary>
	public sealed class DescribeCommand : Command
	{
		public static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		public static CommandArgument Always()
		{
			return new CommandArgument("--always");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument Contains()
		{
			return new CommandArgument("--contains");
		}

		public static CommandArgument Abbrev(int n)
		{
			return new CommandArgument("--abbrev", n.ToString());
		}

		public static CommandArgument Candidates(int n)
		{
			return new CommandArgument("--candidates", n.ToString());
		}

		public static CommandArgument ExactMatch()
		{
			return new CommandArgument("--exact-match");
		}

		public static CommandArgument Long()
		{
			return new CommandArgument("--long");
		}

		public static CommandArgument Debug()
		{
			return new CommandArgument("--debug");
		}

		public static CommandArgument Match(string pattern)
		{
			return new CommandArgument("--match", pattern, ' ');
		}

		public DescribeCommand()
			: base("describe")
		{
		}

		public DescribeCommand(params CommandArgument[] args)
			: base("describe", args)
		{
		}

		public DescribeCommand(IList<CommandArgument> args)
			: base("describe", args)
		{
		}
	}
}
