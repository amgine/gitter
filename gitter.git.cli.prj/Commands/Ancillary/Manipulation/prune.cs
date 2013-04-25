namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	/// <summary>Prune all unreachable objects from the object database.</summary>
	sealed class PruneCommand : Command
	{
		public static CommandArgument DryRun()
		{
			return CommandArgument.DryRun();
		}

		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public static CommandArgument Expire(DateTime expire)
		{
			return new CommandArgument("--expire", Utility.FormatDate(expire, DateFormat.UnixTimestamp), ' ');
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public PruneCommand()
			: base("prune")
		{
		}

		public PruneCommand(params CommandArgument[] args)
			: base("prune", args)
		{
		}

		public PruneCommand(IList<CommandArgument> args)
			: base("prune", args)
		{
		}
	}
}
