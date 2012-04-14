namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>List the contents of a tree object.</summary>
	public sealed class LsTreeCommand : Command
	{
		public static CommandArgument Directories()
		{
			return new CommandArgument("-d");
		}

		public static CommandArgument Recurse()
		{
			return new CommandArgument("-r");
		}

		public static CommandArgument Tree()
		{
			return new CommandArgument("-t");
		}

		public static CommandArgument Long()
		{
			return new CommandArgument("--long");
		}

		public static CommandArgument NameOnly()
		{
			return new CommandArgument("--name-only");
		}

		public static CommandArgument FullName()
		{
			return new CommandArgument("--full-name");
		}

		public static CommandArgument FullTree()
		{
			return new CommandArgument("--full-tree");
		}

		public static CommandArgument Abbrev()
		{
			return new CommandArgument("--abbrev");
		}

		public static CommandArgument Abbrev(int n)
		{
			return new CommandArgument("--abbrev", n.ToString(System.Globalization.CultureInfo.InvariantCulture), '=');
		}

		public static CommandArgument NullTerminate()
		{
			return new CommandArgument("-z");
		}

		public LsTreeCommand()
			: base("ls-tree")
		{
		}

		public LsTreeCommand(params CommandArgument[] args)
			: base("ls-tree", args)
		{
		}

		public LsTreeCommand(IList<CommandArgument> args)
			: base("ls-tree", args)
		{
		}
	}
}
