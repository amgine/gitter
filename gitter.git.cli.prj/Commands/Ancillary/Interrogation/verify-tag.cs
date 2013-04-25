namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Check the GPG signature of tags.</summary>
	sealed class VerifyTagCommand : Command
	{
		/// <summary>Print the contents of the tag object before validating it.</summary>
		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public VerifyTagCommand()
			: base("verify-tag")
		{
		}

		public VerifyTagCommand(params CommandArgument[] args)
			: base("verify-tag", args)
		{
		}

		public VerifyTagCommand(IEnumerable<CommandArgument> args)
			: base("verify-tag", args)
		{
		}
	}
}
