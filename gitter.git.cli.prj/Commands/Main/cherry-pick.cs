namespace gitter.Git.AccessLayer.CLI
{
	using System.Globalization;
	using System.Collections.Generic;

	/// <summary>Apply the change introduced by an existing commit.</summary>
	public sealed class CherryPickCommand : Command
	{
		public static CommandArgument Edit()
		{
			return new CommandArgument("--edit");
		}

		public static CommandArgument Mainline(int number)
		{
			return new CommandArgument("--mainline", number.ToString(CultureInfo.InvariantCulture), ' ');
		}

		public static CommandArgument NoCommit()
		{
			return new CommandArgument("--no-commit");
		}

		public static CommandArgument SignOff()
		{
			return new CommandArgument("--signoff");
		}

		public static CommandArgument FastForward()
		{
			return new CommandArgument("--ff");
		}

		public static CommandArgument AllowEmpty()
		{
			return new CommandArgument("--allow-empty");
		}

		public static CommandArgument AllowEmptyMessage()
		{
			return new CommandArgument("--allow-empty-message");
		}

		public static CommandArgument KeepRedundantCommits()
		{
			return new CommandArgument("--keep-redundant-commits");
		}

		public static CommandArgument Continue()
		{
			return new CommandArgument("--continue");
		}

		public static CommandArgument Quit()
		{
			return new CommandArgument("--quit");
		}

		public static CommandArgument Abort()
		{
			return new CommandArgument("--abort");
		}

		public CherryPickCommand()
			: base("cherry-pick")
		{
		}

		public CherryPickCommand(params CommandArgument[] args)
			: base("cherry-pick", args)
		{
		}

		public CherryPickCommand(IList<CommandArgument> args)
			: base("cherry-pick", args)
		{
		}
	}
}
