namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>List references in a local repository.</summary>
	public sealed class ShowRefCommand : Command
	{
		public static CommandArgument Head()
		{
			return new CommandArgument("--head");
		}

		public static CommandArgument Dereference()
		{
			return new CommandArgument("--dereference");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument Hash()
		{
			return new CommandArgument("--hash");
		}

		public static CommandArgument Hash(int n)
		{
			return new CommandArgument("--hash", n.ToString());
		}

		public static CommandArgument Heads()
		{
			return new CommandArgument("--heads");
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument Verify()
		{
			return new CommandArgument("--verify");
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public ShowRefCommand()
			: base("show-ref")
		{
		}

		public ShowRefCommand(params CommandArgument[] args)
			: base("show-ref", args)
		{
		}

		public ShowRefCommand(IList<CommandArgument> args)
			: base("show-ref", args)
		{
		}
	}
}
