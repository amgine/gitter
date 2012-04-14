namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Find by binary search the change that introduced a bug.</summary>
	public sealed class BisectCommand : Command
	{
		public BisectCommand()
			: base("bisect")
		{
		}

		public BisectCommand(params CommandArgument[] args)
			: base("bisect", args)
		{
		}

		public BisectCommand(IList<CommandArgument> args)
			: base("bisect", args)
		{
		}
	}
}
