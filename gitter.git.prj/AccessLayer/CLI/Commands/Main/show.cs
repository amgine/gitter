namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Show various types of objects.</summary>
	public sealed class ShowCommand : Command
	{
		public CommandArgument AbbrevCommit()
		{
			return new CommandArgument("--abbrev-commit");
		}

		public ShowCommand()
			: base("show")
		{
		}

		public ShowCommand(params CommandArgument[] args)
			: base("show", args)
		{
		}

		public ShowCommand(IList<CommandArgument> args)
			: base("show", args)
		{
		}
	}
}
