namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Move or rename a file, a directory, or a symlink.</summary>
	public sealed class MvCommand : Command
	{
		/// <summary>Force renaming or moving of a file even if the target exists.</summary>
		public static CommandArgument Force()
		{
			return new CommandArgument("-f");
		}

		/// <summary>
		/// Skip move or rename actions which would lead to an error condition. An error happens when
		/// a source is neither existing nor controlled by GIT, or when it would overwrite an existing
		/// file unless -f is given. 
		/// </summary>
		public static CommandArgument SkipErrors()
		{
			return new CommandArgument("-k");
		}

		/// <summary>Do nothing; only show what would happen.</summary>
		public static CommandArgument DryRun()
		{
			return CommandArgument.DryRun();
		}

		public MvCommand()
			: base("mv")
		{
		}

		public MvCommand(params CommandArgument[] args)
			: base("mv", args)
		{
		}

		public MvCommand(IList<CommandArgument> args)
			: base("mv", args)
		{
		}
	}
}
