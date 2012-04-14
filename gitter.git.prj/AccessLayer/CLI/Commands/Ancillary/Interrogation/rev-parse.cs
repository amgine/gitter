namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Get and set repository or global options.</summary>
	public sealed class RevParseCommand : Command
	{
		public static CommandArgument GitDir()
		{
			return new CommandArgument("--git-dir");
		}

		public RevParseCommand()
			: base("rev-parse")
		{
		}

		public RevParseCommand(params CommandArgument[] args)
			: base("rev-parse", args)
		{
		}

		public RevParseCommand(IList<CommandArgument> args)
			: base("rev-parse", args)
		{
		}
	}
}
