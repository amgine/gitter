namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Apply the change introduced by an existing commit.</summary>
	public sealed class CherryPickCommand : Command
	{
		public static CommandArgument NoCommit()
		{
			return new CommandArgument("--no-commit");
		}

		public CherryPickCommand()
			: base("cherry-pick")
		{
		}

		public CherryPickCommand(params CommandArgument[] args)
			: base("cherry-pick", args)
		{
		}

		public CherryPickCommand(IList<CommandArgument> args)
			: base("cherry-pick", args)
		{
		}
	}
}
