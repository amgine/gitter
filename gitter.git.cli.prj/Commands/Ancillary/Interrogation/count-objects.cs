namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	sealed class CountObjectsCommand : Command
	{
		public static CommandArgument Verbose()
		{
			return new CommandArgument("--verbose");
		}

		public CountObjectsCommand()
			: base("count-objects")
		{
		}

		public CountObjectsCommand(params CommandArgument[] args)
			: base("count-objects", args)
		{
		}

		public CountObjectsCommand(IEnumerable<CommandArgument> args)
			: base("count-objects", args)
		{
		}
	}
}
