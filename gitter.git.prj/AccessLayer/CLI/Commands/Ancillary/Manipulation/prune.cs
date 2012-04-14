namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Prune all unreachable objects from the object database.</summary>
	public sealed class PruneCommand : Command
	{
		public PruneCommand()
			: base("prune")
		{
		}

		public PruneCommand(params CommandArgument[] args)
			: base("prune", args)
		{
		}

		public PruneCommand(IList<CommandArgument> args)
			: base("prune", args)
		{
		}
	}
}
