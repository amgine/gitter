namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Prepare patches for e-mail submission.</summary>
	public sealed class FormatPatchCommand : Command
	{
		public FormatPatchCommand()
			: base("format-patch")
		{
		}

		public FormatPatchCommand(params CommandArgument[] args)
			: base("format-patch", args)
		{
		}

		public FormatPatchCommand(IList<CommandArgument> args)
			: base("format-patch", args)
		{
		}
	}
}
