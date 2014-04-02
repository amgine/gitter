#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Manage set of tracked repositories.</summary>
	public sealed class RemoteCommand : Command
	{
		public static RemoteCommand FormatAddCommand(string name, string url, string master, string[] branches, bool mirror, bool fetch, bool tags)
		{
			var args = new ICommandArgument[1+2+(master!=null?1:0)+(branches!=null?branches.Length:0)+(mirror?1:0)+(fetch?(tags?2:1):0)];
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
			args[id++] = new CommandParameter(name);
			args[id++] = new CommandParameter(url);
			return new RemoteCommand(args);
		}

		public static RemoteCommand FormatRemoveCommand(string name)
		{
			return new RemoteCommand(Remove(), new CommandParameter(name));
		}

		public static ICommandArgument Verbose()
		{
			return CommandFlag.Verbose();
		}

		public static ICommandArgument Fetch()
		{
			return new CommandFlag("-f");
		}

		public static ICommandArgument Cached()
		{
			return new CommandFlag("-n");
		}

		public static ICommandArgument Tags()
		{
			return new CommandFlag("--tags");
		}

		public static ICommandArgument NoTags()
		{
			return new CommandFlag("--no-tags");
		}

		public static ICommandArgument Mirror()
		{
			return new CommandFlag("--mirror");
		}

		public static ICommandArgument Master(string branch)
		{
			return new CommandParameterValue("-m", branch, ' ');
		}

		public static ICommandArgument TrackBranch(string branch)
		{
			return new CommandParameterValue("-t", branch, ' ');
		}

		public static ICommandArgument Show()
		{
			return new CommandParameter("show");
		}

		public static ICommandArgument Add()
		{
			return new CommandParameter("add");
		}

		public static ICommandArgument Remove()
		{
			return new CommandParameter("rm");
		}

		public static ICommandArgument Rename()
		{
			return new CommandParameter("rename");
		}

		public static ICommandArgument SetHead()
		{
			return new CommandParameter("set-head");
		}

		public static ICommandArgument Prune()
		{
			return new CommandParameter("prune");
		}

		public static ICommandArgument Update()
		{
			return new CommandParameter("update");
		}

		public static ICommandArgument DryRun()
		{
			return new CommandFlag("--dry-run");
		}

		public RemoteCommand()
			: base("remote")
		{
		}

		public RemoteCommand(params ICommandArgument[] args)
			: base("remote", args)
		{
		}

		public RemoteCommand(IList<ICommandArgument> args)
			: base("remote", args)
		{
		}
	}
}
