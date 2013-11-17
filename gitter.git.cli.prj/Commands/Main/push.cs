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
	using System;
	using System.Collections.Generic;

	/// <summary>Update remote refs along with associated objects.</summary>
	public sealed class PushCommand : Command
	{
		public static PushCommand FormatPushCommand(string remote, ICollection<string> branches, bool force, bool thin, bool tags)
		{
			var args = new CommandArgument[branches.Count + 3 + (force?1:0) + (tags?1:0)];
			int arg = 0;
			if(tags)
			{
				args[arg++] = Tags();
			}
			if(force)
			{
				args[arg++] = Force();
			}
			args[arg++] = thin ? Thin() : NoThin();
			args[arg++] = Porcelain();
			args[arg++] = new CommandArgument(remote);
			foreach(var branch in branches)
			{
				args[arg++] = new CommandArgument(branch);
			}
			return new PushCommand(args);
		}

		public static PushCommand FormatRemoveReferenceCommand(string remote, string reference)
		{
			return new PushCommand(
				Porcelain(),
				new CommandArgument(remote),
				new CommandArgument(":" + reference));
		}

		public static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		public static CommandArgument Mirror()
		{
			return new CommandArgument("--mirror");
		}

		public static CommandArgument Delete()
		{
			return new CommandArgument("--delete");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument Thin()
		{
			return new CommandArgument("--thin");
		}

		public static CommandArgument NoThin()
		{
			return new CommandArgument("--no-thin");
		}

		public static CommandArgument Porcelain()
		{
			return new CommandArgument("--porcelain");
		}

		public static CommandArgument Progress()
		{
			return new CommandArgument("--progress");
		}

		public static CommandArgument SetUpstream()
		{
			return new CommandArgument("--set-upstream");
		}

		public static CommandArgument ReceivePack(string receivePack)
		{
			return new CommandArgument("--receive-pack", receivePack, '=');
		}

		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		public PushCommand()
			: base("push")
		{
		}

		public PushCommand(params CommandArgument[] args)
			: base("push", args)
		{
		}

		public PushCommand(IList<CommandArgument> args)
			: base("push", args)
		{
		}
	}
}
