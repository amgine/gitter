namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Manage set of tracked repositories.</summary>
	public sealed class RemoteCommand : Command
	{
		public static RemoteCommand FormatAddCommand(string name, string url, string master, string[] branches, bool mirror, bool fetch, bool tags)
		{
			var args = new CommandArgument[1+2+(master!=null?1:0)+(branches!=null?branches.Length:0)+(mirror?1:0)+(fetch?(tags?2:1):0)];
			int id = 0;
			args[id++] = Add();
			if(branches != null && branches.Length != 0)
			{
				foreach(var b in branches)
				{
					args[id++] = TrackBranch(b);
				}
			}
			if(!string.IsNullOrEmpty(master))
			{
				args[id++] = Master(master);
			}
			if(fetch)
			{
				args[id++] = Fetch();
				if(tags) args[id++] = Tags();
			}
			if(mirror)
			{
				args[id++] = Mirror();
			}
			args[id++] = new CommandArgument(name);
			args[id++] = new CommandArgument(url);
			return new RemoteCommand(args);
		}

		public static RemoteCommand FormatRemoveCommand(string name)
		{
			return new RemoteCommand(Remove(), new CommandArgument(name));
		}

		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public static CommandArgument Fetch()
		{
			return new CommandArgument("-f");
		}

		public static CommandArgument Cached()
		{
			return new CommandArgument("-n");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument NoTags()
		{
			return new CommandArgument("--no-tags");
		}

		public static CommandArgument Mirror()
		{
			return new CommandArgument("--mirror");
		}

		public static CommandArgument Master(string branch)
		{
			return new CommandArgument("-m", branch, ' ');
		}

		public static CommandArgument TrackBranch(string branch)
		{
			return new CommandArgument("-t", branch, ' ');
		}

		public static CommandArgument Show()
		{
			return new CommandArgument("show");
		}

		public static CommandArgument Add()
		{
			return new CommandArgument("add");
		}

		public static CommandArgument Remove()
		{
			return new CommandArgument("rm");
		}

		public static CommandArgument Rename()
		{
			return new CommandArgument("rename");
		}

		public static CommandArgument SetHead()
		{
			return new CommandArgument("set-head");
		}

		public static CommandArgument Prune()
		{
			return new CommandArgument("prune");
		}

		public static CommandArgument Update()
		{
			return new CommandArgument("update");
		}

		public static CommandArgument DryRun()
		{
			return new CommandArgument("--dry-run");
		}

		public RemoteCommand()
			: base("remote")
		{
		}

		public RemoteCommand(params CommandArgument[] args)
			: base("remote", args)
		{
		}

		public RemoteCommand(IList<CommandArgument> args)
			: base("remote", args)
		{
		}
	}
}
