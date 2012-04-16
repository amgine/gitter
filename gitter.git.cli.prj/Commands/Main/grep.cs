namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Print lines matching a pattern.</summary>
	public sealed class GrepCommand : Command
	{
		public GrepCommand()
			: base("grep")
		{
		}

		public GrepCommand(params CommandArgument[] args)
			: base("grep", args)
		{
		}

		public GrepCommand(IList<CommandArgument> args)
			: base("grep", args)
		{
		}
	}
}
