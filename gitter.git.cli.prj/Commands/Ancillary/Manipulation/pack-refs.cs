namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Pack heads and tags for efficient repository access.</summary>
	sealed class PackRefsCommand : Command
	{
		/// <summary>
		/// The command by default packs all tags and refs that are already packed, and leaves other refs alone.
		/// This is because branches are expected to be actively developed and packing their tips does not help performance.
		/// This option causes branch tips to be packed as well. Useful for a repository with many branches of historical interests.
		/// </summary>
		public static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		/// <summary>
		/// The command usually removes loose refs under $GIT_DIR/refs hierarchy after packing them.
		/// This option tells it not to.
		/// </summary>
		public static CommandArgument NoPrune()
		{
			return new CommandArgument("--no-prune");
		}

		public PackRefsCommand()
			: base("pack-refs")
		{
		}

		public PackRefsCommand(params CommandArgument[] args)
			: base("pack-refs", args)
		{
		}

		public PackRefsCommand(IEnumerable<CommandArgument> args)
			: base("pack-refs", args)
		{
		}
	}
}
