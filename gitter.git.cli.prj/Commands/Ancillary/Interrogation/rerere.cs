namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Reuse recorded resolution of conflicted merges.</summary>
	/// <remarks>You need to set the configuration variable rerere.enabled in order to enable this command.</remarks>
	sealed class RerereCommand : Command
	{
		public static CommandArgument Clear()
		{
			return new CommandArgument("clear");
		}

		public static CommandArgument Forget(string pathspec)
		{
			return new CommandArgument("forget", pathspec, ' ');
		}

		public static CommandArgument Diff()
		{
			return new CommandArgument("diff");
		}

		public static CommandArgument Remaining()
		{
			return new CommandArgument("remaining");
		}

		public static CommandArgument Status()
		{
			return new CommandArgument("status");
		}

		public static CommandArgument Gc()
		{
			return new CommandArgument("gc");
		}

		public RerereCommand()
			: base("rerere")
		{
		}

		public RerereCommand(params CommandArgument[] args)
			: base("rerere", args)
		{
		}

		public RerereCommand(IEnumerable<CommandArgument> args)
			: base("rerere", args)
		{
		}
	}
}
