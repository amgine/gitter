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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;

using gitter.Framework;

/// <summary>Update remote refs along with associated objects.</summary>
public sealed class PushCommand : Command
{
	public static PushCommand FormatPushCommand(string remote, Many<string> branches, bool force, bool thin, bool tags)
	{
		var args = new ICommandArgument[branches.Count + 3 + (force?1:0) + (tags?1:0)];
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
		args[arg++] = new CommandParameter(remote);
		foreach(var branch in branches)
		{
			args[arg++] = new CommandParameter(branch);
		}
		return new PushCommand(args);
	}

	public static PushCommand FormatRemoveReferenceCommand(string remote, string reference)
	{
		return new PushCommand(
			Porcelain(),
			new CommandParameter(remote),
			new CommandParameter(":" + reference));
	}

	public static ICommandArgument All()
		=> new CommandFlag("--all");

	public static ICommandArgument Mirror()
		=> new CommandFlag("--mirror");

	public static ICommandArgument Delete()
		=> new CommandFlag("--delete");

	public static ICommandArgument Force()
		=> new CommandFlag("--force");

	public static ICommandArgument Tags()
		=> new CommandFlag("--tags");

	public static ICommandArgument Thin()
		=> new CommandFlag("--thin");

	public static ICommandArgument NoThin()
		=> new CommandFlag("--no-thin");

	public static ICommandArgument Porcelain()
		=> new CommandFlag("--porcelain");

	public static ICommandArgument Progress()
		=> new CommandFlag("--progress");

	public static ICommandArgument SetUpstream()
		=> new CommandFlag("--set-upstream");

	public static ICommandArgument ReceivePack(string receivePack)
		=> new CommandParameterValue("--receive-pack", receivePack, '=');

	public static ICommandArgument Verbose()
		=> CommandFlag.Verbose;

	public static ICommandArgument Quiet()
		=> CommandFlag.Quiet;

	public PushCommand()
		: base("push")
	{
	}

	public PushCommand(params ICommandArgument[] args)
		: base("push", args)
	{
	}

	public PushCommand(IList<ICommandArgument> args)
		: base("push", args)
	{
	}
}
