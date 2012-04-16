namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	sealed class BlameCommand : Command
	{
		public static CommandArgument Porcelain()
		{
			return new CommandArgument("--porcelain");
		}

		public BlameCommand()
			: base("blame")
		{
		}

		public BlameCommand(params CommandArgument[] args)
			: base("blame", args)
		{
		}

		public BlameCommand(IList<CommandArgument> args)
			: base("blame", args)
		{
		}
	}
}
