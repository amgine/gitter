namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Summarize 'git-log' output.</summary>
	public sealed class ShortLogCommand : Command
	{
		internal static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		/// <summary>Suppress commit description and provide a commit count summary only.</summary>
		public static CommandArgument Summary()
		{
			return new CommandArgument("--summary");
		}

		/// <summary>Sort output according to the number of commits per author instead of author alphabetic order.</summary>
		public static CommandArgument Numbered()
		{
			return new CommandArgument("--numbered");
		}

		/// <summary>Show the email address of each author.</summary>
		public static CommandArgument Email()
		{
			return new CommandArgument("--email");
		}

		public ShortLogCommand()
			: base("shortlog")
		{
		}

		public ShortLogCommand(params CommandArgument[] args)
			: base("shortlog", args)
		{
		}

		public ShortLogCommand(IList<CommandArgument> args)
			: base("shortlog", args)
		{
		}
	}
}
