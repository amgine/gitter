namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Stash the changes in a dirty working directory away.</summary>
	public sealed class StashCommand : Command
	{
		public static CommandArgument Save()
		{
			return new CommandArgument("save");
		}

		public static CommandArgument List()
		{
			return new CommandArgument("list");
		}

		public static CommandArgument Show()
		{
			return new CommandArgument("show");
		}

		public static CommandArgument Pop()
		{
			return new CommandArgument("pop");
		}

		public static CommandArgument Apply()
		{
			return new CommandArgument("apply");
		}

		public static CommandArgument Branch()
		{
			return new CommandArgument("branch");
		}

		public static CommandArgument Clear()
		{
			return new CommandArgument("clear");
		}

		public static CommandArgument Drop()
		{
			return new CommandArgument("drop");
		}

		public static CommandArgument Create()
		{
			return new CommandArgument("create");
		}

		public static CommandArgument NoKeepIndex()
		{
			return new CommandArgument("--no-keep-index");
		}

		public static CommandArgument KeepIndex()
		{
			return new CommandArgument("--keep-index");
		}

		public static CommandArgument Index()
		{
			return new CommandArgument("--index");
		}

		public static CommandArgument IncludeUntracked()
		{
			return new CommandArgument("--include-untracked");
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public StashCommand()
			: base("stash")
		{
		}

		public StashCommand(params CommandArgument[] args)
			: base("stash", args)
		{
		}

		public StashCommand(IList<CommandArgument> args)
			: base("stash", args)
		{
		}
	}
}
