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

using System.Collections.Generic;
using System.Globalization;

/// <summary>Clone a repository into a new directory.</summary>
/// <remarks>
/// <code>
/// <![CDATA[
/// git clone
///   [--template=<template-directory>]
///   [-l]
///   [-s]
///   [--no-hardlinks]
///   [-q]
///   [-n]
///   [--bare]
///   [--mirror]
///   [-o <name>]
///   [-b <name>]
///   [-u <upload-pack>]
///   [--reference <repository>]
///   [--separate-git-dir <git-dir>]
///   [--depth <depth>]
///   [--[no-]single-branch]
///   [--[no-]tags]
///   [--recurse-submodules[=<pathspec>]]
///   [--[no-]shallow-submodules]
///   [--[no-]remote-submodules]
///   [--jobs <n>]
///   [--sparse]
///   [--[no-]reject-shallow]
///   [--filter=<filter-spec>]
///   [--also-filter-submodules]
///   [--]
///   <repository>
///   [<directory>]
/// ]]>
/// </code>
/// </remarks>
public sealed class CloneCommand : Command
{
	const string CloneCommandName = "clone";

	public static ICommandArgument Template(string template)
		=> new CommandParameterPathValue("--template", template, '=');

	public static ICommandArgument Local { get; }
		= new CommandFlag("-l", "--local");

	public static ICommandArgument Shared { get; }
		= new CommandFlag("-s", "--shared");

	public static ICommandArgument NoHardlinks { get; }
		= new CommandFlag("--no-hardlinks");

	public static ICommandArgument Reference(string path)
		=> new CommandParameterPathValue("--reference", path, ' ');

	public static ICommandArgument ReferenceIfAble(string path)
		=> new CommandParameterPathValue("--reference-if-able", path, ' ');

	public static ICommandArgument Dissociate { get; }
		= new CommandFlag("--dissociate");

	public static ICommandArgument Quiet
		=> CommandFlag.Quiet;

	/// <summary>Display the progressbar, even in case the standard output is not a terminal.</summary>
	public static ICommandArgument Verbose
		=> CommandFlag.Verbose;

	public static ICommandArgument Progress { get; }
		= new CommandFlag("--progress");

	public static ICommandArgument NoCheckout { get; }
		= new CommandFlag("-n", "--no-checkout");

	public static ICommandArgument ServerOption(string option)
		=> new CommandParameterValue("--server-option", option, '=');

	public static ICommandArgument RejectShallow { get; }
		= new CommandFlag("--reject-shallow");

	public static ICommandArgument NoRejectShallow { get; }
		= new CommandFlag("--no-reject-shallow");

	/// <summary>
	/// Make a bare GIT repository. That is, instead of creating directory and placing the administrative files in 
	/// directory/.git, make the directory itself the $GIT_DIR. This obviously implies the -n  because there is nowhere
	/// to check out the working tree. Also the branch heads at the remote are copied directly to corresponding local branch
	/// heads, without mapping them to refs/remotes/origin/. When this option is used, neither remote-tracking branches nor
	/// the related configuration variables are created.
	/// </summary>
	public static ICommandArgument Bare { get; }
		= new CommandFlag("--bare");

	public static ICommandArgument Sparse { get; }
		= new CommandFlag("--sparse");

	public static ICommandArgument Filter(string filter)
		=> new CommandParameterValue("--filter", filter, '=');

	public static ICommandArgument AlsoFilterSubmodules { get; }
		= new CommandFlag("--also-filter-submodules");

	/// <summary>Set up a mirror of the remote repository. This implies --bare.</summary>
	public static ICommandArgument Mirror { get; }
		= new CommandFlag("--mirror");

	/// <summary>Instead of using the remote name origin to keep track of the upstream repository, use <paramref name="name"/>.</summary>
	public static ICommandArgument Origin(string name)
		=> new CommandParameterValue("--origin", name, ' ');

	/// <summary>
	///	Create a shallow clone with a history truncated to the specified number of revisions. A shallow repository has a
	///	number of limitations (you cannot clone or fetch from it, nor push from nor into it), but is adequate if you are
	///	only interested in the recent history of a large project with a long history, and would want to send in fixes as patches.
	/// </summary>
	public static ICommandArgument Depth(int depth)
		=> new CommandParameterIntValue("--depth", depth, ' ');

	public static ICommandArgument Recursive { get; }
		= new CommandFlag("--recursive");

	public static ICommandArgument NoMoreOptions
		=> CommandFlag.NoMoreOptions;

	public CloneCommand()
		: base(CloneCommandName)
	{
	}

	public CloneCommand(params ICommandArgument[] args)
		: base(CloneCommandName, args)
	{
	}

	public CloneCommand(IList<ICommandArgument> args)
		: base(CloneCommandName, args)
	{
	}
}
