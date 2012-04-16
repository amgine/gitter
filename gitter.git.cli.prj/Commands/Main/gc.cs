namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Cleanup unnecessary files and optimize the local repository.</summary>
	public sealed class GcCommand : Command
	{
		public GcCommand()
			: base("gc")
		{
		}

		public GcCommand(params CommandArgument[] args)
			: base("gc", args)
		{
		}

		public GcCommand(IList<CommandArgument> args)
			: base("gc", args)
		{
		}
	}
}
