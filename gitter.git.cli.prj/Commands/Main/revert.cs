namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Revert an existing commit.</summary>
	public sealed class RevertCommand : Command
	{
		public static CommandArgument Mainline(int n)
		{
			return new CommandArgument("--mainline", n.ToString(), ' ');
		}

		public static CommandArgument Edit()
		{
			return new CommandArgument("--edit");
		}

		public static CommandArgument NoEdit()
		{
			return new CommandArgument("--no-edit");
		}

		public static CommandArgument NoCommit()
		{
			return new CommandArgument("--no-commit");
		}

		public static CommandArgument SignOff()
		{
			return new CommandArgument("--signoff");
		}

		public RevertCommand()
			: base("revert")
		{
		}

		public RevertCommand(params CommandArgument[] args)
			: base("revert", args)
		{
		}

		public RevertCommand(IList<CommandArgument> args)
			: base("revert", args)
		{
		}
	}
}
