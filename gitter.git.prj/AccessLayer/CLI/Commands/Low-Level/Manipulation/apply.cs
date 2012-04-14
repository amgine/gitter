namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>List references in a local repository.</summary>
	public sealed class ApplyCommand : Command
	{
		public static CommandArgument Cached()
		{
			return new CommandArgument("--cached");
		}

		public static CommandArgument Index()
		{
			return new CommandArgument("--index");
		}

		public static CommandArgument Check()
		{
			return new CommandArgument("--check");
		}

		public static CommandArgument Reverse()
		{
			return new CommandArgument("--reverse");
		}

		public static CommandArgument FromStdin()
		{
			return new CommandArgument("-");
		}

		public static CommandArgument Whitespace(string action)
		{
			return new CommandArgument("--whitespace", action, '=');
		}

		public static CommandArgument UnidiffZero()
		{
			return new CommandArgument("--unidiff-zero");
		}

		public ApplyCommand()
			: base("apply")
		{
		}

		public ApplyCommand(params CommandArgument[] args)
			: base("apply", args)
		{
		}

		public ApplyCommand(IList<CommandArgument> args)
			: base("apply", args)
		{
		}
	}
}
