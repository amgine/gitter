namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Run merge conflict resolution tools to resolve merge conflicts.</summary>
	public sealed class MergeToolCommand : Command
	{
		public static CommandArgument Tool(string tool)
		{
			return new CommandArgument("--tool", tool, '=');
		}

		public static CommandArgument Prompt()
		{
			return new CommandArgument("--prompt");
		}

		public static CommandArgument NoPrompt()
		{
			return new CommandArgument("--no-prompt");
		}

		public MergeToolCommand()
			: base("mergetool")
		{
		}

		public MergeToolCommand(params CommandArgument[] args)
			: base("mergetool", args)
		{
		}

		public MergeToolCommand(IList<CommandArgument> args)
			: base("mergetool", args)
		{
		}
	}
}
