namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	public sealed class DiffIndexCommand : Command
	{
		public static CommandArgument NameStatus()
		{
			return new CommandArgument("--name-status");
		}

		public static CommandArgument Cached()
		{
			return new CommandArgument("--cached");
		}

		/// <summary>\0 line termination on output.</summary>
		public static CommandArgument ZeroLineTermination()
		{
			return new CommandArgument("-z");
		}

		public DiffIndexCommand()
			: base("diff-index")
		{
		}

		public DiffIndexCommand(params CommandArgument[] args)
			: base("diff-index", args)
		{
		}

		public DiffIndexCommand(IList<CommandArgument> args)
			: base("diff-index", args)
		{
		}
	}
}
