namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Move objects and refs by archive.</summary>
	public sealed class BundleCommand : Command
	{
		public BundleCommand()
			: base("bundle")
		{
		}

		public BundleCommand(params CommandArgument[] args)
			: base("bundle", args)
		{
		}

		public BundleCommand(IList<CommandArgument> args)
			: base("bundle", args)
		{
		}
	}
}
