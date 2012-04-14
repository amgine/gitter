namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Remove files from the working tree and from the index.</summary>
	public sealed class RmCommand : Command
	{
		public static CommandArgument Recursive()
		{
			return new CommandArgument("-r");
		}

		public static CommandArgument IgnoreUnmatch()
		{
			return new CommandArgument("--ignore-unmatch");
		}

		public static CommandArgument Cached()
		{
			return new CommandArgument("--cached");
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public static CommandArgument DryRun()
		{
			return CommandArgument.DryRun();
		}

		public RmCommand()
			: base("rm")
		{
		}

		public RmCommand(params CommandArgument[] args)
			: base("rm", args)
		{
		}

		public RmCommand(IList<CommandArgument> args)
			: base("rm", args)
		{
		}
	}
}
