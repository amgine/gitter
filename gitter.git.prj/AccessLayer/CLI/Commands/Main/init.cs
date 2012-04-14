namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
	public sealed class InitCommand : Command
	{
		public static CommandArgument Template(string template)
		{
			return new CommandArgument("--template", template, '=');
		}

		public static CommandArgument Shared(string permissions)
		{
			return new CommandArgument("--shared", permissions, '=');
		}

		public static CommandArgument Bare()
		{
			return new CommandArgument("--bare");
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public InitCommand()
			: base("init")
		{
		}

		public InitCommand(params CommandArgument[] args)
			: base("init", args)
		{
		}

		public InitCommand(IList<CommandArgument> args)
			: base("init", args)
		{
		}
	}
}
