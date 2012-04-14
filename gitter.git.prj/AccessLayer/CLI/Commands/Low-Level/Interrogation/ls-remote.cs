namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>List references in a remote repository.</summary>
	public sealed class LsRemoteCommand : Command
	{
		public static LsRemoteCommand FormatGetBranchesCommand(string remote)
		{
			return new LsRemoteCommand(
				Heads(),
				new CommandArgument(remote),
				new CommandArgument("/refs/heads/*"));
		}

		public static CommandArgument Heads()
		{
			return new CommandArgument("--heads");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument UploadPack(string path)
		{
			return new CommandArgument("--upload-pack", path, '=');
		}

		public LsRemoteCommand()
			: base("ls-remote")
		{
		}

		public LsRemoteCommand(params CommandArgument[] args)
			: base("ls-remote", args)
		{
		}

		public LsRemoteCommand(IList<CommandArgument> args)
			: base("ls-remote", args)
		{
		}
	}
}
