namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Fetch from and merge with another repository or a local branch.</summary>
	public sealed class PullCommand : Command
	{
		public PullCommand()
			: base("pull")
		{
		}

		public PullCommand(params CommandArgument[] args)
			: base("pull", args)
		{
		}

		public PullCommand(IList<CommandArgument> args)
			: base("pull", args)
		{
		}
	}
}
